using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultiplier = 2f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    [SerializeField]
    private bool _isTripleShotActive = false;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    //[SerializeField]
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private int _score;
    private UIManager _uiManager;
    // Start is called before the first frame update
    void Start()
    {
        // take the cuttent position = new position (0, 0, 0)
	    transform.position = new Vector3(0, 0, 0);

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI manager is NULL");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The spawn manager is NULL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();    
        }
    }
    
    void CalculateMovement() 
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        //transform.Translate(Vector3.right * 5 * Time.deltaTime);
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);
        
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f,0),0);

        if (transform.position.x > 11.3f) 
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        } else if (transform.position.x < -11.3f) 
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }

    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives--;

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);

  // erase all enemies
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemyToDestroy in enemies)
            {
                Destroy(enemyToDestroy);
            }
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speed /= _speedMultiplier;
        _isSpeedBoostActive = false;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);        
    //    StartCoroutine(ShieldPowerDownRoutine());
    }

    //IEnumerator ShieldPowerDownRoutine()
    //{
    //    yield return new WaitForSeconds(5.0f);
    //    _isShieldActive = false;
    //}

    // method to add score
    public void AddScore(int points)
    {
        // update score in ui
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
