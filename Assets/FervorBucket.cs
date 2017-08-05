using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FervorBucket : MonoBehaviour {
    private float _fervor = 20.0f;
    private float c_maxFervor = 100.0f;

    [SerializeField]
    private ProgressBarPro _healthBar;
	// Use this for initialization
	void Start () {
        _healthBar.SetValue(_fervor, c_maxFervor);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public float CostToConsume()
    {
        return 50.0f - _fervor;
    }

    public float Consume()
    {
        return CostToConsume();
    }

    public float GainFervor()
    {
        _fervor += 1.0f;
        _healthBar.SetValue(_fervor, c_maxFervor);
        return 1.0f;
    }
    

    public float LoseFervor()
    {
        _fervor -= 1.0f;
        _healthBar.SetValue(_fervor, c_maxFervor);
        return -1.0f;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        FervorBucket bucket = coll.gameObject.GetComponent<FervorBucket>();

        if(bucket == null)
        {
            return;
        }

        if(_fervor > 50)
        {
            bucket.LoseFervor();
        }
        else
        {
            bucket.GainFervor();
        }
    }
}
