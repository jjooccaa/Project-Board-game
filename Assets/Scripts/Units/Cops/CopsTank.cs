using System.Collections.Generic;
using UnityEngine;

public class CopsTank : Unit
{
    private const int MOVES = 3;
    private const int RANGE = 1;
    private const int startingNumOfACTION = 1;
    private const int startingHEALTH = 100;
    private const int startingDAMAGE = 10;
    private const int constShield = 10; // passive
    
    private void Awake()
    {
        copsType = CopsUnitType.CopsTank;

        moves = MOVES;
        attackRange = RANGE;
        numberOfActions = startingNumOfACTION;

        health = startingHEALTH;
        healthBar.SetMaxHealth(health);

        damage = startingDAMAGE;
        
    }
    protected override void Update()
    {
        base.Update();
        shield = constShield;
    }
    public override void Passive()
    {
        // when hit absorbs 10 dmg 
        // implemented
    }

    public override void Attack(int x, int y)
    {
        base.Attack(x, y);
        if (countingActions <= numberOfActions)
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
}
