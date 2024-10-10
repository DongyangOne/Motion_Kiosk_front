using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Login
[System.Serializable]
public class LoginRequest
{
    public string username;
    public string password;

    public LoginRequest(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

[System.Serializable]
public class LoginData
{
    public string token;
}
[System.Serializable]
public class LoginResponse
{
    public int code;
    public string message;
    public LoginData data;
}

public class Login : MonoBehaviour
{
    private string username = "";
    private string password = "";
    private string apiURL = "http://116.39.208.72:8022";
    public string token;
    private bool isLoggedin = false;

    private InputField inputID;
    private InputField inputPW;
    public static Login instance;


    void Start()
    {
 
        inputID.onValueChanged.AddListener(OnChangeID);
        inputPW.onValueChanged.AddListener(OnChangePW);

    }


    public void OnChangeID(string value)
    {
        username = value;
    }

    public void OnChangePW(string value)
    {
        password = value;
    }

    public void login()
    {
        StartCoroutine(PostLogin());

    }


    // API Login
    public IEnumerator PostLogin()
    {
        string url = apiURL + "/users/login";

        // 로그인 요청 데이터 생성
        LoginRequest loginRequest = new LoginRequest(username, password);
        string json = JsonUtility.ToJson(loginRequest);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
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
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("로그인 에러: " + webRequest.error);
            }
            else
            {
                // 요청 성공 시 응답 처리
                string responseText = webRequest.downloadHandler.text;
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(responseText);

                if (webRequest.responseCode == 200)
                {
                    PlayerPrefs.SetString("token", loginResponse.data.token);
                    Debug.Log("로그인 성공");
                    PlayerPrefs.SetInt("isLoggedIn", 1);
                    SceneManager.LoadScene("Start");

                }
                else
                {
                    Debug.Log("로그인 실패: " + loginResponse.message);
                }

            }
        }
    }
}
