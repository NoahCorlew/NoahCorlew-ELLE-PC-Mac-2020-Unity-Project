using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public int moduleIDonButton;

    public void pickModule(int moduleID)
    {
        ModuleSelect.setSelectedModule(moduleID);
    }

    public void setButtonModule(int n)
    {
        moduleIDonButton = n;
    }
}
