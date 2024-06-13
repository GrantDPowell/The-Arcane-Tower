using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CampManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    private GameObject playerInstance;

    public List<LevelData> levels;
    public GameObject shopUI;
    public GameObject loadoutPanel; // Reference to the loadout panel
    public GameObject notEnoughGemsText;
    public PlayerStats playerStats;

    public List<SpellCard> availableSpellCards;
    public List<PlayerCard> availablePlayerCards;

    void Start()
    {
        SpawnPlayer();
        CloseShop();
        HideNotEnoughGemsText();
        if (loadoutPanel != null)
        {
            loadoutPanel.SetActive(false);
        }
    }

    void SpawnPlayer()
    {
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            playerInstance = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
        }

        var playerLoadoutSystem = playerInstance.GetComponent<PlayerLoadoutSystem>();
        playerLoadoutSystem.playerStats = playerStats;

        var spellSystem = playerInstance.GetComponentInChildren<SpellSystem>();
        spellSystem.playerStats = playerStats;

        playerLoadoutSystem.LoadLoadoutFromStats();
        playerLoadoutSystem.ApplyLoadoutModifiers();
    }

    public void SelectLevel(LevelData levelData)
    {
        if (levelData != null)
        {
            var playerLoadoutSystem = playerInstance.GetComponent<PlayerLoadoutSystem>();
            playerLoadoutSystem.SaveLoadoutToStats();
            SceneManager.LoadScene(levelData.sceneName);
        }
    }

    public void OpenShop()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(true);
            shopUI.GetComponent<ShopUIManager>().PopulateShop();
        }
    }

    public void CloseShop()
    {
        if (shopUI != null)
        {
            shopUI.SetActive(false);
        }
    }

    public void ShowLoadout()
    {
        if (loadoutPanel != null)
        {
            loadoutPanel.SetActive(true);
            loadoutPanel.GetComponent<LoadoutUIManager>().PopulateLoadout();
            CloseShop();
        }
    }

    public void CloseLoadout()
    {
        if (loadoutPanel != null)
        {
            loadoutPanel.SetActive(false);
            OpenShop();
        }
    }

    public void BuySpellCard(SpellCard card)
    {
        if (playerStats.gems >= card.cost)
        {
            playerStats.gems -= card.cost;
            playerStats.savedLoadoutSpellCards.Add(card);
            var playerLoadoutSystem = playerInstance.GetComponent<PlayerLoadoutSystem>();
            playerLoadoutSystem.LoadLoadoutFromStats();
            playerLoadoutSystem.ApplyLoadoutModifiers();
            availableSpellCards.Remove(card);
        }
        else
        {
            ShowNotEnoughGemsText();
        }
    }

    public void BuyPlayerCard(PlayerCard card)
    {
        if (playerStats.gems >= card.cost)
        {
            playerStats.gems -= card.cost;
            playerStats.savedLoadoutPlayerCards.Add(card);
            var playerLoadoutSystem = playerInstance.GetComponent<PlayerLoadoutSystem>();
            playerLoadoutSystem.LoadLoadoutFromStats();
            playerLoadoutSystem.ApplyLoadoutModifiers();
            availablePlayerCards.Remove(card);
        }
        else
        {
            ShowNotEnoughGemsText();
        }
    }

    public void RefundSpellCard(SpellCard card)
    {
        if (CanRefundSpellCard(card))
        {
            playerStats.gems += card.cost;
            playerStats.savedLoadoutSpellCards.Remove(card);
            var playerLoadoutSystem = playerInstance.GetComponent<PlayerLoadoutSystem>();
            playerLoadoutSystem.LoadLoadoutFromStats();
            playerLoadoutSystem.ApplyLoadoutModifiers();
            availableSpellCards.Add(card);
        }
    }

    public void RefundPlayerCard(PlayerCard card)
    {
        if (CanRefundPlayerCard(card))
        {
            playerStats.gems += card.cost;
            playerStats.savedLoadoutPlayerCards.Remove(card);
            var playerLoadoutSystem = playerInstance.GetComponent<PlayerLoadoutSystem>();
            playerLoadoutSystem.LoadLoadoutFromStats();
            playerLoadoutSystem.ApplyLoadoutModifiers();
            availablePlayerCards.Add(card);
        }
    }

    private bool CanRefundSpellCard(SpellCard card)
    {
        foreach (var savedCard in playerStats.savedLoadoutSpellCards)
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
        foreach (var savedCard in playerStats.savedLoadoutPlayerCards)
        {
            if (savedCard.prerequisite == card)
            {
                return false; // Cannot refund if another card has this as a prerequisite
            }
        }
        return true;
    }

    public void ShowNotEnoughGemsText()
    {
        if (notEnoughGemsText != null)
        {
            notEnoughGemsText.SetActive(true);
            StartCoroutine(RemoveAfterSeconds(3, notEnoughGemsText));
        }
    }

    public void HideNotEnoughGemsText()
    {
        if (notEnoughGemsText != null)
        {
            notEnoughGemsText.SetActive(false);
        }
    }

    IEnumerator RemoveAfterSeconds(int seconds, GameObject obj)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }
}
