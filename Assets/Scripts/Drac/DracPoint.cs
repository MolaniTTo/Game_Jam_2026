using UnityEngine;

public class DracPoint : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;

    public bool TryOccupy()
    {
        if (IsOccupied) return false;
        IsOccupied = true;
        return true;
    }

    public void Release()
    {
        IsOccupied = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = IsOccupied ? Color.red : Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}