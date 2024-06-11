using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public LevelData levelData;

    void OnMouseDown()
    {
        CampManager campManager = FindObjectOfType<CampManager>();
        if (campManager != null)
        {
            campManager.SelectLevel(levelData);
        }
    }
}
