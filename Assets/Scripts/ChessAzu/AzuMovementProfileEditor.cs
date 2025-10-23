#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AzuMovementProfile))]
public class AzuMovementProfileEditor : Editor
{
    SerializedProperty pieceNameKeyProp;
    SerializedProperty movementMaskProp;

    const int SIZE = AzuMovementProfile.SIZE;
    const int CENTER_INDEX = AzuMovementProfile.CENTER_INDEX;

    void OnEnable()
    {
        pieceNameKeyProp = serializedObject.FindProperty("pieceNameKey");
        movementMaskProp = serializedObject.FindProperty("movementMask");
        if (movementMaskProp.arraySize != 25) movementMaskProp.arraySize = 25;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(pieceNameKeyProp);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Movement Mask (5×5)", EditorStyles.boldLabel);
        DrawMaskGrid();

        EditorGUILayout.Space(6);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Clear")) ApplyAll(false);
            if (GUILayout.Button("Fill"))  ApplyAll(true);
            if (GUILayout.Button("Invert")) InvertAll();
        }

        serializedObject.ApplyModifiedProperties();
        EditorGUILayout.HelpBox("Top row is +Y (up). Center 'P' is the piece and is ignored.", MessageType.None);
    }

    void DrawMaskGrid()
    {
        var box = GUILayoutUtility.GetRect(1, 1, GUILayout.Height(5 * 22 + 12));
        box = new Rect(box.x, box.y, box.width, 5 * 22 + 12);
        GUI.Box(box, GUIContent.none);

        Rect g = new Rect(box.x + 6, box.y + 6, box.width - 12, box.height - 12);
        float cellW = Mathf.Min(28f, (g.width - 4f) / SIZE);
        float cellH = 20f;

        for (int row = 0; row < SIZE; row++)
        {
            for (int col = 0; col < SIZE; col++)
            {
                int idx = row * SIZE + col;
                Rect r = new Rect(g.x + col * cellW, g.y + row * cellH, cellW, cellH);

                if (idx == CENTER_INDEX)
                    EditorGUI.LabelField(r, "P", EditorStyles.centeredGreyMiniLabel);
                else
                {
                    var sp = movementMaskProp.GetArrayElementAtIndex(idx);
                    sp.boolValue = EditorGUI.Toggle(r, sp.boolValue);
                }
            }
        }
    }

    void ApplyAll(bool v)
    {
        for (int i = 0; i < 25; i++)
        {
            if (i == CENTER_INDEX) continue;
            movementMaskProp.GetArrayElementAtIndex(i).boolValue = v;
        }
    }

    void InvertAll()
    {
        for (int i = 0; i < 25; i++)
        {
            if (i == CENTER_INDEX) continue;
            var sp = movementMaskProp.GetArrayElementAtIndex(i);
            sp.boolValue = !sp.boolValue;
        }
    }
}
#endif
