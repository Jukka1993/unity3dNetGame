using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    public GameObject humanPrefab;
    public BaseHuman myHuman;
    public Dictionary<string, BaseHuman> otherHumans;
    public float time = 0f;
    public bool haveListed = false;
	// Use this for initialization
	void Start () {
        otherHumans = new Dictionary<string, BaseHuman>();
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Move", OnMove);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.AddListener("List", OnList);
        NetManager.Connect("127.0.0.1", 8888);
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        float x = Random.Range(0, 5);
        float z = Random.Range(0, 5);
        obj.transform.position = new Vector3(x, 0, z);
        myHuman = obj.AddComponent<CtrlHuman>();
        myHuman.desc = NetManager.GetDesc();
        Vector3 pos = myHuman.transform.position;
        Vector3 eul = myHuman.transform.eulerAngles;
        string sendStr = "Enter|";
        sendStr += NetManager.GetDesc() + ",";
        sendStr += pos.x + ",";
        sendStr += pos.y + ",";
        sendStr += pos.z + ",";
        sendStr += eul.y;
        NetManager.Send(sendStr);
        NetManager.Send("List|");


    }
    public void OnList(string msg)
    {
        Debug.Log("OnList " + msg);
        string[] split = msg.Split(',');
        for(int i = 0; i < split.Length-3; i+=6)
        {
            string desc = split[i];
            if(desc == NetManager.GetDesc())
            {
                continue;
            }
            float x = float.Parse(split[i + 1]);
            float y = float.Parse(split[i + 2]);
            float z = float.Parse(split[i + 3]);
            float eulY = float.Parse(split[i + 4]);
            int hp = int.Parse(split[i + 5]);
            GameObject obj = (GameObject)Instantiate(humanPrefab);
            obj.transform.position = new Vector3(x, y, z);
            obj.transform.eulerAngles = new Vector3(0, eulY, 0);
            BaseHuman h = obj.AddComponent<SyncHuman>();
            h.desc = desc;
            otherHumans.Add(desc, h);
        }

    }
    public void OnEnter(string msg)
    {
        Debug.Log("OnEnter " + msg);
        string[] split = msg.Split(',');
        string desc = split[0];
        float x = float.Parse(split[1]);
        float y = float.Parse(split[2]);
        float z = float.Parse(split[3]);
        float eulY = float.Parse(split[4]);
        if(desc == NetManager.GetDesc())
        {
            return;
        }
        GameObject obj = (GameObject)Instantiate(humanPrefab);
        obj.transform.position = new Vector3(x, y, z);
        obj.transform.eulerAngles = new Vector3(0, eulY, 0);
        BaseHuman h = obj.AddComponent<SyncHuman>();
        h.desc = desc;
        otherHumans.Add(desc, h);
    }
    public void OnMove(string msg)
    {
        Debug.Log("OnMove " + msg);
        
        string[] split = msg.Split(',');
        string desc = split[0];
        if(desc == NetManager.GetDesc())
        {
            return;
        }
        BaseHuman other;
        otherHumans.TryGetValue(desc, out other);
        if(other != null)
        {
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            other.MoveTo(new Vector3(x, y, z));
        }


    }
    public void OnLeave(string msg) {
        Debug.Log("OnLeave " + msg);
        //Destroy(otherHumans[msg]);
        if (!otherHumans.ContainsKey(msg))
        {
            return;
        }
        BaseHuman h = otherHumans[msg];
        Destroy(h.gameObject);
        otherHumans.Remove(msg);
    }

    // Update is called once per frame
    void Update () {
        //time += Time.deltaTime;
        //if (time > 1 && haveListed == false)
        //{
        //    haveListed = true;
        //    NetManager.Send("List|");
        //}
        NetManager.Update();
	}
}
