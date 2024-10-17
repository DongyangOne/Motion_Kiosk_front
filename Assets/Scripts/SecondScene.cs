using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SequentialComponentController : MonoBehaviour
{
    public GameObject[] components;  // 순차적으로 켜고 끌 컴포넌트들
    public string nextSceneName;     // 마지막 컴포넌트가 끝난 후 넘어갈 씬 이름

    void Start()
    {
        foreach (GameObject component in components)
        {
            component.SetActive(false);  // 모든 컴포넌트를 시작 시 비활성화
        }
        // 컴포넌트를 순차적으로 켜고 끄는 코루틴 실행
        StartCoroutine(ActivateComponentsSequentially());
    }

    // 컴포넌트들을 순차적으로 활성화/비활성화하는 코루틴
    IEnumerator ActivateComponentsSequentially()
    {
        foreach (GameObject component in components)
        {
            // 현재 컴포넌트를 활성화
            component.SetActive(true);
            Debug.Log(component.name + " 활성화됨");

            // 3초 대기
            yield return new WaitForSeconds(3f);

            // 현재 컴포넌트를 비활성화
            component.SetActive(false);
            Debug.Log(component.name + " 비활성화됨");
        }

        // 모든 컴포넌트가 끝난 후 씬 전환
        SceneManager.LoadScene(nextSceneName);
    }
}
