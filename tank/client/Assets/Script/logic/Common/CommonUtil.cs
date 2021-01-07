using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonUtil {

	public static void OpenTip(string tipStr, TipPanel.ConfirmDelegate confirmDelegate = null)
    {
        PanelManager.Open<TipPanel>(tipStr, confirmDelegate);
    }
    public static double GetTimeStamp()
    {
        //return Time.
        //TimeSpan tss = new TimeSpan(System.DateTime.Now.Ticks);
        //Debug.Log("HERE");
        //Debug.Log(tss.TotalMilliseconds);
        double temp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        //Debug.Log(temp);
        return temp;;
        //return (float)tss.TotalMilliseconds;
        //DateTime dateStart = new DateTime(1970, 1, 1, 8, 0, 0);
        //    int timeStamp = Convert.ToInt32((dt - dateStart).TotalSeconds);
        //    return timeStamp;
    }
}
