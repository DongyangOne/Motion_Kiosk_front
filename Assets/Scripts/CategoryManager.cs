using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    public GameObject categoryContainer; // 카테고리들이 포함된 부모 객체

    // 카테고리를 보여주는 메서드
    public void ShowCategory(string category)
    {
        // 모든 카테고리 비활성화
        foreach (Transform child in categoryContainer.transform)
        {
            child.gameObject.SetActive(false);
        }

        // 주어진 카테고리에 따라 해당 페이지를 활성화합니다.
        switch (category.ToLower())
        {
            case "coffee":
                categoryContainer.transform.Find("CategoryPageCoffee").gameObject.SetActive(true);
                break;
            case "tea":
                categoryContainer.transform.Find("CategoryPageTea").gameObject.SetActive(true);
                break;
            case "noncoffee":
                categoryContainer.transform.Find("CategoryPageNonCoffee").gameObject.SetActive(true);
                break;
            case "dessert":
                categoryContainer.transform.Find("CategoryPageDessert").gameObject.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown category: " + category);
                break;
        }
    }
}
