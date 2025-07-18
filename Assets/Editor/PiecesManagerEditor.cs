using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PiecesManager))]
public class PiecesManagerEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if(GUILayout.Button("Position Pieces")){
            GameObject.FindFirstObjectByType<PiecesManager>().InitialPosition();
        }
    }
}
