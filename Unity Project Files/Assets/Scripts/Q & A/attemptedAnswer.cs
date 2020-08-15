using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attemptedAnswer
{
    public bool correct;
    public question question;
    public answer answer;

    public attemptedAnswer(bool correct, question question, answer answer)
    {
        this.correct = correct;
        this.question = question;
        this.answer = answer;
    }
}
