using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    
    protected virtual void PlaceJeton(GameManager.GameTurn turn)
    {
        if (turn == GameManager.GameTurn.Wait)return;
        if (!Input.GetMouseButtonDown(0)) return;
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var cursorPosInt = new Vector2(Mathf.Round(cursorPos.x),Mathf.Round(cursorPos.y));
        if (cursorPosInt.x > MapManager.instance.w || cursorPosInt.y > MapManager.instance.h || cursorPosInt.x < 0 || cursorPosInt.y < 0)return;
        var y = CheckForCollone(cursorPosInt);
        var spriteRenderer = MapManager.instance.mapArrayInGame[(int)cursorPosInt.x,y].gameObject
            .GetComponent<SpriteRenderer>();
        switch (turn)
        {
            case GameManager.GameTurn.Ia :
                spriteRenderer.color = Color.yellow;
                MapManager.instance.mapArray[(int)cursorPosInt.x, y] = MapManager.TileState.Yellow;
                FinishTurn(GameManager.GameTurn.Wait);
                StartCoroutine(SwitchTurnAndWait(0.1f, GameManager.GameTurn.Player));
                break;
            case GameManager.GameTurn.Player :
                spriteRenderer.color = Color.red;
                MapManager.instance.mapArray[(int)cursorPosInt.x, y] = MapManager.TileState.Red;
                FinishTurn(GameManager.GameTurn.Wait);
                StartCoroutine(SwitchTurnAndWait(0.1f, GameManager.GameTurn.Ia));
                break;
        }
        
        MapManager.instance.CheckForWinOrNull((int)cursorPosInt.x, y,turn);
    }

    IEnumerator SwitchTurnAndWait(float time,GameManager.GameTurn turn)
    {
        yield return new WaitForSeconds(time);
        FinishTurn(turn);
    }
    
    protected virtual void FinishTurn(GameManager.GameTurn switchPlayer)
    {
        GameManager.instance.currentPlayer = switchPlayer;
    }
    
    private int CheckForCollone(Vector2 objectPlace)
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
