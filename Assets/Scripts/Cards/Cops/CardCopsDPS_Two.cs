using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCopsDPS_Two : Card
{

    void Start()
    {
        name = "Cops DPS card two";
        cardDescription = "If Cops DPS has 50 or less HP, permanently increase movement by 2";
        type = Type.Cops;
    }


    public override void Effect()
    {
        if (team == Team.White)
        {
            if (board.whiteUnits[0].copsType == CopsUnitType.CopsDPS)
            {

                if (board.whiteUnits[0].health <= 50)
                {
                    board.whiteUnits[0].moves += 2;
                }
                Debug.Log("CopsDPS card two played. Increase moves by 2");
            }
        }

        if (team == Team.Black)
        {
            if (board.blackUnits[0].copsType == CopsUnitType.CopsDPS)
            {

                if (board.blackUnits[0].health <= 50)
                {
                    board.blackUnits[0].moves += 2;
                }
                Debug.Log("CopsDPS card two played. Increase moves by 2");
            }
        }
    }
}
