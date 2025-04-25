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

    private int GetBestMove()
    {
        float bestScore = float.NegativeInfinity;
        int bestColumn = -1;

        for (int col = 0; col < MapManager.instance.w; col++)
        {
            int row = GetLowestEmptyRow(col, MapManager.instance.mapArray);
            if (row == -1) continue;

            var boardCopy = DeepCopyArray(MapManager.instance.mapArray);
            boardCopy[col, row] = MapManager.TileState.Yellow;

            float score = MiniMax(boardCopy, 1, false); 

            if (score > bestScore)
            {
                bestScore = score;
                bestColumn = col;
            }
        }

        return bestColumn;
    }

    
    private float MiniMax(MapManager.TileState[,] board, int profondeur, bool iaTurn)
    {
        if (profondeur == profondeurMax)
        {
            return EvalBoard(board, iaTurn);
        }

        float bestValue = iaTurn ? float.NegativeInfinity : float.PositiveInfinity;

        for (int col = 0; col < MapManager.instance.w; col++)
        {
            int row = GetLowestEmptyRow(col, board);
            if (row == -1) continue;

            MapManager.TileState[,] boardCopy = board.Clone() as MapManager.TileState[,];
            MapManager.TileState currentPlayer = iaTurn ? MapManager.TileState.Yellow : MapManager.TileState.Red;
            boardCopy[col, row] = currentPlayer;

            // Vérifie si ce coup donne une victoire immédiate
            if (CheckWinSimulated(col, row, boardCopy, currentPlayer))
            {
                return iaTurn ? 10000 - profondeur : -10000 + profondeur;
            }

            float eval = MiniMax(boardCopy, profondeur + 1, !iaTurn);

            if (iaTurn)
            {
                bestValue = Mathf.Max(bestValue, eval);
            }
            else
            {
                bestValue = Mathf.Min(bestValue, eval);
            }
        }

        return bestValue;
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
        int bestMove = GetBestMove();
        if (bestMove == -1) return;

        int newY = GetLowestEmptyRow(bestMove, MapManager.instance.mapArray);
        if (newY == -1) return;

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
    
    private int GetLowestEmptyRow(int col, MapManager.TileState[,] board)
    {
        int width = board.GetLength(0);
        int height = board.GetLength(1);

        if (col < 0 || col >= width)
        {
            Debug.LogWarning($"Colonne invalide: {col} (width: {width})");
            return -1;
        }

        for (int row = 0; row < height; row++)
        {
            if (board[col, row] == MapManager.TileState.Empty)
                return row;
        }

        return -1;
    }

    
    private bool CheckWinSimulated(int x, int y, MapManager.TileState[,] board, MapManager.TileState player)
    {
        int[][] directions = new int[][] {
            new int[] { 1, 0 },  // horizontal
            new int[] { 0, 1 },  // vertical
            new int[] { 1, 1 },  // diagonal /
            new int[] { 1, -1 }  // diagonal \
        };

        foreach (var dir in directions)
        {
            int count = 1;

            for (int d = -1; d <= 1; d += 2)
            {
                int dx = dir[0] * d;
                int dy = dir[1] * d;
                int nx = x + dx;
                int ny = y + dy;

                while (nx >= 0 && ny >= 0 && nx < MapManager.instance.w && ny < MapManager.instance.h && board[nx, ny] == player)
                {
                    count++;
                    nx += dx;
                    ny += dy;
                }
            }

            if (count >= 4)
                return true;
        }

        return false;
    }

}
