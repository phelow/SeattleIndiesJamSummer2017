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
    private float _maximumMovementForce = 5000.0f;

    [SerializeField]
    private Transform _model;

    [SerializeField]
    private GameObject p_fervorGoo;

    [SerializeField]
    private GameObject p_harvestGoo;

    private float _fervor = 500;

    private float c_maxFervor = 1000;

    [SerializeField]
    private LayerMask _ignoreBuildings;

    [SerializeField]
    private GameObject[] p_harvestExplosions;

    private bool _charging = false;

    public static SpaceShipMovement s_instance;

    float roll = 0;
    float targetRoll = 0;

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
            _fervor = Mathf.Clamp(_fervor + 1.0f, 0.0f, c_maxFervor);

            yield return new WaitForSeconds(1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float prevAngle = transform.rotation.eulerAngles.z;
        float angle = 180 + Mathf.Atan2(_rigidbody.velocity.y, _rigidbody.velocity.x) * Mathf.Rad2Deg;
        Quaternion quat = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        transform.rotation = Quaternion.Lerp(transform.rotation, quat, Time.deltaTime);

        float angleDiff = Mathf.DeltaAngle(angle, prevAngle);
        targetRoll = Mathf.Clamp(angleDiff, -80, 80);

        roll = Mathf.Lerp(roll, targetRoll, Time.deltaTime);
        _model.localRotation = Quaternion.AngleAxis(roll, new Vector3(1, 0, 0));

        bool leftPressed = Input.GetMouseButton(0);
        bool rightPressed = Input.GetMouseButton(1);

        Vector3 mousePosition = Input.mousePosition;
        Vector3 relativePosition = mousePosition - new Vector3(Screen.width / 2, Screen.height / 2);
        Vector3 movement = new Vector3(relativePosition.x / 100, relativePosition.y / 100);

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f)
        {
            movement = new Vector3(Input.GetAxis("Horizontal") * 100.0f, Input.GetAxis("Vertical") * 100.0f, 0.0f);
        }

        //if outside the circle move

        _charging = false;
        _rigidbody.AddForce(movement.normalized * _movementForce * Time.deltaTime);

        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, _conversionRadius, Vector2.up, 1.0f, _ignoreBuildings);
        Debug.DrawLine(transform.position, movement, Color.red);
        if (hit.Length == 0)
        {
            return;
        }


        foreach (RaycastHit2D h in hit)
        {
            FervorBucket bucket = h.collider.transform.GetComponent<FervorBucket>();

            if (bucket == null)
            {
                continue;
            }

            if (!bucket.IsConverted())
            {
                _charging = true;
                //if inside the circle convert
                bucket.Convert();

                bucket.gameObject.GetComponent<SpringyShackle>().StartShackling(this.gameObject);

                WinCondition.s_instance.NewPersonChained();
            }
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
