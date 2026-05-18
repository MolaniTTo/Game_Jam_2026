using UnityEngine;
using System.Collections.Generic;

public class CameraPointMover : MonoBehaviour
{
    public List<CameraPoint> cameraPoints;
    [SerializeField] private float travelTime = 30f;

    private int currentIndex = 0;
    private float elapsed = 0f;

    private void Update()
    {
        if (cameraPoints == null || cameraPoints.Count < 2) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / travelTime);

        Transform origen = cameraPoints[currentIndex].target;
        Transform desti = cameraPoints[(currentIndex + 1) % cameraPoints.Count].target;

        transform.position = Vector3.Lerp(origen.position, desti.position, t);
        transform.rotation = Quaternion.Lerp(origen.rotation, desti.rotation, t);

        if (elapsed >= travelTime)
        {
            elapsed = 0f;
            currentIndex = (currentIndex + 1) % cameraPoints.Count;
        }
    }
}