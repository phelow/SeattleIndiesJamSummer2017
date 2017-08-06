using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinTimer : MonoBehaviour {
    [SerializeField]
    private Text _text;
	// Use this for initialization
	void Start () {
        
        _text.text = "Your Time:" + PlayerPrefs.GetString("CurrentTimeString","1:10") + " Best Time:" + PlayerPrefs.GetString("LowTimeString","9:55");

        if (PlayerPrefs.GetInt("CurrentTime") < PlayerPrefs.GetInt("LowTime",9999))
        {
            PlayerPrefs.SetString("LowTimeString", PlayerPrefs.GetString("CurrentTimeString"));
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
