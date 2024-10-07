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

            if(message.text.Contains("매장"))
            {
                MenuScene();
                STT.SetActive(false);
            }
            else if(message.text.Contains("포장"))
            {
                STT.SetActive(false);
                MenuScene();
            }
            else
            {
                reAudio.PlayOneShot(replay, 1.0f);
                StartRecording();
            }

            // if(message.text.Contains("커피"))
            // {
            //     //커피 카테고리로 이동
            //UICarousel의 currentPage를 1
            // }
            // else if(message.text.Contains("차"))
            // {
            //     //차 카테고리로 이동
            //UICarousel의 currentPage를 2
            // }
            // else if(message.text.Contains("과일음료"))
            // {
            //     //과일음료 카테고리로 이동
            //UICarousel의 currentPage를 3
            // }
            // else if(message.text.Contains("스무디"))
            // {
            //     //스무디 카테고리로 이동
            //UICarousel의 currentPage를 4
            // }
            // else if(message.text.Contains("디저트"))
            // {
            //     //디저트 카테고리로 이동
            //UICarousel의 currentPage를 5
            // }
            // else
            // {
            //     reAudio.PlayOneShot(replay, 1.0f);
            //     StartRecording();
            // }

            // if(message.text.Contains("장바구니"))
            // {
            //     Cart.SetActive(true);
            //     STT.SetActive(false);
            // }

            // if(message.text.Contains("결제하기"))
            // {
            //     Cart.SetActive(false);
            //     payment.SetActive(true);
            //     STT.SetActive(false);
            // }

            // if(message.text.Contains("카드"))
            // {
            //     StartCoroutine(HandleCardText());
            // }

            // if(message.text.Contains("아메리카노"))
            // {
            //     modal.SetActive(true);
            //     STT.SetActive(false);
            //     UIModal uiModal = FindObjectOfType<UIModal>();
            //     if (uiModal != null)
            //     {
            //         uiModal.OpenModal("아메리카노", 2000, "americano");
            //     }
            // }

            // if(message.text.Contains("에스프레소"))
            // {
            //     modal.SetActive(true);
            //     STT.SetActive(false);
            //     UIModal uiModal = FindObjectOfType<UIModal>();
            //     if (uiModal != null)
            //     {
            //         uiModal.OpenModal("에스프레소", 1500, "espresso");
            //     }
            // }

            // if(message.text.Contains("카페라떼") || message.text.Contains("카페 라떼"))
            // {
            //     modal.SetActive(true);
            //     STT.SetActive(false);
            //     UIModal uiModal = FindObjectOfType<UIModal>();
            //     if (uiModal != null)
            //     {
            //         uiModal.OpenModal("카페라떼", 3000, "cafelatte");
            //     }
            // }

            // if(message.text.Contains("카페모카") || message.text.Contains("카페 모카"))
            // {
            //     modal.SetActive(true);
            //     STT.SetActive(false);
            //     UIModal uiModal = FindObjectOfType<UIModal>();
            //     if (uiModal != null)
            //     {
            //         uiModal.OpenModal("카페모카", 3000, "cafemocha");
            //     }
            // }
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
