using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CopsUnitType
{
    None = 0,
    CopsDPS = 1, // dps
    CopsBoss = 2, // boss
    CopsTank = 3, // tank
    CopsSupport = 4, // support
}
public enum RebelsUnitType
{

    None = 0,
    RebelsDPS = 1, // dps
    RebelsBoss = 2, // boss
    RebelsTank = 3, // tank
    RebelsSupport = 4, // support
}

public enum SpecialOpsUnitType
{

}
public enum OldOnesUnitType
{

}
public class Unit : MonoBehaviour
{
    protected BoardManager board;

    public int currentX;
    public int currentY;
    public Team team;
    public int moves { get; set; }
    public int attackRange { get; set; }
    public int healRange { get; set; }
    public int numberOfActions { get; set; }
    public int health { get; set; }
    public int damage { get; set; }
    public int bonusDamage { get; set; }
    public int heal { get; set; }
    public int shield { get; set; }    

    public int countingMoves { get; set; }
    public int countingActions { get; set; }

    //Type

    public CopsUnitType copsType;
    public RebelsUnitType rebelType;
    public SpecialOpsUnitType sOType;
    public OldOnesUnitType oldOnesType;

    public Vector3 desiredPosition;
    public Vector3 desiredScale = Vector3.one;

    // UI

    public HealthBar healthBar;
    public ParticleSystem fireDeathParticle;

    void Start()
    {
        board = GameObject.Find("Board").GetComponent<BoardManager>();
        // rotate unit depending on team
        transform.rotation = Quaternion.Euler((team == Team.White) ? Vector3.zero : new Vector3(0, 180, 0));

        
    }

