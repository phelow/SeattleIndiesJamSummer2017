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

        SetColor();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Convert()
    {
        _fervor = 100.0f;
    }

    public float CostToConsume()
    {
        return _fervor;
    }

    public float Consume()
    {
        return CostToConsume();
    }

    public bool IsConverted()
    {
        return _fervor > 50.0f;
    }

    public float GainFervor()
    {
        _fervor += .5f;
        StartCoroutine(FervorRoutine());
        return 1.0f;
    }

    private IEnumerator FervorRoutine()
    {
        _fervor += .5f;
        _healthBar.SetValue(_fervor, c_maxFervor);
        yield return new WaitForSeconds(1.0f);
        
        SetColor();
    }

    private void SetColor()
    {
        if (_fervor > 50.0f)
        {
            _healthBar.SetBarColor(Color.red);
        }
        else
        {
            _healthBar.SetBarColor(Color.blue);
        }
    }
    

    public float LoseFervor()
    {
        _fervor -= 1.0f;
        _healthBar.SetValue(_fervor, c_maxFervor);
        SetColor();
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
