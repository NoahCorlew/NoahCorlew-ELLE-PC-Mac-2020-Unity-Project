using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;


[Serializable]
public class ModuleInfo
{
    public int ID;
    public string name;
    public int pb;
    public int wr;
    public int questionNum;
    public int correctQuestionAnswered;
    public int wrongQuestionAnswered;
    public float timeSpent;


    // Not used: questionNum, wr
    public ModuleInfo(int ID, string name, int pb, int wr, int questionNum, int correctQuestionAnswered, int wrongQuestionAnswered, float timeSpent)
    {
        this.ID = ID;
        this.name = name;
        this.pb = pb;
        this.wr = wr;
        this.questionNum = questionNum;
        this.correctQuestionAnswered = correctQuestionAnswered;
        this.wrongQuestionAnswered = wrongQuestionAnswered;
        this.timeSpent = timeSpent;
    }

    public float getTime()
    {
        return this.timeSpent;
    }

    public void addTime(float time)
    {
        this.timeSpent += time;
    }

    public void addCorrect()
    {
        this.correctQuestionAnswered += 1;
        this.saveJson();
    }

    public void addWrong()
    {
        this.wrongQuestionAnswered += 1;
        this.saveJson();
    }

    public double getPercentCorrect()
    {
        if (this.correctQuestionAnswered >= 1 && this.wrongQuestionAnswered == 0)
            return 100;

        if (this.correctQuestionAnswered == 0 && this.wrongQuestionAnswered == 0)
            return 0;

        else
            return Math.Round((double)this.correctQuestionAnswered / ((double)this.wrongQuestionAnswered + (double)this.correctQuestionAnswered), 3);
    }

    public string getName()
    {
        return this.name;
    }

    public int getID()
    {
        return this.ID;
    }

    public int getTotalQuestionsAnswered()
    {
        return this.correctQuestionAnswered + this.wrongQuestionAnswered;
    }

    public int getPB()
    {
        return this.pb;
    }

    public void setPB(int pb)
    {
        this.pb = pb;
    }

    public string createJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void saveJson()
    {
        string path = Application.persistentDataPath + "/UserRecords/Module" + this.ID + ".json";

        var myFile = File.Create(path);
        myFile.Close();

        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(this.createJson());
        writer.Close();
    }

}
