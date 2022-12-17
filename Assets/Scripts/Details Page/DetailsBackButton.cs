using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DetailsBackButton : MonoBehaviour
{

    // Povratak na "List page"
    public void ClickBack()
    {
        SceneManager.LoadScene("List Page");
    }
}
