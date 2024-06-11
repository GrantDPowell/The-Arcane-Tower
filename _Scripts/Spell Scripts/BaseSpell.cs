using UnityEngine;

[CreateAssetMenu(fileName = "BaseSpell", menuName = "Spells/BaseSpell")]
public class BaseSpell : ScriptableObject
{
    public string spellName;
    public GameObject spellPrefab;

    public float baseDamage;
    public float baseSpeed;
    public float baseRange;
    public float attackCooldown;
    public string animationTrigger;
}
