using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Método de comunicación con el UI
/// Permite actualizar la barra de vida y la puntuación
/// </summary>
public class UIController : MonoBehaviour
{
    #region VARIABLES
    private Healthbar healthbar;    // Barra de vida (asset externo)
    private int score;  // Puntuación
    public Text scoreUI;
    public GameObject gameOverUI;
    public GameObject scoreTxtGameOverUI;
    public GameObject scoreGameOverUI;
    //public Text score;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        healthbar = GetComponentInChildren<Healthbar>();
    }
    private void Start()
    {
        healthbar.SetHealth(100);
    }
    #endregion

    #region PUBLIC METHODS
    public void SetHealth(int health)
    {
        healthbar.SetHealth(health);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreUI.text = score.ToString();
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        scoreTxtGameOverUI.SetActive(true);
        scoreGameOverUI.SetActive(true);
        scoreGameOverUI.GetComponent<Text>().text = score.ToString();
        int maxScore = PlayerPrefs.GetInt("maxScore");
        if (score > maxScore)
            PlayerPrefs.SetInt("maxScore", score);
    }
    #endregion
}
