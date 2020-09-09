using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //GameObject skinRes = ResManager.LoadPrefab("TankPrefab/tankPrefab");
        //GameObject skin = (GameObject)Instantiate(skinRes);
        //skin.transform.SetParent(transform);
        //skin.transform.position = Vector3.zero;
        GameObject tankObj = new GameObject("myTank");
        BaseTank baseTank = tankObj.AddComponent<CtrlTank>();
        baseTank.Init("TankPrefab/tankPrefab");
        baseTank.transform.gameObject.AddComponent<CameraFollow>();
	}
	
	
}
