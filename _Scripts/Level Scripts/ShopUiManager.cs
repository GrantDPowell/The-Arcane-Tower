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

    private Transform[] childListSpells;
    private Transform[] childListPlayer;

    private void Start()
    {
        //PopulateShop();
    }

    public void PopulateShop()
    {
        // Clear existing items
        foreach (Transform child in spellCardList)
        {
            //child.gameObject.SetActive(false);
            // remove the old card item
            
            //Destroy(child.gameObject);
            // how do i remove the old card item?
            
        }

        foreach (Transform child in playerCardList)
        {
            //child.gameObject.SetActive(false);
            //Destroy(child.gameObject);
            // how do i remove the old card item?
        }

        // Populate spell cards
        int i = 0;
        Transform[] childListSpells = spellCardList.GetComponentsInChildren<Transform>();
        foreach (var card in campManager.availableSpellCards)
        {
            // place the card on each child of the spell card list
            
            Transform child = childListSpells[i];          
            
            GameObject item = Instantiate(cardItemPrefab, child);
            item.GetComponent<CardItem>().Initialize(card, "Spell");
        
            i ++;
        
        }

        // Populate player cards
        i = 0;
        Transform[] childListPlayer = playerCardList.GetComponentsInChildren<Transform>();
        foreach (var card in campManager.availablePlayerCards)
        {
            
            Transform child = childListPlayer[i]; 

            GameObject item = Instantiate(cardItemPrefab, child);
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
