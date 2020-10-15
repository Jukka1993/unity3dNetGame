using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EButton : Button, IPointerClickHandler, IEventSystemHandler, ISubmitHandler
{
    public bool isPressed = false;
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Debug.Log("OnPointerDown");//按下按钮
        isPressed = true;

    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Debug.Log("OnPointerEnter");//进入按钮范围

    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        Debug.Log("OnPointerExit");//退出按钮范围

    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        Debug.Log("OnPointerUp");//松开按钮
        isPressed = false;


    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        Debug.Log("OnPointerClick");//完成点击

    }
    //base.OnPointerEnter
    //base.OnPointerExit
    //base.OnPointerUp
    //base.OnPointerClick

    //}
}
