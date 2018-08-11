using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurnManager : MonoBehaviour {

    public static TurnManager _Instance;
    private List<ActiveEntity> turnOrder_ = new List<ActiveEntity> { };
    private int currentIndex_ = 0;

    public List<ActiveEntity> turnOrder {
        get {
            return turnOrder_;
        }
    }

    public ActiveEntity currentActive {

        get {
            if (turnOrder.Count > currentIndex_) {
                return turnOrder[currentIndex_];
            }
            else {
                Debug.Log("Reached end of list, updating!");
                UpdateTurnOrder();
                return currentActive;
            }
        }
    }

    void Awake() {
        if (_Instance == null) {
            _Instance = this;
        }
    }

    public void Start() {

        UpdateTurnOrder();
        

    }

    public void UpdateTurnOrder() {
        
        Debug.Log("Updating turn order");
        turnOrder.Clear();
        foreach (ActiveEntity entity in FindObjectsOfType<ActiveEntity>()) {
            turnOrder.Add(entity);
            entity.controllerActive = false;
        }
        turnOrder.Sort((x,y) => -y.speed_.CompareTo(-x.speed_));

        currentIndex_ = 0;
        currentActive.controllerActive = true;

        /*
        for (int i = 0; i < turnOrder_.Count; i++) {
            Debug.Log(turnOrder_[i].gameObject.name);
        }*/
    }



    public void NextTurn() {

        currentActive.controllerActive = false;
        currentIndex_ += 1;
        currentActive.controllerActive = true;
        Debug.Log(currentIndex_);
        
        
    }



}