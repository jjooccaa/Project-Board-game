using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardManager : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] public float tileSize = 1.0f;
    [SerializeField] public float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] public float deathSize = 0.5f;
    [SerializeField] public float deathSpacing = 0.3f;
   

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabsCops;
    [SerializeField] private GameObject[] prefabsRebels;
    [SerializeField] private GameObject[] prefabsSpecialOps;
    [SerializeField] private GameObject[] prefabsOldOnes;

    [SerializeField] private TextMeshProUGUI textWarnings;

    // LOGIC
    public Unit[,] unitsOnBoard;
    public Unit currentlySelected;
    public List<Unit> whiteUnits = new List<Unit>();
    public List<Unit> blackUnits = new List<Unit>();

    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<Vector2Int> availableAttackRange = new List<Vector2Int>();
    private List<Vector2Int> availableHealRange = new List<Vector2Int>();

    public List<Unit> deadWhites = new List<Unit>();
    public List<Unit> deadBlacks = new List<Unit>();

    private const int TILE_COUNT_x = 10;
    public int TILE_COUNT_X { get { return TILE_COUNT_x; } }
    private const int TILE_COUNT_y = 10;
    public int TILE_COUNT_Y { get { return TILE_COUNT_y; } }

    public GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    public Vector3 bounds;

    private bool isWhiteTurn;
    public bool IsWhiteTurn { get { return isWhiteTurn; } set {isWhiteTurn = value; } }

    private bool isMoving;
    private bool isShooting;
    private bool isHealing;



    private void Awake()
    {
        // white team has first turn
        isWhiteTurn = true;

        isMoving = false;
        isShooting = false;
        isHealing = false;
       
        GenerateAllTiles(tileSize, TILE_COUNT_x, TILE_COUNT_y);

        SpawnAllUnits();
        PositionAllUnits();


    }
    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "HighlightMove", "HighlightAttackRange", "HighlightHealRange")))
        {
            // Get the indexes of the tile we've hit with raycast
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            //If we are hovering a tile for first time
            if (currentHover == -Vector2Int.one)
            { 
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we are hovering a tile after first time, and are not in moving, shooting or healing phase
            if ((currentHover != hitPosition) && !isMoving && !isShooting && !isHealing)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile"); 

                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            if ((currentHover != hitPosition) && isMoving && !isShooting && !isHealing) // -||- and we are in moving phase
            {
                // if there are available moves, change layer to move tiles, if there are not, cange it back to regular tile
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(availableMoves, currentHover) ? LayerMask.NameToLayer("HighlightMove") : LayerMask.NameToLayer("Tile"));

                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            if ((currentHover != hitPosition) && !isMoving && isShooting && !isHealing) // -||- and we are in shooting phase
            {
                // if there are available shooting range, change layer to range tiles, if there are not, change it back to regular tile
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidRange(availableAttackRange, currentHover) ? LayerMask.NameToLayer("HighlightAttackRange") : LayerMask.NameToLayer("Tile"));

                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            if ((currentHover != hitPosition) && !isMoving && !isShooting && isHealing)
            {
                // if there are available healing range, change layer to heal tile, if ther are not, change it back to regular tile
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidHealRange(availableHealRange, currentHover) ? LayerMask.NameToLayer("HighlightHealRange") : LayerMask.NameToLayer("Tile")); ;
            }

            // If we press down left click on the mouse
            if (Input.GetMouseButtonDown(0))
            {
                if (unitsOnBoard[hitPosition.x, hitPosition.y] !=null) // checking if there is any unit on selected tile
                {
                    // Is it white team or black team turn
                    if ((unitsOnBoard[hitPosition.x, hitPosition.y].team == Team.White && isWhiteTurn) || (unitsOnBoard[hitPosition.x, hitPosition.y].team == Team.Black && !isWhiteTurn))
                    {
                        isMoving = true;
                        currentlySelected = unitsOnBoard[hitPosition.x, hitPosition.y];
                        // Get a list of where unit can go, highlight tiles as well
                        availableMoves = currentlySelected.GetAvailableMoves(TILE_COUNT_x, TILE_COUNT_y);
                        HighlightMoveTile();
                    }
                }
            }
            // If we are realising the mouse left button
            if (currentlySelected != null && Input.GetMouseButtonUp(0))
            {
                currentlySelected.MoveTo(hitPosition.x, hitPosition.y);// check if unit can move there, if it can, move it

                isMoving = false;
                currentlySelected = null;
                RemoveHighlighTile();
            }
            // If We press down right click on the mouse
            if (Input.GetMouseButtonDown(1))
            {
                if(unitsOnBoard[hitPosition.x, hitPosition.y] != null) // checking if there is any unit on selected tile
                {
                    // Is it white team or black team turn
                    if((unitsOnBoard[hitPosition.x, hitPosition.y].team == Team.White && isWhiteTurn) || (unitsOnBoard[hitPosition.x, hitPosition.y].team == Team.Black && !isWhiteTurn))
                    {
                        isShooting = true;
                        currentlySelected = unitsOnBoard[hitPosition.x, hitPosition.y];
                        //Get a list of where unit can shoot, highlight tiles as well
                        availableAttackRange = currentlySelected.GetAvailableRange(currentlySelected.attackRange,TILE_COUNT_x, TILE_COUNT_y);
                        HighlightAttackRangeTile();

                    }
                }
            }
            // If we are realising the mouse right button
            if (currentlySelected != null && Input.GetMouseButtonUp(1))
            {
                
                currentlySelected.Attack(hitPosition.x, hitPosition.y);

                isShooting = false;
                currentlySelected = null;
                RemoveHighlighAttackRangeTile();
            
            }
            // If we press down middle mouse button
            if (Input.GetMouseButtonDown(2))
            {
                if (unitsOnBoard[hitPosition.x, hitPosition.y] != null) // checking if there is any unit on selected tile
                {
                    // Is it white team or black team turn
                    if ((unitsOnBoard[hitPosition.x, hitPosition.y].team == Team.White && isWhiteTurn) || (unitsOnBoard[hitPosition.x, hitPosition.y].team == Team.Black && !isWhiteTurn))
                    {
                        isHealing = true;
                        currentlySelected = unitsOnBoard[hitPosition.x, hitPosition.y];
                        //Get a list of where unit can heal, highlight tiles as well
                        availableHealRange = currentlySelected.GetAvailableRange(currentlySelected.healRange, TILE_COUNT_x, TILE_COUNT_y);
                        HighlightHealRangeTile();
                    }
                }
            }
            // When we realise middle mouse button
            if (currentlySelected != null && Input.GetMouseButtonUp(2))
            {
                currentlySelected.Heal(hitPosition.x, hitPosition.y);

                isHealing = false;
                currentlySelected = null;
                RemoveHighlightHealRangeTile();

            }
        }
        else // if we move mouse from the board, reset hover
        {
            if (currentHover != -Vector2Int.one && !isMoving && !isShooting && !isHealing)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
            if (currentHover != -Vector2Int.one && isMoving && !isShooting && !isHealing)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(availableMoves, currentHover) ? LayerMask.NameToLayer("HighlightMove") : LayerMask.NameToLayer("Tile"));
                currentHover = -Vector2Int.one;
            }
            if (currentHover != -Vector2Int.one && !isMoving && isShooting && !isHealing)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidRange(availableAttackRange, currentHover) ? LayerMask.NameToLayer("HighlightAttackRange") : LayerMask.NameToLayer("Tile"));
                currentHover = -Vector2Int.one;
            }
            if (currentHover != -Vector2Int.one && !isMoving && !isShooting && isHealing)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidHealRange(availableHealRange, currentHover) ? LayerMask.NameToLayer("HighlightHealRange") : LayerMask.NameToLayer("Tile"));
                currentHover = -Vector2Int.one;
            }

        }

    }

    // Generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {

        yOffset += transform.position.y; //move pieces up when board moves too
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;


        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }

    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        // create empty mesh for tile 
        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        //make triangles for mesh tile

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    // Spawning of the pieces
    private void SpawnAllUnits()
    {
        unitsOnBoard = new Unit[TILE_COUNT_x, TILE_COUNT_y];

        // White team
        unitsOnBoard[1, 0] = SpawnSingleUnit(CopsUnitType.CopsDPS, Team.White);
        unitsOnBoard[5, 0] = SpawnSingleUnit(CopsUnitType.CopsBoss, Team.White);
        unitsOnBoard[7, 0] = SpawnSingleUnit(CopsUnitType.CopsTank, Team.White);
        unitsOnBoard[8, 0] = SpawnSingleUnit(CopsUnitType.CopsSupport, Team.White);


        // Black team
        unitsOnBoard[1, 9] = SpawnSingleUnit(RebelsUnitType.RebelsDPS, Team.Black);
        unitsOnBoard[4, 9] = SpawnSingleUnit(RebelsUnitType.RebelsBoss, Team.Black);
        unitsOnBoard[7, 9] = SpawnSingleUnit(RebelsUnitType.RebelsTank, Team.Black);
        unitsOnBoard[8, 9] = SpawnSingleUnit(RebelsUnitType.RebelsSupport, Team.Black);


    }

    private Unit SpawnSingleUnit(CopsUnitType type, Team team)
    {
        Unit u = Instantiate(prefabsCops[(int)type - 1], transform).GetComponent<Unit>();

        u.copsType = type;
        u.team = team;
        if(team == Team.White) // add unit to list if white team
        {
            whiteUnits.Add(u);
        }
        if(team == Team.Black)// for black team
        {
            blackUnits.Add(u);
        }

        return u;
    }
    // overlaod for rebel
    private Unit SpawnSingleUnit(RebelsUnitType type, Team team)
    {
        Unit u = Instantiate(prefabsRebels[(int)type - 1], transform).GetComponent<Unit>();

        u.rebelType = type;
        u.team = team;
        if (team == Team.White) // add unit to list if white team
        {
            whiteUnits.Add(u);
        }
        if (team == Team.Black)// for black team
        {
            blackUnits.Add(u);
        }

        return u;
    }
    // overload for special ops
    private Unit SpawnSingleUnit(SpecialOpsUnitType type, Team team)
    {
        Unit u = Instantiate(prefabsSpecialOps[(int)type - 1], transform).GetComponent<Unit>();

        u.sOType = type;
        u.team = team;
        if (team == Team.White) // add unit to list if white team
        {
            whiteUnits.Add(u);
        }
        if (team == Team.Black)// for black team
        {
            blackUnits.Add(u);
        }

        
        return u;
    }
    // overload for Old ones
    private Unit SpawnSingleUnit(OldOnesUnitType type, Team team)
    {
        Unit u = Instantiate(prefabsOldOnes[(int)type - 1], transform).GetComponent<Unit>();

        u.oldOnesType = type;
        u.team = team;
        if (team == Team.White) // add unit to list if white team
        {
            whiteUnits.Add(u);
        }
        if (team == Team.Black)// for black team
        {
            blackUnits.Add(u);
        }

        
        return u;
    }

    // Highlight Tiles
    private void HighlightMoveTile()
    {
        // highlight all tiles where unit can move
        for(int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("HighlightMove");
        }

    }
    private void HighlightAttackRangeTile()
    {
        // highlight all tiles where unit can shoot
        for (int i = 0; i < availableAttackRange.Count; i++)
        {
            tiles[availableAttackRange[i].x, availableAttackRange[i].y].layer = LayerMask.NameToLayer("HighlightAttackRange");
        }
    }
    private void HighlightHealRangeTile()
    {
        for (int i = 0; i < availableHealRange.Count; i++)
        {
            tiles[availableHealRange[i].x, availableHealRange[i].y].layer = LayerMask.NameToLayer("HighlightHealRange");
        }
    }


    private void RemoveHighlighTile()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
     
        availableMoves.Clear();

    }
    private void RemoveHighlighAttackRangeTile()
    {
        for (int i = 0; i < availableAttackRange.Count; i++)
        {
            tiles[availableAttackRange[i].x, availableAttackRange[i].y].layer = LayerMask.NameToLayer("Tile");
        }

        availableAttackRange.Clear();

    }
    private void RemoveHighlightHealRangeTile()
    {
        for (int i = 0; i < availableHealRange.Count; i++)
        {
            tiles[availableHealRange[i].x, availableHealRange[i].y].layer = LayerMask.NameToLayer("Tile");
        }

        availableHealRange.Clear();

    }

    // Positioning
    private void PositionAllUnits()
    {
        for (int x = 0; x < TILE_COUNT_x; x++)
        {
            for (int y = 0; y < TILE_COUNT_y; y++)
            {
                if (unitsOnBoard[x, y] != null)
                {
                    PositionSingleUnit(x, y, true);
                }
           }
        }
    }

    public void PositionSingleUnit(int x, int y, bool force = false)
    {
        unitsOnBoard[x, y].currentX = x;
        unitsOnBoard[x, y].currentY = y;
        unitsOnBoard[x, y].SetPosition(GetTileCenter(x,y), force);
   }

   private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    // Operations
    public bool ContainsValidMove(List<Vector2Int> moves, Vector2 pos)
    {
        // go through all available moves and if it's matching one, return true
        for (int i = 0; i < moves.Count; i++)
        {
            if(moves[i].x == pos.x && moves[i].y == pos.y)
            {
                return true;
            }
        }

        return false;
    }
    public bool ContainsValidRange(List<Vector2Int> range, Vector2 pos)
    {
        // go through all available range and if it's matching one, return true
        for (int i = 0; i < range.Count; i++)
        {
            if (range[i].x == pos.x && range[i].y == pos.y)
            {
                return true;
            }
        }

        return false;
    }
    private bool ContainsValidHealRange(List<Vector2Int> healRange, Vector2 pos)
    {
        // go through all available range and if it's matching one, return true
        for (int i = 0; i < healRange.Count; i++)
        {
            if (healRange[i].x == pos.x && healRange[i].y == pos.y)
            {
                return true;
            }
        }

        return false;
    }
    private Vector2Int LookupTileIndex(GameObject hitinfo)
    {
        for (int x = 0; x < TILE_COUNT_x; x++)
        {
            for (int y = 0; y < TILE_COUNT_y; y++)
            {
                if (tiles[x, y] == hitinfo)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return -Vector2Int.one; // If loop doesn't find tile - break game :D should never happens
    }

    public IEnumerator ShowTextWarningOnScreen(string text, float duration)
    {
        textWarnings.gameObject.SetActive(true);
        textWarnings.text = text;

        yield return new WaitForSeconds(duration);

        textWarnings.gameObject.SetActive(false);
        textWarnings.text = "";

    }

}
