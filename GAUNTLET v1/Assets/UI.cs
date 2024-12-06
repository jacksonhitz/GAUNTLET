using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public Slider healthSlider; 
    public Text healthText; 

    public PlayerHealth playerHealth;

    int maxHealth = 100;
    int currentHealth;

    void Start()
    {

        currentHealth = maxHealth;
    }

    public void UpdateHealth()
    {
        healthSlider.value = playerHealth.currentHealth;

        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

}
