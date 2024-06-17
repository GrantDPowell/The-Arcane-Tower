using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public List<GameObject> xpPrefabs; // Prefabs for XP drops
    public List<GameObject> gemPrefabs; // Prefabs for gem drops
    public List<GameObject> goldPrefabs; // Prefabs for gold drops

    public void DropItems(Vector3 position, int xpAmount, int gemAmount, int goldAmount)
    {
        DropSpecificItems(position, xpAmount, xpPrefabs);
        DropSpecificItems(position, gemAmount, gemPrefabs);
        DropSpecificItems(position, goldAmount, goldPrefabs);
    }
  

    private void DropSpecificItems(Vector3 position, int amount, List<GameObject> prefabs)
    {
        int remainingAmount = amount;

        for (int i = prefabs.Count - 1; i >= 0; i--)
        {
            GameObject prefab = prefabs[i];
            int prefabValue = (int)Mathf.Pow(2, i); // Assuming the prefabs are in powers of 2

            while (remainingAmount >= prefabValue)
            {
                Instantiate(prefab, position + Random.insideUnitSphere * 1.5f, Quaternion.identity);
                remainingAmount -= prefabValue;
            }
        }
    }
}
