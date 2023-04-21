using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;  // 跟随的目标，这里的target是我们的PlayerBird
    Vector2 offset;
    [HideInInspector]
    public bool isFollow = false;
    public Transform limit;   // limit作为参考的限制范围的物体
    Rect limitRect;           //Rect代表矩形的结构体

    void Start()
    {
        offset =  transform.position - target.position;   // 偏移量 = 当前摄像机的位置 - 目标的位置
        // 计算限制范围的Rect
        limitRect.size = limit.localScale;   // 大小就是白色半透明的localScale
        limitRect.center = limit.position;   // 中心点就是limit的position
        // 打印4个边界值
        Debug.Log($"rect:{limitRect.xMin} {limitRect.xMax},{limitRect.yMin} {limitRect.yMax}");
    }

    void Update()
    {if (isFollow) { Follow(); }}   // 跟随的时候每一帧都跟随

    public void Follow()
    {
        Vector3 pos = target.position + new Vector3(offset.x, offset.y, 0);
        pos.z = -10;
        // 和下面限定作用一致
        //if (v.x < limitRect.xMin) { v.x = limitRect.xMin; }
        //if (v.x > limitRect.xMax) { v.x = limitRect.xMax; }
        //if (v.y < limitRect.yMin) { v.y = limitRect.yMin; }
        //if (v.y > limitRect.yMax) { v.y = limitRect.yMax; }
        // 限定v.x和v.y在limitRect定义的范围内
        pos.x = Mathf.Clamp(pos.x, limitRect.xMin, limitRect.xMax);
        pos.y = Mathf.Clamp(pos.y, limitRect.yMin, limitRect.yMax);
        transform.position = pos;
    }
}
