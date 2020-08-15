using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnswerJson
{
    public int termID;
    public string audioLocation;
    public string imageLocation;
    public string front;
    public string back;
    public string type;
    public string gender;
    public string language;

    public static AnswerJson createFromJson(string jsonString)
    {
        return JsonUtility.FromJson<AnswerJson>(jsonString);
    }

    public string createJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}
