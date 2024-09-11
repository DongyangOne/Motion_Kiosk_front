using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMultiSelect : MonoBehaviour, ModalReset {
    private List<Button> buttons;
    private HashSet<string> selectedOptions = new HashSet<string>();

    public Color selectedBackground;
    public Color selectedColor;
    public Color unselectedBackground = Color.white;
    public Color unselectedColor = Color.black;

    void Start() {
        buttons = new List<Button>();
        Button[] pageComponents = transform.GetComponentsInChildren<Button>();
        for (int i = 0;i<pageComponents.Length;i++) {
            int index = i;
            Button button = pageComponents[i];
            button.onClick.AddListener(() => ToggleOptionSelect(index));
            buttons.Add(button);
        }
        UpdateButtonStates();
    }

    private void ToggleOptionSelect(int index) {
        if (buttons == null || buttons.Count == 0) return;
        
        Button button = buttons[index];
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        ColorBlock cb = button.colors;
        
        if (selectedOptions.Contains(buttonText.text)) 
        {
            //이미 선택된 상태라면 선택 해제
            selectedOptions.Remove(buttonText.text);
            cb.normalColor = unselectedBackground;
            cb.pressedColor = unselectedBackground;
            cb.selectedColor = unselectedBackground;
            cb.highlightedColor = unselectedBackground;
            buttonText.color = unselectedColor;
        } else {
            //선택되지 않은 상태라면 선택
            selectedOptions.Add(buttonText.text);
            cb.normalColor = selectedBackground;
            cb.pressedColor = selectedBackground;
            cb.selectedColor = selectedBackground;
            cb.highlightedColor = selectedBackground;
            buttonText.color = selectedColor;
        }
        button.colors = cb;
    }

    public void ModalOptionReset() {
        selectedOptions.Clear();
        UpdateButtonStates();
    }

    private void UpdateButtonStates() {
        if (buttons == null) return;
        for (int i = 0; i < buttons.Count; i++) {
            Button button = buttons[i];
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            ColorBlock cb = button.colors;

            if (selectedOptions.Contains(buttonText.text)) 
            {
                cb.normalColor = selectedBackground;
                cb.pressedColor = selectedBackground;
                cb.selectedColor = selectedBackground;
                cb.highlightedColor = selectedBackground;
                buttonText.color = selectedColor;
            } else {
                cb.normalColor = unselectedBackground;
                cb.pressedColor = unselectedBackground;
                cb.selectedColor = unselectedBackground;
                cb.highlightedColor = unselectedBackground;
                buttonText.color = unselectedColor;
            }
            button.colors = cb;
        }
    }

    public List<string> GetSelectedOptions() {
        return new List<string>(selectedOptions);
    }
}
