using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAController : EntityController
{
    [SerializeField] private GameManager.GameTurn myType;
    [SerializeField] private int profondeurMax;

    private void Update()
    {
        PlaceJeton(myType);
    }

    private float MiniMax(MapManager.TileState[,] board, int profondeur, bool iaTurn)
    {
        
        if (profondeur == profondeurMax) //TODO: check for end game
        {
            return EvalBoard(board, iaTurn);  
        }

        float bestScore = iaTurn ? int.MinValue : int.MaxValue;
        int bestColumn = -1;

        for (int i = 0; i < MapManager.instance.w; i++)
        {
            if (IsColumnFull(i)) continue;

            int y = CheckForCollone(new Vector2(i, 3));
            var boardCopy = DeepCopyArray(board);
            
            boardCopy[i, y] = iaTurn ? MapManager.TileState.Yellow : MapManager.TileState.Red;

            float eval = MiniMax(boardCopy, profondeur + 1, !iaTurn);

            
            if ((iaTurn && eval > bestScore) || (!iaTurn && eval < bestScore))
            {
                bestScore = eval;
                bestColumn = i;
            }
        }

        return profondeur == 0 ? bestColumn : bestScore; 
    }


    private float EvalBoard(MapManager.TileState[,] board, bool iaTurn)
    {
        float totalScore = 0;

        for (int x = 0; x < MapManager.instance.w; x++)
        {
            for (int y = 0; y < MapManager.instance.h; y++)
            {
                switch (board[x, y])
                {
                    case MapManager.TileState.Yellow:
                        totalScore += EvaluationForActualPlayer(x, y, board);
                        break;
                    case MapManager.TileState.Red:
                        totalScore -= EvaluationForActualPlayer(x, y, board);
                        break;
                }
            }
        }
        return iaTurn ? totalScore : -totalScore; 
    }

    protected override void PlaceJeton(GameManager.GameTurn turn)
    {
        if (GameManager.instance.currentPlayer != myType || turn == GameManager.GameTurn.Wait) return;

        MapManager.instance.StackMap();

        // Trouver le meilleur coup avec MiniMax
        int bestMove = (int)MiniMax(MapManager.instance.mapArray, 0, true);

        if (bestMove == -1) return;
        int newY = CheckForCollone(new Vector2(bestMove, 3));
        PlaceJetonAt(bestMove, newY, turn);
    }

    
    /*
    protected override void PlaceJeton(GameManager.GameTurn turn)
    {
        if (GameManager.instance.currentPlayer != myType || turn == GameManager.GameTurn.Wait) return;
        int newX=0;
        int newY=0;
        MapManager.instance.StackMap(); 
        while (true)
        {
            
            MapManager.TileState[,] copyArray = DeepCopyArray(MapManager.instance.mapArray);
            float maxValue = 0;
            
            for (var i = 0; i < MapManager.instance.w; i++)
            {
                if (IsColumnFull(i))
                {
                    continue;
                }
                
                var y = CheckForCollone(new Vector2(i, 3));
              
                
                var tokenStocking = MapManager.instance.mapArray[i, y]; 
                MapManager.instance.mapArray[i,y] = MapManager.TileState.Yellow;
                if (MapManager.instance.CheckForWin(i,y,GameManager.GameTurn.Ia,true)) 
                {
                    newY = y;
                    newX = i;
                    MapManager.instance.mapArray[i,y] = tokenStocking; 
                    break;
                }
                MapManager.instance.mapArray[i,y] = tokenStocking; 
              
              
                if (EvaluationForActualPlayer(i,y,copyArray) > maxValue)
                {
                    // evaluation réaliser avant l'assignation
                    maxValue = EvaluationForActualPlayer(i, y,copyArray);
                    newY = y;
                    newX = i;
                }
                
                
                
                tokenStocking = MapManager.instance.mapArray[i, y]; 
                MapManager.instance.mapArray[i,y] = MapManager.TileState.Red;
                if (MapManager.instance.CheckForWin(i,y,GameManager.GameTurn.You,false)) // Le problem il est la je pense
                {
                    newX = i;
                    newY = y;
                    MapManager.instance.mapArray[i,y] = tokenStocking; 
                    break;
                }
                MapManager.instance.mapArray[i,y] = tokenStocking;  
                
            }
            if (newY <= MapManager.instance.h)
            {
                break;
            }
        }
        PlaceJetonAt(newX,newY,turn);
    }*/

    private void PlaceJetonAt(int x, int y,GameManager.GameTurn turn)
    {
        var spriteRenderer = MapManager.instance.mapArrayInGame[x,y].gameObject
            .GetComponent<SpriteRenderer>();
        SwitchForColor(turn, spriteRenderer, x,y);
        
        MapManager.instance.CheckForWin(x,y,turn,true);
        MapManager.instance.CheckForNul();
        GameManager.instance.UpdateTurnText();
    }
    
    
   private MapManager.TileState[,] DeepCopyArray(MapManager.TileState[,] original)
   {
       int width = MapManager.instance.w;
       int height = MapManager.instance.h;
       MapManager.TileState[,] newArray = new MapManager.TileState[width, height];

       for (int x = 0; x < width; x++)
       {
           for (int y = 0; y < height; y++)
           {
               newArray[x, y] = original[x, y];
           }
       }
       return newArray;
   }

   private bool IsColumnFull(int x)
   {
       return MapManager.instance.mapArray[x, MapManager.instance.h - 1] != MapManager.TileState.Empty;
   }

    private float EvaluationForActualPlayer(int x, int y,MapManager.TileState[,]copyArray)
    {
        float cpt = CheckForVN(x, y, copyArray) + CheckForHN(x,y,copyArray) + 
                    CheckForDRN(x,y,copyArray) + CheckForDLN(x,y,copyArray); //condition line
        
        //for ia check fatal win coup
        var tokenStocking = MapManager.instance.mapArray[x, y]; 
        MapManager.instance.mapArray[x,y] = MapManager.TileState.Yellow;
        if (MapManager.instance.CheckForWin(x,y,GameManager.GameTurn.Ia,true))
        {
            cpt += 1000;
        }
        MapManager.instance.mapArray[x,y] = tokenStocking; 
        
        //Check to block player
        tokenStocking = MapManager.instance.mapArray[x, y]; 
        MapManager.instance.mapArray[x,y] = MapManager.TileState.Red;
        if (MapManager.instance.CheckForWin(x,y,GameManager.GameTurn.You,false)) 
        {
            cpt += 1000;
        }
        MapManager.instance.mapArray[x,y] = tokenStocking;
        
        return cpt;
    }
    
    private float CheckForVN(int x, int y, MapManager.TileState[,] copyArray)
    {
        var cpt = 0;
        var tileStateCurrently = copyArray[x, y];

        for (var i = 1; i < 4; i++) 
        {
            var newY = y - i; 
            if (newY < 0 || copyArray[x, newY] != tileStateCurrently) break;
            cpt++;
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
