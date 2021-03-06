﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyBossPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;

    private bool _stopSpawning = false;
    [SerializeField]
    private bool _WaveSpawn = true;
    private int _enemyWaveCount = 1;

    private int _currentEnemiesCount = 0;
    private int _currentWavePowerupToSpawn = 0;
    private int _currentWavePowerupSpawned = 0;

    private float _percentEnemies = 50f;
    
    public void SpawnEnemy()
    {
        _currentEnemiesCount++;
        Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
        Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity, _enemyContainer.transform);
    }

    public void SpawnEnemyBoss()
    {
        _currentEnemiesCount++;
        Vector3 posToSpawn = new Vector3(0, 7, 0);
        GameObject newBoss = Instantiate(_enemyBossPrefab, posToSpawn, Quaternion.identity, _enemyContainer.transform);
    }

    public void OnEnemyDeath()
    {
        _currentEnemiesCount--;
    }

    public void OnPowerupDissolve()
    {

    }

    public void StartSpawning()
    {
        BalancedSpawningEnemiesVersusPowerups(50);

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

    private void BalancedSpawningEnemiesVersusPowerups(int percentEnemies)
    {
        if (percentEnemies > 100 || percentEnemies < 1)
        {
            Debug.Log("Incorrect percentage for Balance Spawning system");
            return;
        }

        _percentEnemies = percentEnemies;
    
    }

    private void CalculatePowerupBalance(int enemiesSpawned)
    {
        //get 1 percent of enemies.   enemiesSpawned = 3     == 70%
        if (enemiesSpawned == 0)
        {
            Debug.Log("Incorrect spawned amount of enemies");
            return;
        }
        //100 = _percentEnemies + percentPowerups
        //100 =  70 + 30
        //?   =  3  +  ?
        // 70 / 3 = 23.3   - percent of one enemy
        // 100/23.3 = 4.29  == 100%
        // 4.29 - 3 = 1.29 Round up to 2
        float oneEnemyPercent = _percentEnemies / enemiesSpawned;
        _currentWavePowerupToSpawn = Mathf.CeilToInt(Mathf.Ceil(100f / oneEnemyPercent) - enemiesSpawned);
    }

    private bool isPrime(int number)
    {
        if (number == 1) return false;
        if (number == 2) return true;

        var limit = Mathf.Ceil(Mathf.Sqrt(number)); //hoisting the loop limit

        for (int i = 2; i <= limit; ++i)
            if (number % i == 0)
                return false;
        return true;

    }

    IEnumerator SpawnWaveEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        

        while (_stopSpawning == false)
        {
            int _enemyAlive = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (_enemyAlive == 0)
            {
                _enemyAlive = _enemyWaveCount;
                CalculatePowerupBalance(_enemyWaveCount);
                _currentWavePowerupSpawned = 0;

                if (_enemyWaveCount > 3 && isPrime(_enemyWaveCount))
                {

                    SpawnEnemyBoss();

                }
                else
                {
                    for (int amount = 0; amount < _enemyWaveCount; amount++)
                    {
                        SpawnEnemy();
                    }
                    
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
            SpawnEnemy();
            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(5.0f);

        while (_stopSpawning == false)
        {
            int randomPowerup = Random.Range(0, 7);

            bool spawn = true;

            if (_WaveSpawn && _currentWavePowerupSpawned >= _currentWavePowerupToSpawn) randomPowerup = 3; // spawn = false;

            if (randomPowerup == 5 && Random.Range(0, 2) == 0) randomPowerup = 3; //second fire powerup. spawn rarely, ammo instead

            if (randomPowerup == 4 && Random.Range(0, 2) == 0) randomPowerup = 3; //ammo frequent. health rarely

            if (spawn)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
                Instantiate(_powerups[randomPowerup], posToSpawn, Quaternion.identity);
                _currentWavePowerupSpawned++;
            }

            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
