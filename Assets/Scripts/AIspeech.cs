using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Samples.Whisper;

public class AIspeech : MonoBehaviour
{
    public AudioSource startAudio;
    public GameObject STT;
    private bool AudioPlayed = false;

    public Whisper whisper;

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Start")
        {
            StartScene();
        }

        if(SceneManager.GetActiveScene().name == "MenuPage")
        {
            StartScene();
        }
    }

    void Update()
    {
        if(AudioPlayed && !startAudio.isPlaying)
        {
            STT.SetActive(true);
            whisper.StartRecording();
            AudioPlayed = false;
        }
    }

    public void StartScene()
    {
        startAudio.Play();
        AudioPlayed = true;
    }
}