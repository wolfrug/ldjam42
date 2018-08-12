using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance_;

    public PlayerController player_;

    public string[] playerNames_;
    public DestroyableEntity[] classes_;




    public DestroyableEntity class_;

    public static string name_ = "";
    public static int currentPlayerLevel_ = 20;
    public static int playerXP_ = 0;

    void Awake() {
        if (instance_ == null) {
            instance_ = this;
        }
        else if (instance_ != this) {
            Destroy(gameObject);
        }

        if (PlayerPrefs.GetString("PlayerClass") == "") {
            class_ = classes_[Random.Range(0, classes_.Length)];
            SkillsManager.instance_.playerClass_ = class_;
            PlayerPrefs.SetString("PlayerClass", class_.name_);
        }
        else {
            string pclass = PlayerPrefs.GetString("PlayerClass");
            foreach (DestroyableEntity classname in classes_) {
                if (classname.name_ == pclass) {
                    class_ = classname;
                    SkillsManager.instance_.playerClass_ = class_;
                };
            }
        }
    }

    void Start() {

        name_ = PlayerPrefs.GetString("PlayerName");
        //currentPlayerLevel_ = Mathf.Clamp(PlayerPrefs.GetInt("PlayerLevel"), 1, 99);
        playerXP_ = PlayerPrefs.GetInt("PlayerExperience");

        // Make random player name
        if (name_ == "") {
             name_ = playerNames_[Random.Range(0, playerNames_.Length)];
            PlayerPrefs.SetString("PlayerName", name_);
        }
        // Pick 3 random skills


        MainUIManager.instance_.playerName_.text = name_;
        // Set class name
        MainUIManager.instance_.playerClass_.text = class_.name_;
        //Grab controller
        player_ = FindObjectOfType<PlayerController>();
        //update UI
        MainUIManager.instance_.UpdateUI();
    }

    public void SetPlayerClass(DestroyableEntity playerclass) {
        class_ = playerclass;
    }

   

    

    public void AddXP(DestroyableEntity enemy) {
        // Just add XP based on the stats of the destroyed enemy, since they don't level up etc.
        float xpToAdd = (float)(enemy.health_ * 0.5) + (enemy.dodge_) + (enemy.attackRating_) + (enemy.criticalChance_) + (float)(enemy.damage_ * 0.5f) + (float)(enemy.armor_ * 0.5f) + (float)(enemy.armorPiercing_ * 0.5f) + (enemy.speed_);
        playerXP_ += (int)xpToAdd;
        LevelUp();
        MainUIManager.instance_.UpdateXP();
    }

    public int XPNeeded() {
        int xpNeeded = Mathf.FloorToInt((currentPlayerLevel_ - 1) * 1.2f + 5);
        return xpNeeded;

    }

    public void LevelUp() {

        if (playerXP_ >= XPNeeded()) {
            currentPlayerLevel_ += 1;
            playerXP_ = 0;
            player_.playerStats_.UpdateStats(true);
            MainUIManager.instance_.UpdateUI();
            SkillsManager.instance_.UpdateSkills();
        }

    }



}
