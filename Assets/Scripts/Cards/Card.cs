using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Card : MonoBehaviour
{

    protected BoardManager board;
    protected GameManager gameManager;

    public Team team;
    public Type type;

    protected string cardName;
    protected string cardDescription;

    public Vector3 desiredPosition;
    public Vector3 desiredRotation;
    public Vector3 desiredScale;

    public bool isClicked;
    public bool isInHand; 

    private void Awake()
    {
        board = GameObject.Find("Board").GetComponent<BoardManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Start()
    {
        
    }

    protected virtual void Update()
    {
        // move position slowly
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 6);
        transform.rotation = Quaternion.Euler(desiredRotation);
    }

    
    public virtual void Effect()
    {
        
    }

    public void OnMouseDown()
    {
        Debug.Log("Clicked");
        isClicked = true;

    }
    public void OnMouseOver()
    {
        
        if (isInHand)
        {
            SetScale(new Vector3(0.2f, 0.2f, 0.2f), true);
        }
    }
    public void OnMouseExit()
    {
        SetScale(new Vector3(0.1f, 0.1f, 0.1f), true);
        isClicked = false;
        
    }

    public void Print()
    {
        Debug.Log(cardName);
    }

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
        {
            transform.position = desiredPosition;
        }
    }
    public virtual void SetRotation(Vector3 rotationAngels,bool force = false)
    {
        desiredRotation = rotationAngels;
        if(force)
        {
            transform.rotation = Quaternion.Euler(desiredRotation);
        }
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if(force) // if you want to move it with force
        {
            transform.localScale = scale;
        }
    }

}
