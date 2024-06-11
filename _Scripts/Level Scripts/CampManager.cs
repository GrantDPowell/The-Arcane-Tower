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
    public PlayerStats playerStats;

    public List<SpellCard> availableSpellCards;
    public List<PlayerCard> availablePlayerCards;

    void Start()
    {
        SpawnPlayer();
        CloseShop();
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
            Debug.Log("Not enough gems to buy this card.");
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
            Debug.Log("Not enough gems to buy this card.");
        }
    }
}
