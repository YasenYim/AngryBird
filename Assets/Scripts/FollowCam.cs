using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;  // �����Ŀ�꣬�����target�����ǵ�PlayerBird
    Vector2 offset;
    [HideInInspector]
    public bool isFollow = false;
    public Transform limit;   // limit��Ϊ�ο������Ʒ�Χ������
    Rect limitRect;           //Rect������εĽṹ��

    void Start()
    {
        offset =  transform.position - target.position;   // ƫ���� = ��ǰ�������λ�� - Ŀ���λ��
        // �������Ʒ�Χ��Rect
        limitRect.size = limit.localScale;   // ��С���ǰ�ɫ��͸����localScale
        limitRect.center = limit.position;   // ���ĵ����limit��position
        // ��ӡ4���߽�ֵ
        Debug.Log($"rect:{limitRect.xMin} {limitRect.xMax},{limitRect.yMin} {limitRect.yMax}");
    }

    void Update()
    {if (isFollow) { Follow(); }}   // �����ʱ��ÿһ֡������

    public void Follow()
    {
        Vector3 pos = target.position + new Vector3(offset.x, offset.y, 0);
        pos.z = -10;
        // �������޶�����һ��
        //if (v.x < limitRect.xMin) { v.x = limitRect.xMin; }
        //if (v.x > limitRect.xMax) { v.x = limitRect.xMax; }
        //if (v.y < limitRect.yMin) { v.y = limitRect.yMin; }
        //if (v.y > limitRect.yMax) { v.y = limitRect.yMax; }
        // �޶�v.x��v.y��limitRect����ķ�Χ��
        pos.x = Mathf.Clamp(pos.x, limitRect.xMin, limitRect.xMax);
        pos.y = Mathf.Clamp(pos.y, limitRect.yMin, limitRect.yMax);
        transform.position = pos;
    }
}
