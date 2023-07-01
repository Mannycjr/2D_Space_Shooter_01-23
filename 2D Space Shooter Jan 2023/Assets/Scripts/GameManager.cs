using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _isGameOver;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainGame_Scene")
        {
            if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true) {
                SceneManager.LoadScene(1); // Reloads same scene, Game scene
            }
            if (Input.GetKeyDown(KeyCode.M) && _isGameOver == true)
            {
                SceneManager.LoadScene(0); // Loads Main Menu
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape) && _isGameOver == true) // 
        {
            Application.Quit();
        }

    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    public void GameNotOver()
    {
        _isGameOver = false;
    }
}
