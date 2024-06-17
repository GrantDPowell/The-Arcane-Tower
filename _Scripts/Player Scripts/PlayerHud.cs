using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    public PlayerStats playerStats;

    public GameObject hudPanel;

    public TMP_Text strengthText;
    public TMP_Text dexterityText;
    public TMP_Text constitutionText;
    public TMP_Text intelligenceText;
    public TMP_Text wisdomText;
    public TMP_Text charismaText;

    public Slider healthSlider;
    public TMP_Text healthText;
    public Slider xpSlider;
    public TMP_Text expText;

    public TMP_Text levelText;

    public TMP_Text gemTxt;
    public TMP_Text coinTxt;

    public bool showHud = true;

    private void Update()
    {
        if (showHud)
        {
            UpdateStatsDisplay();
        }
        
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

            gemTxt.text = "Gems: " + playerStats.gems;

            healthText.text = playerStats.currentHealth + "/" + playerStats.maxHealth;
            expText.text = playerStats.experiencePoints + "/" + playerStats.ExperienceThreshold();
            coinTxt.text = "Coins: " + playerStats.coins;
        }
    }

    public void HudOn()
    {
        showHud = true;
        hudPanel.SetActive(true);

    }

    public void HudOff()
    {
        showHud = false;
        hudPanel.SetActive(false);
    }
}
