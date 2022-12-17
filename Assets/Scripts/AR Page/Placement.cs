using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Placement : MonoBehaviour
{
    public GameObject ARObjectFox;
    public GameObject Indicator;

    private GameObject spawnedObject;
    private Pose placementPose;
    private ARRaycastManager raycastManager;
    private bool placementValid = false;


    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();
    }

    // Pri dodiru postavlja objekt ako je moguce i objekt vec nije postavljen
    void Update()
    {
        if(spawnedObject == null && placementValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }

        UpdatePose();
        UpdateIndicator();
    }

    // Azurira indikator ovisno da li je pozicija valjana
    void UpdateIndicator()
    {
        if (spawnedObject == null && placementValid)
        {
            Indicator.SetActive(true);
            Indicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            Indicator.SetActive(false);
        }
    }

    // Provjerava valjanost pozicije za postavljanje indikatora
    void UpdatePose()
    {
        var center = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(center, hits, TrackableType.Planes);

        placementValid = hits.Count > 0;

        if (placementValid)
        {
            placementPose = hits[0].pose;
        }
    }

    // Funkcija za instanciranje objekta iz prefaba
    void PlaceObject()
    {
          spawnedObject = Instantiate(ARObjectFox, placementPose.position, placementPose.rotation);
    }
}
