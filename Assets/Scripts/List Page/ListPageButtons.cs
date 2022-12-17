using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class ListPageButtons : MonoBehaviour
{
    // Odlazak na "Details page" za odabrani topic te spremanje odabranih postavki
    public void TopicClick()
    {
        TextMeshProUGUI[] buttonTexts = transform.GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI buttonText in buttonTexts)
        {
            if (buttonText.transform.name == "Topic Name")
            {
                PlayerPrefs.SetString("Topic Name", buttonText.text);
            }
            if (buttonText.transform.name == "Number")
            {
                PlayerPrefs.SetString("Topic Number", buttonText.text);
            }
        }

        SceneManager.LoadScene("Details Page");
    }

    // Povratak na odabir jezika
    public void ClickBack()
    {
        SceneManager.LoadScene("Language Page");
    }

    // Pokretanje AR zaslona (extra page)
    public void ClickExtra()
    {
        SceneManager.LoadScene("AR Page");
    }
}
