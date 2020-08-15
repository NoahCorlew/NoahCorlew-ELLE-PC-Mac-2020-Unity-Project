using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulePageManager : MonoBehaviour
{
    public GameObject pButton;
    public GameObject nButton;
    public static int currentPage = 0;
    private int maxPages;

    void Start()
    {
        maxPages = (int) Mathf.Ceil(ModuleSelect.moduleCount / 9);
    }

    void Update()
    {
        if (currentPage == 0)
            pButton.SetActive(false);
        else
            pButton.SetActive(true);


        if (currentPage == maxPages)
            nButton.SetActive(false);
        else
            nButton.SetActive(true);
    }


    public void nextPage()
    {
        currentPage++;

        ModuleSelect.nPage = true;
    }

    public void prevPage()
    {
        currentPage--;
        ModuleSelect.pPage = true;
    }

}
