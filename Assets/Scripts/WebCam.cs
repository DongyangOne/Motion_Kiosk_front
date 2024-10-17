using UnityEngine;
using UnityEngine.UI;

public class WebCamDisplay : MonoBehaviour
{
    private WebCamTexture webCamTexture; // 웹캠 텍스처
    public RawImage rawImage;            // 화면에 영상을 출력할 RawImage UI 컴포넌트
    public AspectRatioFitter aspectRatioFitter; // 비율 맞추기 위한 컴포넌트 (선택 사항)

    void Start()
    {
        // 웹캠 장치 목록 가져오기
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0) // 웹캠이 있을 경우
        {
            // 첫 번째 웹캠 장치를 사용
            webCamTexture = new WebCamTexture(devices[0].name);

            // RawImage에 WebCamTexture 연결
            rawImage.texture = webCamTexture;
            webCamTexture.Play(); // 웹캠 실행

            // AspectRatioFitter로 비율 맞추기 (선택 사항)
            if (aspectRatioFitter != null)
            {
                aspectRatioFitter.aspectRatio = (float)webCamTexture.width / webCamTexture.height;
            }
        }
        else
        {
            Debug.LogWarning("웹캠을 찾을 수 없습니다.");
        }
    }

    // 씬 종료 시 웹캠 정지
    void OnApplicationQuit()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop(); // 웹캠 중지
        }
    }
}
