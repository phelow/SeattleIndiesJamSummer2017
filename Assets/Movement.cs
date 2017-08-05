﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed;

    private Vector3 targetPosition;

    private float _maxTime = 10.0f;
    private float _timeLeft = 10.0f;

    [SerializeField]
    private ProgressBarPro _timeBar;

    private FervorBucket bucket;

    [SerializeField]
    private Rigidbody2D rb;
    private Vector2 direction;

    [SerializeField]
    private LayerMask _buildingLayer;

    [SerializeField]
    private Animator _animator;

    private GameObject _attached;

    void Start()
    {
        bucket = this.GetComponent<FervorBucket>();
        StartCoroutine(TickDown());
        StartCoroutine(MoveCharacter());
    }

    public void SetAttached(GameObject go)
    {
        _attached = go;
    }

    private IEnumerator TickDown()
    {
        _timeBar.SetValue(0.0f, _maxTime);
        while (true)
        {
            if(_attached == null && bucket.IsConverted())
            {
                _timeLeft--;
                if(_timeLeft == 0)
                {
                    Destroy(this.gameObject);
                }
                _timeBar.SetValue(_timeLeft, _maxTime);
            }

            yield return new WaitForSeconds(1.0f);
        }
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
        _animator.SetFloat("Speed", rb.velocity.magnitude);
        float angle = 180 + Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        Quaternion quat = Quaternion.AngleAxis(angle, new Vector3(0,0,1));
        transform.rotation = quat;
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
