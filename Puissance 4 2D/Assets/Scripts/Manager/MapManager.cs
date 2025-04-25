using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Manager
{
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
        [SerializeField] private int compteurBeforeNul = 0;

        private Color defaultColor;

        public Stack<TileState[,]> undoList = new Stack<TileState[,]>();
        public Stack<TileState[,]> redoList = new Stack<TileState[,]>();
    
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
            StackMap();
            defaultColor = mapArrayInGame[1, 1].gameObject.GetComponent<SpriteRenderer>().color;
            RefreshMap();
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

        public bool CheckForWin(int x, int y,GameManager.GameTurn turn,bool realCheck)
        {
            if (!CheckForV(x, y) && !CheckForH(x, y) && !CheckForDRight(x, y) && !CheckForDLeft(x, y)) return false;
            if (realCheck) // Check pour vérifier si c'est l'ia qui test ou pas.
            {
                Debug.Log(turn + " Win");
            }
            
            return true;
        }

        public void CheckForNul()
        {
            compteurBeforeNul++;
            if (compteurBeforeNul > 42)
            {
                Debug.Log("Match Nul");
            }
        }
    
        private bool CheckForDRight(int x, int y)
        {
            var tileStateCurrently = mapArray[x, y];
        
            if (tileStateCurrently == TileState.Empty) return false;
        
            int cpt = 1;
            for (int i = 1; i < 4; i++)
            {
                int newX = x + i;
                int newY = y + i;
                if (newX >= w || newY >= h || mapArray[newX,newY] != tileStateCurrently )break;
                cpt++;
            }
            for (int i = 1; i < 4; i++)
            {
                int newX = x - i;
                int newY = y - i;
                if ( newX < 0 || newY < 0 || mapArray[newX,newY] != tileStateCurrently)break;
                cpt++;
            }
        
            return cpt >= 4;
        }

        private bool CheckForDLeft(int x, int y)
        {
        
            var tileStateCurrently = mapArray[x, y];
        
            if (tileStateCurrently == TileState.Empty) return false;
        
            int cpt = 1;
            for (int i = 1; i < 4; i++)
            {
                int newX = x - i;
                int newY = y + i;
                if (newX < 0 || newY >= h || mapArray[newX,newY] != tileStateCurrently )break;
                cpt++;
            }
            for (int i = 1; i < 4; i++)
            {
                int newX = x + i;
                int newY = y - i;
                if ( newX >= w || newY < 0 || mapArray[newX,newY] != tileStateCurrently )break;
                cpt++;
            }
            return cpt >= 4;
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

    
        private bool CheckForV(int x, int y) {
            TileState tileStateCurrently = mapArray[x, y];
            if (tileStateCurrently == TileState.Empty) return false;

            int cpt = 1;
    
        
            for (int i = 1; i < 4; i++) {
                int newY = y - i;
                if (newY < 0 || mapArray[x, newY] != tileStateCurrently) break;
                cpt++;
            }
    
        
            for (int i = 1; i < 4; i++) {
                int newY = y + i;
                if (newY >= h || mapArray[x, newY] != tileStateCurrently) break;
                cpt++;
            }
    
            return cpt >= 4;
        }

    
        public void StackMap()
        {
            //call it after each Turn
            TileState[,] tileStatesToSecure = new TileState[7,6];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    tileStatesToSecure[i, j] = mapArray[i, j];
                }
            }
            undoList.Push(tileStatesToSecure);
        }

        public void RefreshMap()
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    switch (mapArray[i,j])
                    {
                        case TileState.Empty : mapArrayInGame[i,j].gameObject.GetComponent<SpriteRenderer>().color = defaultColor;
                            break;
                        case TileState.Red : mapArrayInGame[i,j].gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                            break;
                        case TileState.Yellow : mapArrayInGame[i,j].gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}
