using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LanguagePageButtons : MonoBehaviour
{
    // Prelazak na "List Page" te spremanje odabranog jezika
    public void LanguageClick()
    {
        PlayerPrefs.SetString("Language", transform.GetComponentInChildren<TextMeshProUGUI>().text);
        SceneManager.LoadScene("List Page");
    }
}
