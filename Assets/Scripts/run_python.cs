using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class run_python : MonoBehaviour
{
    Process psi;
    // Start is called before the first frame update
    public void startMotion()
    {
        //Debug.Log("실행시작");
        //psi = new Process();

        //psi.StartInfo.FileName = "/usr/bin/python3"; //window 버전으로 경로 변경 필;

        //psi.StartInfo.Arguments = "/Users/choehyelim/Desktop/빅데이터분석_과제/test4.py"; //저장된 위치로 변경 필;
        ////psi.StartInfo.Arguments = Application.dataPath + "/Scenes/puthon/test4.py";

        //psi.StartInfo.CreateNoWindow = true;

        //psi.Start();
    }

    void OnApplicationQuit()
    {
        if (psi != null && !psi.HasExited)
        {
            Debug.Log("Python 프로세스 종료");
            psi.Kill(); // Python 프로세스 종료
            psi.Dispose(); // 리소스 해제
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
