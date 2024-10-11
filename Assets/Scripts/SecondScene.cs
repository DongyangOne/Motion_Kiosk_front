using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SecondScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { 
        StartCoroutine(MoveToNextSceneAfterDelay(3));
    }

    private IEnumerator MoveToNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // 지정된 시간만큼 대기
        SceneManager.LoadScene("SecondScene1");  // 3초 후에 다음 씬으로 전환
    }

    void Update()
    {
        
    }
}