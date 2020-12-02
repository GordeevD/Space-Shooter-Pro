using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;

    // ID for powerups
    // 0 = Triple Shot
    // 1 = Speed 
    // 2 = Shields
    // 3 = Ammo
    // 4 = Life
    // 5 = Projectile
    // 6 = Negative powerup
    //
    [SerializeField]
    private int _powerupID;
    [SerializeField]
    private AudioClip _audioClip;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -4.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            AudioSource.PlayClipAtPoint(_audioClip, transform.position);

            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActive();
                        break;
                    case 3:
                        player.AmmoReboot();
                        break;
                    case 4:
                        player.Heal();
                        break;
                    case 5:
                        player.SecondaryFire();
                        break;
                    case 6:
                        player.NegativePowerup();
                        break;
                    default:
                        Debug.Log("Default Value " + _powerupID);
                        break;
                }
            }

            Destroy(this.gameObject);
        }
    }
}
