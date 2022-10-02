using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    White = 1,
    Black = 2,
}
public enum Type
{
    None = 0,
    Cops = 1,
    Rebels = 2,
    TheOldOnes = 3,
}
public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject boardObject;
    private BoardManager board;
    [SerializeField] GameObject cardManagerObj;

    // Prefabs
    [Header("Prefab Cards")]
    [SerializeField] private GameObject[] prefabsCopsTypeCards;
    [SerializeField] private GameObject[] prefabsRebelsTypeCards;


    // Cards and decks
    public Deck<Card> whiteTeamDeck = new Deck<Card>(new List<Card>());
    public Deck<Card> blackTeamDeck = new Deck<Card>(new List<Card>());
    private List<Card> cardsInWhiteTeamHand = new List<Card>();
    private List<Card> cardsInBlackTeamHand = new List<Card>();

    public Deck<Card> copsDeck = new Deck<Card>(new List<Card>());
    public Deck<Card> rebelsDeck = new Deck<Card>(new List<Card>());

    //Buttons
    [Header("Buttons")]
    [SerializeField] GameObject whiteEndTurnButton;
    [SerializeField] GameObject blackEndTurnButton;

    // Logic
    private int countingSmndCards;
    public int numberOfWhiteTurns;
    public int numberOfBlackTurns; 


    void Awake()
    {
        board = boardObject.GetComponent<BoardManager>();

        blackEndTurnButton.SetActive(false);

        countingSmndCards = 0;
        numberOfWhiteTurns = 0;
        numberOfBlackTurns = 0;

        SpawnDeck(Type.Cops, Team.White); // spawn white team cops deck
        SpawnDeck(Type.Rebels, Team.Black); // spawn black team rebels deck

        // draw 5 cards for white team and 4 for black at the beginning
        cardsInWhiteTeamHand = whiteTeamDeck.Draw(5); 
        cardsInBlackTeamHand = blackTeamDeck.Draw(4);
    }

    private void Start()
    {
        PositionCardsInHand(Team.White); // position cards in hand for white team
        PositionCardsInHand(Team.Black); // for black team
    }

    // Update is called once per frame
    void Update()
    {
        // if white team card is clicked, summon it and remove it
        if (board.IsWhiteTurn)
        {
            PlayCard(Team.White); // play and summon card for white team
        }
        // if black team card is clicked, summon it and remove it
        if (!board.IsWhiteTurn)
        {
            PlayCard(Team.Black); // play and summon card for black team
        }

    }


    // when player presses end turn button, reset everything and opponent draws another card
    public void EndWhiteTurn()
    { 
        board.IsWhiteTurn = false;

        //call CopsDPS's Passive
        if (board.blackUnits[0].copsType == CopsUnitType.CopsDPS)
        {
            if (board.blackUnits[0].health > 0) // checking if CopsDPS is alive
            {
                board.blackUnits[0].Passive();
            }
        }
        // reset moves, actions and summoned cards
        foreach (Unit u in board.whiteUnits)
        {
            u.ResetMoves();
            u.ResetActions();
        }
        countingSmndCards = 0;

        // opponent draws card when turn is over
        List<Card> drawnCards = blackTeamDeck.Draw(1);
        if (drawnCards != null)
        {
            Card c = drawnCards[0];
            cardsInBlackTeamHand.Add(c);
        }
        if (drawnCards == null)
        {
            Debug.Log("No more cards");
        }
        PositionCardsInHand(Team.Black);


        // Show Black team EndTurnButton and hide White team EndTurnButton
        whiteEndTurnButton.SetActive(false);
        blackEndTurnButton.SetActive(true);

        // Turns passed counter
        if(numberOfWhiteTurns != 0)
        {
            numberOfBlackTurns++;
        }
    }
    public void EndBlackTurn()
    {
        board.IsWhiteTurn = true;

        //CopsDPS's Passive
        if (board.whiteUnits[0].copsType == CopsUnitType.CopsDPS)
        {
            if (board.whiteUnits[0].health > 0) // checking if CopsDPS is alive
            {
                board.whiteUnits[0].Passive();
            }
        }
        // reset moves, actions and summoned cards
        foreach (Unit u in board.blackUnits)
        {
            u.ResetMoves();
            u.ResetActions();
        }
        countingSmndCards = 0;

        // opponent draws card when turn is over
        List<Card> drawnCards = whiteTeamDeck.Draw(1);
        if (drawnCards != null)
        {
            Card c = drawnCards[0];
            cardsInWhiteTeamHand.Add(c);
        }
        if (drawnCards == null)
        {
            Debug.Log("No more cards");
        }
        PositionCardsInHand(Team.White); // position cards for white team

        // Show White team EndTurnButton and hide Black EndTurnButton
        whiteEndTurnButton.SetActive(true);
        blackEndTurnButton.SetActive(false);

        // Turns passed counter
        numberOfWhiteTurns++;
    }



    // Cards
    private void SpawnSingleCard(Card c, int cardPlace)
    {
        if(c.type == Type.Cops)
        {
            Instantiate(prefabsCopsTypeCards[cardPlace], transform);
            
        }
        if(c.type == Type.Rebels)
        {
            Instantiate(prefabsRebelsTypeCards[cardPlace], transform);
        }
    }
    private void SpawnDeck(Type type, Team team)
    {
        if (team == Team.White) // white team deck
        {
            whiteTeamDeck = GetDeck(type,team);
            for (int i = 0; i < whiteTeamDeck.Cards().Count; i++)
            {
                Card c = whiteTeamDeck.Cards()[i];
                c.SetPosition(new Vector3(6.52f, 0.52f, -3.67f), true);
                c.SetRotation(new Vector3(90, 0, 0), true);
            }
        }

        if (team == Team.Black) // black team deck
        {
            blackTeamDeck = GetDeck(type, team);

            for (int i = 0; i < blackTeamDeck.Cards().Count; i++)
            {
                Card c = blackTeamDeck.Cards()[i];
                c.SetPosition(new Vector3(-6.52f, 0.52f, 3.67f), true);
                c.SetRotation(new Vector3(90, 0, 0), true);
            }
        }

    }
    public Deck<Card> GetDeck(Type type,Team team)
    {
        if (type == Type.Cops)
        {
            return CopsDeck(team);
        }
        if (type == Type.Rebels)
        {
            return RebelsDeck(team);
        }

        return null;

    }
    private Deck<Card> CopsDeck(Team team)
    {
        for (int i = 0; i < prefabsCopsTypeCards.Length; i++)
        {
 
            Card c = Instantiate(prefabsCopsTypeCards[i], transform).GetComponent<Card>();
            c.team = team;
            copsDeck.Cards().Add(c);
        }

        return copsDeck;
    }

    private Deck<Card> RebelsDeck(Team team)
    {
        for (int i = 0; i < prefabsRebelsTypeCards.Length; i++)
        {
            Card c = Instantiate(prefabsRebelsTypeCards[i], transform).GetComponent<Card>();
            c.team = team;
            rebelsDeck.Cards().Add(c);
        }

        return rebelsDeck;
    }

    private void PositionCardsInHand(Team team)
    {
        // white team, position cards depending on number
        if (team == Team.White)
        {
            var leftPoint = new Vector3(-1.6f, 0.5f, -6.5f);
            var rightPoint = new Vector3(1.6f, 0.5f, -6.5f);
            var delta = (rightPoint - leftPoint).magnitude;
            var howManyCardsInHand = cardsInWhiteTeamHand.Count;
            var howManyGapsBetweenCards = howManyCardsInHand - 1;
            var gapFromOneCardToNextOne = delta / howManyGapsBetweenCards;
            int theHighestIndex = howManyCardsInHand;

            //float totalTwist = 30f;
            //float twistPerCard = totalTwist / howManyCardsInHand;
            //float startTwist = -1f * (totalTwist / 2f);

            for (int i = 0; i < theHighestIndex; i++)
            {
                cardsInWhiteTeamHand[i].SetPosition(leftPoint, true);
                cardsInWhiteTeamHand[i].SetPosition(cardsInWhiteTeamHand[i].transform.position += new Vector3(i * gapFromOneCardToNextOne, 0, 0), true);
                cardsInWhiteTeamHand[i].SetRotation(new Vector3(45, 0, 0), true);
                cardsInWhiteTeamHand[i].isInHand = true;

                //float twistforThisCard = startTwist + (i * twistPerCard);
                //cardsInWhiteTeamHand[i].SetRotation(new Vector3(45f, 0f, twistforThisCard),true);

            }
        }

        // blak team
        if (team == Team.Black)
        {
            var leftPoint = new Vector3(-1.8f, 0.5f, 6.5f);
            var rightPoint = new Vector3(1.8f, 0.5f, 6.5f);
            var delta = (rightPoint - leftPoint).magnitude;
            var howManyCardsInHand = cardsInBlackTeamHand.Count;
            var howManyGapsBetweenCards = howManyCardsInHand - 1;
            var gapFromOneCardToNextOne = delta / howManyGapsBetweenCards;
            int theHighestIndex = howManyCardsInHand;
            for (int i = 0; i < theHighestIndex; i++)
            {
                cardsInBlackTeamHand[i].SetPosition(leftPoint, true);
                cardsInBlackTeamHand[i].SetPosition(cardsInBlackTeamHand[i].transform.position += new Vector3(i * gapFromOneCardToNextOne, 0, 0), true);
                cardsInBlackTeamHand[i].SetRotation(new Vector3(-45, 0, 0), true);
                cardsInBlackTeamHand[i].isInHand = true;
            }
        }
    }

    private void PlayCard(Team team)
    {
        if (team == Team.White) 
        {
            for (int i = 0; i < cardsInWhiteTeamHand.Count; i++)
            {
                if (countingSmndCards == 0) // if this is the first card that is summond in this turn
                {
                    if (cardsInWhiteTeamHand[i].isClicked)
                    {
                        cardsInWhiteTeamHand[i].Effect();
                        cardsInWhiteTeamHand[i].SetPosition(new Vector3(0, 1, -3), false); // if card is summoned move card to the board 
                        //cardsInWhiteTeamHand[i].SetRotation(new Vector3(-90, 0, 0), false);
                        StartCoroutine(DelayDestroyingObject(cardsInWhiteTeamHand[i].gameObject, 3)); // remove card from board after "" seconds
                        cardsInWhiteTeamHand.Remove(cardsInWhiteTeamHand[i]); // remove card from list
                        PositionCardsInHand(Team.White); // position all cards
                        countingSmndCards++; // count how many cards are summoned in one turn 
                    }
                }
            }
        }
        if (team == Team.Black) 
        {
            for (int i = 0; i < cardsInBlackTeamHand.Count; i++)
            {
                if (countingSmndCards == 0) // if this is the first card that is summond in this turn
                {
                    if (cardsInBlackTeamHand[i].isClicked)
                    {
                        cardsInBlackTeamHand[i].Effect();
                        cardsInBlackTeamHand[i].SetPosition(new Vector3(0, 1, 3), false); // if card is summoned move card to the board 
                        StartCoroutine(DelayDestroyingObject(cardsInBlackTeamHand[i].gameObject, 3)); // remove card from board after "" seconds
                        cardsInBlackTeamHand.Remove(cardsInBlackTeamHand[i]);// remove card from list
                        PositionCardsInHand(Team.Black);// position all cards
                        countingSmndCards++;// count how many cards are summoned in one turn 
                    }
                }
            }
        }
    }


    IEnumerator DelayDestroyingObject(GameObject obj, float duration)
    {
        // lock mouse and temp disable raycasting on board, after duration destroy object and enable everything again

        Collider collTileOne = board.tiles[4, 4].GetComponent<Collider>();
        Collider collTileTwo = board.tiles[5, 4].GetComponent<Collider>();
        collTileOne.enabled = false;
        collTileTwo.enabled = false;

        Cursor.lockState = CursorLockMode.Locked; // temp lock mouse input

        yield return new WaitForSeconds(duration);

        Destroy(obj);
        Cursor.lockState = CursorLockMode.None; // unlock mouse input

        collTileOne.enabled = true;
        collTileTwo.enabled = true;
    }
}
