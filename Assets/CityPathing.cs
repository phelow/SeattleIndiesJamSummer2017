using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityPathing : MonoBehaviour
{
    public int xBlockSize = 5;
    public int yBlockSize = 5;
    public int xIntersectionCount = 10;
    public int yIntersectionCount = 10;

    [SerializeField]
    private LayerMask _ignoreCharacters;

    public Vector2 GetSize()
    {
        return new Vector2(xBlockSize * xIntersectionCount, yBlockSize * yIntersectionCount);
    }

}
