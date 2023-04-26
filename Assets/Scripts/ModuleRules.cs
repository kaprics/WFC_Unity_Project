using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Module", menuName = "Modules/New Module")]
public class ModuleRules : ScriptableObject
{
    public GameObject gfx;

    public List<Mods> up = new();
    public List<Mods> down = new();
    public List<Mods> left = new();
    public List<Mods> right = new();
}

[System.Serializable]
public struct Mods : IEqualityComparer<Mods>
{
    public string name;
    public ModuleRules modRules;
    public float probability;

    public bool Equals(Mods x, Mods y)
    {
        return x.name == y.name;
    }

    public int GetHashCode(Mods obj)
    {
        return obj.name.GetHashCode();
    }
}
        