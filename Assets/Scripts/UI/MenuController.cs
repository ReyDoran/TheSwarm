using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controlador del menú
/// </summary>
public class MenuController : MonoBehaviour
{
    public GameObject arcadeBtn;
    public GameObject maxScoreTxt;
    public GameObject scoreTxt;
    public GameObject controlsBtn;    
    public GameObject quitBtn;
    public GameObject controlsTxt;
    public GameObject backBtn;

    private void Awake()
    {
        scoreTxt.GetComponent<Text>().text = PlayerPrefs.GetInt("maxScore", 0).ToString();
    }
    public void ShowControlsMenu()
    {
        arcadeBtn.SetActive(false);
        maxScoreTxt.SetActive(false);
        scoreTxt.SetActive(false);
        controlsBtn.SetActive(false);
        quitBtn.SetActive(false);
        controlsTxt.SetActive(true);
        backBtn.SetActive(true);
    }

    public void ShowMainMenu()
    {
        arcadeBtn.SetActive(true);
        maxScoreTxt.SetActive(true);
        scoreTxt.SetActive(true);
        controlsBtn.SetActive(true);
        quitBtn.SetActive(true);
        controlsTxt.SetActive(false);
        backBtn.SetActive(false);
    }

    public void PlayArcadeMode()
    {
        SceneManager.LoadScene("ArcadeScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
