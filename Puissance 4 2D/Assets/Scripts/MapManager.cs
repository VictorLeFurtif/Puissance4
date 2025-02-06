using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject firstTile;
    [SerializeField] private GameObject secondTile;
    private bool darkerTilePreviously = false;
    public int x = 7;
    public int y = 6;
    public TileState[,] mapArray = new TileState[7,6];
    public GameObject[,] mapArrayInGame = new GameObject[7, 6];
    public static MapManager instance;
    [SerializeField] private GameObject placeHolder;
    [SerializeField] private float scalePlaceHolderX;
    [SerializeField] private float scalePlaceHolderY;
    [SerializeField] private float placeHolderPositionX;
    [SerializeField] private float placeHolderPositionY;

    public enum TileState
    {
        Yellow,
        Red,
        Empty
    }
    
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
    }

    private void Start()
    {
        GenerateMapEnum();
        SetGrill();
        GenerateMapGameObjects();
    }
    
    private void SetGrill()
    {
        placeHolder.transform.position = new Vector3(placeHolderPositionX, placeHolderPositionY,-1);
        placeHolder.transform.localScale = new Vector3(scalePlaceHolderX,scalePlaceHolderY,1);
    }
    
    private TileState[,] GenerateMapEnum()
    {
        for (var i = 0; i < x; i++)
        {
            for (var j = 0; j < y; j++)
            {
                mapArray[i, j] = TileState.Empty;
            }
        }
        return mapArray;
    }

    private GameObject[,] GenerateMapGameObjects()
    {
        for (var i = 0; i < x; i++)
        {
            darkerTilePreviously = !darkerTilePreviously;
            for (var j = 0; j < y; j++)
            {
                var tileToSpawn = darkerTilePreviously ? firstTile : secondTile;
                darkerTilePreviously = !darkerTilePreviously;

                var tile = Instantiate(tileToSpawn, new Vector3(i, j, 0), Quaternion.identity);
                mapArrayInGame[i, j] = tile;
            }
        }
        return mapArrayInGame;
    }
}
