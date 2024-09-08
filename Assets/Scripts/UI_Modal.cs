using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIModal : MonoBehaviour {
    public Image menuImage;
    public TextMeshProUGUI title;
    public TextMeshProUGUI price;

    private void Start() {
        transform.gameObject.SetActive(false);
    }

    public void OpenModal(string menuName) {
        Debug.Log("OpenModal");
        ResetModal();
        
        transform.gameObject.SetActive(true);

        title.text = menuName;
        Sprite image = Resources.Load<Sprite>("Menu/" + menuName);
        menuImage.sprite = image;
    }

    public void CloseModal() {
        transform.gameObject.SetActive(false);
    }

    public void ResetModal() {
        ModalReset[] resetComponents = transform.GetComponentsInChildren<ModalReset>(true);
        foreach (ModalReset components in resetComponents) {
            components.ModalOptionReset();
        }
    }
}
