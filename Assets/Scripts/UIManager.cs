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
    [SerializeField]
    private Slider _ammoCount;
    private Vector3 _ammoCountStartPosition;
    [SerializeField]
    private Slider _thrustersHUD;
    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL");
        }

        _ammoCountStartPosition = _ammoCount.transform.localPosition;
    }

    // Update is called once per frame
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives >= 0 && currentLives < 4)
        {
            _livesImg.sprite = _liveSprites[currentLives];
            if (currentLives == 0) // Dead
            {
                GameOverSequence();
            }
        }
    }

    public void UpdateAmmo(byte _laserCount)
    {
        _ammoCount.value = _laserCount;
    }

    public void OutOfAmmo()
    {
        StartCoroutine(ShakeAmmoRoutine());
    }

    IEnumerator ShakeAmmoRoutine()
    {
        _ammoCount.transform.localPosition = _ammoCountStartPosition;
        _ammoCount.transform.localPosition = _ammoCountStartPosition + new Vector3(5f, 0, 0);
        yield return new WaitForSeconds(0.1f);
        _ammoCount.transform.localPosition = _ammoCountStartPosition;
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

    public void UpdateThruster(byte val)
    {
        _thrustersHUD.value = val;
    }
    public void SetThrusterBarColor(Color val)
    {
        Image img = _thrustersHUD.fillRect.GetComponent<Image>();
        if (img != null && val != null)
        {
            img.color = val;
        }
    }
}
