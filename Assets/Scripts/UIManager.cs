using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    //handle to text
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;

    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        //_liveSprites[] = 3;
        //assign text component to the
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL");
        }
    }

    // Update is called once per frame
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        _livesImg.sprite = _liveSprites[currentLives];
        if (currentLives  == 0) // Dead
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
