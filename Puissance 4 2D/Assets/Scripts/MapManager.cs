using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject firstTile;
    [SerializeField] private GameObject secondTile;
    public int x;
    public int y;
    private GameObject[,] mapArray;
    private bool darkerTilePreviously = false;
    public static MapManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GenerateMap();
    }

    private void GenerateMap()
    {
        mapArray = new GameObject[x,y];

        for (var i = 0; i < x; i++)
        {
            darkerTilePreviously = !darkerTilePreviously;
            for (var j = 0; j < y; j++)
            {
                var tileToSpawn = darkerTilePreviously ? firstTile : secondTile;
                darkerTilePreviously = !darkerTilePreviously;

                var tile = Instantiate(tileToSpawn, new Vector3(i, j, 0), Quaternion.identity);
                mapArray[i, j] = tile;
            }
        }
    }
}
