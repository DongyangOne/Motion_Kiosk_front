using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class UIModal : MonoBehaviour {
    public GameObject menuModal;
    public GameObject cartModal;
    public GameObject payModal;
    public GameObject cardModal;
    public GameObject completed;

    public TextMeshProUGUI title;
    public TextMeshProUGUI price;
    public Image menuImage;

    private string modalOption = "";

    private void Start() {
        // transform.gameObject.SetActive(false);
    }

    public void MenuModal() {
        CloseAll();
        transform.gameObject.SetActive(true);
        menuModal.SetActive(true);
    }

    public void CartModal()
    {
        CloseAll();
        transform.gameObject.SetActive(true);
        cartModal.SetActive(true);
        CartModalFunc cartModalScript = cartModal.GetComponent<CartModalFunc>();
        cartModalScript.OpenCartModal();
    }

    // 결제 방법 선택에서 뒤로가기 하면 나오는 화면 선택
    public void PaymentModalOption() {
        if (modalOption == "menu") {
            MenuModal();
        }
        else {
            CartModal();
        }
    }
    // 결제 방법 선택
    public void PaymentModal(string option, int totalPrice) {
        modalOption = option;
        CloseAll();
        transform.gameObject.SetActive(true);
        payModal.SetActive(true);

        PaymentModal payModalScript = payModal.GetComponent<PaymentModal>();
        payModalScript.setPaymentTotalPrice(totalPrice);
    }
    // 카드 선택
    public void CardModal()
    {
        CloseAll();
        transform.gameObject.SetActive(true);
        cardModal.SetActive(true);
        OpenPayComplete();
    }
    // 주문 완료
    public void PayCompleted()
    {
        CloseAll();
        transform.gameObject.SetActive(true);
        completed.SetActive(true);
        ReturnMain();
    }
    // 주문 종료 후 리셋
    public void ResetMain() {
        CartModalFunc cartModalScript = cartModal.GetComponent<CartModalFunc>();
        cartModalScript.ResetCart();
        CloseModal();
        SceneManager.LoadScene("Start");
    }
    // 모든 모달 끄기
    public void CloseAll() {
        menuModal.SetActive(false);
        cartModal.SetActive(false);
        payModal.SetActive(false);
        cardModal.SetActive(false);
        completed.SetActive(false);
    }

    public void CloseModal() {
        CloseAll();
        transform.gameObject.SetActive(false);
    }

    // 딜레이
    private IEnumerator ExecuteAfterDelay(Action action) {
        yield return new WaitForSeconds(5);
        action();
    }

    public void OpenPayComplete() {
        StartCoroutine(ExecuteAfterDelay(PayCompleted));
    }

    public void ReturnMain() {
        StartCoroutine(ExecuteAfterDelay(ResetMain));
    }

    public void UpdateMenuData(MenuData menuData)
    {
        title.text = menuData.name; //메뉴 이름
        price.text = string.Format("{0:n0}원", menuData.price);
        LoadImage(menuData.img); //이미지 로드
    }

    private void LoadImage(string url)
    {
        StartCoroutine(LoadImageCoroutine(url));
    }

    private IEnumerator LoadImageCoroutine(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                menuImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                menuImage.SetNativeSize();
            }
        }
    }
}
