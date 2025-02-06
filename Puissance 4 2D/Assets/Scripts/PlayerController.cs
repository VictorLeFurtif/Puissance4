using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{
    
    void Update()
    {
        PlaceJeton(GameManager.GameTurn.Player);
    }

    protected override void PlaceJeton(GameManager.GameTurn turn)
    {
        if (GameManager.instance.currentPlayer == GameManager.GameTurn.Player)
        {
            base.PlaceJeton(turn);
            GameManager.instance.UpdateTurnText();
        }
    }
}
