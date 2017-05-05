﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPadEnemy : MonoBehaviour
{
    /// <summary>
    /// The AudioVisualization object refrence for frequency data
    /// </summary>
    [SerializeField]
    private AudioVisualization m_AV;

    [SerializeField]
    private GameObject m_EnemyPrefab;

    [SerializeField]
    private int m_InstantSpawnBand;

    [SerializeField]
    private int m_SpawnRateBand;

    [SerializeField, Range(0, 1)]
    private float m_SpawnThreshold;

    [SerializeField]
    private int m_HealthBand;
    [SerializeField]
    private int m_SpeedBand;
    [SerializeField]
    private int m_SpinBand;

    private Vector3[] m_SubSpawnPads;
    private int m_LastSubSpawn;

    [SerializeField]
    private float m_MaxHealth;

    private float m_Health, m_Speed, m_Spin;

    public List<Transform> m_EnemyPath = new List<Transform>();


    private void Start ()
    {
        enabled =
            (m_AV != null) &&
            (m_EnemyPrefab != null) &&
            (m_InstantSpawnBand < m_AV.frequencyBands) &&
            (m_SpawnRateBand < m_AV.frequencyBands);

        m_SubSpawnPads = new Vector3[9];

        for(int i = 0; i < 9; ++i)
        {
            m_SubSpawnPads[i] = new Vector3((i % 3f), (i / 3), (0));
            m_SubSpawnPads[i] += transform.position;
        }
        
        for (int i = 0; i < m_EnemyPath.Count - 1; ++i)
        {
            m_EnemyPath[i].LookAt(m_EnemyPath[i + 1]);
        }

        m_MaxHealth = m_MaxHealth < 1 ? 1 : m_MaxHealth;

        StartCoroutine(_ConstantSpawn());
	}

    private IEnumerator _ConstantSpawn()
    {
        yield return new WaitUntil(() => m_AV.m_CurrentFrequencyStereo[m_SpawnRateBand] >= 0.1);

        while(enabled)
        {
            SpawnEnemy(m_Health, 2, m_Spin);
            yield return new WaitForSeconds((2.0f - (m_AV.m_CurrentFrequencyStereo[m_SpawnRateBand])));
        }
    }

    private void Update ()
    {
        SetVariables();
        InstantSpawn();

	}

    private void InstantSpawn()
    {
        if ((m_AV.m_DeltaFrequencyStereo[m_InstantSpawnBand]) >= m_SpawnThreshold)
        {
            SpawnEnemy(m_Health + 1, m_Speed * 1.5f, m_Spin / 2);
            print("Spawn");
        }
    }

    private void SetVariables()
    {
        m_Health = 1 + ((m_MaxHealth - 1) * m_AV.m_CurrentFrequencyStereo[m_HealthBand]);
        m_Speed = 1 + (m_AV.m_CurrentFrequencyStereo[m_SpeedBand]);
        m_Spin = m_AV.m_DeltaFrequencyStereo[m_SpinBand] * 10;
    }

    private void SpawnEnemy(float health, float speed, float spin)
    {
        int r = Random.Range(0, 8);
        GameObject drone = Instantiate(m_EnemyPrefab, m_SubSpawnPads[r], transform.rotation) as GameObject;
        drone.GetComponent<Enemy>().SetInitValues(health, speed, spin, m_EnemyPath);
        drone.transform.localScale *= 0.5f + (health / m_MaxHealth);
    }   
}