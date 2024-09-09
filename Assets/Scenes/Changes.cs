using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Changes : MonoBehaviour
{
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2) // 0,1.2 세 번째 씬 되면 3초후 씬 전환
        {
            Invoke("SecondScene1", 3f);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3) 
        {
            Invoke("ThirdScene", 3f);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            Invoke("FourthScene", 3f);
        }
    }
    void SecondScene1()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // 세 번째 씬 + 1 후 네 번째 씬 전환
    }
    void ThirdScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    void FourthScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void OnClickLoginButton()
    {
        Debug.Log("로그인하기");
    }

    public void FirstChange()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void SceneChange()
    {
        SceneManager.LoadScene("SecondScene");
    }
    public void SceneChange1()
    {
        SceneManager.LoadScene("FourthScene");
    }
    public void SceneChange2()
    {
        SceneManager.LoadScene("ThirdScene");
    }
    public void SceneChange3()
    {
        SceneManager.LoadScene("FourthScene");
    }
}
