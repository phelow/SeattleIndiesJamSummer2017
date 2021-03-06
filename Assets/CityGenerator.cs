﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    public int xMapSize = 18;
    public int yMapSize = 18;
    public float blockSize = 6;

    public List<BlockFootprint> blocks;

    private bool[,] isBlockOccupied;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        isBlockOccupied = new bool[yMapSize, xMapSize];

        for (int y = 1; y < yMapSize - 1; y++)
        {
            for (int x = 1; x < xMapSize - 1; x++)
            {
                if (!IsSurroundingAreaClear(y, x))
                {
                    continue;
                }

                blocks.Shuffle();

                foreach (BlockFootprint block in blocks)
                {
                    if (!CanPlaceBlockAt(block, y, x))
                    {
                        continue;
                    }

                    PlaceBlockAt(block, y, x);
                    break;
                }
            }
        }
    }

    void PlaceBlockAt(BlockFootprint block, int y, int x)
    {
        int xRandom = (int)Random.Range(-blockSize/2, blockSize/2);
        int yRandom = (int)Random.Range(-blockSize/2, blockSize/2);

        Vector3 position = new Vector3(x * blockSize, y * blockSize, 0);

        position.x = Mathf.Clamp(position.x + xRandom, 0.0f, xMapSize*blockSize - block.footprintRows[0].Length*blockSize);
        position.y = Mathf.Clamp(position.y + yRandom, 0.0f, yMapSize*blockSize - block.footprintRows[0].Length*blockSize);

        Vector3 globalPosition = transform.TransformPoint(position) + new Vector3(xRandom, yRandom, 0);
        Instantiate<BlockFootprint>(block, globalPosition, Quaternion.identity, null);

        for (int yFootprint = 0; yFootprint < block.footprintRows.Count; yFootprint++)
        {
            for (int xFootprint = 0; xFootprint < block.footprintRows[yFootprint].Length; xFootprint++)
            {
                char isOccupiedChar = block.footprintRows[yFootprint][xFootprint];
                bool isOccupied = isOccupiedChar != '0';
                if (isOccupied)
                {
                    isBlockOccupied[y + yFootprint, x + xFootprint] = true;
                }
            }
        }
    }

    bool CanPlaceBlockAt(BlockFootprint block, int y, int x)
    {
        for (int yFootprint = 0; yFootprint < block.footprintRows.Count; yFootprint++)
        {
            for (int xFootprint = 0; xFootprint < block.footprintRows[yFootprint].Length; xFootprint++)
            {
                char isOccupiedChar = block.footprintRows[yFootprint][xFootprint];
                bool isOccupied = isOccupiedChar != '0';
                if (isOccupied && !IsSurroundingAreaClear(y + yFootprint, x + xFootprint))
                {
                    return false;
                }
            }
        }

        return true;
    }

    bool IsSurroundingAreaClear(int yCoord, int xCoord)
    {
        if (yCoord < 0 || yCoord >= yMapSize || xCoord < 0 || xCoord >= xMapSize)
        {
            return false;
        }

        for (int y = yCoord - 1; y <= yCoord + 1; y++)
        {
            for (int x = xCoord - 1; x <= xCoord + 1; x++)
            {
                if (y < 0 || y >= yMapSize || x < 0 || x >= xMapSize)
                {
                    continue;
                }

                if (isBlockOccupied[y, x])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
