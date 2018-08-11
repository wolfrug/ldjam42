using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance_;

    public string[] playerNames_;

    public Text playerName_;
    public Text playerClass_;
    public Text playerLevel_;
    public Text playerEnergy_;

    public Text playerhealth_;
    public Text playerDodge_;
    public Text playerAttackRating_;
    public Text playerCritChance_;
    public Text playerDamage_;
    public Text playerArmor_;
    public Text playerAP_;
    public Text playerSpeed_;

    public DestroyableEntity class_;

    public static int currentPlayerLevel_ = 56;
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
        UpdateUI();
    }
    
public void UpdateUI() {

        playerLevel_.text = currentPlayerLevel_.ToString();
        playerEnergy_.text = class_.Energy(currentPlayerLevel_).ToString();
        playerhealth_.text = class_.Health(currentPlayerLevel_).ToString();
        playerDodge_.text = class_.Dodge(currentPlayerLevel_).ToString();
        playerAttackRating_.text = class_.AttackRating(currentPlayerLevel_).ToString();
        playerCritChance_.text = class_.CriticalChance(currentPlayerLevel_).ToString();
        playerDamage_.text = class_.Damage(currentPlayerLevel_).ToString();
        playerArmor_.text = class_.Armor(currentPlayerLevel_).ToString();
        playerAP_.text = class_.AP(currentPlayerLevel_).ToString();
        playerSpeed_.text = class_.Speed(currentPlayerLevel_).ToString();
    }



}
