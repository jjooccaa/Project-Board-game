using System.Collections.Generic;
using UnityEngine;

public class RebelsBoss : Unit
{
    private const int startingMOVES = 1;
    private const int startingRANGE = 3;
    private const int startingNumOfACTION = 1;
    private const int startingHEALTH = 100;
    private const int startingDAMAGE = 20;

    void Awake()
    {
        rebelType = RebelsUnitType.RebelsBoss;

        moves = startingMOVES;
        attackRange = startingRANGE;
        numberOfActions = startingNumOfACTION;

        health = startingHEALTH;
        healthBar.SetMaxHealth(health);

        damage = startingDAMAGE;
    }
    protected override void Update()
    {
        base.Update();
        CheckPassive();

    }
    public override void Passive()
    {
        // +10 dmg for all allies one tile away, +5 dmg for all allies two tiles away
        // implemented (one tile all away is working, two tiles away partially working)
        
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

                    Debug.Log("Attack!!! Attack damage is: " + attackDamage);

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
                    countingActions++;

                }
            }
        }
    }

    private bool CheckForRangeOneOrTwo(Unit u, int range)
    {

        if (range == 1)
        {
            if ((currentY == u.currentY && (currentX == u.currentX + range || currentX == u.currentX - range))
                || (currentX == u.currentX && (currentY == u.currentY + range || currentY == u.currentY - range))
                || ((currentY == u.currentY + range || currentY == u.currentY - range) && (currentX == u.currentX + range || currentX == u.currentX - range)))
            {
                return true;
            }
        } 
        if (range == 2)
        {
            if (((currentX - 1 == u.currentX || currentX - 2 == u.currentX || currentX - 3 == u.currentX || currentX == u.currentX || currentX + 1 == u.currentX || currentX + 2 == u.currentX || currentX + 3 == u.currentX) 
                && (currentY + 2 == u.currentY || currentY - 2 == u.currentY))
                || ((currentX - range == u.currentX || currentX + range == u.currentX) && (currentY - 1 == u.currentY || currentY == u.currentY || currentY + 1 == u.currentY)))
            {
                return true;
            }
        }
        return false;
    }

    public void CheckPassive()
    {
        if (team == Team.White) {
            foreach (Unit u in board.whiteUnits)
            {
                if (u.rebelType != RebelsUnitType.RebelsBoss)
                {
                    if (CheckForRangeOneOrTwo(u, 1))
                    {
                        u.bonusDamage = 10;
                        
                    } else if (CheckForRangeOneOrTwo(u, 2))
                    {
                        u.bonusDamage = 5;
                    } else
                    {
                        u.bonusDamage = 0;
                    }
                }
            }
        } else {
            foreach (Unit u in board.blackUnits)
            {
                if (u.rebelType != RebelsUnitType.RebelsBoss)
                {
                    if (CheckForRangeOneOrTwo(u, 1))
                    {
                        u.bonusDamage = 10;
                    } else if (CheckForRangeOneOrTwo(u, 2))
                    {
                        u.bonusDamage = 5;
                    } else
                    {
                        u.bonusDamage = 0;
                    }
                }
            }
        }
    }
}