using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.Connect("127.0.0.1", 8888);
	}
    public void OnEnter(string msg)
    {
        Debug.Log("OnEnter " + msg);
    }
    public void OnMove(string msg)
    {
        Debug.Log("OnMove " + msg);
    }
    public void OnLeave(string msg) {
        Debug.Log("OnLeave " + msg);
    }

    // Update is called once per frame
    void Update () {
        NetManager.Update();
	}
}
