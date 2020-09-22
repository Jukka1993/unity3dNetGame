using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed = 100f;
    public BaseTank tank;
    private GameObject skin;
    private float lifeTime = 30f;
    Rigidbody rigidBody;
    public void Init()
    {
        GameObject skinRes = ResManager.LoadPrefab("Prefabs/ModelPre/Bullet");
        skin = (GameObject)Instantiate(skinRes);
        skin.transform.parent = this.transform;
        skin.transform.localPosition = Vector3.zero;
        skin.transform.localEulerAngles = Vector3.zero;

        rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.useGravity = false;
    }
    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if(lifeTime < 0)
        {
            GameObject explode = ResManager.LoadPrefab("Particles/fire");
            Instantiate(explode, transform.position, transform.rotation);
            Destroy(gameObject);
            return;
        }
        transform.position += transform.forward * speed * Time.deltaTime;
    }
    private void OnCollisionEnter(Collision collisionInfo)
    {
        GameObject collObj = collisionInfo.gameObject;
        BaseTank hitTank = collObj.GetComponent<BaseTank>();
        if (hitTank == tank)
        {
            return;
        }
        if (hitTank != null)
        {
            //hitTank.Attacked(35);
            SendMsgHit(tank, hitTank);
        }
        GameObject explode = ResManager.LoadPrefab("Particles/fire");
        Instantiate(explode, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    private void SendMsgHit(BaseTank tank, BaseTank hitTank)
    {
        if (hitTank == null || tank == null)
        {
            return;
        }
        if(tank.id != GameMain.id)
        {
            return;
        }
        MsgHit msg = new MsgHit();
        msg.targetId = hitTank.id;
        msg.id = GameMain.id;
        msg.x = transform.position.x;
        msg.y = transform.position.y;
        msg.z = transform.position.z;
        NetManager.Send(msg);
    }
}
