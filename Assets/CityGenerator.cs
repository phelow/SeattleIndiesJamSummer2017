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

}
