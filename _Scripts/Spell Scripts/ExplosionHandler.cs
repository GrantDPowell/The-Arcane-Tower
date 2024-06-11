using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    public float explosionLifetime = 2f; // Duration before the explosion is destroyed

    void Start()
    {
        // Destroy the explosion object after the specified lifetime
        Destroy(gameObject, explosionLifetime);
    }
}
