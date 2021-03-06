﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    private static int enemyCountInScene = 0;
    private static int maxEnemyCount = 250;

    private float _fervor = 0.0f;
    private float c_maxFervor = 100.0f;

    [SerializeField]
    private ProgressBarPro _progressBar;

    [SerializeField]
    private GameObject p_enemy;

    public bool isConverted;

    public void Convert(bool toConvert)
    {
        isConverted = toConvert;
    }

    public void Start()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(180, 0, 0), 1.0f);
        StartCoroutine(SpawnUnit());
    }

    public void Update()
    {
        if (!isConverted)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(180,0,0),Time.deltaTime);
        }
        else
        {

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0,0), Time.deltaTime);
        }
    }

    IEnumerator SpawnUnit()
    {
        yield return new WaitForSeconds(5.0f);

        while (true)
        {
            if (enemyCountInScene >= maxEnemyCount)
            {
                yield return new WaitForSeconds(10.0f);
                continue;
            }

            Vector3 spawnPoint = transform.position + new Vector3(6.0f, 0, 0);
            Collider2D collision = Physics2D.OverlapCircle(spawnPoint, p_enemy.GetComponentInChildren<CircleCollider2D>().radius);
            if (collision == null)
            {
                GameObject enemy = Instantiate(p_enemy, transform.position, p_enemy.transform.rotation, null);
                IncrementEnemyCount();
                enemy.GetComponent<DestroyTracker>().OnGameObjectDestroyed.AddListener(DecrementEnemyCount);
            }

            yield return new WaitForSeconds(10.0f);
        }
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        FervorBucket bucket = coll.gameObject.GetComponent<FervorBucket>();

        if(bucket == null)
        {
            return;
        }

        if (bucket.IsConverted())
        {
            _fervor++;
        }
        else
        {
            _fervor--;
        }

        _fervor = Mathf.Clamp(_fervor, 0.0f, c_maxFervor);

        if (_progressBar != null)
        {
            _progressBar.SetValue(_fervor, c_maxFervor);
        }
    }


    public void IncrementEnemyCount()
    {
        enemyCountInScene++;
    }

    public void DecrementEnemyCount()
    {
        enemyCountInScene--;
    }
}
