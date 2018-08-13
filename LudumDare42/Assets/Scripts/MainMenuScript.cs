using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour {

    public Button continuegame;

	// Use this for initialization
	void Start () {
		
        string lastLevel = PlayerPrefs.GetString("LastLevel");
        if (lastLevel != "" && continuegame != null) {
            continuegame.interactable = true;
                }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void QuitApp() {

        Application.Quit();
    }

    public void ContinueApp() {
        GameManager.instance_.ContinueFromLastLevel();
    }

    public void BackToMainMenu() {

        GameManager.instance_.LoadLevel("startscene");
    }

    public void StartApp() {

        PlayerPrefs.SetString("LastLevel", "");
        GameManager.instance_.ResetPlayer();
        UnityEngine.SceneManagement.SceneManager.LoadScene("level1");
    }

}
