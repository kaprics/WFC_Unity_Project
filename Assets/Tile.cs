using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    public (int x, int y) coordinates;
    [FormerlySerializedAs("finalModule")] public ModuleRules finalModuleRules;
    public List<ModuleRules> possibleModules = new List<ModuleRules>();
    public float entropy;

    private void Awake()
    {
        entropy = Mathf.Infinity;
    }

    public void Collapse()
    {
        finalModuleRules = possibleModules[Random.Range(0, possibleModules.Count)];
        Instantiate(finalModuleRules.gfx, transform.position, Quaternion.identity);
    }
}
