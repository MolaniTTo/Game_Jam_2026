using System.Collections.Generic;
using UnityEngine;

public class WaypointZone : MonoBehaviour
{
    public static WaypointZone Instance { get; private set; }

    [SerializeField] private List<DracPoint> waypoints;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public List<DracPoint> GetPoints() => waypoints;
}