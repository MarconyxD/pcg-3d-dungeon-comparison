using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dissertation.PCG
{
    public sealed class GrammarBasedDungeonGenerator : MonoBehaviour
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
        [Header("Pure grammar rules")]
        [Tooltip("Menor tamanho permitido para uma sala gerada por uma regra da gramatica, em celulas.")]
        public int minRoomSize = 5;
        [Tooltip("Maior tamanho permitido para uma sala comum gerada por uma regra da gramatica, em celulas.")]
        public int maxRoomSize = 12;
        [Tooltip("Numero maximo de simbolos espaciais que a gramatica tentara materializar por pavimento.")]
        public int maxRooms = 24;
        [Tooltip("Espaco minimo entre retangulos de salas durante a interpretacao espacial da gramatica.")]
        public int roomPadding = 1;
        [Tooltip("Quantidade de tentativas de posicionamento para cada simbolo terminal da gramatica.")]
        public int roomPlacementAttempts = 350;
        [Tooltip("Comprimento minimo do caminho principal derivado pela gramatica, por pavimento.")]
        public int minMainPathLength = 7;
        [Tooltip("Comprimento maximo do caminho principal derivado pela gramatica, por pavimento.")]
        public int maxMainPathLength = 12;
        [Tooltip("Chance de uma regra do caminho principal gerar uma ramificacao lateral.")]
        [Range(0f, 1f)] public float branchRuleChance = 0.45f;
        [Tooltip("Quantidade maxima de ramificacoes derivadas por pavimento.")]
        public int maxGrammarBranches = 5;
        [Tooltip("Menor comprimento de uma ramificacao derivada pela gramatica.")]
        public int minBranchLength = 1;
        [Tooltip("Maior comprimento de uma ramificacao derivada pela gramatica.")]
        public int maxBranchLength = 3;
        [Tooltip("Chance de uma regra terminal de ramificacao virar uma sala de tesouro.")]
        [Range(0f, 1f)] public float treasureRuleChance = 0.35f;
        [Tooltip("Chance de uma regra terminal de ramificacao virar uma sala de armadilha.")]
        [Range(0f, 1f)] public float trapRuleChance = 0.25f;
        [Tooltip("Chance de a direcao da derivacao espacial mudar entre uma sala e outra.")]
        [Range(0f, 1f)] public float grammarTurnChance = 0.55f;
        [Tooltip("Chance de a gramatica adicionar uma regra de retorno/atalho entre simbolos ja derivados.")]
        [Range(0f, 1f)] public float loopRuleChance = 0.35f;
        [Tooltip("Quando ativo, a ultima regra do ultimo pavimento tenta gerar uma arena de chefe ampla.")]
        public bool forceBossArenaRule = true;
        [Tooltip("Tamanho minimo, em celulas, para a arena de chefe gerada pela regra final.")]
        public int bossRoomMinSize = 10;
        [Tooltip("Tamanho maximo, em celulas, para a arena de chefe gerada pela regra final.")]
        public int bossRoomMaxSize = 15;
        [Tooltip("Distancia minima entre centros de salas consecutivas na interpretacao da gramatica.")]
        public int grammarStepMin = 8;
        [Tooltip("Distancia maxima entre centros de salas consecutivas na interpretacao da gramatica.")]
        public int grammarStepMax = 14;
        [Tooltip("Largura dos corredores gerados entre simbolos consecutivos da derivacao, em celulas.")]
        public int corridorWidth = 3;
        [Tooltip("Folga adicional nas aberturas entre sala e corredor.")]
        public int doorwayExtraClearance = 1;
        [Tooltip("Numero maximo de regras de loop/atalho adicionadas pela gramatica.")]
        public int extraLoopConnections = 2;
        [Tooltip("Distancia maxima, em celulas, para uma regra de loop conectar duas salas ja derivadas.")]
        public float maxExtraLoopDistance = 26f;

        [Header("Pure grammar verticality")]
        [Tooltip("Quando ativo, permite que regras da propria gramatica criem conexoes verticais entre pavimentos.")]
        public bool enableMultiFloorGrammar = true;
        [Tooltip("Quantidade de pavimentos derivados pela gramatica.")]
        public int floorCount = 2;
        [Tooltip("Quantidade desejada de regras verticais entre cada par de pavimentos adjacentes.")]
        public int verticalRulesPerFloorPair = 1;
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
        public string metricsFilePrefix = "grammar_based";
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

        private enum GrammarNodeKind
        {
            Entrance,
            Room,
            Branch,
            Treasure,
            Trap,
            BossArena,
            Exit
        }

        private sealed class GrammarNode
        {
            public int id;
            public int parentId = -1;
            public int verticalParentId = -1;
            public int floorIndex;
            public int derivationDepth;
            public GrammarNodeKind kind;
            public Vector2Int preferredDirection;
            public bool isMainPath;
            public DungeonRoom room;
            public readonly List<int> loopTargets = new List<int>();
        }

        private void Start()
        {
            if (generateOnStart)
            {
                GenerateDungeon();
            }
        }

        [ContextMenu("Generate Grammar-Based Dungeon")]
        public void GenerateDungeon()
        {
            int selectedSeed = randomizeSeed ? System.Environment.TickCount : seed;
            GenerateForSeed(selectedSeed, instantiateGeometry, clearBeforeGenerate, true);
        }

        [ContextMenu("Run Grammar-Based Measurement Test")]
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
            report.algorithmName = "Grammar-Based Generation";
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
                runReport.parameters = GrammarBasedParameterNotes.CreateRunResults(metrics, context);
                report.runs.Add(runReport);
            }

            float diversityRatio = safeRunCount == 0 ? 0f : (float)uniqueHashes.Count / safeRunCount;
            context = CreateReportContext(seedReproducible, safeRunCount, uniqueHashes.Count, diversityRatio);
            report.uniqueTopologyCount = uniqueHashes.Count;
            report.topologyDiversityRatio = diversityRatio;

            for (int i = 0; i < report.runs.Count; i++)
            {
                DungeonQualitativeScorer.ApplyScores(report.runs[i].metrics, diversityRatio, safeRunCount);
                report.runs[i].parameters = GrammarBasedParameterNotes.CreateRunResults(report.runs[i].metrics, context);
            }

            report.aggregateParameters = GrammarBasedParameterNotes.CreateAggregateResults(report.runs, context);
            report.summary = BuildBatchSummary(report, seedReproducible);
            LastBatchReport = report;

            string folder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            DungeonReportPaths paths = DungeonReportExporter.ExportBatchReport(report, folder, metricsFilePrefix);
            Debug.Log("Grammar-Based Generation parameter test exported. Markdown: " + paths.markdownPath + " JSON: " + paths.jsonPath);

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
                Transform existing = transform.Find("Generated Grammar-Based Dungeon");
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
            int effectiveFloorCount = enableMultiFloorGrammar ? Mathf.Max(2, floorCount) : 1;
            LastLayout = new DungeonLayout(safeWidth, safeDepth, floorHeight, effectiveFloorCount);

            List<GrammarNode> grammarNodes = DeriveGrammar(effectiveFloorCount);
            PlaceGrammarRooms(LastLayout, grammarNodes);
            ConnectGrammarRooms(LastLayout, grammarNodes);
            ConnectGrammarVerticalRules(LastLayout, grammarNodes);
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

            Debug.Log("Grammar-Based Generation dungeon generated. Rooms: " + LastLayout.rooms.Count + ", seed: " + selectedSeed);
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

        private List<GrammarNode> DeriveGrammar(int effectiveFloorCount)
        {
            List<GrammarNode> nodes = new List<GrammarNode>();
            List<int> previousFloorMainPath = null;
            int safeMaxSymbolsPerFloor = Mathf.Max(2, maxRooms);

            for (int floor = 0; floor < effectiveFloorCount; floor++)
            {
                int mainLength = Mathf.Clamp(
                    rng.Next(Mathf.Max(2, minMainPathLength), Mathf.Max(minMainPathLength, maxMainPathLength) + 1),
                    2,
                    safeMaxSymbolsPerFloor);

                List<int> mainPath = new List<int>();
                int parentId = -1;
                Vector2Int direction = RandomCardinalDirection();

                for (int i = 0; i < mainLength; i++)
                {
                    if (i > 0 && rng.NextDouble() < grammarTurnChance)
                    {
                        direction = TurnDirection(direction);
                    }

                    GrammarNodeKind kind = SelectMainPathKind(floor, effectiveFloorCount, i, mainLength);
                    GrammarNode node = AddGrammarNode(nodes, kind, floor, parentId, i, direction, true);
                    mainPath.Add(node.id);
                    parentId = node.id;
                    uniqueModules.Add("grammar_rule_main_path");
                    uniqueModules.Add("grammar_symbol_" + kind.ToString().ToLowerInvariant());
                }

                if (enableMultiFloorGrammar && floor > 0 && previousFloorMainPath != null && previousFloorMainPath.Count > 0)
                {
                    int verticalCount = Mathf.Max(1, verticalRulesPerFloorPair);
                    for (int i = 0; i < verticalCount && i < mainPath.Count; i++)
                    {
                        int upperIndex = Mathf.Clamp(i * Mathf.Max(1, mainPath.Count / verticalCount), 0, mainPath.Count - 1);
                        int lowerIndex = Mathf.Clamp(i * Mathf.Max(1, previousFloorMainPath.Count / verticalCount), 0, previousFloorMainPath.Count - 1);
                        nodes[mainPath[upperIndex]].verticalParentId = previousFloorMainPath[lowerIndex];
                        uniqueModules.Add("grammar_rule_vertical");
                    }
                }

                int remainingSymbols = Mathf.Max(0, safeMaxSymbolsPerFloor - mainPath.Count);
                int branchBudget = Mathf.Min(Mathf.Max(0, maxGrammarBranches), remainingSymbols);
                for (int branch = 0; branch < branchBudget; branch++)
                {
                    if (mainPath.Count < 2 || rng.NextDouble() > branchRuleChance)
                    {
                        continue;
                    }

                    int anchorIndex = rng.Next(1, Mathf.Max(2, mainPath.Count - 1));
                    int branchParentId = mainPath[Mathf.Clamp(anchorIndex, 0, mainPath.Count - 1)];
                    Vector2Int branchDirection = TurnDirection(nodes[branchParentId].preferredDirection);
                    int branchLength = rng.Next(Mathf.Max(1, minBranchLength), Mathf.Max(minBranchLength, maxBranchLength) + 1);

                    for (int depth = 0; depth < branchLength && remainingSymbols > 0; depth++)
                    {
                        GrammarNodeKind branchKind = SelectBranchKind(depth, branchLength);
                        GrammarNode branchNode = AddGrammarNode(nodes, branchKind, floor, branchParentId, depth + 1, branchDirection, false);
                        branchParentId = branchNode.id;
                        remainingSymbols--;
                        uniqueModules.Add("grammar_rule_branch");
                        uniqueModules.Add("grammar_symbol_" + branchKind.ToString().ToLowerInvariant());

                        if (rng.NextDouble() < grammarTurnChance)
                        {
                            branchDirection = TurnDirection(branchDirection);
                        }
                    }
                }

                AddGrammarLoopRules(nodes, floor);
                previousFloorMainPath = mainPath;
            }

            return nodes;
        }

        private GrammarNode AddGrammarNode(
            List<GrammarNode> nodes,
            GrammarNodeKind kind,
            int floorIndex,
            int parentId,
            int depth,
            Vector2Int preferredDirection,
            bool isMainPath)
        {
            GrammarNode node = new GrammarNode();
            node.id = nodes.Count;
            node.kind = kind;
            node.floorIndex = floorIndex;
            node.parentId = parentId;
            node.derivationDepth = depth;
            node.preferredDirection = preferredDirection == Vector2Int.zero ? RandomCardinalDirection() : preferredDirection;
            node.isMainPath = isMainPath;
            nodes.Add(node);
            return node;
        }

        private GrammarNodeKind SelectMainPathKind(int floor, int effectiveFloorCount, int index, int length)
        {
            if (floor == 0 && index == 0)
            {
                return GrammarNodeKind.Entrance;
            }

            bool isLastNode = index == length - 1;
            bool isLastFloor = floor == effectiveFloorCount - 1;
            if (isLastNode && isLastFloor && forceBossArenaRule)
            {
                return GrammarNodeKind.BossArena;
            }

            if (isLastNode)
            {
                return GrammarNodeKind.Exit;
            }

            return GrammarNodeKind.Room;
        }

        private GrammarNodeKind SelectBranchKind(int depth, int branchLength)
        {
            bool isTerminal = depth == branchLength - 1;
            if (isTerminal && rng.NextDouble() < treasureRuleChance)
            {
                return GrammarNodeKind.Treasure;
            }

            if (isTerminal && rng.NextDouble() < trapRuleChance)
            {
                return GrammarNodeKind.Trap;
            }

            return GrammarNodeKind.Branch;
        }

        private void AddGrammarLoopRules(List<GrammarNode> nodes, int floorIndex)
        {
            int target = Mathf.Max(0, extraLoopConnections);
            int added = 0;
            int attempts = 0;
            while (added < target && attempts < Mathf.Max(20, target * 50))
            {
                attempts++;
                if (rng.NextDouble() > loopRuleChance)
                {
                    continue;
                }

                GrammarNode a = nodes[rng.Next(0, nodes.Count)];
                GrammarNode b = nodes[rng.Next(0, nodes.Count)];
                if (a.id == b.id || a.floorIndex != floorIndex || b.floorIndex != floorIndex)
                {
                    continue;
                }

                if (a.parentId == b.id || b.parentId == a.id || a.loopTargets.Contains(b.id) || b.loopTargets.Contains(a.id))
                {
                    continue;
                }

                a.loopTargets.Add(b.id);
                uniqueModules.Add("grammar_rule_loop");
                added++;
            }
        }

        private void PlaceGrammarRooms(DungeonLayout layout, List<GrammarNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                GrammarNode node = nodes[i];
                Vector2Int desiredCenter = GetDesiredCenterForNode(layout, nodes, node);
                DungeonRoom room;
                if (!TryPlaceGrammarRoom(layout, node, desiredCenter, out room))
                {
                    continue;
                }

                node.room = room;
                layout.rooms.Add(room);
                MarkRoomCells(layout, room);
            }
        }

        private Vector2Int GetDesiredCenterForNode(DungeonLayout layout, List<GrammarNode> nodes, GrammarNode node)
        {
            if (node.verticalParentId >= 0 && node.verticalParentId < nodes.Count && nodes[node.verticalParentId].room != null)
            {
                return nodes[node.verticalParentId].room.CenterCell;
            }

            if (node.parentId < 0 || node.parentId >= nodes.Count || nodes[node.parentId].room == null)
            {
                int x = layout.width / 2 + rng.Next(-4, 5);
                int z = layout.depth / 2 + rng.Next(-4, 5);
                return new Vector2Int(x, z);
            }

            Vector2Int parentCenter = nodes[node.parentId].room.CenterCell;
            int minStep = Mathf.Max(4, grammarStepMin);
            int maxStep = Mathf.Max(minStep, grammarStepMax);
            int step = rng.Next(minStep, maxStep + 1);
            Vector2Int direction = node.preferredDirection == Vector2Int.zero ? RandomCardinalDirection() : node.preferredDirection;
            Vector2Int perpendicular = new Vector2Int(-direction.y, direction.x);
            int jitter = rng.Next(-2, 3);
            return parentCenter + direction * step + perpendicular * jitter;
        }

        private bool TryPlaceGrammarRoom(DungeonLayout layout, GrammarNode node, Vector2Int desiredCenter, out DungeonRoom room)
        {
            int attempts = Mathf.Max(1, roomPlacementAttempts);
            int padding = Mathf.Max(0, roomPadding);
            int width;
            int depth;
            GetGrammarRoomSize(node.kind, out width, out depth);

            Vector2Int[] directions = ShuffledDirectionsWithPreferred(node.preferredDirection);
            for (int attempt = 0; attempt < attempts; attempt++)
            {
                Vector2Int center = desiredCenter;
                if (attempt > 0)
                {
                    Vector2Int direction = directions[attempt % directions.Length];
                    int radius = Mathf.Max(2, attempt / directions.Length + 1);
                    center += direction * rng.Next(1, Mathf.Max(2, radius + 1));
                    center += new Vector2Int(rng.Next(-radius, radius + 1), rng.Next(-radius, radius + 1));
                }

                RectInt bounds = BoundsFromCenter(center, width, depth);
                if (!IsRoomInsideMap(layout, bounds) || OverlapsExistingRoom(layout, bounds, node.floorIndex, padding))
                {
                    continue;
                }

                room = new DungeonRoom();
                room.id = layout.rooms.Count;
                room.floorIndex = node.floorIndex;
                room.bounds = bounds;
                room.moduleId = "grammar_" + node.kind.ToString().ToLowerInvariant();
                uniqueModules.Add(room.moduleId);
                return true;
            }

            room = null;
            return false;
        }

        private void GetGrammarRoomSize(GrammarNodeKind kind, out int width, out int depth)
        {
            int safeMin = Mathf.Max(2, minRoomSize);
            int safeMax = Mathf.Max(safeMin, maxRoomSize);
            if (kind == GrammarNodeKind.BossArena)
            {
                int bossMin = Mathf.Max(safeMin, bossRoomMinSize);
                int bossMax = Mathf.Max(bossMin, bossRoomMaxSize);
                width = rng.Next(bossMin, bossMax + 1);
                depth = rng.Next(bossMin, bossMax + 1);
                return;
            }

            if (kind == GrammarNodeKind.Entrance || kind == GrammarNodeKind.Exit)
            {
                width = rng.Next(safeMin, Mathf.Max(safeMin, safeMax - 1) + 1);
                depth = rng.Next(safeMin, Mathf.Max(safeMin, safeMax - 1) + 1);
                return;
            }

            if (kind == GrammarNodeKind.Treasure || kind == GrammarNodeKind.Trap)
            {
                int terminalMax = Mathf.Max(safeMin, Mathf.RoundToInt(safeMax * 0.85f));
                width = rng.Next(safeMin, terminalMax + 1);
                depth = rng.Next(safeMin, terminalMax + 1);
                return;
            }

            width = rng.Next(safeMin, safeMax + 1);
            depth = rng.Next(safeMin, safeMax + 1);
        }

        private void ConnectGrammarRooms(DungeonLayout layout, List<GrammarNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                GrammarNode node = nodes[i];
                if (node.room == null)
                {
                    continue;
                }

                if (node.parentId >= 0 && node.parentId < nodes.Count && nodes[node.parentId].room != null)
                {
                    ConnectRooms(layout, nodes[node.parentId].room, node.room, false);
                    uniqueModules.Add("grammar_rule_sequence");
                }

                for (int loop = 0; loop < node.loopTargets.Count; loop++)
                {
                    int targetId = node.loopTargets[loop];
                    if (targetId < 0 || targetId >= nodes.Count || nodes[targetId].room == null)
                    {
                        continue;
                    }

                    if (node.room.floorIndex != nodes[targetId].room.floorIndex || layout.HasConnection(node.room.id, nodes[targetId].room.id))
                    {
                        continue;
                    }

                    float distance = Vector2Int.Distance(node.room.CenterCell, nodes[targetId].room.CenterCell);
                    if (distance <= maxExtraLoopDistance)
                    {
                        ConnectRooms(layout, node.room, nodes[targetId].room, true);
                    }
                }
            }
        }

        private void ConnectGrammarVerticalRules(DungeonLayout layout, List<GrammarNode> nodes)
        {
            if (!enableMultiFloorGrammar || layout.floorCount < 2)
            {
                return;
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                GrammarNode node = nodes[i];
                if (node.verticalParentId < 0 || node.verticalParentId >= nodes.Count)
                {
                    continue;
                }

                GrammarNode parent = nodes[node.verticalParentId];
                if (node.room == null || parent.room == null || layout.HasConnection(parent.room.id, node.room.id))
                {
                    continue;
                }

                ConnectVerticalRooms(layout, parent.room, node.room);
            }
        }

        private RectInt BoundsFromCenter(Vector2Int center, int width, int depth)
        {
            return new RectInt(center.x - width / 2, center.y - depth / 2, width, depth);
        }

        private static bool IsRoomInsideMap(DungeonLayout layout, RectInt bounds)
        {
            return bounds.xMin >= 1 &&
                   bounds.yMin >= 1 &&
                   bounds.xMax < layout.width - 1 &&
                   bounds.yMax < layout.depth - 1;
        }

        private Vector2Int RandomCardinalDirection()
        {
            return CardinalDirections[rng.Next(0, CardinalDirections.Length)];
        }

        private Vector2Int TurnDirection(Vector2Int direction)
        {
            if (direction == Vector2Int.zero)
            {
                return RandomCardinalDirection();
            }

            if (rng.NextDouble() < 0.5)
            {
                return new Vector2Int(-direction.y, direction.x);
            }

            return new Vector2Int(direction.y, -direction.x);
        }

        private Vector2Int[] ShuffledDirectionsWithPreferred(Vector2Int preferred)
        {
            Vector2Int[] result = new Vector2Int[CardinalDirections.Length];
            for (int i = 0; i < CardinalDirections.Length; i++)
            {
                result[i] = CardinalDirections[i];
            }

            for (int i = 0; i < result.Length; i++)
            {
                int swap = rng.Next(i, result.Length);
                Vector2Int temp = result[i];
                result[i] = result[swap];
                result[swap] = temp;
            }

            if (preferred != Vector2Int.zero)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i] == preferred)
                    {
                        result[i] = result[0];
                        result[0] = preferred;
                        break;
                    }
                }
            }

            return result;
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
                uniqueModules.Add("grammar_based_room");
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
                room.moduleId = "grammar_based_node";
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

        private void ConnectLegacySpatialEmbeddingFloor(DungeonLayout layout, List<DungeonRoom> floorRooms)
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
            if (!enableMultiFloorGrammar || layout.floorCount < 2 || verticalRulesPerFloorPair <= 0)
            {
                return;
            }

            for (int floor = 0; floor < layout.floorCount - 1; floor++)
            {
                int added = 0;
                int targetCount = Mathf.Max(1, verticalRulesPerFloorPair);
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
            uniqueModules.Add("grammar_based_vertical_edge");
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
            uniqueModules.Add(isExtraLoop ? "grammar_based_loop_edge" : "grammar_based_tree_edge");
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
                "Grammar-Based Generation",
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
            context.supportsVerticalConnectors = enableMultiFloorGrammar && floorCount > 1 && verticalRulesPerFloorPair > 0;
            context.supportsMultiFloor = enableMultiFloorGrammar && floorCount > 1;
            context.runCount = runCount;
            context.uniqueTopologyCount = uniqueTopologyCount;
            context.topologyDiversityRatio = diversityRatio;
            return context;
        }

        private static string BuildBatchSummary(DungeonBatchReport report, bool seedReproducible)
        {
            return "Teste Grammar-Based Generation executado com " + report.runCount
                + " seed(s). Topologias unicas: " + report.uniqueTopologyCount
                + "/" + report.runCount
                + ". Diversidade topologica: " + (report.topologyDiversityRatio * 100f).ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)
                + "%. Reprodutibilidade por seed: " + (seedReproducible ? "aprovada" : "falhou") + ".";
        }

        private void InstantiateLayout(DungeonLayout layout)
        {
            GameObject rootObject = new GameObject("Generated Grammar-Based Dungeon");
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
                uniqueModules.Add("grammar_based_floor_opening");
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

                GameObject lowerInstance = SpawnOptional(lowerPrefab, "grammar_based_stairs_up", CellToWorld(lowerCell, lower.floorIndex, 0f), rotation);

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
                Debug.LogWarning("No Grammar-Based Generation layout available to export as a 2D map. Generate a dungeon first.");
                return;
            }

            string metricsFolder = Path.Combine(Application.persistentDataPath, metricsFolderName);
            string mapFolder = Path.Combine(metricsFolder, string.IsNullOrEmpty(mapExportSubfolderName) ? "Maps" : mapExportSubfolderName);
            List<string> paths = DungeonMap2DExporter.ExportFloorMaps(
                LastLayout,
                mapFolder,
                metricsFilePrefix,
                "GRAMMAR",
                resolvedSeed,
                exportLabel,
                Mathf.Max(4, mapPixelsPerCell),
                mapIncludeGrid,
                mapIncludeLegend);

            Debug.Log("Grammar-Based Generation 2D map exported:\n" + string.Join("\n", paths.ToArray()));
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

    public static class GrammarBasedParameterNotes
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
            if (parameterName == "numRoomsTarget") return "Contagem de simbolos terminais da gramatica materializados como salas.";
            if (parameterName == "connectivityRatio") return "Busca sobre as conexoes derivadas pelas regras sequenciais, de ramificacao, loop e verticalidade.";
            if (parameterName == "verticalVariance") return "Desvio padrao das alturas dos nos/salas entre pavimentos.";
            if (parameterName == "fillPercentage") return "Celulas ocupadas por salas e corredores divididas pelo total do grid.";
            if (parameterName == "branchFactor") return "Media de conexoes entre simbolos terminais derivados.";
            if (parameterName == "avgPathLength") return "Distancias calculadas sobre a estrutura terminal gerada pela gramatica.";
            if (parameterName == "uniqueModules") return "Contagem de simbolos, regras logicas e prefabs usados pela montagem visual.";
            if (parameterName == "navigableVolumeRatio") return "Estimativa logica de celulas navegaveis antes de NavMesh.";
            if (parameterName == "criticalPathLength") return "Maior distancia encontrada a partir do simbolo inicial materializado.";
            if (parameterName == "avgAlternativePathLength") return "Media das regras de loop/atalho materializadas.";
            if (parameterName == "SupportsRandomEnemySpawns") return "Verifica se o sistema de spawn usa salas/celulas geradas por simbolos terminais.";
            if (parameterName == "SupportsLootDistribution") return "Verifica se loot pode ser distribuido sobre salas/celulas derivadas.";
            if (parameterName == "SupportsTraps") return "Verifica se armadilhas podem ser marcadas em areas navegaveis.";
            if (parameterName == "SupportsBacktrackingLoops") return "Conta execucoes em que regras de loop produziram ciclos navegaveis.";
            if (parameterName == "SupportsVerticalConnectors") return "Conta execucoes com regras verticais materializadas entre pavimentos.";
            if (parameterName == "SupportsMultiFloor") return "Conta execucoes com mais de um pavimento conectado por regras verticais.";
            if (parameterName == "SupportsBossArena") return "Conta execucoes com regra de arena ou sala ampla acima da area minima configurada.";
            if (parameterName == "SeedReproducible") return "Gera a mesma seed duas vezes e compara o hash topologico.";
            if (parameterName == "RuntimeRegeneration") return "Conta execucoes abaixo do limite de tempo configurado.";
            if (parameterName == "BudgetAwareSpawns") return "Verifica se os orcamentos de inimigos, loot e armadilhas cabem nas celulas livres.";
            if (parameterName == "Replayability") return "Estimativa baseada na diversidade de hashes topologicos entre seeds.";
            if (parameterName == "Debuggability") return "Estimativa baseada na clareza das regras de producao e dos simbolos materializados.";
            if (parameterName == "Flow") return "Estimativa baseada em conectividade, caminho critico e ramificacao.";
            if (parameterName == "Legibility") return "Estimativa baseada em conectividade, densidade e legibilidade da derivacao espacial.";
            if (parameterName == "StructuralVariety") return "Estimativa baseada em quantidade de salas, modulos e loops.";
            if (parameterName == "layoutGenerationMilliseconds") return "Tempo para expandir regras, interpretar simbolos no grid e conectar derivacoes.";
            if (parameterName == "geometryInstantiationMilliseconds") return "Tempo gasto instanciando prefabs/objetos visuais quando habilitado.";
            if (parameterName == "metricsCalculationMilliseconds") return "Tempo para calcular e consolidar metricas apos a geracao.";
            if (parameterName == "totalGenerationMilliseconds") return "Tempo total medido da execucao.";
            if (parameterName == "generatedGameObjectCount") return "Quantidade de GameObjects criados durante a montagem visual.";
            if (parameterName == "occupiedCellCount") return "Quantidade de celulas ocupadas no grid logico.";
            if (parameterName == "connectionCount") return "Quantidade de conexoes criadas por regras sequenciais, laterais, loops e verticalidade.";
            if (parameterName == "managedMemoryDeltaKB") return "Variacao aproximada de memoria gerenciada durante a execucao.";
            return "Parametro medido na variante Grammar-Based Generation.";
        }

        private static string InterpretationFor(string parameterName, string existing)
        {
            if (parameterName == "numRoomsTarget") return "Indica quantos simbolos terminais da gramatica viraram salas no grid.";
            if (parameterName == "connectivityRatio") return "Mostra se a derivacao sequencial e suas ramificacoes permaneceram conectadas.";
            if (parameterName == "verticalVariance") return "Valor acima de 0 indica que regras verticais geraram pavimentos conectados.";
            if (parameterName == "branchFactor") return "Valores maiores indicam mais derivacoes laterais, atalhos e variacao de fluxo.";
            if (parameterName == "criticalPathLength") return "Aproxima o percurso entre o simbolo inicial e o terminal mais distante.";
            if (parameterName == "SupportsVerticalConnectors") return "Parametro atendido quando regras verticais da propria gramatica foram materializadas.";
            if (parameterName == "SupportsMultiFloor") return "Parametro atendido quando a gramatica derivou pavimentos conectados.";
            if (parameterName == "Debuggability") return "A derivacao por regras facilita explicar por que cada sala existe.";
            if (parameterName == "layoutGenerationMilliseconds") return "Representa o custo da expansao e interpretacao da gramatica.";
            if (parameterName == "connectionCount") return "Reflete a complexidade topologica resultante das regras de producao.";
            return existing.Replace("BSP", "Grammar-Based Generation").Replace("bsp", "grammar_based").Replace("Room Graph", "Grammar-Based Generation").Replace("WFC", "Grammar-Based Generation");
        }

        private static string NoteFor(string parameterName)
        {
            if (parameterName == "numRoomsTarget") return "A gramatica controla quantidade por regras de caminho principal, ramificacoes e terminais; a interpretacao espacial pode descartar simbolos se nao houver espaco.";
            if (parameterName == "connectivityRatio") return "A sequencia derivada tende a ser conectada porque cada simbolo nasce de um simbolo anterior; falhas indicam limitacao de embedding espacial.";
            if (parameterName == "verticalVariance") return "Verticalidade e suportada quando a gramatica inclui regras entre pavimentos.";
            if (parameterName == "fillPercentage") return "A densidade depende do numero de simbolos terminais, tamanhos das salas e comprimento dos corredores.";
            if (parameterName == "branchFactor") return "Ramificacao e controlada por regras laterais e por regras de loop.";
            if (parameterName == "avgPathLength") return "Caminhos sao consequencia da ordem de derivacao, nao de busca posterior ou reparo.";
            if (parameterName == "uniqueModules") return "A variedade vem dos simbolos de entrada, sala, ramificacao, tesouro, armadilha, arena, saida e dos assets configurados.";
            if (parameterName == "navigableVolumeRatio") return "A navegabilidade e estimada no grid logico; validacao fisica final ainda exige NavMesh.";
            if (parameterName == "criticalPathLength") return "A gramatica pode controlar um caminho principal longo por regras sequenciais.";
            if (parameterName == "avgAlternativePathLength") return "Loops sao suportados apenas quando a gramatica inclui regras de retorno/atalho.";
            if (parameterName == "SupportsRandomEnemySpawns") return "Spawns usam salas/celulas derivadas; balanceamento de combate ainda e camada de gameplay.";
            if (parameterName == "SupportsLootDistribution") return "Loot pode usar simbolos Treasure ou salas derivadas, mas progressao economica exige regra complementar.";
            if (parameterName == "SupportsTraps") return "Armadilhas podem usar simbolos Trap ou gargalos, mas pacing de desafio exige camada semantica.";
            if (parameterName == "SupportsBacktrackingLoops") return "Suporte nativo quando a gramatica inclui producoes de loop; nao precisa misturar outro algoritmo.";
            if (parameterName == "SupportsVerticalConnectors") return "Implementado por regras verticais da propria gramatica, sem BSP, Room Graph, WFC ou outro metodo.";
            if (parameterName == "SupportsMultiFloor") return "Implementado por simbolos em pavimentos diferentes e producoes verticais entre eles.";
            if (parameterName == "SupportsBossArena") return "Suportado por uma regra terminal especifica de BossArena, com tamanho configuravel.";
            if (parameterName == "SeedReproducible") return "O Grammar-Based Generation e deterministico quando todas as escolhas usam a mesma seed.";
            if (parameterName == "RuntimeRegeneration") return "O custo cresce com quantidade de simbolos, tentativas de embedding e numero de pavimentos.";
            if (parameterName == "BudgetAwareSpawns") return "Orcamentos sao aplicados apos a derivacao, sobre celulas livres.";
            if (parameterName == "Replayability") return "A variacao entre seeds depende da escolha das producoes e da interpretacao espacial.";
            if (parameterName == "Debuggability") return "A gramatica e facil de documentar porque cada resultado pode ser explicado por uma sequencia de regras.";
            if (parameterName == "Flow") return "O controle de fluxo e uma vantagem forte, pois caminho principal, ramificacoes e loops sao regras explicitas.";
            if (parameterName == "Legibility") return "A legibilidade depende da interpretacao espacial: regras claras podem gerar mapas confusos se o embedding ficar apertado.";
            if (parameterName == "StructuralVariety") return "A variedade estrutural depende do repertorio de producoes; aumentar a gramatica aumenta expressividade.";
            if (parameterName == "layoutGenerationMilliseconds") return "Mede custo de expandir regras, posicionar simbolos e abrir corredores; compare separado da instanciacao Unity.";
            if (parameterName == "geometryInstantiationMilliseconds") return "Custo visual depende dos prefabs/Unity, nao do Grammar-Based Generation puro.";
            if (parameterName == "metricsCalculationMilliseconds") return "Custo da instrumentacao, nao do algoritmo.";
            if (parameterName == "totalGenerationMilliseconds") return "Inclui layout, instanciacao visual e metricas; use junto com o tempo logico.";
            if (parameterName == "generatedGameObjectCount") return "Reflete peso da montagem visual da cena derivada.";
            if (parameterName == "occupiedCellCount") return "Proxy de escala espacial resultante dos simbolos e corredores materializados.";
            if (parameterName == "connectionCount") return "Metrica natural para a gramatica, pois corresponde ao numero de relacoes materializadas entre simbolos.";
            if (parameterName == "managedMemoryDeltaKB") return "Estimativa sujeita ao GC da Unity; use como indicio comparativo.";
            return "Parametro avaliado na variante Grammar-Based Generation pura.";
        }
    }
}

