using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMultiSelect : MonoBehaviour, ModalReset {
    private List<Button> buttons;

    public Color selectedBackground;
    public Color selectedColor;

    private string selectedOption = "";

    void Start() {
        buttons = new List<Button>();
        Button[] pageComponents = transform.GetComponentsInChildren<Button>();
        for (int i = 0;i<pageComponents.Length;i++) {
            int index = i;
            Button button = pageComponents[i];
            button.onClick.AddListener(()=>OptionSelect(index));
            buttons.Add(button);
        }
        OptionSelect(0);
    }

    private void OptionSelect(int index) {
        if (buttons == null || buttons.Count == 0) return;
        for (int i = 0; i < buttons.Count; i++) {
            Button button = buttons[i];
            ColorBlock cb = button.colors;
            
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            
            if (i != index) {
                cb.normalColor = Color.white;
                cb.pressedColor = Color.white;
                cb.selectedColor = Color.white;
                cb.highlightedColor = Color.white;
                buttonText.color = Color.black;
            } else {
                // 선택된거
                cb.normalColor = selectedBackground;
                cb.pressedColor = selectedBackground;
                cb.selectedColor = selectedBackground;
                cb.highlightedColor = selectedBackground;
                buttonText.color = selectedColor;
                selectedOption = buttonText.text;
                Debug.Log("Selected" +selectedOption);
            }
            button.colors = cb;
        }
    }

    public string GetSelected() {
        return selectedOption;
    }

    public void ModalOptionReset() {
        OptionSelect(0);
    }
    
}

