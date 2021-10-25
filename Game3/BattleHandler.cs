using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using System.Linq;
using UnityEngine;
using CodeMonkey.Utils;

public class BattleHandler : MonoBehaviour
{
    [Header("Firebase")]
    public static Firebase.Auth.FirebaseAuth auth;
    public static DatabaseReference reference;
    public static DataSnapshot snapshot;
    public static FirebaseUser User; 
    public GameObject Witch;
    public Animator WitchAnimator;
    
    public GameObject Goblin;
    public Animator GoblinAnimator;
    public GameObject Mushroom;
    public Animator MushroomAnimator;
    public GameObject Skeleton;
    public Animator SkeletonAnimator;

    public GameObject QuestionPlaceholder;

    public GameObject ActionPlaceholder;

    public GameObject MessagePlaceholder;

    public GameObject EndGameScreen;

    public TMP_Text ScoreText;

    public TMP_Text QuestionText;

    public GameObject cross;
    public GameObject tick;

    public Transform ListContent;
    public GameObject ElementTemplate;

    private int BestScore;

    private int score = 0;

    private GameObject EnemyNow;
    private Animator EnemyAnimator;
    public GameObject[] options;
    public List<QuestionAndAnswers> QuestionsAndAnswers;

    

    public int currentQuestion;
    public int enemyNo = 0;//0 = Goblin 1 = mushroom 2 = skelton
    
    public int enemyCounter = 0;//End the game when the counter = 3 
    public int enemyMove;// 0 for wait , 1 for attack

    public int playerMove; // 0 for attack , 1 for dodge , 2 for magic , 3 for heal

    public int turns; //0 for player , 1 for enemy

    private HealthSystem healthSystem;

    private World_Bar healthbar;

    private World_Bar enHealthbar;

    private HealthSystem enemyHealthSystem;

    private bool success;

     private void Awake(){
        auth = FirebaseAuth.DefaultInstance ; //get the Auth created by other scene.
        //Debug.Log(FirebaseAuth.DefaultInstance.CurrentUser.UserId);
        User = FirebaseAuth.DefaultInstance.CurrentUser;
        reference  =  FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(LoadUserData());
     }
     private void Update(){
         if(turns==1){
             turns--;
             if(enemyMove==1){
                enemyMove--;
                    if(enemyHealthSystem.getHealth()>0){
                        StartCoroutine(EnAttackAnim());
                    }
             }else{
                enemyMove++;
                showPlaceholder(1);
             }
         }
     }

     private void Start(){
        showPlaceholder(2);
        chooseEnemy();
        healthSystem = new HealthSystem(100);
        healthbar = new World_Bar(Witch.transform,new Vector3(0,0.5f),new Vector3(100,20),Color.grey,Color.red,1f,100,new World_Bar.Outline{color  = Color.black,size=1f});
        generateQuestion();
        showPlaceholder(1);
     }

     private void showPlaceholder(int i){ //0 = Question , 1 = Action , 2 = Message , 3 = EndGame Screen , default = close all
         switch(i){
            case 0: QuestionPlaceholder.SetActive(true);
                    ActionPlaceholder.SetActive(false);
                    MessagePlaceholder.SetActive(false);
                    EndGameScreen.SetActive(false);
                    
                    break;
            case 1: QuestionPlaceholder.SetActive(false);
                    ActionPlaceholder.SetActive(true);
                    MessagePlaceholder.SetActive(false);
                    EndGameScreen.SetActive(false);
                    break;
            case 2: QuestionPlaceholder.SetActive(false);
                    ActionPlaceholder.SetActive(false);
                    MessagePlaceholder.SetActive(true);
                    EndGameScreen.SetActive(false);
                    break;
            case 3: QuestionPlaceholder.SetActive(false);
                    ActionPlaceholder.SetActive(false);
                    MessagePlaceholder.SetActive(false);
                    EndGameScreen.SetActive(true);
                    break;
            default:QuestionPlaceholder.SetActive(false);
                    ActionPlaceholder.SetActive(false);
                    MessagePlaceholder.SetActive(false);
                    EndGameScreen.SetActive(false);
                    break;
         }
     }

