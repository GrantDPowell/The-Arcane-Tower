using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelSystem : MonoBehaviour
{
    public PlayerStats playerStats;
    public List<SpellCard> allSpellCards = new List<SpellCard>();
    public List<PlayerCard> allPlayerCards = new List<PlayerCard>();

    private List<SpellCard> availableSpellCards = new List<SpellCard>();
    private List<PlayerCard> availablePlayerCards = new List<PlayerCard>();

    public GameObject levelUpUI; // Reference to the Level Up UI GameObject
    public SpellSystem spellSystem;

    public Dictionary<Rarity, float> rarityWeights = new Dictionary<Rarity, float> // Define weights for each rarity
    {
        { Rarity.Common, 60f },
        { Rarity.Rare, 25f },
        { Rarity.Epic, 10f },
        { Rarity.Legendary, 5f }
    };

    private Queue<int> levelUpQueue = new Queue<int>();

    private void Start()
    {
        InitializeLevelUpUI(); // Ensure initialization on start
        RefreshAvailableCards(); // Initialize available cards
    }

    public void GainExperience(int amount)
    {
        playerStats.experiencePoints += amount;
        while (playerStats.experiencePoints >= playerStats.ExperienceThreshold())
        {
            playerStats.experiencePoints -= playerStats.ExperienceThreshold();
            levelUpQueue.Enqueue(1); // Queue a level-up
        }
        ProcessLevelUps();
    }

    private void ProcessLevelUps()
    {
        if (levelUpQueue.Count > 0 && !levelUpUI.activeSelf)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        playerStats.level++;
        RefreshAvailableCards();
        ShowLevelUpOptions();
    }

    private void ShowLevelUpOptions()
    {
        //Debug.Log("ShowLevelUpOptions: Validating cards");

        if (levelUpUI == null)
        {
            Debug.LogError("levelUpUI is not assigned!");
            return;
        }

        // Select 3 random valid cards (either spell or player) based on rarity and display them in the UI
        List<object> options = new List<object>();
        options.AddRange(availableSpellCards);
        options.AddRange(availablePlayerCards);

        List<object> randomOptions = GetRandomOptions(options, 3);

        var levelUpUIScript = levelUpUI.GetComponent<LevelUpUI>();
        if (levelUpUIScript == null)
        {
            Debug.LogError("LevelUpUI component not found on the assigned levelUpUI GameObject!");
            return;
        }

        levelUpUIScript.ShowOptions(randomOptions);
        levelUpUI.SetActive(true);
    }

    private List<T> GetValidCards<T>(List<T> allCards, List<T> activeCards) where T : ScriptableObject
    {
        if (allCards == null)
        {
            Debug.LogError("allCards list is null");
            return new List<T>();
        }

        List<T> validCards = new List<T>();
        foreach (T card in allCards)
        {
            if (!activeCards.Contains(card) && CanActivateCard(card, activeCards))
            {
                validCards.Add(card);
            }
        }
        return validCards;
    }

    private bool CanActivateCard<T>(T card, List<T> activeCards) where T : ScriptableObject
    {
        if (card is SpellCard spellCard)
        {
            if (spellCard.prerequisite != null && !activeCards.Contains(spellCard.prerequisite as T))
            {
                return false;
            }

            foreach (var incompatibleCard in spellCard.incompatibleCards)
            {
                if (activeCards.Contains(incompatibleCard as T))
                {
                    return false;
                }
            }
        }
        else if (card is PlayerCard playerCard)
        {
            if (playerCard.prerequisite != null && !activeCards.Contains(playerCard.prerequisite as T))
            {
                return false;
            }

            foreach (var incompatibleCard in playerCard.incompatibleCards)
            {
                if (activeCards.Contains(incompatibleCard as T))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void ApplyCard(object card)
    {
        if (card is SpellCard spellCard)
        {
            spellSystem.AddCard(spellCard);
            availableSpellCards.Remove(spellCard); // Remove selected card from the pool
        }
        else if (card is PlayerCard playerCard)
        {
            playerStats.ApplyPlayerCard(playerCard);
            availablePlayerCards.Remove(playerCard); // Remove selected card from the pool
        }
        levelUpUI.SetActive(false);

        if (levelUpQueue.Count > 0)
        {
            levelUpQueue.Dequeue();
            ProcessLevelUps();
        }
    }

    public void InitializeLevelUpUI()
    {
        if (levelUpUI == null)
        {
            Debug.LogError("levelUpUI is not assigned in the Inspector!");
        }
        else
        {
            levelUpUI.SetActive(false);
        }
    }

    public void RefreshAvailableCards()
    {
        availableSpellCards = GetValidCards(allSpellCards, playerStats.activeSpellCards);
        availablePlayerCards = GetValidCards(allPlayerCards, playerStats.activePlayerCards);
    }

    private List<object> GetRandomOptions(List<object> options, int count)
    {
        List<object> selectedOptions = new List<object>();

        while (selectedOptions.Count < count && options.Count > 0)
        {
            float totalWeight = 0;
            foreach (var option in options)
            {
                totalWeight += GetWeight(option);
            }

            float randomValue = Random.Range(0, totalWeight);
            float cumulativeWeight = 0;

            foreach (var option in options)
            {
                cumulativeWeight += GetWeight(option);
                if (randomValue < cumulativeWeight)
                {
                    selectedOptions.Add(option);
                    options.Remove(option);
                    break;
                }
            }
        }

        return selectedOptions;
    }

    private float GetWeight(object card)
    {
        if (card is SpellCard spellCard)
        {
            return rarityWeights[spellCard.rarity];
        }
        else if (card is PlayerCard playerCard)
        {
            return rarityWeights[playerCard.rarity];
        }
        return 1;
    }
}
