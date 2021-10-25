using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ListObject : MonoBehaviour
{
    public TMP_Text Question;
    public TMP_Text Answer;
    public TMP_Text Reason;

    public void NewListElement (string question, string answer, string reason)
    {
        Question.text = question;
        Answer.text = answer;
        Reason.text = reason;

    }
    
}
