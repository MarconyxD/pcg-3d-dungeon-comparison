using Dissertation.PCG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DrunkardWalkDungeonGenerator))]
public sealed class DrunkardWalkDungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12f);

        DrunkardWalkDungeonGenerator generator = (DrunkardWalkDungeonGenerator)target;

        GUIContent generateContent = new GUIContent(
            "Generate Drunkard Walk Dungeon",
            "Gera uma dungeon usando Drunkard Walk puro: caminhantes aleatorios escavam celulas do grid.");
        GUIContent presetContent = new GUIContent(
            "Apply Balanced Walk Preset",
            "Aplica valores equilibrados para um passeio conectado, com corredores largos e algumas areas abertas.");
        GUIContent testContent = new GUIContent(
            "Run Drunkard Walk Measurement Test",
            "Executa varias seeds e exporta o relatorio dos parametros mensuraveis para Drunkard Walk.");
        GUIContent mapContent = new GUIContent(
            "Export 2D Map",
            "Exporta um PNG 2D por pavimento da ultima dungeon Drunkard Walk gerada.");
        GUIContent clearContent = new GUIContent(
            "Clear Dungeon",
            "Remove da cena a dungeon gerada anteriormente.");

        if (GUILayout.Button(generateContent, GUILayout.Height(34f)))
        {
            generator.GenerateDungeon();
        }

        if (GUILayout.Button(presetContent, GUILayout.Height(28f)))
        {
            Undo.RecordObject(generator, "Apply Balanced Walk Preset");
            generator.ApplyBalancedWalkPreset();
            EditorUtility.SetDirty(generator);
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

