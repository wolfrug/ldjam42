using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private Rigidbody rb;
    public bool isActive_;
    public bool debugWaiter_;

	// Use this for initialization
	void Start () {
        rb = GetComponentInChildren<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

        if (isActive_) {
            if (!debugWaiter_) {
                Debug.Log("Enemy " + gameObject.name + " activated!");
                StartCoroutine(DebugWaiter());
            };
        }
		
	}

    private IEnumerator DebugWaiter() {
        debugWaiter_ = true;
        yield return new WaitForSeconds(1f);
        TurnManager._Instance.NextTurn();
        debugWaiter_ = false;
    }

    public void Die(Vector3 direction) {

        rb.isKinematic = false;
        rb.WakeUp();
        rb.AddForce(direction*-5f, ForceMode.Impulse);
        rb.gameObject.tag = "Untagged";
        rb.gameObject.layer = 9;

    }

}
