using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    public (int x, int y) coordinates;
    [FormerlySerializedAs("finalModule")] public ModuleRules finalModuleRules;
    public List<Mods> possibleModules = new List<Mods>();
    public float entropy;

    private void Awake()
    {
        entropy = Mathf.Infinity;
    }

    public void Collapse()
    {
        finalModuleRules = possibleModules[Random.Range(0, possibleModules.Count)].modRules;
        Instantiate(finalModuleRules.gfx, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
    }
}
