using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    public CityPathing pathing;
    public float moveSpeed;

    private Vector3 targetPosition;
    private Rigidbody rb;
    private Vector2 direction;

	void Start () {
        if (pathing == null) {
            pathing = Object.FindObjectOfType<CityPathing>();
        }
        
        rb = GetComponent<Rigidbody>();

        int random = (int)Random.Range(0, 2);
        direction = new Vector2(random, random == 1 ? 0 : 1);
        targetPosition = pathing.GetDestination(transform.position, direction);
        Debug.Log(direction);
    }

    void Update () {
        Vector3 rel = targetPosition - transform.position;
        if (rel.magnitude < 0.1f) {
            targetPosition = pathing.GetDestination(transform.position, direction);
            rel = targetPosition - transform.position;
            direction = new Vector2(rel.x, rel.y);
            Debug.Log(direction);
        }

        rb.velocity = rel.normalized * moveSpeed;
    }
}
