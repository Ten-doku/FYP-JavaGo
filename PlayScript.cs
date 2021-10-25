using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using System.Linq;

public class PlayScript : MonoBehaviour
{        [Header("Firebase")]
    public static Firebase.Auth.FirebaseAuth auth;
    public static DatabaseReference reference;
    public static DataSnapshot snapshot;
    public static FirebaseUser User; 

    private int BestScore;

    public Text scoreTxt;
   
    private int score;
        private void Awake(){
        auth = FirebaseAuth.DefaultInstance ; //get the Auth created by other scene.
        //Debug.Log(FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        User = FirebaseAuth.DefaultInstance.CurrentUser;
        reference  =  FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(LoadUserData());
  
    }

     private IEnumerator LoadUserData(){
        //Get the currently logged in user data
        var DBTask = reference.Child("users").Child(User.UserId).GetValueAsync();

         yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

         Debug.Log("DBTask:"+DBTask.Result);

        if (DBTask.Exception != null) {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null){
            //No data exists yet 
            BestScore = 0;
        }else{
            
            //Data has been retrieved
            snapshot = DBTask.Result;
            BestScore = int.Parse(snapshot.Child("Game1BestScore").Value.ToString());
            
        }
    }

    public void endButton(){
        StartCoroutine(UploadData());
    }
    private IEnumerator UploadData(){

        int score = int.Parse(scoreTxt.text);
        if (score > BestScore){
            var DBTask2 = reference.Child("users").Child(User.UserId).Child("Game1BestScore").SetValueAsync(score);
            yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);
            if (DBTask2.Exception != null){
                    Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
                }else{
            }
        }
        
    }


  
}
