using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))    // ���������ʼ��ק
        {
            GameMode.Instance.BeginDrag(); // ��ʼ��ק
        }
        else
        {
            if (Input.GetMouseButton(0))   // ������ק   https://blog.csdn.net/yichang666/article/details/72764878/
            {
                GameMode.Instance.Drag(Input.mousePosition); // ��굱ǰ�����괫���ȥ
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            GameMode.Instance.EndDrag();   // ������ק
        }
    }
}

