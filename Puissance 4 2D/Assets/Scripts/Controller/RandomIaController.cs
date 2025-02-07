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
        int newX = 0;
        int newY = 0;
        while (true)
        {
            if (GameManager.instance.currentPlayer != myType) return;
            if (turn == GameManager.GameTurn.Wait)return;
            newX = Random.Range(0, MapManager.instance.w);
            newY = Random.Range(0, MapManager.instance.h);
            Vector2 newVector = new Vector2(newX, newY);
            newY = CheckForCollone(newVector);
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

    
}
