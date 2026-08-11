using Dissertation.PCG;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellularAutomataDungeonGenerator))]
public sealed class CellularAutomataDungeonGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(12f);

        CellularAutomataDungeonGenerator generator = (CellularAutomataDungeonGenerator)target;

        GUIContent generateContent = new GUIContent(
            "Generate Cellular Automata Dungeon",
            "Gera uma dungeon usando Cellular Automata puro: ruido inicial e regras locais de nascimento/sobrevivencia.");
        GUIContent presetContent = new GUIContent(
            "Apply Balanced Cave Preset",
            "Aplica valores mais equilibrados para conectar melhor as manchas sem transformar quase todo o mapa em chao.");
        GUIContent testContent = new GUIContent(
            "Run Cellular Automata Measurement Test",
            "Executa varias seeds e exporta o relatorio dos parametros mensuraveis para Cellular Automata.");
        GUIContent mapContent = new GUIContent(
            "Export 2D Map",
            "Exporta um PNG 2D por pavimento da ultima dungeon Cellular Automata gerada.");
        GUIContent clearContent = new GUIContent(
            "Clear Dungeon",
            "Remove da cena a dungeon gerada anteriormente.");

        if (GUILayout.Button(generateContent, GUILayout.Height(34f)))
        {
            generator.GenerateDungeon();
        }

        if (GUILayout.Button(presetContent, GUILayout.Height(28f)))
        {
            Undo.RecordObject(generator, "Apply Balanced Cave Preset");
            generator.ApplyBalancedCavePreset();
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
