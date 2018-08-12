using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance_;
    public AudioSource audio_;

    public string[] scenes_;
    public int currentScene_;

    public AudioClip[] musics_;

    public AudioClip[] hitSounds_;
    public AudioClip[] deathSounds_;
    public AudioClip[] missSounds_;

    public string nextLevel_;

    void Awake() {

        if (instance_ == null) {
            instance_ = this;
        }
        else {
            GameObject.Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }

    public void PlayMusic(int song = -1) {
        //-1 = play random

        AudioClip targetclip;

        if (song < 0 || song > musics_.Length) {

            targetclip = musics_[Random.Range(0, musics_.Length)];
            audio_.clip = targetclip;
            audio_.Play();
        }
        else {
            audio_.clip = musics_[song];
            audio_.Play();
        }

    }

    public void PlayHitSound() {
        AudioClip targetclip;
            targetclip = hitSounds_[Random.Range(0, hitSounds_.Length)];
            audio_.PlayOneShot(targetclip);
    }

    public void RestartLevel() {

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        
    }

    public void LoadNextLevel() {

        if (nextLevel_ == "") {
            LoadLevel(currentScene_ + 1, true);
        }
        else {
            LoadLevel(nextLevel_);
            nextLevel_ = "";
        }

    }

    public void LoadLevel(int level, bool saveStats = false) {

        if (level < scenes_.Length) {
            if (saveStats) {
                PlayerPrefs.SetInt("LastLevel", level);
                PlayerPrefs.SetInt("PlayerLevel", PlayerManager.currentPlayerLevel_);
                PlayerPrefs.SetInt("PlayerExperience", PlayerManager.playerXP_);
            }
            SceneManager.LoadSceneAsync(scenes_[level]);
        }
        else {
            // No more scenes, back to start!
            SceneManager.LoadSceneAsync(scenes_[0]);
        }
    }
    public void LoadLevel(string level, bool saveStats = false) {

        if (level != "") {
            if (saveStats) {
                PlayerPrefs.SetInt("LastLevel", FindSceneIndex(level));
                PlayerPrefs.SetInt("PlayerLevel", PlayerManager.currentPlayerLevel_);
                PlayerPrefs.SetInt("PlayerExperience", PlayerManager.playerXP_);
            }
            SceneManager.LoadSceneAsync(level);
        }
        else {
            // No more scenes, back to start!
            SceneManager.LoadSceneAsync(scenes_[0]);
        }
    }

    public int FindSceneIndex(string sceneName) {
        for (int i = 0; i < scenes_.Length; i++) {
            if (scenes_[i] == sceneName) {
                return i;
            }
        }
        return -1;
    }

    public void ResetPlayer() {
        PlayerPrefs.SetInt("LastLevel", 0);
        PlayerPrefs.SetString("PlayerName", "");
        PlayerPrefs.SetInt("PlayerLevel", 1);
        PlayerPrefs.SetInt("PlayerExperience", 0);

    }

    public void QuitGame() {

        Application.Quit();
    }

    public void GameOverDeath() {

        MainUIManager.instance_.GameOverPanel();
    }

	// Use this for initialization
	void Start () {
        // Play music!
        PlayMusic();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
