using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dissertation.PCG
{
    public sealed class RoomGraphDungeonGenerator : MonoBehaviour
    {
        [Header("Generation")]
        [Tooltip("Valor usado para gerar sempre a mesma dungeon. Mantendo a mesma seed, o resultado deve ser reproduzivel.")]
        public int seed = 12345;
        [Tooltip("Quando ativo, ignora a seed fixa e cria uma seed nova a cada geracao.")]
        public bool randomizeSeed;
        [Tooltip("Quando ativo, gera a dungeon automaticamente ao iniciar a cena.")]
        public bool generateOnStart = true;
        [Tooltip("Quando ativo, remove a dungeon anterior antes de gerar uma nova.")]
        public bool clearBeforeGenerate = true;
        [Tooltip("Largura do grid logico da dungeon, em celulas.")]
        public int mapWidth = 64;
        [Tooltip("Profundidade do grid logico da dungeon, em celulas.")]
        public int mapDepth = 64;
        [Tooltip("Menor tamanho permitido para uma sala, em celulas.")]
        public int minRoomSize = 5;
        [Tooltip("Maior tamanho permitido para uma sala, em celulas.")]
        public int maxRoomSize = 12;
        [Tooltip("Numero maximo de salas que o grafo tentara posicionar por pavimento.")]
        public int maxRooms = 24;
        [Tooltip("Espaco minimo entre retangulos de salas durante a etapa de embedding do grafo.")]
        public int roomPadding = 1;
        [Tooltip("Quantidade de tentativas de posicionamento para cada no/sala do grafo.")]
        public int roomPlacementAttempts = 350;
        [Tooltip("Largura dos corredores gerados a partir das arestas do grafo, em celulas.")]
        public int corridorWidth = 3;
        [Tooltip("Folga adicional nas aberturas entre sala e corredor.")]
        public int doorwayExtraClearance = 1;
        [Tooltip("Numero de arestas extras adicionadas ao grafo para gerar loops e backtracking.")]
        public int extraLoopConnections = 2;
        [Tooltip("Distancia maxima, em celulas, para permitir uma aresta extra entre duas salas.")]
        public float maxExtraLoopDistance = 26f;

        [Header("Pure Room Graph verticality")]
        [Tooltip("Quando ativo, usa arestas verticais do proprio Room Graph para conectar pavimentos.")]
        public bool enableMultiFloorRoomGraph = true;
        [Tooltip("Quantidade de pavimentos gerados pelo grafo de salas.")]
        public int floorCount = 2;
        [Tooltip("Quantidade de arestas verticais entre cada par de pavimentos adjacentes.")]
        public int verticalConnectionsPerFloorPair = 1;
        [Tooltip("Distancia horizontal maxima desejada entre salas de pavimentos adjacentes para criar uma escada. Se nenhuma sala estiver dentro do raio, o par mais proximo e usado.")]
        public float verticalConnectorSearchRadius = 10f;
        [Tooltip("Raio, em celulas, da abertura criada no piso superior ao redor da chegada da escada.")]
        public int verticalOpeningRadius = 1;
        [Tooltip("Deslocamento da abertura do piso superior na direcao frontal da escada.")]
        public int verticalOpeningForwardOffsetCells = 1;

        [Header("3D placement")]
        [Tooltip("Biblioteca com os prefabs do KayKit ou de outro pacote modular usado para montar a dungeon.")]
        public DungeonAssetLibrary assetLibrary;
        [Tooltip("Quando ativo, instancia os objetos 3D. Quando inativo, apenas gera o layout logico e as metricas.")]
        public bool instantiateGeometry = true;
        [Tooltip("Centraliza a dungeon em torno da origem da cena.")]
        public bool centerOnOrigin = true;
        [Tooltip("Tamanho de cada celula do grid em unidades Unity.")]
        public float tileSize = 2f;
        [Tooltip("Altura entre pavimentos em unidades Unity.")]
        public float floorHeight = 4f;
        [Tooltip("Altura usada pelas paredes primitivas de fallback.")]
        public float wallHeight = 3f;
        [Tooltip("Espessura usada pelas paredes primitivas de fallback.")]
        public float wallThickness = 0.25f;
        [Tooltip("Correcao de rotacao das paredes em graus.")]
        public float wallYawOffset;
        [Tooltip("Deslocamento vertical aplicado aos prefabs de parede.")]
        public float wallYOffset;
        [Tooltip("Escala aplicada aos prefabs instanciados pelo gerador.")]
        public Vector3 prefabInstanceScale = Vector3.one;

        [Header("Fallback primitives")]
        [Tooltip("Quando ativo, usa cubos simples caso os prefabs KayKit nao estejam configurados.")]
        public bool usePrimitiveFallbacks = true;
        [Tooltip("Material aplicado aos pisos primitivos de fallback.")]
        public Material fallbackFloorMaterial;
        [Tooltip("Material aplicado as paredes primitivas de fallback.")]
        public Material fallbackWallMaterial;

        [Header("Semantic spawns")]
        [Tooltip("Probabilidade de cada sala receber objetos decorativos da lista de props.")]
        [Range(0f, 1f)] public float propRoomChance = 0.75f;
        [Tooltip("Quantidade minima de objetos decorativos por sala selecionada.")]
        public int minPropsPerRoom = 0;
        [Tooltip("Quantidade maxima de objetos decorativos por sala selecionada.")]
        public int maxPropsPerRoom = 3;
        [Tooltip("Orcamento total de inimigos que o gerador tentara posicionar na dungeon.")]
        public int enemyBudget = 0;
        [Tooltip("Orcamento total de itens/recompensas que o gerador tentara posicionar na dungeon.")]
        public int lootBudget = 0;
        [Tooltip("Orcamento total de armadilhas que o gerador tentara posicionar na dungeon.")]
        public int trapBudget = 0;

        [Header("Metrics")]
        [Tooltip("Quando ativo, exporta automaticamente as metricas da dungeon em JSON e CSV a cada geracao.")]
        public bool exportMetricsOnGenerate = true;
        [Tooltip("Nome da pasta, dentro de Application.persistentDataPath, onde os arquivos de metricas serao salvos.")]
        public string metricsFolderName = "PCGMetrics";
        [Tooltip("Prefixo usado no nome dos arquivos exportados.")]
        public string metricsFilePrefix = "room_graph";
        [Tooltip("Area minima, em celulas, para considerar uma sala como possivel arena de chefe.")]
        public int bossArenaMinAreaCells = 80;

        [Header("2D map export")]
        [Tooltip("Quando ativo, exporta automaticamente um mapa 2D em PNG ao clicar em Generate Dungeon.")]
        public bool export2DMapOnGenerate = true;
        [Tooltip("Quando ativo, exporta mapas 2D para cada seed durante Run Measurement Test.")]
        public bool export2DMapsDuringMeasurementTest;
        [Tooltip("Quando ativo, exporta o mapa 2D da ultima dungeon instanciada apos Run Measurement Test.")]
        public bool export2DMapForLastTestDungeon = true;
        [Tooltip("Subpasta criada dentro da pasta de metricas para armazenar os mapas 2D.")]
        public string mapExportSubfolderName = "Maps";
        [Tooltip("Tamanho, em pixels, de cada celula do grid logico no mapa exportado.")]
        public int mapPixelsPerCell = 10;
        [Tooltip("Quando ativo, desenha linhas finas entre celulas.")]
        public bool mapIncludeGrid = true;
        [Tooltip("Quando ativo, adiciona uma legenda lateral.")]
        public bool mapIncludeLegend = true;

        [Header("Automated Tests")]
        [Tooltip("Quantidade de execucoes usadas no teste comparativo por multiplas seeds.")]
        public int testRunCount = 10;
        [Tooltip("Primeira seed usada no teste. As execucoes seguintes usam seed + 1, seed + 2 e assim por diante.")]
        public int testFirstSeed = 1000;
        [Tooltip("Tempo maximo, em milissegundos, para considerar que a regeneracao em runtime foi atendida.")]
        public float runtimeRegenerationMaxMilliseconds = 250f;
        [Tooltip("Quando ativo, apos o teste, instancia na cena a ultima dungeon testada.")]
        public bool instantiateLastTestDungeon = true;
        [Tooltip("Quando ativo, mede tambem o custo de instanciar prefabs em cada seed do teste.")]
        public bool measureVisualInstantiationInTests;

        public DungeonLayout LastLayout { get; private set; }
        public DungeonMetrics LastMetrics { get; private set; }
        public DungeonBatchReport LastBatchReport { get; private set; }

        private readonly HashSet<string> uniqueModules = new HashSet<string>();
        private readonly HashSet<string> openWallEdges = new HashSet<string>();
        private readonly HashSet<string> reservedSpawnCells = new HashSet<string>();
        private readonly HashSet<string> floorOpeningCells = new HashSet<string>();
        private System.Random rng;
        private Transform dungeonRoot;
        private int resolvedSeed;
        private int propsSpawned;
        private int enemiesSpawned;
        private int lootSpawned;
        private int trapsSpawned;
        private int generatedGameObjectCount;

        private static readonly Vector2Int[] CardinalDirections =
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateDungeon();
            }
        }

        [ContextMenu("Generate Room Graph Dungeon")]
        public void GenerateDungeon()
        {
            int selectedSeed = randomizeSeed ? System.Environment.TickCount : seed;
            GenerateForSeed(selectedSeed, instantiateGeometry, clearBeforeGenerate, true);
        }

        [ContextMenu("Run Room Graph Measurement Test")]
        public void RunMeasurementTest()
        {
            if (clearBeforeGenerate)
            {
                ClearDungeon();
            }

            int safeRunCount = Mathf.Max(1, testRunCount);
            bool seedReproducible = VerifySeedReproducibility(testFirstSeed);
            DungeonReportContext context = CreateReportContext(seedReproducible, safeRunCount, 0, 0f);
            DungeonBatchReport report = new DungeonBatchReport();
            report.algorithmName = "Room Graph";
            report.generatedAtUtc = System.DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
            report.runCount = safeRunCount;

            HashSet<string> uniqueHashes = new HashSet<string>();
            for (int i = 0; i < safeRunCount; i++)
            {
                int runSeed = testFirstSeed + i;
                DungeonMetrics metrics = GenerateForSeed(runSeed, measureVisualInstantiationInTests, measureVisualInstantiationInTests, false);
                string topologyHash = DungeonTopologyHasher.Compute(LastLayout);
                uniqueHashes.Add(topologyHash);

                if (export2DMapsDuringMeasurementTest)
                {
                    ExportCurrent2DMaps("run_" + (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture));
                }

                DungeonRunReport runReport = new DungeonRunReport();
                runReport.runIndex = i + 1;
                runReport.seed = runSeed;
                runReport.topologyHash = topologyHash;
                runReport.metrics = metrics;
                runReport.parameters = RoomGraphParameterNotes.CreateRunResults(metrics, context);
                report.runs.Add(runReport);
            }

            float diversityRatio = safeRunCount == 0 ? 0f : (float)uniqueHashes.Count / safeRunCount;
            context = CreateReportContext(seedReproducible, safeRunCount, uniqueHashes.Count, diversityRatio);
            report.uniqueTopologyCount = uniqueHashes.Count;
            report.topologyDiversityRatio = diversityRatio;

            for (int i = 0; i < report.runs.Count; i++)
            {
                DungeonQualitativeScorer.ApplyScores(report.runs[i].metrics, diversityRatio, safeRunCount);
                report.runs[i].parameters = RoomGraphParameterNotes.CreateRunResults(report.runs[i].metrics, context);
            }

            report.aggregateParameters = RoomGraphParameterNotes.CreateAggregateResults(report.runs, context);
            report.summary = BuildBatchSummary(report, seedReproducible);
            LastBatchReport = report;

            string folder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            DungeonReportPaths paths = DungeonReportExporter.ExportBatchReport(report, folder, metricsFilePrefix);
            Debug.Log("Room Graph parameter test exported. Markdown: " + paths.markdownPath + " JSON: " + paths.jsonPath);

            if (instantiateLastTestDungeon)
            {
                GenerateForSeed(testFirstSeed + safeRunCount - 1, instantiateGeometry, true, false);
                if (export2DMapForLastTestDungeon)
                {
                    ExportCurrent2DMaps("last_test_seed");
                }
            }
        }

        [ContextMenu("Export 2D Map")]
        public void ExportCurrent2DMap()
        {
            ExportCurrent2DMaps("manual");
        }

        [ContextMenu("Clear Dungeon")]
        public void ClearDungeon()
        {
            if (dungeonRoot == null)
            {
                Transform existing = transform.Find("Generated Room Graph Dungeon");
                if (existing != null)
                {
                    dungeonRoot = existing;
                }
            }

            if (dungeonRoot == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(dungeonRoot.gameObject);
            }
            else
            {
                DestroyImmediate(dungeonRoot.gameObject);
            }

            dungeonRoot = null;
        }

        private DungeonMetrics GenerateForSeed(int selectedSeed, bool shouldInstantiateGeometry, bool shouldClear, bool shouldExportSingleMetrics)
        {
            if (shouldClear)
            {
                ClearDungeon();
            }

            PrepareGenerationState(selectedSeed);

            long memoryBeforeBytes = System.GC.GetTotalMemory(false);
            System.Diagnostics.Stopwatch totalStopwatch = System.Diagnostics.Stopwatch.StartNew();
            System.Diagnostics.Stopwatch layoutStopwatch = System.Diagnostics.Stopwatch.StartNew();

            int safeWidth = Mathf.Max(16, mapWidth);
            int safeDepth = Mathf.Max(16, mapDepth);
            int effectiveFloorCount = enableMultiFloorRoomGraph ? Mathf.Max(2, floorCount) : 1;
            LastLayout = new DungeonLayout(safeWidth, safeDepth, floorHeight, effectiveFloorCount);

            for (int floor = 0; floor < effectiveFloorCount; floor++)
            {
                List<DungeonRoom> floorRooms = CreateRoomNodes(LastLayout, floor);
                ConnectRoomGraphFloor(LastLayout, floorRooms);
            }

            AddExtraLoops(LastLayout);
            AddVerticalConnectors(LastLayout);
            AssignStartAndGoal(LastLayout);
            layoutStopwatch.Stop();

            float geometryMs = 0f;
            if (shouldInstantiateGeometry)
            {
                System.Diagnostics.Stopwatch geometryStopwatch = System.Diagnostics.Stopwatch.StartNew();
                InstantiateLayout(LastLayout);
                geometryStopwatch.Stop();
                geometryMs = (float)geometryStopwatch.Elapsed.TotalMilliseconds;
            }

            System.Diagnostics.Stopwatch metricsStopwatch = System.Diagnostics.Stopwatch.StartNew();
            LastMetrics = CreateMetrics(LastLayout, selectedSeed);
            metricsStopwatch.Stop();
            totalStopwatch.Stop();

            long memoryAfterBytes = System.GC.GetTotalMemory(false);
            ApplyPerformanceMetrics(
                LastMetrics,
                (float)layoutStopwatch.Elapsed.TotalMilliseconds,
                geometryMs,
                (float)metricsStopwatch.Elapsed.TotalMilliseconds,
                (float)totalStopwatch.Elapsed.TotalMilliseconds,
                memoryBeforeBytes,
                memoryAfterBytes);

            if (shouldExportSingleMetrics && exportMetricsOnGenerate)
            {
                string folder = Path.Combine(Application.persistentDataPath, metricsFolderName);
                DungeonMetricsExporter.ExportJsonAndCsv(LastMetrics, folder, metricsFilePrefix);
            }

            if (shouldExportSingleMetrics && export2DMapOnGenerate)
            {
                ExportCurrent2DMaps("single");
            }

            Debug.Log("Room Graph dungeon generated. Rooms: " + LastLayout.rooms.Count + ", seed: " + selectedSeed);
            return LastMetrics;
        }

        private void PrepareGenerationState(int selectedSeed)
        {
            resolvedSeed = selectedSeed;
            rng = new System.Random(resolvedSeed);
            uniqueModules.Clear();
            openWallEdges.Clear();
            reservedSpawnCells.Clear();
            floorOpeningCells.Clear();
            propsSpawned = 0;
            enemiesSpawned = 0;
            lootSpawned = 0;
            trapsSpawned = 0;
            generatedGameObjectCount = 0;
        }

        private List<DungeonRoom> CreateRoomNodes(DungeonLayout layout, int floorIndex)
        {
            List<DungeonRoom> floorRooms = new List<DungeonRoom>();
            int targetRooms = Mathf.Max(1, maxRooms);
            int safeMinRoomSize = Mathf.Max(2, minRoomSize);
            int largestRoomAllowedByMap = Mathf.Max(safeMinRoomSize, Mathf.Min(layout.width - 2, layout.depth - 2));
            int safeMaxRoomSize = Mathf.Min(Mathf.Max(safeMinRoomSize, maxRoomSize), largestRoomAllowedByMap);

            for (int i = 0; i < targetRooms; i++)
            {
                DungeonRoom room;
                if (!TryCreateRoom(layout, floorIndex, safeMinRoomSize, safeMaxRoomSize, out room))
                {
                    continue;
                }

                layout.rooms.Add(room);
                floorRooms.Add(room);
                MarkRoomCells(layout, room);
                uniqueModules.Add("room_graph_room");
            }

            return floorRooms;
        }

        private bool TryCreateRoom(DungeonLayout layout, int floorIndex, int safeMinRoomSize, int safeMaxRoomSize, out DungeonRoom room)
        {
            int attempts = Mathf.Max(1, roomPlacementAttempts);
            int padding = Mathf.Max(0, roomPadding);

            for (int attempt = 0; attempt < attempts; attempt++)
            {
                int width = rng.Next(safeMinRoomSize, safeMaxRoomSize + 1);
                int depth = rng.Next(safeMinRoomSize, safeMaxRoomSize + 1);
                int maxX = Mathf.Max(2, layout.width - width - 1);
                int maxZ = Mathf.Max(2, layout.depth - depth - 1);
                int x = rng.Next(1, maxX);
                int z = rng.Next(1, maxZ);
                RectInt bounds = new RectInt(x, z, width, depth);

                if (OverlapsExistingRoom(layout, bounds, floorIndex, padding))
                {
                    continue;
                }

                room = new DungeonRoom();
                room.id = layout.rooms.Count;
                room.floorIndex = floorIndex;
                room.bounds = bounds;
                room.moduleId = "room_graph_node";
                return true;
            }

            room = null;
            return false;
        }

        private static bool OverlapsExistingRoom(DungeonLayout layout, RectInt bounds, int floorIndex, int padding)
        {
            RectInt padded = new RectInt(bounds.xMin - padding, bounds.yMin - padding, bounds.width + padding * 2, bounds.height + padding * 2);
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                DungeonRoom existing = layout.rooms[i];
                if (existing.floorIndex != floorIndex)
                {
                    continue;
                }

                if (padded.Overlaps(existing.bounds))
                {
                    return true;
                }
            }

            return false;
        }

        private static void MarkRoomCells(DungeonLayout layout, DungeonRoom room)
        {
            for (int x = room.bounds.xMin; x < room.bounds.xMax; x++)
            {
                for (int z = room.bounds.yMin; z < room.bounds.yMax; z++)
                {
                    layout.MarkCell(x, z, room.floorIndex, DungeonCellKind.Room);
                }
            }
        }

        private void ConnectRoomGraphFloor(DungeonLayout layout, List<DungeonRoom> floorRooms)
        {
            if (floorRooms.Count < 2)
            {
                return;
            }

            List<DungeonRoom> connected = new List<DungeonRoom>();
            List<DungeonRoom> remaining = new List<DungeonRoom>(floorRooms);
            connected.Add(remaining[0]);
            remaining.RemoveAt(0);

            while (remaining.Count > 0)
            {
                DungeonRoom bestA = null;
                DungeonRoom bestB = null;
                float bestDistance = float.PositiveInfinity;

                for (int a = 0; a < connected.Count; a++)
                {
                    for (int b = 0; b < remaining.Count; b++)
                    {
                        float distance = Vector2Int.Distance(connected[a].CenterCell, remaining[b].CenterCell);
                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            bestA = connected[a];
                            bestB = remaining[b];
                        }
                    }
                }

                if (bestA == null || bestB == null)
                {
                    break;
                }

                ConnectRooms(layout, bestA, bestB, false);
                connected.Add(bestB);
                remaining.Remove(bestB);
            }
        }

        private void AddExtraLoops(DungeonLayout layout)
        {
            int added = 0;
            int attempts = 0;
            int target = Mathf.Max(0, extraLoopConnections);
            while (added < target && attempts < Mathf.Max(30, target * 80))
            {
                attempts++;
                if (layout.rooms.Count < 2)
                {
                    return;
                }

                DungeonRoom a = layout.rooms[rng.Next(0, layout.rooms.Count)];
                DungeonRoom b = layout.rooms[rng.Next(0, layout.rooms.Count)];
                if (a.id == b.id || a.floorIndex != b.floorIndex || layout.HasConnection(a.id, b.id))
                {
                    continue;
                }

                float distance = Vector2Int.Distance(a.CenterCell, b.CenterCell);
                if (distance > maxExtraLoopDistance)
                {
                    continue;
                }

                ConnectRooms(layout, a, b, true);
                added++;
            }
        }

        private void AddVerticalConnectors(DungeonLayout layout)
        {
            if (!enableMultiFloorRoomGraph || layout.floorCount < 2 || verticalConnectionsPerFloorPair <= 0)
            {
                return;
            }

            for (int floor = 0; floor < layout.floorCount - 1; floor++)
            {
                int added = 0;
                int targetCount = Mathf.Max(1, verticalConnectionsPerFloorPair);
                while (added < targetCount)
                {
                    DungeonRoom lower;
                    DungeonRoom upper;
                    if (!TryFindVerticalRoomPair(layout, floor, floor + 1, out lower, out upper))
                    {
                        break;
                    }

                    ConnectVerticalRooms(layout, lower, upper);
                    added++;
                }
            }
        }

        private bool TryFindVerticalRoomPair(DungeonLayout layout, int lowerFloor, int upperFloor, out DungeonRoom lower, out DungeonRoom upper)
        {
            lower = null;
            upper = null;
            float bestScore = float.PositiveInfinity;
            float fallbackScore = float.PositiveInfinity;
            DungeonRoom fallbackLower = null;
            DungeonRoom fallbackUpper = null;

            for (int a = 0; a < layout.rooms.Count; a++)
            {
                DungeonRoom candidateLower = layout.rooms[a];
                if (candidateLower.floorIndex != lowerFloor)
                {
                    continue;
                }

                for (int b = 0; b < layout.rooms.Count; b++)
                {
                    DungeonRoom candidateUpper = layout.rooms[b];
                    if (candidateUpper.floorIndex != upperFloor || layout.HasConnection(candidateLower.id, candidateUpper.id))
                    {
                        continue;
                    }

                    float distance = Vector2Int.Distance(candidateLower.CenterCell, candidateUpper.CenterCell);
                    bool overlaps = RectsOverlap(candidateLower.bounds, candidateUpper.bounds);
                    float score = overlaps ? distance - 1000f : distance;

                    if (distance < fallbackScore)
                    {
                        fallbackScore = distance;
                        fallbackLower = candidateLower;
                        fallbackUpper = candidateUpper;
                    }

                    if ((overlaps || distance <= verticalConnectorSearchRadius) && score < bestScore)
                    {
                        bestScore = score;
                        lower = candidateLower;
                        upper = candidateUpper;
                    }
                }
            }

            if (lower != null && upper != null)
            {
                return true;
            }

            lower = fallbackLower;
            upper = fallbackUpper;
            return lower != null && upper != null;
        }

        private void ConnectVerticalRooms(DungeonLayout layout, DungeonRoom roomA, DungeonRoom roomB)
        {
            DungeonRoom lower = roomA.floorIndex <= roomB.floorIndex ? roomA : roomB;
            DungeonRoom upper = lower == roomA ? roomB : roomA;
            Vector2Int lowerCell;
            Vector2Int upperCell;
            GetVerticalConnectorCells(lower, upper, out lowerCell, out upperCell);

            layout.MarkCell(lowerCell.x, lowerCell.y, lower.floorIndex, DungeonCellKind.Room);
            layout.MarkCell(upperCell.x, upperCell.y, upper.floorIndex, DungeonCellKind.Room);
            reservedSpawnCells.Add(SpawnCellKey(lowerCell, lower.floorIndex));
            reservedSpawnCells.Add(SpawnCellKey(upperCell, upper.floorIndex));
            RegisterUpperFloorOpening(layout, lowerCell, upperCell, upper.floorIndex);
            layout.AddMarker(DungeonMapMarkerKind.StairsUp, lowerCell, lower.floorIndex, "Escada");
            layout.AddMarker(DungeonMapMarkerKind.VerticalExit, upperCell, upper.floorIndex, "Chegada");

            DungeonConnection connection = new DungeonConnection();
            connection.roomAId = lower.id;
            connection.roomBId = upper.id;
            connection.fromCell = lowerCell;
            connection.toCell = upperCell;
            connection.gridDistance = Vector2Int.Distance(lowerCell, upperCell);
            connection.isVertical = true;
            connection.isExtraLoop = false;
            layout.connections.Add(connection);
            uniqueModules.Add("room_graph_vertical_edge");
        }

        private void RegisterUpperFloorOpening(DungeonLayout layout, Vector2Int lowerCell, Vector2Int upperCell, int upperFloorIndex)
        {
            Vector2Int direction = StairForwardDirection(lowerCell, upperCell);
            int radius = Mathf.Max(0, verticalOpeningRadius);
            int forwardOffset = Mathf.Max(0, verticalOpeningForwardOffsetCells);

            for (int step = 0; step <= forwardOffset; step++)
            {
                Vector2Int center = upperCell + direction * step;
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dz = -radius; dz <= radius; dz++)
                    {
                        Vector2Int openingCell = center + new Vector2Int(dx, dz);
                        string key = SpawnCellKey(openingCell, upperFloorIndex);
                        floorOpeningCells.Add(key);
                        reservedSpawnCells.Add(key);
                        layout.MarkFloorOpening(openingCell.x, openingCell.y, upperFloorIndex);
                    }
                }
            }
        }

        private static bool RectsOverlap(RectInt a, RectInt b)
        {
            return a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin;
        }

        private static Vector2Int StairForwardDirection(Vector2Int lowerCell, Vector2Int upperCell)
        {
            Vector2Int delta = upperCell - lowerCell;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x >= 0 ? Vector2Int.right : Vector2Int.left;
            }

            if (delta.y != 0)
            {
                return delta.y >= 0 ? Vector2Int.up : Vector2Int.down;
            }

            return Vector2Int.up;
        }

        private void GetVerticalConnectorCells(DungeonRoom lower, DungeonRoom upper, out Vector2Int lowerCell, out Vector2Int upperCell)
        {
            if (RectsOverlap(lower.bounds, upper.bounds))
            {
                int minX = Mathf.Max(lower.bounds.xMin, upper.bounds.xMin);
                int maxX = Mathf.Min(lower.bounds.xMax - 1, upper.bounds.xMax - 1);
                int minZ = Mathf.Max(lower.bounds.yMin, upper.bounds.yMin);
                int maxZ = Mathf.Min(lower.bounds.yMax - 1, upper.bounds.yMax - 1);
                int x = rng.Next(minX, maxX + 1);
                int z = rng.Next(minZ, maxZ + 1);
                lowerCell = new Vector2Int(x, z);
                upperCell = lowerCell;
                return;
            }

            lowerCell = lower.CenterCell;
            upperCell = upper.CenterCell;
        }

        private void ConnectRooms(DungeonLayout layout, DungeonRoom a, DungeonRoom b, bool isExtraLoop)
        {
            Vector2Int centerA = a.CenterCell;
            Vector2Int centerB = b.CenterCell;
            bool horizontalFirst = rng.NextDouble() < 0.5;
            Doorway doorwayA;
            Doorway doorwayB;

            if (horizontalFirst)
            {
                Vector2Int dirA = DirectionTowardX(centerA, centerB);
                Vector2Int dirB = DirectionTowardZ(centerB, centerA);
                if (dirA == Vector2Int.zero) dirA = DirectionTowardZ(centerA, centerB);
                if (dirB == Vector2Int.zero) dirB = DirectionTowardX(centerB, centerA);
                doorwayA = CarveDoorway(layout, a, dirA);
                doorwayB = CarveDoorway(layout, b, dirB);
                MarkHorizontalCorridor(layout, doorwayA.outsideCell.x, doorwayB.outsideCell.x, doorwayA.outsideCell.y, a.floorIndex);
                MarkVerticalCorridor(layout, doorwayA.outsideCell.y, doorwayB.outsideCell.y, doorwayB.outsideCell.x, a.floorIndex);
            }
            else
            {
                Vector2Int dirA = DirectionTowardZ(centerA, centerB);
                Vector2Int dirB = DirectionTowardX(centerB, centerA);
                if (dirA == Vector2Int.zero) dirA = DirectionTowardX(centerA, centerB);
                if (dirB == Vector2Int.zero) dirB = DirectionTowardZ(centerB, centerA);
                doorwayA = CarveDoorway(layout, a, dirA);
                doorwayB = CarveDoorway(layout, b, dirB);
                MarkVerticalCorridor(layout, doorwayA.outsideCell.y, doorwayB.outsideCell.y, doorwayA.outsideCell.x, a.floorIndex);
                MarkHorizontalCorridor(layout, doorwayA.outsideCell.x, doorwayB.outsideCell.x, doorwayB.outsideCell.y, a.floorIndex);
            }

            DungeonConnection connection = new DungeonConnection();
            connection.roomAId = a.id;
            connection.roomBId = b.id;
            connection.fromCell = doorwayA.insideCell;
            connection.toCell = doorwayB.insideCell;
            connection.gridDistance = Mathf.Abs(doorwayA.outsideCell.x - doorwayB.outsideCell.x) + Mathf.Abs(doorwayA.outsideCell.y - doorwayB.outsideCell.y);
            connection.isVertical = false;
            connection.isExtraLoop = isExtraLoop;
            layout.connections.Add(connection);
            uniqueModules.Add(isExtraLoop ? "room_graph_loop_edge" : "room_graph_tree_edge");
        }

        private void MarkHorizontalCorridor(DungeonLayout layout, int xA, int xB, int z, int floorIndex)
        {
            int min = Mathf.Min(xA, xB);
            int max = Mathf.Max(xA, xB);
            int minOffset;
            int maxOffset;
            GetCorridorOffsets(out minOffset, out maxOffset);

            for (int x = min; x <= max; x++)
            {
                for (int offset = minOffset; offset <= maxOffset; offset++)
                {
                    layout.MarkCell(x, z + offset, floorIndex, DungeonCellKind.Corridor);
                }
            }
        }

        private void MarkVerticalCorridor(DungeonLayout layout, int zA, int zB, int x, int floorIndex)
        {
            int min = Mathf.Min(zA, zB);
            int max = Mathf.Max(zA, zB);
            int minOffset;
            int maxOffset;
            GetCorridorOffsets(out minOffset, out maxOffset);

            for (int z = min; z <= max; z++)
            {
                for (int offset = minOffset; offset <= maxOffset; offset++)
                {
                    layout.MarkCell(x + offset, z, floorIndex, DungeonCellKind.Corridor);
                }
            }
        }

        private struct Doorway
        {
            public Vector2Int insideCell;
            public Vector2Int outsideCell;
        }

        private Doorway CarveDoorway(DungeonLayout layout, DungeonRoom room, Vector2Int direction)
        {
            if (direction == Vector2Int.zero)
            {
                direction = Vector2Int.right;
            }

            int floorIndex = room.floorIndex;
            Vector2Int center = room.CenterCell;
            Vector2Int inside = GetDoorwayInsideCell(room, direction, center);
            Vector2Int outside = inside + direction;
            int minOffset;
            int maxOffset;
            GetCorridorOffsets(out minOffset, out maxOffset);

            int clearance = Mathf.Max(0, doorwayExtraClearance);
            for (int offset = minOffset - clearance; offset <= maxOffset + clearance; offset++)
            {
                Vector2Int shiftedInside = ShiftDoorwayCell(room, inside, direction, offset);
                Vector2Int shiftedOutside = shiftedInside + direction;
                layout.MarkCell(shiftedInside.x, shiftedInside.y, floorIndex, DungeonCellKind.Room);
                layout.MarkCell(shiftedOutside.x, shiftedOutside.y, floorIndex, DungeonCellKind.Corridor);
                openWallEdges.Add(WallEdgeKey(shiftedInside, direction, floorIndex));
            }

            Doorway doorway = new Doorway();
            doorway.insideCell = inside;
            doorway.outsideCell = outside;
            return doorway;
        }

        private static Vector2Int DirectionTowardX(Vector2Int from, Vector2Int to)
        {
            if (to.x > from.x) return Vector2Int.right;
            if (to.x < from.x) return Vector2Int.left;
            return Vector2Int.zero;
        }

        private static Vector2Int DirectionTowardZ(Vector2Int from, Vector2Int to)
        {
            if (to.y > from.y) return Vector2Int.up;
            if (to.y < from.y) return Vector2Int.down;
            return Vector2Int.zero;
        }

        private Vector2Int GetDoorwayInsideCell(DungeonRoom room, Vector2Int direction, Vector2Int preferredCenter)
        {
            if (direction.x > 0)
            {
                return new Vector2Int(room.bounds.xMax - 1, Mathf.Clamp(preferredCenter.y, room.bounds.yMin, room.bounds.yMax - 1));
            }

            if (direction.x < 0)
            {
                return new Vector2Int(room.bounds.xMin, Mathf.Clamp(preferredCenter.y, room.bounds.yMin, room.bounds.yMax - 1));
            }

            if (direction.y > 0)
            {
                return new Vector2Int(Mathf.Clamp(preferredCenter.x, room.bounds.xMin, room.bounds.xMax - 1), room.bounds.yMax - 1);
            }

            return new Vector2Int(Mathf.Clamp(preferredCenter.x, room.bounds.xMin, room.bounds.xMax - 1), room.bounds.yMin);
        }

        private Vector2Int ShiftDoorwayCell(DungeonRoom room, Vector2Int insideCell, Vector2Int direction, int offset)
        {
            if (direction.x != 0)
            {
                int z = Mathf.Clamp(insideCell.y + offset, room.bounds.yMin, room.bounds.yMax - 1);
                return new Vector2Int(insideCell.x, z);
            }

            int x = Mathf.Clamp(insideCell.x + offset, room.bounds.xMin, room.bounds.xMax - 1);
            return new Vector2Int(x, insideCell.y);
        }

        private void GetCorridorOffsets(out int minOffset, out int maxOffset)
        {
            int width = Mathf.Max(1, corridorWidth);
            minOffset = -width / 2;
            maxOffset = minOffset + width - 1;
        }

        private void AssignStartAndGoal(DungeonLayout layout)
        {
            if (layout.rooms.Count == 0)
            {
                return;
            }

            DungeonRoom start = layout.rooms[0];
            for (int i = 1; i < layout.rooms.Count; i++)
            {
                if (layout.rooms[i].floorIndex > start.floorIndex)
                {
                    continue;
                }

                if (layout.rooms[i].floorIndex < start.floorIndex)
                {
                    start = layout.rooms[i];
                    continue;
                }

                Vector2Int candidate = layout.rooms[i].CenterCell;
                Vector2Int current = start.CenterCell;
                if (candidate.x + candidate.y < current.x + current.y)
                {
                    start = layout.rooms[i];
                }
            }

            DungeonRoom goal = start;
            float farthest = -1f;
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                DungeonRoom candidateRoom = layout.rooms[i];
                float distance = RoomDistance3D(start, candidateRoom);
                if (distance > farthest)
                {
                    farthest = distance;
                    goal = candidateRoom;
                }
            }

            layout.startRoomId = start.id;
            layout.goalRoomId = goal.id;
            layout.AddMarker(DungeonMapMarkerKind.Start, start.CenterCell, start.floorIndex, "Inicio");
            layout.AddMarker(DungeonMapMarkerKind.Goal, goal.CenterCell, goal.floorIndex, "Saida");
        }

        private float RoomDistance3D(DungeonRoom a, DungeonRoom b)
        {
            float horizontal = Vector2Int.Distance(a.CenterCell, b.CenterCell) * tileSize;
            float vertical = Mathf.Abs(a.floorIndex - b.floorIndex) * floorHeight;
            return horizontal + vertical;
        }

        private DungeonMetrics CreateMetrics(DungeonLayout layout, int selectedSeed)
        {
            bool hasVerticalConnectors = HasVerticalConnections(layout);
            bool hasMultiFloor = HasMultipleConnectedFloors(layout);
            DungeonMetrics metrics = DungeonMetricsCalculator.Calculate(
                layout,
                selectedSeed,
                "Room Graph",
                tileSize,
                uniqueModules.Count,
                hasVerticalConnectors,
                hasMultiFloor,
                bossArenaMinAreaCells);

            metrics.spawnableRoomCells = CountSpawnableRoomCells(layout);
            metrics.enemyBudgetTarget = enemyBudget;
            metrics.lootBudgetTarget = lootBudget;
            metrics.trapBudgetTarget = trapBudget;
            metrics.propsSpawned = propsSpawned;
            metrics.enemiesSpawned = enemiesSpawned;
            metrics.lootSpawned = lootSpawned;
            metrics.trapsSpawned = trapsSpawned;

            metrics.booleans.SupportsRandomEnemySpawns = true;
            metrics.booleans.SupportsLootDistribution = true;
            metrics.booleans.SupportsTraps = true;
            metrics.booleans.SupportsVerticalConnectors = hasVerticalConnectors;
            metrics.booleans.SupportsMultiFloor = hasMultiFloor;
            metrics.booleans.BudgetAwareSpawns = IsBudgetWithinCapacity(metrics.spawnableRoomCells);

            DungeonQualitativeScorer.ApplyScores(metrics, 0f, 1);
            return metrics;
        }

        private void ApplyPerformanceMetrics(
            DungeonMetrics metrics,
            float layoutMs,
            float geometryMs,
            float metricsMs,
            float totalMs,
            long memoryBeforeBytes,
            long memoryAfterBytes)
        {
            metrics.layoutGenerationMilliseconds = layoutMs;
            metrics.geometryInstantiationMilliseconds = geometryMs;
            metrics.metricsCalculationMilliseconds = metricsMs;
            metrics.totalGenerationMilliseconds = totalMs;
            metrics.generationMilliseconds = totalMs;
            metrics.occupiedCellCount = LastLayout != null ? LastLayout.CountOccupiedCells() : 0;
            metrics.connectionCount = LastLayout != null ? LastLayout.connections.Count : 0;
            metrics.generatedGameObjectCount = generatedGameObjectCount;
            metrics.managedMemoryBeforeMB = BytesToMegabytes(memoryBeforeBytes);
            metrics.managedMemoryAfterMB = BytesToMegabytes(memoryAfterBytes);
            metrics.managedMemoryDeltaKB = (memoryAfterBytes - memoryBeforeBytes) / 1024f;
            metrics.booleans.RuntimeRegeneration = totalMs <= runtimeRegenerationMaxMilliseconds;
        }

        private static float BytesToMegabytes(long bytes)
        {
            return bytes / (1024f * 1024f);
        }

        private int CountSpawnableRoomCells(DungeonLayout layout)
        {
            int count = 0;
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                DungeonRoom room = layout.rooms[i];
                for (int x = room.bounds.xMin; x < room.bounds.xMax; x++)
                {
                    for (int z = room.bounds.yMin; z < room.bounds.yMax; z++)
                    {
                        Vector2Int cell = new Vector2Int(x, z);
                        if (!reservedSpawnCells.Contains(SpawnCellKey(cell, room.floorIndex)) && cell != room.CenterCell)
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        private bool IsBudgetWithinCapacity(int spawnableCells)
        {
            return enemyBudget + lootBudget + trapBudget <= spawnableCells;
        }

        private bool HasVerticalConnections(DungeonLayout layout)
        {
            for (int i = 0; i < layout.connections.Count; i++)
            {
                if (layout.connections[i].isVertical)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasMultipleConnectedFloors(DungeonLayout layout)
        {
            if (layout.floorCount < 2)
            {
                return false;
            }

            return HasVerticalConnections(layout);
        }

        private bool VerifySeedReproducibility(int seedToVerify)
        {
            GenerateForSeed(seedToVerify, false, false, false);
            string firstHash = DungeonTopologyHasher.Compute(LastLayout);
            GenerateForSeed(seedToVerify, false, false, false);
            string secondHash = DungeonTopologyHasher.Compute(LastLayout);
            return firstHash == secondHash;
        }

        private DungeonReportContext CreateReportContext(bool seedReproducible, int runCount, int uniqueTopologyCount, float diversityRatio)
        {
            DungeonReportContext context = new DungeonReportContext();
            context.enemyPrefabsConfigured = assetLibrary != null && assetLibrary.enemyPrefabs != null && assetLibrary.enemyPrefabs.Count > 0;
            context.lootPrefabsConfigured = assetLibrary != null && assetLibrary.lootPrefabs != null && assetLibrary.lootPrefabs.Count > 0;
            context.trapPrefabsConfigured = assetLibrary != null && assetLibrary.trapPrefabs != null && assetLibrary.trapPrefabs.Count > 0;
            context.enemyBudget = enemyBudget;
            context.lootBudget = lootBudget;
            context.trapBudget = trapBudget;
            context.bossArenaMinAreaCells = bossArenaMinAreaCells;
            context.runtimeRegenerationMaxMilliseconds = runtimeRegenerationMaxMilliseconds;
            context.seedReproducibleVerified = seedReproducible;
            context.supportsVerticalConnectors = enableMultiFloorRoomGraph && floorCount > 1 && verticalConnectionsPerFloorPair > 0;
            context.supportsMultiFloor = enableMultiFloorRoomGraph && floorCount > 1;
            context.runCount = runCount;
            context.uniqueTopologyCount = uniqueTopologyCount;
            context.topologyDiversityRatio = diversityRatio;
            return context;
        }

        private static string BuildBatchSummary(DungeonBatchReport report, bool seedReproducible)
        {
            return "Teste Room Graph executado com " + report.runCount
                + " seed(s). Topologias unicas: " + report.uniqueTopologyCount
                + "/" + report.runCount
                + ". Diversidade topologica: " + (report.topologyDiversityRatio * 100f).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)
                + "%. Reprodutibilidade por seed: " + (seedReproducible ? "aprovada" : "falhou") + ".";
        }

        private void InstantiateLayout(DungeonLayout layout)
        {
            GameObject rootObject = new GameObject("Generated Room Graph Dungeon");
            rootObject.transform.SetParent(transform, false);
            dungeonRoot = rootObject.transform;

            foreach (DungeonGridCell cell in layout.OccupiedGridCells())
            {
                CreateFloor(cell);
            }

            foreach (DungeonGridCell cell in layout.OccupiedGridCells())
            {
                for (int i = 0; i < CardinalDirections.Length; i++)
                {
                    Vector2Int direction = CardinalDirections[i];
                    Vector2Int neighbor = cell.Cell2D + direction;
                    if (!layout.IsOccupied(neighbor, cell.floorIndex) && !IsOpenWallEdge(cell.Cell2D, direction, cell.floorIndex))
                    {
                        CreateWall(cell, direction);
                    }
                }
            }

            CreateVerticalConnectorVisuals(layout);
            CreateRoomMarkers(layout);
            SpawnRoomProps(layout);
            SpawnBudgetedObjects(layout, enemyBudget, "enemy", GetEnemyPrefab);
            SpawnBudgetedObjects(layout, lootBudget, "loot", GetLootPrefab);
            SpawnBudgetedObjects(layout, trapBudget, "trap", GetTrapPrefab);
        }

        private void CreateFloor(DungeonGridCell cell)
        {
            if (floorOpeningCells.Contains(SpawnCellKey(cell.Cell2D, cell.floorIndex)) || LastLayout.IsFloorOpening(cell.Cell2D, cell.floorIndex))
            {
                uniqueModules.Add("room_graph_floor_opening");
                return;
            }

            Vector3 position = CellToWorld(cell.Cell2D, cell.floorIndex, 0f);
            if (assetLibrary != null && assetLibrary.floorTilePrefab != null)
            {
                GameObject instance = Instantiate(assetLibrary.floorTilePrefab, position, Quaternion.identity, dungeonRoot);
                instance.transform.localScale = prefabInstanceScale;
                uniqueModules.Add(assetLibrary.floorTilePrefab.name);
                generatedGameObjectCount++;
                return;
            }

            if (!usePrimitiveFallbacks)
            {
                return;
            }

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Fallback Floor";
            floor.transform.SetParent(dungeonRoot, false);
            floor.transform.position = position + new Vector3(0f, -0.05f, 0f);
            floor.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);
            ApplyMaterial(floor, fallbackFloorMaterial);
            uniqueModules.Add("fallback_floor");
            generatedGameObjectCount++;
        }

        private void CreateWall(DungeonGridCell cell, Vector2Int direction)
        {
            Vector3 center = CellToWorld(cell.Cell2D, cell.floorIndex, wallYOffset);
            Vector3 offset = new Vector3(direction.x * tileSize * 0.5f, 0f, direction.y * tileSize * 0.5f);
            Vector3 position = center + offset;
            bool eastWestWall = direction.x != 0;
            Quaternion rotation = Quaternion.Euler(0f, (eastWestWall ? 90f : 0f) + wallYawOffset, 0f);

            if (assetLibrary != null && assetLibrary.wallPrefab != null)
            {
                GameObject instance = Instantiate(assetLibrary.wallPrefab, position, rotation, dungeonRoot);
                instance.transform.localScale = prefabInstanceScale;
                uniqueModules.Add(assetLibrary.wallPrefab.name);
                generatedGameObjectCount++;
                return;
            }

            if (!usePrimitiveFallbacks)
            {
                return;
            }

            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Fallback Wall";
            wall.transform.SetParent(dungeonRoot, false);
            wall.transform.position = position + new Vector3(0f, wallHeight * 0.5f, 0f);
            wall.transform.rotation = rotation;
            wall.transform.localScale = eastWestWall
                ? new Vector3(wallThickness, wallHeight, tileSize + wallThickness)
                : new Vector3(tileSize + wallThickness, wallHeight, wallThickness);
            ApplyMaterial(wall, fallbackWallMaterial);
            uniqueModules.Add("fallback_wall");
            generatedGameObjectCount++;
        }

        private void CreateVerticalConnectorVisuals(DungeonLayout layout)
        {
            for (int i = 0; i < layout.connections.Count; i++)
            {
                DungeonConnection connection = layout.connections[i];
                if (!connection.isVertical)
                {
                    continue;
                }

                DungeonRoom roomA = layout.GetRoomById(connection.roomAId);
                DungeonRoom roomB = layout.GetRoomById(connection.roomBId);
                if (roomA == null || roomB == null)
                {
                    continue;
                }

                DungeonRoom lower = roomA.floorIndex <= roomB.floorIndex ? roomA : roomB;
                DungeonRoom upper = lower == roomA ? roomB : roomA;
                Vector2Int lowerCell = lower == roomA ? connection.fromCell : connection.toCell;
                Vector2Int upperCell = lower == roomA ? connection.toCell : connection.fromCell;
                Quaternion rotation = Quaternion.Euler(0f, DirectionToYaw(upperCell - lowerCell), 0f);

                GameObject lowerPrefab = assetLibrary != null && assetLibrary.stairsUpPrefab != null
                    ? assetLibrary.stairsUpPrefab
                    : assetLibrary != null ? assetLibrary.stairsDownPrefab : null;

                GameObject lowerInstance = SpawnOptional(lowerPrefab, "room_graph_stairs_up", CellToWorld(lowerCell, lower.floorIndex, 0f), rotation);

                if (lowerInstance == null && usePrimitiveFallbacks)
                {
                    CreateFallbackVerticalConnector(lowerCell, upperCell, lower.floorIndex, upper.floorIndex, rotation);
                }
            }
        }

        private void CreateFallbackVerticalConnector(Vector2Int lowerCell, Vector2Int upperCell, int lowerFloor, int upperFloor, Quaternion rotation)
        {
            Vector3 lowerPosition = CellToWorld(lowerCell, lowerFloor, 0.25f);
            GameObject column = GameObject.CreatePrimitive(PrimitiveType.Cube);
            column.name = "Fallback Vertical Connector";
            column.transform.SetParent(dungeonRoot, false);
            column.transform.position = lowerPosition + new Vector3(0f, floorHeight * 0.5f, 0f);
            column.transform.rotation = rotation;
            column.transform.localScale = new Vector3(tileSize * 0.8f, floorHeight, tileSize * 0.8f);
            ApplyMaterial(column, fallbackWallMaterial);
            uniqueModules.Add("fallback_vertical_connector");
            generatedGameObjectCount++;
        }

        private static float DirectionToYaw(Vector2Int direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                return direction.x >= 0 ? 90f : 270f;
            }

            return direction.y >= 0 ? 0f : 180f;
        }

        private void CreateRoomMarkers(DungeonLayout layout)
        {
            DungeonRoom start = layout.GetRoomById(layout.startRoomId);
            DungeonRoom goal = layout.GetRoomById(layout.goalRoomId);

            if (start != null)
            {
                reservedSpawnCells.Add(SpawnCellKey(start.CenterCell, start.floorIndex));
                SpawnOptional(assetLibrary != null ? assetLibrary.startMarkerPrefab : null, "start_marker", CellToWorld(start.CenterCell, start.floorIndex, 0.1f), Quaternion.identity);
            }

            if (goal != null)
            {
                reservedSpawnCells.Add(SpawnCellKey(goal.CenterCell, goal.floorIndex));
                SpawnOptional(assetLibrary != null ? assetLibrary.goalMarkerPrefab : null, "goal_marker", CellToWorld(goal.CenterCell, goal.floorIndex, 0.1f), Quaternion.identity);
            }
        }

        private void SpawnRoomProps(DungeonLayout layout)
        {
            if (assetLibrary == null || assetLibrary.propPrefabs == null || assetLibrary.propPrefabs.Count == 0)
            {
                return;
            }

            for (int i = 0; i < layout.rooms.Count; i++)
            {
                if (rng.NextDouble() > propRoomChance)
                {
                    continue;
                }

                int count = rng.Next(Mathf.Max(0, minPropsPerRoom), Mathf.Max(minPropsPerRoom, maxPropsPerRoom) + 1);
                for (int p = 0; p < count; p++)
                {
                    Vector2Int cell;
                    if (!TryGetFreeRoomCell(layout.rooms[i], out cell))
                    {
                        continue;
                    }

                    GameObject prefab = assetLibrary.GetRandomProp(rng);
                    Quaternion rotation = Quaternion.Euler(0f, rng.Next(0, 4) * 90f, 0f);
                    SpawnOptional(prefab, "prop", CellToWorld(cell, layout.rooms[i].floorIndex, 0f), rotation);
                    layout.AddMarker(DungeonMapMarkerKind.Prop, cell, layout.rooms[i].floorIndex, "Prop");
                    reservedSpawnCells.Add(SpawnCellKey(cell, layout.rooms[i].floorIndex));
                    propsSpawned++;
                }
            }
        }

        private void SpawnBudgetedObjects(DungeonLayout layout, int budget, string moduleLabel, System.Func<GameObject> prefabGetter)
        {
            if (budget <= 0 || layout.rooms.Count == 0)
            {
                return;
            }

            int spawned = 0;
            int attempts = 0;
            while (spawned < budget && attempts < budget * 30)
            {
                attempts++;
                DungeonRoom room = layout.rooms[rng.Next(0, layout.rooms.Count)];
                Vector2Int cell;
                if (!TryGetFreeRoomCell(room, out cell))
                {
                    continue;
                }

                GameObject prefab = prefabGetter();
                Quaternion rotation = Quaternion.Euler(0f, rng.Next(0, 4) * 90f, 0f);
                SpawnOptional(prefab, moduleLabel, CellToWorld(cell, room.floorIndex, 0f), rotation);
                layout.AddMarker(GetMarkerKind(moduleLabel), cell, room.floorIndex, moduleLabel);
                reservedSpawnCells.Add(SpawnCellKey(cell, room.floorIndex));
                IncrementSpawnCounter(moduleLabel);
                spawned++;
            }
        }

        private static DungeonMapMarkerKind GetMarkerKind(string moduleLabel)
        {
            if (moduleLabel == "enemy") return DungeonMapMarkerKind.Enemy;
            if (moduleLabel == "loot") return DungeonMapMarkerKind.Loot;
            if (moduleLabel == "trap") return DungeonMapMarkerKind.Trap;
            return DungeonMapMarkerKind.Prop;
        }

        private void IncrementSpawnCounter(string moduleLabel)
        {
            if (moduleLabel == "enemy")
            {
                enemiesSpawned++;
            }
            else if (moduleLabel == "loot")
            {
                lootSpawned++;
            }
            else if (moduleLabel == "trap")
            {
                trapsSpawned++;
            }
        }

        private GameObject GetEnemyPrefab()
        {
            return assetLibrary != null ? assetLibrary.GetRandomEnemy(rng) : null;
        }

        private GameObject GetLootPrefab()
        {
            return assetLibrary != null ? assetLibrary.GetRandomLoot(rng) : null;
        }

        private GameObject GetTrapPrefab()
        {
            return assetLibrary != null ? assetLibrary.GetRandomTrap(rng) : null;
        }

        private bool TryGetFreeRoomCell(DungeonRoom room, out Vector2Int cell)
        {
            for (int attempt = 0; attempt < 40; attempt++)
            {
                int x = rng.Next(room.bounds.xMin, room.bounds.xMax);
                int z = rng.Next(room.bounds.yMin, room.bounds.yMax);
                cell = new Vector2Int(x, z);
                if (!reservedSpawnCells.Contains(SpawnCellKey(cell, room.floorIndex)) && cell != room.CenterCell)
                {
                    return true;
                }
            }

            cell = room.CenterCell;
            return false;
        }

        private GameObject SpawnOptional(GameObject prefab, string fallbackModuleId, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
            {
                uniqueModules.Add(fallbackModuleId);
                return null;
            }

            GameObject instance = Instantiate(prefab, position, rotation, dungeonRoot);
            instance.transform.localScale = prefabInstanceScale;
            uniqueModules.Add(prefab.name);
            generatedGameObjectCount++;
            return instance;
        }

        private Vector3 CellToWorld(Vector2Int cell, int floorIndex, float yOffset)
        {
            float originX = centerOnOrigin ? -mapWidth * tileSize * 0.5f : 0f;
            float originZ = centerOnOrigin ? -mapDepth * tileSize * 0.5f : 0f;
            return new Vector3(originX + cell.x * tileSize, floorIndex * floorHeight + yOffset, originZ + cell.y * tileSize);
        }

        private void ExportCurrent2DMaps(string exportLabel)
        {
            if (LastLayout == null)
            {
                Debug.LogWarning("No Room Graph layout available to export as a 2D map. Generate a dungeon first.");
                return;
            }

            string metricsFolder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            string mapFolder = Path.Combine(metricsFolder, string.IsNullOrEmpty(mapExportSubfolderName) ? "Maps" : mapExportSubfolderName);
            List<string> paths = DungeonMap2DExporter.ExportFloorMaps(
                LastLayout,
                mapFolder,
                metricsFilePrefix,
                "ROOM GRAPH",
                resolvedSeed,
                exportLabel,
                Mathf.Max(4, mapPixelsPerCell),
                mapIncludeGrid,
                mapIncludeLegend);

            Debug.Log("Room Graph 2D map exported:\n" + string.Join("\n", paths.ToArray()));
        }

        private static string SpawnCellKey(Vector2Int cell, int floorIndex)
        {
            return floorIndex + ":" + cell.x + "," + cell.y;
        }

        private static void ApplyMaterial(GameObject instance, Material material)
        {
            if (material == null)
            {
                return;
            }

            Renderer renderer = instance.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        private bool IsOpenWallEdge(Vector2Int cell, Vector2Int direction, int floorIndex)
        {
            return openWallEdges.Contains(WallEdgeKey(cell, direction, floorIndex));
        }

        private static string WallEdgeKey(Vector2Int cell, Vector2Int direction, int floorIndex)
        {
            int ax = cell.x;
            int az = cell.y;
            int bx = cell.x + direction.x;
            int bz = cell.y + direction.y;

            if (bx < ax || (bx == ax && bz < az))
            {
                int tempX = ax;
                int tempZ = az;
                ax = bx;
                az = bz;
                bx = tempX;
                bz = tempZ;
            }

            return floorIndex + ":" + ax + "," + az + "|" + bx + "," + bz;
        }
    }

    public static class RoomGraphParameterNotes
    {
        public static List<DungeonParameterResult> CreateRunResults(DungeonMetrics metrics, DungeonReportContext context)
        {
            List<DungeonParameterResult> results = DungeonParameterEvaluator.CreateRunResults(metrics, context);
            Apply(results);
            return results;
        }

        public static List<DungeonParameterResult> CreateAggregateResults(List<DungeonRunReport> runs, DungeonReportContext context)
        {
            List<DungeonParameterResult> results = DungeonParameterEvaluator.CreateAggregateResults(runs, context);
            Apply(results);
            return results;
        }

        private static void Apply(List<DungeonParameterResult> results)
        {
            for (int i = 0; i < results.Count; i++)
            {
                DungeonParameterResult result = results[i];
                result.collectionMethod = MethodFor(result.parameterName);
                result.interpretation = InterpretationFor(result.parameterName, result.interpretation);
                result.bspApplicability = NoteFor(result.parameterName);
            }
        }

        private static string MethodFor(string parameterName)
        {
            if (parameterName == "numRoomsTarget") return "Contagem de nos/salas posicionados pelo Room Graph.";
            if (parameterName == "connectivityRatio") return "Busca no grafo de salas a partir da sala inicial.";
            if (parameterName == "verticalVariance") return "Desvio padrao das alturas dos nos/salas entre pavimentos.";
            if (parameterName == "fillPercentage") return "Celulas ocupadas por salas e corredores divididas pelo total do grid.";
            if (parameterName == "branchFactor") return "Media de arestas por no/sala no grafo.";
            if (parameterName == "avgPathLength") return "Distancias calculadas no grafo explicito de salas.";
            if (parameterName == "uniqueModules") return "Contagem de modulos logicos e prefabs usados pela montagem do Room Graph.";
            if (parameterName == "navigableVolumeRatio") return "Estimativa logica de celulas navegaveis antes de NavMesh.";
            if (parameterName == "criticalPathLength") return "Maior distancia encontrada no grafo a partir da sala inicial.";
            if (parameterName == "avgAlternativePathLength") return "Media das arestas extras marcadas como loops.";
            if (parameterName == "SupportsRandomEnemySpawns") return "Verifica se o sistema de spawn usa salas/celulas geradas pelo grafo.";
            if (parameterName == "SupportsLootDistribution") return "Verifica se loot pode ser distribuido sobre salas/celulas do grafo.";
            if (parameterName == "SupportsTraps") return "Verifica se armadilhas podem ser marcadas em areas navegaveis.";
            if (parameterName == "SupportsBacktrackingLoops") return "Conta execucoes em que o grafo recebeu ciclos navegaveis.";
            if (parameterName == "SupportsVerticalConnectors") return "Conta execucoes com arestas verticais entre pavimentos.";
            if (parameterName == "SupportsMultiFloor") return "Conta execucoes com mais de um pavimento conectado por arestas verticais.";
            if (parameterName == "SupportsBossArena") return "Conta execucoes com ao menos uma sala acima da area minima configurada.";
            if (parameterName == "SeedReproducible") return "Gera a mesma seed duas vezes e compara o hash topologico.";
            if (parameterName == "RuntimeRegeneration") return "Conta execucoes abaixo do limite de tempo configurado.";
            if (parameterName == "BudgetAwareSpawns") return "Verifica se os orcamentos de inimigos, loot e armadilhas cabem nas celulas livres.";
            if (parameterName == "Replayability") return "Estimativa baseada na diversidade de hashes topologicos entre seeds.";
            if (parameterName == "Debuggability") return "Estimativa baseada na natureza explicita do grafo de nos e arestas.";
            if (parameterName == "Flow") return "Estimativa baseada em conectividade, caminho critico e ramificacao.";
            if (parameterName == "Legibility") return "Estimativa baseada em conectividade, densidade e ramificacao do grafo.";
            if (parameterName == "StructuralVariety") return "Estimativa baseada em quantidade de salas, modulos e loops.";
            if (parameterName == "layoutGenerationMilliseconds") return "Tempo para criar, posicionar e conectar o Room Graph logico.";
            if (parameterName == "geometryInstantiationMilliseconds") return "Tempo gasto instanciando prefabs/objetos visuais quando habilitado.";
            if (parameterName == "metricsCalculationMilliseconds") return "Tempo para calcular e consolidar metricas apos a geracao.";
            if (parameterName == "totalGenerationMilliseconds") return "Tempo total medido da execucao.";
            if (parameterName == "generatedGameObjectCount") return "Quantidade de GameObjects criados durante a montagem visual.";
            if (parameterName == "occupiedCellCount") return "Quantidade de celulas ocupadas no grid logico.";
            if (parameterName == "connectionCount") return "Quantidade de arestas/conexoes no grafo.";
            if (parameterName == "managedMemoryDeltaKB") return "Variacao aproximada de memoria gerenciada durante a execucao.";
            return "Parametro medido na variante Room Graph.";
        }

        private static string InterpretationFor(string parameterName, string existing)
        {
            if (parameterName == "numRoomsTarget") return "Indica quantos nos do grafo foram efetivamente posicionados como salas.";
            if (parameterName == "connectivityRatio") return "Mostra se as salas ficaram conectadas no componente principal do grafo.";
            if (parameterName == "verticalVariance") return "Valor acima de 0 indica variacao vertical gerada por pavimentos e arestas verticais.";
            if (parameterName == "branchFactor") return "Valores maiores indicam mais bifurcacoes e loops no grafo.";
            if (parameterName == "criticalPathLength") return "Aproxima o percurso principal entre inicio e ponto mais distante no grafo.";
            if (parameterName == "SupportsVerticalConnectors") return "Parametro atendido quando existem arestas verticais Room Graph entre pavimentos.";
            if (parameterName == "SupportsMultiFloor") return "Parametro atendido quando ha pavimentos conectados dentro do mesmo grafo.";
            if (parameterName == "Debuggability") return "A estrutura de nos e arestas facilita inspecao, depuracao e reproducao por seed.";
            if (parameterName == "layoutGenerationMilliseconds") return "Representa o custo algoritmico principal do Room Graph.";
            if (parameterName == "connectionCount") return "Reflete diretamente a complexidade topologica do grafo.";
            return existing.Replace("BSP", "Room Graph").Replace("bsp", "room graph");
        }

        private static string NoteFor(string parameterName)
        {
            if (parameterName == "numRoomsTarget") return "Room Graph controla salas como nos do grafo; a etapa de embedding pode gerar menos salas se nao houver espaco sem sobreposicao.";
            if (parameterName == "connectivityRatio") return "Room Graph e forte neste parametro: a arvore de conexoes garante componente principal conectado quando ha pelo menos duas salas.";
            if (parameterName == "verticalVariance") return "Room Graph puro suporta verticalidade ao criar arestas verticais entre nos de pavimentos adjacentes.";
            if (parameterName == "fillPercentage") return "A densidade depende do numero de nos, tamanhos de sala e sucesso do posicionamento espacial do grafo.";
            if (parameterName == "branchFactor") return "O fator de ramificacao e diretamente controlado por arvore inicial e arestas extras de loop.";
            if (parameterName == "avgPathLength") return "Caminhos sao naturais para Room Graph, pois o algoritmo ja trabalha com grafo explicito.";
            if (parameterName == "uniqueModules") return "A variedade vem das categorias logicas do grafo, conectores, corredores e assets configurados.";
            if (parameterName == "navigableVolumeRatio") return "A navegabilidade e estimada no grid logico; validacao fisica final ainda exige NavMesh.";
            if (parameterName == "criticalPathLength") return "Room Graph mede bem percurso principal porque distancias no grafo sao parte central do modelo.";
            if (parameterName == "avgAlternativePathLength") return "Loops e atalhos sao uma vantagem direta do Room Graph, bastando adicionar arestas extras.";
            if (parameterName == "SupportsRandomEnemySpawns") return "Room Graph oferece salas/nos claros para spawns; balanceamento de combate e camada de gameplay.";
            if (parameterName == "SupportsLootDistribution") return "Loot pode ser distribuido por nos/salas ou por distancia no grafo.";
            if (parameterName == "SupportsTraps") return "Armadilhas podem ser colocadas em salas, corredores ou arestas especificas do grafo.";
            if (parameterName == "SupportsBacktrackingLoops") return "Suporte nativo quando o grafo recebe arestas extras alem da arvore de conectividade.";
            if (parameterName == "SupportsVerticalConnectors") return "Implementado como arestas verticais do proprio Room Graph, sem usar BSP ou outro algoritmo auxiliar.";
            if (parameterName == "SupportsMultiFloor") return "Implementado por nos distribuidos em pavimentos e arestas verticais entre camadas.";
            if (parameterName == "SupportsBossArena") return "Suportado se algum no/sala for grande o suficiente; pode exigir parametros de tamanho ou uma regra de no especial.";
            if (parameterName == "SeedReproducible") return "O Room Graph e deterministico quando todas as escolhas usam a mesma seed.";
            if (parameterName == "RuntimeRegeneration") return "Room Graph tende a ser rapido; a etapa mais sensivel e o posicionamento sem sobreposicao das salas.";
            if (parameterName == "BudgetAwareSpawns") return "Orcamentos sao controlados por camada de spawn sobre os nos/salas gerados.";
            if (parameterName == "Replayability") return "A variacao entre seeds depende da combinacao entre grafo, embedding espacial e arestas extras.";
            if (parameterName == "Debuggability") return "Room Graph e facil de depurar porque a topologia pode ser inspecionada como nos e arestas.";
            if (parameterName == "Flow") return "O controle de fluxo e uma das maiores vantagens do Room Graph, pois caminhos e ramificacoes sao explicitos.";
            if (parameterName == "Legibility") return "A legibilidade depende do embedding: o grafo e claro, mas corredores podem cruzar se o layout espacial ficar apertado.";
            if (parameterName == "StructuralVariety") return "A variedade estrutural e boa em macrofluxo; variedade local ainda depende de regras de decoracao/assets.";
            if (parameterName == "layoutGenerationMilliseconds") return "Mede custo de criar nos, posiciona-los e conectar o grafo; compare separado da instanciacao Unity.";
            if (parameterName == "geometryInstantiationMilliseconds") return "Custo visual depende dos prefabs/Unity, nao do Room Graph puro.";
            if (parameterName == "metricsCalculationMilliseconds") return "Custo da instrumentacao, nao do algoritmo.";
            if (parameterName == "totalGenerationMilliseconds") return "Inclui layout, instanciacao visual e metricas; use junto com o tempo logico.";
            if (parameterName == "generatedGameObjectCount") return "Reflete peso da montagem visual da cena gerada pelo grafo.";
            if (parameterName == "occupiedCellCount") return "Proxy de escala espacial resultante do embedding dos nos e corredores.";
            if (parameterName == "connectionCount") return "Metrica muito natural para Room Graph, pois corresponde diretamente ao numero de arestas.";
            if (parameterName == "managedMemoryDeltaKB") return "Estimativa sujeita ao GC da Unity; use como indicio comparativo.";
            return "Parametro avaliado na variante Room Graph pura.";
        }
    }
}
