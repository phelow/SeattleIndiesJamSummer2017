using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    private static int enemyCountInScene = 0;
    private static int maxEnemyCount = 200;

    private float _fervor = 0.0f;
    private float c_maxFervor = 100.0f;

    [SerializeField]
    private ProgressBarPro _progressBar;

    [SerializeField]
    private GameObject p_enemy;

    private bool _isConverted;

    public void Start()
    {
        StartCoroutine(SpawnUnit());
    }

    public void Update()
    {
        if (_isConverted)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,-180,0),Time.deltaTime);
        }
        else
        {

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0,0), Time.deltaTime);
        }
    }

    IEnumerator SpawnUnit()
    {
        while (true)
        {
            if (enemyCountInScene >= maxEnemyCount)
            {
                yield return new WaitForSeconds(10.0f);
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

        if (_fervor < 50)
        {
            _isConverted = false;
        }
        else
        {
            _isConverted = true;
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
