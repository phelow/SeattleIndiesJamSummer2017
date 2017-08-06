using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class IntroController : MonoBehaviour {
    [SerializeField]
    UnityEngine.UI.Image _image;

    [SerializeField]
    GoToNext _goToNext;

    void Start() {
        _goToNext.enabled = false;
    }

    // Update is called once per frame
	void Update () {
	    if(Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            _image.gameObject.SetActive(false);
            _goToNext.enabled = true;
        }
	}
}
