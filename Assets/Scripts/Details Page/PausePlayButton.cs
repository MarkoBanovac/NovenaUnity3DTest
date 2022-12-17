using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePlayButton : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject Pause;
    public GameObject Play;

    private void Start()
    {
        Play.SetActive(false);
        Pause.SetActive(true);
    }

    // Pokrece ili pauzira Audio, te mijenja ikonu gumba Pause/Play
    public void PlayPauseAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            Pause.SetActive(false);
            Play.SetActive(true);
        }
        else
        {
            audioSource.Play();
            Play.SetActive(false);
            Pause.SetActive(true);
        }
    } 
}
