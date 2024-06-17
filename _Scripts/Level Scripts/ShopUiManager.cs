using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public GameObject shopPanel;
    public Transform spellCardList;
    public Transform playerCardList;
    public GameObject cardItemPrefab;
    public CampManager campManager;
    public LoadoutUIManager loadoutUIManager; // Reference to LoadoutUIManager

    public PlayerStats playerStats;
    public TMP_Text gemTxt;

    private void Start()
    {
        PopulateShop();
    }

    public void PopulateShop()
    {
        RemoveDuplicateCards();
        gemTxt.text = "Gems: " + playerStats.gems;
        // Clear existing items
        ClearChildItems(spellCardList);
        ClearChildItems(playerCardList);

        // Populate spell cards
        int i = 0;
        Transform[] childListSpells = GetChildTransforms(spellCardList);
        foreach (var card in campManager.availableSpellCards)
        {
            if (CanDisplayCard(card) && i < childListSpells.Length)
            {
                Transform child = childListSpells[i];
                GameObject item = Instantiate(cardItemPrefab, child);
                item.GetComponent<CardItem>().Initialize(card, "Spell");
                i++;
            }
        }

        // Populate player cards
        i = 0;
        Transform[] childListPlayer = GetChildTransforms(playerCardList);
        foreach (var card in campManager.availablePlayerCards)
        {
            if (CanDisplayCard(card) && i < childListPlayer.Length)
            {
                Transform child = childListPlayer[i];
                GameObject item = Instantiate(cardItemPrefab, child);
                item.GetComponent<CardItem>().Initialize(card, "Player");
                i++;
            }
        }
    }

    private void RemoveDuplicateCards()
    {
        // Create temporary lists to store cards to be removed
        List<SpellCard> spellCardsToRemove = new List<SpellCard>();
        List<PlayerCard> playerCardsToRemove = new List<PlayerCard>();

        // Check for duplicate spell cards
        foreach (var card in campManager.availableSpellCards)
        {
            if (campManager.playerStats.savedLoadoutSpellCards.Contains(card))
            {
                spellCardsToRemove.Add(card);
            }
        }

        // Remove duplicate spell cards
        foreach (var card in spellCardsToRemove)
        {
            campManager.availableSpellCards.Remove(card);
        }

        // Check for duplicate player cards
        foreach (var card in campManager.availablePlayerCards)
        {
            if (campManager.playerStats.savedLoadoutPlayerCards.Contains(card))
            {
                playerCardsToRemove.Add(card);
            }
        }

        // Remove duplicate player cards
        foreach (var card in playerCardsToRemove)
        {
            campManager.availablePlayerCards.Remove(card);
        }
    }

    public void BuySpellCard(SpellCard card)
    {
        campManager.BuySpellCard(card);
        PopulateShop();
    }

    public void BuyPlayerCard(PlayerCard card)
    {
        campManager.BuyPlayerCard(card);
        PopulateShop();
    }

    private bool CanDisplayCard(SpellCard card)
    {
        if (card.prerequisite != null && !campManager.playerStats.savedLoadoutSpellCards.Contains(card.prerequisite))
        {
            return false;
        }

        if (campManager.playerStats.savedLoadoutSpellCards.Contains(card))
        {
            return false;
        }

        return true;
    }

    private bool CanDisplayCard(PlayerCard card)
    {
        if (card.prerequisite != null && !campManager.playerStats.savedLoadoutPlayerCards.Contains(card.prerequisite))
        {
            return false;
        }

        if (campManager.playerStats.savedLoadoutPlayerCards.Contains(card))
        {
            return false;
        }

        return true;
    }

    private void ClearChildItems(Transform parent)
    {
        foreach (Transform child in parent)
        {
            foreach (Transform grandChild in child)
            {
                Destroy(grandChild.gameObject);
            }
        }
    }

    private Transform[] GetChildTransforms(Transform parent)
    {
        List<Transform> childTransforms = new List<Transform>();
        foreach (Transform child in parent)
        {
            if (child != parent)
            {
                childTransforms.Add(child);
            }
        }
        return childTransforms.ToArray();
    }

    public void ShowLoadout()
    {
        shopPanel.SetActive(false);
        loadoutUIManager.ShowLoadout();
    }

    public void HideLoadout()
    {
        loadoutUIManager.HideLoadout();
        shopPanel.SetActive(true);
    }

    public void RefundSpellCard(SpellCard card)
    {
        Debug.Log("Refund Spell Card OLD IMPLEMENTATION");
        //campManager.RefundSpellCard(card);
        //loadoutUIManager.PopulateLoadout();
    }

    public void RefundPlayerCard(PlayerCard card)
    {
        //campManager.RefundPlayerCard(card);
        //loadoutUIManager.PopulateLoadout();
    }
}
