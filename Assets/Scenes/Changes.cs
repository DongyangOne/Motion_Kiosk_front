using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Changes : MonoBehaviour
{
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2) // 0,1.2 �� ��° �� �Ǹ� 3���� �� ��ȯ
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // �� ��° �� + 1 �� �� ��° �� ��ȯ
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
        Debug.Log("�α����ϱ�");
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
