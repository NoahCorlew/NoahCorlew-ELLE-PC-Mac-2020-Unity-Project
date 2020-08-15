using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class answer
{
    int answerID;
    string answerPhrase;
    Texture2D answerImage;
    public string imageLocation;
    public string audioLocation;

    public answer(int answerID, string answerPhrase, Texture2D answerImage, string imageLocation, string audioLocation)
    {
        this.answerID = answerID;
        this.answerPhrase = answerPhrase;
        this.answerImage = answerImage;
        this.imageLocation = imageLocation;
        this.audioLocation = audioLocation;
    }

    public string getAnswerPhrase()
    {
        return answerPhrase;
    }

    public int getAnswerID()
    {
        return answerID;
    }

    public void setAnswerImage(Texture2D t)
    {
        answerImage = t;
    }

    public string getImageLocation()
    {
        return imageLocation;
    }

    public string getAudioLocation()
    {
        return audioLocation;
    }
}
