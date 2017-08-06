using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRandomForces : MonoBehaviour {
    [SerializeField]
    private Rigidbody2D _rigidbody;
	// Use this for initialization
	void Start () {
        StartCoroutine(MoveBackground());
    }
	
    private IEnumerator MoveBackground()
    {
        while (true)
        {
            _rigidbody.AddForce(new Vector2(Random.Range(-1000.0f, 1000.0f), Random.Range(-1000.0f, 1000.0f)));
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }
}
