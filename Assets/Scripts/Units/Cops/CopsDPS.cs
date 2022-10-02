using System. Collections.Generic;
using UnityEngine;

public class CopsDPS : Unit
{
    private const int MOVES = 2;
    private const int RANGE = 5;
    private const int startingNumOfACTION = 1;
    private const int startingHEALTH = 100;
    private const int startingDAMAGE = 15;

    
    private void Awake()
    {
        copsType = CopsUnitType.CopsDPS;
        moves = MOVES;
        attackRange = RANGE;
        numberOfActions = startingNumOfACTION;

        health = startingHEALTH;
        healthBar.SetMaxHealth(health);

        damage = startingDAMAGE;
    }
    
    // if enemy units at the beginning of the turn have 15 or less hp, they are destroyed, but CopsDPS can't move or attack after that
    // implemented. It's called when player presses end turn button
    public override void Passive()
    {

        if (team == Team.White) {

            foreach (Unit u in board.blackUnits)
            {
                if(u.health <= 15)
                {
                    u.health = 0;
                    Debug.Log("Aurelia passive activated");
                    board.whiteUnits[0].GetComponent<Unit>().countingActions++;
                    board.whiteUnits[0].GetComponent<Unit>().countingMoves++;
                }

                
            }

        }

        if (team == Team.Black)
        {

            foreach (Unit u in board.whiteUnits)
            {
                if (u.health <= 15)
                {
                    u.health = 0;
                    Debug.Log("Aurelia passive activated");
                    board.blackUnits[0].GetComponent<Unit>().countingActions++;
                    board.blackUnits[0].GetComponent<Unit>().countingMoves++;
                }
            }
        }

    }
    public override void Attack(int x, int y)
    {
        base.Attack(x, y);
        if (countingActions < numberOfActions)
        {
            // if there is another unit at targeted tile
            if (board.unitsOnBoard[x, y] != null)
            {
                Unit otherU = board.unitsOnBoard[x, y];

                if (otherU.team == team) // if it's the same team, don't attack
                {
                    StartCoroutine(board.ShowTextWarningOnScreen("You can't attack your teammates", 2.0f));
                    return;
                }
                if (otherU.team != team) // if it's enemy team, attack
                {
                    int attackDamage = damage + bonusDamage;

                    Debug.Log("Attack!!! Attack damage is: " + attackDamage); int remaining = damage - otherU.shield;

                    if (remaining <= 0)
                    {
                        otherU.shield -= attackDamage;
                    }
                    else
                    {
                        otherU.shield = 0;
                        otherU.health -= attackDamage;
                    }
                    countingActions++;

                }
            }
        }

    }
}
