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
    private float protectedTime = 0.1f;   // ��0.1s��ʱ�����ܵ�����

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        birdAnim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isBirdDead)
        {
            Debug.Log("С��������" + collision.collider.name);// ����С����ײ���Ķ�����ʲô���ǵ��棬С��Ӷ���ѧ����ͨ�����ȥ��һ˲��
            if (startFlyTime + protectedTime < Time.time)        
            {
                birdAnim.SetTrigger("birdHit");  // �����ק��ʱ���ֱ���������棬��û����������������������ʱ����Ѿ������˶�����
                Invoke("Die", 2.5f);
                isBirdDead = true;
            }
        }
    }


    // С����������
    void Die()
    {GameMode.Instance.OnPlayerBirdDie();}

    // ��ʼ��ʱ
    public void StartFly()
    {
        startFlyTime = Time.time;
    }

    // ���к�С��λ�����ú���
    public void ResetBird()
    {
        rigid.isKinematic = true;  // true�����λ�Ժ����رգ���������ѧ����ʾ������������
        transform.rotation = Quaternion.identity; // С����ת��0  //rigid.rotation = 0;   // ���ú���ߵȼ�
        rigid.velocity = Vector2.zero;  // С���ٶȹ�0
        rigid.angularVelocity = 0;      // ���ٶȹ�0
        isBirdDead = false;// ������0
    }
}
