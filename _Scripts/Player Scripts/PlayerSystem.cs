using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    public PlayerStats playerStats;
    private PlayerLevelSystem playerLevelSystem;

    private void Awake()
    {
        playerLevelSystem = GetComponent<PlayerLevelSystem>();
        if (playerLevelSystem == null)
        {
            Debug.LogError("PlayerLevelSystem component not found on PlayerSystem!");
        }
    }

    public void GainExperience(int amount)
    {
        playerLevelSystem.GainExperience(amount); // Delegate experience gain to PlayerLevelSystem
    }
}
