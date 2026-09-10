using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public RawImage healthFill;  // ← Changed from "Image" to "RawImage"
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateBar();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) TakeDamage(10);
        if (Input.GetKeyDown(KeyCode.R)) Heal(10);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateBar();
        if (currentHealth <= 0) Debug.Log("💀 Game Over");
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (healthFill != null)
        {
  
            float percent = (float)currentHealth / maxHealth;
            Rect uvRect = healthFill.uvRect;
            uvRect.width = percent;  // Shrink the visible part of the image
            healthFill.uvRect = uvRect;
        }
    }
}