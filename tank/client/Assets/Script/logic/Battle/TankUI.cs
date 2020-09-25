using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TankUI : MonoBehaviour {
    public Text nameText;
    public Transform bloodBar;
    public Transform bloodBarShadow;
    public int maxWidth = 160;
    public float maxHp = 100;
    public float curHp = 100;
    public void Init(string name,float hp)
    {
        nameText = transform.Find("nameTemplate").GetComponent<Text>();
        bloodBar = transform.Find("BloodBar");
        bloodBarShadow = transform.Find("BloodBarShadow");
        nameText.text = name;
        UpdateHp(hp);
    }
    public void UpdateHp(float hp)
    {
        curHp = hp;
        bloodBar.GetComponent<RectTransform>().sizeDelta = new Vector2(curHp / maxHp * maxWidth, 10);
        DOTween.To(() => bloodBarShadow.GetComponent<RectTransform>().sizeDelta.x, x => { bloodBarShadow.GetComponent<RectTransform>().sizeDelta = new Vector2(x, 10); },curHp / maxHp * maxWidth, 0.5f);

    }
}
