using UnityEngine;
using TMPro;

public class CartModalFunc : MonoBehaviour
{
    public GameObject cartContent; // 장바구니 항목이 표시될 UI 오브젝트 (컨텐츠)
    public GameObject cartItemPrefab; // 장바구니 항목을 표시할 UI 프리팹
    public TextMeshProUGUI totalPriceText; // 총 가격을 표시할 텍스트

    private CartManager cartManager;

    private void Start()
    {
        cartManager = FindObjectOfType<CartManager>(); // CartManager 찾기
        cartManager.CartUpdated += UpdateCartDisplay; // 장바구니 업데이트 시 호출되는 함수 등록
    }

    // 장바구니 내용을 화면에 표시
    public void UpdateCartDisplay()
    {
        // 기존 장바구니 UI 항목들 제거
        foreach (Transform child in cartContent.transform)
        {
            Destroy(child.gameObject);
        }

        // 장바구니 항목을 가져와 UI에 표시
        int totalPrice = 0;
        foreach (CartItem item in cartManager.GetCartItems())
        {
            GameObject cartItemObject = Instantiate(cartItemPrefab, cartContent.transform);

            // 장바구니 항목에 텍스트 업데이트
            TextMeshProUGUI[] texts = cartItemObject.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = item.name;
            texts[1].text = "수량: " + item.quantity.ToString();
            texts[2].text = "가격: " + (item.price * item.quantity).ToString();

            totalPrice += item.price * item.quantity; // 총 가격 계산
        }

        // 총 가격 업데이트
        totalPriceText.text = "총 가격: " + totalPrice.ToString() + "원";
    }
}
