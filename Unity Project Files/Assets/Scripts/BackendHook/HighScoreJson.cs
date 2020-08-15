using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[System.Serializable]
public class HighScoreJson
{
    public int score;
    public string usernames;

    public static AnswerJson createFromJson(string jsonString)
    {
        return JsonUtility.FromJson<AnswerJson>(jsonString);
    }

    public string createJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}
