using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Amount : MonoBehaviour, ModalReset
{
    private int amount;

    public TextMeshProUGUI amountText;

    private void Start()
    {
        amount = 1;
    }

    public void PlusButton()
    {
        if (amount >= 100) return;
        amount++;
        UpdateAmount();
    }

    public void MinusButton()
    {
        if (amount <= 1) return;
        amount--;
        UpdateAmount();
    }

    private void UpdateAmount()
    {
        amountText.text = amount.ToString();
    }

    public void ModalOptionReset()
    {
        amount = 1;
        UpdateAmount();
    }

}
