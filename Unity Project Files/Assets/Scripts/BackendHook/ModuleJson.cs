using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModuleJson
{
    public QuestionJson[] questionJsons;
    public int moduleID;

    public static ModuleJson createFromJson(string jsonString)
    {
        return JsonUtility.FromJson<ModuleJson>(jsonString);
    }
}
