using UnityEngine;

public class Resolution : MonoBehaviour
{
    void Start()
    {
        // 화면 해상도를 1080x1920으로 설정
        Screen.SetResolution(1080, 1920, true);
        
        // 전체 화면 모드로 설정 (True: 전체 화면)
        Screen.fullScreen = true;

        // 화면의 비율을 강제로 맞추는 설정
        Camera.main.aspect = 1080f / 1920f;
    }
}
