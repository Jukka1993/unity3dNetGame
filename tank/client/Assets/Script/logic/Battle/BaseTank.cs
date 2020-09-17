using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTank : MonoBehaviour {
    //坦克模型
    private GameObject skin;
    //转向速度
    public float steer = 20f;
    //移动速度
    public float speed = 30f;
    //炮塔旋转速度
    public float turretSpeed = 30f;

    //炮塔
    public Transform turret;
    //炮管
    public Transform gun;
    //发射点
    public Transform firePoint;
    public int hp = 100;
    public int id = -1;
    public int camp = 0;
    private void Start()
    {
        
    }
    protected Rigidbody rigidBody;
    public void Update()
    {
        
    }

    public virtual void Init(string skinPath)
    {
        //皮肤
        GameObject skinRes = ResManager.LoadPrefab(skinPath);
        skin = (GameObject)Instantiate(skinRes);
        skin.transform.SetParent(this.transform);
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;
        //物理
        rigidBody = gameObject.AddComponent<Rigidbody>();
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = new Vector3(0, 2.5f, 1.47f);
        boxCollider.size = new Vector3(7, 5, 12);
        //炮塔炮管
        turret = skin.transform.Find("Turret");
        gun = turret.transform.Find("Gun");
        firePoint = gun.transform.Find("FirePoint");
    }
}
