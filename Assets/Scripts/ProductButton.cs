// using UnityEngine;
//
// public class ProductButton : MonoBehaviour
// {
//     public string productName;  // 제품 이름
//     public int price;           // 가격
//     public CartManager cartManager;  // 장바구니 매니저 참조
//     public UIModal modal;
//
//     // Amount 클래스를 참조하여 수량을 가져옴
//     public Amount amountManager;
//
//
//     private void Start()
//     {
//
//         cartManager = FindObjectOfType<CartManager>(); // CartManager 참조
//     }
//
//     public void OnAddToCartButtonClicked()
//     {
//         // Amount 클래스로부터 현재 수량 가져오기
//         int quantity = amountManager != null ? amountManager.amountText != null ? int.Parse(amountManager.amountText.text) : 1 : 1;
//
//         // 선택된 옵션들 가져오기 - string으로 변환하기 !! 수정
//         string testoptions = "ICE" + " 샷추가"; 
//         
//         Debug.Log(name+ quantity+ price + testoptions);
//         
//         // 장바구니에 제품 추가
//         // cartManager.AddToCart(name, quantity, price, testoptions);
//         modal.CloseModal();
//     }
// }
