using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutOverTime : MonoBehaviour {
    [SerializeField]
    Line2D.Line2DRenderer line;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float dist = Vector3.Distance(line.points[0].pos, line.points[1].pos);

        if(dist < .1f)
        {
            Destroy(this.gameObject);
        }

        line.points[0].pos = Vector3.Lerp(line.points[0].pos, line.points[1].pos, Time.deltaTime);

    }
}
