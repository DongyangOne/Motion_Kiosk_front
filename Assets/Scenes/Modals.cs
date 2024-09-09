using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modals : MonoBehaviour
{
    public GameObject Popup0; // ù ��° �˾�â
    public GameObject Popup;  // �� ��° �˾�â
    public GameObject Popup1; // �� ��° �˾�â
    public GameObject Popup2; // �� ��° �˾�â
    public float delayTime = 3f; // ������ �ð� (3��)

    void Start()
    {
        ShowPopup0(); // ù ��° �˾� ǥ��
    }

    // ù ��° �˾� 
    public void ShowPopup0()
    {
        Popup0.SetActive(true);
        Popup.SetActive(false);
        Popup1.SetActive(false);
        Popup2.SetActive(false);
    }

    // �� ��° �˾� 
    public void ShowPopup()
    {
        Popup0.SetActive(false);
        Popup.SetActive(true);
        Popup1.SetActive(false);
        Popup2.SetActive(false);
    }

    // �� ��° �˾� ǥ�� �� ������ �� �� ��° �˾����� ��ȯ
    public void ShowPopup1()
    {
        Popup.SetActive(false);
        Popup1.SetActive(true);
        Popup2.SetActive(false);

        // 3�� �� �� ��° �˾����� ��ȯ
        Invoke("ShowPopup2", delayTime);
    }

    // �� ��° �˾� ǥ��
    public void ShowPopup2()
    {
        Popup1.SetActive(false);
        Popup2.SetActive(true);
    }
}
