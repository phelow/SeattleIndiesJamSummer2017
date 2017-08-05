using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DestroyTracker : MonoBehaviour
{
    public UnityEvent OnGameObjectDestroyed = new UnityEvent();
    void OnDestroy() {
        OnGameObjectDestroyed.Invoke();
    }
}
