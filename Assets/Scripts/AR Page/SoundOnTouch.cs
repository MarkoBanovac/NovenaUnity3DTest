using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnTouch : MonoBehaviour
{
    private Vector2 touchPosition = default;
    public Camera arCamera;

    public AudioSource hitAudio;

    // funkcija koja na svaki dodir ekrana (touch) projicira zraku koja ako dode u kontak s colliderom na modelu lisice reproducira zvuk
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touchPosition);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    Debug.Log(hitObject.transform.name);
                    if (hitObject.transform.CompareTag("SpawnedObject")) 
                    {
                        hitAudio.Play();
                    }
                }
            }
            
        }
    }
}
