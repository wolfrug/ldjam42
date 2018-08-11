using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveEntity : MonoBehaviour {

    public int health_;
    public float dodge_;
    public float attackRating_;
    public float criticalChance_;
    public int damage_;
    public int armor_;
    public int armorPiercing_;
    public float speed_;
    public int energy_;

    public GameObject textPrefab_;
    public Image healthIndicator_;
    public Text turnIndicator_;

    public DestroyableEntity reference_;

    public EnemyController enemyController_;
    public PlayerController playerController_;

    void Awake() {

        

        enemyController_ = GetComponentInChildren<EnemyController>(true);
        playerController_ = GetComponentInChildren<PlayerController>(true);
        
        UpdateStats(true);

        textPrefab_.SetActive(false);

    }

    public void UpdateStats(bool updateHealthAndEnergy = false) {
        int currentLevel = 1;
        if (playerController_ != null) {
            currentLevel = PlayerManager.currentPlayerLevel_;
        }
        if (updateHealthAndEnergy) {
            health_ = reference_.Health(currentLevel);
            energy_ = reference_.Energy(currentLevel);
        };

        dodge_ = reference_.Dodge(currentLevel);
        attackRating_ = reference_.AttackRating(currentLevel);
        criticalChance_ = reference_.CriticalChance(currentLevel);
        damage_ = reference_.Damage(currentLevel);
        armor_ = reference_.Armor(currentLevel);
        armorPiercing_ = reference_.AP(currentLevel);
        speed_ = reference_.Speed(currentLevel);
        


    }

    public int health {
        get {
            return health_;
        }
        set {
            health_ += value;
            int currentLevel = 1;
            if (playerController_ != null) { currentLevel = PlayerManager.currentPlayerLevel_; };
            healthIndicator_.fillAmount = (float)health_/ (float)reference_.Health(currentLevel);
            PlayerManager.instance_.UpdateHealth();
        }
    }

    public int energy {
        get {
            return energy_;
        }
        set {
            energy_ += value;
        }
    }


    public string turnOrder {
        get {
            return turnIndicator_.text;
        }
        set {
            turnIndicator_.text = value;
        }
    }

    public bool controllerActive {
        get {
            if (enemyController_ != null) {
                return enemyController_.isActive_;
            }
            else if (playerController_ != null) {
                return playerController_.isActive_;
            }
            else {
                return false;
            }
        }
        set {
            if (enemyController_ != null) {
                //Debug.Log("Setting enemy controller to " + value);
                enemyController_.isActive_ = value;
                if (value == true) {
                    enemyController_.DoMove();
                };
            }
            else if (playerController_ != null) {
                //Debug.Log("Setting player controller to " + value);
                playerController_.isActive_ = value;
            }
            else {
                Debug.Log("No controller found on " + gameObject.name);
            }
        }
    }

    public TextMeshProUGUI createText {
        get {
            TextMeshProUGUI obj = Instantiate(textPrefab_, textPrefab_.transform.parent).GetComponentInChildren<TextMeshProUGUI>(true);
            obj.transform.parent.gameObject.SetActive(true);
            return obj;
        }
    }
    public void destroyText(TextMeshProUGUI trg) {
        Destroy(trg.transform.parent.gameObject, 1f);
    }

    public bool AttemptCritical(float roll, float criticalChance) {

        if (roll > (1f-criticalChance)) {
            TextMeshProUGUI tmpObj = createText;
            tmpObj.color = Color.yellow;
            tmpObj.text = "Critical hit!";
            destroyText(tmpObj);
            return true;
        }
        else {
            return false;
        }

    }

    public bool AttemptDodge(float roll) {
        if (roll < dodge_) {

            TextMeshProUGUI tmpObj = createText;
            tmpObj.color = Color.green;
            tmpObj.text = "Dodged!";
            destroyText(tmpObj);
            return true;
        }
        else {
            return false;
        }

    }

    public void AttemptDoDamage(int damage, int armorpierce) {

        if (armorpierce < armor_) {
            TextMeshProUGUI tmpObj = createText;
            tmpObj.text = "Blocked!";
            tmpObj.color = Color.gray;
            destroyText(tmpObj);
        }
        else {
            TextMeshProUGUI tmpObj = createText;
            tmpObj.text = "\n-" + (damage - armor_).ToString();
            tmpObj.color = Color.red;
            destroyText(tmpObj);
            health =- (damage - armor_);
        }

    }

}
