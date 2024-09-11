using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyClicker
{
    static HierarchyClicker()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
    }

    private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go != null && go.name == "에스프레소")
        {
            // 클릭하여 선택하기
            Selection.activeGameObject = go;
        }
    }
}
