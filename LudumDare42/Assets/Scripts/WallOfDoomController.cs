using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfDoomController : MonoBehaviour {

    public GameObject wall_;
    public int directionofMovement_;
    public int stepsToTake_ = 10;
    public int stepsTaken_ = 0;
    public bool reverseMovement_ = true;
    public Grid movementGrid_;

    public int gridUnitsToMove_ = 1;
    private float movementSteps_;

    public bool isActive_;
    public bool isMoving_;

    public bool controllerActive {
        get {
                return isActive_;
            }
        set {
            isActive_ = value;
        }
    }

    public void MoveWall() {
        if (isActive_) {
            if (stepsTaken_ < stepsToTake_) {
                isMoving_ = false;
                switch (directionofMovement_) {
                    case 0: {
                            MoveUp(true);
                            break;
                        }
                    case 1: {
                            MoveDown(true);
                            break;
                        }
                    case 2: {
                            MoveLeft(true);
                            break;
                        }
                    case 3: {
                            MoveRight(true);
                            break;
                        }
                }
                stepsTaken_ += 1;
            };
        };
        /*
        if (reverseMovement_ && stepsTaken_ >= stepsToTake_) {
            // Go back the opposite way
            switch (directionofMovement_) {
                case 0: {
                        directionofMovement_ = 1;
                        break;
                    }
                case 1: {
                        directionofMovement_ = 0;
                        break;
                    }
                case 2: {
                        directionofMovement_ = 3;
                        break;
                    }
                case 3: {
                        directionofMovement_ = 2;
                        break;
                    }
            }
            stepsTaken_ = 0;
        }*/
        
    }

    void OnTriggerEnter(Collider coll) {
        //Debug.Log("Collided with " + coll.gameObject.name);
        ActiveEntity entity = coll.gameObject.GetComponentInParent<ActiveEntity>();

        if(entity != null) {
            if (entity.health > 0) {
                entity.AttemptDoDamage(999, 999);
            };
        }
    }

    public IEnumerator SmoothMove(Vector3 position) {

        isMoving_ = true;
        StartCoroutine(FinishMove(position));
        Vector3 velocity = Vector3.zero;
        while (isMoving_) {
            wall_.transform.position = Vector3.SmoothDamp(wall_.transform.position, position, ref velocity, 0.1f);
            if (velocity.magnitude < 0.1f) { break; };
            yield return new WaitForEndOfFrame();
        }
        isMoving_ = false;
    }

    public IEnumerator FinishMove(Vector3 position) {
        yield return new WaitUntil(() => !isMoving_);
        wall_.transform.position = position;
    }

    // Use this for initialization
    void Start () {

        if (movementGrid_ == null) {
            movementGrid_ = FindObjectOfType<Grid>();
        }

        movementSteps_ = movementGrid_.cellSize.x + movementGrid_.cellGap.x;

    }

    public Vector3 MoveUp(bool move = false) {
        if (move) {
            StartCoroutine(SmoothMove(wall_.transform.position + new Vector3(0f, 0f, movementSteps_)));
        }
        return wall_.transform.position + new Vector3(0f, 0f, movementSteps_);
    }
    public Vector3 MoveDown(bool move = false) {
        if (move) {
            StartCoroutine(SmoothMove(wall_.transform.position + new Vector3(0f, 0f, -movementSteps_)));
        }
        return wall_.transform.position + new Vector3(0f, 0f, -movementSteps_);
    }
    public Vector3 MoveLeft(bool move = false) {
        if (move) {
            StartCoroutine(SmoothMove(wall_.transform.position + new Vector3(-movementSteps_, 0f, 0f)));
        }
        return wall_.transform.position + new Vector3(-movementSteps_, 0f, 0f);
    }
    public Vector3 MoveRight(bool move = false) {

        if (move) {
            StartCoroutine(SmoothMove(wall_.transform.position + new Vector3(movementSteps_, 0f, 0f)));
        }

        return wall_.transform.position + new Vector3(movementSteps_, 0f, 0f);
    }

}
