using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringyShackle : MonoBehaviour {
    [SerializeField]
    private Line2D.Line2DRenderer line;

	// Use this for initialization

    public void Update()
    {
        if(line.points == null || line.points.Count < 2)
        {
            line.points = new List<Line2D.Line2DPoint>();
            line.points.Add(new Line2D.Line2DPoint(new Vector3(99999.0f, 99999.0f), .1f, Color.black));
        }
    }

    public void StartShackling(GameObject target)
    {
        StartCoroutine(LineRoutine(target));
    }

    // Update is called once per frame
    private IEnumerator LineRoutine (GameObject target) {
        line.points = new List<Line2D.Line2DPoint>();
        line.points.Add(new Line2D.Line2DPoint());
        line.points.Add(new Line2D.Line2DPoint());

        SpringJoint2D spring = target.gameObject.AddComponent<SpringJoint2D>();
        spring.connectedBody = this.GetComponent<Rigidbody2D>();
        spring.anchor = transform.position - target.transform.position;
        spring.autoConfigureDistance = false;
        while (true)
        {
            if(target == null || line == null)
            {
                Destroy(this.gameObject);
            }

            line.points[0].pos = target.transform.position;
            line.points[0].width = .1f;
            line.points[1].pos = transform.position;
            line.points[1].width = .1f;
            yield return new WaitForFixedUpdate();
        }
    }
}
