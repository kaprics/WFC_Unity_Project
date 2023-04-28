using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Priority_Queue;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    [SerializeField] private int tilesPerStep = 1;
    [SerializeField] private Vector2 startCoordinates;
    [SerializeField] private int gridSizeX;
    [SerializeField] private int gridSizeY;
    [SerializeField] private Tile tilePrefab;
    private readonly Dictionary<(int x, int y), Tile> _tileGrid = new();
    private readonly SimplePriorityQueue<(int x, int y)> _tilesPriorityQueue = new();
    
    private void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        for (var x = 0; x < gridSizeX; x++)
        {
            for (var y = 0; y < gridSizeY; y++)
            {
                var t = Instantiate(tilePrefab, new Vector3(x * 2, 0, y * 2), Quaternion.identity, transform);
                t.Coordinates = (x, y);
                _tileGrid.Add((x, y), t);
                _tilesPriorityQueue.Enqueue((x,y), _tileGrid[(x,y)].entropy);
            }
        }

        StartCoroutine(Wfc());
    }

    private IEnumerator Wfc()
    {
        _tilesPriorityQueue.UpdatePriority(((int)startCoordinates.x, (int)startCoordinates.y), 0);
        
        for (var i = 0; i < gridSizeX*gridSizeY; i++)
        {
            if (i % tilesPerStep == 0)
            {
                yield return new WaitForFixedUpdate();
            }
            var coordinates = _tilesPriorityQueue.Dequeue();
            _tileGrid[coordinates].Collapse();
            UpdateConstraints(coordinates);
        }
    }
    
    private void UpdateConstraints((int x, int y) coordinates)
    {
        var queue = new Queue<Tile>();
        queue.Enqueue(_tileGrid[(coordinates.x, coordinates.y)]);

        while (queue.Count > 0)
        {
            var currentTile = queue.Dequeue();
            var neighbours = GetNeighbours(currentTile.Coordinates);

            PropagateConstraints(neighbours[0], 0, queue, currentTile);
            PropagateConstraints(neighbours[1], 1, queue, currentTile);
            PropagateConstraints(neighbours[2], 2, queue, currentTile);
            PropagateConstraints(neighbours[3], 3, queue, currentTile);
        }
    }

    private void PropagateConstraints((int x, int y) neighbourCoordinates, int index, Queue<Tile> queue, Tile currentTile)
    {
        if (neighbourCoordinates == (-1, -1)) return;
        var legalTiles = new List<Mods>();

        foreach (var mods in currentTile.possibleModules)
        {
            switch (index)
            {
                case 0:
                    legalTiles.AddRange(mods.modRules.up);
                    break;
                case 1:
                    legalTiles.AddRange(mods.modRules.down);
                    break;
                case 2:
                    legalTiles.AddRange(mods.modRules.right);
                    break;
                case 3:
                    legalTiles.AddRange(mods.modRules.left);
                    break;
            }
        }
                
        legalTiles = legalTiles.Distinct().ToList();

        var oldCount = _tileGrid[neighbourCoordinates].possibleModules.Count;
        _tileGrid[neighbourCoordinates].possibleModules = _tileGrid[neighbourCoordinates].possibleModules.Intersect(legalTiles, new Mods()).ToList();
        _tileGrid[neighbourCoordinates].CalculateEntropy();
        if(_tilesPriorityQueue.Contains(neighbourCoordinates))
            _tilesPriorityQueue.UpdatePriority(neighbourCoordinates, _tileGrid[neighbourCoordinates].entropy);
        var newCount = _tileGrid[neighbourCoordinates].possibleModules.Count;

        if (oldCount != newCount)
        {
            if(_tileGrid[neighbourCoordinates].finalModuleRules != null) return;
            queue.Enqueue(_tileGrid[neighbourCoordinates]);
        }
    }

    private List<(int x, int y)> GetNeighbours((int x, int y) coordinates)
    {
        var neighbours = new List<(int x, int y)>();
        
        if(_tileGrid.ContainsKey((coordinates.x, coordinates.y + 1)) && _tileGrid[(coordinates.x, coordinates.y + 1)].finalModuleRules == null) neighbours.Add((coordinates.x, coordinates.y + 1));
        else neighbours.Add((-1, -1));
        if(_tileGrid.ContainsKey((coordinates.x, coordinates.y - 1)) && _tileGrid[(coordinates.x, coordinates.y - 1)].finalModuleRules == null) neighbours.Add((coordinates.x, coordinates.y - 1));
        else neighbours.Add((-1, -1));
        if(_tileGrid.ContainsKey((coordinates.x + 1, coordinates.y)) && _tileGrid[(coordinates.x + 1, coordinates.y)].finalModuleRules == null) neighbours.Add((coordinates.x + 1, coordinates.y));
        else neighbours.Add((-1, -1));
        if(_tileGrid.ContainsKey((coordinates.x - 1, coordinates.y)) && _tileGrid[(coordinates.x - 1, coordinates.y)].finalModuleRules == null) neighbours.Add((coordinates.x - 1, coordinates.y));
        else neighbours.Add((-1, -1));

        return neighbours;
    }
}
