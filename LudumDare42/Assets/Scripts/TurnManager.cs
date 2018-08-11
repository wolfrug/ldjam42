﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurnManager : MonoBehaviour {

    public static TurnManager _Instance;
    private List<ActiveEntity> turnOrder_ = new List<ActiveEntity> { };
    private ActiveEntity[] allEntities_;
    private int currentIndex_ = 0;

    public ActiveEntity currentActive {

        get {
            if (turnOrder_.Count > currentIndex_) {
                return turnOrder_[currentIndex_];
            }
            else {
                
                return null;
            }
        }
    }

    void Awake() {
        if (_Instance == null) {
            _Instance = this;
        }
        else if (_Instance != this) {
            Destroy(gameObject);
        }
        allEntities_ = FindObjectsOfType<ActiveEntity>();
    }

    public void Start() {

        //StartCoroutine(UpdateTurnOrder());

    }

    void StartNewTurn() {
        StopAllCoroutines();
        turnOrder_.Clear();
        turnOrder_.Capacity = 0;
        foreach (ActiveEntity entity in allEntities_) {
            if (entity.health > 0) {
                turnOrder_.Add(entity);
            }
            entity.controllerActive = false;
        }
        turnOrder_.Sort((x, y) => y.speed_.CompareTo(x.speed_));

        // If everyone is dead, we end the game
        if (turnOrder_.Count == 0) {
            EndGame();
            return;
        }

        for (int i = 0; i < turnOrder_.Count; i++) {
            turnOrder_[i].turnOrder = "Turn: " + i.ToString();
        }

        StartCoroutine(WallsOfDoom());
    }

    IEnumerator WallsOfDoom() {
        // All walls of doom run last, so run them now
        foreach (WallOfDoomController doom in FindObjectsOfType<WallOfDoomController>()) {
            doom.MoveWall();
        }
        yield return new WaitForSeconds(1f);

        currentIndex_ = 0;
        if (currentActive != null) {
            currentActive.controllerActive = true; }
    }



    public void NextTurn(ActiveEntity sender) {

        if (currentActive != null) {

            if (sender == currentActive) {
                currentActive.controllerActive = false;
                currentIndex_ += 1;
                if (currentActive != null) {
                    currentActive.controllerActive = true;
                }
                else {
                    StartNewTurn();
                }
            }
            else {
                Debug.Log("Wrong sender attempted to end turn: " + sender.gameObject.name);
            }
        }
        else {
            StartNewTurn();
        };
        //Debug.Log(currentIndex_);

        
        
    }

    public void EndGame() {
        Debug.LogWarning("FINISHED GAME: ALL DEAD");
    }

}