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
    private GameObject _rightEngine, _leftEngine;
    [SerializeField]
    private int _score;
    private UIManager _uiManager;
    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;
    private byte _shieldStrength = 0;
    [SerializeField]
    private byte _laserCount = 15;
    [SerializeField]
    private AudioClip _laserOutOfAmmo;
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

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("The AudioScourse is NULL.");
        }
        if (_laserCount == 0)
        {
            _laserCount = 15;
            _uiManager.UpdateAmmo(_laserCount);
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

        float speed = _speed;
        if (Input.GetKey(KeyCode.LeftShift)) {
            speed *= 2;
        }

        transform.Translate(direction * speed * Time.deltaTime);
        
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
        if (_laserCount < 1)
        {
            // sound, ammo shake
            _audioSource.clip = _laserOutOfAmmo;
            _audioSource.Play();
            _uiManager.OutOfAmmo();
            return;
        }
        _laserCount -= 1;
        _uiManager.UpdateAmmo(_laserCount);

        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.clip = _laserSoundClip;
        _audioSource.Play();
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            _shieldStrength -= 1;
            if (_shieldStrength < 1)
            {
                _isShieldActive = false;
                _shieldVisualizer.SetActive(false);
            }
            else
            {
                ChangeShieldColor(_shieldStrength);
            }
            return;
        }

        _lives--;

        
        //if lives = 2, damage right engine
        // if 1 damage left engine
        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        } else if (_lives == 1) {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
        else
        {
            ShakeCamera(0.5f, 0.2f); // duartion and magnitude
        }

    }

    public void ShakeCamera(float _shakeDuration, float _shakeMagnitude)
    {
        StartCoroutine(ShakeRoutine(_shakeDuration, _shakeMagnitude));
    }

    IEnumerator ShakeRoutine(float _shakeDuration, float _shakeMagnitude)
    {
        Camera camera = Camera.main;
        Vector3 cameraPosition = camera.transform.localPosition;
        float endTime = Time.time + _shakeDuration;
        while (endTime > Time.time)
            {
                camera.transform.localPosition = cameraPosition + Random.insideUnitSphere * _shakeMagnitude;
                yield return null;
            }
        camera.transform.localPosition = cameraPosition;
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

        if (_shieldStrength < 3)
        {
            _shieldStrength += 1;
            ChangeShieldColor(_shieldStrength);
        }
    }

    void ChangeShieldColor(byte _shieldStrength)
    {
        List<Color> _shieldColors = new List<Color>() { Color.white, Color.green, Color.red };
        SpriteRenderer shieldSpriteRender = _shieldVisualizer.GetComponent<SpriteRenderer>();
        shieldSpriteRender.color = _shieldColors[_shieldStrength - 1];
    }

    // method to add score
    public void AddScore(int points)
    {
        // update score in ui
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
