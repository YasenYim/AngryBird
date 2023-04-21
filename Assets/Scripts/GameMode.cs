using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public static GameMode Instance { get; private set; }  // GameMode����
    public Transform bird;
    PlayerBird playerBird;    // bird���ϵĽű����
    public Transform center;  // ����м��λ��
    public float maxDistance = 3; // С������ĵ��������
    public float maxForce = 600;  // ����ʱ������
    [Range(0,10)]
    public int playerLives = 3; // ���Դ�����С������
    public int enemyNum = 3;  // ʣ���������
    LineRenderer[] lines;     // LineRenderer��һ�����飬������
    FollowCam cam;            // ��ȡ��FollowCam�����
    public Transform prefabPoint;
    Transform[] points;
    bool isBirdFlying = false;// ��Ϸ״̬��С���Ѿ��ɳ������߻��ڵ�������׶�
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
        bird.position = center.position;               // һ��ʼ���λ�þʹ�������м�
        Rigidbody2D rigid = bird.GetComponent<Rigidbody2D>();
        rigid.isKinematic = true;    // ����ѧ����Ϊtrue��С������Ӱ��
        lines = center.parent.GetComponentsInChildren<LineRenderer>();
        lines[0].SetPosition(1, bird.position);     // 1ָ����Ƥ����յ���һͷ
        lines[1].SetPosition(1, bird.position);     // �ڶ���Ƥ����յ������λ�ã�0����㣬1���յ�
        cam = Camera.main.GetComponent<FollowCam>(); // Camera.main������������������ΪFollowCam�ǹ���������������
        // ͳ�Ƶ�������
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyNum = enemies.Length;
        // ���������ߵĵ�
        points = new Transform[20];
        for (int i=0;i<points.Length;i++)
        {
            points[i] = Instantiate(prefabPoint);
            points[i].gameObject.SetActive(false);
        }
    }

    // ��ʼ��ק����
    public void BeginDrag()
    {
        if (isBirdFlying){return;}
        // ��ʾ���еĸ�����
        ShowPoints(true);
    }
    // ������ק����
    public void Drag(Vector3 mousePos)   // �����������λ��
    {
        if (isBirdFlying) { return; }
        // ��Ļ����ת��������
        Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
        pos.z = 0;             // ���������֮ǰ����ʾ���������֮����ʾ��ǿ�иĳ�0
        // С�������ĵ������
        Vector3 dist = pos - center.position;
        if (dist.magnitude > maxDistance) { dist = dist.normalized * maxDistance; }      // v.normalized�ĳ���Ϊ1�������ק�ľ�����������룬��ô�������ֻ��ȡ��������3
        bird.position = center.position + dist;     // С��ǰ��λ��
        // ����Ƥ��
        lines[0].SetPosition(1, bird.position);     // 1ָ����Ƥ��ĵڶ�ͷ
        lines[1].SetPosition(1, bird.position);     // �ڶ���Ƥ����յ������λ�ã�0����㣬1���յ�
        // ���Ƹ�����
        Rigidbody2D rigid = bird.GetComponent<Rigidbody2D>(); // ��ø���
        float f = dist.magnitude / maxDistance * maxForce;
        float V0Magnitude = f * Time.fixedDeltaTime / rigid.mass;  // ���ٶȴ�С��V0 = F*0.02s��/m С��V0�Ĵ�С
        Vector2 v0 = -dist.normalized * V0Magnitude;  // ���ٶȷ���
        // ����ʱ��t��С�����ģ�
        // p = bird.position + (v0 * t + 0.5f * Physics2D.gravity * t * t);
        float t = 0;
        for (int i = 0; i < points.Length; i++)
        {
            t += 0.2f; // ʱ��ÿ�μ��0.2f
            Vector2 p = (Vector2)bird.position + (v0 * t + 0.5f * Physics2D.gravity * t * t);
            points[i].position = p;         // ��ÿһ�����λ�ô�������
        }
    }

    // ������ק����
    public void EndDrag()
    {
        if (isBirdFlying) { return; }
        // �ж�С������ĵ���룬׼������
        Vector3 dist = bird.position - center.position;
        // ����ϳ����ľ���̫С������
        if (dist.magnitude < 0.01f) { return; }
        // ʩ����������С��
        Rigidbody2D rigid = bird.GetComponent<Rigidbody2D>();
        rigid.isKinematic = false;    // start��ʱ����ѧ��������Ϊtrue��С���ܵ�Ӱ�죬������ʱ������Ϊfalse
        float power = dist.magnitude / maxDistance * maxForce;  // ������������ʱ������Ϊ��ǰ������
        rigid.AddForce(power * -dist.normalized);               // �����������Ĵ�С�Լ�����
        playerBird.StartFly();  // С��ʼ��
        cam.isFollow = true;
        isBirdFlying = true;
        // �������еĸ�����
        ShowPoints(false);
        // Ƥ���λ
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
        Debug.Log("���������ˣ���Ϸ����");
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
            // ��Ϸ�ɹ�
            Debug.Log("��Ϸ�ɹ����!");
        }
    }

}
