using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System;
using System.IO;

public class DetailsController : MonoBehaviour
{
    public JsonClassList myjsons = new JsonClassList();
    public string jsonData = "";

    private string filePath;

    public List<Texture2D> Gallery;
    public RawImage Background;

    public AudioClip audioClip;
    public AudioSource audioSource;
    public Slider progressBar;
    public TextMeshProUGUI audioTime;

    public TextMeshProUGUI TopicName;
    public TextMeshProUGUI TopicNumber;

    private string path;
    private int index = 0;

    // Postavljanje URL-ova te pokretanje coroutina
    void Start()
    {
        path = Application.persistentDataPath;
        filePath = Application.persistentDataPath + "/JSONdata.json";

        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = "jar:file://" + filePath;
            path = "jar:file://" + path;
        }
        else
        {
            //filePath = "file://" + filePath;
        }

        StartCoroutine(LoadJSON(filePath));
        StartCoroutine(LoadGalleryAndAudio());

        progressBar.onValueChanged.AddListener(OnSliderValueChanged);
    }

    // Citanje JSON datoteke iz persistentDataPath
    IEnumerator LoadJSON(string filePath)
    {
        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            yield return null;
        }

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Error loading audio file: " + www.error);
            yield break;
        }
        else
        {
            jsonData = www.downloadHandler.text;
            yield break;
        }
    }

    // Ovisno o odabranom jeziku i topicu pronalazi tražene slike i zvuk u JSONu te pokrece coroutine za njihovo dohvatanje
    IEnumerator LoadGalleryAndAudio()
    {
        while (jsonData == string.Empty)
        {
            yield return new WaitForSeconds(0.05f);
        }
        if (jsonData != string.Empty)
        {
            myjsons = JsonUtility.FromJson<JsonClassList>(jsonData);

            foreach (TranslatedContent language in myjsons.TranslatedContents)
            {
                if (language.LanguageName == PlayerPrefs.GetString("Language"))
                {
                    foreach (Topic topic in language.Topics)
                    {
                        if (topic.Name == PlayerPrefs.GetString("Topic Name"))
                        {
                            TopicName.text = PlayerPrefs.GetString("Topic Name");
                            TopicNumber.text = PlayerPrefs.GetString("Topic Number");

                            foreach (Media mediaType in topic.Media)
                            {
                                if (mediaType.Photos == null)
                                {
                                    StartCoroutine(LoadAudio(path + mediaType.FilePath));
                                }
                                if (mediaType.Photos != null)
                                {
                                    foreach (Photo photo in mediaType.Photos)
                                    {
                                        StartCoroutine(LoadImage(path + photo.Path));
                                    }
                                }
                            }


                        }
                    }
                }
            }

            StartCoroutine(ChangeBackground());
        }
    }


    // Postavljanje brojaca koji prati koliko je vremenski proslo pjesme
    private void Update()
    {
        if (audioSource.clip != null)
        {
            progressBar.value = audioSource.time / audioSource.clip.length;

            float currentTimeInMinutes = audioSource.time / 60.0f;
            float maxTimeInMinutes = audioSource.clip.length / 60.0f;
            DateTime currTime = DateTime.MinValue.AddMinutes(currentTimeInMinutes);
            DateTime maxTime = DateTime.MinValue.AddMinutes(maxTimeInMinutes);

            audioTime.text = currTime.ToString("mm:ss") + "/" + maxTime.ToString("mm:ss");
        }
    }

    // Pri kliku na Slider azurira trenutno vremensko stanje zvuka
    void OnSliderValueChanged(float value)
    {
        audioSource.time = value * audioSource.clip.length;
    }

    // Dohvatanje Audio datoteke iz persistentDataPath
    IEnumerator LoadAudio(string filePath)
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.MPEG);
        yield return www.SendWebRequest();

        while (!www.isDone)
        {
            yield return null;
        }

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Error loading audio file: " + www.error);
            yield break;
        }
        else
        {
            audioClip = DownloadHandlerAudioClip.GetContent(www);

            audioSource.clip = audioClip;
            audioSource.Play();
            yield break;
        }
    }

    // Dohvatanje Slika iz persistentDataPath
    IEnumerator LoadImage(string filePath)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(filePath);
        yield return www.SendWebRequest();

        while (!www.isDone && www.downloadProgress < 1.0f)
        {
            yield return null;
        }

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("Error loading jpg file: " + www.error);
            yield break;
        }
        else
        {
            Gallery.Add(((DownloadHandlerTexture)www.downloadHandler).texture);
            yield break;
        }
    }

    // Coroutina koja odbrojava 5 sekundi te mijenja pozadinske slike
    IEnumerator ChangeBackground()
    {
        while (true)
        {
            if (Gallery.Count == 0)
            {
                yield return new WaitForSeconds(1);
            }
            if (Gallery.Count > 0)
            {
                Background.texture = Gallery[index];
                index = (index + 1) % Gallery.Count;

                yield return new WaitForSeconds(5);
            }
        }
    }
}
