using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipMovement : MonoBehaviour
{

    [SerializeField]
    private CircleCollider2D _conversionRadius;

    [SerializeField]
    private Rigidbody2D _rigidbody;

    [SerializeField]
    private float _movementForce = 100.0f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool leftPressed = Input.GetMouseButton(0);
        bool rightPressed = Input.GetMouseButton(1);
                //if outside the circle move

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f)
        {
            worldPos = new Vector3(Input.GetAxis("Horizontal") * 100.0f, Input.GetAxis("Vertical") * 100.0f, 0.0f);
        }

        Vector2 movement = (new Vector2(worldPos.x, worldPos.y) - new Vector2(transform.position.x, transform.position.y));

        if ((leftPressed || rightPressed) && _conversionRadius.radius < Vector2.Distance(transform.position,worldPos))
        {
            _rigidbody.AddForce(movement.normalized * _movementForce * Time.deltaTime);

            return;
        }

        if (leftPressed)
        {
            //if inside the circle convert
        }
        else if (rightPressed)
        {
            //if inside the circle harvest
        }
    }
}
