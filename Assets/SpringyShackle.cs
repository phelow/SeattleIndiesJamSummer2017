using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringyShackle : MonoBehaviour {
    [SerializeField]
    private Line2D.Line2DRenderer line;

	// Use this for initialization
	void Start () {
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
        while (true)
        {
            line.points[0].pos = target.transform.position;
            line.points[1].pos = transform.position;
            yield return new WaitForFixedUpdate();
        }
    }
}
