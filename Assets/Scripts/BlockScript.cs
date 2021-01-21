using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    //Variables for Tetris Board and Game
    public static int width = 15, height = 22;
    public static Transform[,] coordinatesFilled = new Transform[width, height];
    public bool isGameOver = false;
    public Vector3 rotationPoint;
    //Variables for block speed movement and delay response
    float previousTime;
    float fallingTime = 0.5f;
    private float xSpeed = 0.1f;
    private float ySpeed = 0.05f;
    private float buttonDelay = 0.2f;
    private float xTime = 0;
    private float yTime = 0;
    private float xbuttonDelayTime = 0;
    private float ybuttonDelayTime = 0;
    private bool xImmediatie = false;
    private bool yImmediatie = false;

    //Variables for touch inputs
    private int xTouchSensitivity = 4; //control block speed following the touch horizontal
    private int yTouchSensitivity = 4; //control block speed following the touch vertical
    Vector2 prevPos = Vector2.zero;
    Vector2 direction = Vector2.zero;
    bool isMoved = false; //to decide either block movement or rotation

    void Update()
    {
        //for phone control
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                //initial position of the touch
                prevPos = new Vector2(touch.position.x, touch.position.y);
            } 
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPos = touch.deltaPosition;
                direction = touchDeltaPos.normalized; //helps to decide right or left
                //Left
                if (Mathf.Abs(touch.position.x - prevPos.x) > xTouchSensitivity && direction.x < 0 
                    && touch.deltaPosition.y > -10 && touch.deltaPosition.y < 10 )
                {
                    LeftMove();
                    prevPos = touch.position;
                    isMoved = true;
                } //Right
                else if(Mathf.Abs(touch.position.x - prevPos.x) > xTouchSensitivity && direction.x > 0
                    && touch.deltaPosition.y > -10 && touch.deltaPosition.y < 10)
                {
                    RightMove();
                    prevPos = touch.position;
                    isMoved = true;
                }//Down
                else if(Mathf.Abs(touch.position.y - prevPos.y) >= yTouchSensitivity && direction.y < 0
                    && touch.deltaPosition.x > -10 && touch.deltaPosition.x < 10)
                {
                    DownMove();
                    prevPos = touch.position;
                    isMoved = true;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if(!isMoved && touch.position.x > Screen.width / 4)
                {
                    RotateBlock();
                }
                isMoved = false;
            }
        }

        //Computer Controls
        if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            xTime = 0;
            xbuttonDelayTime = 0;
            xImmediatie = false;
            
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            yTime = 0;
            yImmediatie = false;
            ybuttonDelayTime = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            RightMove();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            LeftMove();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RotateBlock();
        }
        else if (Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallingTime / 10 : fallingTime))
        { 
            DownMove();
        }
    }

    private void RightMove()
    {
        if (xImmediatie)
        {
          /**  if (xbuttonDelayTime < buttonDelay)
            {
                xbuttonDelayTime += Time.deltaTime;
                return;
            }*/

            if (xTime < xSpeed)
            {
                xTime += Time.deltaTime;
                return;
            }
        }

        if (!xImmediatie)
        {
            xImmediatie = true;
        }

        xTime = 0;

        transform.position += new Vector3(1, 0, 0);
        if (!CheckMove())
        {
            transform.position -= new Vector3(1, 0, 0);
        }
    }

    private void LeftMove()
    {
        if (xImmediatie)
        {
           /** if (xbuttonDelayTime < buttonDelay)
            {
                xbuttonDelayTime += Time.deltaTime;
                return;
            }*/

            if (xTime < xSpeed)
            {
                xTime += Time.deltaTime;
                return;
            }
        }

        if (!xImmediatie)
        {
            xImmediatie = true;
        }

        xTime = 0;

        transform.position += new Vector3(-1, 0, 0);
        if (!CheckMove())
        {
            transform.position -= new Vector3(-1, 0, 0);
        }
    }

    private void DownMove()
    {
        if (yImmediatie)
        {
          /**  if (ybuttonDelayTime < buttonDelay)
            {
                ybuttonDelayTime += Time.deltaTime;
                return;
            }*/

            if (yTime < ySpeed)
            {
                yTime += Time.deltaTime;
                return;
            }
        }

        if (!yImmediatie)
        {
            yImmediatie = true;
        }

        yTime = 0;

        transform.position += new Vector3(0, -1, 0);

        if (!CheckMove())
        {
            transform.position -= new Vector3(0, -1, 0);
            if (!isGameOver)
            {
                AddCordinate();
                CheckLine();
            }
            this.enabled = false;
            FindObjectOfType<ShapeCreator>().CreateShape();
        }
        previousTime = Time.time;
    }

    private void RotateBlock()
    {
        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
      
        if (!CheckMove())
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
        }
    }

    private bool CheckMove()
    {
        foreach (Transform children in transform)
        {
            int x = Mathf.RoundToInt(children.transform.position.x);
            int y = Mathf.RoundToInt(children.transform.position.y);

            if (x < 0 || x >= width || y < 0)
            {
                return false;
            }


            if (coordinatesFilled[x, y] != null)
            {
                return false;
            }

        } 
        
        return true;
    }

    private void AddCordinate()
    {
        foreach (Transform children in transform)
        {
            int x = Mathf.RoundToInt(children.transform.position.x);
            int y = Mathf.RoundToInt(children.transform.position.y);

            if(y >= height)
            {
                isGameOver = true;

            }
            else
            {
                isGameOver = false;
                coordinatesFilled[x, y] = children;
            }
        }
    }

    private void CheckLine()
    {
        int x = 0;
        for(int i = height - 1; i >=0; i--)
        {
            if (LineFilled(i))
            {
                x++;
                ClearLine(i);
                FindObjectOfType<ShapeCreator>().ScoreUpdate(true);
            }
        }

        if(x == 0)
        {
            FindObjectOfType<ShapeCreator>().ScoreUpdate(false);
        }
    }

    private bool LineFilled(int i)
    {
        for(int k = 0; k < width; k++)
        {
            if( coordinatesFilled[k,i] == null)
            {
                return false;
            }
        }

        return true;
    }

    private void ClearLine(int i)
    {
        for (int k = 0; k < width; k++)
        {
            Destroy(coordinatesFilled[k, i].gameObject);
            coordinatesFilled[k, i] = null;
        }

        for (int y = i; y < height; y++)
        {
           for(int x = 0; x < width; x++)
            {
                if(coordinatesFilled[x, y] != null)
                {
                    coordinatesFilled[x, y - 1] = coordinatesFilled[x, y];
                    coordinatesFilled[x, y] = null;
                    coordinatesFilled[x, y - 1].transform.position -= new Vector3(0, 1, 0 );
                }
            }
        }
    }

    public bool CheckAboveGrid(bool yes)
    {
        return yes;

    }

    public bool GameOver()
    {
        if (isGameOver)
        {
            return true;
        }
        return false;
    }
}
