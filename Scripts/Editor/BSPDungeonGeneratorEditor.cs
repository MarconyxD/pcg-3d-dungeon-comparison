using Dissertation.PCG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BSPDungeonGenerator))]
public sealed class BSPDungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12f);

        BSPDungeonGenerator generator = (BSPDungeonGenerator)target;

        GUIContent generateContent = new GUIContent(
            "Generate Dungeon",
            "Gera uma nova dungeon usando os parâmetros atuais do componente.");
        GUIContent clearContent = new GUIContent(
            "Clear Dungeon",
            "Remove da cena a dungeon gerada anteriormente.");
        GUIContent testContent = new GUIContent(
            "Run Measurement Test",
            "Executa varias seeds e exporta um relatorio com todos os parametros mensuraveis.");
        GUIContent mapContent = new GUIContent(
            "Export 2D Map",
            "Exporta um PNG 2D por pavimento da ultima dungeon gerada, com simbolos e legenda.");

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
