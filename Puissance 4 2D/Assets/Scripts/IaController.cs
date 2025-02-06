using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IaController : EntityController
{
    
    void Update()
    {
        PlaceJeton(GameManager.GameTurn.Ia);
    }

    protected override void PlaceJeton(GameManager.GameTurn turn)
    {
        if (GameManager.instance.currentPlayer == GameManager.GameTurn.Ia)
        {
            base.PlaceJeton(turn);
            GameManager.instance.UpdateTurnText();
        }
        
    }
}
