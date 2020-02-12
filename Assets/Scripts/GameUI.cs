using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject GameLoseUI;
    public GameObject GameWinUI;
    private bool gameIsOver;
    
    // Start is called before the first frame update
    void Start()
    {
        Guard.GardSpotPlayer += ShowGameLoseUI;
        FindObjectOfType<Player>().FinAtteinte += ShowGameWinUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void ShowGameWinUI()
    {
        OnGameOver(GameWinUI);
    }
    
    void ShowGameLoseUI()
    {
       OnGameOver(GameLoseUI);
    }

    void OnGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        gameIsOver = true;
        Guard.GardSpotPlayer -= ShowGameLoseUI;
        FindObjectOfType<Player>().FinAtteinte -= ShowGameWinUI;

    }
}
