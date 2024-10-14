using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PaymentModal : MonoBehaviour {
    public Transform totalPrice;
    public UIModal modal;
    public void setPaymentTotalPrice(int price) {
        TextMeshProUGUI priceText = totalPrice.GetComponent<TextMeshProUGUI>();
        
        priceText.text = "총 가격: " + price.ToString() + "원";
    }

    public void SelectCash() {
        modal.PayCompleted();
    }
}
