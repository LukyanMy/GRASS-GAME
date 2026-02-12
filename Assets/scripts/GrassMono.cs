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

    // Red dot texture/sprite for tile visuals
    private Texture2D _dotTexture;
    private Sprite _dotSprite;

    // Configurable dot appearance
    public int dotTextureSize = 32;
    public float dotDisplaySize = 0.2f;

    private GameObject CreateTile(string name, Vector3 position)
    {
        var go = new GameObject(name);
        go.transform.position = position;

        // Parent under Tiles container for organization
        if (_tilesParent != null)
            go.transform.SetParent(_tilesParent.transform, worldPositionStays: true);

        // Add a SpriteRenderer and assign the generated red-dot sprite
        if (_dotSprite != null)
        {
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _dotSprite;
            sr.color = Color.red;
            // Ensure dots render above other geometry by default
            sr.sortingOrder = 100;
            // Scale so circle fits desired world size
            go.transform.localScale = new Vector3(dotDisplaySize, dotDisplaySize, dotDisplaySize);
        }

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

        // Generate red dot texture and sprite
        GenerateRedDotSprite(Mathf.Max(8, dotTextureSize));

        tilesAdder();
    }

    public void tilesAdder()
    {
        for (int j = 0; j < amountOfTiles; j++)
        {
            for (int i = 0; i < amountOfTiles; i++)
            {
                    CreateTile("hexTileT1", new Vector3(Mathf.Sqrt(3) * i + Mathf.Sqrt(3) / 2, 0, j * (Mathf.Sqrt(3) + 1)));
                    CreateTile("hexTileT2", new Vector3(Mathf.Sqrt(3) * i, 0, j * (Mathf.Sqrt(3) + 1) + 0.5f));
                    CreateTile("hexTileT3", new Vector3(Mathf.Sqrt(3) * i, 0, j * (Mathf.Sqrt(3) + 1) + Mathf.Sqrt(3) / 2 + 0.5f));
                    CreateTile("hexTileT4", new Vector3(Mathf.Sqrt(3) * i + Mathf.Sqrt(3) / 2, 0,j * (Mathf.Sqrt(3) + 1) + 2));
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

    // Generates a circular red texture and creates a Sprite from it.

    private void GenerateRedDotSprite(int size)
    {
        int s = Mathf.Max(8, size);
        var tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        float cx = (s - 1) * 0.5f;
        float cy = (s - 1) * 0.5f;
        float radius = s * 0.5f;

        for (int y = 0; y < s; y++)
        {
            for (int x = 0; x < s; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01((radius - dist) + 0.5f);
                a = Mathf.SmoothStep(0f, 1f, a);
                Color col = new Color(1f, 0f, 0f, a);
                tex.SetPixel(x, y, col);
            }
        }

        tex.Apply();

        _dotTexture = tex;
        _dotSprite = Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), s);
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
