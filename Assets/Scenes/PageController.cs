using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PageController : MonoBehaviour
{
    public GameObject Cart;
    public GameObject Payment;
    public GameObject STT;
    public GameObject CardPay;
    public GameObject completed;
    // [SerializeField] private Text message;

    // // void Start()
    // // {
    // //     UpdateMessage(message.text);
    // // }

    // // void Update()
    // // {
    // //     if (message.text == "장바구니")
    // //     {
    // //         CartPage();
    // //         STT.SetActive(false);
    // //     }
    // // }

    // public void UpdateMessage(string newText)
    // {
    //     message.text = newText;
    // }

    public void CartPage()
    {
        Cart.SetActive(true);
        Payment.SetActive(false);
    }

    public void PaymentPage()
    {
        Cart.SetActive(false);
        Payment.SetActive(true);
    }

    public void CancelPage()
    {
        Cart.SetActive(false);
        Payment.SetActive(false);
    }

    public void SttOn()
    {
        STT.SetActive(true);
    }

    public void SttOff()
    {
        STT.SetActive(false);
    }

    public void CardPayPage()
    {
        CardPay.SetActive(true);
        StartCoroutine(DeactivateCardPayAfterDelay(3f));
    }

    private IEnumerator DeactivateCardPayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CardPay.SetActive(false);
        completed.SetActive(true);
        
        yield return new WaitForSeconds(3f);
        
        SceneManager.LoadScene("Start");
    }
}
