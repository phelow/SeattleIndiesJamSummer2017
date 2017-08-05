using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringyShackle : MonoBehaviour {
    [SerializeField]
    private Line2D.Line2DRenderer line;

	// Use this for initialization
	void Start () {
    }
	
    public void StartShackling()
    {
        StartCoroutine(LineRoutine());
    }

	// Update is called once per frame
	private IEnumerator LineRoutine () {
        line.points = new List<Line2D.Line2DPoint>();
        line.points.Add(new Line2D.Line2DPoint());
        line.points.Add(new Line2D.Line2DPoint());

        SpringJoint2D spring = SpaceShipMovement.s_instance.gameObject.AddComponent<SpringJoint2D>();
        spring.connectedBody = this.GetComponent<Rigidbody2D>();
        spring.anchor = transform.position - SpaceShipMovement.s_instance.transform.position; 
        while (true)
        {
            line.points[0].pos = SpaceShipMovement.s_instance.transform.position;
            line.points[1].pos = transform.position;
            yield return new WaitForFixedUpdate();
        }
    }
}
