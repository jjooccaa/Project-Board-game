using System.Collections.Generic;
using UnityEngine;

public class CopsBoss : Unit
{
    private const int MOVES = 3;
    private const int RANGE = 3;
    private const int startingNumOfACTION = 1;
    private const int startingHEALTH = 100;
    private const int startingDAMAGE = 10;
    private const int passiveDamage = 5;
    private void Awake()
    {
        copsType = CopsUnitType.CopsBoss;

        moves = MOVES;
        attackRange = RANGE;
        numberOfActions = startingNumOfACTION;

        health = startingHEALTH;
        healthBar.SetMaxHealth(health);

        damage = startingDAMAGE;
    }

    public override void Passive()
    {

            // 50 % to deal additional 5 hp dmg
            // implemented
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

                if (otherU.team == team) // if it's the same team, show warning
                {
                    StartCoroutine(board.ShowTextWarningOnScreen("You can't attack your teammates", 2.0f));
                    return;
                }
                if (otherU.team != team) // if it's enemy team, attack
                {
                    int attackDamage = damage + bonusDamage;

                    Debug.Log("Attack!!! Attack damage is: " + attackDamage);

                    // Passive
                    var randomInt = Random.Range(0, 1);
                    if (randomInt == 1)
                    {
                        Debug.Log("Passive Activated.");
                        int remaining = (attackDamage + passiveDamage) - otherU.shield;
                        if (remaining <= 0)
                        {
                            otherU.shield -= (attackDamage + passiveDamage);
                        }
                        else
                        {
                            otherU.shield = 0;
                            otherU.health -= (attackDamage + passiveDamage);
                        }
                    }else
                        {
                        int remaining = attackDamage - otherU.shield;
                        if (remaining <= 0)
                        {
                            otherU.shield -= attackDamage;
                        }
                        else
                        {
                            otherU.shield = 0;
                            otherU.health -= attackDamage;
                        }
                    }

                        countingActions++;
                }
            }
        }
    }
}
