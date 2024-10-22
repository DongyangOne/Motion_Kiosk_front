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
        private bool isModalOpen = false;
        public GameObject STT;
        public GameObject cartModal;
        public GameObject modal;
        public UIModal uiModal;
        public AudioSource reAudio;
        public AudioClip replay;
        public AudioClip coffeeClip;
        public AudioClip teaClip;
        public AudioClip noncoffeeClip;
        public AudioClip dessertClip;
        public AudioClip hotClip;
        public AudioClip iceClip;
        public AudioClip americanoClip;
        public AudioClip cafeLatteClip;
        public AudioClip tomatoClip;
        public AudioClip milkTeaClip;
        public AudioClip saltBreadClip;
        public AudioClip paymentClip;
        public MenuModal menuModal;
        public UIMultiSelect multiSelect;

        [SerializeField] private Button addCartButton;
        [SerializeField] private Button payButton;
        [SerializeField] private Button cardButton;
        [SerializeField] private Button cashButton;

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
            LoadMenuData();

            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            STT.SetActive(false);
            if(SceneManager.GetActiveScene().name == "Start")
            {
                STT.SetActive(true);
                StartRecording();
            }
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

            bool commandHandled = HandleVoiceCommand(res.Text);

            if (!commandHandled)
            {
                reAudio.clip = replay;
                reAudio.Play();
                StartCoroutine(WaitForReplayEnd(replay.length));
            }

            isRecording = false;
            time = 0; 
            progressBar.fillAmount = 0;
        }

        private IEnumerator WaitForReplayEnd(float duration)
        {
            yield return new WaitForSeconds(duration);
            StartRecording();
        }

        private bool HandleVoiceCommand(string command)
        {
            command = command.ToLower();

            if (isModalOpen && command.Contains("뒤로"))
            {
                CloseModal();
                return true;
            }

            foreach (var menu in menuDataList)
            {
                if (command.Contains(menu.name.ToLower()))
                {
                    Debug.Log("Retrieved MenuData: " + menu);
                    menuModal.OpenModal(menu);
                    isModalOpen = true;
                    STT.SetActive(false);
                    StartCoroutine(ActivateSTTAfterDelay(5f));

                    if (menu.name.ToLower() == "아메리카노")
                    {
                        reAudio.clip = americanoClip;
                        reAudio.Play();
                        message.text = "아메리카노 선택하셨습니다.";
                    }
                    else if (menu.name.ToLower() == "카페라떼")
                    {
                        reAudio.clip = cafeLatteClip;
                        reAudio.Play();
                        message.text = "카페라데 선택하셨습니다.";
                    }
                    else if (menu.name.ToLower() == "밀크티")
                    {
                        reAudio.clip = milkTeaClip;
                        reAudio.Play();
                        message.text = "밀크티 선택하셨습니다.";
                    }
                    else if (menu.name.ToLower() == "토마토 주스")
                    {
                        reAudio.clip = tomatoClip;
                        reAudio.Play();
                        message.text = "토마토 주스 선택하셨습니다.";
                    }
                    else if (menu.name.ToLower() == "소금빵")
                    {
                        reAudio.clip = saltBreadClip;
                        reAudio.Play();
                        message.text = "소금빵 선택하셨습니다.";
                    }
                    return true;
                }
            }

            // 카테고리 이동 명령어 처리
            CategoryManager categoryManager = FindObjectOfType<CategoryManager>();
            if (command.Equals("접촉") || command.Equals("비접촉") || command.Contains("음성"))
            {
                SceneManager.LoadScene("SelectWhere");
                return true;
            }
            if (command.Contains("매장") || command.Contains("포장"))
            {
                // 모션으로 안가고 터치로 가게함
                SceneManager.LoadScene("MenuPageTouch");
                return true;
            }
            if (command.Contains("커피"))
            {
                categoryManager.ShowCategory("coffee");
                message.text = "커피 카테고리로 이동합니다.";
                STT.SetActive(false);
                reAudio.clip = coffeeClip;
                reAudio.Play();
                StartCoroutine(ActivateSTTAfterDelay(5f));
                return true;
            }
            else if (command.Contains("차"))
            {
                categoryManager.ShowCategory("tea");
                message.text = "차 카테고리로 이동합니다.";
                STT.SetActive(false);
                reAudio.clip = teaClip;
                reAudio.Play();
                StartCoroutine(ActivateSTTAfterDelay(5f));
                return true;
            }
            else if (command.Contains("논커피"))
            {
                categoryManager.ShowCategory("noncoffee");
                message.text = "논커피 카테고리로 이동합니다.";
                STT.SetActive(false);
                reAudio.clip = noncoffeeClip;
                reAudio.Play();
                StartCoroutine(ActivateSTTAfterDelay(5f));
                return true;
            }
            else if (command.Contains("디저트"))
            {
                categoryManager.ShowCategory("dessert");
                message.text = "디저트 카테고리로 이동합니다.";
                STT.SetActive(false);
                reAudio.clip = dessertClip;
                reAudio.Play();
                StartCoroutine(ActivateSTTAfterDelay(5f));
                return true;
            }
            if (command.Contains("핫"))
            {
                STT.SetActive(false);
                multiSelect.OptionSelect(0);
                reAudio.clip = hotClip;
                reAudio.Play();
                message.text = "핫 음료 선택되었습니다.";
                StartCoroutine(ActivateSTTAfterDelay(5f));
                return true;
            }
            else if (command.Contains("아이스"))
            {
                STT.SetActive(false);
                multiSelect.OptionSelect(1);
                reAudio.clip = iceClip;
                reAudio.Play();
                message.text = "아이스 음료 선택되었습니다.";
                StartCoroutine(ActivateSTTAfterDelay(5f));
                return true;
            }
            if (command.Contains("담기") || (command.Contains("닮기")))
            {
                addCartButton.onClick.Invoke();
                message.text = "상품이 장바구니에 담겼습니다.";
                STT.SetActive(false);
                StartCoroutine(ActivateSTTAfterDelay(5f));
                return true;
            }
            else if (command.Contains("장바구니"))
            {
                modal.SetActive(true);
                cartModal.SetActive(true);
                message.text = "장바구니를 확인합니다.";
                STT.SetActive(false);
                StartCoroutine(ActivateSTTAfterDelay(5f));
                return true;
            }
            else if (command.Equals("결제") || (command.Equals("결재") ||(command.Equals("결제하기") || (command.Equals("결재하기")))))
            {
                addCartButton.onClick.Invoke();
                payButton.onClick.Invoke();
                reAudio.clip = paymentClip;
                reAudio.Play();
                message.text = "결제를 시작합니다.";
                STT.SetActive(false);
                StartCoroutine(ActivateSTTAfterDelay(7f));
                return true;
            }
            if (command.Contains("카드"))
            {
                cardButton.onClick.Invoke();
                message.text = "카드로 결제합니다.";
                STT.SetActive(false);
                // StartCoroutine(ActivateSTTAfterDelay(5f));
                return true;
            }
            else if (command.Contains("현금"))
            {
                cashButton.onClick.Invoke();
                message.text = "현금으로 결제합니다.";
                STT.SetActive(false);
                // StartCoroutine(ActivateSTTAfterDelay(5f));
                return true;
            }
            else
            {
                message.text = "다시 말해주세요.";
                STT.SetActive(true);
                return false;
            }

            STT.SetActive(false);
            return false;
        }

        private void CloseModal()
        {
            modal.SetActive(false);
            cartModal.SetActive(false);
            isModalOpen = false;
            STT.SetActive(false);
            
            // if (menuModal != null)
            // {
            //     menuModal.CloseModal();
            // }

            StartCoroutine(ActivateSTTAfterDelay(3f));
        }
        private IEnumerator ActivateSTTAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            STT.SetActive(true);
            StartRecording();
            yield return new WaitForSeconds(5f);
            StartRecording();
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
