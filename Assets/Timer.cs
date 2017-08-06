using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    [SerializeField]
    private Text _text;

	// Use this for initialization
	void Start () {
        StartCoroutine(CountUp());
	}

    private IEnumerator CountUp()
    {
        for(int i = 0; true; i++)
        {
            _text.text = i/60 + ":"+("" + i%60).PadLeft(2,'0');
            PlayerPrefs.SetInt("CurrentTime", i);
            PlayerPrefs.SetString("CurrentTimeString",_text.text);

            yield return new WaitForSeconds(1.0f);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
