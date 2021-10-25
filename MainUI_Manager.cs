using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainUI_Manager : MonoBehaviour
{
    public static MainUI_Manager instance;
    public GameObject HomePage;
    public GameObject GamePage;
    public GameObject StatPage;
    public GameObject LeaderboardPage;
    public GameObject OtherPage;
    public GameObject CreditPage;

    public GameObject Game1Scoreboard;
    public GameObject Game2Scoreboard;
    public GameObject Game3Scoreboard;



     private void Awake(){
        if (instance == null){
            instance = this;
        }else if (instance != null){
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }
    
    
    public void HomePageON(){
        HomePage.SetActive(true);
        GamePage.SetActive(false);
        StatPage.SetActive(false);
        LeaderboardPage.SetActive(false);
        OtherPage.SetActive(false);
        CreditPage.SetActive(false);

    }
    public void GamePageON(){
        HomePage.SetActive(false);
        GamePage.SetActive(true);
        StatPage.SetActive(false);
        LeaderboardPage.SetActive(false);
        OtherPage.SetActive(false);
        CreditPage.SetActive(false);
    }

    public void StatPageON(){
        HomePage.SetActive(false);
        GamePage.SetActive(false);
        StatPage.SetActive(true);
        LeaderboardPage.SetActive(false);
        OtherPage.SetActive(false);
        CreditPage.SetActive(false);

    }
    public void LeaderboardPageON(){
        HomePage.SetActive(false);
        GamePage.SetActive(false);
        StatPage.SetActive(false);
        LeaderboardPage.SetActive(true);
        OtherPage.SetActive(false);
        CreditPage.SetActive(false);
    }

    public void OtherPageON(){
        HomePage.SetActive(false);
        GamePage.SetActive(false);
        StatPage.SetActive(false);
        LeaderboardPage.SetActive(false);
        OtherPage.SetActive(true);
        CreditPage.SetActive(false);

    }

    public void CreditPageON(){
        OtherPage.SetActive(false);
        CreditPage.SetActive(true);
    }
    
    public void BackToOther(){
        OtherPage.SetActive(true);
        CreditPage.SetActive(false);
    }
    
    public void Quit(){
       

        Application.Quit();
    }

    public void Scoreboard1(){
        Game1Scoreboard.SetActive(true);
        Game2Scoreboard.SetActive(false);
        Game3Scoreboard.SetActive(false);

    }

    public void Scoreboard2(){
        Game1Scoreboard.SetActive(false);
        Game2Scoreboard.SetActive(true);
        Game3Scoreboard.SetActive(false);

    }

    public void Scoreboard3(){
        Game1Scoreboard.SetActive(false);
        Game2Scoreboard.SetActive(false);
        Game3Scoreboard.SetActive(true);

    }

    public void Game1On(){
        SceneManager.LoadScene("BoxGame");
    }
     
      public void Game2On(){
        SceneManager.LoadScene("Play");
    }

    public void Game3On(){
        SceneManager.LoadScene("Game3");
    }


}
