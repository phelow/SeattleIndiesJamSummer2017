using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOnDestroy : MonoBehaviour {
    public GameObject[] go;

    void OnDestroy()
    {
        for (int i = 0; i < go.Length; i++)
        {
            Instantiate(go[i], transform.position, transform.rotation, null);
        }
    }
}
