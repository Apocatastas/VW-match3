using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] gamePiecePrefabs;


    public int borderSize = 2;

    Camera[] cameras;

    public float swapTime = 0.5f;

    Tile[,] m_allTiles;
    GamePiece[,] m_allGamePieces;

    Tile m_clickedTile;
    Tile m_targetTile;




    // Use this for initialization
    void Start()
    {
        cameras = Camera.allCameras;
        m_allTiles = new Tile[width, height];
        m_allGamePieces = new GamePiece[width, height];
        SetupTiles();
        SetupCamera();
        FillRandom();

    }

    // Update is called once per frame
    void Update()
    {

    }



    void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity) as GameObject;

                tile.name = "Tile (" + i + "," + j + ")";

                m_allTiles[i, j] = tile.GetComponent<Tile>();
                tile.transform.parent = transform;

                m_allTiles[i, j].Init(i, j, this);

            }
        }
    }

    void SetupCamera()
    {

        Vector3 temp;

        temp.x = (width - 1) / 2f;
        temp.y = (height - 1) / 2f;
        temp.z = -10f;

        float sw = Screen.width;
        float sh = Screen.height;

        cameras[0].transform.position = temp;

        float aspectRatio = sw / sh;
        Debug.Log("A " + aspectRatio + " = " + " screen width" + Screen.width + " screen height" + Screen.height);
        float verticalSize = height / 2f + borderSize;
        Debug.Log("V " + verticalSize);
        float horizontalSize = (width / 2f + borderSize) / aspectRatio;
        Debug.Log("H " + horizontalSize);
        cameras[0].orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;




    }

    GameObject GetRandomGamePiece()
    {
        int randomIdx = Random.Range(0, gamePiecePrefabs.Length);
        Debug.Log(gamePiecePrefabs.Length);
        Debug.Log(randomIdx);

        if (gamePiecePrefabs[randomIdx] == null)
        {
            Debug.LogWarning("BOARD: " + randomIdx + " does not contain a valid GamePiece prefab!");

        }
        return gamePiecePrefabs[randomIdx];
    }

    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        if (gamePiece == null)
        {

            Debug.LogWarning("BOARD: Invalid GamePiece!");
            return;
        }

        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;
        if (IsWithinBounds(x, y))
        {
            m_allGamePieces[x, y] = gamePiece;
        }

        gamePiece.SetCoord(x, y);


    }

    bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    public void FillRandom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;
                if (randomPiece != null)
                {
                    randomPiece.GetComponent<GamePiece>().Init(this);
                    PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i, j);
                    randomPiece.transform.parent = transform;
                }

            }
        }


    }


    public void ClickTile(Tile tile)

    {

        if (m_clickedTile == null)
        {

            m_clickedTile = tile;
        }
        //Debug.Log("Clicked tile: " + tile.name);


    }

    public void DragToTile(Tile tile)


    {

        if (m_clickedTile != null && isNextTo(tile, m_clickedTile))
        {

            m_targetTile = tile;
        }


    }

    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {
            SwitchTiles(m_clickedTile, m_targetTile);
        }
        m_clickedTile = null;
        m_targetTile = null;
    }

    void SwitchTiles(Tile clickedTile, Tile targetTile)

    {
        GamePiece clickedPiece = m_allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
        GamePiece targetPiece = m_allGamePieces[targetTile.xIndex, targetTile.yIndex];

        clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
        targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
    }

    bool isNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex) { return true; };
        if (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.xIndex) { return true; };
        return false;

    }

    List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece startPiece = null;

        if (IsWithinBounds(startX, startY))
        {
            startPiece = m_allGamePieces[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }

        int nextX=0;
        int nextY=0;

        int maxValue;

        if (width > height)
        {
            maxValue = width;
        }
        else
        {
            maxValue = height;
        }

        for (int i = 1; i < maxValue; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;


            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            GamePiece nextPiece = m_allGamePieces[nextX, nextY];
            if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece)) 
            {
                matches.Add(nextPiece);
             }
            else
           {
                break;
            }


        }

        if (matches.Count >= minLength) 
        {
            return matches;
          }
        else
        {
            return null;
        }
    }
}
