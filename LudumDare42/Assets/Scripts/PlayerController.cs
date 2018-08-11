using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour {

    public GameObject playerObj_;
    public Grid movementGrid_;
    public ActiveEntity playerStats_;
    public bool isActive_ = true;
    public int gridUnitsToMove_ = 1;
    private float movementSteps_;
    private bool isMoving_ = false;
    private Rigidbody rb;

	// Use this for initialization
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

        TurnManager mng = TurnManager._Instance;
    }
	
    public bool GetValidMove(Vector3 goal) {

        /*for (int i = 0; i < Physics.OverlapSphere(goal, 0.5f, 9).Length; i++) {
            Debug.Log(Physics.OverlapSphere(goal, 0.5f, 9)[i].gameObject.name);
        }*/
        Collider[] colls = Physics.OverlapSphere(goal, 0.1f, 9);

        if (colls.Length == 0) {
            //Debug.Log("Valid move! ");
            return true;
        }
        else {
            //Debug.Log("Invalid move! Checking for enemy");
            EnemyController enemy = DetectDestroyable(colls);
            if (enemy != null) {
                StartCoroutine(Attack(enemy));
            }
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
        Debug.Log("Attacking " + enemy.gameObject.name);

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
        }
        else {
            if (!enemyStats.AttemptDodge(attackRoll + playerStats_.attackRating_)) {
                enemyStats.AttemptDoDamage(damage, armorPierce);
            };
        };
        if (enemyStats.health <= 0) {
            enemy.Die((playerObj_.transform.position - enemy.transform.position).normalized);
        }

        yield return new WaitForSeconds(0.5f);
        TurnManager._Instance.NextTurn();
    }


    public IEnumerator SmoothMove(Vector3 position) {
        isMoving_ = true;
        Vector3 velocity = Vector3.zero;
        while (true) {
            playerObj_.transform.position = Vector3.SmoothDamp(playerObj_.transform.position, position, ref velocity, 0.1f);
            if (velocity.magnitude < 0.1f) { break; };
            yield return new WaitForEndOfFrame();
        }
        playerObj_.transform.position = position;
        isMoving_ = false;
        TurnManager._Instance.NextTurn();

    }

    public Vector3 MoveUp(bool move = false) {
        if (move) {
            StartCoroutine(SmoothMove(playerObj_.transform.position + new Vector3(0f, 0f, movementSteps_)));
        }
        return playerObj_.transform.position + new Vector3(0f, 0f, movementSteps_);
    }
    public Vector3 MoveDown(bool move = false) {
        if (move) {
            StartCoroutine(SmoothMove(playerObj_.transform.position + new Vector3(0f, 0f, -movementSteps_)));
        }
        return playerObj_.transform.position + new Vector3(0f, 0f, -movementSteps_);
    }
    public Vector3 MoveLeft(bool move = false) {
        if (move) {
            StartCoroutine(SmoothMove(playerObj_.transform.position + new Vector3(-movementSteps_, 0f, 0f)));
        }
        return playerObj_.transform.position + new Vector3(-movementSteps_, 0f, 0f);
    }
    public Vector3 MoveRight(bool move = false) {

        if (move) {
            StartCoroutine(SmoothMove(playerObj_.transform.position + new Vector3(movementSteps_, 0f, 0f)));
        }

        return playerObj_.transform.position + new Vector3(movementSteps_, 0f, 0f);
    }

    public void Die(Vector3 direction) {

        rb.isKinematic = false;
        rb.WakeUp();
        rb.AddForce(direction * -5f, ForceMode.Impulse);
        rb.gameObject.tag = "Untagged";
        rb.gameObject.layer = 9;

    }

    // Update is called once per frame
    void Update () {

        if (isActive_) {
            if (!isMoving_) {
                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    //Debug.Log("Up!");
                    if (GetValidMove(MoveUp())) {
                        MoveUp(true);
                    }

                }

                if (Input.GetKeyUp(KeyCode.DownArrow)) {
                    //Debug.Log("Down!");
                    if (GetValidMove(MoveDown())) {
                        MoveDown(true);
                    }
                }

                if (Input.GetKeyUp(KeyCode.LeftArrow)) {
                    //Debug.Log("Left!");
                    if (GetValidMove(MoveLeft())) {
                        MoveLeft(true);
                    }
                }

                if (Input.GetKeyUp(KeyCode.RightArrow)) {
                    //Debug.Log("Right!");
                    if (GetValidMove(MoveRight())) {
                        MoveRight(true);
                    }
                }
            }
        }

    }
}
