using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellCard", menuName = "ScriptableObjects/SpellCard", order = 1)]
public class SpellCard : ScriptableObject
{
    [Header("Card Info")]
    public string cardName;
    public Rarity rarity;

    [Header("Stat Additive Modifiers")]
    public float addDamage;
    public float addSpeed;
    public float addRange;

    [Header("Stat Multiplicative Modifiers")]
    public float damageMultiplier;
    public float speedMultiplier;
    public float rangeMultiplier;

    [Header("Special Properties")]
    public bool isHoming;
    public int splitCount;
    public GameObject modifiedPrefab;
    [Header("Cost")]
    public int cost;

    [Header("Prerequisites and Incompatibilities")]
    public SpellCard prerequisite;
    public List<SpellCard> incompatibleCards = new List<SpellCard>();
}