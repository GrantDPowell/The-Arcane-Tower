using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
public class PlayerStats : ScriptableObject
{
    [Header("Base Stats")]
    public int baseStrength;
    public int baseDexterity;
    public int baseConstitution;
    public int baseIntelligence;
    public int baseWisdom;
    public int baseCharisma;

    [Header("Experience")]
    public int xpCurve;
    public int experiencePoints;
    public int level;

    [Header("Current Stats")]
    public int currentStrength;
    public int currentDexterity;
    public int currentConstitution;
    public int currentIntelligence;
    public int currentWisdom;
    public int currentCharisma;
    
    [Header("Health")]
    public float currentHealth;
    public float maxHealth;
    
    [Header("Gems/Coins")]
    public int gems;
    public int coins;

    [Header("Active Cards")]
    public List<PlayerCard> activePlayerCards = new List<PlayerCard>();
    public List<SpellCard> activeSpellCards = new List<SpellCard>();
    [Header("Saved Loadout")]
    public List<SpellCard> savedLoadoutSpellCards = new List<SpellCard>();
    public List<PlayerCard> savedLoadoutPlayerCards = new List<PlayerCard>();

    

    private void OnEnable()
    {
        // ResetStats(); Removed to prevent resetting stats on every play
        // Instead, Stats are reset upon death
    }

    public void ResetStats()
    {
        currentStrength = baseStrength;
        currentDexterity = baseDexterity;
        currentConstitution = baseConstitution;
        currentIntelligence = baseIntelligence;
        currentWisdom = baseWisdom;
        currentCharisma = baseCharisma;

        experiencePoints = 0;
        level = 1;

        maxHealth = currentConstitution * 10;
        currentHealth = maxHealth;

        activePlayerCards.Clear();
        activeSpellCards.Clear();
    }

    public int ExperienceThreshold()
    {
        return level * xpCurve; // Example threshold calculation
    }

    public void ApplyPlayerCard(PlayerCard card)
    {
        if (CanActivatePlayerCard(card) && !activePlayerCards.Contains(card))
        {
            activePlayerCards.Add(card);
            ApplyCards();
        }
        else
        {
            Debug.Log($"Cannot apply card: {card.cardName}. Either prerequisites not met or already active.");
        }
    }

    public void ApplyCards()
    {
        int additiveStrength = 0;
        int additiveDexterity = 0;
        int additiveConstitution = 0;
        int additiveIntelligence = 0;
        int additiveWisdom = 0;
        int additiveCharisma = 0;

        float multiplicativeStrength = 1.0f;
        float multiplicativeDexterity = 1.0f;
        float multiplicativeConstitution = 1.0f;
        float multiplicativeIntelligence = 1.0f;
        float multiplicativeWisdom = 1.0f;
        float multiplicativeCharisma = 1.0f;

        foreach (var card in activePlayerCards)
        {
            additiveStrength += card.addStrength;
            additiveDexterity += card.addDexterity;
            additiveConstitution += card.addConstitution;
            additiveIntelligence += card.addIntelligence;
            additiveWisdom += card.addWisdom;
            additiveCharisma += card.addCharisma;

            multiplicativeStrength += (card.multiplyStrength);
            multiplicativeDexterity += (card.multiplyDexterity);
            multiplicativeConstitution += (card.multiplyConstitution);
            multiplicativeIntelligence += (card.multiplyIntelligence);
            multiplicativeWisdom += (card.multiplyWisdom);
            multiplicativeCharisma += (card.multiplyCharisma);
        }

        currentStrength = Mathf.RoundToInt((baseStrength + additiveStrength) * multiplicativeStrength);
        currentDexterity = Mathf.RoundToInt((baseDexterity + additiveDexterity) * multiplicativeDexterity);
        currentConstitution = Mathf.RoundToInt((baseConstitution + additiveConstitution) * multiplicativeConstitution);
        currentIntelligence = Mathf.RoundToInt((baseIntelligence + additiveIntelligence) * multiplicativeIntelligence);
        currentWisdom = Mathf.RoundToInt((baseWisdom + additiveWisdom) * multiplicativeWisdom);
        currentCharisma = Mathf.RoundToInt((baseCharisma + additiveCharisma) * multiplicativeCharisma);

        maxHealth = currentConstitution * 10;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (currentHealth <= maxHealth * 0.6f)
        {
            currentHealth += maxHealth * 0.2f;
        }
    }

    public bool CanActivatePlayerCard(PlayerCard card)
    {
        if (card.prerequisite != null && !activePlayerCards.Contains(card.prerequisite))
        {
            Debug.Log($"Card {card.cardName} cannot be activated. Missing prerequisite: {card.prerequisite.cardName}");
            return false;
        }

        foreach (var incompatibleCard in card.incompatibleCards)
        {
            if (activePlayerCards.Contains(incompatibleCard))
            {
                Debug.Log($"Card {card.cardName} cannot be activated. Incompatible with: {incompatibleCard.cardName}");
                return false;
            }
        }

        return true;
    }

    public void AddStrength(int amount) => currentStrength += amount;
    public void MultiplyStrength(float multiplier) => currentStrength = Mathf.RoundToInt(currentStrength * multiplier);
    public void AddDexterity(int amount) => currentDexterity += amount;
    public void MultiplyDexterity(float multiplier) => currentDexterity = Mathf.RoundToInt(currentDexterity * multiplier);
    public void AddConstitution(int amount) => currentConstitution += amount;
    public void MultiplyConstitution(float multiplier) => currentConstitution = Mathf.RoundToInt(currentConstitution * multiplier);
    public void AddIntelligence(int amount) => currentIntelligence += amount;
    public void MultiplyIntelligence(float multiplier) => currentIntelligence = Mathf.RoundToInt(currentIntelligence * multiplier);
    public void AddWisdom(int amount) => currentWisdom += amount;
    public void MultiplyWisdom(float multiplier) => currentWisdom = Mathf.RoundToInt(currentWisdom * multiplier);
    public void AddCharisma(int amount) => currentCharisma += amount;
    public void MultiplyCharisma(float multiplier) => currentCharisma = Mathf.RoundToInt(currentCharisma * multiplier);

    // Methods to get modified stats
    public float GetDefense() => currentStrength * 0.05f;
    public float GetMoveSpeed() => currentDexterity * 0.1f;
    public float GetAttackCooldown() => currentDexterity * 0.05f;
    public float GetMagicDamage() => currentIntelligence * 0.1f;
    public float GetSpellSpeed() => currentWisdom * 0.15f;
    public float GetSpellRange() => currentWisdom * 1f;
    public float GetShopDiscount() => currentCharisma * 0.01f;
}
