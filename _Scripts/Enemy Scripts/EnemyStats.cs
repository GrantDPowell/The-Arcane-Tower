using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats", order = 1)]
public class EnemyStats : ScriptableObject
{
    [Header("Base Stats")]
    public int baseStrength; // defense
    public int baseDexterity; // move speed, attack cooldown
    public int baseConstitution; // health
    public int baseIntelligence; // damage
    public int baseWisdom; // spell speed and range

    public int baseExperiencePoints; // base XP drop
    public int baseGoldDrop; // base gold drop

    [Header("Scaling Factors")]
    public int strengthPerLevel;
    public int dexterityPerLevel;
    public int constitutionPerLevel;
    public int intelligencePerLevel;
    public int wisdomPerLevel;

    public int experiencePointsPerLevel;
    public int goldDropPerLevel;

    [Header("Current Stats")]
    public int currentStrength;
    public int currentDexterity;
    public int currentConstitution;
    public int currentIntelligence;
    public int currentWisdom;

    public int currentExperiencePoints;
    public int currentGoldDrop;

    private void OnEnable()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        currentStrength = baseStrength;
        currentDexterity = baseDexterity;
        currentConstitution = baseConstitution;
        currentIntelligence = baseIntelligence;
        currentWisdom = baseWisdom;
        currentExperiencePoints = baseExperiencePoints;
        currentGoldDrop = baseGoldDrop;
    }

    public void ScaleStats(int playerLevel)
    {
        currentStrength = baseStrength + (strengthPerLevel * playerLevel);
        currentDexterity = baseDexterity + (dexterityPerLevel * playerLevel);
        currentConstitution = baseConstitution + (constitutionPerLevel * playerLevel);
        currentIntelligence = baseIntelligence + (intelligencePerLevel * playerLevel);
        currentWisdom = baseWisdom + (wisdomPerLevel * playerLevel);
        currentExperiencePoints = baseExperiencePoints + (experiencePointsPerLevel * playerLevel);
        currentGoldDrop = baseGoldDrop + (goldDropPerLevel * playerLevel);
    }

    public float GetDefense()
    {
        return currentStrength * 0.1f;
    }

    public float GetMoveSpeed()
    {
        return currentDexterity * 0.1f;
    }

    public float GetAttackCooldown()
    {
        return currentDexterity * 0.01f;
    }

    public float GetHealth()
    {
        return currentConstitution * 10;
    }

    public float GetDamage()
    {
        return currentIntelligence * 0.1f;
    }

    public float GetSpellSpeed()
    {
        return currentWisdom * 0.4f;
    }

    public float GetSpellRange()
    {
        return currentWisdom * 1f;
    }

    public int GetExperiencePoints()
    {
        return currentExperiencePoints;
    }
    public int GetGoldDrop()
    {
        return currentGoldDrop;
    }
}
