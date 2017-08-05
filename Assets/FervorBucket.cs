using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FervorBucket : MonoBehaviour {
    private float _fervor = 20.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float Consume()
    {
        return _fervor - 50.0f;
    }
}
