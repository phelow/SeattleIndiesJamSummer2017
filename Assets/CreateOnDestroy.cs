using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateOnDestroy : MonoBehaviour {
    public GameObject[] go;
    private bool isQuitting;
    public Vector3 offset;

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
            Instantiate(go[i], transform.position + offset, transform.rotation, null);
        }
    }
}
