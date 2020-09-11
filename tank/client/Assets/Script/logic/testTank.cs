using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTank : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PanelManager.Init();
        Debug.Log("AA");
        PanelManager.Open<LoginPanel>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
