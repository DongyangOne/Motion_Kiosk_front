using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CategoryData {
    public string categoryName { get; set; }
    public List<MenuData> menu { get; set; }
}

public class MenuData {
    public string name { get; set; }
    public int price { get; set; }
    public string imageURL { get; set; }
}


public class CategoryDataManager : MonoBehaviour {
    // 원래 존제하는 오브젝트
    public GameObject modalObject;
    
    // 만들어질 오브젝트 컴포넌트
    public GameObject categoryPage;
    public GameObject menuPage;
    public GameObject menuCell;
    public GameObject emptyMenuCell;

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

    private GameObject CreateEmptyCell(GameObject parent) {
        GameObject newObject = Instantiate(emptyMenuCell, parent.transform);
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

    private GameObject CreateMenuCell(GameObject parent, MenuData cellData) {
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
                        modalController.OpenModal(cellData.name, cellData.price, cellData.imageURL);
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
            Sprite newSprite = Resources.Load<Sprite>("Menu/" + cellData.imageURL);
            if (newSprite != null)
            {
                imageComponent.sprite = newSprite;
            }
            else
            {
                Debug.LogError("이미지를 찾을 수 없습니다: " + "Menu/americano");
            }
        }

        // 이름 변경
        GameObject menuName = newObject.transform.Find("MenuName").gameObject;
        UpdateText(menuName, cellData.name);
        
        // 가격 변경
        GameObject menuPrice = newObject.transform.Find("MenuPrice").gameObject;
        UpdateText(menuPrice, cellData.price.ToString() + "원");
        
        return newObject;
    }

    public void addCategory(List<CategoryData> categoryList) {
        // 데이터 불러오는 부분으로 수정해야함
        CategoryData coffee = new CategoryData {
            categoryName = "커피",
            menu = new List<MenuData>()
        };
        coffee.menu.AddRange(
            new MenuData[] {
                new MenuData { name = "아메리카노", price = 4500, imageURL = "americano" },
                new MenuData { name = "에스프레소", price = 4000, imageURL = "espresso" },
                new MenuData { name = "카페라떼", price = 4500, imageURL = "cafelatte" },
                new MenuData { name = "카페모카", price = 5000, imageURL = "cafemocha" },
                new MenuData { name = "바닐라라떼", price = 5000, imageURL = "vanillalatte" }
            }
        );
        categoryList.Add(coffee);
        
        
        CategoryData tea = new CategoryData {
            categoryName = "차",
            menu = new List<MenuData>()
        };
        tea.menu.AddRange(
            new MenuData[] {
                new MenuData { name = "케모마일", price = 2000, imageURL = "chamomile" },
                new MenuData { name = "얼그레이", price = 2000, imageURL = "earlgrey" },
                new MenuData { name = "쟈스민", price = 2000, imageURL = "jasmine" },
                new MenuData { name = "페퍼민트", price = 1500, imageURL = "peppermint" }
            }
        );
        
        categoryList.Add(tea);
        
        CategoryData drink = new CategoryData {
            categoryName = "과일음료",
            menu = new List<MenuData>()
        };
        drink.menu.AddRange(
            new MenuData[] {
                new MenuData { name = "자몽", price = 2000, imageURL = "grapefruit" },
                new MenuData { name = "자몽얼그레이", price = 2000, imageURL = "grapefruitearlgrey" },
                new MenuData { name = "허니레몬", price = 2000, imageURL = "honeylemon" },
                new MenuData { name = "레몬", price = 1500, imageURL = "lemon" },
                new MenuData { name = "녹차라떼", price = 1500, imageURL = "greentealatte" },
            }
        );
        categoryList.Add(drink);
        
        CategoryData smoothie = new CategoryData {
            categoryName = "스무디",
            menu = new List<MenuData>()
        };
        smoothie.menu.AddRange(
            new MenuData[] {
                new MenuData { name = "망고", price = 2000, imageURL = "mango" },
                new MenuData { name = "딸기", price = 2000, imageURL = "strawberry" },
                new MenuData { name = "요거트", price = 2000, imageURL = "yogurt" },
            }
        );
        categoryList.Add(smoothie);
        
        CategoryData dessert = new CategoryData {
            categoryName = "디져트",
            menu = new List<MenuData>()
        };
        dessert.menu.AddRange(
            new MenuData[] {
                new MenuData { name = "디져트1", price = 2000, imageURL = "dessert1" },
                new MenuData { name = "디져트2", price = 2000, imageURL = "dessert2" },
                new MenuData { name = "디져트3", price = 2000, imageURL = "dessert3" },
                new MenuData { name = "디져트4", price = 2000, imageURL = "dessert4" },
            }
        );
        categoryList.Add(dessert);
        CategoryData dessert2 = new CategoryData {
            categoryName = "디져트",
            menu = new List<MenuData>()
        };
        dessert.menu.AddRange(
            new MenuData[] {
                new MenuData { name = "디져트1", price = 2000, imageURL = "dessert1" },
                new MenuData { name = "디져트2", price = 2000, imageURL = "dessert2" },
                new MenuData { name = "디져트3", price = 2000, imageURL = "dessert3" },
                new MenuData { name = "디져트4", price = 2000, imageURL = "dessert4" },
            }
        );
        categoryList.Add(dessert);
        CategoryData dessert3 = new CategoryData {
            categoryName = "디져트",
            menu = new List<MenuData>()
        };
        dessert.menu.AddRange(
            new MenuData[] {
                new MenuData { name = "디져트1", price = 2000, imageURL = "dessert1" },
                new MenuData { name = "디져트2", price = 2000, imageURL = "dessert2" },
                new MenuData { name = "디져트3", price = 2000, imageURL = "dessert3" },
                new MenuData { name = "디져트4", price = 2000, imageURL = "dessert4" },
            }
        );
        categoryList.Add(dessert);
    }
    
    void Start() {

        // 데이터 불러오는 부분으로 수정해야함
        List<CategoryData> categoryList = new List<CategoryData>();
        addCategory(categoryList);

        for (int i = 0; i < categoryList.Count; i++) {
            GameObject newCategoryPage = CreateCategoryPage(categoryList[i].categoryName);
            GameObject menuSlide = newCategoryPage.transform.Find("MenuSlide").gameObject;
            GameObject menuContainer = menuSlide.transform.Find("MenuContainer").gameObject;

            int menuPageCount = 0;
            GameObject newMenuPage = CreateMenuPage(menuContainer, categoryList[i].categoryName + menuPageCount);
            for (int j = 0; j < categoryList[i].menu.Count; j++) {
                if (j % 4 == 0 && j != 0) {
                    menuPageCount++;
                    newMenuPage = CreateMenuPage(menuContainer, categoryList[i].categoryName + (menuPageCount));
                }
                // Instantiate(MenuCell, newMenuPage.transform);
                //
                CreateMenuCell(newMenuPage, categoryList[i].menu[j]);
            }

            // EmptyCell 보충
            int remainder = categoryList[i].menu.Count % 4;
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
        
        Debug.Log("category:"+categoryRect.localPosition);
        Debug.Log("pos:"+categoryRect.position);

        UICarousel categoryCarousel = transform.GetComponent<UICarousel>();
        categoryCarousel.ResetCarousel();
    }
}
