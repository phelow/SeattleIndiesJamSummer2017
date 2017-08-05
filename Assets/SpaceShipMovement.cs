using System.Collections;
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

    // Use this for initialization
    void Start()
    {
        p_harvestExplosions = Resources.LoadAll<GameObject>("Explosions");
        _progressBar.SetValue(_fervor, c_maxFervor);
    }

    // Update is called once per frame
    void Update()
    {
        bool leftPressed = Input.GetMouseButton(0);
        bool rightPressed = Input.GetMouseButton(1);
        //if outside the circle move

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(transform.position, worldPos);

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f)
        {
            worldPos = new Vector3(Input.GetAxis("Horizontal") * 100.0f, Input.GetAxis("Vertical") * 100.0f, 0.0f);
        }

        Vector2 movement = (new Vector2(worldPos.x, worldPos.y) - new Vector2(transform.position.x, transform.position.y));

        if ((leftPressed || rightPressed) && _conversionRadius < distance)
        {
            _rigidbody.AddForce(movement.normalized * _movementForce * Time.deltaTime);

            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, movement, _conversionRadius, _ignoreBuildings);
        Debug.DrawLine(transform.position, movement, Color.red);
        if (hit == null || hit.transform == null || hit.transform.GetComponent<Movement>() == null)
        {
            return;
        }

        Line2D.Line2DRenderer renderer = null;

        if (leftPressed)
        {
            //if inside the circle convert
            renderer = GameObject.Instantiate(p_fervorGoo, hit.collider.transform.position, transform.rotation, null).GetComponent<Line2D.Line2DRenderer>();

            renderer.points = new List<Line2D.Line2DPoint>();
            renderer.points.Add(new Line2D.Line2DPoint(transform.position, .1f, Color.red));
            renderer.points.Add(new Line2D.Line2DPoint(hit.collider.transform.position, .1f, Color.red));
            renderer.GetComponent<FadeOutOverTime>().Setup(hit.collider.transform);
            _progressBar.SetValue(_fervor, c_maxFervor);
        }
        else if (rightPressed)
        {
            //if inside the circle harvest
            renderer = GameObject.Instantiate(p_fervorGoo, hit.collider.transform.position, transform.rotation, null).GetComponent<Line2D.Line2DRenderer>();
            renderer.points = new List<Line2D.Line2DPoint>();
            renderer.points.Add(new Line2D.Line2DPoint(hit.collider.transform.position, .1f, Color.blue));
            renderer.points.Add(new Line2D.Line2DPoint(transform.position, .1f, Color.blue));
            renderer.GetComponent<FadeOutOverTime>().Setup(transform);

            FervorBucket bucket = hit.collider.transform.GetComponent<FervorBucket>();

            if (_fervor < bucket.CostToConsume())
            {
                return;
            }
            _fervor -= bucket.Consume();
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
