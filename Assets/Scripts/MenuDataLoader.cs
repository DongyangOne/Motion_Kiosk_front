using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;




// Data
[System.Serializable]
public class MenuData {
    public string id;
    public string name;
    public int price;
    public string category;
    public string status;
    public string img;
    public List<string> options;
}
[System.Serializable]
public class CategoryData {
    public string categoryName;
    public List<MenuData> menuList;

    public CategoryData(string categoryName) {
        this.categoryName = categoryName;
        this.menuList = new List<MenuData>();
    }
}


// API
[System.Serializable]
public class ApiResponse {
    public int code;
    public bool success;
    public string message;
    public List<MenuData> data;
}




public class MenuDataLoader : MonoBehaviour {
    private string apiURL = "http://116.39.208.72:8022";



    // 이미지 불러오기 실패
    public Sprite notFoundSprite;
    
    // 데이터 로드 완료 이벤트
    public event Action<Dictionary<string, CategoryData>> OnDataLoaded;
    
    // category data dict - string key , CategoryData value로 이루어짐
    Dictionary<string, CategoryData> categoryDict = new Dictionary<string, CategoryData>();




    // API Get Menu
    IEnumerator GetDataRequest(string url, string token) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError(webRequest.error);
            }
            else {
                
                // json 파싱
                string json = webRequest.downloadHandler.text;
                // Debug.Log(json);
                ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(json);
                
                // 데이터 저장
                foreach (MenuData menu in apiResponse.data) {
                    // 카테고리가 이미 존재하는지 확인
                    if (!categoryDict.ContainsKey(menu.category)) {
                        categoryDict[menu.category] = new CategoryData(menu.category);
                    }

                    // 메뉴 추가
                    categoryDict[menu.category].menuList.Add(new MenuData {
                        id = menu.id,
                        name = menu.name,
                        price = menu.price,
                        category = menu.category,
                        img = menu.img,
                        options = menu.options
                    });
                }
                
                // 결과 출력 (카테고리별로 묶인 메뉴)
                // foreach (var category in categoryDict) {
                //     Debug.Log("Category: " + category.Key);
                //     foreach (MenuData menu in category.Value.menuList) {
                //         Debug.Log("Menu: " + menu.menuname + " - Price: " + menu.price);
                //     }
                // }

                OnDataLoaded?.Invoke(categoryDict);
            }
        }
    }



    private IEnumerator LoginAndLoadData() {
        // 로그인이 안되있다면 로그인
        if (PlayerPrefs.GetInt("isLoggedIn") == 0) {
            yield return StartCoroutine(Login.instance.PostLogin());

            if (PlayerPrefs.GetInt("isLoggedIn") == 0) {
                Debug.Log("로그인이 필요합니다.");
                yield break;
            }
        }

        yield return StartCoroutine(GetDataRequest(apiURL + "/api2/menu", PlayerPrefs.GetString("token")));
    }

    public void LoadData() {
        StartCoroutine(LoginAndLoadData());
    }
    
    // API Get Image
    private IEnumerator GetImageRequest(string url, Image imgObject) {
        
// Url, token 수정!!!
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url)) {
            // 요청 전송
            yield return webRequest.SendWebRequest();

            // 에러 처리
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError) {
                imgObject.sprite = notFoundSprite;
                imgObject.color = Color.black;
            } else {
                // 요청 성공 시 텍스처를 가져와 스프라이트로 변환
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imgObject.sprite = sprite;
                imgObject.color = Color.white;
                imgObject.SetNativeSize();
            }
        }
    }
    
    private IEnumerator LoginAndLoadImg(string imgURL, Image imgObject) {
        // 로그인이 안되있다면 로그인
        if (PlayerPrefs.GetInt("isLoggedIn") == 0)
        {
            yield return StartCoroutine(Login.instance.PostLogin());

            if (PlayerPrefs.GetInt("isLoggedIn") == 0)
            {
                Debug.Log("로그인이 필요합니다.");
                yield break;
            }
        }

        yield return StartCoroutine(GetImageRequest(imgURL, imgObject));
    }
    
    // url 수정!!
    public void LoadImage(string imgURL, Image imgObject) {
        StartCoroutine(LoginAndLoadImg(imgURL, imgObject));
    }
    
    
}
