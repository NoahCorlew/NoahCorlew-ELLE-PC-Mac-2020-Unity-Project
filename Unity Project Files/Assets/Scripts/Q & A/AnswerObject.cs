using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

// Connects the question in the questionCollection dictionary in BackendHook to the physical card
// answerID = key in dictionary for the question
public class AnswerObject : MonoBehaviour
{
    public int answerID;
    public answer answer;
    public string answerText;
    public string cardArtLocation;
    public RawImage cardArt;
    private bool setImage = false;

    void Start()
    {
        answerText = answer.getAnswerPhrase();
    }

    void Update()
    {
        cardArtLocation = "";

        if (File.Exists(cardArtLocation) && !setImage)
        {
            Debug.Log("SET" + answer);
            cardArt = Resources.Load(cardArtLocation) as RawImage;
            setImage = true;
        }
    }
}
