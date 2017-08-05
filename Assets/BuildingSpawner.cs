using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    private static int enemyCountInScene = 0;
    private static int maxEnemyCount = 200;

    [SerializeField]
    private GameObject p_enemy;

    public void Start()
    {
        StartCoroutine(SpawnUnit());
    }

    IEnumerator SpawnUnit()
    {
        while(true)
        {
            if (enemyCountInScene >= maxEnemyCount) {
                yield return new WaitForSeconds(10.0f);
            }

            Vector3 spawnPoint = transform.position + new Vector3(6.0f,0,0);
            Collider2D collision = Physics2D.OverlapCircle(spawnPoint, p_enemy.GetComponentInChildren<CircleCollider2D>().radius);
            if (collision == null)
            {
                GameObject enemy = Instantiate(p_enemy, transform.position,transform.rotation,null);
                IncrementEnemyCount();
                enemy.GetComponent<DestroyTracker>().OnGameObjectDestroyed.AddListener(DecrementEnemyCount);
            }

            yield return new WaitForSeconds(10.0f);
        }
    }

    public void IncrementEnemyCount() {
        enemyCountInScene++;
    }

    public void DecrementEnemyCount() {
        enemyCountInScene--;
    }
}
