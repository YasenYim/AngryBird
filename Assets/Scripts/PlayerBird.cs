using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBird : MonoBehaviour
{
    public bool isBirdDead = false;
    Rigidbody2D rigid;
    float startFlyTime=0;
    [HideInInspector]
    public Animator birdAnim;
    private float protectedTime = 0.1f;   // 在0.1s的时间内受到保护

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        birdAnim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isBirdDead)
        {
            Debug.Log("小鸟碰到了" + collision.collider.name);// 测试小鸟碰撞到的东西是什么，是地面，小鸟从动力学到普通刚体出去的一瞬间
            if (startFlyTime + protectedTime < Time.time)        
            {
                birdAnim.SetTrigger("birdHit");  // 如果拖拽的时候就直接碰到地面，还没有碰到地面外的其他物体的时候就已经触发了动画。
                Invoke("Die", 2.5f);
                isBirdDead = true;
            }
        }
    }


    // 小鸟死亡函数
    void Die()
    {GameMode.Instance.OnPlayerBirdDie();}

    // 开始计时
    public void StartFly()
    {
        startFlyTime = Time.time;
    }

    // 飞行后小鸟位置重置函数
    public void ResetBird()
    {
        rigid.isKinematic = true;  // true代表归位以后刚体关闭，开启动力学，表示不受力的作用
        transform.rotation = Quaternion.identity; // 小鸟旋转归0  //rigid.rotation = 0;   // 作用和左边等价
        rigid.velocity = Vector2.zero;  // 小鸟速度归0
        rigid.angularVelocity = 0;      // 角速度归0
        isBirdDead = false;// 死亡归0
    }
}
