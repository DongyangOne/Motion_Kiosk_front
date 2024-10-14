using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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

        [SerializeField] private GameObject CategorySlide;

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

            HandleVoiceCommand(res.Text); //음성 명령 처리

            //replay 음성 클립을 재생하도록 분리
            if (IsValidCommand(res.Text))
            {
                //유효한 명령어일 경우 추가 행동 수행
                ProcessValidCommand(res.Text);
            }
            else
            {
                //유효하지 않은 명령어일 경우 다시 말씀해 주세요
                reAudio.PlayOneShot(replay, 1.0f);
                StartRecording();
            }
        }

        private bool IsValidCommand(string command)
        {
            //유효한 명령어
            return command.Contains("매장") || command.Contains("포장") ||
                command.Contains("장바구니") || command.Contains("결제하기") ||
                command.Contains("아메리카노") || command.Contains("에스프레소") ||
                command.Contains("카페라떼") || command.Contains("카페 모카") ||
                command.Contains("차") || command.Contains("커피") ||
                command.Contains("과일음료") || command.Contains("스무디") ||
                command.Contains("디저트") ||
                command.Contains("바닐라라떼") || command.Contains("바닐라 라떼") ||
                command.Contains("케모마일") ||
                command.Contains("얼그레이") ||
                command.Contains("쟈스민") ||
                command.Contains("페퍼민트") ||
                command.Contains("자몽") ||
                command.Contains("자몽얼그레이") || command.Contains("자몽 얼그레이") ||
                command.Contains("허니레몬") ||
                command.Contains("레몬") ||
                command.Contains("녹차라떼") ||
                command.Contains("망고") ||
                command.Contains("딸기") ||
                command.Contains("요거트") ||
                command.Contains("디저트1") ||
                command.Contains("디저트2") ||
                command.Contains("디저트3") ||
                command.Contains("디저트4");
        }

        private void ProcessValidCommand(string command)
        {
            if (command.Contains("매장"))
            {
                MenuScene();
                STT.SetActive(false);
            }
            else if (command.Contains("포장"))
            {
                STT.SetActive(false);
                MenuScene();
            }
            else if (command.Contains("장바구니"))
            {
                Cart.SetActive(true);
                STT.SetActive(false);
            }
            else if (command.Contains("결제하기"))
            {
                Cart.SetActive(false);
                payment.SetActive(true);
                STT.SetActive(false);
            }
            else if (command.Contains("카드"))
            {
                StartCoroutine(HandleCardText());
            }
            else if (command.Contains("아메리카노"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("아메리카노", 2000, "americano");
            }
            else if (command.Contains("에스프레소"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("에스프레소", 1500, "espresso");
            }
            else if (command.Contains("카페라떼") || command.Contains("카페 라떼"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("카페라떼", 3000, "cafelatte");
            }
            else if (command.Contains("카페모카") || command.Contains("카페 모카"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("카페모카", 3000, "cafemocha");
            }
            else if (command.Contains("바닐라라떼") || command.Contains("바닐라 라떼"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("바닐라라떼", 3000, "vanillalatte");
            }
            else if (command.Contains("케모마일"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("케모마일", 2000, "chamomile");
            }
            else if (command.Contains("얼그레이"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("얼그레이", 2000, "earlgrey");
            }
            else if (command.Contains("쟈스민"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("쟈스민", 2000, "jasmine");
            }
            else if (command.Contains("페퍼민트") || (command.Contains("페퍼 민트")))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("페퍼민트", 2000, "peppermint");
            }
            else if (command.Contains("자몽"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("자몽", 3500, "grapefruit");
            }
            else if (command.Contains("자몽얼그레이") || (command.Contains("자몽 얼그레이")))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("자몽얼그레이", 3500, "grapefruitearlgrey");
            }
            else if (command.Contains("허니레몬"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("허니레몬", 3500, "honeylemon");
            }
            else if (command.Contains("레몬"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("레몬", 3500, "lemon");
            }
            else if (command.Contains("녹차라떼") || (command.Contains("녹차 라떼")))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("녹차라떼", 2000, "greentealatte");
            }
            else if (command.Contains("망고"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("망고", 2000, "mango");
            }
            else if (command.Contains("딸기"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("딸기", 2000, "strawberry");
            }
            else if (command.Contains("요거트"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("요거트", 2000, "yogurt");
            }
            else if (command.Contains("디저트1"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("디저트1", 2000, "dessert1");
            }
            else if (command.Contains("디저트2"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("디저트2", 2000, "dessert2");
            }
            else if (command.Contains("디저트3"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("디저트3", 2000, "dessert3");
            }
            else if (command.Contains("디저트4"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                OpenModalWithDelay("디저트4", 2000, "dessert4");
            }
            else
            {
                //필요한 유효한 명령어 처리
                HandleVoiceCommand(command);
            }
        }

        private void OpenModalWithDelay(string title, int duration, string modalKey)
        {
            UIModal uiModal = FindObjectOfType<UIModal>();
            if (uiModal != null)
            {
                uiModal.OpenModal(title, duration, modalKey);
            }
        }

        private void HandleVoiceCommand(string recognizedText)
        {
            Debug.Log("인식된 텍스트: " + recognizedText);

            //음성 인식에 따라 카테고리 이동
            if (recognizedText.Contains("차"))
            {
                ActivateCategory("CategoryPage차");
            }
            else if (recognizedText.Contains("커피"))
            {
                ActivateCategory("CategoryPage커피");
            }
            else if (recognizedText.Contains("과일음료") || recognizedText.Contains("과일 음료"))
            {
                ActivateCategory("CategoryPage과일음료");
            }
            else if (recognizedText.Contains("스무디"))
            {
                ActivateCategory("CategoryPage스무디");
            }
            else if (recognizedText.Contains("디저트"))
            {
                ActivateCategory("CategoryPage디저트");
            }
            //else
            //{
            //    //잘못된 음성 인식 처리
            //    reAudio.PlayOneShot(replay, 1.0f);
            //}

            //다음 음성을 위한 초기화
            //StartRecording();
        }

        private void ActivateCategory(string categoryName)
        {
            //선택한 카테고리 페이지만 활성화
            foreach (Transform page in CategorySlide.transform)
            {
                page.gameObject.SetActive(false); //모든 페이지 비활성화
            }

            //선택한 카테고리 페이지 활성화
            var categoryPage = CategorySlide.transform.Find(categoryName);
            if (categoryPage != null)
            {
                categoryPage.gameObject.SetActive(true);
                Debug.Log($"'{categoryName}' 페이지로 이동합니다.");
            }
            else
            {
                Debug.LogWarning($"'{categoryName}' 페이지를 찾을 수 없습니다.");
            }
        }

        private List<Transform> GetChildren(Transform parent)
        {
            return parent.Cast<Transform>().ToList();
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
                    EndRecording();
                    time = 0;
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