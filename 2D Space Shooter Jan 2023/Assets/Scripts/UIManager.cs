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
    [SerializeField] private Text _gameOverText;

    [SerializeField] private float _textFlickerDelay = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = _scoreTextPrefix + 0;
        _gameOverText.gameObject.active = false;
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
        Debug.Log("UIManager::DisplayGameOver start");
        _gameOverText.gameObject.active=true;
        StartCoroutine(FlickerGameOverText());
    }

    IEnumerator FlickerGameOverText()
    {
        Debug.Log("UIManager::FlickerGameOverText start");
        while (true)
        {
            Debug.Log("UIManager::FlickerGameOverText loop at top");
            _gameOverText.gameObject.active=false;
            yield return new WaitForSeconds(_textFlickerDelay);
            Debug.Log("UIManager::FlickerGameOverText Delay DONE");
            _gameOverText.gameObject.active=true;
        }
        
    }
}
