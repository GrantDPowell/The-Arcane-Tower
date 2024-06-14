using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LoadoutUIManager : MonoBehaviour
{
    public GameObject loadoutPanel; // Reference to the loadout panel
    public Transform[] loadoutPageParents; // Array of parents for loadout pages
    public GameObject loadoutCardItemPrefab; // CardItem prefab for loadout
    public CampManager campManager;
    public Button nextPageButton;
    public Button previousPageButton;
    public TMP_Text pageNumberText; // Use TMP_Text instead of Text

    public TMP_Text gemTxt; // Use TMP_Text instead of Text

    private int currentPage = 0;

    private void Start()
    {
        PopulateLoadout();
        UpdatePageNavigation();
    }

    public void PopulateLoadout()
    {
        // Clear existing items in loadout pages
        foreach (var pageParent in loadoutPageParents)
        {
            ClearChildItems(pageParent);
        }

        // Populate loadout cards
        List<SpellCard> sortedSpellCards = new List<SpellCard>(campManager.playerStats.savedLoadoutSpellCards);
        List<PlayerCard> sortedPlayerCards = new List<PlayerCard>(campManager.playerStats.savedLoadoutPlayerCards);

        // Sort cards to place cards with prerequisites last
        sortedSpellCards.Sort((a, b) => (b.prerequisite != null).CompareTo(a.prerequisite != null));
        sortedPlayerCards.Sort((a, b) => (b.prerequisite != null).CompareTo(a.prerequisite != null));

        

        int index = 0;
        foreach (var card in sortedSpellCards)
        {
            int pageIndex = index / 24;
            int cardIndex = index % 24;
            if (pageIndex < loadoutPageParents.Length)
            {
                if (CanRefundSpellCard(card))
                {
                    Transform child = loadoutPageParents[pageIndex].GetChild(cardIndex);
                    GameObject item = Instantiate(loadoutCardItemPrefab, child);
                    item.GetComponent<CardItemForLoadout>().Initialize(card, "Spell");
                    index++;
                }
                // Transform child = loadoutPageParents[pageIndex].GetChild(cardIndex);
                // GameObject item = Instantiate(loadoutCardItemPrefab, child);
                // item.GetComponent<CardItemForLoadout>().Initialize(card, "Spell");
                // index++;
                // TODO REMOVE OLD CODE 
            }
        }

        //index = 0;
        foreach (var card in sortedPlayerCards)
        {
            int pageIndex = index / 24;
            int cardIndex = index % 24;
            if (pageIndex < loadoutPageParents.Length)
            {
                if (CanRefundPlayerCard(card))
                {
                    Transform child = loadoutPageParents[pageIndex].GetChild(cardIndex);
                    GameObject item = Instantiate(loadoutCardItemPrefab, child);
                    item.GetComponent<CardItemForLoadout>().Initialize(card, "Player");
                    index++;
                }
                // Transform child = loadoutPageParents[pageIndex].GetChild(cardIndex);
                // GameObject item = Instantiate(loadoutCardItemPrefab, child);
                // item.GetComponent<CardItemForLoadout>().Initialize(card, "Player");
                // index++;
                // TODO REMOVE OLD CODE
            }
        }

        UpdatePageNavigation();
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

    public void ShowLoadout()
    {
        loadoutPanel.SetActive(true);
        PopulateLoadout();
        UpdatePageNavigation();
    }

    public void HideLoadout()
    {
        loadoutPanel.SetActive(false);
    }

    public void RefundSpellCard(SpellCard card)
    {
        campManager.RefundSpellCard(card);
        PopulateLoadout();
    }

    public void RefundPlayerCard(PlayerCard card)
    {
        campManager.RefundPlayerCard(card);
        PopulateLoadout();
    }

    public void NextPage()
    {
        if (currentPage < loadoutPageParents.Length - 1)
        {
            currentPage++;
            UpdatePageNavigation();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePageNavigation();
        }
    }

    private void UpdatePageNavigation()
    {
        for (int i = 0; i < loadoutPageParents.Length; i++)
        {
            loadoutPageParents[i].gameObject.SetActive(i == currentPage);
        }

        pageNumberText.text = $"Page {currentPage + 1} / {loadoutPageParents.Length}";

        previousPageButton.interactable = currentPage > 0;
        nextPageButton.interactable = currentPage < loadoutPageParents.Length - 1;

        gemTxt.text = "Gems: " + campManager.playerStats.gems;
    }

    private bool CanRefundSpellCard(SpellCard card)
    {
        foreach (var savedCard in campManager.playerStats.savedLoadoutSpellCards)
        {
            if (savedCard.prerequisite == card)
            {
                return false; // Cannot refund if another card has this as a prerequisite
            }
        }
        return true;
    }

    private bool CanRefundPlayerCard(PlayerCard card)
    {
        foreach (var savedCard in campManager.playerStats.savedLoadoutPlayerCards)
        {
            if (savedCard.prerequisite == card)
            {
                return false; // Cannot refund if another card has this as a prerequisite
            }
        }
        return true;
    }
}
