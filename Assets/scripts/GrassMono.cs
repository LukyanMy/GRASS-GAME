using System;
using System.Collections.Generic;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.ProBuilder;

public class GrassMono : MonoBehaviour
{
    private LineRenderer line;

    public const string layoutHex = "";

    private readonly List<GameObject> _createdTiles = new List<GameObject>();

    public GameObject[] tiles;
    public int amountOfTiles = 5;

    public float connectionRadius = 1f;

    // Shared material for all connection line
    private Material _connectionMaterial;

    // Container for tiles in the hierarchy
    private GameObject _tilesParent;

    private GameObject CreateTile(string name, Vector3 position)
    {
        var go = new GameObject(name);
        go.transform.position = position;

        _createdTiles.Add(go);
        return go;
    }

    void Start()
    {
        line = GetComponent<LineRenderer>();
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
            line.enabled = false;
        }

        // Create or find the tiles container in the hierarchy
        var existing = transform.Find("Tiles");
        if (existing != null)
            _tilesParent = existing.gameObject;
        else
        {
            _tilesParent = new GameObject("Tiles");
            _tilesParent.transform.SetParent(transform, worldPositionStays: true);
        }

        _connectionMaterial = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        tilesAdder();
    }

    public void tilesAdder()
    {
        for (int j = 0; j < amountOfTiles; j++)
        {
            float constant = j * (Mathf.Sqrt(2f) + 1);
            for (int i = 0; i < amountOfTiles; i += 3)
            {
                CreateTile("hexTileM1", new Vector3(i, 0f, j));
                CreateTile("hexTileT", new Vector3(Mathf.Sqrt(2f) / 2f + i + 1, 0f, Mathf.Sqrt(2f) / 2f + j));
                CreateTile("hexTileT", new Vector3(Mathf.Sqrt(2f) / 2f + i + 2, 0f, Mathf.Sqrt(2f) / 2f + j));
            }
            for (int i = 0; i < amountOfTiles; i += 3)
            {
                CreateTile("hexTileM1", new Vector3(i, 0f, -1 + j));
                CreateTile("hexTileT", new Vector3(Mathf.Sqrt(2f) / 2f + i + 1, 0f, -Mathf.Sqrt(2f) / 2f - 1 + j));
                CreateTile("hexTileT", new Vector3(Mathf.Sqrt(2f) / 2f + i + 2, 0f, -Mathf.Sqrt(2f) / 2f - 1 + j));
            }
        }

        tiles = _createdTiles.ToArray();
        Debug.Log(tiles.Length);

        tilesConnector(tiles);
    }

    private void tilesConnector(GameObject[] tiles)
    {
        if (tiles == null || tiles.Length == 0)
            return;

        float radius = Mathf.Max(0f, connectionRadius);
        float radiusSq = radius * radius;

        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] == null) continue;
            Vector3 posA = tiles[i].transform.position;

            for (int j = 0; j < tiles.Length; j++)
            {
                if (tiles[j] == null || tiles[j] == tiles[i]) continue;
                Vector3 posB = tiles[j].transform.position;

                float distSq = (posA - posB).sqrMagnitude;
                if (distSq <= radiusSq)
                {
                    CreateConnection(posA, posB);
                }
            }
        }
    }

    // used to store and draw lines (help)
    private void CreateConnection(Vector3 a, Vector3 b)
    {
        var conn = new GameObject("Connection");
        conn.transform.SetParent(transform, worldPositionStays: true);

        var lr = conn.AddComponent<LineRenderer>();
        lr.material = _connectionMaterial;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);

        lr.numCapVertices = 2;
        lr.numCornerVertices = 2;
    }
}
