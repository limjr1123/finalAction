#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameDataSaveLoadManager))]
public class GameDataSaveLoadManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GameDataSaveLoadManager mgr = (GameDataSaveLoadManager)target;
        if (GUILayout.Button("더미 데이터 생성"))
        {
            mgr.CreateDummyData();
        }
    }
}
#endif