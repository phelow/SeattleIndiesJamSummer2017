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
    private ProgressBarPro _progressBar;

    [SerializeField]
    private LayerMask _ignoreBuildings;

    [SerializeField]
    private GameObject [] p_harvestExplosions;

    private bool _charging = false;


    // Use this for initialization
    void Start()
    {
        p_harvestExplosions = Resources.LoadAll<GameObject>("Explosions");
        _progressBar.SetValue(_fervor, c_maxFervor);
        StartCoroutine(AddFervor());
    }

    public IEnumerator AddFervor()
    {
        while (true)
        {
            _fervor = Mathf.Clamp(_fervor + 1.0f,0.0f,c_maxFervor);
            _progressBar.SetValue(_fervor, c_maxFervor);

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

        if (leftPressed && (!bucket.IsConverted() || _charging))
        {
            _charging = true;
            //if inside the circle convert
            renderer = GameObject.Instantiate(p_fervorGoo, hit.collider.transform.position, transform.rotation, null).GetComponent<Line2D.Line2DRenderer>();

            renderer.points = new List<Line2D.Line2DPoint>();
            renderer.points.Add(new Line2D.Line2DPoint(transform.position, .1f, Color.red));
            renderer.points.Add(new Line2D.Line2DPoint(hit.collider.transform.position, .1f, Color.red));
            renderer.GetComponent<FadeOutOverTime>().Setup(hit.collider.transform);
            _progressBar.SetValue(_fervor, c_maxFervor);
            if (_fervor < 10.0f)
            {
                _progressBar.SetValue(0.0f, c_maxFervor);
                return;
            }
            _fervor -= hit.collider.GetComponent<FervorBucket>().GainFervor();
            _progressBar.SetValue(_fervor, c_maxFervor);

        }
        else if (rightPressed && bucket.IsConverted())
        {
            _charging = false;
            //if inside the circle harvest
            renderer = GameObject.Instantiate(p_fervorGoo, hit.collider.transform.position, transform.rotation, null).GetComponent<Line2D.Line2DRenderer>();
            renderer.points = new List<Line2D.Line2DPoint>();
            renderer.points.Add(new Line2D.Line2DPoint(hit.collider.transform.position, .1f, Color.blue));
            renderer.points.Add(new Line2D.Line2DPoint(transform.position, .1f, Color.blue));
            renderer.GetComponent<FadeOutOverTime>().Setup(transform);
            

            _fervor += bucket.Consume();
            StartCoroutine(DelayedDestory(GameObject.Instantiate(p_harvestExplosions[Random.Range(0,p_harvestExplosions.Length)], (hit.collider.transform.position + Camera.main.transform.position) /2, hit.collider.transform.rotation, null)));
            Destroy(hit.collider.gameObject);
            _progressBar.SetValue(_fervor, c_maxFervor);
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
