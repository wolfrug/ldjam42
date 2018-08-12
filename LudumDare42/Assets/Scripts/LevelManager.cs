using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public static LevelManager instance_;
    public int enemiesKilled_;

    private List<GridTrigger> exits_ = new List<GridTrigger> { };

    void Awake() {
        if (instance_ == null) {
            instance_ = this;
        }
        else {
            Destroy(gameObject);
        }
    }


	// Use this for initialization
	void Start () {
	
        // Grab all grid triggers with exit IDs
        foreach (GridTrigger trigger in FindObjectsOfType<GridTrigger>()) {
            if (trigger.id_ == "Exit") {
                trigger.grid_triggered.AddListener(ExitLevel);
                exits_.Add(trigger);
            }
        }
        
        // Listen to enemy death
        foreach (EnemyController controller in FindObjectsOfType<EnemyController>()) {
            controller.unit_dead.AddListener(UnitDead);
        }

	}

    void ExitLevel(ActiveEntity entity, GridTrigger trigger) {
        Debug.Log("Exit reached!");
        MainUIManager.instance_.LevelComplete();
    }
	
    void UnitDead(EnemyController controller) {
        enemiesKilled_ += 1;
        if (controller.enemyStats_.reference_.name_ == "Enemy Knight" || controller.enemyStats_.reference_.name_ == "Enemy Ninja") {
            // Killed the bad guy! You win!
            FinishLevelAndSetNextLevel("endScene");
        };
    }

    public void FinishLevelAndSetNextLevel(string sceneName) {
        GameManager.instance_.nextLevel_ = sceneName;
        MainUIManager.instance_.LevelComplete();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
