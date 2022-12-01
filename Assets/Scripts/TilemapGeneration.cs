using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGeneration : MonoBehaviour
{
    [Header("Tilemaps")]
    Tilemap tilemap;

    int[,] map;

    [Header("Map Propieties")]
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int boundsSize = 5;
    [SerializeField] Tiles tile;

    [Header("Noise map Propieties")]
    [SerializeField] bool randomSeed = true;
    [SerializeField] float noisemapScale = 1;
    [SerializeField] [Range(0, 1)] float smooth = 0.5f;
    [SerializeField] Wave[] waves;

    private void Start()
    {
        GenerateMap();
    }

    private void Update()
    {
        if(Input.GetMouseButton(1))
        {
            SetTile(WorldToMapCoordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition)),0);
        }

        if (Input.GetMouseButton(0))
        {
            SetTile(WorldToMapCoordinate(Camera.main.ScreenToWorldPoint(Input.mousePosition)), 1);
        }
    }

    public Vector3Int WorldToMapCoordinate(Vector3 worldPos)
    {
        return tilemap.WorldToCell(worldPos);
    }

    public void SetTile(Vector3Int pos, int id)
    {
        map[pos.x, pos.y] = id;
        ModifyTilemap(pos.x, pos.y);
    }

    void GenerateMap()
    {
        float[,] noise = GetNoiseMap(noisemapScale, waves);
        map = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = noise[x, y] > smooth ? 1 : 0;
            }
        }
        GenerateBounds(boundsSize);
        GenerateTilemap();
    }

    void GenerateTilemap()
    {
        if (tilemap == null) tilemap = GetComponent<Tilemap>();
        tilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] >= 1)
                {
                        tilemap.SetTile(new Vector3Int(x, y), GetTile(x, y));
                }
            }
        }
    }

    void ModifyTilemap(int x, int y)
    {
        if (map[x, y] == 0)
            tilemap.SetTile(new Vector3Int(x, y), null);
        else
        {
            tilemap.SetTile(new Vector3Int(x, y), GetTile(x, y));
        }

        for (int _x = x - 1; _x <= x + 1; _x++)
        {
            for (int _y = y - 1; _y <= y + 1; _y++)
            {
                if (map[_x, _y] == 1)
                    tilemap.SetTile(new Vector3Int(_x, _y), GetTile(_x, _y));
            }
        }
    }

    TileBase GetTile(int x, int y)
    {
        TileBase _tile = tile.middle;
        
        if (y + 1 < height && y - 1 < height && y != 0 && x + 1 < width && x - 1 < width && x != 0)
        {
            //Top - Down - Left - Right
            if (map[x, y + 1] == 0)
                _tile = tile.topMiddle;
            else
            if (map[x, y - 1] == 0)
                _tile = tile.bottomMiddle;
            else
            if (map[x - 1, y] == 0)
                _tile = tile.middleRight;
            else
            if (map[x + 1, y] == 0)
                _tile = tile.middleLeft;

            //Corners
                //TopRight
            if (map[x + 1, y + 1] == 0 && map[x, y + 1] == 0 && map[x + 1, y] == 0)
                _tile = tile.topRight;
            else
                //Top Left
            if (map[x - 1, y + 1] == 0 && map[x, y + 1] == 0 && map[x - 1, y] == 0)
                _tile = tile.topLeft;
            else
                //BotRight
            if (map[x + 1, y - 1] == 0 && map[x, y - 1] == 0 && map[x + 1, y] == 0)
                _tile = tile.bottomLeft;
            else
                //BotLeft
            if (map[x - 1, y - 1] == 0 && map[x, y - 1] == 0 && map[x - 1, y] == 0)
                _tile = tile.bottomRight;

            if (GetNumberOfNeighbors(x, y) <= 0)
                _tile = tile.Decoration;
        }
        
        return _tile;
    }

    void GenerateBounds(int size)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x <= size - 1|| x >= width - size || y <= size - 1 || y >= height - size)
                    map[x, y] = 1;
            }
        }
    }

    float[,] GetNoiseMap(float scale, Wave[] wavesFunc)
    {
        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                float normalization = 0.0f;

                float samplePosX = (float)x * scale;
                float samplePosY = (float)y * scale;

                foreach (var wave in wavesFunc)
                {
                    float seed = randomSeed == false ? 0 : Time.time * 100;
                    noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise((float)samplePosX * wave.frequency + wave.seed + seed,(float)samplePosY * wave.frequency + wave.seed + seed);
                    normalization += wave.amplitude;
                }
                noiseMap[x, y] /= normalization;
            }
        }
        return noiseMap;
    }
    
    int GetNumberOfNeighbors(int x, int y)
    {
        int number = -1;

        for (int _x = x - 1; _x <= x + 1; _x++)
            for (int _y = y - 1; _y <= y + 1; _y++)
                if (map[_x, _y] == 1)
                   number += 1;

        return number;
    }

    //Generate map button
    private void OnGUI()
    {
        if (GUILayout.Button("Genearte"))
            GenerateMap();

        GUILayout.Label("Controls:\nMouse1=Place tile\nMouse2=Remove tile\nE/Q=Up and Down");
    }
    
}

[Serializable]
public class Tiles
{
    public RuleTile ruleTile;
    [Space(10)]
    public TileBase topRight;
    public TileBase topMiddle;
    public TileBase topLeft;
    [Space(10)]
    public TileBase middleRight;
    public TileBase middle;
    public TileBase middleLeft;
    [Space(10)]
    public TileBase bottomRight;
    public TileBase bottomMiddle;
    public TileBase bottomLeft;
    [Space(10)]
    public TileBase CornerBottomRight;
    public TileBase CornerBottomLeft;
    public TileBase CornerTopRight;
    public TileBase CornerTopLeft;
    [Space(10)]
    public TileBase Decoration;
}
