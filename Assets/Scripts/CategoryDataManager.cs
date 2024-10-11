using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryDataManager : MonoBehaviour {
    public MenuDataLoader dataLoader;
    public NavDataManager navManager;
    
    // 원래 존제하는 오브젝트
    public GameObject modalObject;
    
    // 만들어질 오브젝트 컴포넌트
    public GameObject categoryPage;
    public GameObject menuPage;
    public GameObject menuCell;
    public GameObject emptyMenuCell;
    
    // object 만드는 함수
    private GameObject CreateCategoryPage(string objectName) {
        Transform categoryContainer = transform.Find("CategoryContainer");
        GameObject newObject = Instantiate(categoryPage, categoryContainer);
        newObject.name = "CategoryPage" + objectName;
        return newObject;
    }
    private GameObject CreateMenuPage(GameObject parent, string objectName) {
        GameObject newObject = Instantiate(menuPage, parent.transform);
        newObject.name =  "MenuPage" + objectName;
        return newObject;
    }

    private void CreateEmptyCell(GameObject parent) {
        Instantiate(emptyMenuCell, parent.transform);
        // GameObject newObject = Instantiate(emptyMenuCell, parent.transform);
        // return newObject;
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

    private void CreateMenuCell(GameObject parent, MenuData cellData) {
        GameObject newObject = Instantiate(menuCell, parent.transform);
        newObject.name = cellData.name;
        
        Button button = newObject.GetComponent<Button>();
        if (button != null) {
            button.onClick.AddListener(()=>{
                if (modalObject != null)
                {
                    UIModal modalController = modalObject.GetComponent<UIModal>();
                    if (modalController != null)
                    {
                        //image
                        modalController.OpenModal(cellData.name, cellData.price, cellData.img);
                    }
                    else
                    {
                        Debug.LogError("ModalController 컴포넌트를 찾을 수 없습니다.");
                    }
                }
                else
                {
                    Debug.LogError("ModalObject가 할당되지 않았습니다.");
                }});
        }

        // 이미지 변경
        Image imageComponent = newObject.transform.Find("MenuImage/Image").GetComponent<Image>();
        if (imageComponent != null)
        {
            //image
            dataLoader.LoadImage(cellData.img, imageComponent);
        }

        // 이름 변경
        GameObject menuName = newObject.transform.Find("MenuName").gameObject;
        UpdateText(menuName, cellData.name);
        
        // 가격 변경
        GameObject menuPrice = newObject.transform.Find("MenuPrice").gameObject;
        UpdateText(menuPrice, cellData.price.ToString() + "원");
        
        // return newObject;
    }

    
    
    private void CategoryRender(Dictionary<string, CategoryData> categoryDict) {
        foreach (string categoryName in categoryDict.Keys) {
            GameObject newCategoryPage = CreateCategoryPage(categoryName);
            GameObject menuSlide = newCategoryPage.transform.Find("MenuSlide").gameObject;
            GameObject menuContainer = menuSlide.transform.Find("MenuContainer").gameObject;

            int menuPageCount = 0;
            GameObject newMenuPage = CreateMenuPage(menuContainer, categoryName + menuPageCount);
            for (int j = 0; j < categoryDict[categoryName].menuList.Count; j++) {
                if (j % 4 == 0 && j != 0) {
                    menuPageCount++;
                    newMenuPage = CreateMenuPage(menuContainer, categoryName + (menuPageCount));
                }
                // Instantiate(MenuCell, newMenuPage.transform);
                //
                CreateMenuCell(newMenuPage, categoryDict[categoryName].menuList[j]);
            }

            // EmptyCell 보충
            int remainder = categoryDict[categoryName].menuList.Count % 4;
            if (remainder != 0) {
                int differ = 4 - remainder;
                for (int j = 0; j < differ; j++) {
                    CreateEmptyCell(newMenuPage);
                }
            }

            RectTransform menuRect = menuSlide.transform.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(menuRect);
            
            UICarousel menuCarousel = menuSlide.GetComponent<UICarousel>();
            menuCarousel.ResetCarousel();
        }

        RectTransform categoryRect = transform.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(categoryRect);

        Transform categoryContainer = transform.Find("CategoryContainer");
        RectTransform categoryContainerRect = categoryContainer.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(categoryContainerRect);

        UICarousel categoryCarousel = transform.GetComponent<UICarousel>();
        categoryCarousel.ResetCarousel();
        
        navManager.NavRender(categoryDict);
    }

    void Start() {
        dataLoader.OnDataLoaded += CategoryRender;
        dataLoader.LoadData();
    }

}