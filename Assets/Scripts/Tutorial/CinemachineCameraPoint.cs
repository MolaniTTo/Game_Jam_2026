using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraPoint : MonoBehaviour
{
    [SerializeField] private int activePriority = 20;
    private CinemachineCamera vcam;
    private int originalPriority;

    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
        if (vcam != null)
            originalPriority = vcam.Priority.Value;
    }

    public void Activate()
    {
        if (vcam != null)
            vcam.Priority = new PrioritySettings { Value = activePriority };
    }

    public void Deactivate()
    {
        if (vcam != null)
            vcam.Priority = new PrioritySettings { Value = originalPriority };
    }
}