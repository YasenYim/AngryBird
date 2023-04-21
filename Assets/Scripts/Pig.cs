using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    public float maxImpulse = 2;  // 最大冲击力
    Animator pigAnim;

    private void Start()
    {
        pigAnim = GetComponent<Animator>();
    }

    // 碰撞以后，collision获取碰撞的信息
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 判断撞击力
        float impulse  = 0;
        for (int i = 0; i < collision.contactCount;i++)
        {
            var touch = collision.contacts[i];
            if (touch.normalImpulse > impulse)  // 切向力是平行于地面的力，而法向力是垂直于地面的力，垂直于地面的力太大那么就会出事故。所以真正让小猪破裂的力是法向力。
            {
                impulse = touch.normalImpulse;  // collision存储了很多的受力点，最后把法向力传给了impulse
            }
        }
        if (impulse > maxImpulse)
        {
            Debug.Log($"物体：{gameObject.name}冲击力大小是：{impulse}");
            pigAnim.SetTrigger("Die");  // 播放爆炸动画
            Debug.Log("播放了小猪爆炸动画。");
            Invoke("Die", 0.5f);  // 延迟0.5s后死亡
        }

        

    }

    // 延迟0.5s调用死亡的函数
    void Die()
    {
        GameMode.Instance.OnPigDie();
        Destroy(gameObject);
    }


}
