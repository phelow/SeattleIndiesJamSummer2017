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

    public Vector3 GetDestination(Vector3 currentGlobalPosition, Vector2 currentDirection)
    {
        currentDirection.Normalize();
        currentDirection.x = Mathf.Round(currentDirection.x);
        currentDirection.y = Mathf.Round(currentDirection.y);

        Vector3 localPosition = transform.InverseTransformPoint(currentGlobalPosition);
        int xIntersection = (int)Mathf.Round(localPosition.x / xBlockSize);
        int yIntersection = (int)Mathf.Round(localPosition.y / yBlockSize);

        List<Vector2> nextCandidates = new List<Vector2> {
            new Vector2(xIntersection + (int)currentDirection.y, yIntersection - (int)currentDirection.x), // Clockwise 90deg rotation
            new Vector2(xIntersection - (int)currentDirection.y, yIntersection + (int)currentDirection.x), // CCW 90deg rotation
            new Vector2(xIntersection + (int)currentDirection.x, yIntersection + (int)currentDirection.y), // Straight
        };

        List<Vector2> viableNextPositions = new List<Vector2>();

        for (int i = 0; i < nextCandidates.Count; i++) {
            Vector2 candidate = nextCandidates[i];
            int xCandidate = (int)candidate.x;
            int yCandidate = (int)candidate.y;
            if (xCandidate >= 0 && xCandidate <= xIntersectionCount &&
                yCandidate >= 0 && yCandidate <= yIntersectionCount)
            {
                viableNextPositions.Add(candidate);
            }
        }

        bool haveNextIntersection = false;
        Vector2 nextIntersection = new Vector2(0,0);

        while(!haveNextIntersection)
        {
            int index = (int)Random.Range(0, viableNextPositions.Count);
            Vector2 tempIntersection = viableNextPositions[index];

            Vector2 intersectionPosition = new Vector2(transform.position.x + tempIntersection.x * xBlockSize, transform.position.z + tempIntersection.y * yBlockSize);
            if(Physics2D.Raycast(currentGlobalPosition, intersectionPosition, 10))
            {
                // We hit something, try again
                continue;
            }

            nextIntersection = viableNextPositions[index];
            haveNextIntersection = true;
        }


        Vector3 nextPosition = new Vector3(nextIntersection.x * xBlockSize, currentGlobalPosition.y, nextIntersection.y * yBlockSize);
        nextPosition = transform.TransformPoint(nextPosition);
        return nextPosition;
    }
}
