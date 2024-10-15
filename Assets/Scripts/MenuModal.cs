using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
            if (menuData == null)
            {
                // Debug.LogError("MenuData is null 1");
                return;
            }

            Debug.Log(selectedOption);
            
            string selectedOption = multiSelect.GetSelected();
            cartModal.AddToCart(menuData.name, quantity, menuData.price, selectedOption, menuData.img);
            modal.CloseModal();
        });

        payBtn.onClick.AddListener(() => {
            if (menuData == null)
            {
                // Debug.LogError("MenuData is null 2");
                return;
            }
            modal.PaymentModal("menu", menuData.price);
        });
    }

    public void OpenModal(MenuData data) {
        // Debug.Log(data);
        if (data == null)
        {
            // Debug.LogError("MenuData is null 3");
            return;
        }

        modal.MenuModal();
        
        // data reset
        ModalReset[] resetComponents = transform.GetComponentsInChildren<ModalReset>(true);
        foreach (ModalReset components in resetComponents) {
            components.ModalOptionReset();
        }

        menuData = data;
        quantity = 1;
        title.text = menuData.name;
        price.text = string.Format("{0:n0}원", menuData.price);
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
