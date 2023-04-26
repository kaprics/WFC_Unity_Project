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
            switch (m.gameObject.transform.position.z)
            {
                case > 0:
                    moduleRules.up.Add(m.modRules);
                    break;
                case < 0:
                    moduleRules.down.Add(m.modRules);
                    break;
            }

            switch (m.gameObject.transform.position.x)
            {
                case > 0:
                    moduleRules.right.Add(m.modRules);
                    break;
                case < 0:
                    moduleRules.left.Add(m.modRules);
                    break;
            }
        }

        EditorUtility.SetDirty(moduleRules);
    }
}
    