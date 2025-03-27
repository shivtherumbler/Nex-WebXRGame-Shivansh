using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Wave
{
    public string WaveName;
    public int NoOfEnemies;
    public GameObject[] TypeOfEnemies;
    public float SpawnInterval;
}

public class WaveSpawner : MonoBehaviour
{
    public Wave[] Waves;
    public Transform[] SpawnPoints;
    public TextMeshProUGUI waveNo;
    public TextMeshProUGUI healthleft;
    public TextMeshProUGUI enemyKilled;
    public PlayerHealth player;

    private Wave currentWave;
    private int currentWaveNumber;
    private float nextSpawnTime;
    public int Points;

    private bool canSpawn = true;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        currentWave = Waves[currentWaveNumber];
        SpawnWave();
        GameObject[] totalEnemies = GameObject.FindGameObjectsWithTag("AI");
        if (totalEnemies.Length == 0 && !canSpawn)
        {
            if (currentWaveNumber + 1 != Waves.Length)
            {
                currentWaveNumber++;
                canSpawn = true;

            }
        }
        waveNo.text = (currentWaveNumber+1).ToString();
        healthleft.text = (player.currentHealth + "%");
        enemyKilled.text = player.totalkills.ToString();
    }

    void SpawnWave()
    {
        if (canSpawn && nextSpawnTime < Time.time)
        {
            GameObject randomEnemy = currentWave.TypeOfEnemies[Random.Range(0, currentWave.TypeOfEnemies.Length)];
            Transform randomPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length)];
            Instantiate(randomEnemy, randomPoint.position, Quaternion.identity);
            currentWave.NoOfEnemies--;
            nextSpawnTime = Time.time + currentWave.SpawnInterval;
            if (currentWave.NoOfEnemies == 0)
            {
                canSpawn = false;
            }

        }
    }
}
