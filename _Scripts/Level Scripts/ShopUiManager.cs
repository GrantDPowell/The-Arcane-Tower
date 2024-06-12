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
            //child.gameObject.SetActive(false);
            //Destroy(child.gameObject);
        }

        foreach (Transform child in playerCardList)
        {
            //child.gameObject.SetActive(false);
            //Destroy(child.gameObject);
        }

        // Populate spell cards
        int i = 0;
        foreach (var card in campManager.availableSpellCards)
        {
            // place the card on each child of the spell card list
            Transform[] childList = spellCardList.GetComponentsInChildren<Transform>();
            Transform child = childList[i];          
            
            GameObject item = Instantiate(cardItemPrefab, child);
            item.GetComponent<CardItem>().Initialize(card, "Spell");
        
            i ++;
        
        }

        // Populate player cards
        i = 0;
        foreach (var card in campManager.availablePlayerCards)
        {
            Transform[] childList = playerCardList.GetComponentsInChildren<Transform>();
            Transform child = childList[i]; 

            GameObject item = Instantiate(cardItemPrefab, playerCardList);
            item.GetComponent<CardItem>().Initialize(card, "Player");

            i ++;
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
