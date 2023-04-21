using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    public float maxImpulse = 2;  // �������
    Animator pigAnim;

    private void Start()
    {
        pigAnim = GetComponent<Animator>();
    }

    // ��ײ�Ժ�collision��ȡ��ײ����Ϣ
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �ж�ײ����
        float impulse  = 0;
        for (int i = 0; i < collision.contactCount;i++)
        {
            var touch = collision.contacts[i];
            if (touch.normalImpulse > impulse)  // ��������ƽ���ڵ�����������������Ǵ�ֱ�ڵ����������ֱ�ڵ������̫����ô�ͻ���¹ʡ�����������С�����ѵ����Ƿ�������
            {
                impulse = touch.normalImpulse;  // collision�洢�˺ܶ�������㣬���ѷ�����������impulse
            }
        }
        if (impulse > maxImpulse)
        {
            Debug.Log($"���壺{gameObject.name}�������С�ǣ�{impulse}");
            pigAnim.SetTrigger("Die");  // ���ű�ը����
            Debug.Log("������С��ը������");
            Invoke("Die", 0.5f);  // �ӳ�0.5s������
        }

        

    }

    // �ӳ�0.5s���������ĺ���
    void Die()
    {
        GameMode.Instance.OnPigDie();
        Destroy(gameObject);
    }


}
