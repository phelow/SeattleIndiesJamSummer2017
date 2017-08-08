using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class GoToNext : MonoBehaviour {
    [SerializeField]
    List<Sprite> _sprites;

    [SerializeField]
    UnityEngine.UI.Image _image;

    [SerializeField]
    UnityEngine.UI.Image _introSpaceField;

    void Start()
    {
        if (_introSpaceField != null)
        {
            _introSpaceField.enabled = false;
        }
    }

    int i = 0;
    // Update is called once per frame
	void Update () {
	    if(Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if(i < _sprites.Count-1)
            {

                i++;
                _image.sprite = _sprites[i];
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
            }

            if (_introSpaceField != null && i > 2)
            {
                _introSpaceField.enabled = false;
            }
            else
            {
                _introSpaceField.enabled = true;
            }
        }
	}
}
