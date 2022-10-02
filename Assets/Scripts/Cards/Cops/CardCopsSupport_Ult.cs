using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardCopsSupport_Ult : Card
{
    private int increaseHealthFor;

    // Start is called before the first frame update
    void Start()
    {
        cardName = "Cops Support ULT card";
        cardDescription = "Heal yourself and all allies for 20 hp. Allies can't increase their health in next turn.";
        type = Type.Cops;
        increaseHealthFor = 20;
    }

    public override void Effect()
    {
        // half implemented
        // half implemented
        if (team == Team.White)
        {
            foreach (Unit u in board.whiteUnits)
            {
                u.IncreaseHealth(increaseHealthFor);
            }
            Debug.Log("");
        }

        if (team == Team.Black)
        {
            foreach (Unit u in board.blackUnits)
            {
                u.IncreaseHealth(increaseHealthFor);
            }
            Debug.Log("");
            
        }
    }


}
