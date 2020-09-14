using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Assets.Script.proto;

public class RoomPanel : BasePanel {
    public Transform playerContent;
    public GameObject playerItem;
    public Button startButton;
    public Button exitButton;
    public override void OnInit()
    {
        base.OnInit();
        skinPath = "Prefabs/UIPre/RoomPanel";
        layer = PanelManager.Layer.Panel;

    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        playerContent = skin.transform.Find("PlayerList/Scroll View/Viewport/Content");
        playerItem = skin.transform.Find("PlayerList/Scroll View/Viewport/PlayerItem").gameObject;
        startButton = skin.transform.Find("ActionArea/StartButton").GetComponent<Button>();
        exitButton = skin.transform.Find("ActionArea/ExitButton").GetComponent<Button>();


        startButton.onClick.AddListener(OnStartButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);

        NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgStartBattle", OnMsgStartBattle);

        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        NetManager.Send(msg);
    }
    public override void OnClose()
    {
        base.OnClose();
        NetManager.RemoveMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.RemoveMsgListener("MsgStartBattle", OnMsgStartBattle);
    }
    private void OnMsgStartBattle(MsgBase msgBase)
    {
        MsgStartBattle msg = (MsgStartBattle)msgBase;
        if(msg.result == 0)
        {
            Close();
        } else
        {
            CommonUtil.OpenTip("开战失败！两队至少都需要一名玩家，只有队长可以开始战斗!");
        }
    }
    private void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
        if(msg.result == 0)
        {
            CommonUtil.OpenTip("退出房间");
            PanelManager.Open<RoomListPanel>();
            Close();
        } else
        {
            CommonUtil.OpenTip("退出房间失败");
        }
    }
    private void OnMsgGetRoomInfo(MsgBase msgBase)
    {
        MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
        //msg.players
        for(int i = playerContent.childCount-1;i>= 0; i--)
        {
            GameObject o = playerContent.GetChild(i).gameObject;
            Destroy(o);
        }
        if(msg.players == null || msg.players.Length == 0)
        {
            return;
        }
        for(int i = 0; i < msg.players.Length; i++)
        {
            GeneratePlayer(msg.players[i]);
        }

    }
    private void GeneratePlayer(PlayerInfo playerInfo)
    {
        GameObject player = Instantiate(playerItem);
        player.transform.SetParent(playerContent);
        player.SetActive(true);
        Text nameText = player.transform.Find("NameText").GetComponent<Text>();
        Text groupText = player.transform.Find("GroupText").GetComponent<Text>();
        Text achieveText = player.transform.Find("AchieveText").GetComponent<Text>();
        Button kickButton = player.transform.Find("Button").GetComponent<Button>();

        nameText.text = playerInfo.name;
        groupText.text = playerInfo.camp == 0 ? "红方" : "蓝方";
        achieveText.text = playerInfo.winCount + "胜" + playerInfo.lostCount + "负";
        kickButton.onClick.AddListener(delegate () { OnKickButtonClick(playerInfo.id); });

    }
    private void OnKickButtonClick(int playerId)
    {
        Debug.Log("请" + playerId + "出去");
    }
    private void OnStartButtonClicked()
    {
        Debug.Log("OnStartButtonClicked");
        MsgStartBattle msg = new MsgStartBattle();
        NetManager.Send(msg);
    }
    private void OnExitButtonClicked()
    {
        Debug.Log("OnExitButtonClicked");
        MsgLeaveRoom msg = new MsgLeaveRoom();
        NetManager.Send(msg);
    }
}
