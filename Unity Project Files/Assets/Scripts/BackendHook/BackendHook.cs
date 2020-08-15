using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Net;
using System;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Linq;

/*
 http://34.239.123.94/Images/_imageLocation_
http://34.239.123.94/Audios/_audioLocation_

*/

public class BackendHook : MonoBehaviour
{
    public static string ipAdress = "https://endlesslearner.com/api";
    //public static string ipAdress = "http://45.55.61.182:5000/api";

    public static string sessionID;
    public static string moduleJsonString;
    bool loginFlag = false;
    bool getModuleFlag = false;
    public static bool modulesFound = false;
    public static bool collectionMade = false;
    public static string loginTokenString;
    public static Dictionary<string, int> avalibleModulesDic = new Dictionary<string, int>();
    public static bool flippyfloppyflag;

    public static Dictionary<int, int> moduleQCount = new Dictionary<int, int>();
    public static Dictionary<int, int> highScores = new Dictionary<int, int>();
    public static Dictionary<int, int> personalHighScores = new Dictionary<int, int>();
    public static List<int> modulesWithImages = new List<int>();

    // Gets the json of the module with a specific ID
    public static IEnumerator getModule(int ID)
    {
        TokenJson token = TokenJson.createFromJson(loginTokenString);
        string url = ipAdress  + "/modulequestions";

        WWWForm form = new WWWForm();
        form.AddField("moduleID", ID);

        UnityWebRequest getModuleRequest = UnityWebRequest.Post(url, form);

        getModuleRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

        yield return getModuleRequest.SendWebRequest();

        moduleJsonString = getModuleRequest.downloadHandler.text;

        Debug.Log(moduleJsonString);

        // Splits the questions and answers out of it, and stores it in the collectionmanager dictionaries
        QuestionJson[] questionJsonObjects = JsonHelper.getJsonArray<QuestionJson>(moduleJsonString);
        AnswerJson[] answerJsonObjects = new AnswerJson[500];

        int i;
        foreach (QuestionJson q in questionJsonObjects)
        {
            // Longform should be the only question type with multiple answers
            if (q.type == "LONGFORM")
            {
                if (!CollectionManager.questionCollection.ContainsKey(q.questionID))
                    CollectionManager.questionCollection.Add(q.questionID, new question(q.questionID, q.answerIDList, q.questionText, null, null));

                i = 0;
                foreach (AnswerJson a in q.answers)
                {
                    q.answerIDList[i] = a.termID;
                    if (!CollectionManager.answerCollection.ContainsKey(a.termID))
                    {
                        CollectionManager.answerCollection.Add(a.termID, new answer(a.termID, a.back, null, a.imageLocation, a.audioLocation));
                    }
                    i++;
                }
            }
            else if (q.type == "MATCH")
            {
                foreach (AnswerJson a in q.answers)
                {
                    q.answerIDList[0] = a.termID;
                    
                    if (!CollectionManager.questionCollection.ContainsKey(q.questionID))
                        CollectionManager.questionCollection.Add(q.questionID, new question(q.questionID, q.answerIDList, "Match: " + a.front, null, null));

                    if (!CollectionManager.answerCollection.ContainsKey(a.termID))
                    {
                        CollectionManager.answerCollection.Add(a.termID, new answer(a.termID, a.back, null, a.imageLocation, a.audioLocation));
                    }
                }
            }
            else if (q.type == "PHRASE")
            {
                foreach (AnswerJson a in q.answers)
                {
                    q.answerIDList[0] = a.termID;

                    if (!CollectionManager.questionCollection.ContainsKey(q.questionID))
                        CollectionManager.questionCollection.Add(q.questionID, new question(q.questionID, q.answerIDList, q.questionText, null, null));

                    if (!CollectionManager.answerCollection.ContainsKey(a.termID))
                    {
                        CollectionManager.answerCollection.Add(a.termID, new answer(a.termID, a.back, null, a.imageLocation, a.audioLocation));
                        //Debug.Log("Term ID " + a.termID);
                    }
                }
            }
            else if (q.type == "IMAGE")
            {
                foreach (AnswerJson a in q.answers)
                {
                    q.answerIDList[0] = a.termID;

                    if (!CollectionManager.questionCollection.ContainsKey(q.questionID))
                        CollectionManager.questionCollection.Add(q.questionID, new question(q.questionID, q.answerIDList, q.questionText + a.front, null, null));

                    if (!CollectionManager.answerCollection.ContainsKey(a.termID))
                    {
                        CollectionManager.answerCollection.Add(a.termID, new answer(a.termID, a.back, null, a.imageLocation, a.audioLocation));
                        //Debug.Log("Term ID " + a.termID);
                    }
                }
            }
            else
            {
                //Debug.Log("QUESTION HAS INVALID TYPE " + q.type);
            }
        }

        Debug.Log("CollectionMade");
        collectionMade = true;
    }