     private void chooseEnemy(){
         enemyNo = Random.Range(0,3);
         switch (enemyNo)
         {
             case 0:
                    EnemyNow = Goblin;
                    EnemyAnimator = GoblinAnimator;
                    enemyHealthSystem = new HealthSystem(40);
                    enHealthbar =new World_Bar(EnemyNow.transform,new Vector3(0,0.3f),new Vector3(100,20),Color.grey,Color.red,1f,100,new World_Bar.Outline{color  = Color.black,size=1f});
                    EnemyNow.SetActive(true);
                    EnemyAnimator.SetFloat("hp",40f);
                    break;
             
             case 1:
                    EnemyNow = Mushroom;
                    EnemyAnimator = MushroomAnimator;
                    enemyHealthSystem = new HealthSystem(30);
                    enHealthbar = new World_Bar(EnemyNow.transform,new Vector3(0,0.3f),new Vector3(100,20),Color.grey,Color.red,1f,100,new World_Bar.Outline{color  = Color.black,size=1f});
                    EnemyNow.SetActive(true);
                    EnemyAnimator.SetFloat("hp",30f);
                    break;
             
             case 2:
                    EnemyNow = Skeleton;
                    EnemyAnimator = SkeletonAnimator;
                    enemyHealthSystem = new HealthSystem(50);
                    enHealthbar = new World_Bar(EnemyNow.transform,new Vector3(0,0.3f),new Vector3(100,20),Color.grey,Color.red,1f,100,new World_Bar.Outline{color  = Color.black,size=1f});
                    EnemyNow.SetActive(true);
                    EnemyAnimator.SetFloat("hp",50f);
                    break;
             
         }enemyCounter++;
         Debug.Log(enemyCounter);
     }

    private void generateQuestion(){
        currentQuestion = Random.Range(0,QuestionsAndAnswers.Count);
        QuestionText.text = QuestionsAndAnswers[currentQuestion].Question;
        setAnswers();

    }

    public void correct(){
        AudioManager.instance.audioSource.PlayOneShot(AudioManager.instance.Button);
        updateScore(50);
        success = true;
        StartCoroutine(showTick());
        showPlaceholder(2);
        switch(playerMove){
            case 1 :turns++;
                    break;
            case 2 :StartCoroutine(MagicAnim());
                    break;
            case 3 :healing();
                    break;
            case 0 :StartCoroutine(AttackAnim());
                    break;
        }
        generateQuestion();
        
        
        
    }

    public void wrong(){
        AudioManager.instance.audioSource.PlayOneShot(AudioManager.instance.Button);
        updateList();
        success = false;
        StartCoroutine(showCross());
        showPlaceholder(2);
        switch(playerMove){
            case 1 :turns++;
                    break;
            case 2 :turns++;
                    break;
            case 3 :healing();
                    break;
            case 0 :turns++;
                    break;
        }
        
        generateQuestion();

    }

    private void updateScore(int i){
        score += i;
        ScoreText.text = "Score:"+score; 
    }

    private void updateList(){
        //for(int i=0;i<10;i++){
        GameObject ListElement = Instantiate(ElementTemplate, ListContent);
        string question = QuestionsAndAnswers[currentQuestion].Question;
        string answer = QuestionsAndAnswers[currentQuestion].Answers[QuestionsAndAnswers[currentQuestion].CorrectAnswer-1];
        string reason = QuestionsAndAnswers[currentQuestion].Reason;
        ListElement.GetComponent<ListObject>().NewListElement(question , answer , reason);
        //}
    }

   


    private IEnumerator showTick(){
        tick.SetActive(true);
        yield return new WaitForSeconds(1);
        tick.SetActive(false);
        
    }


