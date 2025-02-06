using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
  

    private void Update()
    {
        placeJeton();
    }

    private void placeJeton()
    {
        if (GameManager.instance.currentPlayer != GameManager.GameTurn.Player) return;
        if (!Input.GetMouseButtonDown(0)) return;
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var cursorPosInt = new Vector2((int)cursorPos.x, (int)cursorPos.y);
        var y = CheckForCollone(cursorPosInt);
        var spriteRenderer = MapManager.instance.mapArrayInGame[(int)cursorPosInt.x,y].gameObject
            .GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        MapManager.instance.mapArray[(int)cursorPosInt.x, y] = MapManager.TileState.Red;

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
