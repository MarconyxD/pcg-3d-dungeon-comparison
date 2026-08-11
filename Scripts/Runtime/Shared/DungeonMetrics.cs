using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Dissertation.PCG
{
    [Serializable]
    public sealed class DungeonBooleanCapabilities
    {
        public bool SupportsRandomEnemySpawns;
        public bool SupportsLootDistribution;
        public bool SupportsTraps;
        public bool SupportsBacktrackingLoops;
        public bool SupportsVerticalConnectors;
        public bool SupportsMultiFloor;
        public bool SupportsBossArena;
        public bool SeedReproducible;
        public bool RuntimeRegeneration;
        public bool BudgetAwareSpawns;
    }

    [Serializable]
    public sealed class DungeonQualitativeScores
    {
        public int Replayability;
        public int Debuggability;
        public int Flow;
        public int Legibility;
        public int StructuralVariety;
        public string note = "Scores use Likert 1-5. Value 0 means not manually evaluated yet.";
    }

    [Serializable]
    public sealed class DungeonMetrics
    {
        public string algorithmName;
        public int seed;
        public string generatedAtUtc;
        public float generationMilliseconds;
        public float layoutGenerationMilliseconds;
        public float geometryInstantiationMilliseconds;
        public float metricsCalculationMilliseconds;
        public float totalGenerationMilliseconds;

        public int numRoomsTarget;
        public int floorCount;
        public int verticalConnectorCount;
        public int occupiedCellCount;
        public int connectionCount;
        public int generatedGameObjectCount;
        public float managedMemoryBeforeMB;
        public float managedMemoryAfterMB;
        public float managedMemoryDeltaKB;
        public float connectivityRatio;
        public float verticalVariance;
        public float fillPercentage;
        public float branchFactor;
        public float avgPathLength;
        public int uniqueModules;
        public float navigableVolumeRatio;
        public float criticalPathLength;
        public float avgAlternativePathLength;

        public int spawnableRoomCells;
        public int enemyBudgetTarget;
        public int lootBudgetTarget;
        public int trapBudgetTarget;
        public int propsSpawned;
        public int enemiesSpawned;
        public int lootSpawned;
        public int trapsSpawned;

        public DungeonBooleanCapabilities booleans = new DungeonBooleanCapabilities();
        public DungeonQualitativeScores qualitative = new DungeonQualitativeScores();
    }

    public static class DungeonMetricsCalculator
    {
        public static DungeonMetrics Calculate(
            DungeonLayout layout,
            int seed,
            string algorithmName,
            float tileSize,
            int uniqueModuleCount,
            bool supportsVerticalConnectors,
            bool supportsMultiFloor,
            int bossArenaMinAreaCells)
        {
            DungeonMetrics metrics = new DungeonMetrics();
            metrics.algorithmName = algorithmName;
            metrics.seed = seed;
            metrics.generatedAtUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);

            metrics.numRoomsTarget = layout.rooms.Count;
            metrics.floorCount = layout.floorCount;
            metrics.verticalConnectorCount = CountVerticalConnections(layout);
            metrics.fillPercentage = CalculateFillPercentage(layout);
            metrics.connectivityRatio = CalculateConnectivityRatio(layout);
            metrics.branchFactor = CalculateBranchFactor(layout);
            metrics.verticalVariance = CalculateVerticalVariance(layout);
            metrics.uniqueModules = uniqueModuleCount;
            metrics.navigableVolumeRatio = layout.CountOccupiedCells() > 0 ? 100f : 0f;

            Dictionary<int, float> distances = CalculateGraphDistances(layout, tileSize);
            metrics.avgPathLength = CalculateAveragePathLength(distances, layout.startRoomId);
            metrics.criticalPathLength = CalculateCriticalPathLength(distances);
            metrics.avgAlternativePathLength = CalculateAverageAlternativePathLength(layout, tileSize);

            metrics.booleans.SupportsRandomEnemySpawns = true;
            metrics.booleans.SupportsLootDistribution = true;
            metrics.booleans.SupportsTraps = true;
            metrics.booleans.SupportsBacktrackingLoops = HasBacktrackingLoop(layout);
            metrics.booleans.SupportsVerticalConnectors = supportsVerticalConnectors;
            metrics.booleans.SupportsMultiFloor = supportsMultiFloor;
            metrics.booleans.SupportsBossArena = HasBossArena(layout, bossArenaMinAreaCells);
            metrics.booleans.SeedReproducible = true;
            metrics.booleans.RuntimeRegeneration = true;
            metrics.booleans.BudgetAwareSpawns = true;

            return metrics;
        }

        private static float CalculateFillPercentage(DungeonLayout layout)
        {
            int totalCells = layout.width * layout.depth * layout.floorCount;
            if (totalCells == 0)
            {
                return 0f;
            }

            return layout.CountOccupiedCells() * 100f / totalCells;
        }

        private static int CountVerticalConnections(DungeonLayout layout)
        {
            int count = 0;
            for (int i = 0; i < layout.connections.Count; i++)
            {
                if (layout.connections[i].isVertical)
                {
                    count++;
                }
            }

            return count;
        }

        private static float CalculateConnectivityRatio(DungeonLayout layout)
        {
            if (layout.rooms.Count == 0)
            {
                return 0f;
            }

            HashSet<int> visited = new HashSet<int>();
            Queue<int> queue = new Queue<int>();
            int start = layout.startRoomId >= 0 ? layout.startRoomId : layout.rooms[0].id;

            visited.Add(start);
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();
                for (int i = 0; i < layout.connections.Count; i++)
                {
                    DungeonConnection connection = layout.connections[i];
                    int next = -1;
                    if (connection.roomAId == current) next = connection.roomBId;
                    if (connection.roomBId == current) next = connection.roomAId;

                    if (next >= 0 && !visited.Contains(next))
                    {
                        visited.Add(next);
                        queue.Enqueue(next);
                    }
                }
            }

            return visited.Count * 100f / layout.rooms.Count;
        }

        private static float CalculateBranchFactor(DungeonLayout layout)
        {
            if (layout.rooms.Count == 0)
            {
                return 0f;
            }

            int totalDegree = 0;
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                int roomId = layout.rooms[i].id;
                for (int c = 0; c < layout.connections.Count; c++)
                {
                    DungeonConnection connection = layout.connections[c];
                    if (connection.roomAId == roomId || connection.roomBId == roomId)
                    {
                        totalDegree++;
                    }
                }
            }

            return (float)totalDegree / layout.rooms.Count;
        }

        private static float CalculateVerticalVariance(DungeonLayout layout)
        {
            if (layout.rooms.Count == 0)
            {
                return 0f;
            }

            float mean = 0f;
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                mean += layout.rooms[i].floorIndex * layout.floorHeight;
            }
            mean /= layout.rooms.Count;

            float sumSquares = 0f;
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                float height = layout.rooms[i].floorIndex * layout.floorHeight;
                float delta = height - mean;
                sumSquares += delta * delta;
            }

            return Mathf.Sqrt(sumSquares / layout.rooms.Count);
        }

        private static Dictionary<int, float> CalculateGraphDistances(DungeonLayout layout, float tileSize)
        {
            Dictionary<int, float> distances = new Dictionary<int, float>();
            List<int> unresolved = new List<int>();

            for (int i = 0; i < layout.rooms.Count; i++)
            {
                int roomId = layout.rooms[i].id;
                distances[roomId] = float.PositiveInfinity;
                unresolved.Add(roomId);
            }

            if (layout.startRoomId < 0 || !distances.ContainsKey(layout.startRoomId))
            {
                return distances;
            }

            distances[layout.startRoomId] = 0f;

            while (unresolved.Count > 0)
            {
                int bestIndex = -1;
                float bestDistance = float.PositiveInfinity;
                for (int i = 0; i < unresolved.Count; i++)
                {
                    float distance = distances[unresolved[i]];
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestIndex = i;
                    }
                }

                if (bestIndex < 0 || float.IsPositiveInfinity(bestDistance))
                {
                    break;
                }

                int current = unresolved[bestIndex];
                unresolved.RemoveAt(bestIndex);

                for (int i = 0; i < layout.connections.Count; i++)
                {
                    DungeonConnection connection = layout.connections[i];
                    int next = -1;
                    if (connection.roomAId == current) next = connection.roomBId;
                    if (connection.roomBId == current) next = connection.roomAId;

                    if (next < 0 || !distances.ContainsKey(next))
                    {
                        continue;
                    }

                    DungeonRoom roomA = layout.GetRoomById(connection.roomAId);
                    DungeonRoom roomB = layout.GetRoomById(connection.roomBId);
                    float verticalCost = 0f;
                    if (roomA != null && roomB != null)
                    {
                        verticalCost = Mathf.Abs(roomA.floorIndex - roomB.floorIndex) * layout.floorHeight;
                    }

                    float edgeCost = Mathf.Max(1f, connection.gridDistance) * tileSize + verticalCost;
                    float newDistance = distances[current] + edgeCost;
                    if (newDistance < distances[next])
                    {
                        distances[next] = newDistance;
                    }
                }
            }

            return distances;
        }

        private static float CalculateAveragePathLength(Dictionary<int, float> distances, int startRoomId)
        {
            float sum = 0f;
            int count = 0;
            foreach (KeyValuePair<int, float> pair in distances)
            {
                if (pair.Key == startRoomId || float.IsInfinity(pair.Value))
                {
                    continue;
                }

                sum += pair.Value;
                count++;
            }

            return count == 0 ? 0f : sum / count;
        }

        private static float CalculateCriticalPathLength(Dictionary<int, float> distances)
        {
            float max = 0f;
            foreach (KeyValuePair<int, float> pair in distances)
            {
                if (!float.IsInfinity(pair.Value) && pair.Value > max)
                {
                    max = pair.Value;
                }
            }

            return max;
        }

        private static float CalculateAverageAlternativePathLength(DungeonLayout layout, float tileSize)
        {
            float sum = 0f;
            int count = 0;
            for (int i = 0; i < layout.connections.Count; i++)
            {
                DungeonConnection connection = layout.connections[i];
                if (!connection.isExtraLoop)
                {
                    continue;
                }

                sum += Mathf.Max(1f, connection.gridDistance) * tileSize;
                count++;
            }

            return count == 0 ? 0f : sum / count;
        }

        private static bool HasBacktrackingLoop(DungeonLayout layout)
        {
            int edgeCount = layout.connections.Count;
            int nodeCount = layout.rooms.Count;
            return nodeCount > 0 && edgeCount >= nodeCount;
        }

        private static bool HasBossArena(DungeonLayout layout, int bossArenaMinAreaCells)
        {
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                if (layout.rooms[i].AreaCells >= bossArenaMinAreaCells)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static class DungeonMetricsExporter
    {
        public static void ExportJsonAndCsv(DungeonMetrics metrics, string folderPath, string filePrefix)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string safePrefix = string.IsNullOrEmpty(filePrefix) ? "dungeon_metrics" : filePrefix;
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            string jsonPath = Path.Combine(folderPath, safePrefix + "_" + timestamp + ".json");
            string csvPath = Path.Combine(folderPath, safePrefix + "_" + timestamp + ".csv");

            File.WriteAllText(jsonPath, JsonUtility.ToJson(metrics, true));
            File.WriteAllText(csvPath, ToCsv(metrics));

            Debug.Log("PCG metrics exported to: " + jsonPath + " and " + csvPath);
        }

        private static string ToCsv(DungeonMetrics metrics)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("algorithmName,seed,generatedAtUtc,generationMilliseconds,layoutGenerationMilliseconds,geometryInstantiationMilliseconds,metricsCalculationMilliseconds,totalGenerationMilliseconds,numRoomsTarget,floorCount,verticalConnectorCount,occupiedCellCount,connectionCount,generatedGameObjectCount,managedMemoryBeforeMB,managedMemoryAfterMB,managedMemoryDeltaKB,connectivityRatio,verticalVariance,fillPercentage,branchFactor,avgPathLength,uniqueModules,navigableVolumeRatio,criticalPathLength,avgAlternativePathLength,spawnableRoomCells,enemyBudgetTarget,lootBudgetTarget,trapBudgetTarget,propsSpawned,enemiesSpawned,lootSpawned,trapsSpawned,SupportsRandomEnemySpawns,SupportsLootDistribution,SupportsTraps,SupportsBacktrackingLoops,SupportsVerticalConnectors,SupportsMultiFloor,SupportsBossArena,SeedReproducible,RuntimeRegeneration,BudgetAwareSpawns,Replayability,Debuggability,Flow,Legibility,StructuralVariety");
            builder.Append(Escape(metrics.algorithmName)).Append(",");
            builder.Append(metrics.seed).Append(",");
            builder.Append(Escape(metrics.generatedAtUtc)).Append(",");
            builder.Append(Float(metrics.generationMilliseconds)).Append(",");
            builder.Append(Float(metrics.layoutGenerationMilliseconds)).Append(",");
            builder.Append(Float(metrics.geometryInstantiationMilliseconds)).Append(",");
            builder.Append(Float(metrics.metricsCalculationMilliseconds)).Append(",");
            builder.Append(Float(metrics.totalGenerationMilliseconds)).Append(",");
            builder.Append(metrics.numRoomsTarget).Append(",");
            builder.Append(metrics.floorCount).Append(",");
            builder.Append(metrics.verticalConnectorCount).Append(",");
            builder.Append(metrics.occupiedCellCount).Append(",");
            builder.Append(metrics.connectionCount).Append(",");
            builder.Append(metrics.generatedGameObjectCount).Append(",");
            builder.Append(Float(metrics.managedMemoryBeforeMB)).Append(",");
            builder.Append(Float(metrics.managedMemoryAfterMB)).Append(",");
            builder.Append(Float(metrics.managedMemoryDeltaKB)).Append(",");
            builder.Append(Float(metrics.connectivityRatio)).Append(",");
            builder.Append(Float(metrics.verticalVariance)).Append(",");
            builder.Append(Float(metrics.fillPercentage)).Append(",");
            builder.Append(Float(metrics.branchFactor)).Append(",");
            builder.Append(Float(metrics.avgPathLength)).Append(",");
            builder.Append(metrics.uniqueModules).Append(",");
            builder.Append(Float(metrics.navigableVolumeRatio)).Append(",");
            builder.Append(Float(metrics.criticalPathLength)).Append(",");
            builder.Append(Float(metrics.avgAlternativePathLength)).Append(",");
            builder.Append(metrics.spawnableRoomCells).Append(",");
            builder.Append(metrics.enemyBudgetTarget).Append(",");
            builder.Append(metrics.lootBudgetTarget).Append(",");
            builder.Append(metrics.trapBudgetTarget).Append(",");
            builder.Append(metrics.propsSpawned).Append(",");
            builder.Append(metrics.enemiesSpawned).Append(",");
            builder.Append(metrics.lootSpawned).Append(",");
            builder.Append(metrics.trapsSpawned).Append(",");
            builder.Append(Bool(metrics.booleans.SupportsRandomEnemySpawns)).Append(",");
            builder.Append(Bool(metrics.booleans.SupportsLootDistribution)).Append(",");
            builder.Append(Bool(metrics.booleans.SupportsTraps)).Append(",");
            builder.Append(Bool(metrics.booleans.SupportsBacktrackingLoops)).Append(",");
            builder.Append(Bool(metrics.booleans.SupportsVerticalConnectors)).Append(",");
            builder.Append(Bool(metrics.booleans.SupportsMultiFloor)).Append(",");
            builder.Append(Bool(metrics.booleans.SupportsBossArena)).Append(",");
            builder.Append(Bool(metrics.booleans.SeedReproducible)).Append(",");
            builder.Append(Bool(metrics.booleans.RuntimeRegeneration)).Append(",");
            builder.Append(Bool(metrics.booleans.BudgetAwareSpawns)).Append(",");
            builder.Append(metrics.qualitative.Replayability).Append(",");
            builder.Append(metrics.qualitative.Debuggability).Append(",");
            builder.Append(metrics.qualitative.Flow).Append(",");
            builder.Append(metrics.qualitative.Legibility).Append(",");
            builder.Append(metrics.qualitative.StructuralVariety);
            builder.AppendLine();
            return builder.ToString();
        }

        private static string Float(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static string Bool(bool value)
        {
            return value ? "1" : "0";
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
