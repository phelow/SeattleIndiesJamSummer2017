using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
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
            Vector3 spawnPoint = transform.position + new Vector3(6.0f,0,0);
            Collider2D collision = Physics2D.OverlapCircle(spawnPoint, p_enemy.GetComponentInChildren<CircleCollider2D>().radius);
            if(collision == null)
            {
                Instantiate(p_enemy, transform.position,transform.rotation,null);
            }

            yield return new WaitForSeconds(10.0f);
        }
    }
}
