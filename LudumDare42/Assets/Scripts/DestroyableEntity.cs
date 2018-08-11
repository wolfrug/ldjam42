using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "DestroyableEntity", order = 1)]
public class DestroyableEntity : ScriptableObject {
    public string name_;

    public int health_;
    public float dodge_;
    public float attackRating_;
    public float criticalChance_;
    public int damage_;
    public int armor_;
    public int armorPiercing_;
    public float speed_ = 1f;

    public int energy_;

    public float healthLevelCoefficient_ = 0.1f;
    public float dodgeLevelCoefficient = 0.1f;
    public float attackRatingCoefficient = 0.1f;
    public float criticalChanceCoefficient = 0.1f;
    public float damageCoefficient = 0.1f;
    public float armorCoefficient = 0.1f;
    public float apCoefficient = 0.1f;
    public float speedCoefficient = 0.1f;
    public float energyCoefficient = 0.1f;

    public PlayerSkill[] skills_;

    public int Health(int level) {

        int newHealth = (int)(level * healthLevelCoefficient_)+health_;
        return newHealth;
    }
    public float Dodge(int level) {

        float newVal = (level * dodgeLevelCoefficient)+dodge_;
        return newVal;
    }
    public float AttackRating(int level) {

        float newVal = (level * attackRatingCoefficient)+attackRating_;
        return newVal;
    }
    public float CriticalChance(int level) {

        float newVal = (level * criticalChanceCoefficient)+criticalChance_;
        return newVal;
    }
    public int Damage(int level) {

        int newVal = (int)(level * damageCoefficient)+damage_;
        return newVal;
    }
    public int Armor(int level) {

        int newVal = (int)(level * armorCoefficient)+armor_;
        return newVal;
    }
    public int AP(int level) {

        int newVal = (int)(level * apCoefficient)+armorPiercing_;
        return newVal;
    }
    public float Speed(int level) {

        float newVal = (level * speedCoefficient)+speed_;
        return newVal;
    }
    public int Energy(int level) {

        int newVal = (int)(level * energyCoefficient)+energy_;
        return newVal;

    }


}
