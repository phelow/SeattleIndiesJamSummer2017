using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    private CityPathing cityPathing;
    public Transform transformPrefab;

    void Awake()
    {
        cityPathing = GetComponent<CityPathing>();
    }

    void Start()
    {
        //DEBUG
        for (int i = 0; i < 50; i++)
        {
            Vector3 position = new Vector3(Random.Range(0, cityPathing.GetSize().x), 0, Random.Range(0, cityPathing.GetSize().y));
            Transform person = Instantiate<Transform>(transformPrefab, this.transform, false);
            person.localPosition = position;
            person.GetComponent<Movement>().pathing = cityPathing;
        }
    }
}
