using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIModal : MonoBehaviour {
    public Image menuImage;
    public TextMeshProUGUI title;
    public TextMeshProUGUI price;
    public GameObject OptionSlide;
    public GameObject cartModal;
    public GameObject payModal;
    public GameObject cardModal;
    public GameObject completed;

    private void Start() {
        transform.gameObject.SetActive(false);
    }

    public void OpenModal(string menuName, int menuPrice, string imageURL) {
        ResetModal();
        
        // UICarousel optionCarousel = OptionSlide.GetComponent<UICarousel>();
        // if (optionCarousel != null)
        // {
        //     optionCarousel.SetPage(1);
        // }
        //
        
        transform.gameObject.SetActive(true);

        title.text = menuName;
        price.text = menuPrice.ToString();
        Sprite image = Resources.Load<Sprite>("Menu/" + imageURL);
        if (image != null) {
            menuImage.sprite = image;
        }
    }

    public void CartModal()
    {
        cartModal.SetActive(true);
        payModal.SetActive(false);
    }

    public void PaymentModal()
    {
        payModal.SetActive(true);
        cartModal.SetActive(false);
    }

    public void CardModal()
    {
        cardModal.SetActive(true);
        payModal.SetActive(false);
    }

    public void PayCompleted()
    {
        completed.SetActive(true);
        cardModal.SetActive(false);
    }

    public void CloseModal() {
        transform.gameObject.SetActive(false);
        cartModal.SetActive(false);
        payModal.SetActive(false);
        cardModal.SetActive(false);
        completed.SetActive(false);
    }

    public void ResetModal() {
        ModalReset[] resetComponents = transform.GetComponentsInChildren<ModalReset>(true);
        foreach (ModalReset components in resetComponents) {
            components.ModalOptionReset();
        }

    }
}