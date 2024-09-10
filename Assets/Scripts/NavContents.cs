using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NavContents : MonoBehaviour {
    private List<Button> buttons;
    private string[] optionList = { "커피", "에이드", "차", "스무디", "디저트", "샐러드" };
    
    public Color defaultColor;
    public Color selectedColor;
    
    void Start() {
        buttons = new List<Button>();
        List<Transform> pages = GetChildren(transform);
        for (int i = 0; i < pages.Count; i++) {
            
            Button[] pageComponents = pages[i].GetComponentsInChildren<Button>();
            for (int j = 0; j < pageComponents.Length; j++) {
                int index = (i * 3) + j;
                Button button = pageComponents[j];
                button.onClick.AddListener(()=>optionSelect(index));
                buttons.Add(button);
            }
        }
        
        optionSelect(0);
    }

    public List<Transform> GetChildren(Transform parent) {
        List<Transform> children = new List<Transform>();
        
        foreach(Transform child in parent)
        {
            children.Add(child);
        }
    
        return children;
    }

    public void optionSelect(int index) {
        if (buttons == null || buttons.Count == 0) return;
        for (int i = 0; i < buttons.Count; i++) {
            Button button = buttons[i];
            ColorBlock cb = button.colors;
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null) return;
            if (index == i) {
                cb.normalColor = selectedColor;
                cb.pressedColor = selectedColor;
                cb.selectedColor = selectedColor;
                cb.highlightedColor = selectedColor;
                buttonText.color = defaultColor;
            } else {
                cb.normalColor = defaultColor;
                cb.pressedColor = defaultColor;
                cb.selectedColor = defaultColor;
                cb.highlightedColor = defaultColor;
                buttonText.color = selectedColor;
            }
            button.colors = cb;
        }
        
    }
}