    protected virtual void Update()
    {
        if(health <= 0)
        {
            Death();
        }
        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 3);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 5);

        healthBar.SetHealth(health);
        if(health < 20)
        {
            fireDeathParticle.Play();
        }

    }

    public virtual void Passive()
    {

    }

    // Actions
    public virtual void MoveTo(int x, int y)
    {
        //if player has already used his one move
        if (countingMoves >= 1)
        {
            StartCoroutine(board.ShowTextWarningOnScreen("You've already moved", 2.0f)); // show warning text on screen for duration
            return;
        }

        //if unit's chose his own spot 
        if (currentX == x && currentY == y)
        {
            return;
        }

        //if unit's move isn't avaialble, return
        if (!board.ContainsValidMove(GetAvailableMoves(board.TILE_COUNT_X,board.TILE_COUNT_Y), new Vector2Int(x, y)))
        {
            StartCoroutine(board.ShowTextWarningOnScreen("You can't move there!", 2.0f));
            return;
        }

        Vector2Int previousPosition = new Vector2Int(currentX, currentY);

        // Is there another unit on the target position?
        if (board.unitsOnBoard[x, y] != null)
        {
            return;
        }

        board.unitsOnBoard[x, y] = this; // update new position in units array
        board.unitsOnBoard[previousPosition.x, previousPosition.y] = null; // reset prevous position in units array

        board.PositionSingleUnit(x, y);

        countingMoves++;
    }
    public virtual void Attack(int x,int y)
    {
        //if player has already used all his attacks
        if (countingActions >= numberOfActions)
        {
            board.StartCoroutine(board.ShowTextWarningOnScreen("You've already attacked", 2.0f)); // show warning text on screen for duration
            return;
        }
        //if player chose own spot for attack
        if (currentX == x && currentY == y)
        {
            return;
        }
        //if unit's can't attack there
        if (!board.ContainsValidRange(GetAvailableRange(attackRange,board.TILE_COUNT_X,board.TILE_COUNT_Y), new Vector2Int(x, y)))
        {
            board.StartCoroutine(board.ShowTextWarningOnScreen("You can't attack there", 2.0f));
            return;
        }
    }
    public virtual void Heal(int x, int y)
    {
        //if player has already used all his heals
        if (countingActions >= numberOfActions)
        {
            board.StartCoroutine(board.ShowTextWarningOnScreen("You've used all your actions", 2.0f)); // show warning text on screen for duration
            return;
        }
        //if player chose own spot for heal
        if (currentX == x && currentY == y)
        {
            board.StartCoroutine(board.ShowTextWarningOnScreen("You can't heal yourself", 2.0f));
            return;
        }
        //if unit's can't heal there
        if (!board.ContainsValidRange(GetAvailableRange(healRange, board.TILE_COUNT_X, board.TILE_COUNT_Y), new Vector2Int(x, y)))
        {
            board.StartCoroutine(board.ShowTextWarningOnScreen("You can't heal there", 2.0f));
            return;
        }
    }
    public void Death()
    {

        board.unitsOnBoard[currentX, currentY] = null; // remove unit from list
        //otherU.fireDeathParticle.Play(); // play fire animation

        fireDeathParticle.Stop();

        //send unit to graveyard (right part of the board)
        if (team == Team.White)
        {
            
            board.deadWhites.Add(this);
            SetScale(Vector3.one * board.deathSize); // set scale of unit when it dies
            SetPosition(new Vector3(10 * board.tileSize, board.yOffset, -1 * board.tileSize)
                - board.bounds
                + new Vector3(board.tileSize / 2, 0, board.tileSize / 2) // move to the center of the tile
                + (Vector3.forward * board.deathSpacing) * board.deadWhites.Count); // direction where the units go
        }
        if(team == Team.Black)
        {
            board.deadBlacks.Add(this);
            SetScale(Vector3.one * board.deathSize); // set scale of unit when it dies
            SetPosition(new Vector3(-1 * board.tileSize, board.yOffset, 10 * board.tileSize)
                - board.bounds
                + new Vector3(board.tileSize / 2, 0, board.tileSize / 2) // move to the center of the tile
                + (Vector3.back * board.deathSpacing) * board.deadWhites.Count); // direction where the units go
        }
    }
    
    public void IncreaseHealth(int amount)
    {
        if(health + amount >= 100)
        {
            health = 100;
        } else
        {
            health += amount;
        }
    }
    public void DecreaseHealth(int amount)
    {
        health -= amount;
    }

    public List<Vector2Int>GetAvailableMoves(int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int moves = 0;

        // check if team is white or black, if it's white it goes +1, else -1
        //int direction = (team == 0) ? 1 : -1;

        //  front
        for (int i = currentY + 1; i < tileCountY; i++)
        {
            if (moves < this.moves)
            {
                if (board.unitsOnBoard[currentX, i] == null)
                {
                    r.Add(new Vector2Int(currentX, i));
                }

                if (board.unitsOnBoard[currentX, i] != null)
                {

                    break;
                }
            }
            moves++;
        }
        moves = 0;

        //  back
        for (int i = currentY - 1; i >= 0; i--)
        {
            if (moves < this.moves)
            {
                if (board.unitsOnBoard[currentX, i] == null)
                {
                    r.Add(new Vector2Int(currentX, i));
                }

                if (board.unitsOnBoard[currentX, i] != null)
                {

                    break;
                }
            }
            moves++;

        }
        moves = 0;

        //Left
        for (int i = currentX - 1; i >= 0; i--)
        {
            if (moves < this.moves)
            {
                if (board.unitsOnBoard[i, currentY] == null)
                {
                    r.Add(new Vector2Int(i, currentY));
                }

                if (board.unitsOnBoard[i, currentY] != null)
                {

                    break;
                }
            }
            moves++;

        }
        moves = 0;

        //Right
        for (int i = currentX + 1; i < tileCountX; i++)
        {
            if (moves < this.moves)
            {
                if (board.unitsOnBoard[i, currentY] == null)
                {
                    r.Add(new Vector2Int(i, currentY));
                }

                if (board.unitsOnBoard[i, currentY] != null)
                {

                    break;
                }
            }
            moves++;

        }
        moves = 0;

        //Top right
        for (int x = currentX + 1, y = currentY + 1; x < tileCountX && y < tileCountY; x++, y++)
        {
            if (moves < this.moves)
            {
                if (board.unitsOnBoard[x, y] == null)
                {
                    r.Add(new Vector2Int(x, y));
                }
                if (board.unitsOnBoard[x, y] != null)
                {
                    break;
                }
            }
            moves++;
        }
        moves = 0;

        //Top left
        for (int x = currentX - 1, y = currentY + 1; x >= 0 && y < tileCountY; x--, y++)
        {
            if (moves < this.moves)
            {
                if (board.unitsOnBoard[x, y] == null)
                {
                    r.Add(new Vector2Int(x, y));
                }
                if (board.unitsOnBoard[x, y] != null)
                {
                    break;
                }

            }
            moves++;
        }
        moves = 0;

        //bottom right
        for (int x = currentX + 1, y = currentY - 1; x < tileCountX && y >= 0; x++, y--)
        {
            if (moves < this.moves)
            {
                if (board.unitsOnBoard[x, y] == null)
                {
                    r.Add(new Vector2Int(x, y));
                }
                if (board.unitsOnBoard[x, y] != null)
                {
                    break;
                }
            }
            moves++;
        }
        moves = 0;

        //bottom left
        for (int x = currentX - 1, y = currentY - 1; x >= 0 && y >= 0; x--, y--)
        {
            if (moves < this.moves)
            {
                if (board.unitsOnBoard[x, y] == null)
                {
                    r.Add(new Vector2Int(x, y));
                }
                if (board.unitsOnBoard[x, y] != null)
                {
                    break;
                }
            }
            moves++;
        }


        return r;
    }

    public List<Vector2Int> GetAvailableRange(int range, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();


        //  front
        for (int i = currentY + 1, countingR = 0; countingR < range && i < tileCountY; i++, countingR++)
        {
            r.Add(new Vector2Int(currentX, i));

        }


        //  back
        for (int i = currentY - 1, countingR = 0; countingR < range && i >= 0; i--, countingR++)
        {

            r.Add(new Vector2Int(currentX, i));

        }


        //Left
        for (int i = currentX - 1, countingR = 0; countingR < range && i >= 0; i--, countingR++)
        {
            r.Add(new Vector2Int(i, currentY));

        }


        //Right
        for (int i = currentX + 1, countingR = 0; countingR < range && i < tileCountX; i++, countingR++)
        {

            r.Add(new Vector2Int(i, currentY));

        }


        //Top right
        for (int x = currentX + 1, y = currentY + 1, countingR = 0; (x < tileCountX && y < tileCountY) && countingR < range; x++, y++, countingR++)
        {

            r.Add(new Vector2Int(x, y));

        }


        //Top left
        for (int x = currentX - 1, y = currentY + 1, countingR = 0; (x >= 0 && y < tileCountY) && countingR < range; x--, y++, countingR++)
        {

            r.Add(new Vector2Int(x, y));


        }


        //bottom right
        for (int x = currentX + 1, y = currentY - 1, countingR = 0; (x < tileCountX && y >= 0) && countingR < range; x++, y--, countingR++)
        {

            r.Add(new Vector2Int(x, y));


        }


        //bottom left
        for (int x = currentX - 1, y = currentY - 1, countingR = 0; (x >= 0 && y >= 0) && countingR < range; x--, y--, countingR++)
        {

            r.Add(new Vector2Int(x, y));

        }


        return r;
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if(force)
        {
            transform.position = desiredPosition;
        }
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if(force)
        {
            transform.localScale = scale;
        }
    }

    public void ResetMoves()
    {
        countingMoves = 0;
    }

    public void ResetActions()
    {
        countingActions = 0;
    }

}
