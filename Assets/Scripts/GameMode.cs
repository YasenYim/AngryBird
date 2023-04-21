using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public static GameMode Instance { get; private set; }  // GameMode单例
    public Transform bird;
    PlayerBird playerBird;    // bird身上的脚本组件
    public Transform center;  // 树杈中间的位置
    public float maxDistance = 3; // 小鸟和中心点的最大距离
    public float maxForce = 600;  // 拉满时最大的力
    [Range(0,10)]
    public int playerLives = 3; // 尝试次数，小鸟几条命
    public int enemyNum = 3;  // 剩余敌人数量
    LineRenderer[] lines;     // LineRenderer是一个数组，有两条
    FollowCam cam;            // 获取到FollowCam的组件
    public Transform prefabPoint;
    Transform[] points;
    bool isBirdFlying = false;// 游戏状态：小鸟已经飞出，或者还在弹射操作阶段
    Animator birdAnim;
    [HideInInspector]
    public bool isBirdDead = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        birdAnim = bird.GetComponent<Animator>();
        playerBird = bird.GetComponent<PlayerBird>();
        bird.position = center.position;               // 一开始鸟的位置就处于树杈中间
        Rigidbody2D rigid = bird.GetComponent<Rigidbody2D>();
        rigid.isKinematic = true;    // 动力学刚体为true，小鸟不受力影响
        lines = center.parent.GetComponentsInChildren<LineRenderer>();
        lines[0].SetPosition(1, bird.position);     // 1指的是皮筋的终点那一头
        lines[1].SetPosition(1, bird.position);     // 第二条皮筋的终点是鸟的位置，0是起点，1是终点
        cam = Camera.main.GetComponent<FollowCam>(); // Camera.main代表就是主摄像机，因为FollowCam是挂载在摄像机上面的
        // 统计敌人数量
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyNum = enemies.Length;
        // 绘制抛物线的点
        points = new Transform[20];
        for (int i=0;i<points.Length;i++)
        {
            points[i] = Instantiate(prefabPoint);
            points[i].gameObject.SetActive(false);
        }
    }

    // 开始拖拽函数
    public void BeginDrag()
    {
        if (isBirdFlying){return;}
        // 显示所有的辅助点
        ShowPoints(true);
    }
    // 持续拖拽函数
    public void Drag(Vector3 mousePos)   // 传入的是鼠标的位置
    {
        if (isBirdFlying) { return; }
        // 屏幕坐标转世界坐标
        Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
        pos.z = 0;             // 鸟在摄像机之前不显示，在摄像机之后显示，强行改成0
        // 小鸟离中心点的向量
        Vector3 dist = pos - center.position;
        if (dist.magnitude > maxDistance) { dist = dist.normalized * maxDistance; }      // v.normalized的长度为1，如果拖拽的距离大于最大距离，那么最大距离就只能取到最大距离3
        bird.position = center.position + dist;     // 小鸟当前的位置
        // 设置皮筋
        lines[0].SetPosition(1, bird.position);     // 1指的是皮筋的第二头
        lines[1].SetPosition(1, bird.position);     // 第二条皮筋的终点是鸟的位置，0是起点，1是终点
        // 绘制辅助线
        Rigidbody2D rigid = bird.GetComponent<Rigidbody2D>(); // 获得刚体
        float f = dist.magnitude / maxDistance * maxForce;
        float V0Magnitude = f * Time.fixedDeltaTime / rigid.mass;  // 初速度大小（V0 = F*0.02s）/m 小鸟V0的大小
        Vector2 v0 = -dist.normalized * V0Magnitude;  // 初速度方向
        // 过了时间t，小鸟在哪？
        // p = bird.position + (v0 * t + 0.5f * Physics2D.gravity * t * t);
        float t = 0;
        for (int i = 0; i < points.Length; i++)
        {
            t += 0.2f; // 时间每次间隔0.2f
            Vector2 p = (Vector2)bird.position + (v0 * t + 0.5f * Physics2D.gravity * t * t);
            points[i].position = p;         // 把每一个点的位置传给数组
        }
    }

    // 结束拖拽函数
    public void EndDrag()
    {
        if (isBirdFlying) { return; }
        // 判断小鸟和中心点距离，准备弹出
        Vector3 dist = bird.position - center.position;
        // 如果拖出来的距离太小，忽略
        if (dist.magnitude < 0.01f) { return; }
        // 施加力，弹出小鸟
        Rigidbody2D rigid = bird.GetComponent<Rigidbody2D>();
        rigid.isKinematic = false;    // start的时候动力学刚体设置为true，小鸟不受到影响，弹出的时候设置为false
        float power = dist.magnitude / maxDistance * maxForce;  // 比例乘上拉满时最大的力为当前的拉力
        rigid.AddForce(power * -dist.normalized);               // 风别代表向量的大小以及方向
        playerBird.StartFly();  // 小鸟开始飞
        cam.isFollow = true;
        isBirdFlying = true;
        // 隐藏所有的辅助点
        ShowPoints(false);
        // 皮筋归位
        lines[0].SetPosition(1, center.position);    
        lines[1].SetPosition(1, center.position);

        if (bird.position.x > 25 || bird.position.x < -25 || bird.position.y > 30 || bird.position.y < -30)
        {Invoke("Die", 2.5f); isBirdDead = true;}
    }
    void Die()
    { Instance.OnPlayerBirdDie(); }

    public void OnPlayerBirdDie()
    {
        playerLives -= 1;
        if (playerLives > 0)
        {
            bird.position = center.position;  
            isBirdFlying = false;
            cam.isFollow = false;   
            cam.Follow();          
            playerBird.ResetBird(); 
            birdAnim.SetTrigger("birdNormal");
        }
        else
        {
            Invoke("DelayGameOver", 2);
        }
       
    }
   
    void DelayGameOver()
    {
        if (enemyNum <= 0)
        {
            return;
        }
        Debug.Log("机会用完了，游戏结束");
    }

    public void ShowPoints(bool visible)
    {
        foreach ( var p in points )
        {
            p.gameObject.SetActive(visible);
        }
    }

    public void OnPigDie()
    {
        enemyNum--;
        if (enemyNum <= 0)
        {
            // 游戏成功
            Debug.Log("游戏成功完成!");
        }
    }

}
