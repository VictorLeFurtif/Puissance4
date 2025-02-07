using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{

    [SerializeField] private GameManager.GameTurn myType;
    
    void Update()
    {
        PlaceJeton(myType);
    }

    protected override void PlaceJeton(GameManager.GameTurn turn)
    {
        if (GameManager.instance.currentPlayer != myType) return;
        if (turn == GameManager.GameTurn.Wait)return;
        if (!Input.GetMouseButtonDown(0)) return;
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var cursorPosInt = new Vector2(Mathf.Round(cursorPos.x),Mathf.Round(cursorPos.y));
        if (cursorPosInt.x > MapManager.instance.w || cursorPosInt.y > MapManager.instance.h || cursorPosInt.x < 0 || cursorPosInt.y < 0)return;
        var y = CheckForCollone(cursorPosInt);
        var spriteRenderer = MapManager.instance.mapArrayInGame[(int)cursorPosInt.x,y].gameObject
            .GetComponent<SpriteRenderer>();
        SwitchForColor(turn, spriteRenderer, (int)cursorPosInt.x,y);
        MapManager.instance.CheckForWinOrNull((int)cursorPosInt.x, y,turn);
        GameManager.instance.UpdateTurnText();
    }
    
}
