using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
 
    private bool _stopSpawning = false;
    [SerializeField]
    private bool _WaveSpawn = true;
    private int _enemyAlive = 0;
    private int _enemyWaveCount = 1;


    public void StartSpawning()
    {
        if (_WaveSpawn)
        {
            StartCoroutine(SpawnWaveEnemyRoutine());
        }
        else
        {
            StartCoroutine(SpawnEnemyRoutine());
        }
        StartCoroutine(SpawnPowerupRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnWaveEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        

        while (_stopSpawning == false)
        {
            _enemyAlive = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (_enemyAlive == 0)
            {
                _enemyAlive = _enemyWaveCount;
            
                for (int amount = 0; amount < _enemyWaveCount; amount++)
                {
                    Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
                    GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
                    newEnemy.transform.parent = _enemyContainer.transform;
                }

                _enemyWaveCount++;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(5.0f);

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomPowerup = Random.Range(0, 7);

            bool spawn = true;
            if(randomPowerup == 5 && Random.Range(0, 2) == 0) //second fire powerup. spawn rarely
            {
                spawn = false;
            }
            if (spawn)
            {
                GameObject newPowerup = Instantiate(_powerups[randomPowerup], posToSpawn, Quaternion.identity);
            }

            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
