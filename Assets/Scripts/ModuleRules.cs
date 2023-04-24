using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Module", menuName = "Modules/New Module")]
public class ModuleRules : ScriptableObject
{
    public string name;
    public GameObject gfx;

    public List<Mods> up = new List<Mods>();
    public List<Mods> down = new List<Mods>();
    public List<Mods> left = new List<Mods>();
    public List<Mods> right = new List<Mods>();
}

[System.Serializable]
public struct Mods
{
    public ModuleRules modRules;
    public float propability;
}
        