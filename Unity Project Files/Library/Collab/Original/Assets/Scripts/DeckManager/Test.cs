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

public class Test : MonoBehaviour
{
    string loginToken = "e";
    bool loginFlag = false;
    bool testFlag = false;

    void Update()
    {
        if (loginFlag == false)
        {
            StartCoroutine(login("NoahTestUsername", "NoahTestPassword"));
            loginFlag = true;
        }

        if (loginToken != "e" && testFlag == false)
        {
            StartCoroutine(getModule(1));
            testFlag = true;
        }
    }

    public IEnumerator login(string username, string password)
    {
        string url = "http://34.239.123.94:3000/login";
        WWWForm form = new WWWForm();

        form.AddField("username", username);
        form.AddField("password", password);
        UnityWebRequest loginRequest = UnityWebRequest.Post(url, form);

        yield return loginRequest.SendWebRequest();

        loginToken = loginRequest.downloadHandler.text;

        Debug.Log("Token: " + loginToken);
    }

    public IEnumerator getModule(int ID)
    {
        string url = "http://34.239.123.94:3000/modules";
        WWWForm form = new WWWForm();

        form.AddField("moduleID", ID);
        UnityWebRequest getModuleRequest = UnityWebRequest.Get(url);

        Debug.Log("Token in GM " + loginIDfromJson(loginToken).access_token);

        getModuleRequest.SetRequestHeader("header", loginToken);
        yield return getModuleRequest.SendWebRequest();

        Debug.Log(getModuleRequest.downloadHandler.text);
    }

    public static object loginIDfromJson(string json)
    {
        return loginID.CreateFromJSON(json);
    }

}


/*
 * 
        WebClient questionClient = new WebClient();
        questionClient.QueryString.Add("moduleID", "1");
        string result = questionClient.DownloadString("http://34.239.123.94:3000/module");
              Debug.Log(result);

    */

/*
string url = "http://34.239.123.94:3000/register";
WWWForm form = new WWWForm();

form.AddField("username", "NoahTestUsername");
form.AddField("password", "NoahTestPassword");
form.AddField("password_confirm", "NoahTestPassword");

UnityWebRequest www = UnityWebRequest.Post(url, form);

www.SendWebRequest();
*/
