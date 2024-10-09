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

    public void onClickHereButton()
    {
        SceneManager.LoadScene("MenuPage");
    }
    public void onClickTakeOutButton()
    {
        SceneManager.LoadScene("MenuPage");
    }
}
