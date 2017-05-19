﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Transform gameOverPanel;
    public Transform howToPlayPanel;
    public Transform pausePanel;
    public Transform fadePanel;

    [Header("Audio")]
    public AudioSource backGroundMusic;
    public AudioSource pauseClick;
    public AudioSource restartClick;

    static public Transform staticGameOverPanel;

    static public bool isGameOver = false;

    private void Start () {
        isGameOver = false;
        staticGameOverPanel = gameOverPanel;
        fadePanel.gameObject.SetActive(true);
    }

    static public void gameOver() {
        isGameOver = true;
        staticGameOverPanel.gameObject.SetActive(true);
    }

    public void restartGame() {
        restartClick.Play();
        resumeGame();
        StartCoroutine(sceneFade("scene01"));
    }

    public void pauseGame() {
        pausePanel.gameObject.SetActive(true);
        Time.timeScale = 0;
        backGroundMusic.Pause();
        pauseClick.Play();
    }

    public void resumeGame() {
        pausePanel.gameObject.SetActive(false);
        Time.timeScale = 1;
        backGroundMusic.UnPause();
    }

    public void quitGame() {
        Application.Quit();
    }

    public void hideHowToPlay() {
        howToPlayPanel.gameObject.SetActive(false);
    }

    public void increaseQuailty() {
        QualitySettings.IncreaseLevel(true);
    }

    public void decreaseQuailty () {
        QualitySettings.DecreaseLevel(true);
    }

    public void loadMainMenu() {
        StartCoroutine(sceneFade("MainMenu"));
    }

    public IEnumerator sceneFade(string newScene) {
        fadePanel.GetComponent<Animator>().SetBool("fade", true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(newScene);
    }

    void Update() {
        //pausing game when clicking back button
        if (isGameOver) {
            backGroundMusic.Pause();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            pauseGame();
        }
    }
}
