using System.Collections.Generic;
using UnityEngine;

public class RebelsDPS : Unit
{
    private const int MOVES = 4;
    private const int RANGE = 2;
    private const int startingNumOfACTION = 1;
    private const int startingHEALTH = 100;
    private const int startingDAMAGE = 5;

    void Awake()
    {
        rebelType = RebelsUnitType.RebelsDPS;

        moves = MOVES;
        attackRange = RANGE;
        numberOfActions = startingNumOfACTION;

        health = startingHEALTH;
        healthBar.SetMaxHealth(health);

        damage = startingDAMAGE;

    }

    public override void Passive()
    {
        // 33% for double damage
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
                    // Passive
                    var randomInt = Random.Range(1, 3);
                    if (randomInt == 1)
                    {
                        Debug.Log("Passive Activated. Double damage.");
                        int passiveDmg = attackDamage * 2;
                        otherU.health -= passiveDmg;
                    }
                    else
                    {
                        otherU.health -= attackDamage;
                    }
                    countingActions++;
                }
            }
        }
    }
}
