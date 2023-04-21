using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))    // 按下左键开始拖拽
        {
            GameMode.Instance.BeginDrag(); // 开始拖拽
        }
        else
        {
            if (Input.GetMouseButton(0))   // 持续拖拽   https://blog.csdn.net/yichang666/article/details/72764878/
            {
                GameMode.Instance.Drag(Input.mousePosition); // 鼠标当前的坐标传入进去
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            GameMode.Instance.EndDrag();   // 结束拖拽
        }
    }
}

