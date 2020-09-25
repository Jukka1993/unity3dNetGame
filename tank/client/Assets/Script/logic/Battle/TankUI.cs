using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankUI : MonoBehaviour {
    public Text nameText;
    public Transform bloodBar;
    public int maxWidth = 160;
    public float maxHp = 100;
    public float curHp = 100;
    public void Init(string name,float hp)
    {
        nameText = transform.Find("nameTemplate").GetComponent<Text>();
        bloodBar = transform.Find("BloodBar");
        nameText.text = name;
        UpdateHp(hp);
    }
    public void UpdateHp(float hp)
    {
        curHp = hp;
        bloodBar.GetComponent<RectTransform>().sizeDelta = new Vector2(curHp / maxHp * maxWidth, 10);
    }
}
