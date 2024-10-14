using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;


public class UIModal : MonoBehaviour {
    public GameObject menuModal;
    public GameObject cartModal;
    public GameObject payModal;
    public GameObject cardModal;
    public GameObject completed;

    private string modalOption = "";

    private void Start() {
        transform.gameObject.SetActive(false);
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
    
}