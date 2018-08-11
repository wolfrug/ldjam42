using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance_;

    public PlayerController player_;

    public string[] playerNames_;

    public Text playerName_;
    public Text playerClass_;
    public Text playerLevel_;
    public Text playerEnergy_;
    public Text playerXPText_;

    public Text playerhealth_;
    public Text playerDodge_;
    public Text playerAttackRating_;
    public Text playerCritChance_;
    public Text playerDamage_;
    public Text playerArmor_;
    public Text playerAP_;
    public Text playerSpeed_;

    public DestroyableEntity class_;

    public static int currentPlayerLevel_ = 1;
    public static int playerXP_ = 0;

    void Awake() {
        if (instance_ == null) {
            instance_ = this;
        }
        else if (instance_ != this) {
            Destroy(gameObject);
        }
    }

    void Start() {

        // Make random player name
        string randomname = playerNames_[Random.Range(0, playerNames_.Length)];
        playerName_.text = randomname;
        // Set class name
        playerClass_.text = class_.name_;
        //Grab controller
        player_ = FindObjectOfType<PlayerController>();
        //update UI
        UpdateUI();
    }
    
public void UpdateUI() {

        playerLevel_.text = currentPlayerLevel_.ToString();
        playerXPText_.text = playerXP_.ToString() + "/" + XPNeeded().ToString();
        playerEnergy_.text = player_.playerStats_.energy_ + "/" + class_.Energy(currentPlayerLevel_).ToString();
        playerhealth_.text = player_.playerStats_.health.ToString() + "/" + class_.Health(currentPlayerLevel_).ToString();
        playerDodge_.text = class_.Dodge(currentPlayerLevel_).ToString();
        playerAttackRating_.text = class_.AttackRating(currentPlayerLevel_).ToString();
        playerCritChance_.text = class_.CriticalChance(currentPlayerLevel_).ToString();
        playerDamage_.text = class_.Damage(currentPlayerLevel_).ToString();
        playerArmor_.text = class_.Armor(currentPlayerLevel_).ToString();
        playerAP_.text = class_.AP(currentPlayerLevel_).ToString();
        playerSpeed_.text = class_.Speed(currentPlayerLevel_).ToString();
    }

    public void UpdateHealth() {
        playerhealth_.text = player_.playerStats_.health.ToString() + "/" + class_.Health(currentPlayerLevel_).ToString();
    }
    public void UpdateXP() {
        playerXPText_.text = playerXP_.ToString() + "/" + XPNeeded().ToString();
    }
    public void UpdateEnergy() {

    }

    public void AddXP(DestroyableEntity enemy) {
        // Just add XP based on the stats of the destroyed enemy, since they don't level up etc.
        float xpToAdd = (float)(enemy.health_ * 0.5) + (enemy.dodge_) + (enemy.attackRating_) + (enemy.criticalChance_) + (float)(enemy.damage_ * 0.5f) + (float)(enemy.armor_ * 0.5f) + (float)(enemy.armorPiercing_ * 0.5f) + (enemy.speed_);
        playerXP_ += (int)xpToAdd;
        LevelUp();
        UpdateXP();
    }

    public int XPNeeded() {
        int xpNeeded = Mathf.FloorToInt((currentPlayerLevel_ - 1) * 1.2f + 5);
        return xpNeeded;

    }

    public void LevelUp() {

        if (playerXP_ >= XPNeeded()) {
            currentPlayerLevel_ += 1;
            playerXP_ = 0;
            UpdateUI();
            player_.playerStats_.UpdateStats();
        }

    }



}
