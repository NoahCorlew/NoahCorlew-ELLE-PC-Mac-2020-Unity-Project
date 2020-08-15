using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Debug = UnityEngine.Debug;

// Can be expanded to have an image of the word and audio, among other things
public class question
{
    int questionID;             // questionID = questionDropAreaID
    int[] answerIDList;
    string questionPhrase;
    Texture2D questionImage;
    string imageLocation;

    public question(int questionID, int[] answerIDList, string questionPhrase, Texture2D questionImage, string imageLocation)
    {
        this.questionID = questionID;
        this.answerIDList = answerIDList;
        this.questionPhrase = questionPhrase;
        this.questionImage = questionImage;
        this.imageLocation = imageLocation;
    }

    public int getQuestionID()
    {
        return questionID;
    }

    public bool answerQuestion(int answerID)
    {
        return (answerIDList.Contains(answerID));
    }

    public int[] getAnswerIDList()
    {
        return answerIDList;
    }

    public string getQuestionPhrase()
    {
        return questionPhrase;
    }

    public void setAnswerImage(Texture2D t)
    {
        questionImage = t;
    }

    public string getImageLocation()
    {
        return imageLocation;
    }
}
