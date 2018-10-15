using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

    public int width;
    public int height;
    public GameObject tilePrefab;

    Tile[,] m_allTiles;

    // Use this for initialization
    void Start()
    {
        m_allTiles = new Tile[width, height];
        SetupTiles();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetupTiles() {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity) as GameObject;

                tile.name = "Tile (" + i + "," + j + ")";

                m_allTiles[i, j] = tile.GetComponent<Tile>();
                tile.transform.parent = transform;

            }
        }
    }
}
