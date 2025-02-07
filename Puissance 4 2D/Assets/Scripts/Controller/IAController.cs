using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAController : EntityController
{
    [SerializeField] private GameManager.GameTurn myType;

    private void Update()
    {
        PlaceJeton(myType);
    }

    protected override void PlaceJeton(GameManager.GameTurn turn)
    {
        int newX=0;
        int newY=0;
        while (true)
        {
            if (GameManager.instance.currentPlayer != myType) return;
            if (turn == GameManager.GameTurn.Wait)return;
            MapManager.TileState[,] copyArray = new MapManager.TileState[MapManager.instance.w,MapManager.instance.h];
            System.Array.Copy(MapManager.instance.mapArray, copyArray,MapManager.instance.mapArray.Length );
            float maxValue = 0;
            for (var i = 0; i < MapManager.instance.w; i++)
            {
                var y = CheckForCollone(new Vector2(i, 3));
                if (EvaluationForActualPlayer(i,y,copyArray) > maxValue)
                {
                    maxValue = EvaluationForActualPlayer(i, y,copyArray);
                    newY = y;
                    newX = i;
                }
            }
            if (newY <= MapManager.instance.h)
            {
                break;
            }
        }
       
        var spriteRenderer = MapManager.instance.mapArrayInGame[newX,newY].gameObject
            .GetComponent<SpriteRenderer>();
        SwitchForColor(turn, spriteRenderer, newX,newY);
        MapManager.instance.CheckForWinOrNull(newX,newY,turn);
        GameManager.instance.UpdateTurnText();
    }

   /* private float EvaluationToCheckForEnnemy()
    {
        
    }*/
    
    
    private float EvaluationForActualPlayer(int x, int y,MapManager.TileState[,]copyArray)
    {
        float cpt = CheckForVN(x, y, copyArray) + CheckForHN(x,y,copyArray) + 
                    CheckForDRN(x,y,copyArray) + CheckForDLN(x,y,copyArray); //condition line
        if (MapManager.instance.CheckForWinOrNull(x,y,myType))
        {
            cpt += 1000;
        }
        return cpt;
    }

    private float CheckForVN(int x,int y,MapManager.TileState[,]copyArray)
    {
        var cpt = 0;
        MapManager.TileState tileStateCurrently = copyArray[x, y];
        for (var i = 1; i < 4; i++)
        {
            var newY = x - i;
            if (newY < 0 || copyArray[x,newY] != tileStateCurrently)break;
            cpt += 1;
        }
        return cpt;
    }

    private float CheckForHN(int x,int y,MapManager.TileState[,]copyArray)
    {
        var cpt = 0;
        var tileStateCurrently = copyArray[x, y];
        for (int i = 1; i < 4; i++)
        {
            int newX = x + i;
            if (newX >= MapManager.instance.w || copyArray[newX,y] != tileStateCurrently)break;
            cpt++;
        }
        for (int i = 1; i < 4; i++)
        {
            int newX = x - i;
            if (newX < 0 || copyArray[newX,y] != tileStateCurrently)break;
            cpt++;
        }
        return cpt;
    }

    private float CheckForDRN(int x,int y,MapManager.TileState[,]copyArray)
    {
        var tileStateCurrently = copyArray[x, y];
        int cpt = 0;
        for (int i = 1; i < 4; i++)
        {
            int newX = x - i;
            int newY = y + i;
            if (newX < 0 || newY >= MapManager.instance.h || copyArray[newX,newY] != tileStateCurrently )break;
            cpt++;
        }
        for (int i = 1; i < 4; i++)
        {
            int newX = x + i;
            int newY = y - i;
            if ( newX >= MapManager.instance.w || newY < 0 || copyArray[newX,newY] != tileStateCurrently )break;
            cpt++;
        }
        return cpt;
    }

    private float CheckForDLN(int x,int y,MapManager.TileState[,]copyArray)
    {
        var tileStateCurrently = copyArray[x, y];
        int cpt = 0;
        for (int i = 1; i < 4; i++)
        {
            int newX = x + i;
            int newY = y + i;
            if (newX >= MapManager.instance.w || newY >= MapManager.instance.h || copyArray[newX,newY] != tileStateCurrently )break;
            cpt++;
        }
        for (int i = 1; i < 4; i++)
        {
            int newX = x - i;
            int newY = y - i;
            if ( newX < 0 || newY < 0 || copyArray[newX,newY] != tileStateCurrently)break;
            cpt++;
        }

        return cpt;
    }
}
