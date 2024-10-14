using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;


public class OptionData {
    public List<string> options { get; set; }
}


public class OptionDataManager : MonoBehaviour {

    public GameObject OptionSlide;
    public GameObject OptionPage;
    public GameObject OptionRow;
    public GameObject OptionButton;
    public GameObject OptionLine;





    private GameObject CreateOptionPage() {
        Transform optionContainer = OptionSlide.transform.Find("OptionContainer");
        GameObject newObject = Instantiate(OptionPage, optionContainer);
        return newObject;
    }

    private GameObject CreateOptionRow(GameObject parent) {
        GameObject newObject = Instantiate(OptionRow, parent.transform);
        Instantiate(OptionLine, parent.transform);
        return newObject;
    }

    private void UpdateText(GameObject textBox, string text) {
        if (textBox != null) {
            TextMeshProUGUI textComponent = textBox.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = text;
            }
        }
    }
    
    private GameObject CreateOptionButton(GameObject parent, string optionName) {
        GameObject newObject = Instantiate(OptionButton, parent.transform);
        newObject.name = optionName;
        
        // 이름 변경
        GameObject optionNameObj = newObject.transform.Find("OptionName").gameObject;
        UpdateText(optionNameObj, optionName);
        
        return newObject;
    }

  
    void Start() {

        // 데이터 불러오는 부분임 
        List<OptionData> optionData = new List<OptionData>();
        optionData.AddRange(
            new OptionData[] {
                new OptionData {options = new List<string>{"ICE", "HOT"}},
        });

        GameObject newOptionPage = CreateOptionPage();
        for (int i = 0; i < optionData.Count; i++) {
            if (i % 3 == 0 && i != 0) {
                newOptionPage = CreateOptionPage();
            }
            GameObject newOptionRow = CreateOptionRow(newOptionPage);
            for (int j = 0; j < optionData[i].options.Count; j++) {
                CreateOptionButton(newOptionRow, optionData[i].options[j]);
            }
        }
        
            
        Transform optionContainer = OptionSlide.transform.Find("OptionContainer");
        RectTransform navContainerRect = optionContainer.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(navContainerRect);
        
        UICarousel optionCarousel = OptionSlide.GetComponent<UICarousel>();
        optionCarousel.ResetCarousel();
    }

}
