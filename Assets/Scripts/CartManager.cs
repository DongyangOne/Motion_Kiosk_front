// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// [System.Serializable]
// // public class CartItem
// // {
// //     public string name;
// //     public int quantity;
// //     public int price;
// //     public string options;
// //     
// //     public CartItem(string name, int quantity, int price, string options)
// //     {
// //         this.name = name;
// //         this.quantity = quantity;
// //         this.price = price;
// //         this.options = options;
// //     }
// // }
//
// public class CartManager : MonoBehaviour
// {
//     private List<CartItem> cartItems = new List<CartItem>();
//
//
//     public delegate void OnCartUpdated();
//     public event OnCartUpdated CartUpdated;
//
//     public void AddToCart(string name, int quantity, int price, string options, string img)
//     {
//         Debug.Log(quantity);
//         CartItem existingItem = cartItems.Find(item => item.name == name);
//
//         if (existingItem != null)
//         {
//             // 이미 장바구니에 존재하는 항목은 수량만 증가시킴
//             existingItem.quantity += quantity;
//         }
//         else
//         {
//             // 새로운 항목을 추가
//             CartItem newItem = new CartItem(name, quantity, price, options, img);
//             cartItems.Add(newItem);
//         }
//
//         // 장바구니 업데이트 이벤트 호출
//         CartUpdated?.Invoke();
//     }
//
//     internal void AddToCart(string name, int quantity, object price)
//     {
//         throw new NotImplementedException();
//     }
//
//     public List<CartItem> GetCartItems()
//     {
//         return cartItems;
//     }
//     
//
// }
