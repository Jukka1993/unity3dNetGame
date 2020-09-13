using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Assets.Script.proto;

public class RoomListPanel : BasePanel {
    public Text nameText;
    public Text winCountText;
    public Text lostCountText;
    public Button createButton;
    public Button refreshButton;
    public GameObject roomListContent;
    public GameObject roomItem;
    public override void OnInit()
    {
        base.OnInit();
        skinPath = "Prefabs/UIPre/RoomListPanel";
        layer = PanelManager.Layer.Panel;
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        nameText = skin.transform.Find("Info/nameText").GetComponent<Text>();
        winCountText = skin.transform.Find("Info/WinCountText").GetComponent<Text>();
        lostCountText = skin.transform.Find("Info/FailCountText").GetComponent<Text>();
        createButton = skin.transform.Find("Action/CreateRoomButton").GetComponent<Button>();
        refreshButton = skin.transform.Find("Action/RefreshRoomButton").GetComponent<Button>();
        roomListContent = skin.transform.Find("List/Scroll View/Viewport/Content").gameObject;
        roomItem = skin.transform.Find("List/Scroll View/Viewport/RoomItem").gameObject;

        createButton.onClick.AddListener(OnCreateButton);
        refreshButton.onClick.AddListener(OnRefreshButton);

        NetManager.AddMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);

        nameText.text = GameMain.userName;

        MsgGetAchieve msgGetAchieve = new MsgGetAchieve();
        Debug.Log("send msgGetAchieve");
        NetManager.Send(msgGetAchieve);
        MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
        NetManager.Send(msgGetRoomList);
    }
    public override void OnClose()
    {
        base.OnClose();
        NetManager.RemoveMsgListener("MsgGetAchieve", OnMsgGetAchieve);
        NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
    }
    private void OnMsgEnterRoom(MsgBase msgBase)
    {
        MsgEnterRoom msg = (MsgEnterRoom)msgBase;
        if(msg.result == 0)
        {
            PanelManager.Open<RoomPanel>();
            Close();
        } else
        {
            CommonUtil.OpenTip("进入房间失败");
        }

    }
    private void OnMsgCreateRoom(MsgBase msgBase)
    {
        MsgCreateRoom msg = (MsgCreateRoom)msgBase;
        //创建房间成功;
        if(msg.roomId > 0)
        {
            CommonUtil.OpenTip("创建成功");
            PanelManager.Open<RoomPanel>();
            Close();
        } else
        {
            CommonUtil.OpenTip("创建失败");
        }

    }

    private void OnMsgGetRoomList(MsgBase msgBase)
    {
        Debug.Log("====== OnMsgGetRoomList");
        MsgGetRoomList msg = (MsgGetRoomList)msgBase;
        for (int i = roomListContent.transform.childCount - 1; i >= 0; i--)
        {
            GameObject o = roomListContent.transform.GetChild(i).gameObject;
            Destroy(o);
        }
        if(msg.roomList == null || msg.roomList.Length == 0)
        {
            return;
        }
        for (int i = 0; i < msg.roomList.Length; i++)
        {
            GenerateRoom(msg.roomList[i]);
        }
    }
    private void GenerateRoom(RoomInfo roomInfo)
    {
        GameObject o = Instantiate(roomItem);
        o.transform.SetParent(roomListContent.transform);
        o.SetActive(true);
        o.transform.localScale = Vector3.one;

        Transform transform = o.transform;
        Text indexText = transform.Find("IndexText").GetComponent<Text>();
        Text populationText = transform.Find("PopulationText").GetComponent<Text>();
        Text stateText = transform.Find("StateText").GetComponent<Text>();
        Button enterButton = transform.Find("EnterButton").GetComponent<Button>();

        indexText.text = roomInfo.id.ToString();
        populationText.text = roomInfo.population.ToString();
        stateText.text = roomInfo.status == 0 ? "准备中" : "战斗中";
        enterButton.onClick.AddListener(delegate() { OnEnterClick(roomInfo.id); });
    }
    private void OnEnterClick(int roomId)
    {
        MsgEnterRoom msg = new MsgEnterRoom();
        msg.id = roomId;
        NetManager.Send(msg);
    }
    private void OnMsgGetAchieve(MsgBase msgBase)
    {
        MsgGetAchieve msg = (MsgGetAchieve)msgBase;
        Debug.Log("receive OnMsgGetAchieve " +  msg.winCount + " " + msg.failCount);
        winCountText.text = msg.winCount.ToString();
        lostCountText.text = msg.failCount.ToString();
    }
    private void OnCreateButton()
    {
        Debug.Log("OnCreateButton");
        MsgCreateRoom msg = new MsgCreateRoom();
        NetManager.Send(msg);
    }
    private void OnRefreshButton()
    {
        Debug.Log("OnRefreshButton = ");
        MsgGetRoomList msg = new MsgGetRoomList();
        NetManager.Send(msg);
        //MsgLogin msgLogin = new MsgLogin();
        //msgLogin.name = "jjj";
        //msgLogin.pw = "kkk";
        //NetManager.Send(msgLogin);
    }
}
