using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed;

    private Vector3 targetPosition;

    private float _maxTime = 15.0f;
    private float _timeLeft = 15.0f;

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

    [SerializeField]
    private List<GameObject> _explosions;

    void Start()
    {
        bucket = this.GetComponent<FervorBucket>();
        StartCoroutine(TickDown());
        StartCoroutine(MoveCharacter());

        /*
        _explosions = new GameObject[2];
        _explosions[0] = Resources.Load<GameObject>("Explosions/PL Electric Bolt 02");
        _explosions[1] = Resources.Load<GameObject>("Explosions/PL Dark 10");
        */
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
                    StartCoroutine(ExplosionDestroy());
                    //Destroy(this.gameObject);
                }
                _timeBar.SetValue(_timeLeft, _maxTime);
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

    void OnDestroy()
    {
        WinCondition.s_instance.PersonDied();
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

    private IEnumerator ExplosionDestroy()
    {
        Vector3 position = this.gameObject.transform.position;
        Quaternion rotation = this.gameObject.transform.rotation;

        GameObject explosion1 = GameObject.Instantiate(_explosions[0], position, rotation, this.transform);

        yield return new WaitForSeconds(1.0f);

        position = this.gameObject.transform.position;
        rotation = this.gameObject.transform.rotation;

        //explosion1.GetComponent<ParticleSystem>().Stop();
        GameObject explosion2 = GameObject.Instantiate(_explosions[1], position, rotation, null);


        Destroy(this.gameObject);

        //this.gameObject.SetActive(false);
        //explosion2.transform.parent = this.transform;

    }
}
