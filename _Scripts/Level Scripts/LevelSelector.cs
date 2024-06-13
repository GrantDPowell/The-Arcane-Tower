using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public LevelData levelData;



    void OnMouseDown()
    {
        CampManager campManager = FindObjectOfType<CampManager>();
        if (campManager != null)
        {
            // see if player is close nough to the level to select it
            PlayerMovementSystem player = FindObjectOfType<PlayerMovementSystem>();
            if (player != null)
            {
                if (Vector3.Distance(player.transform.position, transform.position) > 5)
                {
                    Debug.Log("Player is too far away to select level");
                    return;
                }
                else
                {
                    Debug.Log("Player is close enough to select level");
                    campManager.SelectLevel(levelData);
                }
            }
            else 
            {
                Debug.Log("Player not found");
            }
            
        }
        else
        {
            Debug.Log("CampManager not found");
        }
    }
}
