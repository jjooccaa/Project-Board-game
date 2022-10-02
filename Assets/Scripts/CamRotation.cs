using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotation : MonoBehaviour
{
    [SerializeField] GameObject board;
    [SerializeField] float rotationSpeed = 30.0f;
    [SerializeField] GameObject uiCanvas;
    Vector3 whiteCamPosition = new Vector3(0, 7, -10);
    Vector3 blackCamPosition = new Vector3(0, 7, 10);


    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.RotateAround(board.transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
            uiCanvas.SetActive(false);         
        }
        else
        {
            uiCanvas.SetActive(true);

            // check who is playing 
            if (board.GetComponent<BoardManager>().IsWhiteTurn)
            {
                RotateToStartPosition(0);
            }
            if (!board.GetComponent<BoardManager>().IsWhiteTurn)
            {
                RotateToStartPosition(1);
            }
        }

    }

    void RotateToStartPosition(int team)
    {
        if(team == 0) // white team start position camera
        {
            transform.position = Vector3.Lerp(transform.position, whiteCamPosition, (rotationSpeed / 12.0f) * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(36, 0, 0), (rotationSpeed / 12.0f) * Time.deltaTime);
            
            
        }
        if(team == 1) // black team start position camera
        {
            transform.position = Vector3.Lerp(transform.position, blackCamPosition, (rotationSpeed / 12.0f) * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(36,180,0), (rotationSpeed / 12.0f) * Time.deltaTime);
            
        }
    }
}
