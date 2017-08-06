using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateOnDestroy : MonoBehaviour {
    public GameObject[] go;
    private bool isQuitting;

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        if (isQuitting)
        {
            return;
        }

        for (int i = 0; i < go.Length; i++)
        {
            Instantiate(go[i], transform.position, transform.rotation, null);
        }
    }
}
