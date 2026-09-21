using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ExitDoor : MonoBehaviour
{
    [SerializeField] private GameObject lockedFeedback; // optional VFX or sound

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance.HasMap)
        {
            GameManager.Instance.TryExit();
        }
        else
        {
            if (lockedFeedback != null) lockedFeedback.SetActive(true);
            Debug.Log("The door is locked. Find the Maze Map first.");
        }
    }
}