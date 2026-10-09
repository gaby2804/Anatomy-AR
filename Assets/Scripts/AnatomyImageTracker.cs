using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AnatomyImageTracker : MonoBehaviour
{
    public GameObject heartPrefab;
    public GameObject brainPrefab;

    private ARTrackedImageManager imageManager;

    private Dictionary<string, GameObject> spawnedObjects =
        new Dictionary<string, GameObject>();

    private void Awake()
    {
        imageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
    }

    private void OnDisable()
    {
        if (imageManager != null)
        {
            imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    private void OnTrackedImagesChanged(
        ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            CreateModel(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateModel(trackedImage);
        }
    }

    private void CreateModel(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        GameObject prefabToSpawn = null;

        if (imageName == "Corazon")
        {
            prefabToSpawn = heartPrefab;
        }
        else if (imageName == "Cerebro")
        {
            prefabToSpawn = brainPrefab;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning(
                "No hay un prefab asignado para: " + imageName);
            return;
        }

        if (spawnedObjects.ContainsKey(imageName))
        {
            return;
        }

        GameObject model = Instantiate(
            prefabToSpawn,
            trackedImage.transform);

        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        spawnedObjects.Add(imageName, model);

        UpdateModel(trackedImage);
    }

    private void UpdateModel(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (!spawnedObjects.ContainsKey(imageName))
        {
            return;
        }

        GameObject model = spawnedObjects[imageName];

        model.SetActive(
            trackedImage.trackingState == TrackingState.Tracking);
    }
}