using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UsernameUI : MonoBehaviour
{
    public Text usernameField;
    public Text placeHolder;

    void Update()
    {
        if (usernameField.text == "")
            placeHolder.text = "....";
        else
            placeHolder.text = "";
    }
}
