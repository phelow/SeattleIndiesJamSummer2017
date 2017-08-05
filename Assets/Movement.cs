using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public CityPathing pathing;
    public float moveSpeed;

    private Vector3 targetPosition;
    [SerializeField]
    private Rigidbody2D rb;
    private Vector2 direction;

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
        int randomDirection = Random.Range(0, 4);
        if (randomDirection == 0)
        {
            return new Vector2(1, 0);
        }
        else if (randomDirection == 1)
        {
            return new Vector2(0, -1);
        }
        else if (randomDirection == 2)
        {
            return new Vector2(0, 1);
        }
        else {
            return new Vector2(-1, 0);
        }
    }
}
