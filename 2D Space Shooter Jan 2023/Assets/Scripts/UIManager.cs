using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    private string _scoreTextPrefix = "Score: ";
    [SerializeField] private Image _livesImageDisplay;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private GameObject _gameOverImage;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = _scoreTextPrefix + 0;
        _gameOverImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = _scoreTextPrefix + score;
    }

    public void UpdateLives(int currentLives)
    {
        _livesImageDisplay.sprite = _livesSprites[currentLives];
    }

    public void DisplayGameOver()
    {
        _gameOverImage.SetActive(true);
    }

}
