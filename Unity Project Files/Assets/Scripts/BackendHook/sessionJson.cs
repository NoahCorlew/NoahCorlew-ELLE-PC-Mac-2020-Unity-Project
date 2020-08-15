using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class sessionJson 
{
    public string sessionID;

    public static sessionJson createFromJson(string jsonString)
    {
        return JsonUtility.FromJson<sessionJson>(jsonString);
    }

    public string getSessionID()
    {
        return sessionID;
    }

}
