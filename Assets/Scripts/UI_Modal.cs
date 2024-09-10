using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIModal : MonoBehaviour {
    public Image menuImage;
    public GameObject cartModal;
    public TextMeshProUGUI title;
    public TextMeshProUGUI price;
    public GameObject OptionSlide;

    private void Start() {
        gameObject.SetActive(false);
    }

    public void OpenModal(string menuName, int menuPrice, string imageURL) {
        ResetModal();
        // UICarousel optionCarousel = OptionSlide.GetComponent<UICarousel>();
        // if (optionCarousel != null) {
        //     optionCarousel.SetPage(1);
        // }

        gameObject.SetActive(true);

        title.text = menuName;
        price.text = menuPrice.ToString();
        Sprite image = Resources.Load<Sprite>("Menu/" + imageURL);
        if (image != null) {
            menuImage.sprite = image;
        } else {
            Debug.LogWarning("Image not found: " + imageURL);
        }
    }

    public void OpenCart() {
        cartModal.SetActive(true);
    }

    public void CloseModal() {
        gameObject.SetActive(false);
        cartModal.SetActive(false);
    }

    public void ResetModal() {
        ModalReset[] resetComponents = GetComponentsInChildren<ModalReset>(true);
        foreach (ModalReset component in resetComponents) {
            component.ModalOptionReset();
        }
    }
}
