using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    private string _scoreTextPrefix = "SCORE: ";
    [SerializeField] private TMP_Text _ammoText;
    private string _ammoTextPrefix = "AMMO: ";
    [SerializeField] private Image _livesImageDisplay;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private TMP_Text _restartText;

    [SerializeField] private float _textFlickerDelay = 0.25f;

    [SerializeField] private Slider _thrustersSlider;
    [SerializeField] private Image _thrustersSliderFill;

    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = _scoreTextPrefix + 0;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("UIManager::Start : no Game_Manager");
        } else
        {
            _gameManager.GameNotOver();
            Debug.Log("UIManager::Start : New Game Started");
        }

        if (_thrustersSlider == null)
        {
            Debug.LogError("UIManager::Start : no _thrustersSlider");
        }

        if (_thrustersSliderFill == null)
        {
            Debug.LogError("UIManager::Start : no _thrustersSliderFill");
        }
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

    public void GameOverSequence()
    {
        Debug.Log("UIManager::DisplayGameOver start");
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        _gameManager.GameOver();
        StartCoroutine("GameOverTextFlickerRoutine");
    }

    IEnumerator GameOverTextFlickerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_textFlickerDelay);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(_textFlickerDelay);
            _gameOverText.gameObject.SetActive(true);
        }

    }

    public void DisplayRestartText()
    {
        _restartText.gameObject.SetActive(true);
    }

    public void UpdateAmmo(int _playerAmmo, int _playerAmmoMax)
    {
        _ammoText.text = _ammoTextPrefix + _playerAmmo.ToString() + " / " + _playerAmmoMax.ToString();
    }

    public void UpdateThrustersSlider(float thrustValue)
    {
        if (thrustValue >= 0 && thrustValue <= 10)
        {
            _thrustersSlider.value = thrustValue;
        }
    }

    public void ThurstersSliderUsableColor(bool usableThrusters)
    {
        if (usableThrusters)
        {
            _thrustersSliderFill.color = Color.green;
        }
        else if (!usableThrusters)
        {
            _thrustersSliderFill.color = Color.red; // 25% of total slider. Logic in Player.cs::ThrustersActive() 
        }
    }
}
