using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using System.Linq;

public class FirebaseMangerMain : MonoBehaviour{
    //Firebase variables
    [Header("Firebase")]
    public static Firebase.Auth.FirebaseAuth auth;
    public static DatabaseReference reference;
    public static DataSnapshot snapshot;
    public static FirebaseUser User; 

    //Main Page variables
    [Header("Main")]
    public TMP_Text welcomeText;

    [Header("Scoreboard")]
    public GameObject scoreElement;
    public Transform  scoreboardContent1;
    public Transform  scoreboardContent2;
    public Transform  scoreboardContent3;

    public TMP_Text StatText;
    public Image achievement1;
    public Image achievement2;
    public Image achievement3;
    public Image achievement4;
    public Image achievement5;
    public Image achievement6;


    
    
    
    private void Awake(){
        auth = FirebaseAuth.DefaultInstance ; //get the Auth created by other scene.
//        Debug.Log(FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        User = FirebaseAuth.DefaultInstance.CurrentUser;
        reference  =  FirebaseDatabase.DefaultInstance.RootReference;

        StartCoroutine(LoadUserData());

        StartCoroutine(LoadScoreboardData());
        

    }


    
    public void SignOutButton(){
        //logout and return to Login.
        auth.SignOut(); 
        SceneManager.LoadScene("LoginRegisterScene");
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
            
        }else{
            
            //Data has been retrieved
            snapshot = DBTask.Result;
            Debug.Log(snapshot.Child("Username").Value.ToString()+"'s Data Get");

            welcomeText.text = "Welcome! " + snapshot.Child("Username").Value.ToString();
            StatText.text += "Best BoxGame Score: "+snapshot.Child("Game2BestScore").Value.ToString();
            StatText.text += "\n\nQuiz Best Score: "+snapshot.Child("Game1BestScore").Value.ToString();
            StatText.text += "\n\nHunting Best Score: "+snapshot.Child("Game3BestScore").Value.ToString();

            if(int.Parse(snapshot.Child("Game1BestScore").Value.ToString())>20){
                achievement1.color = new Color32(255,255,255,255);
            }

            if(int.Parse(snapshot.Child("Game1BestScore").Value.ToString())>30){
                achievement2.color = new Color32(255,255,255,255);
            }

            if(int.Parse(snapshot.Child("Game2BestScore").Value.ToString())>100){
                achievement3.color = new Color32(255,255,255,255);
            }

            if(int.Parse(snapshot.Child("Game2BestScore").Value.ToString())>500){
                achievement4.color = new Color32(255,255,255,255);
            }

            if(int.Parse(snapshot.Child("Game3BestScore").Value.ToString())>1000){
                achievement5.color = new Color32(255,255,255,255);
            }
        
            
            if(int.Parse(snapshot.Child("Game2BestScore").Value.ToString())>0&&int.Parse(snapshot.Child("Game1BestScore").Value.ToString())>0&&int.Parse(snapshot.Child("Game3BestScore").Value.ToString())>0){
                achievement6.color = new Color32(255,255,255,255);
            }


            var lowest = snapshot.Child("Game1BestScore");
            if (int.Parse(lowest.Value.ToString()) > int.Parse(snapshot.Child("Game2BestScore").Value.ToString()))
                lowest = snapshot.Child("Game2BestScore");
            if (int.Parse(lowest.Value.ToString()) > int.Parse(snapshot.Child("Game3BestScore").Value.ToString()))
                lowest = snapshot.Child("Game3BestScore");
          
            Debug.Log(lowest.Key);
           
        }
    }


    private IEnumerator LoadScoreboardData(){
        //Get all the users data ordered by kills amount
        var DBTask = reference.Child("users").OrderByChild("Game1BestScore").LimitToLast(5).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null){
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }else{
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            Debug.Log(snapshot);

            int i = 1;
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>()){
                if(childSnapshot.Child("UserType").Value.ToString().Equals("teacher")){
                    continue;
                }
                string username = childSnapshot.Child("Username").Value.ToString();
                Debug.Log(username);
                
                int score = int.Parse(childSnapshot.Child("Game1BestScore").Value.ToString());
                Debug.Log(childSnapshot.Child("Game1BestScore").Value.ToString());
                string rankingString;
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent1);
                switch(i){
                    case 1 : rankingString = "1ST";break;
                    case 2 : rankingString = "2ND";break;
                    case 3 : rankingString = "3RD";break;
                    default: rankingString = i + "TH";break;
                }

                scoreboardElement.GetComponent<ScoreObject>().NewScoreElement(username, score,rankingString);
                
                i++;


            } 
        }
        DBTask = reference.Child("users").OrderByChild("Game2BestScore").LimitToLast(5).GetValueAsync() ;
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null){
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }else{
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;


            int i = 1;
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>()){
                string username = childSnapshot.Child("Username").Value.ToString();
                if(childSnapshot.Child("UserType").Value.ToString().Equals("teacher")){
                    continue;
                }
                int score = int.Parse(childSnapshot.Child("Game2BestScore").Value.ToString());
                string rankingString;
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent2);
                switch(i){
                    case 1 : rankingString = "1ST";break;
                    case 2 : rankingString = "2ND";break;
                    case 3 : rankingString = "3RD";break;
                    default: rankingString = i + "TH";break;
                }

                scoreboardElement.GetComponent<ScoreObject>().NewScoreElement(username, score,rankingString);
                
                i++;


            } 
        }

        DBTask = reference.Child("users").OrderByChild("Game3BestScore").LimitToLast(5).GetValueAsync() ;
        
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null){
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }else{
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            int i = 1;
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>()){
                string username = childSnapshot.Child("Username").Value.ToString();

                if(childSnapshot.Child("UserType").Value.ToString().Equals("teacher")){
                    continue;
                }
                int score = int.Parse(childSnapshot.Child("Game3BestScore").Value.ToString());
                string rankingString;
                GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent3);
                switch(i){
                    case 1 : rankingString = "1ST";break;
                    case 2 : rankingString = "2ND";break;
                    case 3 : rankingString = "3RD";break;
                    default: rankingString = i + "TH";break;
                }

                scoreboardElement.GetComponent<ScoreObject>().NewScoreElement(username, score,rankingString);
                
                i++;


            } 
        }
    }


}
