using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModulesJson
{
    public int moduleID;
    public int groupID;
    public string name;
    public string language;
    public string complexity;

    public static ModulesJson createFromJson(string jsonString)
    {
        return JsonUtility.FromJson<ModulesJson>(jsonString);
    }
}
