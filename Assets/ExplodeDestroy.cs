using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeDestroy : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ExplosionDestroy());
    }
    private IEnumerator ExplosionDestroy()
    {
        
        yield return new WaitForSeconds(1.0f);

        this.GetComponent<ParticleSystem>().Stop();

        //Destroy(explosion1);
        Destroy(this.gameObject);
    }

}