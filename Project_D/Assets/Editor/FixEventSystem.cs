using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public class FixEventSystem : EditorWindow
{
    [MenuItem("Tools/Fix Event System Input Module")]
    public static void Setup()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            StandaloneInputModule oldModule = eventSystem.gameObject.GetComponent<StandaloneInputModule>();
            if (oldModule != null)
            {
                DestroyImmediate(oldModule);
                Debug.Log("Removed legacy StandaloneInputModule.");
            }

            // Since we are using the new input system, we need InputSystemUIInputModule
            // But to avoid compile errors if the package isn't strictly referenced, we can try adding it by type name
            System.Type newModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (newModuleType != null)
            {
                if (eventSystem.gameObject.GetComponent(newModuleType) == null)
                {
                    eventSystem.gameObject.AddComponent(newModuleType);
                    Debug.Log("Added InputSystemUIInputModule.");
                }
            }
            else
            {
                Debug.LogError("Could not find InputSystemUIInputModule type. Make sure the Input System package is installed.");
            }

            EditorUtility.SetDirty(eventSystem.gameObject);
        }
        else
        {
            Debug.Log("EventSystem not found in scene.");
        }
    }
}