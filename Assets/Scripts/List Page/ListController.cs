using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class ListController : MonoBehaviour
{
    public Button ButtonTemplate;
    public Canvas canvas;

    public JsonClassList myjsons = new JsonClassList();
    public string jsonData = "";

    private string filePath;

    // Postavljanje URL-a za preuzimanje JSON-a, te pokretanje coroutina
    void Start()
    {
        filePath = Application.persistentDataPath + "/JSONdata.json";

        if (Application.platform == RuntimePlatform.Android)
        {
            filePath = "jar:file://" + filePath;
        }
        else
        {
            //filePath = "file://" + filePath;
        }

        StartCoroutine(LoadJSON(filePath));
        StartCoroutine(CreateButtons());
    }

    //Citanje JSON datoteke iz "persistentDataPath"
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

    // Stvaranje tipki po predlosku ovisno o odabranom jeziku i JSON datoteci
    IEnumerator CreateButtons()
    {
        while (jsonData == string.Empty)
        {
            yield return new WaitForSeconds(0.05f);
        }
        if (jsonData != string.Empty)
        {
            myjsons = JsonUtility.FromJson<JsonClassList>(jsonData);
            Vector3 currentPosition = new Vector3(0, 600, 0);
            int topicNumber = 2;

            foreach (TranslatedContent language in myjsons.TranslatedContents)
            {
                if (language.LanguageName == PlayerPrefs.GetString("Language"))
                {
                    foreach (Topic topic in language.Topics)
                    {
                        Button button = Instantiate(ButtonTemplate);

                        button.transform.position = currentPosition + new Vector3(0, -250, 0);
                        currentPosition = button.transform.position;

                        TextMeshProUGUI[] buttonTexts = button.GetComponentsInChildren<TextMeshProUGUI>();
                        foreach (TextMeshProUGUI buttonText in buttonTexts)
                        {
                            if (buttonText.transform.name == "Topic Name")
                            {
                                buttonText.text = topic.Name;
                            }
                            if (buttonText.transform.name == "Number")
                            {
                                buttonText.text = topicNumber.ToString();
                                topicNumber++;
                            }
                        }

                        button.transform.SetParent(canvas.transform, false);
                    }
                }
            }
        }
    }
}
