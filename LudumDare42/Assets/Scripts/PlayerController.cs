using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[System.Serializable]
public class AttackEvent : UnityEvent<EnemyController, string> {
}
[System.Serializable]
public class MoveEvent : UnityEvent<Directions, bool> {
}

public class PlayerController : MonoBehaviour {

    public GameObject playerObj_;
    public GameObject avatar_;
    public Grid movementGrid_;
    public ActiveEntity playerStats_;
    public Animator characterAnimator_;
    public bool isActive_ = true;
    public bool dead_;
    public int gridUnitsToMove_ = 1;
    private float movementSteps_;
    private bool isMoving_ = false;
    private Rigidbody rb;

    public Directions currentFacing_;

    // Player_Attack returns the enemy, and the result (as string)
    public AttackEvent player_Attack = new AttackEvent();
    // Player move returns attempted direction and success/not 
    public MoveEvent player_Move = new MoveEvent();

    // Use this for initialization
    void Awake() {
        if (playerStats_ == null) {
            playerStats_ = GetComponent<ActiveEntity>();
        }
    }

	void Start () {
		if (playerObj_ == null) {
            playerObj_ = gameObject;
        }
        if (movementGrid_ == null) {
            movementGrid_ = FindObjectOfType<Grid>();
        }
        if (playerStats_ == null) {
            playerStats_ = GetComponent<ActiveEntity>();
        }

        rb = GetComponentInChildren<Rigidbody>();

        movementSteps_ = movementGrid_.cellSize.x + movementGrid_.cellGap.x;
    }
	
