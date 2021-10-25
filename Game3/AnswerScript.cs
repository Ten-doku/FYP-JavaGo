using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerScript : MonoBehaviour
{
    public bool isCorrect = false;
    public BattleHandler battleHandler;
    public void Answer(){
        if(isCorrect){
            battleHandler.correct();
        }
        else{
            battleHandler.wrong();
        }
    }
}
