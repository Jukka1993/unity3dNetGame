﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseTank : MonoBehaviour {
    //坦克模型
    private GameObject skin;
    //转向速度
    public float steer = 30f;
    //移动速度
    public float speed = 30f;
    //炮塔旋转速度
    public float turretSpeed = 30f;
    public string tankName = "";
    //炮塔
    public Transform turret;
    //炮管
    public Transform gun;
    //发射点
    public Transform firePoint;
    public TankUIPanel tankUIPanel;
    public TankUI tankUI;
    public float fireCd = 0.5f;
    public float lastFireTime = 0;
    public float _hp = 100;
    public float hp
    {
        get
        {
            return _hp;
        }
        set
        {
            if (tankUI != null)
            {
                tankUI.UpdateHp(value);
            }
            _hp = value;
        }
    }
    public int id = -1;
    public int camp = 0;
    protected int nameUp = 10;
    private void Start()
    {
        
    }
    protected Rigidbody rigidBody;
    public void Update()
    {
        
    }
    public void UIUpdate()
    {
        if(tankUIPanel == null)
        {
            tankUIPanel = PanelManager.GetPanel<TankUIPanel>();
        }
        if (tankUIPanel == null)
        {
            return;
        }
        if (tankUI == null)
        {
            tankUI = tankUIPanel.GenerateTankUI(tankName, hp);
        }
        if (tankUI == null)
        {
            return;
        }
        Vector2 pos;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * nameUp);
        Vector3 posInCamera = Camera.main.worldToCameraMatrix.MultiplyPoint(transform.position);
        if(posInCamera.z < 0 && RectTransformUtility.ScreenPointToLocalPointInRectangle(tankUIPanel.skin.transform.Find("Container").transform as RectTransform, screenPos, null, out pos))
        {
            tankUI.gameObject.SetActive(true);
            tankUI.transform.localPosition = pos;
        } else
        {
            tankUI.gameObject.SetActive(false);
        }
        //}

    }
    public bool IsDie()
    {
        return hp <= 0;
    }
    public void Attacked(float att)
    {
        if (IsDie())
        {
            return;
        }
        hp -= att;
        if (IsDie())
        {
            GameObject obj = ResManager.LoadPrefab("Particles/explosion");
            GameObject explosion = Instantiate(obj, transform.position, transform.rotation);
            explosion.transform.SetParent(transform);
        }
    }
    public Bullet Fire()
    {
        if (IsDie())
        {
            return null;
        }
        GameObject bulletObj = new GameObject("bullet");
        Bullet bullet = bulletObj.AddComponent<Bullet>();
        bullet.Init();
        bullet.tank = this;
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        lastFireTime = Time.time;
        return bullet;
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
