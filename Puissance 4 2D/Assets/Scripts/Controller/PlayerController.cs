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
        switch (turn)
        {
            case GameManager.GameTurn.IaOrSecondPLayer :
                spriteRenderer.color = Color.yellow;
                MapManager.instance.mapArray[(int)cursorPosInt.x, y] = MapManager.TileState.Yellow;
                FinishTurn(GameManager.GameTurn.Wait);
                StartCoroutine(SwitchTurnAndWait(0.1f, GameManager.GameTurn.You));
                break;
            case GameManager.GameTurn.You :
                spriteRenderer.color = Color.red;
                MapManager.instance.mapArray[(int)cursorPosInt.x, y] = MapManager.TileState.Red;
                FinishTurn(GameManager.GameTurn.Wait);
                StartCoroutine(SwitchTurnAndWait(0.1f, GameManager.GameTurn.IaOrSecondPLayer));
                break;
        }
        
        MapManager.instance.CheckForWinOrNull((int)cursorPosInt.x, y,turn);
        GameManager.instance.UpdateTurnText();
    }
    
}
