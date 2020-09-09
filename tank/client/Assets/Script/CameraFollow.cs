using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    //距离矢量
    public Vector3 distance = new Vector3(0, 8, -18);
    //相机
    public Camera mainCamera;
    //偏移值
    public Vector3 offset = new Vector3(0, 5f, 0);
    //相机移动速度
    public float speed = 15f;
    private void Start()
    {
        //设置mainCamera为主相机
        mainCamera = Camera.main;
        //相机初始位置
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        Vector3 initPos = pos - 30 * forward + Vector3.up * 10;
        mainCamera.transform.position = initPos;
    }
    //所有组件update之后发生
    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        Vector3 targetPos = pos;
        targetPos = pos + forward * distance.z;
        targetPos.y += distance.y;
        Vector3 cameraPos = mainCamera.transform.position;
        cameraPos = Vector3.MoveTowards(cameraPos, targetPos, Time.deltaTime * speed);
        mainCamera.transform.position = cameraPos;
        mainCamera.transform.LookAt(pos + offset);

    }
}
