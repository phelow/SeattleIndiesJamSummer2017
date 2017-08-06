using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FervorBucket : MonoBehaviour {
    private bool m_converted = false;
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void Convert()
    {
        m_converted = true;
    }
    
    public bool IsConverted()
    {
        return m_converted;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (m_converted)
        {
            SpringyShackle shackle = coll.gameObject.GetComponent<SpringyShackle>();
            if(shackle == null)
            {
                return;
            }

            if (shackle.gameObject.GetComponent<FervorBucket>().IsConverted())
            {
                return;
            }

            this.GetComponent<Movement>().SetAttached(this.gameObject);
            shackle.StartShackling(this.gameObject);
            coll.gameObject.GetComponent<FervorBucket>().Convert();

            WinCondition.s_instance.NewPersonChained();
        }
    }
}
