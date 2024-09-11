using OpenAI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Add this for scene management
using System.Collections;

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

        // UIModal 필드 추가
        public UIModal uiModal;
        public GameObject modal;

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
            apiKey = PlayerPrefs.GetString("API_KEY", "default_key");

            #if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
            #else
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
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

        private void StartRecording()
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
            message.text = "Transcripting...";

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

            if (message.text == "장바구니")
            {
                Cart.SetActive(true);
                STT.SetActive(false);
            }

            if (message.text == "결제하기")
            {
                Cart.SetActive(false);
                payment.SetActive(true);
                STT.SetActive(false);
            }

            if (message.text == "카드")
            {
                Cart.SetActive(false);
                card.SetActive(true);
                payment.SetActive(false);
                STT.SetActive(false);
                StartCoroutine(TransitionToStartScene());
            }

            if (message.text.Contains("아메리카노"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                UIModal uiModal = FindObjectOfType<UIModal>();
                if (uiModal != null)
                {
                    uiModal.OpenModal("아메리카노", 2000, "americano");
                }
            }

            if (message.text.Contains("에스프레소"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                UIModal uiModal = FindObjectOfType<UIModal>();
                if (uiModal != null)
                {
                    uiModal.OpenModal("에스프레소", 1500, "espresso");
                }
            }

            if (message.text.Contains("카페라떼"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                UIModal uiModal = FindObjectOfType<UIModal>();
                if (uiModal != null)
                {
                    uiModal.OpenModal("카페라떼", 3000, "cafelatte");
                }
            }

            if (message.text.Contains("카페모카"))
            {
                modal.SetActive(true);
                STT.SetActive(false);
                UIModal uiModal = FindObjectOfType<UIModal>();
                if (uiModal != null)
                {
                    uiModal.OpenModal("카페모카", 3000, "cafemocha");
                }
            }
        }

        private IEnumerator TransitionToStartScene()
        {
            yield return new WaitForSeconds(3f); // Wait for 3 seconds
            SceneManager.LoadScene("Start"); // Replace "Start" with the name of your start scene
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
}
