using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsManager : MonoBehaviour {

    public static SkillsManager instance_;

    public PlayerController player_;
    public ActiveEntity playerStats_;

    public GameObject buttonPrefab_;
    public Transform parent_;

    public DestroyableEntity playerClass_;

    public List<GameObject> allButtons_ = new List<GameObject> { };

    void Awake() {
        if (instance_ == null) {
            instance_ = this;
        }
    }

	// Use this for initialization
	void Start () {

        player_ = FindObjectOfType<PlayerController>();
        playerStats_ = player_.playerStats_;
        playerClass_ = PlayerManager.instance_.class_;
        UpdateSkills();

	}

    public void UpdateSkills() {
    
        foreach (PlayerSkill skill in playerClass_.skills_) {

            GameObject newButton = Instantiate(buttonPrefab_, parent_);
            newButton.name = skill.name_;
            Button btn = newButton.GetComponent<Button>();
            allButtons_.Add(newButton);
            Text buttonText = newButton.GetComponentInChildren<Text>();
            buttonText.text = skill.name_ + "\n<size=20><color=orange>" + skill.description_ + "</color>\nCost: " + skill.energyCost_.ToString() + " energy\nDuration: " + skill.turnDuration_.ToString() + " turns</size>";
            newButton.SetActive(true);
            if (PlayerManager.currentPlayerLevel_ >= skill.unlocksAtLevel_) {
                btn.interactable = true;
            }
            else {
                btn.interactable = false;
                buttonText.text += "\n<color=red>Unlocks at level " + skill.unlocksAtLevel_.ToString() + "</color>";
            }
            btn.onClick.AddListener(() => skill.RunSkill(playerStats_));
        }

    }

    public IEnumerator SkillWaiter(PlayerSkill skill, int targetLevel, ActiveEntity target) {

        int currentTurn = TurnManager._Instance.totalTurnCount;
        int targetTurn = skill.turnDuration_ + currentTurn;

        if (skill.turnDuration_ < 0) { // Skills with negative lengths are permanent!
            yield break;
        }

        yield return new WaitUntil(() => TurnManager._Instance.totalTurnCount >= targetTurn);

        skill.DeactivateSkill(target, targetLevel);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
