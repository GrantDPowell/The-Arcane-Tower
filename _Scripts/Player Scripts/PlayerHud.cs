using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    public PlayerStats playerStats;

    public TMP_Text strengthText;
    public TMP_Text dexterityText;
    public TMP_Text constitutionText;
    public TMP_Text intelligenceText;
    public TMP_Text wisdomText;
    public TMP_Text charismaText;

    public Slider healthSlider;
    public Slider xpSlider;

    public TMP_Text levelText;

    private void Update()
    {
        UpdateStatsDisplay();
    }

    private void UpdateStatsDisplay()
    {
        if (playerStats != null)
        {
            strengthText.text = "Strength: " + playerStats.currentStrength;
            dexterityText.text = "Dexterity: " + playerStats.currentDexterity;
            constitutionText.text = "Constitution: " + playerStats.currentConstitution;
            intelligenceText.text = "Intelligence: " + playerStats.currentIntelligence;
            wisdomText.text = "Wisdom: " + playerStats.currentWisdom;
            charismaText.text = "Charisma: " + playerStats.currentCharisma;

            healthSlider.maxValue = playerStats.maxHealth;
            healthSlider.value = playerStats.currentHealth;

            xpSlider.maxValue = playerStats.ExperienceThreshold();
            xpSlider.value = playerStats.experiencePoints;

            levelText.text = "Level: " + playerStats.level;
        }
    }
}
