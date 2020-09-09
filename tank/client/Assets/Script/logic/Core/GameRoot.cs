using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoBehaviour {
    private static GameRoot _instance;
    public GameRoot Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new GameRoot();
            }
            return _instance;
        }
    }
}
