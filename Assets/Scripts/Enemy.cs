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

    // // // // // // // // // // // // // // // // // // // // 
    // canFire = true. Can fire, damage the player
    // superPowerType = 0 - regular, 1 - Laser beam, 2 - heatseeking.
    // behavior = 0 - linear,
    // 1 - horizontal zig-zag side to side
    // 2 - circle
    // 3 - angle
    // 4 - vertical zig-zag
    // 5 - side to side
    // 
    public void setEnemyType(bool canFire, int superPowerType, int behavior)
    {
        
       if (canFire == false) _canFireFunction = canFire;

       if (superPowerType > 0) _superPowerType = superPowerType;

       if (behavior > 0) _movementPattern = behavior;

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

        _movementPattern = Random.Range(0, 6);
        _movementLeftRight = Random.Range(0, 2);

        _loverLimit = Random.Range(0.0f, 4.6f);


    //DEBUG
    setEnemyType(true, 2, 5);
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
            default:
                Debug.Log("The Movement pattern is out of scope: " + _movementPattern.ToString());
                break;
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
        if (_canFireFunction && _canFire && Time.time > _canFireTimer)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFireTimer = Time.time + _fireRate;

            ////
            //  0 - regular laser
            //  1 - double laser
            //  2 - laser beam
            if (_superPowerType == 1)
            {
                GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
                Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
                foreach (Laser laser in lasers)
                {
                    laser.AssignEnemyLaser();
                }

            }
            else if (_superPowerType == 2)
            {
                StartCoroutine(FireLaserBeam());
            }
            else
            {
                GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
                Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
                foreach (Laser laser in lasers)
                {
                    laser.AssignEnemyLaser();
                }
                // will convert to 1 laser soon..
            }


        }
    }

    IEnumerator FireLaserBeam()
    {
        for (byte i = 0; i < 6; i++)
        {
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
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
            _speed = 0;
            _canFire = false;
            _anim.SetTrigger("OnEnemyDeath");

            if (_player != null)
            {
                _player.Damage();
            }
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.4f);

        }
        if (other.CompareTag("Laser"))
        {
            _speed = 0;
            _canFire = false;
            _anim.SetTrigger("OnEnemyDeath");
            Destroy(other.gameObject);
            //add score 10
            if (_player != null)
            {
                _player.AddScore(10);
            }
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.4f);
        }
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
