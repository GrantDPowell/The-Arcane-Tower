using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    void OnMouseDown()
    {
        CampManager campManager = FindObjectOfType<CampManager>();
        if (campManager != null)
        {
            Debug.Log("Opening shop...");
            campManager.OpenShop();
        }
    }
}
