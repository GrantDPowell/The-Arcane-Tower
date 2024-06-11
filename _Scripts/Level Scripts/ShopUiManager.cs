using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public GameObject shopPanel;
    public Transform spellCardList;
    public Transform playerCardList;
    public GameObject cardItemPrefab;
    public CampManager campManager;

    private void Start()
    {
        PopulateShop();
    }

    public void PopulateShop()
    {
        // Clear existing items
        foreach (Transform child in spellCardList)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in playerCardList)
        {
            Destroy(child.gameObject);
        }

        // Populate spell cards
        foreach (var card in campManager.availableSpellCards)
        {
            GameObject item = Instantiate(cardItemPrefab, spellCardList);
            item.GetComponent<CardItem>().Initialize(card, "Spell");
        }

        // Populate player cards
        foreach (var card in campManager.availablePlayerCards)
        {
            GameObject item = Instantiate(cardItemPrefab, playerCardList);
            item.GetComponent<CardItem>().Initialize(card, "Player");
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
}