    public static IEnumerator startSession(int moduleID)
    {
        string url = ipAdress  + "/session";
        WWWForm form = new WWWForm();

        string sessionDate = DateTime.Now.ToString(@"MM\/dd\/yy");
        string startTime = DateTime.Now.ToString("h:mm");

        form.AddField("moduleID", moduleID);
        form.AddField("sessionDate", sessionDate);
        form.AddField("startTime", startTime);
        form.AddField("platform", "cp");

        UnityWebRequest startSessionRequest = UnityWebRequest.Post(url, form);

        TokenJson token = TokenJson.createFromJson(loginTokenString);

        startSessionRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

        Debug.Log("Test Start " + startTime);

        yield return startSessionRequest.SendWebRequest();

        string sessionJsonString = startSessionRequest.downloadHandler.text;

        sessionJson session = sessionJson.createFromJson(sessionJsonString);

        sessionID = session.getSessionID();
    }

    public static IEnumerator endSession(int points)
    {
        string url = ipAdress  + "/endsession";
        WWWForm form = new WWWForm();

        System.DateTime timeNow = DateTime.Now;

        string endTimeStringBefore = timeNow.ToString("h:mm");

        // Due to the time still progressing while the player is paused, we keep track of how long the player is paused and subtract it from the time the player ended the session.
        System.DateTime endTime = timeNow.AddSeconds(-1 * PauseScript.pauseTimer);

        string endTimeString = endTime.ToString("h:mm");
    

        Debug.Log("end time modified: " + endTimeString);
        Debug.Log("end time original: " + endTimeStringBefore);

        form.AddField("sessionID", sessionID);
        form.AddField("endTime", endTimeString);
        form.AddField("playerScore", points.ToString());

        UnityWebRequest endSessionRequest = UnityWebRequest.Post(url, form);

        TokenJson token = TokenJson.createFromJson(loginTokenString);

        endSessionRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

        Time.timeScale = 0;

        yield return endSessionRequest.SendWebRequest();

        Time.timeScale = 1;

        Debug.Log("Time Scale 1: Backendhook");

        if (endSessionRequest.isNetworkError || endSessionRequest.isHttpError)
        {
            Debug.Log(endSessionRequest.error);
        }
        else
        {
            Debug.Log("endSession Upload complete!");
        }
    }

