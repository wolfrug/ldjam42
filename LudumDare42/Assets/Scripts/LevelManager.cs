using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public LevelManager instance_;

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
        // Listen to player death
        PlayerController player = FindObjectOfType<PlayerController>();
        player.player_dead.AddListener(PlayerDead);

	}

    void ExitLevel(ActiveEntity entity, GridTrigger trigger) {
        Debug.Log("Exit reached!");
        MainUIManager.instance_.LevelComplete();
    }
	
    void PlayerDead(PlayerController player) {
        MainUIManager.instance_.GameOverPanel();
    }

    public void FinishLevelAndSetNextLevel(string sceneName) {
        GameManager.instance_.nextLevel_ = sceneName;
        MainUIManager.instance_.LevelComplete();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
