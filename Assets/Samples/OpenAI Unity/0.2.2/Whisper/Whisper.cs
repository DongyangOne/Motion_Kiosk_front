using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

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
        public AudioClip coffeeClip;
        public AudioClip teaClip;
        public AudioClip noncoffeeClip;
        public AudioClip dessertClip;
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

            // 음성 명령어 처리
            bool commandHandled = HandleVoiceCommand(res.Text);

            // 음성 인식 실패 시 replay 클립 재생
            if (!commandHandled)
            {
                reAudio.clip = replay;
                reAudio.Play();
                StartCoroutine(WaitForReplayEnd(replay.length)); // 코루틴 호출
            }

            isRecording = false;
            time = 0; 
            progressBar.fillAmount = 0;
        }

        private IEnumerator WaitForReplayEnd(float duration)
        {
            yield return new WaitForSeconds(duration); // replay 클립 재생이 끝날 때까지 대기
            StartRecording(); // 음성 인식 다시 시작
        }

        private bool HandleVoiceCommand(string command)
        {
            command = command.ToLower();
            foreach (var menu in menuDataList)
            {
                if (command.Contains(menu.name.ToLower()))
                {
                    Debug.Log("Retrieved MenuData: " + menu);
                    menuModal.OpenModal(menu);
                    STT.SetActive(false);
                    return true; // 명령어가 처리된 경우
                }
            }

            // 카테고리 이동 명령어 처리
            CategoryManager categoryManager = FindObjectOfType<CategoryManager>();
            if (command.Contains("커피"))
            {
                categoryManager.ShowCategory("coffee");
                message.text = "커피 카테고리로 이동합니다.";
                STT.SetActive(false);
                reAudio.clip = coffeeClip;
                reAudio.Play();
                StartCoroutine(ActivateSTTAfterDelay(5f)); // 5초 후 STT 활성화
                return true;
            }
            else if (command.Contains("차"))
            {
                categoryManager.ShowCategory("tea");
                message.text = "차 카테고리로 이동합니다.";
                STT.SetActive(false);
                reAudio.clip = teaClip;
                reAudio.Play();
                StartCoroutine(ActivateSTTAfterDelay(5f)); // 5초 후 STT 활성화
                return true;
            }
            else if (command.Contains("논커피"))
            {
                categoryManager.ShowCategory("noncoffee");
                message.text = "논커피 카테고리로 이동합니다.";
                STT.SetActive(false);
                reAudio.clip = noncoffeeClip;
                reAudio.Play();
                StartCoroutine(ActivateSTTAfterDelay(5f)); // 5초 후 STT 활성화
                return true;
            }
            else if (command.Contains("디저트"))
            {
                categoryManager.ShowCategory("dessert");
                message.text = "디저트 카테고리로 이동합니다.";
                STT.SetActive(false);
                reAudio.clip = dessertClip;
                reAudio.Play();
                StartCoroutine(ActivateSTTAfterDelay(5f)); // 5초 후 STT 활성화
                return true;
            }
            else if (command.Equals("접촉") || command.Equals("비접촉") || command.Contains("음성"))
            {
                SceneManager.LoadScene("SelectWhere");
                return true;
            }
            else if (command.Contains("매장") || command.Contains("포장"))
            {
                SceneManager.LoadScene("MenuPage");
                return true;
            }
            else
            {
                message.text = "다시 말해주세요."; // 음성 인식 실패 시 메시지 표시
                STT.SetActive(true);
                return false; // 명령어가 처리되지 않은 경우
            }

            STT.SetActive(false);
            return false; // 명령어가 처리되지 않은 경우
        }

        private IEnumerator ActivateSTTAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay); // 5초 대기
            STT.SetActive(true); // STT 활성화
            StartRecording(); // 음성 인식 시작
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
