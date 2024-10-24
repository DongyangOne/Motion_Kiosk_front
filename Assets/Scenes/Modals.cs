using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modals : MonoBehaviour
{
    public GameObject Popup0; // 첫 번째 팝업창
    public GameObject Popup;  // 두 번째 팝업창
    public GameObject Popup1; // 세 번째 팝업창
    public GameObject Popup2; // 네 번째 팝업창
    public float delayTime = 3f; // 딜레이 시간 (3초)

    void Start()
    {
        ShowPopup0(); // 첫 번째 팝업 표시
    }

    // 첫 번째 팝업 
    public void ShowPopup0()
    {
        Popup0.SetActive(true);
        Popup.SetActive(false);
        Popup1.SetActive(false);
        Popup2.SetActive(false);
    }

    // 두 번째 팝업 
    public void ShowPopup()
    {
        Popup0.SetActive(false);
        Popup.SetActive(true);
        Popup1.SetActive(false);
        Popup2.SetActive(false);
    }

    // 세 번째 팝업 표시 및 딜레이 후 네 번째 팝업으로 전환
    public void ShowPopup1()
    {
        Popup.SetActive(false);
        Popup1.SetActive(true);
        Popup2.SetActive(false);

        // 3초 후 네 번째 팝업으로 전환
        Invoke("ShowPopup2", delayTime);
    }

    // 네 번째 팝업 표시
    public void ShowPopup2()
    {
        Popup1.SetActive(false);
        Popup2.SetActive(true);
    }
}
