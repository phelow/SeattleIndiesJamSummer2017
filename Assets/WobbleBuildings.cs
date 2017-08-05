using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleBuildings : MonoBehaviour {
    [SerializeField]
    private float _wobble = 1.0f;

    [SerializeField]
    private Rigidbody2D _rigidBody;

    Quaternion _originalRotation;

	// Use this for initialization
	void Start () {
        StartCoroutine(WobbleRoutine());
	}

    private IEnumerator WobbleRoutine()
    {
        _originalRotation = transform.rotation;
        while (true)
        {
            _rigidBody.AddTorque(Random.Range(-1.0f, 1.0f));

            transform.rotation = Quaternion.Lerp(transform.rotation, _originalRotation, Time.deltaTime);
            yield return new WaitForSeconds(Mathf.InverseLerp(0, 100.0f, _wobble));


        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
