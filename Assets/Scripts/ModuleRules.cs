using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Module", menuName = "Modules/New Module")]
public class ModuleRules : ScriptableObject
{
    public string name;
    public GameObject gfx;

    public List<ModuleRules> up = new List<ModuleRules>();
    public List<ModuleRules> down = new List<ModuleRules>();
    public List<ModuleRules> left = new List<ModuleRules>();
    public List<ModuleRules> right = new List<ModuleRules>();

}
        