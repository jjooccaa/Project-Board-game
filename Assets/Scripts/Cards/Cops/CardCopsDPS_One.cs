using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCopsDPS_One : Card
{
    private int increaseHealthFor;

    void Start()
    {
        cardName = "Cops DPS first card";
        cardDescription = "Heal yourself for 20 hp";
        type = Type.Cops;
        increaseHealthFor = 20;
    }

    public override void Effect()
    {
        if(team == Team.White)
        {
            if(board.whiteUnits[0].copsType == CopsUnitType.CopsDPS)
            {
                board.whiteUnits[0].IncreaseHealth(increaseHealthFor); 
                Debug.Log("CopsDPS card one played. Healed itself 20 hp");
            }
        }

        if (team == Team.Black)
        {
            if (board.blackUnits[0].copsType == CopsUnitType.CopsDPS)
            {
                board.blackUnits[0].IncreaseHealth(increaseHealthFor); 
                Debug.Log("CopsDPS card one played. Healed itself 20 hp");
            }
        }
    }
}
