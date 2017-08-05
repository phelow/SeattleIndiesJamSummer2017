using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed;

    private Vector3 targetPosition;

    [SerializeField]
    private Rigidbody2D rb;
    private Vector2 direction;

    [SerializeField]
    private LayerMask _buildingLayer;

    void Start()
    {

        StartCoroutine(MoveCharacter());
    }

    private IEnumerator MoveCharacter()
    {
        while (true)
        {
            rb.AddForce(GetNewDirection()*moveSpeed);
            yield return new WaitForSeconds(1.0f);
        }
    }

    void Update()
    {
    }

    public Vector3 GetNewDirection()
    {
        float angle = Random.Range(0, 2*Mathf.PI);
        Vector2 randomDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        RaycastHit2D hit = Physics2D.Raycast(transform.position, randomDirection, 3, _buildingLayer);
        if (hit.collider != null)
        {
            randomDirection = Vector2.Reflect(randomDirection, hit.normal);
        }

        return randomDirection;
    }
}
