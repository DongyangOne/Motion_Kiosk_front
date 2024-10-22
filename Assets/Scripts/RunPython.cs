using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class RunPython : MonoBehaviour
{
    public Process psi;


    void Start()
    {
        if (psi == null || psi.HasExited)  // 프로세스가 종료되었거나 null일 때만 실행
        {
            psi = new Process();
            //psi.StartInfo.FileName = "C:/Users/user/AppData/Local/Programs/Python/Python311";
            psi.StartInfo.FileName = "/usr/bin/python3";  // 시스템에 맞게 경로 설정
            string scriptPath = "/Users/choehyelim/Documents/DMU/ONE/Python/test4.py";
            //string scriptPath = "C:/Users/user/Desktop/Python/test4.py";
            psi.StartInfo.Arguments = scriptPath;

            psi.Start();
            Debug.Log("Python 스크립트가 실행되었습니다.");
        }
        else
        {
            Debug.LogWarning("Python 프로세스가 이미 실행 중입니다.");
        }
    }

    public void ExitPython()
    {
        if (psi != null && !psi.HasExited)
        {
            psi.CloseMainWindow();  // 정상적인 종료 신호 보내기
            psi.WaitForExit(5000);  // 5초 동안 대기

            if (!psi.HasExited)
            {
                psi.Kill();  // 강제 종료
            }
            psi.Dispose();
            Debug.Log("Python 프로세스가 종료되었습니다.");
        }
        else
        {
            Debug.LogWarning("종료할 Python 프로세스가 없습니다.");
        }
    }
}

