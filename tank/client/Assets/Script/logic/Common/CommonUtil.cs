using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonUtil {

	public static void OpenTip(string tipStr)
    {
        PanelManager.Open<TipPanel>(tipStr);
    }
}
