using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCopsSupport_Two : Card
{
    void Start()
    {
        name = "Cops Support card two";
        cardDescription = "Destroy first and second card from opponent's deck";
        type = Type.Cops;
    }


    public override void Effect()
    {
        if (team == Team.White)
        {
            if (board.whiteUnits[3].copsType == CopsUnitType.CopsSupport)
            {
                Destroy(gameManager.blackTeamDeck.Draw(1)[0]);
                Destroy(gameManager.blackTeamDeck.Draw(1)[0]);
                Debug.Log("Cops Support card two effect activated (destroyed two cards from opponent deck)");
            }
        }

        if (team == Team.Black)
        {
            if (board.blackUnits[3].copsType == CopsUnitType.CopsSupport)
            {
                Destroy(gameManager.whiteTeamDeck.Draw(1)[0]);
                Destroy(gameManager.whiteTeamDeck.Draw(1)[0]);
                Debug.Log("Cops Support card two effect activated (destroyed two cards from opponent deck)");
            }
        }
    }
}
