using UnityEngine;
using System.Collections;

public enum Directions {

    NONE = 0000,
    UP = 1000,
    DOWN = 2000,
    LEFT = 3000,
    RIGHT = 4000,
    RELATIVE_BACK = 5000,
    RELATIVE_FORWARD = 6000,
    RELATIVE_LEFT = 7000,
    RELATIVE_RIGHT = 8000
    
}


[CreateAssetMenu(fileName = "Data", menuName = "PlayerSkill", order = 1)]
public class PlayerSkill : ScriptableObject {
    public string name_;
    public string description_;
    public int turnDuration_;
    public int energyCost_;
    public bool skipsPlayerTurn = false;
    public int unlocksAtLevel_;

    public Directions moveTargetInDirection_ = Directions.NONE;
    public int squaresToMove_;
    public string moveAnimationName_ = "Kick";

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
            MainUIManager.instance_.UpdateUI();

            if (skipsPlayerTurn) {
                TurnManager._Instance.NextTurn(target);
            }
            if (moveTargetInDirection_ != Directions.NONE) {
                target.playerController_.player_Attack.AddListener(MoveTarget);
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
        MainUIManager.instance_.UpdateUI();
    }

    public void MoveTarget(EnemyController target, string success) {

        Debug.Log("Attempting to move " + target.gameObject.name + " with attack " + success);

        if (success == "Dodge") {
            SkillsManager.instance_.player_.player_Attack.RemoveAllListeners();
            return;
        }
        
        // Attempts to move the target somehow when activated
        PlayerController player = FindObjectOfType<PlayerController>();

        int direction = DirectionsToInt(moveTargetInDirection_, player);

            if (direction == 0) {
                //Debug.Log("Up!");
                if (target.GetValidMove(target.MoveUp(),false)) {
                    target.MoveUp(true,false,false);
                }
            }

            if (direction == 1) {
                //Debug.Log("Down!");
                if (target.GetValidMove(target.MoveDown(),false)) {
                target.MoveDown(true, false, false);
                }
            }
            if (direction == 2) {
                //Debug.Log("Left!");
                if (target.GetValidMove(target.MoveLeft(),false)) {
                    target.MoveLeft(true,false,false);
                }
            }

            if (direction == 3) {
                //Debug.Log("Right!");
                if (target.GetValidMove(target.MoveRight(),false)) {
                    target.MoveRight(true,false,false);
                }
            }

        if (moveAnimationName_ != "") {
            player.characterAnimator_.SetTrigger(moveAnimationName_);
        };

        SkillsManager.instance_.player_.player_Attack.RemoveAllListeners();

    }

    Directions GetOpposite(Directions trg) {
        switch (trg) {
            case Directions.UP: {
                    return Directions.DOWN;
                }
            case Directions.DOWN: {
                    return Directions.UP;
                }
            case Directions.LEFT: {
                    return Directions.RIGHT;
                }
            case Directions.RIGHT: {
                    return Directions.LEFT;
                }
            case Directions.RELATIVE_BACK: {
                    return Directions.RELATIVE_FORWARD;
                }
            case Directions.RELATIVE_FORWARD: {
                    return Directions.RELATIVE_BACK;
                }
            case Directions.RELATIVE_LEFT: {
                    return Directions.RELATIVE_RIGHT;
                }
            case Directions.RELATIVE_RIGHT: {
                    return Directions.RELATIVE_LEFT;
                }
            default: {
                    return Directions.NONE;
                }
        }
    }
    int DirectionsToInt(Directions trg, PlayerController rel_facing) {
        switch (trg) {
            case Directions.UP: {
                    return 0;
                }
            case Directions.DOWN: {
                    return 1;
                }
            case Directions.LEFT: {
                    return 2;
                }
            case Directions.RIGHT: {
                    return 3;
                }
            case Directions.RELATIVE_BACK: {
                    return DirectionsToInt(rel_facing.currentFacing_, rel_facing);
                }
            case Directions.RELATIVE_FORWARD: {
                    return DirectionsToInt(GetOpposite(rel_facing.currentFacing_), rel_facing);
                }
            default: {
                    return -1;
                }
        }
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
