using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;


public class UIModal : MonoBehaviour {
    public Image menuImage;
    public TextMeshProUGUI title;
    public TextMeshProUGUI price;
    public GameObject OptionSlide;
    public GameObject cartModal;
    public GameObject payModal;
    public GameObject cardModal;
    public GameObject completed;
    public MenuDataLoader dataLoader;


    private void Start() {
        transform.gameObject.SetActive(false);
    }

    public void OpenModal(string menuName, int menuPrice, string imageURL) {
        ResetModal();
        
        // UICarousel optionCarousel = OptionSlide.GetComponent<UICarousel>();
        // if (optionCarousel != null)
        // {
        //     optionCarousel.SetPage(1);
        // }
        //
        
        transform.gameObject.SetActive(true);

        title.text = menuName;
        price.text = menuPrice.ToString();
        StartCoroutine(LoadImageFromURL(imageURL));
        //Sprite image = Resources.Load<Sprite>(imageURL);
        //if (image != null) {
        //    menuImage.sprite = image;
        //}
    }

    IEnumerator LoadImageFromURL(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            // 요청을 보내고 응답을 기다림
            yield return request.SendWebRequest();

            // 네트워크 에러 또는 HTTP 에러 체크
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                // 텍스처 가져오기
                Texture2D texture = DownloadHandlerTexture.GetContent(request);

                // 텍스처를 Sprite로 변환하여 Image에 적용
                if (texture != null)
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    menuImage.sprite = sprite;
                }
            }
        }
    }


    public void CartModal()
    {
        cartModal.SetActive(true);
        payModal.SetActive(false);
    }

    public void PaymentModal()
    {
        payModal.SetActive(true);
        cartModal.SetActive(false);
    }

    public void CardModal()
    {
        cardModal.SetActive(true);
        payModal.SetActive(false);
    }

    public void PayCompleted()
    {
        completed.SetActive(true);
        cardModal.SetActive(false);
    }

    public void CloseModal() {
        transform.gameObject.SetActive(false);
        cartModal.SetActive(false);
        payModal.SetActive(false);
        cardModal.SetActive(false);
        completed.SetActive(false);
    }

    public void ResetModal() {
        ModalReset[] resetComponents = transform.GetComponentsInChildren<ModalReset>(true);
        foreach (ModalReset components in resetComponents) {
            components.ModalOptionReset();
        }

    }
}