using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modals : MonoBehaviour
{
    public GameObject Popup0; // Ã¹ ¹øÂ° ÆË¾÷Ã¢
    public GameObject Popup;  // µÎ ¹øÂ° ÆË¾÷Ã¢
    public GameObject Popup1; // ¼¼ ¹øÂ° ÆË¾÷Ã¢
    public GameObject Popup2; // ³× ¹øÂ° ÆË¾÷Ã¢
    public float delayTime = 3f; // µô·¹ÀÌ ½Ã°£ (3ÃÊ)

    void Start()
    {
        ShowPopup0(); // Ã¹ ¹øÂ° ÆË¾÷ Ç¥½Ã
    }

    // Ã¹ ¹øÂ° ÆË¾÷ 
    public void ShowPopup0()
    {
        Popup0.SetActive(true);
        Popup.SetActive(false);
        Popup1.SetActive(false);
        Popup2.SetActive(false);
    }

    // µÎ ¹øÂ° ÆË¾÷ 
    public void ShowPopup()
    {
        Popup0.SetActive(false);
        Popup.SetActive(true);
        Popup1.SetActive(false);
        Popup2.SetActive(false);
    }

    // ¼¼ ¹øÂ° ÆË¾÷ Ç¥½Ã ¹× µô·¹ÀÌ ÈÄ ³× ¹øÂ° ÆË¾÷À¸·Î ÀüÈ¯
    public void ShowPopup1()
    {
        Popup.SetActive(false);
        Popup1.SetActive(true);
        Popup2.SetActive(false);

        // 3ÃÊ ÈÄ ³× ¹øÂ° ÆË¾÷À¸·Î ÀüÈ¯
        Invoke("ShowPopup2", delayTime);
    }

    // ³× ¹øÂ° ÆË¾÷ Ç¥½Ã
    public void ShowPopup2()
    {
        Popup1.SetActive(false);
        Popup2.SetActive(true);
    }
}
