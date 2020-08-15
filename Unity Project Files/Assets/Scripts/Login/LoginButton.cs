using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Net;
using UnityEngine.SceneManagement;

public class LoginButton : MonoBehaviour
{

    public static bool usernameSelected = false;
    public static bool passwordSelected = false;
    public GameObject badLoginText;
    public InputField usernameInputField;
    public InputField passwordInputfield;
    public Text username;
    public Text password;
    public bool pingModulesOnce = false;
    public bool getModLengths = false;
    public static List<bool> runninggetModuleInfo = new List<bool>();
    public  List<InputField> fields;
    private int _fieldIndexer;
    private float badLoginFlashTimer = 0;
    private bool flashBadLogin = false;
    private bool badLoginShown = false;


void Start()
{
    fields = new List<InputField> {usernameInputField, passwordInputfield};

    usernameInputField.Select();
        _fieldIndexer = 1;
}

void Update()
{
    if (Input.GetKeyDown(KeyCode.Tab))
    {
        if (fields.Count <= _fieldIndexer)
        {
            _fieldIndexer = 0;
        }
        fields[_fieldIndexer].Select();
        _fieldIndexer++;
    }

    if (Input.GetKeyDown(KeyCode.Return))
    {
        loginAttempt();
    }

    if (BackendHook.loginTokenString != null)
    {
        if (BackendHook.loginTokenString.Contains("User Not Found") || BackendHook.loginTokenString.Contains("Incorrect"))
        {
            badLoginShown = true;
        }
        else if (!pingModulesOnce)
        {
            Debug.Log("*hacker voice* I'm in");
            StartCoroutine(BackendHook.avalibleModules());
            pingModulesOnce = true;
        }
    }

    if (badLoginShown == true)
        badLoginText.SetActive(true);

    if (!getModLengths && BackendHook.modulesFound)
    {
        foreach (KeyValuePair<string, int> m in BackendHook.avalibleModulesDic)
        {
            StartCoroutine(BackendHook.getModuleQuestionCount(m.Value));
            StartCoroutine(BackendHook.getModuleHighScore(m.Value));
        }

        getModLengths = true;
    }

    // Don't hate me please
    if (getModLengths)
    {
        if (runninggetModuleInfo.Count == 0)
            SceneManager.LoadScene("ModuleSelect");
    }
}

    public void loginAttempt()
    {
        if (username.text != "" && password.text != "")
        {
            StartCoroutine(BackendHook.login(username.text, passwordInputfield.text));
        }
    }
}
