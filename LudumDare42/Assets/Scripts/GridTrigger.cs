using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Triggerers {

    NONE = 0000,
    PLAYER = 1000,
    ENEMY = 2000,
    //WALL = 3000

}

[System.Serializable]
public class GridTriggerEvent : UnityEvent<ActiveEntity, GridTrigger> {
}


public class GridTrigger : MonoBehaviour {

    public string id_;
    public Triggerers triggerer_ = Triggerers.PLAYER;

    public GridTriggerEvent grid_triggered = new GridTriggerEvent();

    void OnTriggerEnter(Collider coll) {

        bool isPlayer = false;
        bool isEnemy = false;
        //bool isWall = false;

        ActiveEntity entity = coll.gameObject.GetComponentInParent<ActiveEntity>();
        if (entity != null) {
            if (entity.playerController_ != null) {
                isPlayer = true;
            }
            else if (entity.enemyController_ != null) {
                isEnemy = true;
            };
        }
        if (isPlayer && triggerer_ == Triggerers.PLAYER) {

            grid_triggered.Invoke(entity, this);
            Debug.Log("Player triggered!");
        }
        else if (isEnemy && triggerer_ == Triggerers.ENEMY) {
            grid_triggered.Invoke(entity, this);
            Debug.Log("Enemy triggered!");
        };

    }
}
