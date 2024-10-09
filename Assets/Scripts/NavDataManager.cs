using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NavDataManager : MonoBehaviour {

    public GameObject navPage;
    public GameObject navOption;
    public GameObject emptyNavOption;
    public GameObject categorySlide;
    
    
    
    // object 만드는 함수
    private GameObject CreateNavPage(string objectName) {
        Transform navContainer = transform.Find("NavContainer");
        GameObject newObject = Instantiate(navPage, navContainer);
        newObject.name = "NavPage" + objectName;
        return newObject;
    }

    private GameObject CreateEmptyCell(GameObject parent) {
        GameObject newObject = Instantiate(emptyNavOption, parent.transform);
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
    private GameObject CreateNavOption(GameObject parent, string objectName, int index) {
        GameObject newObject = Instantiate(navOption, parent.transform);
        newObject.name = "NavOption" + objectName;
        
        Button button = newObject.GetComponent<Button>();
        if (button != null) {
            button.onClick.AddListener(()=>{
                if (categorySlide != null)
                {
                    UICarousel categoryCarousel = categorySlide.GetComponent<UICarousel>();
                    if (categoryCarousel != null)
                    {
                        categoryCarousel.SetPage(index+1);
                    }
                    else
                    {
                        Debug.LogError("categoryCarousel 컴포넌트를 찾을 수 없습니다.");
                    }
                }
                else
                {
                    Debug.LogError("categorySlide가 할당되지 않았습니다.");
                }});
        }
        
        // 내용 변경
        GameObject navOptionName = newObject.transform.Find("NavOptionName").gameObject;
        UpdateText(navOptionName, objectName);

        
        return newObject;
    }

    public void NavRender(Dictionary<string, CategoryData> categoryDict) {
        List<string> categoryList = new List<string>(categoryDict.Keys);
        
        int navPageCount = 0;
        GameObject newNavPage = CreateNavPage(navPageCount.ToString());
        for (int i = 0; i < categoryList.Count; i++) {
            if (i % 3 == 0 & i != 0) {
                navPageCount++;
                newNavPage = CreateNavPage(navPageCount.ToString());
            }
            CreateNavOption(newNavPage, categoryList[i], i);
            
        }
        
        // EmptyCell 보충
        int remainder = categoryList.Count % 3;
        if (remainder != 0) {
            int differ = 3 - remainder;
            for (int j = 0; j < differ; j++) {
                CreateEmptyCell(newNavPage);
            }
        }
        
        Transform navContainer = transform.Find("NavContainer");
        RectTransform navContainerRect = navContainer.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(navContainerRect);
        
        UICarousel navCarousel = transform.GetComponent<UICarousel>();
        navCarousel.ResetCarousel();
    }

}
