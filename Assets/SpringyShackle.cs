﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringyShackle : MonoBehaviour {
    private Line2D.Line2DRenderer line;

    [SerializeField]
    private Line2D.Line2DRenderer p_line;

	// Use this for initialization
    public void StartShackling(GameObject target)
    {
        StartCoroutine(LineRoutine(target));
    }

    // Update is called once per frame
    private IEnumerator LineRoutine (GameObject target) {
        line = GameObject.Instantiate(p_line,this.transform);

        line.points = new List<Line2D.Line2DPoint>();
        line.points.Add(new Line2D.Line2DPoint());
        line.points.Add(new Line2D.Line2DPoint());

        SpringJoint2D spring = target.gameObject.AddComponent<SpringJoint2D>();
        spring.connectedBody = this.GetComponent<Rigidbody2D>();
        spring.anchor = transform.position - target.transform.position;
        spring.autoConfigureDistance = false;
        spring.distance = 2.0f;
        spring.frequency = 1.0f;
        while (true)
        {
            if(target == null || target.transform == null || target.transform.position == null || line == null)
            {
                Destroy(this.gameObject);
            }
            try
            {
                line.points[0].pos = target.transform.position;
                line.points[0].width = .1f;
                line.points[1].pos = transform.position;
                line.points[1].width = .1f;
            }
            catch
            {

            }
            yield return new WaitForFixedUpdate();
        }
    }
}
