
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Networking;



public class MenuModal : MonoBehaviour
{
    public UIModal modal;
    public CartModalFunc cartModal;
    public MenuDataLoader dataLoader;
    public UIMultiSelect multiSelect;
    
    public Image menuImage;
    public TextMeshProUGUI title;
    public TextMeshProUGUI price;

    public Button minusBtn;
    public Button plusBtn;
    public Transform quantityCount;
    public Button addCartBtn;
    public Button payBtn;
    
    
    // public GameObject OptionSlide;
    // public MenuDataLoader dataLoader;
    
    private int quantity = 1;
    private MenuData menuData;
    
    private void Start(){
        TextMeshProUGUI quantityText = quantityCount.GetComponent<TextMeshProUGUI>();
        // 버튼 이벤트
        // Quantity 버튼
        minusBtn.onClick.AddListener(() => {
            Debug.Log(quantity);
            if (quantity> 1) {
                quantity--;
                if (quantityText != null) {
                    quantityText.text = quantity.ToString();
                }
            }
        });
        plusBtn.onClick.AddListener(() => {
            Debug.Log(quantity);
            if (quantity<= 98) {
                quantity++;
                if (quantityText != null) {
                    quantityText.text = quantity.ToString();
                }
            }
        });
        
        
        // 카트, 결제 버튼
        addCartBtn.onClick.AddListener(() => {
            Debug.Log("AddBtn");
            string selectedOption = multiSelect.GetSelected();
            
            Debug.Log(selectedOption);
            cartModal.AddToCart(menuData.name, quantity, menuData.price, selectedOption, menuData.img);
            modal.CloseModal();
        });
        
        payBtn.onClick.AddListener(() => {
            modal.PaymentModal("menu", menuData.price);
        });}
    
    public void OpenModal(MenuData data) {
        modal.MenuModal();
        
        // data reset
        ModalReset[] resetComponents = transform.GetComponentsInChildren<ModalReset>(true);
        foreach (ModalReset components in resetComponents) {
            components.ModalOptionReset();
        }
        
        
        menuData = data;
        quantity = 1;
        title.text = menuData.name;
        price.text = menuData.price.ToString();
        
        
        TextMeshProUGUI quantityText = quantityCount.GetComponent<TextMeshProUGUI>();
        if (quantityText != null) {
            quantityText.text = quantity.ToString();
        }
        
        // 이미지 변경
        if (menuImage != null)
        {
            //image
            dataLoader.LoadImage(menuData.img, menuImage);
        }
        
        
         // foreach (string option in menuData.options) {
         //     options += option + " ";
         // }
    }
    
}
