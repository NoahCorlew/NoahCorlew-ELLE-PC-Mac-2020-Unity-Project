using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionJson
{
    public int questionID;
    public string audioLocation;
    public string imageLocation;
    public string type;
    public string questionText;
    public int[] answerIDList = new int[50];
    public AnswerJson[] answers = new AnswerJson[50];

    public static QuestionJson createFromJson(string jsonString)
    {
        return JsonUtility.FromJson<QuestionJson>(jsonString);
    }

    public string createJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}
