using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject target;
    private Vector3 velocity = Vector3.zero;
    // Use this for initialization
    void Start() {
        if (target == null) {
            target = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update() {

        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position+new Vector3(0f,12f,-2f), ref velocity, 0.5f);
    }
}
