using Dissertation.PCG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomGraphDungeonGenerator))]
public sealed class RoomGraphDungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12f);

        RoomGraphDungeonGenerator generator = (RoomGraphDungeonGenerator)target;

        GUIContent generateContent = new GUIContent(
            "Generate Room Graph Dungeon",
            "Gera uma nova dungeon usando o algoritmo Room Graph puro.");
        GUIContent testContent = new GUIContent(
            "Run Room Graph Measurement Test",
            "Executa varias seeds e exporta o relatorio dos parametros mensuraveis para Room Graph.");
        GUIContent mapContent = new GUIContent(
            "Export 2D Map",
            "Exporta um PNG 2D por pavimento da ultima dungeon Room Graph gerada.");
        GUIContent clearContent = new GUIContent(
            "Clear Dungeon",
            "Remove da cena a dungeon gerada anteriormente.");

        if (GUILayout.Button(generateContent, GUILayout.Height(34f)))
        {
            generator.GenerateDungeon();
        }

        if (GUILayout.Button(testContent, GUILayout.Height(34f)))
        {
            generator.RunMeasurementTest();
        }

        if (GUILayout.Button(mapContent, GUILayout.Height(28f)))
        {
            generator.ExportCurrent2DMap();
        }

        if (GUILayout.Button(clearContent, GUILayout.Height(24f)))
        {
            generator.ClearDungeon();
        }
    }
}
