using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4f;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;
    [SerializeField]
    private GameObject _laserPrefab;
    private float _fireRate = 3.0f;
    private float _canFireTimer = -1;
    private bool _canFire = true;
    private int _movementPattern = 0;
    private int _movementLeftRight = 0;
    private bool _movementUp = false;

    private float _angle = 0f;

    private bool _canFireFunction = true;
    private float _movementTimer = 0f;
    private float _loverLimit = 0f;
    private int _superPowerType = 0;
    private SpawnManager _spawnManager;
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldPrefab;
    private GameObject _enemyShield;
    private byte _shieldStrength = 1;
    private bool _avoidShots = false;
    [SerializeField]
    private bool _boss = false;
    [SerializeField]
    private GameObject _explosionPrefab;
    private float timer = 0.0f;
    // // // // // // // // // // // // // // // // // // // // 
    // canFire = true. Can fire, damage the player
    //
    // gotShield = true
    //
    // superPowerType = 0 - regular
    //  1 - double laser
    //  2 - laser beam
    //  3 - powerup killer
    //  4 - backwards sniper
    //  5 - fan

    // behavior = 0 - linear,
    // 1 - horizontal zig-zag side to side
    // 2 - circle
    // 3 - angle
    // 4 - vertical zig-zag
    // 5 - side to side
    // 6 - ram. If an enemy is close toa player, the enemy will try and “ram” it
    // 7 - boss. Moves down the screen to the center and stays there.
    // avoidShots - will move away from laser

    public void setEnemyType(bool canFire, bool gotShield, int superPowerType, int behavior, bool avoidShots)
    {

        if (canFire == false) _canFireFunction = canFire;

        _isShieldActive = gotShield;

        if (superPowerType > 0) _superPowerType = superPowerType;

        if (behavior > 0) _movementPattern = behavior;

        _avoidShots = avoidShots;

    }

    void ChangeShieldColor(byte _shieldStrength)
    {
        List<Color> _shieldColors = new List<Color>() { Color.white, Color.green, Color.red };
        SpriteRenderer shieldSpriteRender = _enemyShield.GetComponent<SpriteRenderer>();
        shieldSpriteRender.color = _shieldColors[_shieldStrength - 1];
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }
        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL");
        }
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("The AudioSource is NULL");
        }

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The spawn manager is NULL");
        }

        _movementLeftRight = Random.Range(0, 2);

        _loverLimit = Random.Range(0.0f, 4.6f);

        setEnemyType(Random.value < 0.5f, Random.value < 0.5f, Random.Range(0, 5), Random.Range(0, 7), Random.value < 0.2f);


        _enemyShield = Instantiate(_shieldPrefab, this.transform.position, Quaternion.identity);
        _enemyShield.transform.SetParent(this.transform);
        _enemyShield.transform.localScale = new Vector3(1f, 1f, 1f);

        if (_boss)
        {
            _enemyShield.transform.localScale += new Vector3(0.8f, 0, 0);
            _superPowerType = 5;
            _canFireFunction = true;
            _isShieldActive = true;
            _shieldStrength = 3;
            _movementPattern = 7;
            _loverLimit = 3f;
        }

        if (_isShieldActive)
        {
            _enemyShield.SetActive(true);
        }
        ChangeShieldColor(_shieldStrength);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        Fire();
    }
 
    void CalculateMovement()
    {

        Vector3 vector = Vector3.down;
        switch (_movementPattern)
        {
            // behavior = 0 - linear,
            // 1 - horizontal zig-zag side to side,
            // 2 - circle,
            // 3 - angle,
            // 4 - vertical zig-zag
            // 5 - side to side
            // 6 - ram
            
            case 0:
                break;
            case 1: // side to side zig-zag

                if (_movementUp)
                {
                    vector = Vector3.up;
                    if (transform.position.y > 4.54f) _movementUp = false;
                }
                else
                {
                    if (transform.position.y < 1f) _movementUp = true;  
                }


                if (_movementLeftRight == 1)    // right. change direction
                {
                    if (transform.position.x > 10f) _movementLeftRight = 0;
                }
                else                            // left. change direction
                {
                    if (transform.position.x < -10f) _movementLeftRight = 1;   
                }

                vector = (_movementLeftRight == 0) ? vector + Vector3.left : vector + Vector3.right;

                break;
            case 2: // circling
              
                float RotateSpeed = 1f;
                float RotateRadiusX = 1f;
                float RotateRadiusY = 1f;
                Vector3 _rotationCenter = Vector3.down * _speed * Time.deltaTime;

                _angle +=  RotateSpeed * Time.deltaTime;
                var x = Mathf.Sin(_angle) * RotateRadiusX;
                var y = Mathf.Cos(_angle) * RotateRadiusY;
                vector = _rotationCenter + new Vector3(x, y);

                break;
            case 3: // angle
                vector =  (_movementLeftRight == 0) ? Vector3.down + Vector3.left : Vector3.down + Vector3.right;
                break;
            case 4: // vertical zig-zag 

                _movementTimer += Time.deltaTime;
                if (_movementTimer > 1.2f)
                {
                    _movementLeftRight = (_movementLeftRight == 1) ? 0 : 1;
                    _movementTimer = 0f;
                }

                vector = (_movementLeftRight == 0) ? vector + Vector3.left : vector + Vector3.right;

                break;
            case 5: // side to side

                if (_loverLimit > transform.position.y)
                {
                    if (_movementLeftRight == 1)    // right. change direction
                    {
                        if (transform.position.x > 10f) _movementLeftRight = 0;
                    }
                    else                            // left. change direction
                    {
                        if (transform.position.x < -10f) _movementLeftRight = 1;
                    }

                    vector = (_movementLeftRight == 0) ?  Vector3.left : Vector3.right;
                }

                break;
            case 6:
                if (_player != null)
                {
                    if (Vector3.Distance(transform.position, _player.transform.position) < 4f)
                    {
                        vector = Vector3.zero;
                        Vector3 target = _player.transform.position;
                        transform.position = Vector3.MoveTowards(transform.position, target, _speed * Time.deltaTime);

                        //rotate
                        float rotationSpeed = 5f;
                        float offset = 90f;
                        Vector3 direction = target - transform.position;
                        direction.Normalize();
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        Quaternion rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
                    }
                    else
                    {
                         transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 5f * Time.deltaTime);
                    }
                }
                break;
            case 7:
                if (_loverLimit > transform.position.y)
                {
                    vector = Vector3.zero;
                }
                break;
            default:
                Debug.Log("The Movement pattern is out of scope: " + _movementPattern.ToString());
                break;
        }

        // 7 - avoid shoots
        if (_avoidShots)
        {

            RaycastHit2D hit = Physics2D.CircleCast(transform.position - new Vector3(0, 1f, 0), 2f, Vector2.down * 10);

//          Debug.DrawRay(transform.position - new Vector3(0, 1f, 0), Vector2.down * 10, Color.red, 0.01f);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Laser"))
                {
                    if (!hit.collider.GetComponent<Laser>().IsEnemyLaser())
                    {
                        Vector3 teleport = (hit.collider.transform.position.x > transform.position.x) ? transform.position + new Vector3(-3f, 0, 0) : transform.position + new Vector3(3f, 0, 0);
                        transform.position = teleport;
                    }
                }

            }
        }


        transform.Translate(vector * _speed * Time.deltaTime);

        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7, 0);
        }

        if (transform.position.x > 11.3f)   // left and right of the camera
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void Fire()
    {
        ////
        //  0 - regular laser
        //  1 - double laser
        //  2 - laser beam
        //  3 - powerup killer
        //  4 - backwards sniper
        //  5 - fan

       if (_canFireFunction && _canFire && Time.time > _canFireTimer)
        {
            switch (_superPowerType)
            {
                case 2:
                    _fireRate = Random.Range(3f, 7f);
                    _canFireTimer = Time.time + _fireRate;

                    StartCoroutine(FireLaserBeam());
                    break;
                case 3:
                    RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, 1f, 0), Vector2.down * 10);

                    //Debug.DrawRay(transform.position - new Vector3(0, 1f, 0), Vector2.down * 10, Color.red, 0.01f);

                    if (hit.collider != null)
                    {
                        //Debug.Log(hit.collider.name);
                        if ((hit.collider.CompareTag("Powerup") || hit.collider.CompareTag("Player")) && _canFire && Time.time > _canFireTimer)
                        {
                           ShootOne();
                        }
                    }
                    break;
                case 4:

                    RaycastHit2D hitBack = Physics2D.Raycast(transform.position + new Vector3(0, 1f, 0), Vector2.up * 10);

                    //Debug.DrawRay(transform.position + new Vector3(0, 1f, 0), Vector2.up * 10, Color.red, 0.01f);

                    if (hitBack.collider != null)
                    {
                        if (hitBack.collider.CompareTag("Player"))
                        {
                            ShootOne(true);
                        }
                    }

                    break;
                case 5:
                    ShootFan();
                    break;
                default:
                    ShootOne();
                    // will convert to 1 laser..
                    break;
            }

        }

    }

    private void ShootFan()
    {
        _fireRate = Random.Range(3f, 7f);
        _canFireTimer = Time.time + _fireRate;

        int bulletCount = 5;
        float subtractOffset = 2f;
        float angle = 9f;
        for (int i = 0; i < bulletCount; i++)
        {
            CreateBullet(new Vector3(0f, 0f, ((i - subtractOffset) * angle)));
        }
    }

    void CreateBullet(Vector3 offsetRotation)
    {
        Vector3 laserPosition = transform.position;
        if (_boss)
        {
            laserPosition += new Vector3(0, -1.3f, 0);
        }
        GameObject bulletClone = Instantiate(_laserPrefab, laserPosition, transform.rotation);
        bulletClone.transform.Rotate(offsetRotation);

        Laser[] lasers = bulletClone.GetComponentsInChildren<Laser>();
        foreach (Laser laser in lasers)
        {
            laser.AssignEnemyLaser();
        }
    }

    private void ShootOne(bool backwards = false)
    {
        _fireRate = Random.Range(3f, 7f);
        _canFireTimer = Time.time + _fireRate;

        Vector3 laserPosition = transform.position;
        if (backwards)
        {
            laserPosition += new Vector3(0, 3f, 0);
        } else if(_boss)
        {
            laserPosition += new Vector3(0, -1.3f, 0);
        }

        GameObject enemyLaser = Instantiate(_laserPrefab, laserPosition, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
        foreach (Laser laser in lasers)
        {
            laser.AssignEnemyLaser(backwards);
        }
    }

    IEnumerator FireLaserBeam()
    {
        for (byte i = 0; i < 6; i++)
        {
            Vector3 laserPosition = transform.position;
            if (_boss)
            {
                laserPosition += new Vector3(0, -2f, 0);
            }

            GameObject enemyLaser = Instantiate(_laserPrefab, laserPosition, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            foreach (Laser laser in lasers)
            {
                laser.AssignEnemyLaser();
               
            }
            yield return new WaitForSeconds(0.1f);
        }
        // to be improved..
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
            }

            if (_isShieldActive)
            {
                _shieldStrength -= 1;
                if (_shieldStrength < 1)
                {
                    _isShieldActive = false;
                    _enemyShield.SetActive(false);
                }
                else
                {
                    ChangeShieldColor(_shieldStrength);
                }
                if (_movementPattern != 6)
                {
                    return;
                }
            }

            _speed = 0;
            _canFire = false;
            if (_boss)
            {
                StartCoroutine(TripleExplosion());
            }
            else
            {
                _anim.SetTrigger("OnEnemyDeath");
            }


            OnEnemyDeath();

        }
        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);

            if (_isShieldActive)
            {
                _shieldStrength -= 1;
                if (_shieldStrength < 1)
                {
                    _isShieldActive = false;
                    _enemyShield.SetActive(false);
                }
                else
                {
                    ChangeShieldColor(_shieldStrength);
                }
                return;
            }

            _speed = 0;
            _canFire = false;

            if (_boss)
            {
                StartCoroutine(TripleExplosion());
            }
            else
            {
                _anim.SetTrigger("OnEnemyDeath");
            }
            //add score 10
            if (_player != null)
            {
                _player.AddScore(10);
            }
            OnEnemyDeath();
        }
    }

    IEnumerator TripleExplosion()
    {
        timer += Time.deltaTime;

        while (timer < 1f)
        {
            GameObject explosion3 = Instantiate(_explosionPrefab, transform.position + new Vector3(Random.Range(-1.50f, 1.50f), Random.Range(-1.50f, 1.50f), 0), Quaternion.identity);
            yield return new WaitForSeconds(0.24f);
        }
    }

    private void OnEnemyDeath()
    {
        _audioSource.Play();
        Destroy(GetComponent<Collider2D>());
        Destroy(_enemyShield.gameObject);
        _spawnManager.OnEnemyDeath();
        float delay = 2.4f;
        if (_boss) { delay = 1f; }
        Destroy(this.gameObject, delay);
    }

    private void OnBecameInvisible()
    {
        _canFire = false;
    }
    private void OnBecameVisible()
    {
        _canFire = true;
    }

}
