using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;
using System;


    
[System.Serializable]
public class CartItem
{
    public string name;
    public int quantity;
    public int price;
    public string options;
    public string img;
    
    public CartItem(string name, int quantity, int price, string options, string img)
    {
        this.name = name;
        this.quantity = quantity;
        this.price = price;
        this.options = options;
        this.img = img;
    }
}

public class CartModalFunc : MonoBehaviour
{
    public GameObject cartContent; // 장바구니 항목이 표시될 UI 오브젝트 (컨텐츠)
    public GameObject cartItemPrefab; // 장바구니 항목을 표시할 UI 프리팹
    public TextMeshProUGUI totalPriceText; // 총 가격을 표시할 텍스트
    public ScrollRect scrollView; // 장바구니 항목 스크롤
    public MenuDataLoader dataLoader;

    public UIModal modal;

    private Button payBtn;
    
    private List<CartItem> cartItems = new List<CartItem>();
    private int totalPrice;
    
    public void OpenCartModal() {
        scrollView.verticalNormalizedPosition = 1f;
    }

    
    // private CartManager cartManager;

    private void Start()
    {
        // cartManager = FindObjectOfType<CartManager>(); // CartManager 찾기
        // cartManager.CartUpdated += UpdateCartDisplay; // 장바구니 업데이트 시 호출되는 함수 등록
        Transform pay = transform.Find("Footer/Pay");
        payBtn = pay.GetComponent<Button>();
        payBtn.onClick.AddListener(() => {
            if (cartItems.Count != 0) {
                modal.PaymentModal("cart", totalPrice);
            }
        });
    }

    public void ResetCart() {
        cartItems = new List<CartItem>();
        UpdateCartDisplay();
    }

    private void ChangeText(GameObject textbox, string newText) {
        TextMeshProUGUI text = textbox.GetComponent<TextMeshProUGUI>();

        text.text = newText;
    }

    // 장바구니 내용을 화면에 표시
    public void UpdateCartDisplay()
    {
        Debug.Log("update");
        
        
        
        // 선택 상품이 없으면 결제 버튼 비활성화
        if (payBtn != null) {
            if (cartItems.Count == 0) {
                payBtn.interactable = false;
            }
            else {
                payBtn.interactable = true;
            }
        }
        
        
        // 기존 장바구니 UI 항목들 제거
        Transform cartTransform = cartContent.transform;
        // 자식의 목록을 저장
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in cartTransform)
        {
            children.Add(child.gameObject);
        }
        // 저장한 목록을 이용해 삭제
        foreach (GameObject child in children)
        {
            Destroy(child);
        }
        
        

        // 장바구니 항목을 가져와 UI에 표시
        foreach (CartItem item in GetCartItems())
        {
            GameObject cartItemObject = Instantiate(cartItemPrefab, cartContent.transform);

            // 장바구니 항목 정보 추가
            
            // 이미지 변경
            Transform imageTransform = cartItemObject.transform.Find("Contents/ImageQuantity/Background/Image");
            Image image = imageTransform.GetComponent<Image>();
            dataLoader.LoadImage(item.img, image);
            
            // 메뉴 이름 변경
            Transform name = cartItemObject.transform.Find("Contents/Texts/Name");
            ChangeText(name.GameObject(), item.name);
            
            // 옵션 변경
            Transform options = cartItemObject.transform.Find("Contents/Texts/Options");
            ChangeText(options.GameObject(), item.options);
            
            // 가격 변경
            Transform price = cartItemObject.transform.Find("Contents/Texts/Price");
            ChangeText(price.GameObject(), "₩ "+item.price.ToString());
            
            // 개수 변경
            Transform quantity = cartItemObject.transform.Find("Contents/ImageQuantity/Background/QuantitySelect/Quantity");
            ChangeText(quantity.GameObject(), item.quantity.ToString());
            
            // 메뉴 삭제 버튼
            Transform delete = cartItemObject.transform.Find("Contents/DeleteButton");
            Button deleteBtn = delete.GetComponent<Button>();
            deleteBtn.onClick.AddListener(() => {
                RemoveFromCart(item.name, item.options);
            });
            
            
            // 개수 변경 버튼
            Transform minus = cartItemObject.transform.Find("Contents/ImageQuantity/Background/QuantitySelect/Minus");
            Button minusBtn = minus.GetComponent<Button>();
            minusBtn.onClick.AddListener(() => {
                Debug.Log(-1);
                if (item.quantity >= 2) {
                    // 개수 업데이트
                    item.quantity--;
                    ChangeText(quantity.GameObject(), item.quantity.ToString());
                    
                    // 총 가격 업데이트
                    totalPrice -= item.price;
                    totalPriceText.text = "총 가격: " + totalPrice.ToString() + "원";
                }
            });
            
            Transform plus = cartItemObject.transform.Find("Contents/ImageQuantity/Background/QuantitySelect/Plus");
            Button plusBtn = plus.GetComponent<Button>();
            plusBtn.onClick.AddListener(() => {
                Debug.Log(1);
                if (item.quantity <= 98) {
                    // 개수 업데이트
                    item.quantity++;
                    ChangeText(quantity.GameObject(), item.quantity.ToString());
                    
                    // 총 가격 업데이트
                    totalPrice += item.price;
                    totalPriceText.text = "총 가격: " + totalPrice.ToString() + "원";
                }
            });
            
            
            totalPrice += item.price * item.quantity;
        }
        
        // 아이템 추가 후 스크롤 상단으로
        scrollView.verticalNormalizedPosition = 1f;

        // 총 가격 업데이트
        totalPriceText.text = "총 가격: " + totalPrice.ToString() + "원";
    }

    
    
    // cartManager 가져옴
    public void AddToCart(string name, int quantity, int price, string options, string img)
    {
        Debug.Log("add");
        CartItem existingItem = cartItems.Find(item => item.name == name && item.options == options);

        if (existingItem != null)
        {
            // 이미 장바구니에 존재하는 항목은 수량만 증가시킴
            existingItem.quantity += quantity;
        }
        else
        {
            // 새로운 항목을 추가
            CartItem newItem = new CartItem(name, quantity, price, options, img);
            cartItems.Add(newItem);
        }

        // 장바구니 업데이트 이벤트 호출
        UpdateCartDisplay();
    }

    public void RemoveFromCart(string name, string options) {
        Debug.Log("remove");
        cartItems.RemoveAll(item => item.name == name && item.options == options);
        UpdateCartDisplay();
    }

    public List<CartItem> GetCartItems()
    {
        return cartItems;
    }
    
        
}
