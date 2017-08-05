﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipMovement : MonoBehaviour
{
    [SerializeField]
    private float _conversionRadius;

    [SerializeField]
    private Rigidbody2D _rigidbody;

    [SerializeField]
    private float _movementForce = 100.0f;

    [SerializeField]
    private GameObject p_fervorGoo;

    [SerializeField]
    private GameObject p_harvestGoo;

    private float _fervor = 500;

    private float c_maxFervor = 1000;

    [SerializeField]
    private LayerMask _ignoreBuildings;

    [SerializeField]
    private GameObject [] p_harvestExplosions;

    private bool _charging = false;

    public static SpaceShipMovement s_instance;

    // Use this for initialization
    void Start()
    {
        s_instance = this;
        p_harvestExplosions = Resources.LoadAll<GameObject>("Explosions");
        StartCoroutine(AddFervor());
    }

    public IEnumerator AddFervor()
    {
        while (true)
        {
            _fervor = Mathf.Clamp(_fervor + 1.0f,0.0f,c_maxFervor);

            yield return new WaitForSeconds(1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool leftPressed = Input.GetMouseButton(0);
        bool rightPressed = Input.GetMouseButton(1);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(transform.position, worldPos);

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f)
        {
            worldPos = new Vector3(Input.GetAxis("Horizontal") * 100.0f, Input.GetAxis("Vertical") * 100.0f, 0.0f);
        }

        Vector2 movement = (new Vector2(worldPos.x, worldPos.y) - new Vector2(transform.position.x, transform.position.y));


        //if outside the circle move

        if (/*(leftPressed || rightPressed) && */_conversionRadius < distance)
        {
            _charging = false;
            _rigidbody.AddForce(movement.normalized * _movementForce * (distance / 2) * Time.deltaTime);
             
            return;
        }


        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement, _conversionRadius, _ignoreBuildings);
        Debug.DrawLine(transform.position, movement, Color.red);
        if (hit == null || hit.transform == null || hit.transform.GetComponent<Movement>() == null)
        {
            return;
        }

        Line2D.Line2DRenderer renderer = null;

        if(!(leftPressed || rightPressed))
        {
            _charging = false;
            return;
        }


        FervorBucket bucket = hit.collider.transform.GetComponent<FervorBucket>();

        if (leftPressed && (!bucket.IsConverted()))
        {
            _charging = true;
            //if inside the circle convert
            bucket.Convert();

            bucket.gameObject.GetComponent<SpringyShackle>().StartShackling(this.gameObject);
            
        }
    }

    private IEnumerator DelayedDestory(GameObject particles)
    {
        yield return new WaitForSeconds(1.0f);
        particles.GetComponent<ParticleSystem>().Stop();
        yield return new WaitForSeconds(1.0f);
        Destroy(particles);
    }
}
