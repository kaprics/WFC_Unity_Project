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
    public SimplePriorityQueue<(int x, int y)> toPropagate = new();
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
        yield return new WaitForFixedUpdate();
        var rand = (Random.Range(0, gridSizeX), Random.Range(0, gridSizeY));
        _tiles.UpdatePriority(rand, 0);
        
        for (int i = 0; i < gridSizeX*gridSizeY; i++)
        {
            if (i % tilesAtATime == 0)
            {
                yield return new WaitForFixedUpdate();
            }
            var coordinates = _tiles.Dequeue();
            tiles[coordinates].Collapse();
            UpdateConstraints(coordinates);
        }
    }

    // TODO: That's a lotta duplicate code lol
    private void UpdateConstraints((int x, int y) coordinates)
    {
        // if (tiles.ContainsKey((coordinates.x, coordinates.y + 1)))
        // {
        //     var neighbourTiles = tiles[(coordinates.x, coordinates.y + 1)].possibleModules;
        //     var myTiles = tiles[(coordinates.x, coordinates.y)].finalModuleRules.up;
        //     var newPossibleTiles = neighbourTiles.Intersect(myTiles).ToList();
        //     tiles[(coordinates.x, coordinates.y + 1)].possibleModules = newPossibleTiles;
        //     tiles[(coordinates.x, coordinates.y + 1)].CalculateEntropy();
        //     if (_tiles.Contains((coordinates.x, coordinates.y + 1)))
        //     {
        //         _tiles.UpdatePriority((coordinates.x, coordinates.y + 1), tiles[(coordinates.x, coordinates.y + 1)].entropy);
        //     }
        // }
        // if (tiles.ContainsKey((coordinates.x, coordinates.y - 1)))
        // {
        //     var neighbourTiles = tiles[(coordinates.x, coordinates.y - 1)].possibleModules;
        //     var myTiles = tiles[(coordinates.x, coordinates.y)].finalModuleRules.down;
        //     var newPossibleTiles = neighbourTiles.Intersect(myTiles).ToList();
        //     tiles[(coordinates.x, coordinates.y - 1)].possibleModules = newPossibleTiles;
        //     tiles[(coordinates.x, coordinates.y - 1)].CalculateEntropy();
        //     if (_tiles.Contains((coordinates.x, coordinates.y - 1)))
        //     {
        //         _tiles.UpdatePriority((coordinates.x, coordinates.y - 1), tiles[(coordinates.x, coordinates.y - 1)].entropy);
        //     }
        // }
        // if (tiles.ContainsKey((coordinates.x + 1, coordinates.y)))
        // {
        //     var neighbourTiles = tiles[(coordinates.x + 1, coordinates.y)].possibleModules;
        //     var myTiles = tiles[(coordinates.x, coordinates.y)].finalModuleRules.right;
        //     var newPossibleTiles = neighbourTiles.Intersect(myTiles).ToList();
        //     tiles[(coordinates.x + 1, coordinates.y)].possibleModules = newPossibleTiles;
        //     tiles[(coordinates.x + 1, coordinates.y)].CalculateEntropy();
        //     if (_tiles.Contains((coordinates.x + 1, coordinates.y)))
        //     {
        //         _tiles.UpdatePriority((coordinates.x + 1, coordinates.y), tiles[(coordinates.x + 1, coordinates.y)].entropy);
        //     }
        // }
        // if (tiles.ContainsKey((coordinates.x - 1, coordinates.y)))
        // {
        //     var neighbourTiles = tiles[(coordinates.x - 1, coordinates.y)].possibleModules;
        //     var myTiles = tiles[(coordinates.x, coordinates.y)].finalModuleRules.left;
        //     var newPossibleTiles = neighbourTiles.Intersect(myTiles).ToList();
        //     tiles[(coordinates.x - 1, coordinates.y)].possibleModules = newPossibleTiles;
        //     tiles[(coordinates.x - 1, coordinates.y)].CalculateEntropy();
        //     if (_tiles.Contains((coordinates.x - 1, coordinates.y)))
        //     {
        //         _tiles.UpdatePriority((coordinates.x - 1, coordinates.y), tiles[(coordinates.x - 1, coordinates.y)].entropy);
        //     }
        //
        // }

        var queue = new Queue<Tile>();
        queue.Enqueue(tiles[(coordinates.x, coordinates.y)]);

        while (queue.Count > 0)
        {
            var currentTile = queue.Dequeue();
            var neighbours = GetNeighbours(currentTile.coordinates);

            if (neighbours[0] != (-1, -1))
            {
                var legalTiles = new List<Mods>();

                foreach (var mods in currentTile.possibleModules)
                {
                    legalTiles.AddRange(mods.modRules.up);
                }

                legalTiles = legalTiles.Distinct().ToList();

                var oldCount = tiles[neighbours[0]].possibleModules.Count;
                tiles[neighbours[0]].possibleModules = tiles[neighbours[0]].possibleModules.Intersect(legalTiles).ToList();
                tiles[neighbours[0]].CalculateEntropy();
                if(_tiles.Contains(neighbours[0]))
                    _tiles.UpdatePriority(neighbours[0], tiles[neighbours[0]].entropy);
                var newCount = tiles[neighbours[0]].possibleModules.Count;

                if (oldCount != newCount)
                {
                    if(tiles[neighbours[0]].finalModuleRules != null) continue;
                    queue.Enqueue(tiles[neighbours[0]]);
                }
            }
            if (neighbours[1] != (-1, -1))
            {
                var legalTiles = new List<Mods>();

                foreach (var mods in currentTile.possibleModules)
                {
                    legalTiles.AddRange(mods.modRules.down);
                }
                
                legalTiles = legalTiles.Distinct().ToList();

                var oldCount = tiles[neighbours[1]].possibleModules.Count;
                tiles[neighbours[1]].possibleModules = tiles[neighbours[1]].possibleModules.Intersect(legalTiles).ToList();
                tiles[neighbours[1]].CalculateEntropy();
                if(_tiles.Contains(neighbours[1]))
                    _tiles.UpdatePriority(neighbours[1], tiles[neighbours[1]].entropy);
                var newCount = tiles[neighbours[1]].possibleModules.Count;

                if (oldCount != newCount)
                {
                    if(tiles[neighbours[1]].finalModuleRules != null) continue;
                    queue.Enqueue(tiles[neighbours[1]]);
                }
            }
            if (neighbours[2] != (-1, -1))
            {
                var legalTiles = new List<Mods>();

                foreach (var mods in currentTile.possibleModules)
                {
                    legalTiles.AddRange(mods.modRules.right);
                }
                
                legalTiles = legalTiles.Distinct().ToList();

                var oldCount = tiles[neighbours[2]].possibleModules.Count;
                tiles[neighbours[2]].possibleModules = tiles[neighbours[2]].possibleModules.Intersect(legalTiles).ToList();
                tiles[neighbours[2]].CalculateEntropy();
                if(_tiles.Contains(neighbours[2]))
                    _tiles.UpdatePriority(neighbours[2], tiles[neighbours[2]].entropy);
                var newCount = tiles[neighbours[2]].possibleModules.Count;

                if (oldCount != newCount)
                {
                    if(tiles[neighbours[2]].finalModuleRules != null) continue;
                    queue.Enqueue(tiles[neighbours[2]]);
                }
            }
            if (neighbours[3] != (-1, -1))
            {
                var legalTiles = new List<Mods>();

                foreach (var mods in currentTile.possibleModules)
                {
                    legalTiles.AddRange(mods.modRules.left);
                }
                
                legalTiles = legalTiles.Distinct().ToList();

                var oldCount = tiles[neighbours[3]].possibleModules.Count;
                tiles[neighbours[3]].possibleModules = tiles[neighbours[3]].possibleModules.Intersect(legalTiles).ToList();
                tiles[neighbours[3]].CalculateEntropy();
                if(_tiles.Contains(neighbours[3]))
                    _tiles.UpdatePriority(neighbours[3], tiles[neighbours[3]].entropy);
                var newCount = tiles[neighbours[3]].possibleModules.Count;

                if (oldCount != newCount)
                {
                    if(tiles[neighbours[3]].finalModuleRules != null) continue;
                    queue.Enqueue(tiles[neighbours[3]]);
                }
            }
        }
    }

    private List<(int x, int y)> GetNeighbours((int x, int y) coordinates)
    {
        var neighbours = new List<(int x, int y)>();
        
        if(tiles.ContainsKey((coordinates.x, coordinates.y + 1))) neighbours.Add((coordinates.x, coordinates.y + 1));
        else neighbours.Add((-1, -1));
        if(tiles.ContainsKey((coordinates.x, coordinates.y - 1))) neighbours.Add((coordinates.x, coordinates.y - 1));
        else neighbours.Add((-1, -1));
        if(tiles.ContainsKey((coordinates.x + 1, coordinates.y))) neighbours.Add((coordinates.x + 1, coordinates.y));
        else neighbours.Add((-1, -1));
        if(tiles.ContainsKey((coordinates.x - 1, coordinates.y))) neighbours.Add((coordinates.x - 1, coordinates.y));
        else neighbours.Add((-1, -1));

        return neighbours;
    }
}
