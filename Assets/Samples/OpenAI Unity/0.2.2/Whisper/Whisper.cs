using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking; // 추가된 네임스페이스
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks; // 추가된 네임스페이스

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Button recordButton;
        [SerializeField] private Image progressBar;
        [SerializeField] private Text message;
        [SerializeField] private Dropdown dropdown;
        public GameObject STT;
        public UIModal uiModal;
        public AudioSource reAudio;
        public AudioClip replay;
        public MenuModal menuModal;

        private readonly string fileName = "output.wav";
        private readonly int duration = 5;

        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai;
        private string apiKey;

        private List<MenuData> menuDataList;

        private void Start()
        {
            UpdateMessage(message.text);
            apiKey = LoadApiKey();
            openai = new OpenAIApi(apiKey);
            LoadMenuData(); // 메뉴 데이터 로드

            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            STT.SetActive(false);
            dropdown.onValueChanged.AddListener(ChangeMicrophone);

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
        }

        public void UpdateMessage(string newText)
        {
            message.text = newText;
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }

        public void StartRecording()
        {
            isRecording = true;
            recordButton.enabled = false;

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
        }

        private async void EndRecording()
        {
            message.text = "음성 인식 중...";
            Microphone.End(null);

            byte[] data = SaveWav.Save(fileName, clip);
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() { Data = data, Name = "audio.wav" },
                Model = "whisper-1",
                Language = "ko"
            };
            var res = await openai.CreateAudioTranscription(req);

            progressBar.fillAmount = 0;
            message.text = res.Text;
            recordButton.enabled = true;

            HandleVoiceCommand(res.Text);

            isRecording = false;
            time = 0; 
            progressBar.fillAmount = 0;
        }

        private void HandleVoiceCommand(string command)
        {
            command = command.ToLower();
            foreach (var menu in menuDataList)
            {
                if (command.Contains(menu.name.ToLower()))
                {
                    Debug.Log("Retrieved MenuData: " + menu);
                    menuModal.OpenModal(menu);
                    STT.SetActive(false);
                    return;
                }
            }

            // 카테고리 이동 명령어 처리
            CategoryManager categoryManager = FindObjectOfType<CategoryManager>();
            if (command.Contains("커피"))
            {
                categoryManager.ShowCategory("coffee");
                message.text = "커피 카테고리로 이동합니다.";
            }
            else if (command.Contains("차"))
            {
                categoryManager.ShowCategory("tea");
                message.text = "차 카테고리로 이동합니다.";
            }
            else if (command.Contains("논커피"))
            {
                categoryManager.ShowCategory("noncoffee");
                message.text = "논커피 카테고리로 이동합니다.";
            }
            else if (command.Contains("디저트"))
            {
                categoryManager.ShowCategory("dessert");
                message.text = "디저트 카테고리로 이동합니다.";
            }
            else
            {
                message.text = "다시 말해주세요.";
            }

            STT.SetActive(false);
        }

        private async void LoadMenuData()
        {
            string apiUrl = "http://116.39.208.72:8022/api2/menu";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
            {
                webRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("token"));
                var operation = webRequest.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Delay(100);
                }

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(webRequest.error);
                }
                else
                {
                    ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(webRequest.downloadHandler.text);
                    menuDataList = apiResponse.data;
                }
            }
        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / duration;

                if (time >= duration)
                {
                    EndRecording();
                    time = 0;
                }
            }
        }

        private string LoadApiKey()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("api_config");
            if (jsonFile != null)
            {
                ApiConfig config = JsonUtility.FromJson<ApiConfig>(jsonFile.text);
                return config.apiKey;
            }
            else
            {
                Debug.LogError("API config file not found!");
                return string.Empty;
            }
        }
    }

    [System.Serializable]
    public class ApiResponse
    {
        public int code;
        public bool success;
        public string message;
        public List<MenuData> data;
    }

    [System.Serializable]
    public class ApiConfig
    {
        public string apiKey;
    }
}
