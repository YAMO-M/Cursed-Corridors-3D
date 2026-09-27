using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private GameObject minimapPanel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollectItem();

            if (minimapPanel != null)
            {
                minimapPanel.SetActive(true);
            }

            gameObject.SetActive(false);
        }
    }
}