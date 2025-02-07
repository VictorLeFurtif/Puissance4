using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class RandomIaController : EntityController
{
    
    [SerializeField] private GameManager.GameTurn myType;
    
    private void Update()
    {
        PlaceJeton(myType);
    }

    protected override void PlaceJeton(GameManager.GameTurn turn)
    {
        if (GameManager.instance.currentPlayer != myType) return;
        if (turn == GameManager.GameTurn.Wait)return;
        var newX = Random.Range(0, MapManager.instance.w); // faudra creer un sécu pour pas qu'il soit en dehors à cause des y
        var newY = Random.Range(0, MapManager.instance.h);
        Vector2 newVector = new Vector2(newX, newY);
        newY = CheckForCollone(newVector);
        var spriteRenderer = MapManager.instance.mapArrayInGame[newX,newY].gameObject
            .GetComponent<SpriteRenderer>();
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
        
        MapManager.instance.CheckForWinOrNull(newX,newY,turn);
        GameManager.instance.UpdateTurnText();
        
    }

    
}
