using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _isGameOver = true;

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainGame_Scene" && _isGameOver == true)
        {
            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene(1); // Reloads same scene, Game scene
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                SceneManager.LoadScene(0); // Loads Main Menu
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) // 
        {
            Debug.Log("ESC General");
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

    public void StartSpawning()
    {

    }
}
