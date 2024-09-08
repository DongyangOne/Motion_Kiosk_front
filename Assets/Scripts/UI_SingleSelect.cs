using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISingleSelect : MonoBehaviour, ModalReset {

    public Color selectedBackground;
    public Color selectedColor;
    private Button button;
    private bool isSelected;
    
    private void Start(){
        button = transform.GetComponent<Button>();
        button.onClick.AddListener(()=>OptionSelect());
        isSelected = false;
    }
    
    private void OptionSelect() {
        if (button == null) return;
        ColorBlock cb = button.colors;
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (isSelected) {
            cb.normalColor = Color.white;
            cb.pressedColor = Color.white;
            cb.selectedColor = Color.white;
            cb.highlightedColor = Color.white;
            buttonText.color = Color.black;
        } else {
            cb.normalColor = selectedBackground;
            cb.pressedColor = selectedBackground;
            cb.selectedColor = selectedBackground;
            cb.highlightedColor = selectedBackground;
            buttonText.color = selectedColor;
        }
        button.colors = cb;
        isSelected = !isSelected;
    }
    
    
    public void ModalOptionReset() {
        isSelected = true;
        OptionSelect();
    }

}
