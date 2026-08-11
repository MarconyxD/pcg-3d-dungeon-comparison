using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Dissertation.PCG
{
    public sealed class CellularAutomataDungeonGenerator : MonoBehaviour
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

        [Header("Pure Cellular Automata rules")]
        [Tooltip("Probabilidade inicial de uma celula interna nascer aberta. Valores perto de 0.45 a 0.5 costumam gerar cavernas organicas.")]
        [Range(0.05f, 0.95f)] public float initialOpenChance = 0.47f;
        [Tooltip("Quantidade de iteracoes de suavizacao local. Mais iteracoes deixam as formas maiores e mais arredondadas.")]
        public int smoothingIterations = 5;
        [Tooltip("Celula aberta permanece aberta quando possui pelo menos este numero de vizinhos abertos.")]
        public int survivalOpenNeighborLimit = 4;
        [Tooltip("Celula fechada nasce aberta quando possui pelo menos este numero de vizinhos abertos.")]
        public int birthOpenNeighborLimit = 5;
        [Tooltip("Espessura da borda externa sempre fechada. Mantem o mapa contido sem usar reparo ou outro algoritmo.")]
        public int solidBorderThickness = 1;
        [Tooltip("Quando ativo, diagonais entram na vizinhanca da regra celular 2D.")]
        public bool includeDiagonalNeighbors = true;
        [Tooltip("Quantidade minima de celulas para um componente aberto ser considerado uma sala mensuravel.")]
        public int minimumRoomComponentArea = 12;
        [Tooltip("Quantidade maxima de vizinhos cardinais abertos para uma celula aberta ser classificada como corredor estreito na analise.")]
        public int corridorCardinalNeighborLimit = 2;
        [Tooltip("Quantidade maxima de vizinhos totais abertos para uma celula aberta ser classificada como corredor estreito na analise.")]
        public int corridorTotalNeighborLimit = 4;

        [Header("Pure 3D Cellular Automata")]
        [Tooltip("Quando ativo, executa o Cellular Automata em multiplos pavimentos. A verticalidade e inferida por celulas abertas alinhadas entre camadas.")]
        public bool enableMultiFloorCellularAutomata = false;
        [Tooltip("Quantidade de pavimentos no volume celular.")]
        public int floorCount = 2;
        [Tooltip("Quando ativo, celulas alinhadas em pavimentos vizinhos entram na vizinhanca da regra celular.")]
        public bool includeVerticalNeighborsInRule = true;
        [Tooltip("Numero maximo de conectores verticais inferidos por par de pavimentos. Limita apenas a leitura/visualizacao do resultado, nao repara o mapa.")]
        public int maxVerticalConnectorsPerFloorPair = 2;

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
        [Tooltip("Probabilidade de cada sala extraida receber objetos decorativos da lista de props.")]
        [Range(0f, 1f)] public float propRoomChance = 0.55f;
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
        public string metricsFilePrefix = "cellular_automata";
        [Tooltip("Area minima, em celulas, para considerar uma sala extraida como possivel arena de chefe.")]
        public int bossArenaMinAreaCells = 120;

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
        [Tooltip("Quando ativo, tambem exporta um PNG de massa celular bruta, mostrando celulas abertas e fechadas do Cellular Automata antes da leitura de salas/corredores.")]
        public bool exportCellularMassMap = true;
        [Tooltip("Quando ativo, desenha linhas finas no mapa de massa celular.")]
        public bool cellularMassMapIncludeGrid = false;

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
        private readonly HashSet<string> createdWallEdges = new HashSet<string>();
        private readonly HashSet<string> reservedSpawnCells = new HashSet<string>();
        private readonly Vector2Int[] cardinalDirections =
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        private System.Random rng;
        private Transform dungeonRoot;
        private int resolvedSeed;
        private int propsSpawned;
        private int enemiesSpawned;
        private int lootSpawned;
        private int trapsSpawned;
        private int generatedGameObjectCount;
        private bool[,,] currentAutomataCells;
        private int[,,] roomIdByCell;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateDungeon();
            }
        }

        [ContextMenu("Generate Cellular Automata Dungeon")]
        public void GenerateDungeon()
        {
            int selectedSeed = randomizeSeed ? System.Environment.TickCount : seed;
            GenerateForSeed(selectedSeed, instantiateGeometry, clearBeforeGenerate, true);
        }

        [ContextMenu("Apply Balanced Cave Preset")]
        public void ApplyBalancedCavePreset()
        {
            initialOpenChance = 0.50f;
            smoothingIterations = 4;
            survivalOpenNeighborLimit = 3;
            birthOpenNeighborLimit = 5;
            solidBorderThickness = 1;
            includeDiagonalNeighbors = true;
            minimumRoomComponentArea = 16;
            corridorCardinalNeighborLimit = 2;
            corridorTotalNeighborLimit = 4;
        }

        [ContextMenu("Run Cellular Automata Measurement Test")]
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
            report.algorithmName = "Cellular Automata";
            report.generatedAtUtc = System.DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
            report.runCount = safeRunCount;

            HashSet<string> uniqueHashes = new HashSet<string>();
            for (int i = 0; i < safeRunCount; i++)
            {
                int runSeed = testFirstSeed + i;
                DungeonMetrics metrics = GenerateForSeed(runSeed, measureVisualInstantiationInTests, measureVisualInstantiationInTests, false);
                string topologyHash = ComputeCurrentTopologyHash();
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
                runReport.parameters = CellularAutomataParameterNotes.CreateRunResults(metrics, context);
                report.runs.Add(runReport);
            }

            float diversityRatio = safeRunCount == 0 ? 0f : (float)uniqueHashes.Count / safeRunCount;
            context = CreateReportContext(seedReproducible, safeRunCount, uniqueHashes.Count, diversityRatio);
            report.uniqueTopologyCount = uniqueHashes.Count;
            report.topologyDiversityRatio = diversityRatio;

            for (int i = 0; i < report.runs.Count; i++)
            {
                DungeonQualitativeScorer.ApplyScores(report.runs[i].metrics, diversityRatio, safeRunCount);
                report.runs[i].parameters = CellularAutomataParameterNotes.CreateRunResults(report.runs[i].metrics, context);
            }

            report.aggregateParameters = CellularAutomataParameterNotes.CreateAggregateResults(report.runs, context);
            report.summary = BuildBatchSummary(report, seedReproducible);
            LastBatchReport = report;

            string folder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            DungeonReportPaths paths = DungeonReportExporter.ExportBatchReport(report, folder, metricsFilePrefix);
            Debug.Log("Cellular Automata parameter test exported. Markdown: " + paths.markdownPath + " JSON: " + paths.jsonPath);

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
                Transform existing = transform.Find("Generated Cellular Automata Dungeon");
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
            int safeFloorCount = enableMultiFloorCellularAutomata ? Mathf.Max(2, floorCount) : 1;

            LastLayout = new DungeonLayout(safeWidth, safeDepth, floorHeight, safeFloorCount);
            currentAutomataCells = BuildCellularAutomataVolume(safeWidth, safeDepth, safeFloorCount);
            BuildLayoutFromAutomata(LastLayout, currentAutomataCells);
            ExtractRoomsAndConnections(LastLayout, currentAutomataCells);
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

            Debug.Log("Cellular Automata dungeon generated. Extracted regions: " + LastLayout.rooms.Count + ", seed: " + selectedSeed);
            return LastMetrics;
        }

        private void PrepareGenerationState(int selectedSeed)
        {
            resolvedSeed = selectedSeed;
            rng = new System.Random(resolvedSeed);
            uniqueModules.Clear();
            openWallEdges.Clear();
            createdWallEdges.Clear();
            reservedSpawnCells.Clear();
            propsSpawned = 0;
            enemiesSpawned = 0;
            lootSpawned = 0;
            trapsSpawned = 0;
            generatedGameObjectCount = 0;
            currentAutomataCells = null;
            roomIdByCell = null;
        }

        private bool[,,] BuildCellularAutomataVolume(int width, int depth, int safeFloorCount)
        {
            bool[,,] cells = new bool[width, depth, safeFloorCount];
            int border = Mathf.Max(0, solidBorderThickness);

            for (int floor = 0; floor < safeFloorCount; floor++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        if (IsSolidBorder(x, z, width, depth, border))
                        {
                            cells[x, z, floor] = false;
                        }
                        else
                        {
                            cells[x, z, floor] = rng.NextDouble() < initialOpenChance;
                        }
                    }
                }
            }

            int iterations = Mathf.Max(0, smoothingIterations);
            for (int i = 0; i < iterations; i++)
            {
                bool[,,] next = new bool[width, depth, safeFloorCount];
                for (int floor = 0; floor < safeFloorCount; floor++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < depth; z++)
                        {
                            if (IsSolidBorder(x, z, width, depth, border))
                            {
                                next[x, z, floor] = false;
                                continue;
                            }

                            int openNeighbors = CountOpenNeighbors(cells, x, z, floor);
                            if (cells[x, z, floor])
                            {
                                next[x, z, floor] = openNeighbors >= survivalOpenNeighborLimit;
                            }
                            else
                            {
                                next[x, z, floor] = openNeighbors >= birthOpenNeighborLimit;
                            }
                        }
                    }
                }

                cells = next;
            }

            return cells;
        }

        private void BuildLayoutFromAutomata(DungeonLayout layout, bool[,,] cells)
        {
            for (int floor = 0; floor < layout.floorCount; floor++)
            {
                for (int x = 0; x < layout.width; x++)
                {
                    for (int z = 0; z < layout.depth; z++)
                    {
                        if (!cells[x, z, floor])
                        {
                            continue;
                        }

                        DungeonCellKind kind = IsCorridorLike(cells, x, z, floor) ? DungeonCellKind.Corridor : DungeonCellKind.Room;
                        layout.MarkCell(x, z, floor, kind);
                        uniqueModules.Add(kind == DungeonCellKind.Room ? "ca_open_area" : "ca_narrow_passage");
                    }
                }
            }

            foreach (DungeonGridCell cell in layout.OccupiedGridCells())
            {
                for (int i = 0; i < cardinalDirections.Length; i++)
                {
                    Vector2Int direction = cardinalDirections[i];
                    Vector2Int other = cell.Cell2D + direction;
                    if (layout.InBounds(other, cell.floorIndex) && layout.IsOccupied(other, cell.floorIndex))
                    {
                        openWallEdges.Add(WallEdgeKey(cell.Cell2D, direction, cell.floorIndex));
                    }
                }
            }
        }

        private void ExtractRoomsAndConnections(DungeonLayout layout, bool[,,] cells)
        {
            roomIdByCell = new int[layout.width, layout.depth, layout.floorCount];
            for (int floor = 0; floor < layout.floorCount; floor++)
            {
                for (int x = 0; x < layout.width; x++)
                {
                    for (int z = 0; z < layout.depth; z++)
                    {
                        roomIdByCell[x, z, floor] = -1;
                    }
                }
            }

            bool[,,] visited = new bool[layout.width, layout.depth, layout.floorCount];
            for (int floor = 0; floor < layout.floorCount; floor++)
            {
                for (int x = 0; x < layout.width; x++)
                {
                    for (int z = 0; z < layout.depth; z++)
                    {
                        if (visited[x, z, floor] || layout.cellsByFloor[x, z, floor] != DungeonCellKind.Room)
                        {
                            continue;
                        }

                        List<Vector2Int> component = FloodFillCells(layout, x, z, floor, DungeonCellKind.Room, visited);
                        if (component.Count < Mathf.Max(1, minimumRoomComponentArea))
                        {
                            continue;
                        }

                        AddRoomFromComponent(layout, component, floor, "ca_room_component");
                    }
                }
            }

            if (layout.rooms.Count == 0)
            {
                ExtractOpenComponentsAsRooms(layout);
            }

            ExtractCorridorConnections(layout);
            ExtractVerticalConnections(layout);
        }

        private void ExtractOpenComponentsAsRooms(DungeonLayout layout)
        {
            bool[,,] visited = new bool[layout.width, layout.depth, layout.floorCount];
            for (int floor = 0; floor < layout.floorCount; floor++)
            {
                for (int x = 0; x < layout.width; x++)
                {
                    for (int z = 0; z < layout.depth; z++)
                    {
                        if (visited[x, z, floor] || !layout.IsOccupied(x, z, floor))
                        {
                            continue;
                        }

                        List<Vector2Int> component = FloodFillOccupied(layout, x, z, floor, visited);
                        if (component.Count < Mathf.Max(1, minimumRoomComponentArea))
                        {
                            continue;
                        }

                        AddRoomFromComponent(layout, component, floor, "ca_open_component");
                    }
                }
            }
        }

        private void AddRoomFromComponent(DungeonLayout layout, List<Vector2Int> component, int floor, string moduleId)
        {
            RectInt bounds = BoundsFor(component);
            DungeonRoom room = new DungeonRoom();
            room.id = layout.rooms.Count;
            room.bounds = bounds;
            room.floorIndex = floor;
            room.moduleId = moduleId;
            layout.rooms.Add(room);
            uniqueModules.Add(moduleId);

            for (int i = 0; i < component.Count; i++)
            {
                Vector2Int cell = component[i];
                roomIdByCell[cell.x, cell.y, floor] = room.id;
                layout.MarkCell(cell.x, cell.y, floor, DungeonCellKind.Room);
            }
        }

        private void ExtractCorridorConnections(DungeonLayout layout)
        {
            bool[,,] visited = new bool[layout.width, layout.depth, layout.floorCount];
            for (int floor = 0; floor < layout.floorCount; floor++)
            {
                for (int x = 0; x < layout.width; x++)
                {
                    for (int z = 0; z < layout.depth; z++)
                    {
                        if (visited[x, z, floor] || layout.cellsByFloor[x, z, floor] != DungeonCellKind.Corridor)
                        {
                            continue;
                        }

                        List<Vector2Int> corridorCells = FloodFillCells(layout, x, z, floor, DungeonCellKind.Corridor, visited);
                        Dictionary<int, Vector2Int> touching = FindTouchingRooms(layout, corridorCells, floor);
                        if (touching.Count < 2)
                        {
                            continue;
                        }

                        List<int> roomIds = new List<int>(touching.Keys);
                        roomIds.Sort();
                        for (int a = 0; a < roomIds.Count; a++)
                        {
                            for (int b = a + 1; b < roomIds.Count; b++)
                            {
                                bool loop = roomIds.Count > 2 || layout.connections.Count >= layout.rooms.Count - 1;
                                AddConnection(layout, roomIds[a], roomIds[b], touching[roomIds[a]], touching[roomIds[b]], floor, false, loop);
                            }
                        }
                    }
                }
            }
        }

        private void ExtractVerticalConnections(DungeonLayout layout)
        {
            if (!enableMultiFloorCellularAutomata || layout.floorCount < 2 || roomIdByCell == null)
            {
                return;
            }

            int maxPerPair = Mathf.Max(0, maxVerticalConnectorsPerFloorPair);
            for (int floor = 0; floor < layout.floorCount - 1; floor++)
            {
                int created = 0;
                for (int x = 0; x < layout.width && created < maxPerPair; x++)
                {
                    for (int z = 0; z < layout.depth && created < maxPerPair; z++)
                    {
                        int lowerRoomId = roomIdByCell[x, z, floor];
                        int upperRoomId = roomIdByCell[x, z, floor + 1];
                        if (lowerRoomId < 0 || upperRoomId < 0 || lowerRoomId == upperRoomId)
                        {
                            continue;
                        }

                        if (layout.HasConnection(lowerRoomId, upperRoomId))
                        {
                            continue;
                        }

                        Vector2Int cell = new Vector2Int(x, z);
                        AddConnection(layout, lowerRoomId, upperRoomId, cell, cell, floor, true, false);
                        layout.MarkFloorOpening(x, z, floor + 1);
                        layout.AddMarker(DungeonMapMarkerKind.StairsUp, cell, floor, "Escada");
                        layout.AddMarker(DungeonMapMarkerKind.VerticalExit, cell, floor + 1, "Saida vertical");
                        reservedSpawnCells.Add(SpawnCellKey(cell, floor));
                        reservedSpawnCells.Add(SpawnCellKey(cell, floor + 1));
                        uniqueModules.Add("ca_vertical_overlap");
                        created++;
                    }
                }
            }
        }

        private void AddConnection(DungeonLayout layout, int roomAId, int roomBId, Vector2Int fromCell, Vector2Int toCell, int floor, bool isVertical, bool isExtraLoop)
        {
            if (roomAId == roomBId || roomAId < 0 || roomBId < 0)
            {
                return;
            }

            if (layout.HasConnection(roomAId, roomBId))
            {
                return;
            }

            DungeonRoom roomA = layout.GetRoomById(roomAId);
            DungeonRoom roomB = layout.GetRoomById(roomBId);
            if (roomA == null || roomB == null)
            {
                return;
            }

            DungeonConnection connection = new DungeonConnection();
            connection.roomAId = roomAId;
            connection.roomBId = roomBId;
            connection.fromCell = fromCell;
            connection.toCell = toCell;
            connection.gridDistance = Mathf.Max(1f, Vector2Int.Distance(roomA.CenterCell, roomB.CenterCell));
            connection.isVertical = isVertical;
            connection.isExtraLoop = isExtraLoop;
            layout.connections.Add(connection);

            if (!isVertical)
            {
                openWallEdges.Add(WallEdgeKey(fromCell, DirectionFromTo(fromCell, toCell), floor));
                uniqueModules.Add("ca_corridor_connection");
            }
        }

        private Dictionary<int, Vector2Int> FindTouchingRooms(DungeonLayout layout, List<Vector2Int> corridorCells, int floor)
        {
            Dictionary<int, Vector2Int> touching = new Dictionary<int, Vector2Int>();
            for (int i = 0; i < corridorCells.Count; i++)
            {
                Vector2Int cell = corridorCells[i];
                for (int d = 0; d < cardinalDirections.Length; d++)
                {
                    Vector2Int neighbor = cell + cardinalDirections[d];
                    if (!layout.InBounds(neighbor, floor))
                    {
                        continue;
                    }

                    int roomId = roomIdByCell[neighbor.x, neighbor.y, floor];
                    if (roomId >= 0 && !touching.ContainsKey(roomId))
                    {
                        touching.Add(roomId, cell);
                    }
                }
            }

            return touching;
        }

        private List<Vector2Int> FloodFillCells(DungeonLayout layout, int startX, int startZ, int floor, DungeonCellKind kind, bool[,,] visited)
        {
            List<Vector2Int> result = new List<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(new Vector2Int(startX, startZ));
            visited[startX, startZ, floor] = true;

            while (queue.Count > 0)
            {
                Vector2Int cell = queue.Dequeue();
                result.Add(cell);

                for (int i = 0; i < cardinalDirections.Length; i++)
                {
                    Vector2Int next = cell + cardinalDirections[i];
                    if (!layout.InBounds(next, floor) || visited[next.x, next.y, floor])
                    {
                        continue;
                    }

                    if (layout.cellsByFloor[next.x, next.y, floor] != kind)
                    {
                        continue;
                    }

                    visited[next.x, next.y, floor] = true;
                    queue.Enqueue(next);
                }
            }

            return result;
        }

        private List<Vector2Int> FloodFillOccupied(DungeonLayout layout, int startX, int startZ, int floor, bool[,,] visited)
        {
            List<Vector2Int> result = new List<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(new Vector2Int(startX, startZ));
            visited[startX, startZ, floor] = true;

            while (queue.Count > 0)
            {
                Vector2Int cell = queue.Dequeue();
                result.Add(cell);

                for (int i = 0; i < cardinalDirections.Length; i++)
                {
                    Vector2Int next = cell + cardinalDirections[i];
                    if (!layout.InBounds(next, floor) || visited[next.x, next.y, floor] || !layout.IsOccupied(next, floor))
                    {
                        continue;
                    }

                    visited[next.x, next.y, floor] = true;
                    queue.Enqueue(next);
                }
            }

            return result;
        }

        private void AssignStartAndGoal(DungeonLayout layout)
        {
            if (layout.rooms.Count == 0)
            {
                layout.startRoomId = -1;
                layout.goalRoomId = -1;
                return;
            }

            int startIndex = 0;
            int largestArea = -1;
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                if (layout.rooms[i].AreaCells > largestArea)
                {
                    largestArea = layout.rooms[i].AreaCells;
                    startIndex = i;
                }
            }

            layout.startRoomId = layout.rooms[startIndex].id;
            layout.goalRoomId = layout.startRoomId;

            float bestScore = -1f;
            DungeonRoom start = layout.rooms[startIndex];
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                DungeonRoom candidate = layout.rooms[i];
                float horizontal = Vector2Int.Distance(start.CenterCell, candidate.CenterCell);
                float vertical = Mathf.Abs(start.floorIndex - candidate.floorIndex) * layout.floorHeight / Mathf.Max(0.01f, tileSize);
                float score = horizontal + vertical;
                if (score > bestScore)
                {
                    bestScore = score;
                    layout.goalRoomId = candidate.id;
                }
            }

            DungeonRoom goal = layout.GetRoomById(layout.goalRoomId);
            if (start != null)
            {
                layout.AddMarker(DungeonMapMarkerKind.Start, GetRepresentativeRoomCell(start), start.floorIndex, "Inicio");
            }
            if (goal != null && goal.id != start.id)
            {
                layout.AddMarker(DungeonMapMarkerKind.Goal, GetRepresentativeRoomCell(goal), goal.floorIndex, "Objetivo");
            }
        }

        private DungeonMetrics CreateMetrics(DungeonLayout layout, int selectedSeed)
        {
            bool hasVerticalConnectors = HasVerticalConnections(layout);
            bool hasMultiFloor = layout.floorCount > 1 && hasVerticalConnectors;
            DungeonMetrics metrics = DungeonMetricsCalculator.Calculate(
                layout,
                selectedSeed,
                "Cellular Automata",
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

        private void InstantiateLayout(DungeonLayout layout)
        {
            GameObject rootObject = new GameObject("Generated Cellular Automata Dungeon");
            rootObject.transform.SetParent(transform, false);
            dungeonRoot = rootObject.transform;

            foreach (DungeonGridCell cell in layout.OccupiedGridCells())
            {
                CreateFloor(cell);
            }

            foreach (DungeonGridCell cell in layout.OccupiedGridCells())
            {
                for (int i = 0; i < cardinalDirections.Length; i++)
                {
                    Vector2Int direction = cardinalDirections[i];
                    if (IsOpenWallEdge(cell.Cell2D, direction, cell.floorIndex))
                    {
                        continue;
                    }

                    string edgeKey = WallEdgeKey(cell.Cell2D, direction, cell.floorIndex);
                    if (createdWallEdges.Contains(edgeKey))
                    {
                        continue;
                    }

                    createdWallEdges.Add(edgeKey);
                    CreateWall(cell, direction);
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
            if (LastLayout.IsFloorOpening(cell.Cell2D, cell.floorIndex))
            {
                uniqueModules.Add("ca_floor_opening");
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
            floor.name = "Fallback CA Floor";
            floor.transform.SetParent(dungeonRoot, false);
            floor.transform.position = position + new Vector3(0f, -0.05f, 0f);
            floor.transform.localScale = new Vector3(tileSize, 0.1f, tileSize);
            ApplyMaterial(floor, fallbackFloorMaterial);
            uniqueModules.Add("fallback_floor");
            generatedGameObjectCount++;
        }

        private void CreateWall(DungeonGridCell cell, Vector2Int direction)
        {
            Vector2Int neighbor = cell.Cell2D + direction;
            if (LastLayout.InBounds(neighbor, cell.floorIndex) && LastLayout.IsOccupied(neighbor, cell.floorIndex))
            {
                return;
            }

            Vector3 position = CellToWorld(cell.Cell2D, cell.floorIndex, wallYOffset);
            position += new Vector3(direction.x * tileSize * 0.5f, wallHeight * 0.5f, direction.y * tileSize * 0.5f);
            Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.y), Vector3.up) * Quaternion.Euler(0f, wallYawOffset, 0f);

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
            wall.name = "Fallback CA Wall";
            wall.transform.SetParent(dungeonRoot, false);
            wall.transform.position = position;
            bool northSouth = Mathf.Abs(direction.y) > 0;
            wall.transform.localScale = northSouth
                ? new Vector3(tileSize, wallHeight, wallThickness)
                : new Vector3(wallThickness, wallHeight, tileSize);
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
                Vector2Int cell = connection.fromCell;
                GameObject prefab = assetLibrary != null ? assetLibrary.stairsUpPrefab : null;
                GameObject instance = SpawnOptional(prefab, "ca_stairs_up", CellToWorld(cell, lower.floorIndex, 0f), Quaternion.identity);

                if (instance == null && usePrimitiveFallbacks)
                {
                    GameObject stairs = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    stairs.name = "Fallback CA Stairs";
                    stairs.transform.SetParent(dungeonRoot, false);
                    stairs.transform.position = CellToWorld(cell, lower.floorIndex, 0.3f);
                    stairs.transform.localScale = new Vector3(tileSize, 0.3f, tileSize);
                    ApplyMaterial(stairs, fallbackFloorMaterial);
                    uniqueModules.Add("fallback_stairs");
                    generatedGameObjectCount++;
                }
            }
        }

        private void CreateRoomMarkers(DungeonLayout layout)
        {
            DungeonRoom start = layout.GetRoomById(layout.startRoomId);
            DungeonRoom goal = layout.GetRoomById(layout.goalRoomId);

            if (start != null)
            {
                Vector2Int startCell = GetRepresentativeRoomCell(start);
                reservedSpawnCells.Add(SpawnCellKey(startCell, start.floorIndex));
                SpawnOptional(assetLibrary != null ? assetLibrary.startMarkerPrefab : null, "start_marker", CellToWorld(startCell, start.floorIndex, 0.1f), Quaternion.identity);
            }

            if (goal != null)
            {
                Vector2Int goalCell = GetRepresentativeRoomCell(goal);
                reservedSpawnCells.Add(SpawnCellKey(goalCell, goal.floorIndex));
                SpawnOptional(assetLibrary != null ? assetLibrary.goalMarkerPrefab : null, "goal_marker", CellToWorld(goalCell, goal.floorIndex, 0.1f), Quaternion.identity);
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

        private bool TryGetFreeRoomCell(DungeonRoom room, out Vector2Int cell)
        {
            for (int attempt = 0; attempt < 40; attempt++)
            {
                int x = rng.Next(room.bounds.xMin, room.bounds.xMax);
                int z = rng.Next(room.bounds.yMin, room.bounds.yMax);
                cell = new Vector2Int(x, z);
                if (LastLayout != null &&
                    LastLayout.InBounds(cell, room.floorIndex) &&
                    LastLayout.cellsByFloor[x, z, room.floorIndex] == DungeonCellKind.Room &&
                    !reservedSpawnCells.Contains(SpawnCellKey(cell, room.floorIndex)) &&
                    cell != room.CenterCell)
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

        private void ExportCurrent2DMaps(string exportLabel)
        {
            if (LastLayout == null)
            {
                Debug.LogWarning("No Cellular Automata layout available to export as a 2D map. Generate a dungeon first.");
                return;
            }

            string metricsFolder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            string mapFolder = Path.Combine(metricsFolder, string.IsNullOrEmpty(mapExportSubfolderName) ? "Maps" : mapExportSubfolderName);
            List<string> paths = DungeonMap2DExporter.ExportFloorMaps(
                LastLayout,
                mapFolder,
                metricsFilePrefix,
                "CELLULAR AUTOMATA",
                resolvedSeed,
                exportLabel,
                Mathf.Max(4, mapPixelsPerCell),
                mapIncludeGrid,
                mapIncludeLegend);

            if (exportCellularMassMap)
            {
                paths.AddRange(ExportCurrentCellularMassMaps(mapFolder, exportLabel));
            }

            Debug.Log("Cellular Automata 2D map exported:\n" + string.Join("\n", paths.ToArray()));
        }

        private List<string> ExportCurrentCellularMassMaps(string mapFolder, string exportLabel)
        {
            List<string> paths = new List<string>();
            if (currentAutomataCells == null)
            {
                return paths;
            }

            if (!Directory.Exists(mapFolder))
            {
                Directory.CreateDirectory(mapFolder);
            }

            int pixelsPerCell = Mathf.Clamp(mapPixelsPerCell, 4, 48);
            string safePrefix = string.IsNullOrEmpty(metricsFilePrefix) ? "cellular_automata" : CleanFilePart(metricsFilePrefix);
            string safeLabel = string.IsNullOrEmpty(exportLabel) ? "map" : CleanFilePart(exportLabel);
            string timestamp = System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", System.Globalization.CultureInfo.InvariantCulture);

            for (int floor = 0; floor < currentAutomataCells.GetLength(2); floor++)
            {
                Texture2D texture = RenderCellularMassMap(currentAutomataCells, floor, pixelsPerCell, cellularMassMapIncludeGrid);
                string fileName = safePrefix
                    + "_mass_"
                    + safeLabel
                    + "_seed_"
                    + resolvedSeed.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + "_floor_"
                    + floor.ToString(System.Globalization.CultureInfo.InvariantCulture)
                    + "_"
                    + timestamp
                    + ".png";
                string path = Path.Combine(mapFolder, fileName);
                File.WriteAllBytes(path, texture.EncodeToPNG());
                paths.Add(path);

                if (Application.isPlaying)
                {
                    Destroy(texture);
                }
                else
                {
                    DestroyImmediate(texture);
                }
            }

            return paths;
        }

        private Texture2D RenderCellularMassMap(bool[,,] cells, int floor, int pixelsPerCell, bool includeGrid)
        {
            const int margin = 16;
            const int titleHeight = 0;
            Color32 backgroundColor = new Color32(24, 27, 31, 255);
            Color32 closedColor = new Color32(98, 94, 88, 255);
            Color32 openColor = new Color32(178, 170, 164, 255);
            Color32 gridColor = new Color32(126, 122, 116, 255);

            int cellWidth = cells.GetLength(0);
            int cellDepth = cells.GetLength(1);
            int mapWidthPixels = cellWidth * pixelsPerCell;
            int mapHeightPixels = cellDepth * pixelsPerCell;
            Texture2D texture = new Texture2D(margin + mapWidthPixels + margin, titleHeight + mapHeightPixels + margin, TextureFormat.RGBA32, false);
            Fill(texture, backgroundColor);

            int mapX = margin;
            int mapY = titleHeight;
            for (int x = 0; x < cellWidth; x++)
            {
                for (int z = 0; z < cellDepth; z++)
                {
                    int px = mapX + x * pixelsPerCell;
                    int py = mapY + (cellDepth - 1 - z) * pixelsPerCell;
                    DrawFilledRect(texture, px, py, pixelsPerCell, pixelsPerCell, cells[x, z, floor] ? openColor : closedColor);

                    if (includeGrid)
                    {
                        DrawRect(texture, px, py, pixelsPerCell, pixelsPerCell, gridColor, 1);
                    }
                }
            }

            texture.Apply(false, false);
            return texture;
        }

        private static void Fill(Texture2D texture, Color32 color)
        {
            Color32[] pixels = new Color32[texture.width * texture.height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels32(pixels);
        }

        private static void DrawFilledRect(Texture2D texture, int x, int y, int width, int height, Color32 color)
        {
            int xMin = Mathf.Clamp(x, 0, texture.width);
            int yMin = Mathf.Clamp(y, 0, texture.height);
            int xMax = Mathf.Clamp(x + width, 0, texture.width);
            int yMax = Mathf.Clamp(y + height, 0, texture.height);

            for (int px = xMin; px < xMax; px++)
            {
                for (int py = yMin; py < yMax; py++)
                {
                    texture.SetPixel(px, py, color);
                }
            }
        }

        private static void DrawRect(Texture2D texture, int x, int y, int width, int height, Color32 color, int thickness)
        {
            int safeThickness = Mathf.Max(1, thickness);
            DrawFilledRect(texture, x, y, width, safeThickness, color);
            DrawFilledRect(texture, x, y + height - safeThickness, width, safeThickness, color);
            DrawFilledRect(texture, x, y, safeThickness, height, color);
            DrawFilledRect(texture, x + width - safeThickness, y, safeThickness, height, color);
        }

        private static string CleanFilePart(string text)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (char.IsLetterOrDigit(c) || c == '_' || c == '-')
                {
                    builder.Append(char.ToLowerInvariant(c));
                }
                else if (char.IsWhiteSpace(c))
                {
                    builder.Append('_');
                }
            }

            return builder.Length == 0 ? "map" : builder.ToString();
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
            context.supportsVerticalConnectors = enableMultiFloorCellularAutomata && floorCount > 1;
            context.supportsMultiFloor = enableMultiFloorCellularAutomata && floorCount > 1;
            context.runCount = runCount;
            context.uniqueTopologyCount = uniqueTopologyCount;
            context.topologyDiversityRatio = diversityRatio;
            return context;
        }

        private bool VerifySeedReproducibility(int seedToVerify)
        {
            GenerateForSeed(seedToVerify, false, false, false);
            string firstHash = ComputeCurrentTopologyHash();
            GenerateForSeed(seedToVerify, false, false, false);
            string secondHash = ComputeCurrentTopologyHash();
            return firstHash == secondHash;
        }

        private static string BuildBatchSummary(DungeonBatchReport report, bool seedReproducible)
        {
            return "Teste Cellular Automata executado com " + report.runCount
                + " seed(s). Topologias unicas: " + report.uniqueTopologyCount
                + "/" + report.runCount
                + ". Diversidade topologica: " + (report.topologyDiversityRatio * 100f).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)
                + "%. Reprodutibilidade por seed: " + (seedReproducible ? "aprovada" : "falhou") + ".";
        }

        private string ComputeCurrentTopologyHash()
        {
            if (LastLayout == null || currentAutomataCells == null)
            {
                return LastLayout == null ? "NO_LAYOUT" : DungeonTopologyHasher.Compute(LastLayout);
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("CA|")
                .Append(currentAutomataCells.GetLength(0)).Append("x")
                .Append(currentAutomataCells.GetLength(1)).Append("x")
                .Append(currentAutomataCells.GetLength(2)).Append("|");

            for (int floor = 0; floor < currentAutomataCells.GetLength(2); floor++)
            {
                builder.Append("F").Append(floor).Append(":");
                for (int z = 0; z < currentAutomataCells.GetLength(1); z++)
                {
                    for (int x = 0; x < currentAutomataCells.GetLength(0); x++)
                    {
                        builder.Append(currentAutomataCells[x, z, floor] ? "1" : "0");
                    }
                }
                builder.Append("|");
            }

            builder.Append(DungeonTopologyHasher.Compute(LastLayout));
            return Fnv1A(builder.ToString());
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
                        if (layout.InBounds(cell, room.floorIndex) &&
                            layout.cellsByFloor[x, z, room.floorIndex] == DungeonCellKind.Room &&
                            !reservedSpawnCells.Contains(SpawnCellKey(cell, room.floorIndex)) &&
                            cell != room.CenterCell)
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
            return Mathf.Max(0, enemyBudget) + Mathf.Max(0, lootBudget) + Mathf.Max(0, trapBudget) <= spawnableCells;
        }

        private Vector2Int GetRepresentativeRoomCell(DungeonRoom room)
        {
            Vector2Int center = room.CenterCell;
            if (LastLayout == null)
            {
                return center;
            }

            Vector2Int best = center;
            float bestDistance = float.PositiveInfinity;
            for (int x = room.bounds.xMin; x < room.bounds.xMax; x++)
            {
                for (int z = room.bounds.yMin; z < room.bounds.yMax; z++)
                {
                    Vector2Int cell = new Vector2Int(x, z);
                    if (!LastLayout.InBounds(cell, room.floorIndex) ||
                        LastLayout.cellsByFloor[x, z, room.floorIndex] != DungeonCellKind.Room)
                    {
                        continue;
                    }

                    float distance = Vector2Int.Distance(center, cell);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        best = cell;
                    }
                }
            }

            return best;
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

        private bool IsCorridorLike(bool[,,] cells, int x, int z, int floor)
        {
            int cardinal = CountCardinalOpenNeighbors(cells, x, z, floor);
            int total2D = CountOpenNeighbors2D(cells, x, z, floor);
            return cardinal <= Mathf.Max(0, corridorCardinalNeighborLimit) &&
                   total2D <= Mathf.Max(0, corridorTotalNeighborLimit);
        }

        private int CountOpenNeighbors(bool[,,] cells, int x, int z, int floor)
        {
            int count = CountOpenNeighbors2D(cells, x, z, floor);
            if (includeVerticalNeighborsInRule && enableMultiFloorCellularAutomata)
            {
                if (floor > 0 && cells[x, z, floor - 1]) count++;
                if (floor < cells.GetLength(2) - 1 && cells[x, z, floor + 1]) count++;
            }

            return count;
        }

        private int CountOpenNeighbors2D(bool[,,] cells, int x, int z, int floor)
        {
            int count = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dz == 0)
                    {
                        continue;
                    }

                    if (!includeDiagonalNeighbors && Mathf.Abs(dx) + Mathf.Abs(dz) > 1)
                    {
                        continue;
                    }

                    int nx = x + dx;
                    int nz = z + dz;
                    if (nx < 0 || nz < 0 || nx >= cells.GetLength(0) || nz >= cells.GetLength(1))
                    {
                        continue;
                    }

                    if (cells[nx, nz, floor])
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private int CountCardinalOpenNeighbors(bool[,,] cells, int x, int z, int floor)
        {
            int count = 0;
            for (int i = 0; i < cardinalDirections.Length; i++)
            {
                int nx = x + cardinalDirections[i].x;
                int nz = z + cardinalDirections[i].y;
                if (nx < 0 || nz < 0 || nx >= cells.GetLength(0) || nz >= cells.GetLength(1))
                {
                    continue;
                }

                if (cells[nx, nz, floor])
                {
                    count++;
                }
            }

            return count;
        }

        private static bool IsSolidBorder(int x, int z, int width, int depth, int border)
        {
            return border > 0 && (x < border || z < border || x >= width - border || z >= depth - border);
        }

        private static RectInt BoundsFor(List<Vector2Int> cells)
        {
            int minX = int.MaxValue;
            int minZ = int.MaxValue;
            int maxX = int.MinValue;
            int maxZ = int.MinValue;

            for (int i = 0; i < cells.Count; i++)
            {
                Vector2Int cell = cells[i];
                if (cell.x < minX) minX = cell.x;
                if (cell.y < minZ) minZ = cell.y;
                if (cell.x > maxX) maxX = cell.x;
                if (cell.y > maxZ) maxZ = cell.y;
            }

            return new RectInt(minX, minZ, maxX - minX + 1, maxZ - minZ + 1);
        }

        private static Vector2Int DirectionFromTo(Vector2Int from, Vector2Int to)
        {
            Vector2Int delta = to - from;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return new Vector2Int(delta.x >= 0 ? 1 : -1, 0);
            }

            return new Vector2Int(0, delta.y >= 0 ? 1 : -1);
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

        private Vector3 CellToWorld(Vector2Int cell, int floorIndex, float yOffset)
        {
            float originX = centerOnOrigin ? -mapWidth * tileSize * 0.5f : 0f;
            float originZ = centerOnOrigin ? -mapDepth * tileSize * 0.5f : 0f;
            return new Vector3(originX + cell.x * tileSize, floorIndex * floorHeight + yOffset, originZ + cell.y * tileSize);
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

        private static string SpawnCellKey(Vector2Int cell, int floorIndex)
        {
            return floorIndex + ":" + cell.x + "," + cell.y;
        }

        private static float BytesToMegabytes(long bytes)
        {
            return bytes / (1024f * 1024f);
        }

        private static string Fnv1A(string text)
        {
            unchecked
            {
                uint hash = 2166136261;
                for (int i = 0; i < text.Length; i++)
                {
                    hash ^= text[i];
                    hash *= 16777619;
                }

                return hash.ToString("X8", System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }

    public static class CellularAutomataParameterNotes
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
            if (parameterName == "numRoomsTarget") return "Extracao de componentes abertos e densos apos iteracoes de Cellular Automata.";
            if (parameterName == "connectivityRatio") return "Busca no grafo extraido de componentes abertos; nenhuma conexao artificial e criada.";
            if (parameterName == "verticalVariance") return "Desvio padrao das alturas dos componentes extraidos do volume celular.";
            if (parameterName == "fillPercentage") return "Celulas abertas apos a regra celular divididas pelo total do volume.";
            if (parameterName == "branchFactor") return "Media de conexoes entre componentes extraidos por passagens estreitas detectadas.";
            if (parameterName == "avgPathLength") return "Distancias no grafo extraido do resultado celular.";
            if (parameterName == "uniqueModules") return "Contagem de categorias logicas e prefabs usados na leitura/instanciacao do resultado.";
            if (parameterName == "navigableVolumeRatio") return "Estimativa logica de celulas abertas antes de NavMesh.";
            if (parameterName == "criticalPathLength") return "Maior distancia no grafo extraido a partir do componente inicial.";
            if (parameterName == "avgAlternativePathLength") return "Media de conexoes marcadas como ciclos emergentes entre componentes.";
            if (parameterName == "SupportsVerticalConnectors") return "Conta execucoes com celulas abertas alinhadas entre pavimentos e convertidas em conector vertical inferido.";
            if (parameterName == "SupportsMultiFloor") return "Conta execucoes com mais de um pavimento e pelo menos uma conexao vertical inferida do volume celular.";
            if (parameterName == "layoutGenerationMilliseconds") return "Tempo para inicializar ruido, executar iteracoes celulares e extrair componentes.";
            if (parameterName == "connectionCount") return "Quantidade de conexoes inferidas por gargalos/passagens ou sobreposicoes verticais.";
            return "Parametro medido sobre a saida do Cellular Automata puro.";
        }

        private static string InterpretationFor(string parameterName, string existing)
        {
            if (parameterName == "connectivityRatio") return "Mostra se as cavernas emergentes ficaram conectadas sem flood-fill reparador, carving ou grafo-guia.";
            if (parameterName == "SupportsBacktrackingLoops") return "Parametro atendido somente quando as conexoes emergentes formam ciclos no grafo extraido.";
            if (parameterName == "SupportsVerticalConnectors") return "Parametro atendido apenas se o volume celular multiandar gerar alinhamentos abertos entre camadas.";
            if (parameterName == "SupportsMultiFloor") return "Parametro atendido quando existe conexao vertical inferida do proprio volume celular.";
            if (parameterName == "Debuggability") return "Cellular Automata e reproduzivel e simples de parametrizar, mas o efeito de pequenas mudancas nas regras precisa ser observado por bateladas.";
            if (parameterName == "Legibility") return "A legibilidade depende do equilibrio entre areas abertas, gargalos e ilhas desconectadas.";
            return existing.Replace("BSP", "Cellular Automata").Replace("bsp", "cellular_automata").Replace("Room Graph", "Cellular Automata").Replace("WFC", "Cellular Automata");
        }

        private static string NoteFor(string parameterName)
        {
            if (parameterName == "numRoomsTarget") return "Cellular Automata nao cria salas como entidades planejadas; regioes emergem de celulas abertas.";
            if (parameterName == "connectivityRatio") return "CA puro nao garante conectividade global. Se a metrica ficar baixa, isso e uma limitacao real do algoritmo nesta configuracao.";
            if (parameterName == "verticalVariance") return "Verticalidade so aparece em uma variante celular 3D/multiandar; CA 2D simples nao possui altura.";
            if (parameterName == "fillPercentage") return "Densidade e controlada indiretamente por chance inicial e limites de nascimento/sobrevivencia.";
            if (parameterName == "branchFactor") return "Ramificacao emerge de gargalos naturais, nao de um grafo explicito.";
            if (parameterName == "avgPathLength") return "Caminhos sao medidos depois da extracao; CA nao controla caminho critico diretamente.";
            if (parameterName == "uniqueModules") return "CA tende a variar forma organica, mas nao aumenta variedade modular sozinho.";
            if (parameterName == "navigableVolumeRatio") return "Proxy logico; ilhas desconectadas podem inflar area aberta sem criar navegabilidade util.";
            if (parameterName == "criticalPathLength") return "CA puro nao direciona progressao inicio-fim sem uma camada adicional.";
            if (parameterName == "avgAlternativePathLength") return "Loops podem emergir, mas nao sao garantidos.";
            if (parameterName == "SupportsRandomEnemySpawns") return "Spawns usam regioes abertas extraidas; balanceamento nao e propriedade nativa do CA.";
            if (parameterName == "SupportsLootDistribution") return "Loot pode ser colocado nas regioes abertas, mas progressao por risco depende de outra camada.";
            if (parameterName == "SupportsTraps") return "Armadilhas podem usar celulas abertas/gargalos, mas sem semantica global nativa.";
            if (parameterName == "SupportsBacktrackingLoops") return "CA pode formar ciclos emergentes, mas sem controle direto de loops.";
            if (parameterName == "SupportsVerticalConnectors") return "Possivel apenas em uma leitura 3D/multiandar do CA, por alinhamento de celulas abertas entre camadas.";
            if (parameterName == "SupportsMultiFloor") return "CA 2D puro nao e multiandar; a variante 3D continua celular, mas precisa ser relatada como extensao volumetrica.";
            if (parameterName == "SupportsBossArena") return "Areas grandes podem emergir naturalmente, mas nao como arena semanticamente planejada.";
            if (parameterName == "SeedReproducible") return "Resultado reproduzivel se ruido inicial e iteracoes usam a mesma seed.";
            if (parameterName == "RuntimeRegeneration") return "Custo cresce com tamanho do grid, pavimentos e iteracoes.";
            if (parameterName == "BudgetAwareSpawns") return "Orcamento e aplicado apos a geracao, sobre celulas livres extraidas.";
            if (parameterName == "Replayability") return "Alta variacao visual/organica entre seeds e esperada.";
            if (parameterName == "Debuggability") return "Regras sao simples, mas efeitos globais emergentes exigem analise por metricas.";
            if (parameterName == "Flow") return "Fluxo global nao e garantido; depende de conectividade emergente.";
            if (parameterName == "Legibility") return "Cavernas organicas podem ser bonitas, mas menos legiveis que salas retangulares.";
            if (parameterName == "StructuralVariety") return "Boa variedade morfologica; menor controle sobre semantica estrutural.";
            if (parameterName == "layoutGenerationMilliseconds") return "Inclui ruido inicial, iteracoes celulares e extracao de componentes.";
            if (parameterName == "geometryInstantiationMilliseconds") return "Custo visual depende dos prefabs/Unity, nao do CA puro.";
            if (parameterName == "metricsCalculationMilliseconds") return "Custo da instrumentacao, nao do algoritmo.";
            if (parameterName == "totalGenerationMilliseconds") return "Inclui layout, instanciacao visual e metricas; compare junto do tempo logico.";
            if (parameterName == "generatedGameObjectCount") return "Reflete peso da montagem visual das celulas abertas.";
            if (parameterName == "occupiedCellCount") return "Proxy do volume aberto gerado pelo CA.";
            if (parameterName == "connectionCount") return "Conexoes sao inferidas apos a geracao; nao sao primitivas nativas do CA.";
            if (parameterName == "managedMemoryDeltaKB") return "Estimativa sujeita ao GC da Unity; use como indicio comparativo.";
            return "Parametro avaliado na variante Cellular Automata pura.";
        }
    }
}
