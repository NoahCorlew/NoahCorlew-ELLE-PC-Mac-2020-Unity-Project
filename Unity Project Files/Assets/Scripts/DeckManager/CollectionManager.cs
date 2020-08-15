using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

// Manages the entire collection of QAs from BackendHook
// Once created, should not be edited
public class CollectionManager : MonoBehaviour
{
    // the key is the question/answerID
    public static Dictionary<int, question> questionCollection = new Dictionary<int, question>();
    public static Dictionary<int, answer> answerCollection = new Dictionary<int, answer>();

    public static Dictionary<int, ModuleInfo> moduleCollection = new Dictionary<int, ModuleInfo>();

    public static void createCardCollection()
    {
        // Creates a list of test questions
        if (false)
        {
            for (int i = 0; i != 8; i++)
            {
                questionCollection.Add(i, new question(i, new int[] { i, i + 3 }, "question answers: " + i + " " + (i + 3), null, null));
            }

            // Creates a list of test answers
            for (int i = 0; i != 8; i++)
            {
                answerCollection.Add(i, new answer(i, "answer phrase " + i, null, null, null));
            }
        }
}

    public static void printCollection()
    {
        foreach (KeyValuePair<int, question> entry in questionCollection)
        {
            Debug.Log("Question ID: " + entry.Key + " or " + entry.Value.getQuestionID());
            Debug.Log("    Has text: " + entry.Value.getQuestionPhrase());
            Debug.Log("    Is Answered by: ");
            foreach (int id in entry.Value.getAnswerIDList())
            {
                if (id != 0)
                    Debug.Log(id);
            }
        }
    }
}
