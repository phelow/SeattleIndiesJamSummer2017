using UnityEngine;
using System.Collections;

class MinimapUpdater: MonoBehaviour
{
    private Camera _camera;

    [SerializeField]
    private float _updateTime;

    [SerializeField]
    private RenderTexture _texture;

    void Start()
    {
        _camera = GetComponent<Camera>();

        StartCoroutine(ffffffffff());
    }

    IEnumerator ffffffffff()
    {
        while (true)
        {
            _camera.targetTexture = _texture;

            yield return new WaitForEndOfFrame();

            _camera.targetTexture = null;

            yield return new WaitForSeconds(_updateTime);
        }
    }
}
