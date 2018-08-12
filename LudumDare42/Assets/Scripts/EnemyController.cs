using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DeathEvent : UnityEvent<EnemyController> {
}

    public class EnemyController : MonoBehaviour {

    public GameObject enemyObj_;
    public GameObject avatar_;
    public Grid movementGrid_;
    public ActiveEntity enemyStats_;
    private float movementSteps_;
    private bool isMoving_ = false;
    private Rigidbody rb;
    public Animator characterAnimator_;

    public bool dead_;

    public Transform player_;

    public bool isActive_;
    public bool debugWaiter_;

    public DeathEvent unit_dead = new DeathEvent();

    public Directions currentFacing_ = Directions.RIGHT;
    private int attemptedMoves_ = 0;

	// Use this for initialization
	void Start () {
        rb = GetComponentInChildren<Rigidbody>();

        if (enemyObj_ == null) {
            enemyObj_ = gameObject;
        }
        if (movementGrid_ == null) {
            movementGrid_ = FindObjectOfType<Grid>();
        }
        if (enemyStats_ == null) {
            enemyStats_ = GetComponent<ActiveEntity>();
        }

        player_ = FindObjectOfType<PlayerController>().playerObj_.transform;

        rb = GetComponentInChildren<Rigidbody>();

        movementSteps_ = movementGrid_.cellSize.x + movementGrid_.cellGap.x;


    }

    public int EnemyDirection() {// Return 0 = up, 1 = down, 2 = left, 3 = right for enemy direction

        int returnInt = -1;

        Vector3 playerPosAsInt = player_.localPosition;
        Vector3 selfPosAsInt = enemyObj_.transform.localPosition;
        //Debug.Log("Self x: " + selfPosAsInt.x + " Enemy x: " + playerPosAsInt.x + " Self y:" + selfPosAsInt.y + " Enemy y: " + playerPosAsInt.y);

        // Is right/left closer or up/down
        if (Random.value>0.5f) { // Up/down
            if (playerPosAsInt.y > selfPosAsInt.y) {// Player is above enemy
                returnInt = 0;
            }
            else if (playerPosAsInt.y < selfPosAsInt.y) {// Player is below enemy
                returnInt = 1;
            };
        }
        else {

            if (playerPosAsInt.x > selfPosAsInt.x) { // Player is to the right of enemy
                returnInt = 3;
            }
            else if (playerPosAsInt.x < selfPosAsInt.x) {// Player is to the left
                returnInt = 2;
            }
        }

        //return returnInt;

        // But we only start moving if the player is close enough
        if (Vector3.Distance(player_.transform.position, enemyObj_.transform.position) < 10) {
            //Debug.Log("Moving in direction: " + returnInt);
            return returnInt;
        }
        else {
            //Debug.Log("Player too far away.");
            return -1;
        }

    }

    
    public void DoMove() {
        
        if (isActive_ && !isMoving_) {
            if (!dead_) {
                
                // Check just in case to make sure we -can- move, if we can't, we wait
                if (!GetValidMove(MoveUp()) && !GetValidMove(MoveDown()) && !GetValidMove(MoveLeft()) && !GetValidMove(MoveRight())) {
                    Debug.Log("No move found, ending turn.");
                    EndTurn();
                }

                int direction = EnemyDirection();
                if (direction != -1) {
                    MoveInDirection(direction);
                }
                else {
                    EndTurn();
                };
                attemptedMoves_ += 1;
            }
            else {
                EndTurn();
            }
        }
        else {
            EndTurn();
        }
        StartCoroutine(FailSafe());
    }

    IEnumerator FailSafe() {
        // If turn hasn't ended by now, force end it
        yield return new WaitForSeconds(2f);
        EndTurn();

    }

    void MoveInDirection(int direction) {
        if (direction == 0) {
            //Debug.Log("Up!");
            if (GetValidMove(MoveUp())) {
                MoveUp(true);
                return;
            }
        }

        if (direction == 1) {
            //Debug.Log("Down!");
            if (GetValidMove(MoveDown())) {
                MoveDown(true);
                return;
            }
        }
        if (direction == 2) {
            //Debug.Log("Left!");
            if (GetValidMove(MoveLeft())) {
                MoveLeft(true);
                return;
            }
        }

        if (direction == 3) {
            //Debug.Log("Right!");
            if (GetValidMove(MoveRight())) {
                MoveRight(true);
                return;
            }
        }
        EndTurn();
        /*if (direction == -1) {
            EndTurn();
        }/*
        else {
            //DoMove();
            EndTurn();
        }*/
    }

    void EndTurn() {
        TurnManager._Instance.NextTurn(enemyStats_);
        attemptedMoves_ = 0;
        isMoving_ = false;
        StopAllCoroutines();
    }

    

    public bool GetValidMove(Vector3 goal, bool checkForEnemy = true) {

        // Quit if we've attempted to move more than 4 times
        if (attemptedMoves_ > 4) {
            EndTurn();
        }

        /*for (int i = 0; i < Physics.OverlapSphere(goal, 0.5f, 9).Length; i++) {
            Debug.Log(Physics.OverlapSphere(goal, 0.5f, 9)[i].gameObject.name);
        }*/
        Collider[] colls = Physics.OverlapSphere(goal, 0.1f, 9);

        if (colls.Length == 0) {
            //Debug.Log("Valid move! ");
            return true;
        }
        else {
            if (checkForEnemy) {
                //Debug.Log("Invalid move! Checking for enemy");
                PlayerController enemy = DetectDestroyable(colls);
                if (enemy != null) {
                    StartCoroutine(Attack(enemy));
                    isMoving_ = true;
                }
            };
            return false;
        }

    }

    public PlayerController DetectDestroyable(Collider[] colls) {

        for (int i = 0; i < colls.Length; i++) {
            //Debug.Log("Object " + colls[i].gameObject.name + " has tag " + colls[i].gameObject.tag);
            if (colls[i].gameObject.tag == "Player") {
                PlayerController check = colls[i].gameObject.GetComponentInParent<PlayerController>();
                if (check != null) {
                    return check;
                };
            }
        }
        return null;

    }

    public IEnumerator Attack(PlayerController enemy) {
        Debug.Log(gameObject.name + "is attacking " + enemy.gameObject.name);
        isMoving_ = true;
        ActiveEntity enemyStats = enemy.GetComponentInParent<ActiveEntity>();

        // Attempt attack!
        float attackRoll = Random.Range(0f, 1f);
        int damage = enemyStats_.damage_;
        int armorPierce = enemyStats_.armorPiercing_;
        // Critical hit?!
        if (enemyStats.AttemptCritical(attackRoll, enemyStats_.criticalChance_)) {
            damage *= 2;
            armorPierce = (armorPierce + 1) * 2;
            enemyStats.AttemptDoDamage(damage, armorPierce);
            characterAnimator_.SetTrigger("Critical");
            enemy.characterAnimator_.SetTrigger("Stumble");
        }
        else {
            if (!enemyStats.AttemptDodge(attackRoll + enemyStats_.attackRating_)) {
                enemyStats.AttemptDoDamage(damage, armorPierce);
                characterAnimator_.SetTrigger("Swing");
                enemy.characterAnimator_.SetTrigger("Stumble");
            }
            else {
                characterAnimator_.SetTrigger("Swing");
                enemy.characterAnimator_.SetTrigger("Dodge");
            }
        };
        if (enemyStats.health <= 0) {
            enemy.Die((enemyObj_.transform.position - enemy.transform.position).normalized);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        EndTurn();
        isMoving_ = false;
    }


    public IEnumerator SmoothMove(Vector3 position, bool animateAndEndTurn = true) {
        isMoving_ = true;
        if (animateAndEndTurn) { characterAnimator_.SetTrigger("Walk"); };
        //enemyObj_.transform.localRotation = Quaternion.Euler(direction.x, direction.y,direction.z);
       Vector3 velocity = Vector3.zero;
        while (true) {
            enemyObj_.transform.position = Vector3.SmoothDamp(enemyObj_.transform.position, position, ref velocity, 0.1f);
           
            if (velocity.magnitude < 0.1f) { break; };
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        enemyObj_.transform.position = position;
        isMoving_ = false;
        if (animateAndEndTurn) { EndTurn(); };

    }

    public Vector3 MoveUp(bool move = false, bool changeDirection = true, bool animate = true) {
        if (move) {
            StartCoroutine(SmoothMove(enemyObj_.transform.position + new Vector3(0f, 0f, movementSteps_), animate));
        }
        if (changeDirection) {
            avatar_.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
            currentFacing_ = Directions.UP;
        };
        return enemyObj_.transform.position + new Vector3(0f, 0f, movementSteps_);
    }
    public Vector3 MoveDown(bool move = false, bool changeDirection = true, bool animate = true) {
        if (move) {
            StartCoroutine(SmoothMove(enemyObj_.transform.position + new Vector3(0f, 0f, -movementSteps_), animate));
        }
        if (changeDirection) {
            avatar_.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
            currentFacing_ = Directions.DOWN;
        };
        return enemyObj_.transform.position + new Vector3(0f, 0f, -movementSteps_);
    }
    public Vector3 MoveLeft(bool move = false, bool changeDirection = true, bool animate = true) {
        if (move) {
            StartCoroutine(SmoothMove(enemyObj_.transform.position + new Vector3(-movementSteps_, 0f, 0f), animate));
        }
        if (changeDirection) {
            avatar_.transform.localRotation = Quaternion.Euler(new Vector3(0f, -180f, 0f));
            currentFacing_ = Directions.LEFT;
        };
        return enemyObj_.transform.position + new Vector3(-movementSteps_, 0f, 0f);
    }
    public Vector3 MoveRight(bool move = false, bool changeDirection = true, bool animate = true) {

        if (move) {
            StartCoroutine(SmoothMove(enemyObj_.transform.position + new Vector3(movementSteps_, 0f, 0f), animate));
        }
        if (changeDirection) {
            avatar_.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            currentFacing_ = Directions.RIGHT;
        };
        return enemyObj_.transform.position + new Vector3(movementSteps_, 0f, 0f);
    }


    public void Die(Vector3 direction) {

        //rb.isKinematic = false;
        //rb.WakeUp();
        //rb.AddForce(direction*(enemyStats_.health - 5), ForceMode.Impulse);
        rb.gameObject.tag = "Untagged";
        rb.gameObject.layer = 9;
        dead_ = true;
        characterAnimator_.SetTrigger("Die");
        unit_dead.Invoke(this);
        
    }

}
