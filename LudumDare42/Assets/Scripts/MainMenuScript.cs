using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void QuitApp() {

        Application.Quit();
    }

    public void StartApp() {

        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

}
