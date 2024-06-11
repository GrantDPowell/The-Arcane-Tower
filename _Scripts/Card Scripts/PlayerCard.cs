using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCard", menuName = "ScriptableObjects/PlayerCard", order = 1)]
public class PlayerCard : ScriptableObject
{
    [Header("Card Info")]
    public string cardName;
    public Rarity rarity; // Add rarity

    [Header("Stat Additive Modifiers")]
    public int addStrength;
    public int addDexterity;
    public int addConstitution;
    public int addIntelligence;
    public int addWisdom;
    public int addCharisma;

    [Header("Stat Multiplicative Modifiers")]
    public float multiplyStrength;
    public float multiplyDexterity;
    public float multiplyConstitution;
    public float multiplyIntelligence;
    public float multiplyWisdom;
    public float multiplyCharisma;
    [Header("Cost")]
    public int cost;

    [Header("Prerequisites and Incompatibilities")]
    public PlayerCard prerequisite;
    public List<PlayerCard> incompatibleCards = new List<PlayerCard>();
    
}
