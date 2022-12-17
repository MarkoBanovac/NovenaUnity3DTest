using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class LanguageController : MonoBehaviour
{
    public Button ButtonTemplate;
    public Canvas canvas;

    public JsonClassList myjsons = new JsonClassList();
    public string jsonData = "";

    private string filePath;
    private string sourcePath;
    private string destinationPath;

    public string[] filePaths;


    //Postavljanje URL-ova i pokretanje coroutina
    void Start()
    {
        filePath = Application.persistentDataPath + "/JSONdata.json";

        if (Application.platform == RuntimePlatform.Android)
        {
            sourcePath = "jar:file://" + Application.dataPath + "!/assets/";
        }
        else
        {
            sourcePath = Application.streamingAssetsPath;
        }
        
        destinationPath = Application.persistentDataPath;

        StartCoroutine(CopyDir(sourcePath, destinationPath));

        StartCoroutine(LoadJSON(filePath));
        StartCoroutine(CreateButtons());
    }

    // Cita JSON datoteku iz persistentDataPath
    IEnumerator LoadJSON(string filePath)
    {
        while (System.IO.File.Exists(filePath) == false)
        {
            yield return new WaitForSeconds(0.05f);
        }

        if (System.IO.File.Exists(filePath))
        {
            string fileContents = File.ReadAllText(filePath);

            jsonData = fileContents;
        } 
    }

    //Stvara tipke po predlosku ovisno koliko jezika je postavljeno u JSON datoteci
    IEnumerator CreateButtons()
    {
        while(jsonData == string.Empty)
        {
            yield return new WaitForSeconds(0.05f);
        }
        if (jsonData != string.Empty)
        {
            myjsons = JsonUtility.FromJson<JsonClassList>(jsonData);
            Vector3 currentPosition = new Vector3(0, 75, 0);

            foreach (TranslatedContent language in myjsons.TranslatedContents)
            {
                Button button = Instantiate(ButtonTemplate);

                button.transform.position = currentPosition + new Vector3(0, -225, 0);
                currentPosition = button.transform.position;

                button.GetComponentInChildren<TextMeshProUGUI>().text = language.LanguageName;
                button.transform.SetParent(canvas.transform, false);
            }
        }
      
    }

    // Funkcija koja kopira direktoriji iz streamingAssetsPath u persistentDataPath
    IEnumerator CopyDir(string SourcePath, string DestinationPath)
    {
        // Dio za android platformu, definitivno dio koji mi je zadavao najvise problema.
        // Znam da nije kvalitetno rjesenje al nisam uspio napraviti da radi na drugi nacin nego da idem file po file i kopiram
        if (Application.platform == RuntimePlatform.Android)
        {
            string fileName = "JSONdata.json";
            string streamingAssetsPath = "jar:file://" + Application.dataPath + "!/assets";

            UnityWebRequest request = UnityWebRequest.Get(streamingAssetsPath + "/" + fileName);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error downloading file: " + request.error);
            }
            else
            {
                File.WriteAllBytes(destinationPath + "/" + fileName, request.downloadHandler.data);
            }

            List<string> audioFileNames = new List<string> { "Happy.mp3", "Sad.mp3", "Motivational.mp3", "Epic.mp3", "Party.mp3" };

            for (int i = 0; i < audioFileNames.Count; i++)
            {
                UnityWebRequest requestAUD = UnityWebRequest.Get(streamingAssetsPath + "/Audio/" + audioFileNames[i]);
                yield return requestAUD.SendWebRequest();

                if (requestAUD.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error downloading file: " + requestAUD.error);
                }
                else
                {
                    Directory.CreateDirectory(destinationPath + "/Audio");
                    File.WriteAllBytes(destinationPath + "/Audio/" + audioFileNames[i], requestAUD.downloadHandler.data);
                }
            }

            List<string> imageFileNames = new List<string> { "Happy1.jpg", "Happy2.jpg", "Happy3.jpg", "Sad1.jpg", "Sad2.jpg", "Sad3.jpg", 
                "Party1.jpg", "Party2.jpg", "Party3.jpg", "Space1.jpg", "Space2.jpg", "Space3.jpg", "Fantasy1.jpg", "Fantasy2.jpg", "Fantasy3.jpg" };

            for (int i = 0; i < imageFileNames.Count; i++)
            {
                UnityWebRequest requestIMG = UnityWebRequest.Get(streamingAssetsPath + "/Images/" + imageFileNames[i]);
                yield return requestIMG.SendWebRequest();

                if (requestIMG.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error downloading file: " + requestIMG.error);
                }
                else
                {
                    Directory.CreateDirectory(destinationPath + "/Images");
                    File.WriteAllBytes(destinationPath + "/Images/" + imageFileNames[i], requestIMG.downloadHandler.data);
                }
            }
        }
        //Dio za desktop verziju i testiranje, jednostavna rekurzija za prolazak kroz datoteke
        else
        {
            filePaths = Directory.GetFileSystemEntries(SourcePath);

            foreach (string filePath in filePaths)
            {
                if (Directory.Exists(filePath))
                {
                    string directoryName = Path.GetFileName(filePath);

                    Directory.CreateDirectory(Application.persistentDataPath + "/" + directoryName);

                    yield return StartCoroutine(CopyDir(filePath, DestinationPath + "/" + directoryName));
                }
                else
                {
                    UnityWebRequest www = UnityWebRequest.Get(filePath);

                    yield return www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Debug.LogError(www.error);
                    }
                    else
                    {

                        string fileName = Path.GetFileName(filePath);

                        string destinationPath = DestinationPath + "/" + fileName;

                        File.WriteAllBytes(destinationPath, www.downloadHandler.data);
                    }
                }
            }
        }    
    }
}
