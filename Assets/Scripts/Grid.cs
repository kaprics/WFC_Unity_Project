using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Priority_Queue;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public int gridSizeX;
    public int gridSizeY;
    public Tile tilePrefab;
    public Dictionary<(int x, int y), Tile> tiles = new Dictionary<(int x, int y), Tile>();
    public SimplePriorityQueue<(int x, int y)> _tiles = new SimplePriorityQueue<(int x, int y)>();
    public int tilesAtATime = 1;

    private void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                var t = Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity, transform);
                t.coordinates = (x, y);
                tiles.Add((x, y), t);
                _tiles.Enqueue((x,y), tiles[(x,y)].entropy);
            }
        }

        StartCoroutine(WFC());
    }

    private IEnumerator WFC()
    {
        var rand = (Random.Range(0, gridSizeX), Random.Range(0, gridSizeY));
        _tiles.UpdatePriority(rand, 0);
        
        for (int i = 0; i < gridSizeX*gridSizeY; i++)
        {
            var coordinates = _tiles.Dequeue();
            tiles[coordinates].Collapse();
            UpdateConstraints(coordinates);
            if (i % tilesAtATime == 0)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void UpdateConstraints((int x, int y) coordinates)
    {
        if (tiles.ContainsKey((coordinates.x, coordinates.y + 1)))
        {
            var neighbourTiles = tiles[(coordinates.x, coordinates.y + 1)].possibleModules;
            var myTiles = tiles[(coordinates.x, coordinates.y)].finalModuleRules.up;
            var newPossibleTiles = neighbourTiles.Intersect(myTiles).ToList();
            tiles[(coordinates.x, coordinates.y + 1)].possibleModules = newPossibleTiles;
            tiles[(coordinates.x, coordinates.y + 1)].entropy = newPossibleTiles.Count;
            if (_tiles.Contains((coordinates.x, coordinates.y + 1)))
            {
                _tiles.UpdatePriority((coordinates.x, coordinates.y + 1), tiles[(coordinates.x, coordinates.y + 1)].entropy);
            }
        }
        if (tiles.ContainsKey((coordinates.x, coordinates.y - 1)))
        {
            var neighbourTiles = tiles[(coordinates.x, coordinates.y - 1)].possibleModules;
            var myTiles = tiles[(coordinates.x, coordinates.y)].finalModuleRules.down;
            var newPossibleTiles = neighbourTiles.Intersect(myTiles).ToList();
            tiles[(coordinates.x, coordinates.y - 1)].possibleModules = newPossibleTiles;
            tiles[(coordinates.x, coordinates.y - 1)].entropy = newPossibleTiles.Count;
            if (_tiles.Contains((coordinates.x, coordinates.y - 1)))
            {
                _tiles.UpdatePriority((coordinates.x, coordinates.y - 1), tiles[(coordinates.x, coordinates.y - 1)].entropy);
            }
        }
        if (tiles.ContainsKey((coordinates.x + 1, coordinates.y)))
        {
            var neighbourTiles = tiles[(coordinates.x + 1, coordinates.y)].possibleModules;
            var myTiles = tiles[(coordinates.x, coordinates.y)].finalModuleRules.right;
            var newPossibleTiles = neighbourTiles.Intersect(myTiles).ToList();
            tiles[(coordinates.x + 1, coordinates.y)].possibleModules = newPossibleTiles;
            tiles[(coordinates.x + 1, coordinates.y)].entropy = newPossibleTiles.Count;
            if (_tiles.Contains((coordinates.x + 1, coordinates.y)))
            {
                _tiles.UpdatePriority((coordinates.x + 1, coordinates.y), tiles[(coordinates.x + 1, coordinates.y)].entropy);
            }
        }
        if (tiles.ContainsKey((coordinates.x - 1, coordinates.y)))
        {
            var neighbourTiles = tiles[(coordinates.x - 1, coordinates.y)].possibleModules;
            var myTiles = tiles[(coordinates.x, coordinates.y)].finalModuleRules.left;
            var newPossibleTiles = neighbourTiles.Intersect(myTiles).ToList();
            tiles[(coordinates.x - 1, coordinates.y)].possibleModules = newPossibleTiles;
            tiles[(coordinates.x - 1, coordinates.y)].entropy = newPossibleTiles.Count;
            if (_tiles.Contains((coordinates.x - 1, coordinates.y)))
            {
                _tiles.UpdatePriority((coordinates.x - 1, coordinates.y), tiles[(coordinates.x - 1, coordinates.y)].entropy);
            }

        }
    }
}
