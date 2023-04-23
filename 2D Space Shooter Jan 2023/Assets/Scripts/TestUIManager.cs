using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class TestUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text _gameOverText;
    
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine("GameOverTextFlickerRoutine");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GameOverTextFlickerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(true);
        }

    }

    IEnumerator FlickerGameOverText()
    {
        Debug.Log("UIManager::FlickerGameOverText start");
        while (true)
        {
            Debug.Log("UIManager::FlickerGameOverText loop at top");
            _gameOverText.gameObject.active = false;
            yield return new WaitForSeconds(0.5f);
            Debug.Log("UIManager::FlickerGameOverText Delay DONE");
            _gameOverText.gameObject.active = true;
        }

    }
}
