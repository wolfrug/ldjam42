using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour {

    public GameObject notYourTurnPanel_;
    public GameObject gameOverScreen_;
    public GameObject levelEnteredScreen_;
    public Text hintsScreen_;

    public GameObject levelCompletePanel_;
    public Text levelCompleteStats_;

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

    public string[] hints_;

    public static MainUIManager instance_;
    public PlayerController player_;

    void Awake() {
        if (instance_ == null) {
            instance_ = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    void Start() {

        player_ = FindObjectOfType<PlayerController>();
        Invoke("DisappearLevelEnteredScreen", 2f);
    }

    public void DisappearLevelEnteredScreen() {
        levelEnteredScreen_.GetComponentInChildren<Animator>().SetTrigger("FadeOut");
    }

    public void ActivateWaitingTurnPanel(bool active) {
        notYourTurnPanel_.SetActive(active);
    }

    public void GameOverPanel() {

        gameOverScreen_.SetActive(true);
        hintsScreen_.text = "Hint: " + hints_[Random.Range(0, hints_.Length)];
    }

    public void LevelComplete() {

        levelCompletePanel_.SetActive(true);
        int gainedXP = PlayerManager.playerXP_ - PlayerPrefs.GetInt("PlayerExperience");
        levelCompleteStats_.text = "Level Complete!\n\nExperience gained: " + gainedXP.ToString() + "\nTurns played: " + TurnManager._Instance.totalTurnCount.ToString();

    }

    public void Continue() {

        GameManager.instance_.LoadNextLevel();
    }



    public void UpdateUI() {

        playerLevel_.text = PlayerManager.currentPlayerLevel_.ToString();
        playerXPText_.text = PlayerManager.playerXP_.ToString() + "/" + PlayerManager.instance_.XPNeeded().ToString();
        playerEnergy_.text = player_.playerStats_.energy + "/" + PlayerManager.instance_.class_.Energy(PlayerManager.currentPlayerLevel_).ToString();
        playerhealth_.text = player_.playerStats_.health.ToString() + "/" + PlayerManager.instance_.class_.Health(PlayerManager.currentPlayerLevel_).ToString();
        playerDodge_.text = string.Format("{0:P0}", player_.playerStats_.dodge_);
        playerAttackRating_.text = string.Format("{0:P0}", player_.playerStats_.attackRating_);
        playerCritChance_.text = string.Format("{0:P0}", player_.playerStats_.criticalChance_);
        playerDamage_.text = player_.playerStats_.damage_.ToString();
        playerArmor_.text = player_.playerStats_.armor_.ToString();
        playerAP_.text = player_.playerStats_.armorPiercing_.ToString();
        playerSpeed_.text = ((int)(player_.playerStats_.speed_)).ToString();

        UpdateUIColors();
    }

    public void UpdateUIColors() { // Makes UI elements red/green if they've been enhanced

        ActiveEntity playerEntity = player_.playerStats_;

        if (playerEntity.dodge_ > playerEntity.reference_.Dodge(PlayerManager.currentPlayerLevel_)) {

            playerDodge_.color = Color.green;
        }
        else if (playerEntity.dodge_ < playerEntity.reference_.Dodge(PlayerManager.currentPlayerLevel_)) {
            playerDodge_.color = Color.red;
        }
        else {
            playerDodge_.color = Color.white;
        }
        if (playerEntity.attackRating_ > playerEntity.reference_.AttackRating(PlayerManager.currentPlayerLevel_)) {

            playerAttackRating_.color = Color.green;
        }
        else if (playerEntity.attackRating_ < playerEntity.reference_.AttackRating(PlayerManager.currentPlayerLevel_)) {
            playerAttackRating_.color = Color.red;
        }
        else {
            playerAttackRating_.color = Color.white;
        }
        if (playerEntity.criticalChance_ > playerEntity.reference_.CriticalChance(PlayerManager.currentPlayerLevel_)) {

            playerCritChance_.color = Color.green;
        }
        else if (playerEntity.criticalChance_ < playerEntity.reference_.CriticalChance(PlayerManager.currentPlayerLevel_)) {
            playerCritChance_.color = Color.red;
        }
        else {
            playerCritChance_.color = Color.white;
        }
        if (playerEntity.damage_ > playerEntity.reference_.Damage(PlayerManager.currentPlayerLevel_)) {

            playerDamage_.color = Color.green;
        }
        else if (playerEntity.damage_ < playerEntity.reference_.Damage(PlayerManager.currentPlayerLevel_)) {
            playerDamage_.color = Color.red;
        }
        else {
            playerDamage_.color = Color.white;
        }
        if (playerEntity.armor_ > playerEntity.reference_.Armor(PlayerManager.currentPlayerLevel_)) {

            playerArmor_.color = Color.green;
        }
        else if (playerEntity.armor_ < playerEntity.reference_.Armor(PlayerManager.currentPlayerLevel_)) {
            playerArmor_.color = Color.red;
        }
        else {
            playerArmor_.color = Color.white;
        }
        if (playerEntity.armorPiercing_ > playerEntity.reference_.AP(PlayerManager.currentPlayerLevel_)) {

            playerAP_.color = Color.green;
        }
        else if (playerEntity.armorPiercing_ < playerEntity.reference_.AP(PlayerManager.currentPlayerLevel_)) {
            playerAP_.color = Color.red;
        }
        else {
            playerAP_.color = Color.white;
        }
        if (playerEntity.speed_ > playerEntity.reference_.Speed(PlayerManager.currentPlayerLevel_)) {

            playerSpeed_.color = Color.green;
        }
        else if (playerEntity.speed_ < playerEntity.reference_.Speed(PlayerManager.currentPlayerLevel_)) {
            playerSpeed_.color = Color.red;
        }
        else {
            playerSpeed_.color = Color.white;
        }

    }

    public void UpdateHealth() {
        playerhealth_.text = player_.playerStats_.health.ToString() + "/" + PlayerManager.instance_.class_.Health(PlayerManager.currentPlayerLevel_).ToString();
    }
    public void UpdateXP() {
        playerXPText_.text = PlayerManager.playerXP_.ToString() + "/" + PlayerManager.instance_.XPNeeded().ToString();
    }
    public void UpdateEnergy() {
        playerEnergy_.text = player_.playerStats_.energy + "/" + PlayerManager.instance_.class_.Energy(PlayerManager.currentPlayerLevel_).ToString();
    }

    public void ToggleGameObject(GameObject target) {
        if (target.activeSelf) {
            target.SetActive(false);
        }
        else {
            target.SetActive(true);
        }
    }

    public void Restart() {

        GameManager.instance_.RestartLevel();
    }

    public void Quit() {

        GameManager.instance_.QuitGame();
    }

}
