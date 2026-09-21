using UnityEngine;

public class DebugWinLossTester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) GameManager.Instance.CollectMap();
        if (Input.GetKeyDown(KeyCode.Alpha3)) GameManager.Instance.TryExit();
    }
}