    private IEnumerator showCross(){
        cross.SetActive(true);
        yield return new WaitForSeconds(1);
        cross.SetActive(false);
        
    }
    private void setAnswers(){
        for(int i = 0 ; i < options.Length;i++){
            options[i].GetComponent<AnswerScript>().isCorrect = false;
            options[i].GetComponentInChildren<TextMeshProUGUI>().text = QuestionsAndAnswers[currentQuestion].Answers[i];
            if(QuestionsAndAnswers[currentQuestion].CorrectAnswer == i+1){
                options[i].GetComponent<AnswerScript>().isCorrect = true;
            }
        }
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
            BestScore = int.Parse(snapshot.Child("Game3BestScore").Value.ToString());
            
        }
    }

    public void endButton(){
        StartCoroutine(UploadData());
        SceneManager.LoadScene("MainMenuScene");
    }
    private IEnumerator UploadData(){
        if (score > BestScore){
            var DBTask2 = reference.Child("users").Child(User.UserId).Child("Game3BestScore").SetValueAsync(score);
            yield return new WaitUntil(predicate: () => DBTask2.IsCompleted);
            if (DBTask2.Exception != null){
                    Debug.LogWarning(message: $"Failed to register task with {DBTask2.Exception}");
                }else{
            }
        }
        
    }

     public void attackButton(){
        //StartCoroutine(AttackAnim());
        AudioManager.instance.audioSource.PlayOneShot(AudioManager.instance.Button);
        playerMove = 0 ;
        showPlaceholder(0);

     }
    
     private IEnumerator EnAttackAnim(){
        EnemyAnimator.SetBool("attack",true);
        if(enemyNo == 1){
            AudioManager.instance.audioSource.PlayOneShot(AudioManager.instance.Mushroom);
        }else{
            AudioManager.instance.audioSource.PlayOneShot(AudioManager.instance.Sword);
        }
        yield return new WaitForSeconds(0.5f);
        EnemyAnimator.SetBool("attack",false);
        if(playerMove == 1){ 
            if(success){ 
            WitchAnimator.SetBool("dodge",true);
            yield return new WaitForSeconds(0.2f);
            WitchAnimator.SetBool("dodge",false);
            }else{
                healthSystem.dodge(20);
                healthbar.SetSize(healthSystem.getHealthPercentage());
                WitchAnimator.SetFloat("hp",healthSystem.getHealth());
                StartCoroutine(getHitAnim());
            }
           
        }else {
            StartCoroutine(getHitAnim());
            healthSystem.damage(20,true);
            healthbar.SetSize(healthSystem.getHealthPercentage());
            WitchAnimator.SetFloat("hp",healthSystem.getHealth());
        }
        if(healthSystem.getHealth()<=0){
            healthbar.DestroySelf();
            enHealthbar.DestroySelf();
            showPlaceholder(3);
        }else{
            showPlaceholder(1);
        }
    }


     public void dodgeButton(){
        AudioManager.instance.audioSource.PlayOneShot(AudioManager.instance.Button);
        playerMove = 1 ;
        showPlaceholder(0);
     }

     public void magicButton(){
        AudioManager.instance.audioSource.PlayOneShot(AudioManager.instance.Button);
        playerMove = 2 ;
        showPlaceholder(0);
         
     }

     public void healButton(){
        AudioManager.instance.audioSource.PlayOneShot(AudioManager.instance.Button);
        playerMove = 3 ;
        showPlaceholder(0);
     }



     private IEnumerator AttackAnim(){
        bool weak ;
         WitchAnimator.SetBool("attack",true);
         AudioManager.instance.audioSource.PlayOneShot(AudioManager.instance.Magic1);
         yield return new WaitForSeconds(0.5f);
         WitchAnimator.SetBool("attack",false);
         EnemyAnimator.SetBool("getHit", true);
         yield return new WaitForSeconds(0.2f);
        EnemyAnimator.SetBool("getHit", false);
        if(enemyNo != 1){
           weak = true;
        }else  {weak = false;}
        enemyHealthSystem.damage(10,weak);
        enHealthbar.SetSize(enemyHealthSystem.getHealthPercentage());
        EnemyAnimator.SetFloat("hp",enemyHealthSystem.getHealth());
        yield return new WaitForSeconds(0.3f);
        if(enemyHealthSystem.getHealth() <=0){
            enHealthbar.DestroySelf();
            EnemyNow.SetActive(false);
            if(enemyCounter>3){
                Debug.Log("in");
                healthbar.DestroySelf();
                showPlaceholder(3);
            }else{
                chooseEnemy();
                showPlaceholder(1);
            }
        }else{
            Debug.Log(turns);
            turns++;
            Debug.Log(turns);

        }
      
     }

    private IEnumerator MagicAnim(){
        bool weak ;
         WitchAnimator.SetBool("magic",true);
         AudioManager.instance.audioSource.PlayOneShot(AudioManager.instance.Magic2);
         yield return new WaitForSeconds(0.5f);
         WitchAnimator.SetBool("magic",false);

         EnemyAnimator.SetBool("getHit", true);
         yield return new WaitForSeconds(0.2f);
        EnemyAnimator.SetBool("getHit", false);
        if(enemyNo != 2){
           weak = true;
        }else  {weak = false;}
        enemyHealthSystem.damage(10,weak);
        enHealthbar.SetSize(enemyHealthSystem.getHealthPercentage());
        EnemyAnimator.SetFloat("hp",enemyHealthSystem.getHealth());
        yield return new WaitForSeconds(0.3f);
        if(enemyHealthSystem.getHealth() <=0){
            EnemyNow.SetActive(false);
            if(enemyCounter>3){
                showPlaceholder(3);
            }else{
                chooseEnemy();
                showPlaceholder(1);
            }
        }else{
            Debug.Log(turns);
            turns++;
            Debug.Log(turns);

        }


     }

   
     private IEnumerator getHitAnim(){
         WitchAnimator.SetBool("getHit",true);
         yield return new WaitForSeconds(0.2f);
         WitchAnimator.SetBool("getHit",false);
     }

    private void healing(){
        healthSystem.heal(20 , success);
        healthbar.SetSize(healthSystem.getHealthPercentage());
        WitchAnimator.SetFloat("hp",healthSystem.getHealth());
        AudioManager.instance.audioSource.PlayOneShot(AudioManager.instance.Heal);
       
        turns++;

    }

}
