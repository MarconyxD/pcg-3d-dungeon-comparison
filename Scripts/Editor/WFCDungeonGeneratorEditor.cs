using Dissertation.PCG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WFCDungeonGenerator))]
public sealed class WFCDungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12f);

        WFCDungeonGenerator generator = (WFCDungeonGenerator)target;

        GUIContent generateContent = new GUIContent(
            "Generate WFC Dungeon",
            "Gera uma nova dungeon usando Wave Function Collapse puro, com tiles e sockets de compatibilidade.");
        GUIContent testContent = new GUIContent(
            "Run WFC Measurement Test",
            "Executa varias seeds e exporta o relatorio dos parametros mensuraveis para WFC.");
        GUIContent mapContent = new GUIContent(
            "Export 2D Map",
            "Exporta um PNG 2D por pavimento da ultima dungeon WFC gerada.");
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
