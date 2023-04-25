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
        Random.InitState(1);
        entropy = Mathf.Infinity;
    }

    public void Collapse()
    {
        finalModuleRules = possibleModules[GetRandomModuleWeighted(possibleModules.ToArray())].modRules;
        Instantiate(finalModuleRules.gfx, transform.position, Quaternion.identity);
    }

    private int GetRandomModuleWeighted(Mods[] mods)
    {
        float weightSum = 0f;

        for (int i = 0; i < mods.Length; i++)
        {
            weightSum += mods[i].propability;
        }

        int index = 0;
        int lastIndex = mods.Length - 1;

        while (index < lastIndex)
        {
            if (Random.Range(0, weightSum) < mods[index].propability)
            {
                return index;
            }

            weightSum -= mods[index++].propability;
        }

        return index;
    }

    public void CalculateEntropy()
    {
        var sum = 0f;
        foreach (var module in possibleModules)
        {
            if (module.propability == 1) sum += 1;
            else sum += module.propability * Mathf.Log(module.propability);
        }
        entropy = sum;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, Vector3.one * 0.5f);
    }
}
