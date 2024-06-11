using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LevelUpUI : MonoBehaviour
{
    public Button option1Button;
    public Button option2Button;
    public Button option3Button;
    public GameObject tooltip; // Reference to the tooltip GameObject
    public TextMeshProUGUI tooltipTextCurrent; // Reference to the TextMeshProUGUI component in the tooltip
    public TextMeshProUGUI tooltipTextProjected; // Reference to the TextMeshProUGUI component in the tooltip

    private PlayerLevelSystem playerLevelSystem;
    private PlayerStats playerStats;
    private List<object> currentOptions;

    private void Awake()
    {
        playerLevelSystem = FindObjectOfType<PlayerLevelSystem>();
        playerStats = playerLevelSystem.playerStats; // Get the PlayerStats from PlayerLevelSystem

        option1Button.onClick.AddListener(() => ApplyCard(0));
        option2Button.onClick.AddListener(() => ApplyCard(1));
        option3Button.onClick.AddListener(() => ApplyCard(2));

        // Add event listeners for hover
        option1Button.gameObject.AddComponent<HoverHandler>().Init(ShowTooltip, HideTooltip, 0);
        option2Button.gameObject.AddComponent<HoverHandler>().Init(ShowTooltip, HideTooltip, 1);
        option3Button.gameObject.AddComponent<HoverHandler>().Init(ShowTooltip, HideTooltip, 2);
    }

    public void ShowOptions(List<object> options)
    {
        currentOptions = options;
        //Debug.Log("ShowOptions called with " + options.Count + " options.");

        if (options.Count > 0)
        {
            option1Button.GetComponentInChildren<TextMeshProUGUI>().text = GetCardName(options[0]);
            option1Button.gameObject.SetActive(true);
        }
        else
        {
            option1Button.gameObject.SetActive(false);
        }

        if (options.Count > 1)
        {
            option2Button.GetComponentInChildren<TextMeshProUGUI>().text = GetCardName(options[1]);
            option2Button.gameObject.SetActive(true);
        }
        else
        {
            option2Button.gameObject.SetActive(false);
        }

        if (options.Count > 2)
        {
            option3Button.GetComponentInChildren<TextMeshProUGUI>().text = GetCardName(options[2]);
            option3Button.gameObject.SetActive(true);
        }
        else
        {
            option3Button.gameObject.SetActive(false);
        }
    }

    private string GetCardName(object card)
    {
        if (card is PlayerCard playerCard)
        {
            return playerCard.cardName;
        }
        else if (card is SpellCard spellCard)
        {
            return spellCard.cardName;
        }
        return "Unknown Card";
    }

    private void ApplyCard(int index)
    {
        if (index < currentOptions.Count)
        {
            //Debug.Log("Applying card: " + GetCardName(currentOptions[index]));
            playerLevelSystem.ApplyCard(currentOptions[index]);
            Time.timeScale = 1f;

            // Hide the tooltip when a card is selected
            HideTooltip();
        }
    }

    private void ShowTooltip(int index)
    {
        //Debug.Log("ShowTooltip called for index: " + index);

        if (index < currentOptions.Count)
        {
            PlayerCard playerCard = currentOptions[index] as PlayerCard;
            if (playerCard != null)
            {
                //Debug.Log("Tooltip activated for card: " + playerCard.cardName);
                tooltip.SetActive(true);
                tooltipTextCurrent.text = GetToolTipTextCurrent(playerCard);
                tooltipTextProjected.text = GetToolTipTextProjected(playerCard);
                //Debug.Log("Tooltip Text Current: " + tooltipTextCurrent.text);
                //Debug.Log("Tooltip Text Projected: " + tooltipTextProjected.text);
            }
        }
    }

    private void HideTooltip()
    {
        if (tooltip == null)
        {
            Debug.LogError("Tooltip GameObject is null. Please assign it in the Inspector.");
            return;
        }

        tooltip.SetActive(false);
        tooltipTextCurrent.text = "";
        tooltipTextProjected.text = "";
    }

    private string GetToolTipTextCurrent(PlayerCard card)
    {
        string currentStats = $"Current Stats:\n" +
                              $"Strength: {playerStats.currentStrength}\n" +
                              $"Dexterity: {playerStats.currentDexterity}\n" +
                              $"Constitution: {playerStats.currentConstitution}\n" +
                              $"Intelligence: {playerStats.currentIntelligence}\n" +
                              $"Wisdom: {playerStats.currentWisdom}\n" +
                              $"Charisma: {playerStats.currentCharisma}\n" +
                              $"Health: {playerStats.currentHealth}/{playerStats.maxHealth}\n";

        return $"{currentStats}";
    }

    private string GetToolTipTextProjected(PlayerCard card)
    {
        string projectedStats = $"Projected Stats:\n" +
                                $"Strength: {GetStatText(playerStats.currentStrength, card.addStrength, card.multiplyStrength)}\n" +
                                $"Dexterity: {GetStatText(playerStats.currentDexterity, card.addDexterity, card.multiplyDexterity)}\n" +
                                $"Constitution: {GetStatText(playerStats.currentConstitution, card.addConstitution, card.multiplyConstitution)}\n" +
                                $"Intelligence: {GetStatText(playerStats.currentIntelligence, card.addIntelligence, card.multiplyIntelligence)}\n" +
                                $"Wisdom: {GetStatText(playerStats.currentWisdom, card.addWisdom, card.multiplyWisdom)}\n" +
                                $"Charisma: {GetStatText(playerStats.currentCharisma, card.addCharisma, card.multiplyCharisma)}\n" +
                                $"Health: {playerStats.currentHealth}/{playerStats.maxHealth + (card.addConstitution * 10)}\n";

        return $"{projectedStats}";
    }

    private string GetStatText(int baseValue, int addValue, float multiplyValue)
    {
        string addText = "";
        string multiplyText = "";

        if (addValue != 0)
        {
            string color = addValue > 0 ? "green" : "red";
            addText = $" <color={color}>{(addValue > 0 ? "+" : "")}{addValue}</color>";
        }

        if (multiplyValue != 0)
        {
            string color = multiplyValue > 0 ? "green" : "red";
            multiplyText = $" <color={color}>{(multiplyValue > 0 ? "*" : "")}{multiplyValue:F2}</color>";
        }

        return $"{baseValue}{addText}{multiplyText}";
    }
}

public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private System.Action<int> onEnter;
    private System.Action onExit;
    private int index;

    public void Init(System.Action<int> onEnter, System.Action onExit, int index)
    {
        this.onEnter = onEnter;
        this.onExit = onExit;
        this.index = index;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
    }
}
