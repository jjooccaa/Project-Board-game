using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCopsSupport_One : Card
{
    private bool isEffectActive;
    
    void Start()
    {
        name = "Cops Support card one";
        cardDescription = "Cops Support gets +5 heal next 3 turns";
        type = Type.Cops;
        isEffectActive = false;
    }
    protected override void Update()
    {
        base.Update();
        if (isEffectActive)
        {

        }
    }

    public override void Effect()
    {
        if (team == Team.White)
        {
            if (board.whiteUnits[3].copsType == CopsUnitType.CopsSupport)
            {
                int currentTurn = gameManager.numberOfWhiteTurns;
                isEffectActive = true;
                board.whiteUnits[3].heal += 5;
                Debug.Log("");
            }
        }

        if (team == Team.Black)
        {
            if (board.blackUnits[3].copsType == CopsUnitType.CopsSupport)
            {
                int currentTurn = gameManager.numberOfBlackTurns;
                isEffectActive = true;
                board.blackUnits[3].heal += 5; 
                Debug.Log("");
            }
        }
    }

}
