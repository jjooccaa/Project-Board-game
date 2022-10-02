using System.Collections.Generic;
using UnityEngine;

public class RebelsTank : Unit
{
    private const int MOVES = 2;
    private const int RANGE = 3;
    private const int startingNumOfACTION = 1;
    private const int startingHEALTH = 100;
    private const int startingDAMAGE = 5;
    private const int passiveDamage = 10;

    void Awake()
    {
        rebelType = RebelsUnitType.RebelsTank;

        moves = MOVES;
        attackRange = RANGE;
        numberOfActions = startingNumOfACTION;

        health = startingHEALTH;
        healthBar.SetMaxHealth(health);

        damage = startingDAMAGE;
    }

    public override void Passive()
    {
        // +10 dmg if enemy is close range (one tile away)
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
                    // passive
                    if (CheckPassive(otherU.currentX, otherU.currentY))
                    {
                        Debug.Log("Passive Activated");
                        otherU.health -= (attackDamage + passiveDamage);
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
                    }
                    else
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

    private bool CheckPassive(int x, int y)
    {
        Vector2Int pos = new Vector2Int(x, y);
        int close = 1;
        int tileCountY = 10;
        int tileCountX = 10;

        //  front
        for (int i = currentY + 1, countingR = 0; countingR < close && i < tileCountY; i++, countingR++)
        {
            if(pos.x == currentX && pos.y == i)
            {
                return true;
            }
        }

        //  back
        for (int i = currentY - 1, countingR = 0; countingR < close && i >= 0; i--, countingR++)
        {
            if (pos.x == currentX && pos.y == i)
            {
                return true;
            }
        }

        //Left
        for (int i = currentX - 1, countingR = 0; countingR < close && i >= 0; i--, countingR++)
        {
            if (pos.x == i && pos.y == currentY)
            {
                return true;
            }
        }

        //Right
        for (int i = currentX + 1, countingR = 0; countingR < close && i < tileCountX; i++, countingR++)
        {
            if (pos.x == i && pos.y == currentY)
            {
                return true;
            }
        }

        //Top right
        for (int j = currentX + 1, k = currentY + 1, countingR = 0; (j < tileCountX && k < tileCountY) && countingR < close; j++, k++, countingR++)
        {
            if (pos.x == j && pos.y == k)
            {
                return true;
            }
        }

        //Top left
        for (int j = currentX - 1, k = currentY + 1, countingR = 0; (j >= 0 && k < tileCountY) && countingR < close; j--, k++, countingR++)
        {
            if (pos.x == j && pos.y == k)
            {
                return true;
            }
        }

        //bottom right
        for (int j = currentX + 1, k = currentY - 1, countingR = 0; (j < tileCountX && k >= 0) && countingR < close; j++, k--, countingR++)
        {
            if (pos.x == j && pos.y == k)
            {
                return true;
            }
        }

        //bottom left
        for (int j = currentX - 1, k = currentY - 1, countingR = 0; (j >= 0 && k >= 0) && countingR < close; j--, k--, countingR++)
        {
            if (pos.x == j && pos.y == k)
            {
                return true;
            }
        }

        return false;
    }
}
