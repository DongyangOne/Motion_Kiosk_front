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
        public MenuModal menuModal; // MenuModal 인스턴스

        private readonly string fileName = "output.wav";
        private readonly int duration = 5;

        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai;
        private string apiKey;

        // 메뉴 데이터를 저장할 리스트
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

            // Handle voice command
            HandleVoiceCommand(res.Text);

            // 음성 인식 완료 후 초기화
            isRecording = false; // 녹음 상태 초기화
            time = 0; // 타이머 초기화
            progressBar.fillAmount = 0; // 진행 바 초기화
        }

        private void HandleVoiceCommand(string command)
        {
            // 메뉴 리스트를 순회하며 음성 인식된 명령어에 해당하는 메뉴가 있는지 확인
            foreach (var menu in menuDataList)
            {
                if (command.Contains(menu.name))
                {
                    Debug.Log("Retrieved MenuData: " + menu); // 디버깅 로그
                    menuModal.OpenModal(menu); // 해당 메뉴 데이터로 모달 열기
                    STT.SetActive(false);
                    return; // 한번 찾으면 종료
                }
            }
            Debug.LogWarning("No matching menu item found for command: " + command);
            STT.SetActive(false);
        }

        private MenuData GetMenuData(string menuName)
        {
            foreach (var menu in menuDataList)
            {
                if (menu.name == menuName)
                {
                    return menu; // 올바른 메뉴 데이터 반환
                }
            }
            Debug.LogWarning($"Menu item '{menuName}' not found.");
            return null; // 메뉴가 없을 경우 null 반환
        }

        private async void LoadMenuData()
        {
            // API를 통해 메뉴 데이터를 로드하는 로직
            string apiUrl = "http://116.39.208.72:8022/api2/menu"; // 실제 API URL로 변경하세요
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
                    // JSON 파싱
                    ApiResponse apiResponse = JsonUtility.FromJson<ApiResponse>(webRequest.downloadHandler.text);
                    menuDataList = apiResponse.data; // 메뉴 데이터를 리스트에 저장
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
