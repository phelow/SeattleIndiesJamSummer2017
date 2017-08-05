using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutOverTime : MonoBehaviour {
    [SerializeField]
    Line2D.Line2DRenderer line;

    private const float c_speed = 10.0f;

    // Use this for initialization
	public void Setup (Transform lineZeroAnchor) {
        StartCoroutine(ShrinkRay(lineZeroAnchor));
	}

    private IEnumerator ShrinkRay(Transform anchor)
    {
        while (true)
        {
            if(anchor != null)
            {
                line.points[1].pos = anchor.position;
            }

            float dist = Vector3.Distance(line.points[0].pos, line.points[1].pos);

            if (dist < .1f)
            {
                Destroy(this.gameObject);
            }

            line.points[0].pos = Vector3.Lerp(line.points[0].pos, line.points[1].pos, Time.deltaTime * c_speed);
            yield return new WaitForEndOfFrame();
        }

    }
	
	// Update is called once per frame
	void Update () {

    }
}
