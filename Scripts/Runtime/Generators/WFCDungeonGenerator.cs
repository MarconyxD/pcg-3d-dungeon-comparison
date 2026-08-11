using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Dissertation.PCG
{
    public sealed class WFCDungeonGenerator : MonoBehaviour
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
        [Tooltip("Quantidade minima de celulas ocupadas para aceitar um colapso WFC. Se ficar abaixo, a execucao reinicia com a mesma seed derivada.")]
        public int minimumOccupiedCellsForAcceptedCollapse = 280;
        [Tooltip("Quantidade minima de celulas em um componente de sala para que ele seja contado como sala mensuravel.")]
        public int minimumRoomComponentArea = 8;
        [Tooltip("Quantidade maxima de reinicios do WFC quando ocorre contradicao ou quando o resultado fica vazio demais.")]
        public int maxCollapseRestarts = 20;

        [Header("Pure WFC module weights")]
        [Tooltip("Peso do modulo vazio/parede solida no colapso.")]
        public float emptyWeight = 2.4f;
        [Tooltip("Peso dos modulos de sala no colapso.")]
        public float roomWeight = 3.2f;
        [Tooltip("Peso dos modulos de corredor no colapso.")]
        public float corridorWeight = 1.35f;
        [Tooltip("Peso dos modulos de conector vertical no colapso.")]
        public float verticalConnectorWeight = 0.2f;
        [Tooltip("Quantidade de observacoes iniciais de sala por pavimento. Isto e uma restricao inicial do WFC, nao um reparo pos-processamento.")]
        public int roomObservationsPerFloor = 5;
        [Tooltip("Quando ativo, fixa uma espinha dorsal de corredores como observacao inicial do WFC para favorecer um componente navegavel conexo.")]
        public bool constrainConnectedBackbone = true;
        [Tooltip("Quantidade de ramos observados a partir da espinha dorsal de corredores em cada pavimento.")]
        public int backboneBranchesPerFloor = 6;
        [Tooltip("Comprimento minimo dos ramos observados da espinha dorsal, em macro-celulas WFC.")]
        public int minBackboneBranchLength = 3;
        [Tooltip("Comprimento maximo dos ramos observados da espinha dorsal, em macro-celulas WFC.")]
        public int maxBackboneBranchLength = 7;

        [Header("Pure WFC verticality")]
        [Tooltip("Quando ativo, o WFC usa modulos com sockets verticais para gerar pavimentos conectaveis.")]
        public bool enableMultiFloorWfc = true;
        [Tooltip("Quantidade de pavimentos no volume WFC.")]
        public int floorCount = 2;
        [Tooltip("Quantidade de pares observados de escada/conector vertical entre pavimentos adjacentes. Isto forca a presenca de modulos verticais no proprio WFC.")]
        public int verticalConnectorObservationsPerFloorPair = 1;

        [Header("Pure WFC macro modules")]
        [Tooltip("Raio pintado por cada modulo de sala colapsado. Valor 1 gera blocos 3x3; valor 2 gera blocos 5x5. Isto representa tiles WFC maiores, nao reparo externo.")]
        public int roomBrushRadius = 1;
        [Tooltip("Meia largura das faixas de corredor geradas pelos modulos de corredor. Valor 1 gera corredores com 3 celulas de largura.")]
        public int corridorHalfWidth = 1;
        [Tooltip("Quando ativo, celulas ocupadas adjacentes geradas pelos macro-tiles ficam navegavelmente abertas entre si.")]
        public bool openAdjacentPaintedCells = true;

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
        [Range(0f, 1f)] public float propRoomChance = 0.65f;
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
        public string metricsFilePrefix = "wfc";
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

        private const int North = 0;
        private const int East = 1;
        private const int South = 2;
        private const int West = 3;

        private readonly HashSet<string> uniqueModules = new HashSet<string>();
        private readonly HashSet<string> openWallEdges = new HashSet<string>();
        private readonly HashSet<string> reservedSpawnCells = new HashSet<string>();
        private readonly HashSet<string> createdWallEdges = new HashSet<string>();
        private readonly List<WfcModule> modules = new List<WfcModule>();
        private readonly Dictionary<string, int> moduleIndexById = new Dictionary<string, int>();

        private System.Random rng;
        private Transform dungeonRoot;
        private int resolvedSeed;
        private int propsSpawned;
        private int enemiesSpawned;
        private int lootSpawned;
        private int trapsSpawned;
        private int generatedGameObjectCount;
        private int macroTileStride = 1;
        private int[,,] collapsedModules;

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

        [ContextMenu("Generate WFC Dungeon")]
        public void GenerateDungeon()
        {
            int selectedSeed = randomizeSeed ? System.Environment.TickCount : seed;
            GenerateForSeed(selectedSeed, instantiateGeometry, clearBeforeGenerate, true);
        }

        [ContextMenu("Run WFC Measurement Test")]
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
            report.algorithmName = "Wave Function Collapse";
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
                runReport.parameters = WFCParameterNotes.CreateRunResults(metrics, context);
                report.runs.Add(runReport);
            }

            float diversityRatio = safeRunCount == 0 ? 0f : (float)uniqueHashes.Count / safeRunCount;
            context = CreateReportContext(seedReproducible, safeRunCount, uniqueHashes.Count, diversityRatio);
            report.uniqueTopologyCount = uniqueHashes.Count;
            report.topologyDiversityRatio = diversityRatio;

            for (int i = 0; i < report.runs.Count; i++)
            {
                DungeonQualitativeScorer.ApplyScores(report.runs[i].metrics, diversityRatio, safeRunCount);
                report.runs[i].parameters = WFCParameterNotes.CreateRunResults(report.runs[i].metrics, context);
            }

            report.aggregateParameters = WFCParameterNotes.CreateAggregateResults(report.runs, context);
            report.summary = BuildBatchSummary(report, seedReproducible);
            LastBatchReport = report;

            string folder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            DungeonReportPaths paths = DungeonReportExporter.ExportBatchReport(report, folder, metricsFilePrefix);
            Debug.Log("WFC parameter test exported. Markdown: " + paths.markdownPath + " JSON: " + paths.jsonPath);

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
                Transform existing = transform.Find("Generated WFC Dungeon");
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
            int effectiveFloorCount = enableMultiFloorWfc ? Mathf.Max(2, floorCount) : 1;
            LastLayout = new DungeonLayout(safeWidth, safeDepth, floorHeight, effectiveFloorCount);
            BuildModules(effectiveFloorCount);

            macroTileStride = CalculateMacroTileStride();
            int wfcWidth = Mathf.Max(4, Mathf.CeilToInt(safeWidth / (float)macroTileStride));
            int wfcDepth = Mathf.Max(4, Mathf.CeilToInt(safeDepth / (float)macroTileStride));

            collapsedModules = CollapseWfcVolume(wfcWidth, wfcDepth, effectiveFloorCount);
            BuildLayoutFromCollapsedModules(LastLayout, collapsedModules);
            ExtractRoomsAndConnections(LastLayout, collapsedModules);
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

            Debug.Log("WFC dungeon generated. Rooms: " + LastLayout.rooms.Count + ", seed: " + selectedSeed);
            return LastMetrics;
        }

        private void PrepareGenerationState(int selectedSeed)
        {
            resolvedSeed = selectedSeed;
            rng = new System.Random(resolvedSeed);
            uniqueModules.Clear();
            openWallEdges.Clear();
            reservedSpawnCells.Clear();
            createdWallEdges.Clear();
            modules.Clear();
            moduleIndexById.Clear();
            propsSpawned = 0;
            enemiesSpawned = 0;
            lootSpawned = 0;
            trapsSpawned = 0;
            generatedGameObjectCount = 0;
        }

        private void BuildModules(int effectiveFloorCount)
        {
            AddModule("empty", DungeonCellKind.Empty, 0, false, false, false, false, Mathf.Max(0.01f, emptyWeight));

            AddModule("room_center", DungeonCellKind.Room, OpenMask(true, true, true, true), false, false, false, false, Mathf.Max(0.01f, roomWeight));
            AddModule("room_edge_n", DungeonCellKind.Room, OpenMask(false, true, true, true), false, false, false, false, Mathf.Max(0.01f, roomWeight));
            AddModule("room_edge_e", DungeonCellKind.Room, OpenMask(true, false, true, true), false, false, false, false, Mathf.Max(0.01f, roomWeight));
            AddModule("room_edge_s", DungeonCellKind.Room, OpenMask(true, true, false, true), false, false, false, false, Mathf.Max(0.01f, roomWeight));
            AddModule("room_edge_w", DungeonCellKind.Room, OpenMask(true, true, true, false), false, false, false, false, Mathf.Max(0.01f, roomWeight));
            AddModule("room_corner_ne", DungeonCellKind.Room, OpenMask(false, false, true, true), false, false, false, false, Mathf.Max(0.01f, roomWeight));
            AddModule("room_corner_se", DungeonCellKind.Room, OpenMask(true, false, false, true), false, false, false, false, Mathf.Max(0.01f, roomWeight));
            AddModule("room_corner_sw", DungeonCellKind.Room, OpenMask(true, true, false, false), false, false, false, false, Mathf.Max(0.01f, roomWeight));
            AddModule("room_corner_nw", DungeonCellKind.Room, OpenMask(false, true, true, false), false, false, false, false, Mathf.Max(0.01f, roomWeight));

            AddModule("corridor_ns", DungeonCellKind.Corridor, OpenMask(true, false, true, false), false, false, false, false, Mathf.Max(0.01f, corridorWeight));
            AddModule("corridor_ew", DungeonCellKind.Corridor, OpenMask(false, true, false, true), false, false, false, false, Mathf.Max(0.01f, corridorWeight));
            AddModule("corridor_dead_n", DungeonCellKind.Corridor, OpenMask(true, false, false, false), false, false, false, false, Mathf.Max(0.01f, corridorWeight * 0.25f));
            AddModule("corridor_dead_e", DungeonCellKind.Corridor, OpenMask(false, true, false, false), false, false, false, false, Mathf.Max(0.01f, corridorWeight * 0.25f));
            AddModule("corridor_dead_s", DungeonCellKind.Corridor, OpenMask(false, false, true, false), false, false, false, false, Mathf.Max(0.01f, corridorWeight * 0.25f));
            AddModule("corridor_dead_w", DungeonCellKind.Corridor, OpenMask(false, false, false, true), false, false, false, false, Mathf.Max(0.01f, corridorWeight * 0.25f));
            AddModule("corridor_ne", DungeonCellKind.Corridor, OpenMask(true, true, false, false), false, false, false, false, Mathf.Max(0.01f, corridorWeight));
            AddModule("corridor_es", DungeonCellKind.Corridor, OpenMask(false, true, true, false), false, false, false, false, Mathf.Max(0.01f, corridorWeight));
            AddModule("corridor_sw", DungeonCellKind.Corridor, OpenMask(false, false, true, true), false, false, false, false, Mathf.Max(0.01f, corridorWeight));
            AddModule("corridor_wn", DungeonCellKind.Corridor, OpenMask(true, false, false, true), false, false, false, false, Mathf.Max(0.01f, corridorWeight));
            AddModule("corridor_cross", DungeonCellKind.Corridor, OpenMask(true, true, true, true), false, false, false, false, Mathf.Max(0.01f, corridorWeight * 0.35f));
            AddModule("corridor_t_n", DungeonCellKind.Corridor, OpenMask(true, true, false, true), false, false, false, false, Mathf.Max(0.01f, corridorWeight * 0.55f));
            AddModule("corridor_t_e", DungeonCellKind.Corridor, OpenMask(true, true, true, false), false, false, false, false, Mathf.Max(0.01f, corridorWeight * 0.55f));
            AddModule("corridor_t_s", DungeonCellKind.Corridor, OpenMask(false, true, true, true), false, false, false, false, Mathf.Max(0.01f, corridorWeight * 0.55f));
            AddModule("corridor_t_w", DungeonCellKind.Corridor, OpenMask(true, false, true, true), false, false, false, false, Mathf.Max(0.01f, corridorWeight * 0.55f));

            if (enableMultiFloorWfc && effectiveFloorCount > 1)
            {
                AddModule("stairs_up", DungeonCellKind.Corridor, OpenMask(true, true, true, true), true, false, false, true, Mathf.Max(0.01f, verticalConnectorWeight));
                AddModule("stairs_down", DungeonCellKind.Corridor, OpenMask(true, true, true, true), false, true, true, false, Mathf.Max(0.01f, verticalConnectorWeight));
            }
        }

        private int CalculateMacroTileStride()
        {
            int roomSize = Mathf.Clamp(roomBrushRadius, 0, 4) * 2 + 1;
            int corridorSize = Mathf.Clamp(corridorHalfWidth, 0, 4) * 2 + 1;
            return Mathf.Max(1, Mathf.Max(roomSize, corridorSize));
        }

        private void AddModule(string id, DungeonCellKind cellKind, int openMask, bool opensUp, bool opensDown, bool acceptsUp, bool acceptsDown, float weight)
        {
            WfcModule module = new WfcModule();
            module.id = id;
            module.cellKind = cellKind;
            module.openMask = openMask;
            module.opensUp = opensUp;
            module.opensDown = opensDown;
            module.acceptsUp = acceptsUp;
            module.acceptsDown = acceptsDown;
            module.weight = weight;
            moduleIndexById[id] = modules.Count;
            modules.Add(module);
        }

        private int[,,] CollapseWfcVolume(int width, int depth, int floors)
        {
            int paintedCellsPerCollapsedModule = Mathf.Max(1, macroTileStride * macroTileStride);
            int acceptedMinimum = Mathf.Max(1, Mathf.CeilToInt(minimumOccupiedCellsForAcceptedCollapse / (float)paintedCellsPerCollapsedModule));
            int restarts = Mathf.Max(1, maxCollapseRestarts);
            int[,,] best = null;
            int bestOccupied = -1;

            for (int restart = 0; restart < restarts; restart++)
            {
                int[,,] domains = CreateInitialDomains(width, depth, floors);
                System.Random restartRng = new System.Random(resolvedSeed + restart * 7919);
                ApplyInitialObservations(domains, restartRng);

                bool success = CollapseDomains(domains, restartRng);
                if (!success)
                {
                    continue;
                }

                int[,,] collapsed = ResolveCollapsedModules(domains);
                int occupied = CountOccupiedCollapsed(collapsed);
                if (occupied > bestOccupied)
                {
                    best = collapsed;
                    bestOccupied = occupied;
                }

                if (occupied >= acceptedMinimum)
                {
                    return collapsed;
                }
            }

            if (best != null)
            {
                Debug.LogWarning("WFC accepted the best available collapse, but it did not reach Minimum Occupied Cells. This is a WFC limitation visible in the metrics.");
                return best;
            }

            Debug.LogWarning("WFC reached contradictions in all attempts. Returning an empty collapse so the limitation is visible in the metrics.");
            return CreateEmptyCollapsed(width, depth, floors);
        }

        private int[,,] CreateInitialDomains(int width, int depth, int floors)
        {
            int fullMask = (1 << modules.Count) - 1;
            int[,,] domains = new int[width, depth, floors];
            for (int floor = 0; floor < floors; floor++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        int mask = fullMask;
                        for (int i = 0; i < modules.Count; i++)
                        {
                            WfcModule module = modules[i];
                            bool allowed = true;
                            if (x == 0 && module.IsOpen(West)) allowed = false;
                            if (x == width - 1 && module.IsOpen(East)) allowed = false;
                            if (z == 0 && module.IsOpen(South)) allowed = false;
                            if (z == depth - 1 && module.IsOpen(North)) allowed = false;
                            if (floor == floors - 1 && module.opensUp) allowed = false;
                            if (floor == 0 && module.opensDown) allowed = false;
                            if (!enableMultiFloorWfc && (module.opensUp || module.opensDown || module.acceptsUp || module.acceptsDown)) allowed = false;
                            if (!allowed)
                            {
                                mask &= ~(1 << i);
                            }
                        }

                        domains[x, z, floor] = mask;
                    }
                }
            }

            return domains;
        }

        private void ApplyInitialObservations(int[,,] domains, System.Random observationRng)
        {
            int width = domains.GetLength(0);
            int depth = domains.GetLength(1);
            int floors = domains.GetLength(2);
            int roomCenterMask = MaskFor("room_center");
            int stairUpMask = moduleIndexById.ContainsKey("stairs_up") ? MaskFor("stairs_up") : 0;
            int stairDownMask = moduleIndexById.ContainsKey("stairs_down") ? MaskFor("stairs_down") : 0;

            for (int floor = 0; floor < floors; floor++)
            {
                bool[,] reservedBackboneCells = ApplyConnectivityBackbone(domains, observationRng, floor);
                int observations = Mathf.Max(0, roomObservationsPerFloor);
                for (int i = 0; i < observations; i++)
                {
                    int x = 0;
                    int z = 0;
                    bool found = false;
                    for (int attempt = 0; attempt < 40; attempt++)
                    {
                        x = observationRng.Next(2, Mathf.Max(3, width - 2));
                        z = observationRng.Next(2, Mathf.Max(3, depth - 2));
                        if (!IsReservedBackboneNeighborhood(reservedBackboneCells, x, z) && CountBits(domains[x, z, floor]) > 1)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        continue;
                    }

                    domains[x, z, floor] = roomCenterMask;
                }
            }

            if (!enableMultiFloorWfc || stairUpMask == 0 || stairDownMask == 0)
            {
                return;
            }

            int verticalObservations = Mathf.Max(0, verticalConnectorObservationsPerFloorPair);
            for (int floor = 0; floor < floors - 1; floor++)
            {
                for (int i = 0; i < verticalObservations; i++)
                {
                    int x = observationRng.Next(3, Mathf.Max(4, width - 3));
                    int z = observationRng.Next(3, Mathf.Max(4, depth - 3));
                    if (CountBits(domains[x, z, floor]) == 1 || CountBits(domains[x, z, floor + 1]) == 1)
                    {
                        continue;
                    }

                    domains[x, z, floor] = stairUpMask;
                    domains[x, z, floor + 1] = stairDownMask;
                }
            }
        }

        private bool[,] ApplyConnectivityBackbone(int[,,] domains, System.Random observationRng, int floor)
        {
            int width = domains.GetLength(0);
            int depth = domains.GetLength(1);
            bool[,] path = new bool[width, depth];
            if (!constrainConnectedBackbone || width < 5 || depth < 5)
            {
                return path;
            }

            int centerX = width / 2;
            int centerZ = depth / 2;
            for (int x = 1; x < width - 1; x++)
            {
                path[x, centerZ] = true;
            }

            for (int z = 1; z < depth - 1; z++)
            {
                path[centerX, z] = true;
            }

            int branchCount = Mathf.Max(0, backboneBranchesPerFloor);
            for (int i = 0; i < branchCount; i++)
            {
                bool fromHorizontal = observationRng.NextDouble() < 0.5;
                int startX = fromHorizontal ? observationRng.Next(2, Mathf.Max(3, width - 2)) : centerX;
                int startZ = fromHorizontal ? centerZ : observationRng.Next(2, Mathf.Max(3, depth - 2));
                Vector2Int direction;
                if (fromHorizontal)
                {
                    direction = observationRng.NextDouble() < 0.5 ? new Vector2Int(0, 1) : new Vector2Int(0, -1);
                }
                else
                {
                    direction = observationRng.NextDouble() < 0.5 ? new Vector2Int(1, 0) : new Vector2Int(-1, 0);
                }

                int length = observationRng.Next(
                    Mathf.Max(1, minBackboneBranchLength),
                    Mathf.Max(Mathf.Max(1, minBackboneBranchLength), maxBackboneBranchLength) + 1);
                for (int step = 0; step <= length; step++)
                {
                    int x = startX + direction.x * step;
                    int z = startZ + direction.y * step;
                    if (x <= 0 || z <= 0 || x >= width - 1 || z >= depth - 1)
                    {
                        break;
                    }

                    path[x, z] = true;
                }
            }

            for (int x = 1; x < width - 1; x++)
            {
                for (int z = 1; z < depth - 1; z++)
                {
                    if (!path[x, z])
                    {
                        continue;
                    }

                    int openMask = 0;
                    for (int direction = 0; direction < CardinalDirections.Length; direction++)
                    {
                        Vector2Int offset = CardinalDirections[direction];
                        int nx = x + offset.x;
                        int nz = z + offset.y;
                        if (nx >= 0 && nz >= 0 && nx < width && nz < depth && path[nx, nz])
                        {
                            openMask |= 1 << direction;
                        }
                    }

                    int corridorMask = CorridorDomainMaskForOpenMask(openMask);
                    if (corridorMask != 0)
                    {
                        domains[x, z, floor] = corridorMask;
                    }
                }
            }

            return path;
        }

        private static bool IsReservedBackboneNeighborhood(bool[,] reservedBackboneCells, int x, int z)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    int nx = x + dx;
                    int nz = z + dz;
                    if (nx >= 0 && nz >= 0 && nx < reservedBackboneCells.GetLength(0) && nz < reservedBackboneCells.GetLength(1) && reservedBackboneCells[nx, nz])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private int CorridorDomainMaskForOpenMask(int openMask)
        {
            int mask = 0;
            for (int i = 0; i < modules.Count; i++)
            {
                WfcModule module = modules[i];
                if (module.cellKind == DungeonCellKind.Corridor && !module.opensUp && !module.opensDown && module.openMask == openMask)
                {
                    mask |= 1 << i;
                }
            }

            if (mask != 0)
            {
                return mask;
            }

            return moduleIndexById.ContainsKey("corridor_cross") ? MaskFor("corridor_cross") : 0;
        }

        private bool CollapseDomains(int[,,] domains, System.Random collapseRng)
        {
            int width = domains.GetLength(0);
            int depth = domains.GetLength(1);
            int floors = domains.GetLength(2);

            Queue<WfcCoord> propagationQueue = new Queue<WfcCoord>();
            for (int floor = 0; floor < floors; floor++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        propagationQueue.Enqueue(new WfcCoord(x, z, floor));
                    }
                }
            }

            if (!Propagate(domains, propagationQueue))
            {
                return false;
            }

            while (true)
            {
                WfcCoord cell;
                if (!FindLowestEntropyCell(domains, collapseRng, out cell))
                {
                    return true;
                }

                int selectedModule = ChooseWeightedModule(domains[cell.x, cell.z, cell.floorIndex], collapseRng);
                domains[cell.x, cell.z, cell.floorIndex] = 1 << selectedModule;
                propagationQueue.Enqueue(cell);

                if (!Propagate(domains, propagationQueue))
                {
                    return false;
                }
            }
        }

        private bool Propagate(int[,,] domains, Queue<WfcCoord> queue)
        {
            int width = domains.GetLength(0);
            int depth = domains.GetLength(1);
            int floors = domains.GetLength(2);

            while (queue.Count > 0)
            {
                WfcCoord current = queue.Dequeue();
                int currentMask = domains[current.x, current.z, current.floorIndex];

                for (int direction = 0; direction < CardinalDirections.Length; direction++)
                {
                    Vector2Int offset = CardinalDirections[direction];
                    int nx = current.x + offset.x;
                    int nz = current.z + offset.y;
                    int nf = current.floorIndex;
                    if (nx < 0 || nz < 0 || nx >= width || nz >= depth)
                    {
                        continue;
                    }

                    int oldMask = domains[nx, nz, nf];
                    int restrictedMask = RestrictNeighborMask(currentMask, oldMask, direction, false);
                    if (restrictedMask == 0)
                    {
                        return false;
                    }

                    if (restrictedMask != oldMask)
                    {
                        domains[nx, nz, nf] = restrictedMask;
                        queue.Enqueue(new WfcCoord(nx, nz, nf));
                    }
                }

                if (current.floorIndex + 1 < floors)
                {
                    int oldMask = domains[current.x, current.z, current.floorIndex + 1];
                    int restrictedMask = RestrictNeighborMask(currentMask, oldMask, 1, true);
                    if (restrictedMask == 0)
                    {
                        return false;
                    }

                    if (restrictedMask != oldMask)
                    {
                        domains[current.x, current.z, current.floorIndex + 1] = restrictedMask;
                        queue.Enqueue(new WfcCoord(current.x, current.z, current.floorIndex + 1));
                    }
                }

                if (current.floorIndex - 1 >= 0)
                {
                    int oldMask = domains[current.x, current.z, current.floorIndex - 1];
                    int restrictedMask = RestrictNeighborMask(currentMask, oldMask, -1, true);
                    if (restrictedMask == 0)
                    {
                        return false;
                    }

                    if (restrictedMask != oldMask)
                    {
                        domains[current.x, current.z, current.floorIndex - 1] = restrictedMask;
                        queue.Enqueue(new WfcCoord(current.x, current.z, current.floorIndex - 1));
                    }
                }
            }

            return true;
        }

        private int RestrictNeighborMask(int currentMask, int neighborMask, int direction, bool vertical)
        {
            int result = 0;
            for (int neighborIndex = 0; neighborIndex < modules.Count; neighborIndex++)
            {
                if ((neighborMask & (1 << neighborIndex)) == 0)
                {
                    continue;
                }

                for (int currentIndex = 0; currentIndex < modules.Count; currentIndex++)
                {
                    if ((currentMask & (1 << currentIndex)) == 0)
                    {
                        continue;
                    }

                    if (AreCompatible(modules[currentIndex], modules[neighborIndex], direction, vertical))
                    {
                        result |= 1 << neighborIndex;
                        break;
                    }
                }
            }

            return result;
        }

        private static bool AreCompatible(WfcModule current, WfcModule neighbor, int direction, bool vertical)
        {
            if (vertical)
            {
                if (direction > 0)
                {
                    return current.opensUp == neighbor.opensDown;
                }

                return current.opensDown == neighbor.opensUp;
            }

            int opposite = Opposite(direction);
            return current.IsOpen(direction) == neighbor.IsOpen(opposite);
        }

        private bool FindLowestEntropyCell(int[,,] domains, System.Random collapseRng, out WfcCoord cell)
        {
            int width = domains.GetLength(0);
            int depth = domains.GetLength(1);
            int floors = domains.GetLength(2);
            int bestCount = int.MaxValue;
            float bestNoise = float.MaxValue;
            cell = new WfcCoord();

            for (int floor = 0; floor < floors; floor++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        int count = CountBits(domains[x, z, floor]);
                        if (count <= 1)
                        {
                            continue;
                        }

                        float noise = (float)collapseRng.NextDouble() * 0.001f;
                        if (count < bestCount || (count == bestCount && noise < bestNoise))
                        {
                            bestCount = count;
                            bestNoise = noise;
                            cell = new WfcCoord(x, z, floor);
                        }
                    }
                }
            }

            return bestCount != int.MaxValue;
        }

        private int ChooseWeightedModule(int mask, System.Random collapseRng)
        {
            float total = 0f;
            for (int i = 0; i < modules.Count; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    total += modules[i].weight;
                }
            }

            float pick = (float)collapseRng.NextDouble() * total;
            float cumulative = 0f;
            for (int i = 0; i < modules.Count; i++)
            {
                if ((mask & (1 << i)) == 0)
                {
                    continue;
                }

                cumulative += modules[i].weight;
                if (pick <= cumulative)
                {
                    return i;
                }
            }

            return FirstModuleIndex(mask);
        }

        private int[,,] ResolveCollapsedModules(int[,,] domains)
        {
            int width = domains.GetLength(0);
            int depth = domains.GetLength(1);
            int floors = domains.GetLength(2);
            int[,,] collapsed = new int[width, depth, floors];
            for (int floor = 0; floor < floors; floor++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        collapsed[x, z, floor] = FirstModuleIndex(domains[x, z, floor]);
                    }
                }
            }

            return collapsed;
        }

        private int[,,] CreateEmptyCollapsed(int width, int depth, int floors)
        {
            int emptyIndex = moduleIndexById.ContainsKey("empty") ? moduleIndexById["empty"] : 0;
            int[,,] collapsed = new int[width, depth, floors];
            for (int floor = 0; floor < floors; floor++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        collapsed[x, z, floor] = emptyIndex;
                    }
                }
            }

            return collapsed;
        }

        private void BuildLayoutFromCollapsedModules(DungeonLayout layout, int[,,] collapsed)
        {
            int width = collapsed.GetLength(0);
            int depth = collapsed.GetLength(1);
            int floors = collapsed.GetLength(2);
            int safeRoomRadius = Mathf.Clamp(roomBrushRadius, 0, 4);
            int safeCorridorHalfWidth = Mathf.Clamp(corridorHalfWidth, 0, 4);

            for (int floor = 0; floor < floors; floor++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        WfcModule module = modules[collapsed[x, z, floor]];
                        if (module.cellKind == DungeonCellKind.Empty)
                        {
                            continue;
                        }

                        Vector2Int layoutCenter = CollapsedToLayoutCell(x, z, layout);
                        uniqueModules.Add(module.id);

                        if (module.cellKind == DungeonCellKind.Room)
                        {
                            PaintRoomMacroTile(layout, layoutCenter.x, layoutCenter.y, floor, safeRoomRadius);
                        }
                        else
                        {
                            PaintCorridorMacroTile(layout, layoutCenter.x, layoutCenter.y, floor, module, safeCorridorHalfWidth, macroTileStride);
                        }

                        if (module.opensUp && floor + 1 < floors)
                        {
                            layout.MarkFloorOpening(layoutCenter.x, layoutCenter.y, floor + 1);
                            layout.AddMarker(DungeonMapMarkerKind.StairsUp, layoutCenter, floor, "Escada");
                            layout.AddMarker(DungeonMapMarkerKind.VerticalExit, layoutCenter, floor + 1, "Abertura");
                        }
                    }
                }
            }
        }

        private void PaintRoomMacroTile(DungeonLayout layout, int centerX, int centerZ, int floor, int radius)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dz = -radius; dz <= radius; dz++)
                {
                    layout.MarkCell(centerX + dx, centerZ + dz, floor, DungeonCellKind.Room);
                }
            }

            OpenAdjacentCellsInRect(layout, centerX - radius, centerX + radius, centerZ - radius, centerZ + radius, floor);
        }

        private void PaintCorridorMacroTile(DungeonLayout layout, int centerX, int centerZ, int floor, WfcModule module, int halfWidth, int stepLength)
        {
            layout.MarkCell(centerX, centerZ, floor, DungeonCellKind.Corridor);
            int minX = centerX;
            int maxX = centerX;
            int minZ = centerZ;
            int maxZ = centerZ;

            for (int direction = 0; direction < CardinalDirections.Length; direction++)
            {
                if (!module.IsOpen(direction))
                {
                    continue;
                }

                Vector2Int forward = CardinalDirections[direction];
                Vector2Int side = new Vector2Int(-forward.y, forward.x);
                for (int step = 0; step <= Mathf.Max(1, stepLength); step++)
                {
                    for (int offset = -halfWidth; offset <= halfWidth; offset++)
                    {
                        int x = centerX + forward.x * step + side.x * offset;
                        int z = centerZ + forward.y * step + side.y * offset;
                        layout.MarkCell(x, z, floor, DungeonCellKind.Corridor);
                        minX = Mathf.Min(minX, x);
                        maxX = Mathf.Max(maxX, x);
                        minZ = Mathf.Min(minZ, z);
                        maxZ = Mathf.Max(maxZ, z);
                    }
                }
            }

            OpenAdjacentCellsInRect(layout, minX, maxX, minZ, maxZ, floor);
        }

        private Vector2Int CollapsedToLayoutCell(int collapsedX, int collapsedZ, DungeonLayout layout)
        {
            int x = Mathf.Clamp(collapsedX * macroTileStride + macroTileStride / 2, 0, layout.width - 1);
            int z = Mathf.Clamp(collapsedZ * macroTileStride + macroTileStride / 2, 0, layout.depth - 1);
            return new Vector2Int(x, z);
        }

        private void OpenAdjacentCellsInRect(DungeonLayout layout, int minX, int maxX, int minZ, int maxZ, int floor)
        {
            if (!openAdjacentPaintedCells)
            {
                return;
            }

            int clampedMinX = Mathf.Clamp(minX, 0, layout.width - 1);
            int clampedMaxX = Mathf.Clamp(maxX, 0, layout.width - 1);
            int clampedMinZ = Mathf.Clamp(minZ, 0, layout.depth - 1);
            int clampedMaxZ = Mathf.Clamp(maxZ, 0, layout.depth - 1);

            for (int x = clampedMinX; x <= clampedMaxX; x++)
            {
                for (int z = clampedMinZ; z <= clampedMaxZ; z++)
                {
                    if (!layout.IsOccupied(x, z, floor))
                    {
                        continue;
                    }

                    Vector2Int cell = new Vector2Int(x, z);
                    for (int i = 0; i < CardinalDirections.Length; i++)
                    {
                        Vector2Int neighbor = cell + CardinalDirections[i];
                        if (neighbor.x < clampedMinX || neighbor.x > clampedMaxX || neighbor.y < clampedMinZ || neighbor.y > clampedMaxZ)
                        {
                            continue;
                        }

                        if (layout.IsOccupied(neighbor, floor))
                        {
                            openWallEdges.Add(WallEdgeKey(cell, CardinalDirections[i], floor));
                        }
                    }
                }
            }
        }

        private void ExtractRoomsAndConnections(DungeonLayout layout, int[,,] collapsed)
        {
            int width = layout.width;
            int depth = layout.depth;
            int floors = layout.floorCount;
            int[,,] roomIds = new int[width, depth, floors];
            bool[,,] roomVisited = new bool[width, depth, floors];

            for (int floor = 0; floor < floors; floor++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        roomIds[x, z, floor] = -1;
                    }
                }
            }

            for (int floor = 0; floor < floors; floor++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        if (roomVisited[x, z, floor] || layout.cellsByFloor[x, z, floor] != DungeonCellKind.Room)
                        {
                            continue;
                        }

                        List<Vector2Int> cells = CollectRoomComponent(layout, roomVisited, x, z, floor);
                        if (cells.Count < Mathf.Max(1, minimumRoomComponentArea))
                        {
                            continue;
                        }

                        DungeonRoom room = CreateRoomFromCells(layout.rooms.Count, cells, floor);
                        layout.rooms.Add(room);
                        for (int i = 0; i < cells.Count; i++)
                        {
                            roomIds[cells[i].x, cells[i].y, floor] = room.id;
                        }
                    }
                }
            }

            ExtractConnectionsFromCorridorComponents(layout, collapsed, roomIds);
        }

        private List<Vector2Int> CollectRoomComponent(DungeonLayout layout, bool[,,] visited, int startX, int startZ, int floor)
        {
            List<Vector2Int> cells = new List<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            visited[startX, startZ, floor] = true;
            queue.Enqueue(new Vector2Int(startX, startZ));

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                cells.Add(current);
                for (int direction = 0; direction < CardinalDirections.Length; direction++)
                {
                    Vector2Int next = current + CardinalDirections[direction];
                    if (!layout.InBounds(next, floor) || visited[next.x, next.y, floor])
                    {
                        continue;
                    }

                    if (layout.cellsByFloor[next.x, next.y, floor] != DungeonCellKind.Room)
                    {
                        continue;
                    }

                    if (!IsOpenBetween(current, next, floor))
                    {
                        continue;
                    }

                    visited[next.x, next.y, floor] = true;
                    queue.Enqueue(next);
                }
            }

            return cells;
        }

        private DungeonRoom CreateRoomFromCells(int roomId, List<Vector2Int> cells, int floor)
        {
            int minX = cells[0].x;
            int maxX = cells[0].x;
            int minZ = cells[0].y;
            int maxZ = cells[0].y;
            for (int i = 1; i < cells.Count; i++)
            {
                minX = Mathf.Min(minX, cells[i].x);
                maxX = Mathf.Max(maxX, cells[i].x);
                minZ = Mathf.Min(minZ, cells[i].y);
                maxZ = Mathf.Max(maxZ, cells[i].y);
            }

            DungeonRoom room = new DungeonRoom();
            room.id = roomId;
            room.floorIndex = floor;
            room.bounds = new RectInt(minX, minZ, maxX - minX + 1, maxZ - minZ + 1);
            room.moduleId = "wfc_room_component";
            return room;
        }

        private void ExtractConnectionsFromCorridorComponents(DungeonLayout layout, int[,,] collapsed, int[,,] roomIds)
        {
            int width = layout.width;
            int depth = layout.depth;
            int floors = layout.floorCount;
            bool[,,] visited = new bool[width, depth, floors];

            for (int floor = 0; floor < floors; floor++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        if (visited[x, z, floor] || layout.cellsByFloor[x, z, floor] != DungeonCellKind.Corridor)
                        {
                            continue;
                        }

                        CorridorComponent component = CollectCorridorComponent(layout, collapsed, visited, roomIds, x, z, floor);
                        CreateConnectionsForCorridorComponent(layout, component);
                    }
                }
            }
        }

        private CorridorComponent CollectCorridorComponent(DungeonLayout layout, int[,,] collapsed, bool[,,] visited, int[,,] roomIds, int startX, int startZ, int startFloor)
        {
            CorridorComponent component = new CorridorComponent();
            Queue<WfcCoord> queue = new Queue<WfcCoord>();
            visited[startX, startZ, startFloor] = true;
            queue.Enqueue(new WfcCoord(startX, startZ, startFloor));

            while (queue.Count > 0)
            {
                WfcCoord current = queue.Dequeue();
                component.cells.Add(current);
                WfcModule module = InCollapsedBounds(collapsed, current.x, current.z, current.floorIndex)
                    ? modules[collapsed[current.x, current.z, current.floorIndex]]
                    : null;

                for (int direction = 0; direction < CardinalDirections.Length; direction++)
                {
                    Vector2Int offset = CardinalDirections[direction];
                    int nx = current.x + offset.x;
                    int nz = current.z + offset.y;
                    int nf = current.floorIndex;
                    if (!layout.InBounds(nx, nz, nf) || !IsOpenBetween(new Vector2Int(current.x, current.z), new Vector2Int(nx, nz), nf))
                    {
                        continue;
                    }

                    if (layout.cellsByFloor[nx, nz, nf] == DungeonCellKind.Room)
                    {
                        int roomId = roomIds[nx, nz, nf];
                        if (roomId >= 0)
                        {
                            component.touchingRooms[roomId] = new WfcCoord(nx, nz, nf);
                        }
                    }
                    else if (layout.cellsByFloor[nx, nz, nf] == DungeonCellKind.Corridor && !visited[nx, nz, nf])
                    {
                        visited[nx, nz, nf] = true;
                        queue.Enqueue(new WfcCoord(nx, nz, nf));
                    }
                }

                if (module != null && module.opensUp && current.floorIndex + 1 < collapsed.GetLength(2))
                {
                    TryVisitVerticalCorridor(layout, collapsed, visited, queue, current.x, current.z, current.floorIndex + 1);
                }

                if (module != null && module.opensDown && current.floorIndex - 1 >= 0)
                {
                    TryVisitVerticalCorridor(layout, collapsed, visited, queue, current.x, current.z, current.floorIndex - 1);
                }
            }

            return component;
        }

        private void TryVisitVerticalCorridor(DungeonLayout layout, int[,,] collapsed, bool[,,] visited, Queue<WfcCoord> queue, int x, int z, int floor)
        {
            if (!InCollapsedBounds(collapsed, x, z, floor) || visited[x, z, floor])
            {
                return;
            }

            if (!layout.InBounds(x, z, floor) || layout.cellsByFloor[x, z, floor] != DungeonCellKind.Corridor)
            {
                return;
            }

            visited[x, z, floor] = true;
            queue.Enqueue(new WfcCoord(x, z, floor));
        }

        private void CreateConnectionsForCorridorComponent(DungeonLayout layout, CorridorComponent component)
        {
            List<int> roomIds = new List<int>(component.touchingRooms.Keys);
            roomIds.Sort();
            if (roomIds.Count < 2)
            {
                return;
            }

            int anchor = roomIds[0];
            for (int i = 1; i < roomIds.Count; i++)
            {
                int other = roomIds[i];
                if (layout.HasConnection(anchor, other))
                {
                    continue;
                }

                WfcCoord from = component.touchingRooms[anchor];
                WfcCoord to = component.touchingRooms[other];
                DungeonConnection connection = new DungeonConnection();
                connection.roomAId = anchor;
                connection.roomBId = other;
                connection.fromCell = new Vector2Int(from.x, from.z);
                connection.toCell = new Vector2Int(to.x, to.z);
                connection.gridDistance = Mathf.Abs(from.x - to.x) + Mathf.Abs(from.z - to.z) + Mathf.Abs(from.floorIndex - to.floorIndex) * Mathf.Max(1f, floorHeight / Mathf.Max(0.1f, tileSize));
                connection.isVertical = from.floorIndex != to.floorIndex;
                connection.isExtraLoop = i > 1;
                layout.connections.Add(connection);
            }
        }

        private bool CorridorComponentHasVerticalCell(CorridorComponent component)
        {
            for (int i = 0; i < component.cells.Count; i++)
            {
                WfcCoord cell = component.cells[i];
                WfcModule module = modules[collapsedModules[cell.x, cell.z, cell.floorIndex]];
                if (module.opensUp || module.opensDown)
                {
                    return true;
                }
            }

            return false;
        }

        private void AssignStartAndGoal(DungeonLayout layout)
        {
            if (layout.rooms.Count == 0)
            {
                layout.startRoomId = -1;
                layout.goalRoomId = -1;
                return;
            }

            DungeonRoom start = layout.rooms[0];
            for (int i = 1; i < layout.rooms.Count; i++)
            {
                if (layout.rooms[i].AreaCells > start.AreaCells)
                {
                    start = layout.rooms[i];
                }
            }

            DungeonRoom goal = start;
            float bestDistance = -1f;
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                DungeonRoom candidate = layout.rooms[i];
                float distance = Vector2Int.Distance(start.CenterCell, candidate.CenterCell) + Mathf.Abs(start.floorIndex - candidate.floorIndex) * layout.width;
                if (distance > bestDistance)
                {
                    bestDistance = distance;
                    goal = candidate;
                }
            }

            layout.startRoomId = start.id;
            layout.goalRoomId = goal.id;
            layout.AddMarker(DungeonMapMarkerKind.Start, start.CenterCell, start.floorIndex, "Inicio");
            layout.AddMarker(DungeonMapMarkerKind.Goal, goal.CenterCell, goal.floorIndex, "Saida");
        }

        private DungeonMetrics CreateMetrics(DungeonLayout layout, int selectedSeed)
        {
            bool hasVerticalConnectors = HasVerticalConnections(layout);
            bool hasMultiFloor = HasMultipleConnectedFloors(layout);
            DungeonMetrics metrics = DungeonMetricsCalculator.Calculate(
                layout,
                selectedSeed,
                "Wave Function Collapse",
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
            GameObject rootObject = new GameObject("Generated WFC Dungeon");
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
                uniqueModules.Add("wfc_floor_opening");
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
            floor.name = "Fallback WFC Floor";
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
            wall.name = "Fallback WFC Wall";
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
            for (int i = 0; i < layout.markers.Count; i++)
            {
                DungeonMapMarker marker = layout.markers[i];
                if (marker.kind != DungeonMapMarkerKind.StairsUp)
                {
                    continue;
                }

                GameObject prefab = assetLibrary != null && assetLibrary.stairsUpPrefab != null
                    ? assetLibrary.stairsUpPrefab
                    : assetLibrary != null ? assetLibrary.stairsDownPrefab : null;
                GameObject instance = SpawnOptional(prefab, "wfc_stairs_up", CellToWorld(marker.Cell2D, marker.floorIndex, 0f), Quaternion.identity);
                if (instance == null && usePrimitiveFallbacks)
                {
                    CreateFallbackVerticalConnector(marker.Cell2D, marker.floorIndex);
                }
            }
        }

        private void CreateFallbackVerticalConnector(Vector2Int cell, int floor)
        {
            Vector3 position = CellToWorld(cell, floor, 0.25f);
            GameObject column = GameObject.CreatePrimitive(PrimitiveType.Cube);
            column.name = "Fallback WFC Vertical Connector";
            column.transform.SetParent(dungeonRoot, false);
            column.transform.position = position + new Vector3(0f, floorHeight * 0.5f, 0f);
            column.transform.localScale = new Vector3(tileSize * 0.8f, floorHeight, tileSize * 0.8f);
            ApplyMaterial(column, fallbackWallMaterial);
            uniqueModules.Add("fallback_vertical_connector");
            generatedGameObjectCount++;
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

        private bool TryGetFreeRoomCell(DungeonRoom room, out Vector2Int cell)
        {
            for (int attempt = 0; attempt < 80; attempt++)
            {
                int x = rng.Next(room.bounds.xMin, room.bounds.xMax);
                int z = rng.Next(room.bounds.yMin, room.bounds.yMax);
                cell = new Vector2Int(x, z);
                if (LastLayout != null && LastLayout.InBounds(cell, room.floorIndex) && LastLayout.cellsByFloor[x, z, room.floorIndex] == DungeonCellKind.Room &&
                    !reservedSpawnCells.Contains(SpawnCellKey(cell, room.floorIndex)) && cell != room.CenterCell)
                {
                    return true;
                }
            }

            cell = room.CenterCell;
            return LastLayout != null && LastLayout.InBounds(cell, room.floorIndex) && LastLayout.cellsByFloor[cell.x, cell.y, room.floorIndex] == DungeonCellKind.Room;
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
            context.supportsVerticalConnectors = enableMultiFloorWfc && floorCount > 1 && verticalConnectorObservationsPerFloorPair > 0;
            context.supportsMultiFloor = enableMultiFloorWfc && floorCount > 1;
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
            return "Teste Wave Function Collapse executado com " + report.runCount
                + " seed(s). Topologias unicas: " + report.uniqueTopologyCount
                + "/" + report.runCount
                + ". Diversidade topologica: " + (report.topologyDiversityRatio * 100f).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)
                + "%. Reprodutibilidade por seed: " + (seedReproducible ? "aprovada" : "falhou") + ".";
        }

        private string ComputeCurrentTopologyHash()
        {
            if (LastLayout == null || collapsedModules == null)
            {
                return LastLayout == null ? "NO_LAYOUT" : DungeonTopologyHasher.Compute(LastLayout);
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("WFC|")
                .Append(collapsedModules.GetLength(0)).Append("x")
                .Append(collapsedModules.GetLength(1)).Append("x")
                .Append(collapsedModules.GetLength(2)).Append("|");

            for (int floor = 0; floor < collapsedModules.GetLength(2); floor++)
            {
                builder.Append("F").Append(floor).Append(":");
                for (int z = 0; z < collapsedModules.GetLength(1); z++)
                {
                    for (int x = 0; x < collapsedModules.GetLength(0); x++)
                    {
                        builder.Append(collapsedModules[x, z, floor]).Append(".");
                    }
                }
                builder.Append("|");
            }

            builder.Append(DungeonTopologyHasher.Compute(LastLayout));
            return Fnv1A(builder.ToString());
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

        private void ExportCurrent2DMaps(string exportLabel)
        {
            if (LastLayout == null)
            {
                Debug.LogWarning("No WFC layout available to export as a 2D map. Generate a dungeon first.");
                return;
            }

            string metricsFolder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            string mapFolder = Path.Combine(metricsFolder, string.IsNullOrEmpty(mapExportSubfolderName) ? "Maps" : mapExportSubfolderName);
            List<string> paths = DungeonMap2DExporter.ExportFloorMaps(
                LastLayout,
                mapFolder,
                metricsFilePrefix,
                "WFC",
                resolvedSeed,
                exportLabel,
                Mathf.Max(4, mapPixelsPerCell),
                mapIncludeGrid,
                mapIncludeLegend);

            Debug.Log("WFC 2D map exported:\n" + string.Join("\n", paths.ToArray()));
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
                        if (layout.InBounds(cell, room.floorIndex) && layout.cellsByFloor[x, z, room.floorIndex] == DungeonCellKind.Room &&
                            !reservedSpawnCells.Contains(SpawnCellKey(cell, room.floorIndex)) && cell != room.CenterCell)
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

        private static string SpawnCellKey(Vector2Int cell, int floorIndex)
        {
            return floorIndex + ":" + cell.x + "," + cell.y;
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
            return layout.floorCount > 1 && HasVerticalConnections(layout);
        }

        private bool IsOpenBetween(Vector2Int a, Vector2Int b, int floor)
        {
            Vector2Int direction = b - a;
            return openWallEdges.Contains(WallEdgeKey(a, direction, floor));
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

        private static float BytesToMegabytes(long bytes)
        {
            return bytes / (1024f * 1024f);
        }

        private int MaskFor(string moduleId)
        {
            return 1 << moduleIndexById[moduleId];
        }

        private static int OpenMask(bool n, bool e, bool s, bool w)
        {
            int mask = 0;
            if (n) mask |= 1 << North;
            if (e) mask |= 1 << East;
            if (s) mask |= 1 << South;
            if (w) mask |= 1 << West;
            return mask;
        }

        private static int Opposite(int direction)
        {
            if (direction == North) return South;
            if (direction == East) return West;
            if (direction == South) return North;
            return East;
        }

        private static int CountBits(int value)
        {
            int count = 0;
            while (value != 0)
            {
                value &= value - 1;
                count++;
            }

            return count;
        }

        private static int FirstModuleIndex(int mask)
        {
            for (int i = 0; i < 31; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    return i;
                }
            }

            return 0;
        }

        private int CountOccupiedCollapsed(int[,,] collapsed)
        {
            int count = 0;
            for (int floor = 0; floor < collapsed.GetLength(2); floor++)
            {
                for (int x = 0; x < collapsed.GetLength(0); x++)
                {
                    for (int z = 0; z < collapsed.GetLength(1); z++)
                    {
                        if (modules[collapsed[x, z, floor]].cellKind != DungeonCellKind.Empty)
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        private static bool InCollapsedBounds(int[,,] collapsed, int x, int z, int floor)
        {
            return x >= 0 && z >= 0 && floor >= 0 && x < collapsed.GetLength(0) && z < collapsed.GetLength(1) && floor < collapsed.GetLength(2);
        }

        private sealed class WfcModule
        {
            public string id;
            public DungeonCellKind cellKind;
            public int openMask;
            public bool opensUp;
            public bool opensDown;
            public bool acceptsUp;
            public bool acceptsDown;
            public float weight;

            public bool IsOpen(int direction)
            {
                return (openMask & (1 << direction)) != 0;
            }
        }

        private struct WfcCoord
        {
            public int x;
            public int z;
            public int floorIndex;

            public WfcCoord(int x, int z, int floorIndex)
            {
                this.x = x;
                this.z = z;
                this.floorIndex = floorIndex;
            }
        }

        private sealed class CorridorComponent
        {
            public readonly List<WfcCoord> cells = new List<WfcCoord>();
            public readonly Dictionary<int, WfcCoord> touchingRooms = new Dictionary<int, WfcCoord>();
        }
    }

    public static class WFCParameterNotes
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
            if (parameterName == "numRoomsTarget") return "Extracao de componentes de celulas de sala colapsadas pelo WFC.";
            if (parameterName == "connectivityRatio") return "Busca no grafo extraido dos componentes de sala e corredores do WFC, incluindo a espinha dorsal observada quando ativa.";
            if (parameterName == "verticalVariance") return "Desvio padrao das alturas dos componentes de sala extraidos.";
            if (parameterName == "fillPercentage") return "Celulas ocupadas por modulos WFC divididas pelo total do volume.";
            if (parameterName == "branchFactor") return "Media de conexoes por componente de sala no grafo extraido.";
            if (parameterName == "avgPathLength") return "Distancias no grafo extraido do resultado WFC.";
            if (parameterName == "uniqueModules") return "Contagem de modulos WFC e prefabs efetivamente usados.";
            if (parameterName == "navigableVolumeRatio") return "Estimativa logica de celulas ocupadas antes de NavMesh.";
            if (parameterName == "criticalPathLength") return "Maior distancia no grafo extraido a partir da sala inicial.";
            if (parameterName == "avgAlternativePathLength") return "Media de conexoes extras inferidas em componentes de corredor com mais de duas salas.";
            if (parameterName == "SupportsVerticalConnectors") return "Conta execucoes com pares de modulos WFC stairs_up/stairs_down conectando pavimentos.";
            if (parameterName == "SupportsMultiFloor") return "Conta execucoes com mais de um pavimento e conexao vertical extraida do proprio colapso.";
            if (parameterName == "layoutGenerationMilliseconds") return "Tempo para colapsar o volume WFC, propagar restricoes e extrair componentes.";
            if (parameterName == "connectionCount") return "Quantidade de conexoes inferidas a partir de componentes de corredor.";
            return "Parametro medido sobre a saida colapsada pelo WFC.";
        }

        private static string InterpretationFor(string parameterName, string existing)
        {
            if (parameterName == "connectivityRatio") return "Mostra se o WFC produziu uma dungeon conectada sem reparo global posterior.";
            if (parameterName == "SupportsBacktrackingLoops") return "Parametro atendido quando o resultado WFC contem ciclos inferidos no grafo extraido.";
            if (parameterName == "SupportsVerticalConnectors") return "Parametro atendido somente quando os modulos verticais do WFC aparecem conectados a salas/corredores.";
            if (parameterName == "SupportsMultiFloor") return "Parametro atendido quando ha pavimentos conectados por sockets verticais do proprio WFC.";
            if (parameterName == "Legibility") return "A legibilidade depende muito do conjunto de tiles e das restricoes locais usadas.";
            if (parameterName == "Debuggability") return "WFC e deterministico por seed, mas contradicoes e propagacao de entropia tornam a depuracao menos direta que BSP/Room Graph.";
            return existing.Replace("BSP", "WFC").Replace("bsp", "wfc").Replace("Room Graph", "WFC");
        }

        private static string NoteFor(string parameterName)
        {
            if (parameterName == "numRoomsTarget") return "WFC nao controla salas como entidade global; salas emergem de componentes de tiles.";
            if (parameterName == "connectivityRatio") return "WFC puro nao garante conectividade global por padrao; esta variante usa uma espinha dorsal observada como restricao inicial do proprio WFC, nao como reparo posterior.";
            if (parameterName == "verticalVariance") return "WFC pode representar verticalidade se o tileset tiver sockets verticais.";
            if (parameterName == "fillPercentage") return "Densidade e resultado de pesos e compatibilidades locais, nao de controle global direto.";
            if (parameterName == "branchFactor") return "Ramificacao emerge das conexoes locais; controlar grau medio diretamente exige tileset/pesos bem calibrados.";
            if (parameterName == "avgPathLength") return "Caminhos sao medidos apos extrair um grafo do colapso; nao sao objetivo nativo do WFC.";
            if (parameterName == "uniqueModules") return "WFC tende a ser forte em variedade local quando ha um tileset rico.";
            if (parameterName == "navigableVolumeRatio") return "Proxy logico; validacao final ainda depende de NavMesh e colisores.";
            if (parameterName == "criticalPathLength") return "Caminho critico nao e controlado diretamente pelo WFC puro.";
            if (parameterName == "avgAlternativePathLength") return "Loops podem emergir, mas nao sao garantidos sem regras locais que favorecam ciclos.";
            if (parameterName == "SupportsRandomEnemySpawns") return "Spawns usam celulas de sala extraidas; balanceamento e camada de gameplay.";
            if (parameterName == "SupportsLootDistribution") return "Loot pode ser colocado sobre componentes de sala, mas progressao por risco nao e nativa do WFC.";
            if (parameterName == "SupportsTraps") return "Armadilhas podem usar celulas/corredores resultantes; sem semantica global nativa.";
            if (parameterName == "SupportsBacktrackingLoops") return "WFC pode criar loops emergentes; se a metrica vier baixa, isso e uma limitacao da configuracao pura testada.";
            if (parameterName == "SupportsVerticalConnectors") return "Suportado quando tiles verticais e observacoes iniciais fazem parte do proprio tileset WFC, sem BSP/Room Graph.";
            if (parameterName == "SupportsMultiFloor") return "Multiandar e possivel em WFC 3D com sockets verticais, mas nao e garantido em WFC 2D simples.";
            if (parameterName == "SupportsBossArena") return "Arenas grandes nao sao naturais no WFC local; exigem tiles/padroes que favorecam areas amplas.";
            if (parameterName == "SeedReproducible") return "O colapso e reproduzivel se todas as escolhas usam a mesma seed.";
            if (parameterName == "RuntimeRegeneration") return "Custo depende do tamanho do volume, quantidade de modulos e numero de contradicoes/reinicios.";
            if (parameterName == "BudgetAwareSpawns") return "Orcamento e aplicado depois sobre celulas livres extraidas; nao e propriedade nativa do WFC.";
            if (parameterName == "Replayability") return "WFC costuma variar bem em padroes locais; diversidade topologica depende do tileset.";
            if (parameterName == "Debuggability") return "Mais dificil de depurar que BSP/Room Graph, pois erros aparecem como contradicoes de restricao.";
            if (parameterName == "Flow") return "Fluxo global nao e garantido por restricoes locais; precisa ser medido apos o colapso.";
            if (parameterName == "Legibility") return "Boa legibilidade exige tileset com padroes claros; ruido local pode reduzir leitura espacial.";
            if (parameterName == "StructuralVariety") return "Ponto forte potencial do WFC, principalmente na variedade local de tiles.";
            if (parameterName == "layoutGenerationMilliseconds") return "Inclui colapso, propagacao e extracao de componentes; comparar separado da instanciacao Unity.";
            if (parameterName == "geometryInstantiationMilliseconds") return "Custo visual depende dos prefabs/Unity, nao do WFC puro.";
            if (parameterName == "metricsCalculationMilliseconds") return "Custo da instrumentacao, nao do algoritmo.";
            if (parameterName == "totalGenerationMilliseconds") return "Inclui layout, instanciacao visual e metricas; use junto com o tempo logico.";
            if (parameterName == "generatedGameObjectCount") return "Reflete peso da montagem visual dos tiles colapsados.";
            if (parameterName == "occupiedCellCount") return "Proxy do volume ocupado pelo colapso.";
            if (parameterName == "connectionCount") return "Conexoes sao inferidas apos o colapso; nao sao primitivas nativas do WFC.";
            if (parameterName == "managedMemoryDeltaKB") return "Estimativa sujeita ao GC da Unity; use como indicio comparativo.";
            return "Parametro avaliado na variante WFC pura.";
        }
    }
}
