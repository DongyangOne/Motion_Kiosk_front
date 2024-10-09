using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Button recordButton;
        [SerializeField] private Image progressBar;
        [SerializeField] private Text message;
        [SerializeField] private Dropdown dropdown;
        public GameObject Cart;
        public GameObject card;
        public GameObject payment;
        public GameObject completed;
        public GameObject STT;
        public UIModal uiModal;
        public UICarousel uiCarousel;
        public GameObject modal;

        public AudioSource reAudio;
        public AudioClip replay;

        private readonly string fileName = "output.wav";
        private readonly int duration = 5;

        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai;
        private string apiKey;

        private void Start()
        {
            UpdateMessage(message.text);
            apiKey = LoadApiKey();
            openai = new OpenAIApi(apiKey);

            #if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("웹GL에서는 마이크를 지원하지 않습니다"));
            #else
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            STT.SetActive(false);
            if (STT.activeSelf)
            {
                StartRecording();
                Debug.Log("실행");
            }
            dropdown.onValueChanged.AddListener(ChangeMicrophone);

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
            #endif
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

            #if !UNITY_WEBGL
            clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
            #endif
        }

        private async void EndRecording()
        {
            message.text = "음성 인식 중...";

            #if !UNITY_WEBGL
            Microphone.End(null);
            #endif

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
            
            if(message.text.Contains("커피"))
            {
                uiCarousel.SetPage(1);
            }
            else if(message.text.Contains("차"))
            {
                uiCarousel.SetPage(2);
            }
            else if(message.text.Contains("과일음료"))
            {
                uiCarousel.SetPage(3);
            }
            else if(message.text.Contains("스무디"))
            {
                uiCarousel.SetPage(4);
            }
            else if(message.text.Contains("디저트"))
            {
                uiCarousel.SetPage(5);
            }
            else
            {
                // 잘못된 입력 시 음성클립 재생
                reAudio.PlayOneShot(replay, 1.0f);
                StartRecording();
            }
        }

        private string LoadApiKey()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("api_config");
            if(jsonFile != null)
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

        public void MenuScene()
        {
            SceneManager.LoadScene("MenuPage");
        }

        private IEnumerator HandleCardText()
        {
            card.SetActive(true);
            STT.SetActive(false);
            payment.SetActive(false);
            yield return new WaitForSeconds(3f);

            completed.SetActive(true);
            card.SetActive(false);
            STT.SetActive(false);
            yield return new WaitForSeconds(3f);

            SceneManager.LoadScene("Start");
        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / duration;

                if (time >= duration)
                {
                    time = 0;
                    isRecording = false;
                    EndRecording();
                }
            }
        }
    }

    [System.Serializable]
    public class ApiConfig
    {
        public string apiKey;
    }
}
