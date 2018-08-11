using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "PlayerSkill", order = 1)]
public class PlayerSkill : ScriptableObject {
    public string name_;
    public string description_;
    public int turnDuration_;
    public int energyCost_;
    public bool skipsPlayerTurn = false;
    public int unlocksAtLevel_;

    public int health_;
    public float dodge_;
    public float attackRating_;
    public float criticalChance_;
    public int damage_;
    public int armor_;
    public int armorPiercing_;
    public float speed_;

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


    public void RunSkill(ActiveEntity target) {

        if (SkillsManager.instance_.playerStats_.energy >= energyCost_) {

            int currentLevel = PlayerManager.currentPlayerLevel_;
            SkillsManager.instance_.playerStats_.energy_ -= energyCost_;

            // Run the thingie
            target.health_ += Health(currentLevel);
            target.dodge_ += Dodge(currentLevel);
            target.attackRating_ += AttackRating(currentLevel);
            target.criticalChance_ += CriticalChance(currentLevel);
            target.damage_ += Damage(currentLevel);
            target.armor_ += Armor(currentLevel);
            target.armorPiercing_ += AP(currentLevel);
            target.speed_ += Speed(currentLevel);
            target.energy_ += Energy(currentLevel);
            // And start the waiter on the skillsmanager
            SkillsManager.instance_.StartCoroutine(SkillsManager.instance_.SkillWaiter(this, currentLevel, target));
            // And update UI!
            PlayerManager.instance_.UpdateUI();

            if (skipsPlayerTurn) {
                TurnManager._Instance.NextTurn(target);
            }
        };

    }
    public void DeactivateSkill(ActiveEntity target, int targetLevel) {

        // Remove the buffs according to the level they had when they were given
        target.health_ -= Health(targetLevel);
        target.dodge_ -= Dodge(targetLevel);
        target.attackRating_ -= AttackRating(targetLevel);
        target.criticalChance_ -= CriticalChance(targetLevel);
        target.damage_ -= Damage(targetLevel);
        target.armor_ -= Armor(targetLevel);
        target.armorPiercing_ -= AP(targetLevel);
        target.speed_ -= Speed(targetLevel);
        target.energy_ -= Energy(targetLevel);
        // Update UI
        PlayerManager.instance_.UpdateUI();
    }


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
