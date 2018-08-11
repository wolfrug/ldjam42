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

    public GameObject textPrefab_;
    public Image healthIndicator_;
    public Text turnIndicator_;

    public DestroyableEntity reference_;

    public EnemyController enemyController_;
    public PlayerController playerController_;

    void Start() {

        health_ = reference_.health_;
        dodge_ = reference_.dodge_;
        attackRating_ = reference_.attackRating_;
        criticalChance_ = reference_.criticalChance_;
        damage_ = reference_.damage_;
        armor_ = reference_.armor_;
        armorPiercing_ = reference_.armorPiercing_;
        speed_ = reference_.speed_;

        enemyController_ = GetComponentInChildren<EnemyController>(true);
        playerController_ = GetComponentInChildren<PlayerController>(true);

        textPrefab_.SetActive(false);

    }

    public int health {
        get {
            return health_;
        }
        set {
            health_ += value;
            healthIndicator_.fillAmount = (float)health_/ (float)reference_.health_;
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