    public bool GetValidMove(Vector3 goal, bool checkForEnemy = true) {

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
                EnemyController enemy = DetectDestroyable(colls);
                if (enemy != null) {
                    StartCoroutine(Attack(enemy));
                    isMoving_ = true;
                }
            };
            return false;
        }

    }

    public EnemyController DetectDestroyable(Collider[] colls) {

        for (int i = 0; i < colls.Length; i++) {
           //Debug.Log("Object " + colls[i].gameObject.name + " has tag " + colls[i].gameObject.tag);
            if (colls[i].gameObject.tag == "Destroyable") {
                EnemyController check = colls[i].gameObject.GetComponentInParent<EnemyController>();
                if (check != null) {
                    return check;
                };
            }
       }
        return null;

    }

    public IEnumerator Attack(EnemyController enemy) {
        Debug.Log("Player is Attacking " + enemy.gameObject.name);
        isMoving_ = true;
        ActiveEntity enemyStats = enemy.GetComponentInParent<ActiveEntity>();

        // Attempt attack!
        float attackRoll = Random.Range(0f, 1f);
        int damage = playerStats_.damage_;
        int armorPierce = playerStats_.armorPiercing_;
        // Critical hit?!
        if (enemyStats.AttemptCritical(attackRoll, playerStats_.criticalChance_)) {
            damage *= 2;
            armorPierce = (armorPierce + 1) * 2;
            enemyStats.AttemptDoDamage(damage, armorPierce);
            characterAnimator_.SetTrigger("Critical");
            enemy.characterAnimator_.SetTrigger("Stumble");
            player_Attack.Invoke(enemy, "Critical");
        }
        else {
            if (!enemyStats.AttemptDodge(attackRoll + playerStats_.attackRating_)) {
                enemyStats.AttemptDoDamage(damage, armorPierce);
                characterAnimator_.SetTrigger("Swing");
                enemy.characterAnimator_.SetTrigger("Stumble");
                player_Attack.Invoke(enemy, "Hit");
            }
            else {
                characterAnimator_.SetTrigger("Swing");
                enemy.characterAnimator_.SetTrigger("Dodge");
                player_Attack.Invoke(enemy, "Dodge");
            }
        };
        if (enemyStats.health <= 0) {
            enemy.Die((playerObj_.transform.position - enemy.transform.position).normalized);
            PlayerManager.instance_.AddXP(enemyStats.reference_);
        }
        
        yield return new WaitForSeconds(0.5f);
        EndTurn();
        isMoving_ = false;
    }


    public IEnumerator SmoothMove(Vector3 position, bool changefacing = true, bool endturn = true) {
        isMoving_ = true;
        Vector3 velocity = Vector3.zero;
        characterAnimator_.SetTrigger("Walk");
        //avatar_.transform.localRotation = Quaternion.Euler(direction.x, direction.y, direction.z);
        while (true) {
            playerObj_.transform.position = Vector3.SmoothDamp(playerObj_.transform.position, position, ref velocity, 0.1f);
            if (velocity.magnitude < 0.1f) { break; };
            yield return new WaitForEndOfFrame();
        }
        playerObj_.transform.position = position;
        isMoving_ = false;
        if (endturn) { EndTurn(); };

    }

    public Vector3 MoveUp(bool move = false, bool changeFacing = true, bool endTurn = true) {
        if (move) {
            StartCoroutine(SmoothMove(playerObj_.transform.position + new Vector3(0f, 0f, movementSteps_), changeFacing, endTurn));
        }
        if (changeFacing) {
            currentFacing_ = Directions.UP;
            avatar_.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
        }
        return playerObj_.transform.position + new Vector3(0f, 0f, movementSteps_);
    }
    public Vector3 MoveDown(bool move = false, bool changeFacing = true, bool endTurn = true) {
        if (move) {
            StartCoroutine(SmoothMove(playerObj_.transform.position + new Vector3(0f, 0f, -movementSteps_), changeFacing, endTurn));
        }
        if (changeFacing) {
            currentFacing_ = Directions.DOWN;
            avatar_.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
        };
        return playerObj_.transform.position + new Vector3(0f, 0f, -movementSteps_);
    }
    public Vector3 MoveLeft(bool move = false, bool changeFacing = true, bool endTurn = true) {
        if (move) {
            StartCoroutine(SmoothMove(playerObj_.transform.position + new Vector3(-movementSteps_, 0f, 0f), changeFacing, endTurn));
        }
        if (changeFacing) {
            currentFacing_ = Directions.LEFT;
            avatar_.transform.localRotation = Quaternion.Euler(new Vector3(0f, -180f, 0f));
        }
        return playerObj_.transform.position + new Vector3(-movementSteps_, 0f, 0f);
    }
    public Vector3 MoveRight(bool move = false, bool changeFacing = true, bool endTurn = true) {

        if (move) {
            StartCoroutine(SmoothMove(playerObj_.transform.position + new Vector3(movementSteps_, 0f, 0f), changeFacing, endTurn));
        }
        if (changeFacing) {
            currentFacing_ = Directions.RIGHT;
            avatar_.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
        return playerObj_.transform.position + new Vector3(movementSteps_, 0f, 0f);
    }

    public void Die(Vector3 direction) {

        //rb.isKinematic = false;
        //rb.WakeUp();
        //rb.AddForce(direction * (playerStats_.health - 1), ForceMode.Impulse);
        rb.gameObject.tag = "Untagged";
        rb.gameObject.layer = 9;
        dead_ = true;
        characterAnimator_.SetTrigger("Die");
        GameManager.instance_.GameOverDeath();

    }

    void EndTurn() {
        TurnManager._Instance.NextTurn(playerStats_);
    }

    // Update is called once per frame
    void Update () {
        
        if (isActive_) {
            MainUIManager.instance_.ActivateWaitingTurnPanel(false);
            if (!dead_) {
                if (!isMoving_) {
                    if (Input.GetKeyDown(KeyCode.UpArrow)) {
                        player_Move.Invoke(Directions.UP, GetValidMove(MoveUp()));
                        //Debug.Log("Up!");
                        if (GetValidMove(MoveUp())) {
                            MoveUp(true);
                        }
                        
                    }

                    if (Input.GetKeyUp(KeyCode.DownArrow)) {
                        player_Move.Invoke(Directions.DOWN, GetValidMove(MoveDown()));
                        //Debug.Log("Down!");
                        if (GetValidMove(MoveDown())) {
                            MoveDown(true);
                        }
                    }

                    if (Input.GetKeyUp(KeyCode.LeftArrow)) {
                        player_Move.Invoke(Directions.LEFT, GetValidMove(MoveLeft()));
                        //Debug.Log("Left!");
                        if (GetValidMove(MoveLeft())) {
                            MoveLeft(true);
                        }
                    }

                    if (Input.GetKeyUp(KeyCode.RightArrow)) {
                        player_Move.Invoke(Directions.RIGHT, GetValidMove(MoveRight()));
                        //Debug.Log("Right!");
                        if (GetValidMove(MoveRight())) {
                            MoveRight(true);
                        }
                    }
                    if (Input.GetKeyUp(KeyCode.Space)) {
                        player_Move.Invoke(Directions.NONE, true);
                        //Debug.Log("Waited!!");
                        playerStats_.RegenerateEnergy(1);
                        EndTurn();
                    }
                }
            }
            else {
                EndTurn();
                //this.enabled = false;
            }

        }
        else {
            MainUIManager.instance_.ActivateWaitingTurnPanel(true);
        }
        
    }
}
