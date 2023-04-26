using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
// ReSharper disable CompareOfFloatsByEqualityOperator

public class Tile : MonoBehaviour
{
    public (int x, int y) Coordinates;
    public ModuleRules finalModuleRules;
    public List<Mods> possibleModules = new();
    public float entropy;

    private void Awake()
    {
        entropy = Mathf.Infinity;
    }

    public void Collapse()
    {
        var possibleModule = possibleModules[GetRandomModuleWeighted(possibleModules.ToArray())];
        finalModuleRules = possibleModule.modRules;
        possibleModules.Clear();
        possibleModules.Add(possibleModule);
        Instantiate(finalModuleRules.gfx, transform.position, Quaternion.identity);
    }

    private int GetRandomModuleWeighted(Mods[] mods)
    {
        float weightSum = 0f;

        for (var i = 0; i < mods.Length; i++)
        {
            weightSum += mods[i].probability;
        }

        var index = 0;
        var lastIndex = mods.Length - 1;

        while (index < lastIndex)
        {
            if (Random.Range(0, weightSum) < mods[index].probability)
            {
                return index;
            }

            weightSum -= mods[index++].probability;
        }

        return index;
    }

    public void CalculateEntropy()
    {
        var sum = 0f;
        foreach (var module in possibleModules)
        {
            if (module.probability == 1) sum += 1;
            else sum += module.probability * Mathf.Log(module.probability);
        }
        entropy = sum;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
    }
}
