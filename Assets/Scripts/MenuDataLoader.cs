using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



// Data
[System.Serializable]
public class MenuData {
    public string id;
    public string menuname;
    public int price;
    public string category;
// 수정!!!
    // public string imageURL;
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

// Login
[System.Serializable]
public class LoginRequest {
    public string username;
    public string password;

    public LoginRequest(string username, string password) {
        this.username = username;
        this.password = password;
    }
}

[System.Serializable]
public class LoginData {
    public string token;
}
[System.Serializable]
public class LoginResponse {
    public int code;
    public string message;
    public LoginData data;
}



public class MenuDataLoader : MonoBehaviour {

    private string username = "dongseon";
    private string password = "1111";
    private string apiURL = "http://116.39.208.72:8022";
    private string token;
    private bool isLoggedin = false;
    
    
    // 이미지 불러오기 실패
    public Sprite notFoundSprite;
    
    // 데이터 로드 완료 이벤트
    public event Action<Dictionary<string, CategoryData>> OnDataLoaded;
    
    // category data dict - string key , CategoryData value로 이루어짐
    Dictionary<string, CategoryData> categoryDict = new Dictionary<string, CategoryData>();
    
    // API Login
    private IEnumerator PostLogin() {
        string url = apiURL + "/users/login";
        
        // 로그인 요청 데이터 생성
        LoginRequest loginRequest = new LoginRequest(username, password);
        string json = JsonUtility.ToJson(loginRequest);
        
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST")) {
            // 본문(body) 설정
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // 헤더 설정
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // 요청 전송
            yield return webRequest.SendWebRequest();

            // 에러 처리
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError) {
                Debug.LogError("로그인 에러: " + webRequest.error);
            } else {
                // 요청 성공 시 응답 처리
                string responseText = webRequest.downloadHandler.text;
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);
                
                if (webRequest.responseCode == 200) {
                    token = loginResponse.data.token;
                    Debug.Log("로그인 성공");
                    isLoggedin = true;
                } else {
                    Debug.Log("로그인 실패: " + loginResponse.message);
                }

            }
        }
    }

    
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
                        menuname = menu.menuname,
                        price = menu.price,
                        category = menu.category
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
        if (!isLoggedin) {
            yield return StartCoroutine(PostLogin());

            if (!isLoggedin) {
                Debug.Log("로그인이 필요합니다.");
                yield break;
            }
        }

        yield return StartCoroutine(GetDataRequest(apiURL + "/api/menu/total", token));
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
                Debug.LogError(webRequest.error);
                imgObject.sprite = notFoundSprite;
            } else {
                // 요청 성공 시 텍스처를 가져와 스프라이트로 변환
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                imgObject.sprite = sprite;
                imgObject.SetNativeSize();
            }
        }
    }
    
    private IEnumerator LoginAndLoadImg(string imgURL, Image imgObject) {
        // 로그인이 안되있다면 로그인
        if (!isLoggedin) {
            yield return StartCoroutine(PostLogin());

            if (!isLoggedin) {
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
