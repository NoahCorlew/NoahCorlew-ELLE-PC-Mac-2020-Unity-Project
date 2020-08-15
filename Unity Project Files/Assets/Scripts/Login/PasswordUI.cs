using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class PasswordUI : MonoBehaviour
{
    public Text passwordField;
    public Text placeHolder;


    void Start()
    {

    }

    void Update()
    {
        if (passwordField.text == "")
            placeHolder.text = "....";
        else
            placeHolder.text = "";

    }
}
