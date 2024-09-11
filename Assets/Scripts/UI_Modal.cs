using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIModal : MonoBehaviour
{
    public Image menuImage;
    public TextMeshProUGUI title;
    public TextMeshProUGUI price;
    public GameObject OptionSlide;

    public void OpenModal(string menuName, int menuPrice, string imageURL)
    {
        ResetModal();
        transform.gameObject.SetActive(true);

        title.text = menuName;
        price.text = menuPrice.ToString();
        Sprite image = Resources.Load<Sprite>("Menu/" + imageURL);
        if (image != null)
        {
            menuImage.sprite = image;
        }
    }

    public void CloseModal()
    {
        transform.gameObject.SetActive(false);
    }

    public void ResetModal()
    {
        ModalReset[] resetComponents = transform.GetComponentsInChildren<ModalReset>(true);
        foreach (ModalReset components in resetComponents)
        {
            components.ModalOptionReset();
        }
    }
}
