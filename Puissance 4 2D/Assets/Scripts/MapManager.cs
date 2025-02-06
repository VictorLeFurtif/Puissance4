using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject firstTile;
    [SerializeField] private GameObject secondTile;
    private bool darkerTilePreviously = false;
    [FormerlySerializedAs("x")] public int w = 7;
    [FormerlySerializedAs("y")] public int h = 6;
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
        for (var i = 0; i < w; i++)
        {
            for (var j = 0; j < h; j++)
            {
                mapArray[i, j] = TileState.Empty;
            }
        }
        return mapArray;
    }

    private GameObject[,] GenerateMapGameObjects()
    {
        for (var i = 0; i < w; i++)
        {
            darkerTilePreviously = !darkerTilePreviously;
            for (var j = 0; j < h; j++)
            {
                var tileToSpawn = darkerTilePreviously ? firstTile : secondTile;
                darkerTilePreviously = !darkerTilePreviously;

                var tile = Instantiate(tileToSpawn, new Vector3(i, j, 0), Quaternion.identity);
                mapArrayInGame[i, j] = tile;
            }
        }
        return mapArrayInGame;
    }

    public void CheckForWin(int x, int y,GameManager.GameTurn turn)
    {
        if (CheckForV(x,y) || CheckForH(x,y))
        {
            Debug.Log(turn + " Win");
        }

        
    }

    private bool CheckForH(int x, int y)
    {
        var tileStateCurrently = mapArray[x, y];
        int cpt = 1;
        for (int i = 1; i < 4; i++)
        {
            int newX = x + i;
            if (newX >= w || mapArray[newX,y] != tileStateCurrently)break;
            cpt++;
        }
        for (int i = 1; i < 4; i++)
        {
            int newX = x - i;
            if (newX < 0 || mapArray[newX,y] != tileStateCurrently)break;
            cpt++;
        }
        return cpt >= 4;
    }

    
    private bool CheckForV(int x, int y)
    {
        TileState tileStateCurrently = mapArray[x, y];
        if (y < 3) return false;
        for (var i = y; i > y-3; i--)
        {
            if (i < 0 )return false;
            if (mapArray[x, y-i] != tileStateCurrently) return false;
        }
        return true;
    }
}
