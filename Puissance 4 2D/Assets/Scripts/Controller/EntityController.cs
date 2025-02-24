using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    
    protected abstract void PlaceJeton(GameManager.GameTurn turn);

    protected void SwitchForColor(GameManager.GameTurn turn, SpriteRenderer spriteRenderer, int newX, int newY)
    {
        switch (turn)
        {
            case GameManager.GameTurn.IaOrSecondPLayer :
                spriteRenderer.color = Color.yellow;
                MapManager.instance.mapArray[newX,newY] = MapManager.TileState.Yellow;
                FinishTurn(GameManager.GameTurn.Wait);
                StartCoroutine(SwitchTurnAndWait(0.1f, GameManager.GameTurn.You));
                break;
            case GameManager.GameTurn.You :
                spriteRenderer.color = Color.red;
                MapManager.instance.mapArray[newX,newY] = MapManager.TileState.Red;
                FinishTurn(GameManager.GameTurn.Wait);
                StartCoroutine(SwitchTurnAndWait(0.1f, GameManager.GameTurn.IaOrSecondPLayer));
                break;
        }
    }

    private IEnumerator SwitchTurnAndWait(float time,GameManager.GameTurn turn)
    {
        yield return new WaitForSeconds(time);
        FinishTurn(turn);
    }
    
    protected virtual void FinishTurn(GameManager.GameTurn switchPlayer)
    {
        GameManager.instance.currentPlayer = switchPlayer;
    }

    protected int CheckForCollone(Vector2 objectPlace)
    {
        if (objectPlace.y == 0 && MapManager.instance.mapArray[(int)objectPlace.x,(int)objectPlace.y] 
            == MapManager.TileState.Empty)
        {
            return 0;
        }
        
        for (var i = 0; i < 6; i++)
        {
            if (MapManager.instance.mapArray[(int)objectPlace.x,i] == MapManager.TileState.Red || MapManager.instance.mapArray[(int)objectPlace.x,i] == MapManager.TileState.Yellow)
            {
                  continue;
            }
            return i;
        }

        return 0;
    }
    
}
