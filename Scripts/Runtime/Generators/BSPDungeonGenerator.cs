using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dissertation.PCG
{
    public sealed class BSPDungeonGenerator : MonoBehaviour
    {
        private sealed class BspNode
        {
            public RectInt area;
            public BspNode left;
            public BspNode right;
            public DungeonRoom room;

            public bool IsLeaf
            {
                get { return left == null && right == null; }
            }

            public BspNode(RectInt area)
            {
                this.area = area;
            }
        }

        [Header("Generation")]
        [Tooltip("Valor usado para gerar sempre a mesma dungeon. Mantendo a mesma seed, o resultado deve ser reproduzível.")]
        public int seed = 12345;
        [Tooltip("Quando ativo, ignora a seed fixa e cria uma seed nova a cada geração.")]
        public bool randomizeSeed;
        [Tooltip("Quando ativo, gera a dungeon automaticamente ao iniciar a cena.")]
        public bool generateOnStart = true;
        [Tooltip("Quando ativo, remove a dungeon anterior antes de gerar uma nova.")]
        public bool clearBeforeGenerate = true;
        [Tooltip("Largura do grid lógico da dungeon, em células.")]
        public int mapWidth = 64;
        [Tooltip("Profundidade do grid lógico da dungeon, em células.")]
        public int mapDepth = 64;
        [Tooltip("Quantidade máxima de divisões BSP. Valores maiores tendem a criar mais salas menores.")]
        public int maxSplitDepth = 5;
        [Tooltip("Menor tamanho permitido para uma sala, em células.")]
        public int minRoomSize = 5;
        [Tooltip("Maior tamanho permitido para uma sala, em células.")]
        public int maxRoomSize = 12;
        [Tooltip("Número máximo de salas que o gerador tentará criar por pavimento.")]
        public int maxRooms = 24;
        [Tooltip("Margem interna deixada entre a sala e a borda da partição BSP.")]
        public int roomPadding = 1;
        [Tooltip("Largura dos corredores, em células. Use 3 ou 5 para corredores mais confortáveis com assets modulares.")]
        public int corridorWidth = 3;
        [Tooltip("Folga adicional nas aberturas entre sala e corredor. Ajuda a impedir que paredes fechem a passagem.")]
        public int doorwayExtraClearance = 1;
        [Tooltip("Probabilidade de uma partição continuar sendo dividida. Valores maiores criam estruturas mais fragmentadas.")]
        [Range(0f, 1f)] public float splitChance = 0.92f;
        [Tooltip("Número de conexões extras entre salas. Aumentar este valor cria loops e caminhos alternativos.")]
        public int extraLoopConnections = 2;
        [Tooltip("Distância máxima, em células, para permitir uma conexão extra entre duas salas.")]
        public float maxExtraLoopDistance = 26f;

        [Header("Pure BSP verticality")]
        [Tooltip("Quando ativo, usa uma variante BSP pura com vários pavimentos e conectores verticais.")]
        public bool enableMultiFloorBsp = true;
        [Tooltip("Quantidade de pavimentos BSP gerados. Use 2 ou mais para medir multiandar e verticalidade.")]
        public int floorCount = 2;
        [Tooltip("Quantidade de conectores verticais BSP entre cada par de pavimentos adjacentes.")]
        public int verticalConnectionsPerFloorPair = 1;
        [Tooltip("Distância horizontal máxima desejada entre salas de pavimentos adjacentes para criar uma escada. Se nenhuma sala estiver dentro do raio, o par mais próximo é usado.")]
        public float verticalConnectorSearchRadius = 10f;
        [Tooltip("Raio, em células, da abertura criada no piso superior ao redor da chegada da escada.")]
        public int verticalOpeningRadius = 1;
        [Tooltip("Deslocamento da abertura do piso superior na direção frontal da escada. Use para alinhar melhor prefabs cuja chegada não fica no pivô.")]
        public int verticalOpeningForwardOffsetCells = 1;

        [Header("3D placement")]
        [Tooltip("Biblioteca com os prefabs do KayKit ou de outro pacote modular usado para montar a dungeon.")]
        public DungeonAssetLibrary assetLibrary;
        [Tooltip("Quando ativo, instancia os objetos 3D. Quando inativo, apenas gera o layout lógico e as métricas.")]
        public bool instantiateGeometry = true;
        [Tooltip("Centraliza a dungeon em torno da origem da cena, em vez de começar no canto do grid.")]
        public bool centerOnOrigin = true;
        [Tooltip("Tamanho de cada célula do grid em unidades Unity. Ajuste para casar com o tamanho dos tiles KayKit.")]
        public float tileSize = 2f;
        [Tooltip("Altura entre pavimentos em unidades Unity. Usado para medir verticalVariance e posicionar andares BSP.")]
        public float floorHeight = 4f;
        [Tooltip("Altura usada pelas paredes primitivas de fallback, quando não há prefab de parede configurado.")]
        public float wallHeight = 3f;
        [Tooltip("Espessura usada pelas paredes primitivas de fallback, quando não há prefab de parede configurado.")]
        public float wallThickness = 0.25f;
        [Tooltip("Correção de rotação das paredes em graus. Use se o prefab KayKit estiver virado para o lado errado.")]
        public float wallYawOffset;
        [Tooltip("Deslocamento vertical aplicado aos prefabs de parede.")]
        public float wallYOffset;
        [Tooltip("Escala aplicada aos prefabs instanciados pelo gerador.")]
        public Vector3 prefabInstanceScale = Vector3.one;

        [Header("Fallback primitives")]
        [Tooltip("Quando ativo, usa cubos simples caso os prefabs KayKit não estejam configurados.")]
        public bool usePrimitiveFallbacks = true;
        [Tooltip("Material aplicado aos pisos primitivos de fallback.")]
        public Material fallbackFloorMaterial;
        [Tooltip("Material aplicado às paredes primitivas de fallback.")]
        public Material fallbackWallMaterial;

        [Header("Semantic spawns")]
        [Tooltip("Probabilidade de cada sala receber objetos decorativos da lista de props.")]
        [Range(0f, 1f)] public float propRoomChance = 0.75f;
        [Tooltip("Quantidade mínima de objetos decorativos por sala selecionada.")]
        public int minPropsPerRoom = 0;
        [Tooltip("Quantidade máxima de objetos decorativos por sala selecionada.")]
        public int maxPropsPerRoom = 3;
        [Tooltip("Orçamento total de inimigos que o gerador tentará posicionar na dungeon.")]
        public int enemyBudget = 0;
        [Tooltip("Orçamento total de itens/recompensas que o gerador tentará posicionar na dungeon.")]
        public int lootBudget = 0;
        [Tooltip("Orçamento total de armadilhas que o gerador tentará posicionar na dungeon.")]
        public int trapBudget = 0;

        [Header("Metrics")]
        [Tooltip("Quando ativo, exporta automaticamente as métricas da dungeon em JSON e CSV a cada geração.")]
        public bool exportMetricsOnGenerate = true;
        [Tooltip("Nome da pasta, dentro de Application.persistentDataPath, onde os arquivos de métricas serão salvos.")]
        public string metricsFolderName = "PCGMetrics";
        [Tooltip("Prefixo usado no nome dos arquivos JSON e CSV exportados.")]
        public string metricsFilePrefix = "bsp";
        [Tooltip("Área mínima, em células, para considerar uma sala como possível arena de chefe.")]
        public int bossArenaMinAreaCells = 80;

        [Header("2D map export")]
        [Tooltip("Quando ativo, exporta automaticamente um mapa 2D em PNG ao clicar em Generate Dungeon.")]
        public bool export2DMapOnGenerate = true;
        [Tooltip("Quando ativo, exporta mapas 2D para cada seed durante Run Measurement Test. Em testes grandes, isso gera muitos arquivos.")]
        public bool export2DMapsDuringMeasurementTest;
        [Tooltip("Quando ativo, exporta o mapa 2D da última dungeon instanciada após Run Measurement Test.")]
        public bool export2DMapForLastTestDungeon = true;
        [Tooltip("Subpasta criada dentro da pasta de métricas para armazenar os mapas 2D.")]
        public string mapExportSubfolderName = "Maps";
        [Tooltip("Tamanho, em pixels, de cada célula do grid lógico no mapa exportado.")]
        public int mapPixelsPerCell = 10;
        [Tooltip("Quando ativo, desenha linhas finas entre células para facilitar a leitura do grid.")]
        public bool mapIncludeGrid = true;
        [Tooltip("Quando ativo, adiciona uma legenda lateral com as cores e símbolos usados no mapa.")]
        public bool mapIncludeLegend = true;

        [Header("Automated Tests")]
        [Tooltip("Quantidade de execuções usadas no teste comparativo por múltiplas seeds.")]
        public int testRunCount = 10;
        [Tooltip("Primeira seed usada no teste. As execuções seguintes usam seed + 1, seed + 2 e assim por diante.")]
        public int testFirstSeed = 1000;
        [Tooltip("Tempo máximo, em milissegundos, para considerar que a regeneração em runtime foi atendida.")]
        public float runtimeRegenerationMaxMilliseconds = 250f;
        [Tooltip("Quando ativo, após o teste, instancia na cena a última dungeon testada para inspeção visual.")]
        public bool instantiateLastTestDungeon = true;
        [Tooltip("Quando ativo, mede também o custo de instanciar prefabs em cada seed do teste. Deixe desligado para comparar apenas o custo lógico dos algoritmos.")]
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

        [ContextMenu("Generate Dungeon")]
        public void GenerateDungeon()
        {
            int selectedSeed = randomizeSeed ? System.Environment.TickCount : seed;
            GenerateForSeed(selectedSeed, instantiateGeometry, clearBeforeGenerate, true);
        }

        [ContextMenu("Run Measurement Test")]
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
            report.algorithmName = "Binary Space Partitioning";
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
                runReport.parameters = DungeonParameterEvaluator.CreateRunResults(metrics, context);
                report.runs.Add(runReport);
            }

            float diversityRatio = safeRunCount == 0 ? 0f : (float)uniqueHashes.Count / safeRunCount;
            context = CreateReportContext(seedReproducible, safeRunCount, uniqueHashes.Count, diversityRatio);
            report.uniqueTopologyCount = uniqueHashes.Count;
            report.topologyDiversityRatio = diversityRatio;

            for (int i = 0; i < report.runs.Count; i++)
            {
                DungeonQualitativeScorer.ApplyScores(report.runs[i].metrics, diversityRatio, safeRunCount);
                report.runs[i].parameters = DungeonParameterEvaluator.CreateRunResults(report.runs[i].metrics, context);
            }

            report.aggregateParameters = DungeonParameterEvaluator.CreateAggregateResults(report.runs, context);
            report.summary = BuildBatchSummary(report, seedReproducible);
            LastBatchReport = report;

            string folder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            DungeonReportPaths paths = DungeonReportExporter.ExportBatchReport(report, folder, metricsFilePrefix);
            Debug.Log("PCG parameter test exported. Markdown: " + paths.markdownPath + " JSON: " + paths.jsonPath);

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
                Transform existing = transform.Find("Generated BSP Dungeon");
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
            int effectiveFloorCount = enableMultiFloorBsp ? Mathf.Max(2, floorCount) : 1;
            LastLayout = new DungeonLayout(safeWidth, safeDepth, floorHeight, effectiveFloorCount);

            RectInt rootArea = new RectInt(1, 1, safeWidth - 2, safeDepth - 2);
            for (int floor = 0; floor < effectiveFloorCount; floor++)
            {
                BspNode root = BuildTree(rootArea, 0);
                CreateRooms(root, LastLayout, floor);
                ConnectTree(root, LastLayout);
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

            Debug.Log("BSP dungeon generated. Rooms: " + LastLayout.rooms.Count + ", seed: " + selectedSeed);
            return LastMetrics;
        }

        private void ExportCurrent2DMaps(string exportLabel)
        {
            if (LastLayout == null)
            {
                Debug.LogWarning("No dungeon layout available to export as a 2D map. Generate a dungeon first.");
                return;
            }

            string metricsFolder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            string mapFolder = Path.Combine(metricsFolder, string.IsNullOrEmpty(mapExportSubfolderName) ? "Maps" : mapExportSubfolderName);
            List<string> paths = DungeonMap2DExporter.ExportFloorMaps(
                LastLayout,
                mapFolder,
                metricsFilePrefix,
                "BSP",
                resolvedSeed,
                exportLabel,
                Mathf.Max(4, mapPixelsPerCell),
                mapIncludeGrid,
                mapIncludeLegend);

            Debug.Log("2D dungeon map exported:\n" + string.Join("\n", paths.ToArray()));
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

        private DungeonMetrics CreateMetrics(DungeonLayout layout, int selectedSeed)
        {
            bool hasVerticalConnectors = HasVerticalConnections(layout);
            bool hasMultiFloor = HasMultipleConnectedFloors(layout);
            DungeonMetrics metrics = DungeonMetricsCalculator.Calculate(
                layout,
                selectedSeed,
                "Binary Space Partitioning",
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
                count += Mathf.Max(0, room.AreaCells - 1);
            }

            return count;
        }

        private bool IsBudgetWithinCapacity(int spawnableCells)
        {
            int totalBudget = Mathf.Max(0, enemyBudget) + Mathf.Max(0, lootBudget) + Mathf.Max(0, trapBudget);
            return totalBudget <= spawnableCells;
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
            if (layout.floorCount < 2 || !HasVerticalConnections(layout))
            {
                return false;
            }

            HashSet<int> floors = new HashSet<int>();
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                floors.Add(layout.rooms[i].floorIndex);
            }

            return floors.Count >= 2;
        }

        private bool VerifySeedReproducibility(int selectedSeed)
        {
            GenerateForSeed(selectedSeed, false, false, false);
            string firstHash = DungeonTopologyHasher.Compute(LastLayout);
            GenerateForSeed(selectedSeed, false, false, false);
            string secondHash = DungeonTopologyHasher.Compute(LastLayout);
            return firstHash == secondHash;
        }

        private DungeonReportContext CreateReportContext(bool seedReproducible, int runCount, int uniqueTopologyCount, float topologyDiversityRatio)
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
            context.supportsVerticalConnectors = enableMultiFloorBsp && Mathf.Max(2, floorCount) > 1 && verticalConnectionsPerFloorPair > 0;
            context.supportsMultiFloor = enableMultiFloorBsp && Mathf.Max(2, floorCount) > 1;
            context.runCount = runCount;
            context.uniqueTopologyCount = uniqueTopologyCount;
            context.topologyDiversityRatio = topologyDiversityRatio;
            return context;
        }

        private string BuildBatchSummary(DungeonBatchReport report, bool seedReproducible)
        {
            return "Teste executado com " + report.runCount + " seed(s). " +
                "Topologias unicas: " + report.uniqueTopologyCount + "/" + report.runCount + ". " +
                "Diversidade topologica: " + (report.topologyDiversityRatio * 100f).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + "%. " +
                "Reprodutibilidade por seed: " + (seedReproducible ? "aprovada" : "falhou") + ".";
        }

        private BspNode BuildTree(RectInt area, int depth)
        {
            BspNode node = new BspNode(area);
            int minPartitionSize = Mathf.Max(3, minRoomSize + roomPadding * 2);
            bool canSplitVertical = area.width >= minPartitionSize * 2;
            bool canSplitHorizontal = area.height >= minPartitionSize * 2;

            if (depth >= maxSplitDepth || (!canSplitVertical && !canSplitHorizontal))
            {
                return node;
            }

            bool oversized = area.width > maxRoomSize + roomPadding * 4 || area.height > maxRoomSize + roomPadding * 4;
            if (!oversized && rng.NextDouble() > splitChance)
            {
                return node;
            }

            bool splitVertical = ChooseVerticalSplit(area, canSplitVertical, canSplitHorizontal);
            if (splitVertical)
            {
                int split = rng.Next(minPartitionSize, area.width - minPartitionSize + 1);
                node.left = BuildTree(new RectInt(area.xMin, area.yMin, split, area.height), depth + 1);
                node.right = BuildTree(new RectInt(area.xMin + split, area.yMin, area.width - split, area.height), depth + 1);
            }
            else
            {
                int split = rng.Next(minPartitionSize, area.height - minPartitionSize + 1);
                node.left = BuildTree(new RectInt(area.xMin, area.yMin, area.width, split), depth + 1);
                node.right = BuildTree(new RectInt(area.xMin, area.yMin + split, area.width, area.height - split), depth + 1);
            }

            return node;
        }

        private bool ChooseVerticalSplit(RectInt area, bool canSplitVertical, bool canSplitHorizontal)
        {
            if (canSplitVertical && !canSplitHorizontal) return true;
            if (!canSplitVertical && canSplitHorizontal) return false;

            float ratio = (float)area.width / Mathf.Max(1, area.height);
            if (ratio > 1.25f) return true;
            if (ratio < 0.8f) return false;
            return rng.NextDouble() < 0.5;
        }

        private void CreateRooms(BspNode node, DungeonLayout layout, int floorIndex)
        {
            if (node == null || CountRoomsOnFloor(layout, floorIndex) >= maxRooms)
            {
                return;
            }

            if (!node.IsLeaf)
            {
                CreateRooms(node.left, layout, floorIndex);
                CreateRooms(node.right, layout, floorIndex);
                return;
            }

            int maxWidth = Mathf.Min(maxRoomSize, node.area.width - roomPadding * 2);
            int maxDepth = Mathf.Min(maxRoomSize, node.area.height - roomPadding * 2);
            if (maxWidth < minRoomSize || maxDepth < minRoomSize)
            {
                return;
            }

            int roomWidth = rng.Next(minRoomSize, maxWidth + 1);
            int roomDepth = rng.Next(minRoomSize, maxDepth + 1);
            int minX = node.area.xMin + roomPadding;
            int minZ = node.area.yMin + roomPadding;
            int maxX = node.area.xMax - roomPadding - roomWidth;
            int maxZ = node.area.yMax - roomPadding - roomDepth;
            int roomX = rng.Next(minX, maxX + 1);
            int roomZ = rng.Next(minZ, maxZ + 1);

            DungeonRoom room = new DungeonRoom();
            room.id = layout.rooms.Count;
            room.bounds = new RectInt(roomX, roomZ, roomWidth, roomDepth);
            room.floorIndex = floorIndex;
            room.moduleId = "bsp_rect_room";
            node.room = room;
            layout.rooms.Add(room);
            uniqueModules.Add(room.moduleId);

            for (int x = room.bounds.xMin; x < room.bounds.xMax; x++)
            {
                for (int z = room.bounds.yMin; z < room.bounds.yMax; z++)
                {
                    layout.MarkCell(x, z, floorIndex, DungeonCellKind.Room);
                }
            }
        }

        private int CountRoomsOnFloor(DungeonLayout layout, int floorIndex)
        {
            int count = 0;
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                if (layout.rooms[i].floorIndex == floorIndex)
                {
                    count++;
                }
            }

            return count;
        }

        private DungeonRoom ConnectTree(BspNode node, DungeonLayout layout)
        {
            if (node == null)
            {
                return null;
            }

            if (node.IsLeaf)
            {
                return node.room;
            }

            DungeonRoom leftRoom = ConnectTree(node.left, layout);
            DungeonRoom rightRoom = ConnectTree(node.right, layout);

            if (leftRoom != null && rightRoom != null)
            {
                ConnectRooms(layout, leftRoom, rightRoom, false);
            }

            if (leftRoom != null && rightRoom != null)
            {
                return rng.NextDouble() < 0.5 ? leftRoom : rightRoom;
            }

            return leftRoom != null ? leftRoom : rightRoom;
        }

        private void AddExtraLoops(DungeonLayout layout)
        {
            if (extraLoopConnections <= 0 || layout.rooms.Count < 3)
            {
                return;
            }

            for (int floor = 0; floor < layout.floorCount; floor++)
            {
                List<DungeonRoom> floorRooms = GetRoomsOnFloor(layout, floor);
                if (floorRooms.Count < 3)
                {
                    continue;
                }

                int added = 0;
                int attempts = 0;
                while (added < extraLoopConnections && attempts < 500)
                {
                    attempts++;
                    DungeonRoom a = floorRooms[rng.Next(0, floorRooms.Count)];
                    DungeonRoom b = floorRooms[rng.Next(0, floorRooms.Count)];
                    if (a.id == b.id || layout.HasConnection(a.id, b.id))
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
        }

        private List<DungeonRoom> GetRoomsOnFloor(DungeonLayout layout, int floorIndex)
        {
            List<DungeonRoom> rooms = new List<DungeonRoom>();
            for (int i = 0; i < layout.rooms.Count; i++)
            {
                if (layout.rooms[i].floorIndex == floorIndex)
                {
                    rooms.Add(layout.rooms[i]);
                }
            }

            return rooms;
        }

        private void AddVerticalConnectors(DungeonLayout layout)
        {
            if (!enableMultiFloorBsp || layout.floorCount < 2 || verticalConnectionsPerFloorPair <= 0)
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

            List<DungeonRoom> lowerRooms = GetRoomsOnFloor(layout, lowerFloor);
            List<DungeonRoom> upperRooms = GetRoomsOnFloor(layout, upperFloor);
            float bestScore = float.PositiveInfinity;
            float fallbackScore = float.PositiveInfinity;
            DungeonRoom fallbackLower = null;
            DungeonRoom fallbackUpper = null;

            for (int a = 0; a < lowerRooms.Count; a++)
            {
                for (int b = 0; b < upperRooms.Count; b++)
                {
                    DungeonRoom candidateLower = lowerRooms[a];
                    DungeonRoom candidateUpper = upperRooms[b];
                    if (layout.HasConnection(candidateLower.id, candidateUpper.id))
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
            uniqueModules.Add("bsp_vertical_connector");
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
                Vector2Int shared = new Vector2Int((minX + maxX) / 2, (minZ + maxZ) / 2);
                lowerCell = shared;
                upperCell = shared;
                return;
            }

            lowerCell = ClampCellToRoom(lower.CenterCell, lower);
            upperCell = ClampCellToRoom(lower.CenterCell, upper);
        }

        private static Vector2Int ClampCellToRoom(Vector2Int cell, DungeonRoom room)
        {
            return new Vector2Int(
                Mathf.Clamp(cell.x, room.bounds.xMin, room.bounds.xMax - 1),
                Mathf.Clamp(cell.y, room.bounds.yMin, room.bounds.yMax - 1));
        }

        private static bool RectsOverlap(RectInt a, RectInt b)
        {
            return a.xMin < b.xMax && a.xMax > b.xMin && a.yMin < b.yMax && a.yMax > b.yMin;
        }

        private void ConnectRooms(DungeonLayout layout, DungeonRoom roomA, DungeonRoom roomB, bool isExtraLoop)
        {
            if (roomA.floorIndex != roomB.floorIndex)
            {
                ConnectVerticalRooms(layout, roomA, roomB);
                return;
            }

            Vector2Int a = roomA.CenterCell;
            Vector2Int b = roomB.CenterCell;
            int floorIndex = roomA.floorIndex;
            bool horizontalFirst = rng.NextDouble() < 0.5;
            Vector2Int directionA = horizontalFirst ? DirectionTowardX(a, b) : DirectionTowardZ(a, b);
            Vector2Int directionB = horizontalFirst ? DirectionTowardZ(b, a) : DirectionTowardX(b, a);

            if (directionA == Vector2Int.zero)
            {
                directionA = DirectionTowardZ(a, b);
            }

            if (directionB == Vector2Int.zero)
            {
                directionB = DirectionTowardX(b, a);
            }

            Doorway doorwayA = CarveDoorway(layout, roomA, directionA);
            Doorway doorwayB = CarveDoorway(layout, roomB, directionB);

            if (horizontalFirst)
            {
                CarveHorizontal(layout, doorwayA.outsideCell.x, doorwayB.outsideCell.x, doorwayA.outsideCell.y, floorIndex);
                CarveVertical(layout, doorwayA.outsideCell.y, doorwayB.outsideCell.y, doorwayB.outsideCell.x, floorIndex);
            }
            else
            {
                CarveVertical(layout, doorwayA.outsideCell.y, doorwayB.outsideCell.y, doorwayA.outsideCell.x, floorIndex);
                CarveHorizontal(layout, doorwayA.outsideCell.x, doorwayB.outsideCell.x, doorwayB.outsideCell.y, floorIndex);
            }

            DungeonConnection connection = new DungeonConnection();
            connection.roomAId = roomA.id;
            connection.roomBId = roomB.id;
            connection.fromCell = doorwayA.insideCell;
            connection.toCell = doorwayB.insideCell;
            connection.gridDistance = Mathf.Abs(doorwayA.outsideCell.x - doorwayB.outsideCell.x) + Mathf.Abs(doorwayA.outsideCell.y - doorwayB.outsideCell.y);
            connection.isVertical = false;
            connection.isExtraLoop = isExtraLoop;
            layout.connections.Add(connection);
            uniqueModules.Add(isExtraLoop ? "bsp_loop_connection" : "bsp_tree_connection");
        }

        private void CarveHorizontal(DungeonLayout layout, int xA, int xB, int z, int floorIndex)
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

        private void CarveVertical(DungeonLayout layout, int zA, int zB, int x, int floorIndex)
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

        private void InstantiateLayout(DungeonLayout layout)
        {
            GameObject rootObject = new GameObject("Generated BSP Dungeon");
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
                uniqueModules.Add("bsp_floor_opening");
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

                GameObject lowerInstance = SpawnOptional(lowerPrefab, "bsp_stairs_up", CellToWorld(lowerCell, lower.floorIndex, 0f), rotation);

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
}
