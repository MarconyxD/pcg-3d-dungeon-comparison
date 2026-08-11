using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Dissertation.PCG
{
    public sealed class DrunkardWalkDungeonGenerator : MonoBehaviour
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

        [Header("Pure Drunkard Walk rules")]
        [Tooltip("Percentual aproximado do grid que os caminhantes tentam escavar. Controla a densidade sem usar outro algoritmo.")]
        [Range(0.02f, 0.85f)] public float targetFillPercentage = 0.32f;
        [Tooltip("Limite total de passos aleatorios antes de encerrar a tentativa de escavacao.")]
        public int maxWalkerSteps = 16000;
        [Tooltip("Quantidade de caminhantes simultaneos. Todos partem de celulas ja escavadas para manter o metodo como Drunkard Walk puro conectado.")]
        public int walkerCount = 5;
        [Tooltip("Raio de escavacao em torno do caminhante. 0 escava uma linha de 1 celula; 1 cria corredores mais largos.")]
        public int walkBrushRadius = 1;
        [Tooltip("Chance de o caminhante escolher uma nova direcao a cada passo.")]
        [Range(0f, 1f)] public float turnChance = 0.55f;
        [Tooltip("Chance de um caminhante reiniciar em uma celula ja escavada, criando ramificacoes sem reparar o mapa por fora.")]
        [Range(0f, 1f)] public float branchRestartChance = 0.04f;
        [Tooltip("Chance de escavar uma pequena area circular durante o passeio, formando salas/manchas naturais.")]
        [Range(0f, 1f)] public float roomStampChance = 0.025f;
        [Tooltip("Raio minimo das pequenas areas escavadas pelo caminhante.")]
        public int minRoomStampRadius = 1;
        [Tooltip("Raio maximo das pequenas areas escavadas pelo caminhante.")]
        public int maxRoomStampRadius = 3;
        [Tooltip("Quando ativo, o caminhante tambem pode se mover na diagonal. Desligado preserva corredores ortogonais mais compativeis com tiles modulares.")]
        public bool allowDiagonalWalkSteps;
        [Tooltip("Espessura da borda externa sempre fechada. Mantem o mapa contido sem usar reparo ou outro algoritmo.")]
        public int solidBorderThickness = 1;
        [Tooltip("Quando ativo, diagonais entram na analise local usada para classificar area aberta ou corredor estreito.")]
        public bool includeDiagonalNeighbors = true;
        [Tooltip("Quantidade minima de celulas para um componente aberto ser considerado uma sala mensuravel.")]
        public int minimumRoomComponentArea = 12;
        [Tooltip("Quantidade maxima de vizinhos cardinais abertos para uma celula aberta ser classificada como corredor estreito na analise.")]
        public int corridorCardinalNeighborLimit = 2;
        [Tooltip("Quantidade maxima de vizinhos totais abertos para uma celula aberta ser classificada como corredor estreito na analise.")]
        public int corridorTotalNeighborLimit = 4;

        [Header("Pure 3D Drunkard Walk")]
        [Tooltip("Quando ativo, permite que o Drunkard Walk caminhe entre pavimentos. Continua sendo uma extensao pura do proprio passeio aleatorio.")]
        public bool enableMultiFloorDrunkardWalk = true;
        [Tooltip("Quantidade de pavimentos no volume escavado.")]
        public int floorCount = 2;
        [Tooltip("Chance de um passo tentar subir ou descer um pavimento, quando o modo multiandar estiver ativo.")]
        [Range(0f, 1f)] public float verticalStepChance = 0.015f;
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
        public string metricsFilePrefix = "drunkard_walk";
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
        [Tooltip("Quando ativo, tambem exporta um PNG de massa celular bruta, mostrando celulas abertas e fechadas do Drunkard Walk antes da leitura de salas/corredores.")]
        public bool exportWalkMaskMap = true;
        [Tooltip("Quando ativo, desenha linhas finas no mapa de massa celular.")]
        public bool walkMaskMapIncludeGrid = false;

        [Header("Automated Tests")]
        [Tooltip("Quantidade de execucoes usadas no teste comparativo por multiplas seeds.")]
        public int testRunCount = 30;
        [Tooltip("Primeira seed usada no teste. As execucoes seguintes usam seed + 1, seed + 2 e assim por diante.")]
        public int testFirstSeed = 2000;
        [Tooltip("Tempo maximo, em milissegundos, para considerar que a regeneracao em runtime foi atendida.")]
        public float runtimeRegenerationMaxMilliseconds = 250f;
        [Tooltip("Quando ativo, apos o teste, instancia na cena a ultima dungeon testada.")]
        public bool instantiateLastTestDungeon = true;
        [Tooltip("Quando ativo, mede tambem o custo de instanciar prefabs em cada seed do teste.")]
        public bool measureVisualInstantiationInTests = true;

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
        private bool[,,] currentWalkCells;
        private int[,,] roomIdByCell;

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateDungeon();
            }
        }

        [ContextMenu("Generate Drunkard Walk Dungeon")]
        public void GenerateDungeon()
        {
            int selectedSeed = randomizeSeed ? System.Environment.TickCount : seed;
            GenerateForSeed(selectedSeed, instantiateGeometry, clearBeforeGenerate, true);
        }

        [ContextMenu("Apply Balanced Walk Preset")]
        public void ApplyBalancedWalkPreset()
        {
            targetFillPercentage = 0.32f;
            maxWalkerSteps = 16000;
            walkerCount = 5;
            walkBrushRadius = 1;
            turnChance = 0.55f;
            branchRestartChance = 0.04f;
            roomStampChance = 0.025f;
            minRoomStampRadius = 1;
            maxRoomStampRadius = 3;
            allowDiagonalWalkSteps = false;
            solidBorderThickness = 1;
            includeDiagonalNeighbors = true;
            minimumRoomComponentArea = 18;
            corridorCardinalNeighborLimit = 2;
            corridorTotalNeighborLimit = 4;
        }

        [ContextMenu("Run Drunkard Walk Measurement Test")]
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
            report.algorithmName = "Drunkard Walk";
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
                runReport.parameters = DrunkardWalkParameterNotes.CreateRunResults(metrics, context);
                report.runs.Add(runReport);
            }

            float diversityRatio = safeRunCount == 0 ? 0f : (float)uniqueHashes.Count / safeRunCount;
            context = CreateReportContext(seedReproducible, safeRunCount, uniqueHashes.Count, diversityRatio);
            report.uniqueTopologyCount = uniqueHashes.Count;
            report.topologyDiversityRatio = diversityRatio;

            for (int i = 0; i < report.runs.Count; i++)
            {
                DungeonQualitativeScorer.ApplyScores(report.runs[i].metrics, diversityRatio, safeRunCount);
                report.runs[i].parameters = DrunkardWalkParameterNotes.CreateRunResults(report.runs[i].metrics, context);
            }

            report.aggregateParameters = DrunkardWalkParameterNotes.CreateAggregateResults(report.runs, context);
            report.summary = BuildBatchSummary(report, seedReproducible);
            LastBatchReport = report;

            string folder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            DungeonReportPaths paths = DungeonReportExporter.ExportBatchReport(report, folder, metricsFilePrefix);
            Debug.Log("Drunkard Walk parameter test exported. Markdown: " + paths.markdownPath + " JSON: " + paths.jsonPath);

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
                Transform existing = transform.Find("Generated Drunkard Walk Dungeon");
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
            int safeFloorCount = enableMultiFloorDrunkardWalk ? Mathf.Max(2, floorCount) : 1;

            LastLayout = new DungeonLayout(safeWidth, safeDepth, floorHeight, safeFloorCount);
            currentWalkCells = BuildDrunkardWalkVolume(safeWidth, safeDepth, safeFloorCount);
            BuildLayoutFromWalk(LastLayout, currentWalkCells);
            ExtractRoomsAndConnections(LastLayout, currentWalkCells);
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

            Debug.Log("Drunkard Walk dungeon generated. Extracted regions: " + LastLayout.rooms.Count + ", seed: " + selectedSeed);
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
            currentWalkCells = null;
            roomIdByCell = null;
        }

        private bool[,,] BuildDrunkardWalkVolume(int width, int depth, int safeFloorCount)
        {
            bool[,,] cells = new bool[width, depth, safeFloorCount];
            int border = Mathf.Max(0, solidBorderThickness);
            int interiorWidth = Mathf.Max(1, width - border * 2);
            int interiorDepth = Mathf.Max(1, depth - border * 2);
            int interiorCapacity = interiorWidth * interiorDepth * safeFloorCount;
            int targetOpenCells = Mathf.Clamp(
                Mathf.RoundToInt(interiorCapacity * Mathf.Clamp01(targetFillPercentage)),
                1,
                interiorCapacity);

            List<DungeonGridCell> carvedCells = new List<DungeonGridCell>(targetOpenCells);
            List<DungeonGridCell> walkers = new List<DungeonGridCell>();
            List<Vector2Int> directions = BuildWalkDirections();

            int centerX = Mathf.Clamp(width / 2, border, width - border - 1);
            int centerZ = Mathf.Clamp(depth / 2, border, depth - border - 1);
            int startFloor = enableMultiFloorDrunkardWalk && safeFloorCount > 1 ? safeFloorCount / 2 : 0;
            DungeonGridCell start = new DungeonGridCell(centerX, centerZ, startFloor);
            int openCount = CarveBrush(cells, carvedCells, start.x, start.z, start.floorIndex, Mathf.Max(0, walkBrushRadius), border);

            int safeWalkerCount = Mathf.Max(1, walkerCount);
            for (int i = 0; i < safeWalkerCount; i++)
            {
                walkers.Add(ChooseExistingCarvedCell(carvedCells, start));
            }

            Vector2Int[] currentDirections = new Vector2Int[safeWalkerCount];
            for (int i = 0; i < currentDirections.Length; i++)
            {
                currentDirections[i] = directions[rng.Next(directions.Count)];
            }

            int stepLimit = Mathf.Max(maxWalkerSteps, targetOpenCells);
            int steps = 0;
            while (openCount < targetOpenCells && steps < stepLimit)
            {
                int walkerIndex = rng.Next(walkers.Count);
                DungeonGridCell walker = walkers[walkerIndex];

                if (carvedCells.Count > 0 && rng.NextDouble() < branchRestartChance)
                {
                    walker = ChooseExistingCarvedCell(carvedCells, walker);
                }

                if (rng.NextDouble() < turnChance)
                {
                    currentDirections[walkerIndex] = directions[rng.Next(directions.Count)];
                }

                if (enableMultiFloorDrunkardWalk && safeFloorCount > 1 && rng.NextDouble() < verticalStepChance)
                {
                    int floorDelta = rng.Next(2) == 0 ? -1 : 1;
                    int nextFloor = Mathf.Clamp(walker.floorIndex + floorDelta, 0, safeFloorCount - 1);
                    walker = new DungeonGridCell(walker.x, walker.z, nextFloor);
                }
                else
                {
                    Vector2Int direction = currentDirections[walkerIndex];
                    int nextX = walker.x + direction.x;
                    int nextZ = walker.z + direction.y;

                    if (IsSolidBorder(nextX, nextZ, width, depth, border))
                    {
                        currentDirections[walkerIndex] = directions[rng.Next(directions.Count)];
                        nextX = Mathf.Clamp(nextX, border, width - border - 1);
                        nextZ = Mathf.Clamp(nextZ, border, depth - border - 1);
                    }

                    walker = new DungeonGridCell(nextX, nextZ, walker.floorIndex);
                }

                walkers[walkerIndex] = walker;
                openCount += CarveBrush(cells, carvedCells, walker.x, walker.z, walker.floorIndex, Mathf.Max(0, walkBrushRadius), border);

                if (rng.NextDouble() < roomStampChance)
                {
                    int minRadius = Mathf.Max(0, minRoomStampRadius);
                    int maxRadius = Mathf.Max(minRadius, maxRoomStampRadius);
                    int radius = rng.Next(minRadius, maxRadius + 1);
                    openCount += CarveBrush(cells, carvedCells, walker.x, walker.z, walker.floorIndex, radius, border);
                }

                steps++;
            }

            return cells;
        }

        private List<Vector2Int> BuildWalkDirections()
        {
            List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(0, 1),
                new Vector2Int(1, 0),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0)
            };

            if (allowDiagonalWalkSteps)
            {
                directions.Add(new Vector2Int(1, 1));
                directions.Add(new Vector2Int(1, -1));
                directions.Add(new Vector2Int(-1, 1));
                directions.Add(new Vector2Int(-1, -1));
            }

            return directions;
        }

        private DungeonGridCell ChooseExistingCarvedCell(List<DungeonGridCell> carvedCells, DungeonGridCell fallback)
        {
            if (carvedCells.Count == 0)
            {
                return fallback;
            }

            return carvedCells[rng.Next(carvedCells.Count)];
        }

        private int CarveBrush(bool[,,] cells, List<DungeonGridCell> carvedCells, int centerX, int centerZ, int floor, int radius, int border)
        {
            int opened = 0;
            int safeRadius = Mathf.Max(0, radius);
            int width = cells.GetLength(0);
            int depth = cells.GetLength(1);

            for (int dx = -safeRadius; dx <= safeRadius; dx++)
            {
                for (int dz = -safeRadius; dz <= safeRadius; dz++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dz) > safeRadius + 1)
                    {
                        continue;
                    }

                    int x = centerX + dx;
                    int z = centerZ + dz;
                    if (IsSolidBorder(x, z, width, depth, border) || floor < 0 || floor >= cells.GetLength(2))
                    {
                        continue;
                    }

                    if (cells[x, z, floor])
                    {
                        continue;
                    }

                    cells[x, z, floor] = true;
                    carvedCells.Add(new DungeonGridCell(x, z, floor));
                    opened++;
                }
            }

            return opened;
        }

        private void BuildLayoutFromWalk(DungeonLayout layout, bool[,,] cells)
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
                        uniqueModules.Add(kind == DungeonCellKind.Room ? "dw_open_area" : "dw_narrow_passage");
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

                        AddRoomFromComponent(layout, component, floor, "dw_room_component");
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

                        AddRoomFromComponent(layout, component, floor, "dw_open_component");
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
            if (!enableMultiFloorDrunkardWalk || layout.floorCount < 2 || roomIdByCell == null)
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
                        uniqueModules.Add("dw_vertical_overlap");
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
                uniqueModules.Add("dw_corridor_connection");
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
                "Drunkard Walk",
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
            GameObject rootObject = new GameObject("Generated Drunkard Walk Dungeon");
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
                uniqueModules.Add("dw_floor_opening");
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
            floor.name = "Fallback Drunkard Walk Floor";
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
            position += new Vector3(direction.x * tileSize * 0.5f, 0f, direction.y * tileSize * 0.5f);
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
            wall.name = "Fallback Drunkard Walk Wall";
            wall.transform.SetParent(dungeonRoot, false);
            wall.transform.position = position + new Vector3(0f, wallHeight * 0.5f, 0f);
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
                GameObject instance = SpawnOptional(prefab, "dw_stairs_up", CellToWorld(cell, lower.floorIndex, 0f), Quaternion.identity);

                if (instance == null && usePrimitiveFallbacks)
                {
                    GameObject stairs = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    stairs.name = "Fallback Drunkard Walk Stairs";
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
                Debug.LogWarning("No Drunkard Walk layout available to export as a 2D map. Generate a dungeon first.");
                return;
            }

            string metricsFolder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            string mapFolder = Path.Combine(metricsFolder, string.IsNullOrEmpty(mapExportSubfolderName) ? "Maps" : mapExportSubfolderName);
            List<string> paths = DungeonMap2DExporter.ExportFloorMaps(
                LastLayout,
                mapFolder,
                metricsFilePrefix,
                "DRUNKARD WALK",
                resolvedSeed,
                exportLabel,
                Mathf.Max(4, mapPixelsPerCell),
                mapIncludeGrid,
                mapIncludeLegend);

            if (exportWalkMaskMap)
            {
                paths.AddRange(ExportCurrentWalkMaskMaps(mapFolder, exportLabel));
            }

            Debug.Log("Drunkard Walk 2D map exported:\n" + string.Join("\n", paths.ToArray()));
        }

        private List<string> ExportCurrentWalkMaskMaps(string mapFolder, string exportLabel)
        {
            List<string> paths = new List<string>();
            if (currentWalkCells == null)
            {
                return paths;
            }

            if (!Directory.Exists(mapFolder))
            {
                Directory.CreateDirectory(mapFolder);
            }

            int pixelsPerCell = Mathf.Clamp(mapPixelsPerCell, 4, 48);
            string safePrefix = string.IsNullOrEmpty(metricsFilePrefix) ? "drunkard_walk" : CleanFilePart(metricsFilePrefix);
            string safeLabel = string.IsNullOrEmpty(exportLabel) ? "map" : CleanFilePart(exportLabel);
            string timestamp = System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", System.Globalization.CultureInfo.InvariantCulture);

            for (int floor = 0; floor < currentWalkCells.GetLength(2); floor++)
            {
                Texture2D texture = RenderWalkMaskMap(currentWalkCells, floor, pixelsPerCell, walkMaskMapIncludeGrid);
                string fileName = safePrefix
                    + "_walk_mask_"
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

        private Texture2D RenderWalkMaskMap(bool[,,] cells, int floor, int pixelsPerCell, bool includeGrid)
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
            context.supportsVerticalConnectors = enableMultiFloorDrunkardWalk && floorCount > 1;
            context.supportsMultiFloor = enableMultiFloorDrunkardWalk && floorCount > 1;
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
            return "Teste Drunkard Walk executado com " + report.runCount
                + " seed(s). Topologias unicas: " + report.uniqueTopologyCount
                + "/" + report.runCount
                + ". Diversidade topologica: " + (report.topologyDiversityRatio * 100f).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)
                + "%. Reprodutibilidade por seed: " + (seedReproducible ? "aprovada" : "falhou") + ".";
        }

        private string ComputeCurrentTopologyHash()
        {
            if (LastLayout == null || currentWalkCells == null)
            {
                return LastLayout == null ? "NO_LAYOUT" : DungeonTopologyHasher.Compute(LastLayout);
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("DW|")
                .Append(currentWalkCells.GetLength(0)).Append("x")
                .Append(currentWalkCells.GetLength(1)).Append("x")
                .Append(currentWalkCells.GetLength(2)).Append("|");

            for (int floor = 0; floor < currentWalkCells.GetLength(2); floor++)
            {
                builder.Append("F").Append(floor).Append(":");
                for (int z = 0; z < currentWalkCells.GetLength(1); z++)
                {
                    for (int x = 0; x < currentWalkCells.GetLength(0); x++)
                    {
                        builder.Append(currentWalkCells[x, z, floor] ? "1" : "0");
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
            if (enableMultiFloorDrunkardWalk)
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
            if (x < 0 || z < 0 || x >= width || z >= depth)
            {
                return true;
            }

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

    public static class DrunkardWalkParameterNotes
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
            if (parameterName == "numRoomsTarget") return "Extracao de componentes abertos e densos apos a escavacao dos caminhantes.";
            if (parameterName == "connectivityRatio") return "Busca no grafo extraido de componentes escavados; nenhuma conexao artificial e criada.";
            if (parameterName == "verticalVariance") return "Desvio padrao das alturas dos componentes extraidos do volume escavado.";
            if (parameterName == "fillPercentage") return "Celulas escavadas pelos caminhantes divididas pelo total do volume.";
            if (parameterName == "branchFactor") return "Media de conexoes entre componentes extraidos por passagens estreitas geradas pelo passeio.";
            if (parameterName == "avgPathLength") return "Distancias no grafo extraido do resultado do passeio aleatorio.";
            if (parameterName == "uniqueModules") return "Contagem de categorias logicas e prefabs usados na leitura/instanciacao do resultado.";
            if (parameterName == "navigableVolumeRatio") return "Estimativa logica de celulas abertas antes de NavMesh.";
            if (parameterName == "criticalPathLength") return "Maior distancia no grafo extraido a partir do componente inicial.";
            if (parameterName == "avgAlternativePathLength") return "Media de conexoes marcadas como ciclos emergentes entre componentes.";
            if (parameterName == "SupportsVerticalConnectors") return "Conta execucoes em que o passeio vertical escavou celulas alinhadas entre pavimentos.";
            if (parameterName == "SupportsMultiFloor") return "Conta execucoes com mais de um pavimento e pelo menos uma conexao vertical inferida do proprio passeio.";
            if (parameterName == "layoutGenerationMilliseconds") return "Tempo para executar caminhantes, escavar celulas e extrair componentes.";
            if (parameterName == "connectionCount") return "Quantidade de conexoes inferidas por gargalos/passagens ou passos verticais.";
            return "Parametro medido sobre a saida do Drunkard Walk puro.";
        }

        private static string InterpretationFor(string parameterName, string existing)
        {
            if (parameterName == "connectivityRatio") return "Mostra se as regioes escavadas permaneceram conectadas sem flood-fill reparador, BSP, grafo-guia ou abertura posterior de tuneis.";
            if (parameterName == "SupportsBacktrackingLoops") return "Parametro atendido somente quando as conexoes emergentes formam ciclos no grafo extraido.";
            if (parameterName == "SupportsVerticalConnectors") return "Parametro atendido apenas se o proprio caminhante puder subir/descer e produzir alinhamentos entre pavimentos.";
            if (parameterName == "SupportsMultiFloor") return "Parametro atendido quando existe conexao vertical inferida do passeio multiandar.";
            if (parameterName == "Debuggability") return "Drunkard Walk e reproduzivel e simples de depurar, pois o caminho do caminhante explica diretamente a area escavada.";
            if (parameterName == "Legibility") return "A legibilidade depende do equilibrio entre corredor escavado, areas abertas e excesso de sinuosidade.";
            return existing.Replace("BSP", "Drunkard Walk").Replace("bsp", "drunkard_walk").Replace("Room Graph", "Drunkard Walk").Replace("WFC", "Drunkard Walk");
        }

        private static string NoteFor(string parameterName)
        {
            if (parameterName == "numRoomsTarget") return "Drunkard Walk nao cria salas planejadas; regioes amplas aparecem quando o passeio se auto-intersecta ou recebe stamps locais.";
            if (parameterName == "connectivityRatio") return "Com caminhantes reiniciando apenas em celulas ja escavadas, a tendencia e conectividade alta; se falhar, e limitacao da extracao ou da variante multiandar.";
            if (parameterName == "verticalVariance") return "Verticalidade so aparece quando a opcao multiandar permite passos verticais do proprio caminhante.";
            if (parameterName == "fillPercentage") return "Densidade e controlada por meta de preenchimento, passos maximos, raio do pincel e stamps de sala.";
            if (parameterName == "branchFactor") return "Ramificacao emerge de reinicios em celulas ja escavadas e mudancas de direcao, nao de um grafo explicito.";
            if (parameterName == "avgPathLength") return "Caminhos sao medidos depois da extracao; o passeio nao escolhe objetivo final semanticamente.";
            if (parameterName == "uniqueModules") return "O algoritmo varia forma do caminho; variedade modular depende da biblioteca e da camada de instanciacao.";
            if (parameterName == "navigableVolumeRatio") return "Proxy logico; deve ser confirmado com NavMesh/colisao em uma validacao fisica.";
            if (parameterName == "criticalPathLength") return "Pode gerar caminho longo por caminhada sinuosa, mas nao controla pacing ou progressao de missao sozinho.";
            if (parameterName == "avgAlternativePathLength") return "Loops podem emergir por auto-interseccao do passeio, mas nao sao garantidos por planejamento global.";
            if (parameterName == "SupportsRandomEnemySpawns") return "Spawns usam regioes escavadas; balanceamento nao e propriedade nativa do Drunkard Walk.";
            if (parameterName == "SupportsLootDistribution") return "Loot pode ser colocado nas regioes escavadas, mas progressao por risco depende de outra camada.";
            if (parameterName == "SupportsTraps") return "Armadilhas podem usar celulas escavadas/gargalos, mas sem semantica global nativa.";
            if (parameterName == "SupportsBacktrackingLoops") return "Loops podem aparecer por auto-interseccao, mas nao ha garantia de ciclo jogavel em toda seed.";
            if (parameterName == "SupportsVerticalConnectors") return "Possivel apenas na extensao multiandar em que o proprio caminhante faz passos verticais.";
            if (parameterName == "SupportsMultiFloor") return "Drunkard Walk 2D puro nao e multiandar; o passeio 3D continua puro, mas deve ser relatado como extensao volumetrica.";
            if (parameterName == "SupportsBossArena") return "Areas grandes podem surgir por stamps ou sobreposicao de passos, mas nao sao arenas semanticamente planejadas.";
            if (parameterName == "SeedReproducible") return "Resultado reproduzivel quando o passeio usa a mesma seed.";
            if (parameterName == "RuntimeRegeneration") return "Custo cresce com tamanho do grid, quantidade de passos, caminhantes e raio de escavacao.";
            if (parameterName == "BudgetAwareSpawns") return "Orcamento e aplicado apos a geracao, sobre celulas livres extraidas.";
            if (parameterName == "Replayability") return "Alta variacao de percurso entre seeds e esperada.";
            if (parameterName == "Debuggability") return "O caminho do caminhante e facil de repetir e inspecionar, mas metricas ajudam a quantificar sinuosidade e loops.";
            if (parameterName == "Flow") return "Fluxo tende a ser continuo, mas pode ficar sinuoso demais ou pouco ramificado.";
            if (parameterName == "Legibility") return "Corredores escavados podem ser claros, mas excesso de curvas reduz orientacao.";
            if (parameterName == "StructuralVariety") return "Boa variedade de trajetos; menor controle sobre categorias semanticas de sala.";
            if (parameterName == "layoutGenerationMilliseconds") return "Inclui passeio aleatorio, escavacao e extracao de componentes.";
            if (parameterName == "geometryInstantiationMilliseconds") return "Custo visual depende dos prefabs/Unity, nao do Drunkard Walk puro.";
            if (parameterName == "metricsCalculationMilliseconds") return "Custo da instrumentacao, nao do algoritmo.";
            if (parameterName == "totalGenerationMilliseconds") return "Inclui layout, instanciacao visual e metricas; compare junto do tempo logico.";
            if (parameterName == "generatedGameObjectCount") return "Reflete peso da montagem visual das celulas abertas.";
            if (parameterName == "occupiedCellCount") return "Proxy do volume escavado pelo Drunkard Walk.";
            if (parameterName == "connectionCount") return "Conexoes sao inferidas apos a geracao; o algoritmo nativo e um passeio, nao um grafo explicito.";
            if (parameterName == "managedMemoryDeltaKB") return "Estimativa sujeita ao GC da Unity; use como indicio comparativo.";
            return "Parametro avaliado na variante Drunkard Walk pura.";
        }
    }
}

