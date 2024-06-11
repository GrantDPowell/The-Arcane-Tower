using System.Collections.Generic;
using UnityEngine;

public class PlayerLoadoutSystem : MonoBehaviour
{
    public PlayerStats playerStats;
    private SpellSystem spellSystem;

    private void Awake()
    {
        spellSystem = GetComponentInChildren<SpellSystem>();
    }

    private void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is not assigned in PlayerLoadoutSystem.");
            return;
        }

        spellSystem.playerStats = playerStats;
        LoadLoadoutFromStats();
        ApplyLoadoutModifiers();
    }

    public void ApplyLoadoutModifiers()
    {
        playerStats.activeSpellCards.Clear();
        playerStats.activePlayerCards.Clear();

        foreach (var card in playerStats.savedLoadoutSpellCards)
        {
            if (CanActivateSpellCard(card))
            {
                playerStats.activeSpellCards.Add(card);
            }
        }

        foreach (var card in playerStats.savedLoadoutPlayerCards)
        {
            if (playerStats.CanActivatePlayerCard(card))
            {
                playerStats.ApplyPlayerCard(card);
            }
        }

        ApplyCards();
    }

    public void SaveLoadoutToStats()
    {
        playerStats.savedLoadoutSpellCards = new List<SpellCard>(playerStats.activeSpellCards);
        playerStats.savedLoadoutPlayerCards = new List<PlayerCard>(playerStats.activePlayerCards);
    }

    public void LoadLoadoutFromStats()
    {
        playerStats.activeSpellCards = new List<SpellCard>(playerStats.savedLoadoutSpellCards);
        playerStats.activePlayerCards = new List<PlayerCard>(playerStats.savedLoadoutPlayerCards);
    }

    private bool CanActivateSpellCard(SpellCard card)
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

    private void ApplyCards()
    {
        if (spellSystem != null)
        {
            spellSystem.ApplyCards();
        }

        playerStats.ApplyCards();
    }
}
