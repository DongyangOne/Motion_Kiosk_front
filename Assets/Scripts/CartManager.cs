using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CartManager : MonoBehaviour {
    public GameObject cartItemPrefab;
    public Transform cartContainer;
    public UIMultiSelect multiSelect;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI totalText;

    public void AddSelectedToCart() {
        List<string> selectedOptions = multiSelect.GetSelectedOptions();

        string combinedOptions = string.Join(", ", selectedOptions);

        GameObject cartItem = Instantiate(cartItemPrefab, cartContainer);
        TextMeshProUGUI textComponent = cartItem.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null) 
        {
            textComponent.text = combinedOptions;
        }

        CalculateAmount();
    }

    private void CalculateAmount() 
    {
        string priceString = priceText.text;

        int price;

        bool isPriceValid = int.TryParse(priceString, out price);

        if (isPriceValid) 
        {
            int totalAmount = 0;
            foreach (Transform child in cartContainer) 
            {
                TextMeshProUGUI itemText = child.GetComponentInChildren<TextMeshProUGUI>();
                if (itemText != null) 
                {
                    int itemPrice;
                    if (int.TryParse(priceText.text, out itemPrice)) 
                    {
                        totalAmount += itemPrice;
                    }
                }
            }
            string amountFormatted = totalAmount.ToString("N0") + " 원";
            totalText.text = "총 금액: " + amountFormatted;
        } else {
            totalText.text = "금액 계산 오류";
        }
    }
}
