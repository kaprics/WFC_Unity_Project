using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class ModuleSetup : MonoBehaviour
{
    [FormerlySerializedAs("module")] public ModuleRules moduleRules;

    private void Start()
    {
        moduleRules.up.Clear();
        moduleRules.down.Clear();
        moduleRules.right.Clear();
        moduleRules.left.Clear();
        
        foreach (var m in FindObjectsOfType<Module>())
        {
            if (m.gameObject.transform.position.z > 0)
            {
                moduleRules.up.Add(m.modRules);
            }
            if (m.gameObject.transform.position.z < 0)
            {
                moduleRules.down.Add(m.modRules);
            }
            if (m.gameObject.transform.position.x > 0)
            {
                moduleRules.right.Add(m.modRules);
            }
            if (m.gameObject.transform.position.x < 0)
            {
                moduleRules.left.Add(m.modRules);
            }
            
        }

        EditorUtility.SetDirty(moduleRules);
    }
}
    