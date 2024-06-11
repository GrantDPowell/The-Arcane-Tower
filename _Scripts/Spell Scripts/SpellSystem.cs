using System.Collections.Generic;
using UnityEngine;

public class SpellSystem : MonoBehaviour
{
    public BaseSpell baseSpell;
    public PlayerStats playerStats;

    private float totalDamage;
    private float totalSpeed;
    private float totalRange;
    private bool isHoming;
    private int splitCount;
    private GameObject currentPrefab;

    private void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is not assigned in SpellSystem.");
            return;
        }

        ApplyCards();
    }

    public void AddCard(SpellCard card)
    {
        if (!playerStats.activeSpellCards.Contains(card) && CanActivateSpellCard(card))
        {
            playerStats.activeSpellCards.Add(card);
            ApplyCards();
        }
    }

    public void RemoveCard(SpellCard card)
    {
        if (playerStats.activeSpellCards.Contains(card))
        {
            playerStats.activeSpellCards.Remove(card);
            ApplyCards();
        }
    }

    public void ApplyCards()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is not assigned in SpellSystem.");
            return;
        }

        float additiveDamage = 0f;
        float additiveSpeed = 0f;
        float additiveRange = 0f;

        float multiplicativeDamage = 1f;
        float multiplicativeSpeed = 1f;
        float multiplicativeRange = 1f;

        isHoming = false;
        splitCount = 0;
        currentPrefab = baseSpell.spellPrefab;

        foreach (var card in playerStats.activeSpellCards)
        {
            additiveDamage += card.addDamage;
            additiveSpeed += card.addSpeed;
            additiveRange += card.addRange;

            multiplicativeDamage += card.damageMultiplier;
            multiplicativeSpeed += card.speedMultiplier;
            multiplicativeRange += card.rangeMultiplier;

            if (card.isHoming)
            {
                isHoming = true;
            }

            if (card.splitCount > 0)
            {
                splitCount = card.splitCount;
                if (card.modifiedPrefab != null)
                {
                    currentPrefab = card.modifiedPrefab;
                }
            }
        }

        totalDamage = (baseSpell.baseDamage + additiveDamage) * multiplicativeDamage;
        totalSpeed = (baseSpell.baseSpeed + additiveSpeed) * multiplicativeSpeed;
        totalRange = (baseSpell.baseRange + additiveRange) * multiplicativeRange;
    }

    public bool CanActivateSpellCard(SpellCard card)
    {
        if (card.prerequisite != null && !playerStats.activeSpellCards.Contains(card.prerequisite))
        {
            return false;
        }

        foreach (var incompatibleCard in card.incompatibleCards)
        {
            if (playerStats.activeSpellCards.Contains(incompatibleCard))
            {
                return false;
            }
        }

        return true;
    }

    public (GameObject prefab, float damage, float speed, float range, bool isHoming, int splitCount) GetModifiedSpell()
    {
        ApplyCards();
        return (currentPrefab, totalDamage, totalSpeed, totalRange, isHoming, splitCount);
    }
}
