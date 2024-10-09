using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Changes : MonoBehaviour
{


    public void OnClickLoginButton()
    {
        SceneManager.LoadScene("Start");
    }

    public void onClickMotion()
    {
        SceneManager.LoadScene("SecondScene");

    }
    public void onClickSpeak()
    {
        SceneManager.LoadScene("SelectWhere");
    }

    public void moveManuPage()
    {
        SceneManager.LoadScene("MenuPage");
    }
   

}
