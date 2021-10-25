using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager instance;

    public GameObject loginUI;
    public GameObject registerUI;
    public GameObject registerUI_T;

    private void Awake(){
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    //Functions to change the login screen UI
    public void LoginScreenON() //Back button
    {
        loginUI.SetActive(true);
        registerUI.SetActive(false);
        registerUI_T.SetActive(false);
    }
    public void RegisterScreenON() // Regester button
    {
        loginUI.SetActive(false);
        registerUI_T.SetActive(false);
        registerUI.SetActive(true);
    }

    public void RegisterAsTeacherScreenON() // Regester button
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        registerUI_T.SetActive(true);
    }
}
