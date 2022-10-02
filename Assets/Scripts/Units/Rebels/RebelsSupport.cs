using System.Collections.Generic;
using UnityEngine;

public class RebelsSupport : Unit
{
    private const int MOVES = 4;
    private const int RANGE = 4;
    private const int startingNumOfACTION = 2; // passive
    private const int startingHEALTH = 100;
    private const int startingDAMAGE = 10;
    private const int startingHeal = 10;

    void Awake()
    {
        rebelType = RebelsUnitType.RebelsSupport;

        moves = MOVES;
        attackRange = RANGE;
        healRange = RANGE;
        numberOfActions = startingNumOfACTION;

        health = startingHEALTH;
        healthBar.SetMaxHealth(health);

        damage = startingDAMAGE;
        heal = startingHeal;
    }

    public override void Passive()
    {
        // can attack twice, or heal twice, also you can mix it
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
    public override void Heal(int x, int y)
    {
        base.Heal(x, y);
        if (countingActions < numberOfActions)
        {
            // if there is another unit at targeted tile
            if (board.unitsOnBoard[x, y] != null)
            {
                Unit otherU = board.unitsOnBoard[x,y];

                if (otherU.team == team) // if it's the same team, heal teammate
                {
                    Debug.Log("Heal!!!");
                    otherU.health += heal;
                    countingActions++;
                }
                if (otherU.team != team) // if it's enemy team, attack
                {
                    StartCoroutine(board.ShowTextWarningOnScreen("You can't heal your opponent", 2.0f));
                    return;
                }
            }
        }
    }
}
