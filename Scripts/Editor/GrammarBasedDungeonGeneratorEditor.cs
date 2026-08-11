using Dissertation.PCG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GrammarBasedDungeonGenerator))]
public sealed class GrammarBasedDungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12f);

        GrammarBasedDungeonGenerator generator = (GrammarBasedDungeonGenerator)target;

        GUIContent generateContent = new GUIContent(
            "Generate Grammar-Based Dungeon",
            "Gera uma nova dungeon usando o algoritmo Grammar-Based Generation puro.");
        GUIContent testContent = new GUIContent(
            "Run Grammar-Based Measurement Test",
            "Executa varias seeds e exporta o relatorio dos parametros mensuraveis para Grammar-Based.");
        GUIContent mapContent = new GUIContent(
            "Export 2D Map",
            "Exporta um PNG 2D por pavimento da ultima dungeon Grammar-Based gerada.");
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