    // questionAnswer = true: question
    // questionAnswer = false: answer
    public static IEnumerator downloadImg(string imageName, bool questionAnswer, int ID)
    {
        string url = "https://endlesslearner.com/" + imageName;
        TokenJson token = TokenJson.createFromJson(loginTokenString);

        UnityWebRequest pictureRequest = UnityWebRequestTexture.GetTexture(url);

        pictureRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

        Debug.Log(url);

        yield return pictureRequest.SendWebRequest();

        if (pictureRequest.isNetworkError || pictureRequest.isHttpError)
        {
            Debug.Log("Pic Error:" + pictureRequest.error);
        }
        else
        {
            Debug.Log("Upload complete!");
        }

        Texture2D myTexture = ((DownloadHandlerTexture)pictureRequest.downloadHandler).texture;

        if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Images"))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Images");
        }


        if (questionAnswer == false)
        {
            SaveTextureAsPNG(myTexture, Application.persistentDataPath + " /Images/", "answer" + ID);
        }

        if (questionAnswer == true)
        {
            SaveTextureAsPNG(myTexture, Application.persistentDataPath + " /Images/", "question" + ID);
        }
    }

    public static IEnumerator downloadAudio(string audioName, bool questionAnswer, int ID)
    {
        Debug.Log("AUDIO " + audioName);

        string url = ipAdress + audioName;
        TokenJson token = TokenJson.createFromJson(loginTokenString);

        UnityWebRequest pictureRequest = UnityWebRequestTexture.GetTexture(url);

        pictureRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

        yield return pictureRequest.SendWebRequest();
    }

    // Login
    public static IEnumerator login(string username, string password)
    {
        string url = ipAdress  + "/login";
        WWWForm form = new WWWForm();

        form.AddField("username", username);
        form.AddField("password", password);
        UnityWebRequest loginRequest = UnityWebRequest.Post(url, form);

        Debug.Log("LOGIN ATTEMPT" + username + " " + password);

        yield return loginRequest.SendWebRequest();

        if (loginRequest.isNetworkError || loginRequest.isHttpError)
        {
            Debug.Log(loginRequest.error);
        }
        else
        {
            Debug.Log("Upload complete!");
        }

        loginTokenString = loginRequest.downloadHandler.text;

        Debug.Log("Login attempt " + loginTokenString);
    }

    // Lists the Modules avalible to the player logged in
    public static IEnumerator avalibleModules()
    {
        TokenJson token = TokenJson.createFromJson(loginTokenString);
        string url = ipAdress  + "/modules";

        UnityWebRequest getModulesRequest = UnityWebRequest.Get(url);

        //Debug.Log(token.access_token);
        getModulesRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

        yield return getModulesRequest.SendWebRequest();

        string avalibleModulesJson = getModulesRequest.downloadHandler.text;
        //Debug.Log(avalibleModulesJson);


        ModulesJson[] ModulesJsonObjects = JsonHelper.getJsonArray<ModulesJson>(avalibleModulesJson);


        foreach (ModulesJson m in ModulesJsonObjects)
        {
            avalibleModulesDic.Add(m.name, m.moduleID);
        }

        modulesFound = true;
    }
    
    public static IEnumerator sendQuestionInfo(int questionID, int termID, bool correct)
    {
        LoginButton.runninggetModuleInfo.Add(true);

        TokenJson token = TokenJson.createFromJson(loginTokenString);
        string url = ipAdress  + "/loggedanswer";

        string correct2 = "wrong";

        if (correct == true)
            correct2 = "1";
        if (correct == false)
            correct2 = "0";

        WWWForm form = new WWWForm();
        form.AddField("questionID", questionID);
        form.AddField("termID", termID);
        form.AddField("sessionID", sessionID);
        form.AddField("correct", correct2);

        UnityWebRequest getModuleRequest = UnityWebRequest.Post(url, form);

        getModuleRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

        yield return getModuleRequest.SendWebRequest();

        if (getModuleRequest.isNetworkError || getModuleRequest.isHttpError)
        {
            Debug.Log(getModuleRequest.error);
        }
        else
        {
            Debug.Log("Upload complete!");
        }
    }

    public static void SaveTextureAsPNG(Texture2D texture, string path, string imageName)
    {
        byte[] _bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path + "/" + imageName + ".png", _bytes);
        //Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + path);
    }

    public static IEnumerator getModuleQuestionCount(int ID)
    {
        LoginButton.runninggetModuleInfo.Add(true);

        TokenJson token = TokenJson.createFromJson(loginTokenString);
        string url = ipAdress  + "/modulequestions";

        WWWForm form = new WWWForm();
        form.AddField("moduleID", ID);

        UnityWebRequest getModuleRequest = UnityWebRequest.Post(url, form);

        getModuleRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

        yield return getModuleRequest.SendWebRequest();

        moduleJsonString = getModuleRequest.downloadHandler.text;

        QuestionJson[] questionJsonObjects = JsonHelper.getJsonArray<QuestionJson>(moduleJsonString);

        int i = 0;
        foreach (QuestionJson q in questionJsonObjects)
        {
            if (q.type == "IMAGE")
                modulesWithImages.Add(ID);

            if (q.type == "LONGFORM" || q.type == "MATCH" || q.type == "IMAGE" || q.type == "PHRASE")
            {
                i++;
            }
            else
            {
                Debug.Log("QUESTION HAS INVALID TYPE " + q.type);
            }
        }

        moduleQCount.Add(ID, i);

        LoginButton.runninggetModuleInfo.Remove(true);
    }

    public static IEnumerator getModuleHighScore(int ID)
    {
        LoginButton.runninggetModuleInfo.Add(true);

        TokenJson token = TokenJson.createFromJson(loginTokenString);
        string url = ipAdress  + "/highscores";

        WWWForm form = new WWWForm();
        form.AddField("moduleID", ID);
        form.AddField("platform", "cp");

        UnityWebRequest getModuleRequest = UnityWebRequest.Post(url, form);

        getModuleRequest.SetRequestHeader("Authorization", "Bearer " + token.access_token);

        yield return getModuleRequest.SendWebRequest();

        string highScoreJson = getModuleRequest.downloadHandler.text;

        HighScoreJson[] HighScoreJsons = JsonHelper.getJsonArray<HighScoreJson>(highScoreJson);

        var hsList = new List<int>();

        foreach (HighScoreJson hs in HighScoreJsons)
        {
            if (hs.score != null)
                hsList.Add(hs.score);
        }

        if (hsList.Count != 0)
            highScores.Add(ID, hsList.Max());
        else
            highScores.Add(ID, 0);

        LoginButton.runninggetModuleInfo.Remove(true);
    }

    public IEnumerator createAccount(string username, string password)
    {
        string url = ipAdress  + "/register";
        WWWForm form = new WWWForm();

        form.AddField("username", "username");
        form.AddField("password", "password");
        form.AddField("password_confirm", "password");

        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();
    }
}
