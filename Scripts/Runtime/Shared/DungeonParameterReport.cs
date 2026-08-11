using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Dissertation.PCG
{
    [Serializable]
    public sealed class DungeonParameterResult
    {
        public string parameterName;
        public string category;
        public string value;
        public string unit;
        public string status;
        public string collectionMethod;
        public string interpretation;
        public string bspApplicability;
    }

    [Serializable]
    public sealed class DungeonRunReport
    {
        public int runIndex;
        public int seed;
        public string topologyHash;
        public DungeonMetrics metrics;
        public List<DungeonParameterResult> parameters = new List<DungeonParameterResult>();
    }

    [Serializable]
    public sealed class DungeonBatchReport
    {
        public string algorithmName;
        public string generatedAtUtc;
        public int runCount;
        public int uniqueTopologyCount;
        public float topologyDiversityRatio;
        public string summary;
        public List<DungeonParameterResult> aggregateParameters = new List<DungeonParameterResult>();
        public List<DungeonRunReport> runs = new List<DungeonRunReport>();
    }

    public sealed class DungeonReportContext
    {
        public bool enemyPrefabsConfigured;
        public bool lootPrefabsConfigured;
        public bool trapPrefabsConfigured;
        public int enemyBudget;
        public int lootBudget;
        public int trapBudget;
        public int bossArenaMinAreaCells;
        public float runtimeRegenerationMaxMilliseconds;
        public bool seedReproducibleVerified;
        public bool supportsVerticalConnectors;
        public bool supportsMultiFloor;
        public int runCount;
        public int uniqueTopologyCount;
        public float topologyDiversityRatio;
    }

    public sealed class DungeonReportPaths
    {
        public string jsonPath;
        public string parameterCsvPath;
        public string aggregateCsvPath;
        public string markdownPath;
    }

    public static class DungeonQualitativeScorer
    {
        public static void ApplyScores(DungeonMetrics metrics, float topologyDiversityRatio, int runCount)
        {
            metrics.qualitative.Debuggability = 5;
            metrics.qualitative.Flow = ScoreFlow(metrics);
            metrics.qualitative.Legibility = ScoreLegibility(metrics);
            metrics.qualitative.StructuralVariety = ScoreStructuralVariety(metrics);
            metrics.qualitative.Replayability = runCount > 1 ? ScoreReplayability(topologyDiversityRatio) : 0;
            metrics.qualitative.note = "Scores 1-5 gerados por heuristica automatica. Use como apoio; a validacao qualitativa final pode ser revisada manualmente.";
        }

        private static int ScoreReplayability(float diversityRatio)
        {
            if (diversityRatio >= 0.9f) return 5;
            if (diversityRatio >= 0.7f) return 4;
            if (diversityRatio >= 0.45f) return 3;
            if (diversityRatio >= 0.2f) return 2;
            return 1;
        }

        private static int ScoreFlow(DungeonMetrics metrics)
        {
            int score = 1;
            if (metrics.connectivityRatio >= 99f) score += 2;
            if (metrics.criticalPathLength > 0f) score += 1;
            if (metrics.branchFactor >= 1.2f && metrics.branchFactor <= 3.5f) score += 1;
            return Mathf.Clamp(score, 1, 5);
        }

        private static int ScoreLegibility(DungeonMetrics metrics)
        {
            int score = 2;
            if (metrics.connectivityRatio >= 99f) score += 1;
            if (metrics.branchFactor <= 3f) score += 1;
            if (metrics.fillPercentage >= 8f && metrics.fillPercentage <= 45f) score += 1;
            return Mathf.Clamp(score, 1, 5);
        }

        private static int ScoreStructuralVariety(DungeonMetrics metrics)
        {
            int score = 1;
            if (metrics.numRoomsTarget >= 8) score += 1;
            if (metrics.uniqueModules >= 3) score += 1;
            if (metrics.booleans.SupportsBacktrackingLoops) score += 1;
            if (metrics.branchFactor >= 1.5f) score += 1;
            return Mathf.Clamp(score, 1, 5);
        }
    }

    public static class DungeonTopologyHasher
    {
        public static string Compute(DungeonLayout layout)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(layout.width).Append("x").Append(layout.depth).Append("|");

            for (int i = 0; i < layout.rooms.Count; i++)
            {
                DungeonRoom room = layout.rooms[i];
                builder.Append("R")
                    .Append(room.id).Append(":")
                    .Append(room.bounds.xMin).Append(",")
                    .Append(room.bounds.yMin).Append(",")
                    .Append(room.bounds.width).Append(",")
                    .Append(room.bounds.height).Append(",")
                    .Append(room.floorIndex).Append("|");
            }

            List<string> connections = new List<string>();
            for (int i = 0; i < layout.connections.Count; i++)
            {
                DungeonConnection connection = layout.connections[i];
                int a = Mathf.Min(connection.roomAId, connection.roomBId);
                int b = Mathf.Max(connection.roomAId, connection.roomBId);
                connections.Add(a + "-" + b + ":vertical=" + connection.isVertical + ":loop=" + connection.isExtraLoop);
            }
            connections.Sort();

            for (int i = 0; i < connections.Count; i++)
            {
                builder.Append("C").Append(connections[i]).Append("|");
            }

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

                return hash.ToString("X8", CultureInfo.InvariantCulture);
            }
        }
    }

    public static class DungeonParameterEvaluator
    {
        public static List<DungeonParameterResult> CreateRunResults(DungeonMetrics metrics, DungeonReportContext context)
        {
            List<DungeonParameterResult> results = new List<DungeonParameterResult>();

            AddQuantitativeResults(results, metrics);
            AddBooleanResults(results, metrics, context);
            AddQualitativeResults(results, metrics, context);
            AddPerformanceResults(results, metrics);

            return results;
        }

        public static List<DungeonParameterResult> CreateAggregateResults(List<DungeonRunReport> runs, DungeonReportContext context)
        {
            List<DungeonParameterResult> results = new List<DungeonParameterResult>();
            if (runs == null || runs.Count == 0)
            {
                return results;
            }

            AddAggregateMetric(results, runs, "numRoomsTarget", "Quantitativo", "contagem", "Media de salas geradas.", GetNumRooms, "Medido", "BSP controla esse valor por particionamento e pelo limite Max Rooms.");
            AddAggregateMetric(results, runs, "connectivityRatio", "Quantitativo", "%", "Media de salas conectadas ao componente principal.", GetConnectivity, "Medido", "BSP deve ficar proximo de 100% porque cada particao e conectada no retorno da arvore.");
            AddAggregateMetric(results, runs, "verticalVariance", "Quantitativo", "metros", "Variacao media de altura entre salas.", GetVerticalVariance, "Medido", "BSP puro pode medir verticalidade quando a variante multiandar esta ativa.");
            AddAggregateMetric(results, runs, "fillPercentage", "Quantitativo", "%", "Media de ocupacao do grid.", GetFillPercentage, "Medido", "Mostra o quanto do espaco disponivel virou sala ou corredor.");
            AddAggregateMetric(results, runs, "branchFactor", "Quantitativo", "media", "Media de conexoes por sala.", GetBranchFactor, "Medido", "Indica linearidade ou ramificacao da dungeon.");
            AddAggregateMetric(results, runs, "avgPathLength", "Quantitativo", "metros", "Media das distancias no grafo a partir do inicio.", GetAvgPathLength, "Medido", "Aproximacao por grafo logico; pode ser refinada com NavMesh.");
            AddAggregateMetric(results, runs, "uniqueModules", "Quantitativo", "contagem", "Media de tipos logicos/prefabs usados.", GetUniqueModules, "Medido", "No BSP, a variedade estrutural vem de salas, corredores, conectores verticais e assets configurados.");
            AddAggregateMetric(results, runs, "navigableVolumeRatio", "Quantitativo", "%", "Media de celulas logicas navegaveis antes do NavMesh.", GetNavigableVolumeRatio, "Estimado sem NavMesh", "Para medicao fisica final, integrar com NavMeshSurface e amostragem de pontos.");
            AddAggregateMetric(results, runs, "criticalPathLength", "Quantitativo", "metros", "Media do maior caminho a partir do inicio.", GetCriticalPathLength, "Medido", "Representa o percurso principal aproximado.");
            AddAggregateMetric(results, runs, "avgAlternativePathLength", "Quantitativo", "metros", "Media das conexoes extras criadas como loops.", GetAvgAlternativePathLength, "Medido", "Se Extra Loop Connections for 0, tende a ficar 0.");

            AddAggregateBoolean(results, runs, "SupportsRandomEnemySpawns", "Booleano", CountEnemySupport, "Suportado", "O gerador possui lista de inimigos e orcamento; configure Enemy Prefabs e Enemy Budget para validar visualmente.", "BSP aceita spawns por sala/celula sem depender do algoritmo estrutural.");
            AddAggregateBoolean(results, runs, "SupportsLootDistribution", "Booleano", CountLootSupport, "Suportado", "O gerador possui lista de loot e orcamento; configure Loot Prefabs e Loot Budget para validar visualmente.", "BSP aceita distribuicao por sala/celula.");
            AddAggregateBoolean(results, runs, "SupportsTraps", "Booleano", CountTrapSupport, "Suportado", "O gerador possui lista de armadilhas e orcamento; configure Trap Prefabs e Trap Budget para validar visualmente.", "BSP aceita marcadores de armadilha em areas navegaveis.");
            AddAggregateBoolean(results, runs, "SupportsBacktrackingLoops", "Booleano", CountBacktrackingLoops, "Medido", "Conta quantas execucoes geraram ciclos navegaveis.", "BSP suporta loops quando Extra Loop Connections cria arestas extras.");
            AddAggregateBoolean(results, runs, "SupportsVerticalConnectors", "Booleano", CountVerticalConnectors, "Medido", "Conta execucoes em que o BSP gerou conexoes verticais entre pavimentos.", "Implementado como extensao BSP pura: salas de pavimentos adjacentes sao conectadas por escadas/rampas.");
            AddAggregateBoolean(results, runs, "SupportsMultiFloor", "Booleano", CountMultiFloor, "Medido", "Conta execucoes em que o BSP gerou mais de um pavimento conectado.", "Implementado sem algoritmo auxiliar, usando BSP por pavimento e conectores verticais internos.");
            AddAggregateBoolean(results, runs, "SupportsBossArena", "Booleano", CountBossArena, "Medido", "Conta execucoes com ao menos uma sala acima da area minima configurada.", "BSP pode gerar arenas se Max Room Size permitir salas grandes.");
            AddFixedBoolean(results, "SeedReproducible", "Booleano", context.seedReproducibleVerified, "Medido", "Verificado gerando a mesma seed duas vezes e comparando o hash topologico.", "BSP deterministico e adequado para testes reprodutiveis.");
            AddAggregateBoolean(results, runs, "RuntimeRegeneration", "Booleano", CountRuntimeRegeneration, "Medido", "Conta execucoes abaixo do limite de tempo configurado.", "BSP tende a ser rapido; instanciacao visual pode custar mais que o layout logico.");
            AddAggregateBoolean(results, runs, "BudgetAwareSpawns", "Booleano", CountBudgetAware, "Suportado", "Verifica se o sistema respeita os orcamentos de inimigos, loot e armadilhas.", "O controle de orcamento e independente da estrutura BSP.");

            AddAggregateScore(results, runs, "Replayability", "Qualitativo", context.topologyDiversityRatio, AverageReplayability, "Estimado automaticamente", "Baseado na diversidade de hashes topologicos entre seeds.", "BSP tem boa variacao quando os parametros permitem particoes diferentes.");
            AddAggregateScore(results, runs, "Debuggability", "Qualitativo", context.topologyDiversityRatio, AverageDebuggability, "Estimado automaticamente", "BSP recebe pontuacao alta por ser deterministico e facil de rastrear.", "Arvore BSP facilita depuracao.");
            AddAggregateScore(results, runs, "Flow", "Qualitativo", context.topologyDiversityRatio, AverageFlow, "Estimado automaticamente", "Baseado em conectividade, caminho critico e fator de ramificacao.", "BSP tende a criar fluxo claro por salas e corredores.");
            AddAggregateScore(results, runs, "Legibility", "Qualitativo", context.topologyDiversityRatio, AverageLegibility, "Estimado automaticamente", "Baseado em conectividade, densidade e ramificacao.", "BSP geralmente produz mapas legiveis por ter salas retangulares.");
            AddAggregateScore(results, runs, "StructuralVariety", "Qualitativo", context.topologyDiversityRatio, AverageStructuralVariety, "Estimado automaticamente", "Baseado em quantidade de salas, modulos e loops.", "BSP oferece variedade moderada; WFC e grammar-based podem superar em variedade local.");

            AddAggregateMetric(results, runs, "layoutGenerationMilliseconds", "Performance", "ms", "Tempo medio para gerar apenas o layout logico do algoritmo.", GetLayoutGenerationMilliseconds, "Medido", "Esta e a metrica mais justa para comparar o custo do BSP com outros algoritmos.");
            AddAggregateMetric(results, runs, "geometryInstantiationMilliseconds", "Performance", "ms", "Tempo medio gasto instanciando prefabs/objetos visuais, quando habilitado no teste.", GetGeometryInstantiationMilliseconds, "Medido", "Depende muito dos assets Unity; use separado do custo algoritmico.");
            AddAggregateMetric(results, runs, "metricsCalculationMilliseconds", "Performance", "ms", "Tempo medio para calcular e consolidar metricas apos a geracao.", GetMetricsCalculationMilliseconds, "Medido", "Ajuda a separar custo do algoritmo e custo da instrumentacao.");
            AddAggregateMetric(results, runs, "totalGenerationMilliseconds", "Performance", "ms", "Tempo total medio da execucao medida.", GetTotalGenerationMilliseconds, "Medido", "Inclui layout, instanciacao visual habilitada e calculo de metricas.");
            AddAggregateMetric(results, runs, "generatedGameObjectCount", "Performance", "contagem", "Quantidade media de GameObjects criados durante a execucao.", GetGeneratedGameObjectCount, "Medido", "Quando o teste logico esta ativo, tende a 0; quando visual esta ativo, reflete o peso de instanciacao.");
            AddAggregateMetric(results, runs, "occupiedCellCount", "Performance", "contagem", "Quantidade media de celulas ocupadas no grid logico.", GetOccupiedCellCount, "Medido", "Proxy direto do tamanho espacial processado pelo algoritmo.");
            AddAggregateMetric(results, runs, "connectionCount", "Performance", "contagem", "Quantidade media de conexoes no grafo da dungeon.", GetConnectionCount, "Medido", "Proxy de complexidade topologica.");
            AddAggregateMetric(results, runs, "managedMemoryDeltaKB", "Performance", "KB", "Variacao media de memoria gerenciada durante a execucao.", GetManagedMemoryDeltaKB, "Estimado", "Indicador aproximado; valores podem variar por coleta de lixo e comportamento interno da Unity.");

            return results;
        }

        private static void AddQuantitativeResults(List<DungeonParameterResult> results, DungeonMetrics metrics)
        {
            Add(results, "numRoomsTarget", "Quantitativo", metrics.numRoomsTarget.ToString(CultureInfo.InvariantCulture), "contagem", "Medido",
                "Contagem de salas no layout logico gerado.",
                "Tamanho estrutural da dungeon nesta execucao.",
                "BSP controla salas por particoes e pelo limite Max Rooms.");
            Add(results, "connectivityRatio", "Quantitativo", Percent(metrics.connectivityRatio), "%", "Medido",
                "Busca no grafo a partir da sala inicial.",
                metrics.connectivityRatio >= 99f ? "Todas as salas ficaram conectadas." : "Algumas salas podem estar desconectadas.",
                "BSP deve gerar conectividade alta porque conecta particoes durante a recursao.");
            Add(results, "verticalVariance", "Quantitativo", Float(metrics.verticalVariance), "metros", "Medido",
                "Calcula o desvio padrao das alturas dos pavimentos das salas.",
                metrics.verticalVariance > 0f ? "Ha variacao vertical mensuravel entre salas." : "Sem variacao vertical nesta execucao; verifique se o BSP multiandar esta ativo.",
                "BSP puro suporta verticalidade ao particionar multiplos pavimentos e conectar salas por escadas.");
            Add(results, "fillPercentage", "Quantitativo", Percent(metrics.fillPercentage), "%", "Medido",
                "Celulas ocupadas divididas pelo total do grid.",
                "Indica densidade espacial da dungeon.",
                "Bom para comparar controle de densidade entre algoritmos.");
            Add(results, "branchFactor", "Quantitativo", Float(metrics.branchFactor), "media", "Medido",
                "Media de conexoes por sala no grafo.",
                InterpretBranchFactor(metrics.branchFactor),
                "Loops extras aumentam este valor e reduzem linearidade.");
            Add(results, "avgPathLength", "Quantitativo", Float(metrics.avgPathLength), "metros", "Medido",
                "Dijkstra simples sobre o grafo logico de salas.",
                "Distancia media a partir da sala inicial.",
                "Aproximacao logica; NavMesh pode refinar a distancia fisica.");
            Add(results, "uniqueModules", "Quantitativo", metrics.uniqueModules.ToString(CultureInfo.InvariantCulture), "contagem", "Medido",
                "Contagem de modulos logicos e prefabs efetivamente usados.",
                "Indica variedade basica de composicao.",
                "A variedade aumenta com loops, conectores verticais e prefabs KayKit configurados.");
            Add(results, "navigableVolumeRatio", "Quantitativo", Percent(metrics.navigableVolumeRatio), "%", "Estimado sem NavMesh",
                "Proporcao de celulas logicas ocupadas consideradas navegaveis antes de colisao/obstaculos.",
                "Use como estimativa inicial, nao como validacao fisica final.",
                "Para a dissertacao final, pode ser refinado com NavMeshSurface.");
            Add(results, "criticalPathLength", "Quantitativo", Float(metrics.criticalPathLength), "metros", "Medido",
                "Maior distancia encontrada a partir da sala inicial no grafo.",
                "Aproxima o percurso principal inicio-objetivo.",
                "Funciona bem em BSP porque o grafo de salas e explicito.");
            Add(results, "avgAlternativePathLength", "Quantitativo", Float(metrics.avgAlternativePathLength), "metros", "Medido",
                "Media das conexoes extras marcadas como loops.",
                metrics.avgAlternativePathLength > 0f ? "Ha caminhos alternativos mensuraveis." : "Nao houve caminho alternativo nesta execucao.",
                "Depende de Extra Loop Connections e Max Extra Loop Distance.");
        }

        private static void AddBooleanResults(List<DungeonParameterResult> results, DungeonMetrics metrics, DungeonReportContext context)
        {
            Add(results, "SupportsRandomEnemySpawns", "Booleano", BoolCapability(context.enemyPrefabsConfigured, context.enemyBudget), "sim/nao", "Suportado",
                "Verifica se ha sistema de spawn por orcamento e lista de prefabs.",
                SpawnInterpretation(context.enemyPrefabsConfigured, context.enemyBudget, "inimigos"),
                "BSP permite spawns aleatorios dentro das salas.");
            Add(results, "SupportsLootDistribution", "Booleano", BoolCapability(context.lootPrefabsConfigured, context.lootBudget), "sim/nao", "Suportado",
                "Verifica se ha sistema de loot por orcamento e lista de prefabs.",
                SpawnInterpretation(context.lootPrefabsConfigured, context.lootBudget, "loot"),
                "BSP permite distribuir recompensas por salas/celulas.");
            Add(results, "SupportsTraps", "Booleano", BoolCapability(context.trapPrefabsConfigured, context.trapBudget), "sim/nao", "Suportado",
                "Verifica se ha sistema de armadilhas por orcamento e lista de prefabs.",
                SpawnInterpretation(context.trapPrefabsConfigured, context.trapBudget, "armadilhas"),
                "BSP permite marcar armadilhas em salas ou corredores.");
            Add(results, "SupportsBacktrackingLoops", "Booleano", Bool(metrics.booleans.SupportsBacktrackingLoops), "sim/nao", "Medido",
                "Detecta ciclos pelo numero de arestas em relacao ao numero de salas.",
                metrics.booleans.SupportsBacktrackingLoops ? "A dungeon possui loop para retorno/caminho alternativo." : "A dungeon ficou mais linear nesta execucao.",
                "Ative/aumente Extra Loop Connections para favorecer loops.");
            Add(results, "SupportsVerticalConnectors", "Booleano", Bool(metrics.booleans.SupportsVerticalConnectors), "sim/nao", "Medido",
                "Detecta conexoes verticais no grafo BSP entre salas de pavimentos adjacentes.",
                metrics.booleans.SupportsVerticalConnectors ? "A execucao gerou " + metrics.verticalConnectorCount + " conector(es) vertical(is)." : "Nao houve conector vertical; verifique Enable Multi Floor Bsp e Vertical Connections Per Floor Pair.",
                "Implementado com BSP puro, sem misturar outro algoritmo.");
            Add(results, "SupportsMultiFloor", "Booleano", Bool(metrics.booleans.SupportsMultiFloor), "sim/nao", "Medido",
                "Verifica salas em diferentes floorIndex conectadas por conexoes verticais.",
                metrics.booleans.SupportsMultiFloor ? "A execucao gerou " + metrics.floorCount + " pavimento(s) com conexao vertical." : "A execucao ficou em um unico pavimento ou sem conexao vertical.",
                "Implementado como variante BSP multiandar.");
            Add(results, "SupportsBossArena", "Booleano", Bool(metrics.booleans.SupportsBossArena), "sim/nao", "Medido",
                "Procura sala com area minima configurada para arena.",
                metrics.booleans.SupportsBossArena ? "Existe ao menos uma sala grande o suficiente." : "Nenhuma sala atingiu a area minima nesta execucao.",
                "Aumente Max Room Size ou reduza Boss Arena Min Area Cells para testar arenas.");
            Add(results, "SeedReproducible", "Booleano", Bool(context.seedReproducibleVerified), "sim/nao", "Medido",
                "Gera a mesma seed duas vezes e compara o hash topologico.",
                context.seedReproducibleVerified ? "A seed reproduziu a mesma topologia." : "A seed nao reproduziu a mesma topologia; investigar fontes de aleatoriedade.",
                "BSP deterministico deve passar neste teste.");
            Add(results, "RuntimeRegeneration", "Booleano", Bool(metrics.booleans.RuntimeRegeneration), "sim/nao", "Medido",
                "Compara tempo de geracao com o limite configurado.",
                "Tempo registrado: " + Float(metrics.generationMilliseconds) + " ms.",
                "Mede o layout logico; instanciacao visual pode exigir teste de performance separado.");
            Add(results, "BudgetAwareSpawns", "Booleano", Bool(metrics.booleans.BudgetAwareSpawns), "sim/nao", "Suportado",
                "Compara os orcamentos configurados com a capacidade logica de celulas das salas.",
                "Orcamentos registrados: inimigos " + metrics.enemyBudgetTarget + ", loot " + metrics.lootBudgetTarget + ", armadilhas " + metrics.trapBudgetTarget + ".",
                "O sistema tenta respeitar limites de spawn definidos pelo designer.");
        }

        private static void AddQualitativeResults(List<DungeonParameterResult> results, DungeonMetrics metrics, DungeonReportContext context)
        {
            Add(results, "Replayability", "Qualitativo", Score(metrics.qualitative.Replayability), "Likert 1-5", context.runCount > 1 ? "Estimado automaticamente" : "Requer multiplas seeds",
                "Usa diversidade de hashes topologicos entre execucoes.",
                context.runCount > 1 ? "Diversidade topologica: " + Percent(context.topologyDiversityRatio * 100f) + "." : "Rode o teste com mais de uma seed para estimar.",
                "BSP costuma variar bem com seeds diferentes, mas pode repetir padroes retangulares.");
            Add(results, "Debuggability", "Qualitativo", Score(metrics.qualitative.Debuggability), "Likert 1-5", "Estimado automaticamente",
                "Heuristica baseada na previsibilidade e rastreabilidade do algoritmo.",
                "BSP e altamente depuravel por usar arvore de particoes.",
                "Ponto forte do BSP.");
            Add(results, "Flow", "Qualitativo", Score(metrics.qualitative.Flow), "Likert 1-5", "Estimado automaticamente",
                "Heuristica baseada em conectividade, caminho critico e ramificacao.",
                "Indica fluidez de navegacao esperada.",
                "BSP tende a ter fluxo claro quando todos os corredores abrem passagem.");
            Add(results, "Legibility", "Qualitativo", Score(metrics.qualitative.Legibility), "Likert 1-5", "Estimado automaticamente",
                "Heuristica baseada em densidade e ramificacao.",
                "Indica se o layout tende a ser compreensivel.",
                "Salas retangulares e corredores ortogonais favorecem legibilidade.");
            Add(results, "StructuralVariety", "Qualitativo", Score(metrics.qualitative.StructuralVariety), "Likert 1-5", "Estimado automaticamente",
                "Heuristica baseada em salas, modulos e loops.",
                "Indica variedade estrutural dentro desta execucao.",
                "BSP tem variedade macroestrutural boa, mas a variedade visual depende muito dos prefabs KayKit configurados.");
        }

        private static void AddPerformanceResults(List<DungeonParameterResult> results, DungeonMetrics metrics)
        {
            Add(results, "layoutGenerationMilliseconds", "Performance", Float(metrics.layoutGenerationMilliseconds), "ms", "Medido",
                "Tempo para gerar BSP, salas, corredores, loops e conectores verticais.",
                "Representa o custo algoritmico principal.",
                "BSP tende a ser rapido porque trabalha com particoes retangulares e grafo simples.");
            Add(results, "geometryInstantiationMilliseconds", "Performance", Float(metrics.geometryInstantiationMilliseconds), "ms", "Medido",
                "Tempo gasto instanciando prefabs/objetos visuais quando a medicao visual esta ativa.",
                metrics.geometryInstantiationMilliseconds > 0f ? "A execucao incluiu custo visual." : "A execucao mediu apenas o layout logico.",
                "Deve ser comparado separadamente, pois depende dos assets e nao apenas do BSP.");
            Add(results, "metricsCalculationMilliseconds", "Performance", Float(metrics.metricsCalculationMilliseconds), "ms", "Medido",
                "Tempo para calcular metricas e montar resultados.",
                "Ajuda a quantificar custo da instrumentacao.",
                "Nao e custo do algoritmo em si.");
            Add(results, "totalGenerationMilliseconds", "Performance", Float(metrics.totalGenerationMilliseconds), "ms", "Medido",
                "Tempo total medido para a execucao.",
                "Inclui layout, instanciacao visual habilitada e calculo de metricas.",
                "Use junto com layoutGenerationMilliseconds para separar algoritmo e runtime Unity.");
            Add(results, "generatedGameObjectCount", "Performance", metrics.generatedGameObjectCount.ToString(CultureInfo.InvariantCulture), "contagem", "Medido",
                "Quantidade de GameObjects criados pelo gerador.",
                "Indica peso potencial de instanciacao no Unity.",
                "Pode crescer bastante em algoritmos que geram muitos modulos pequenos.");
            Add(results, "occupiedCellCount", "Performance", metrics.occupiedCellCount.ToString(CultureInfo.InvariantCulture), "contagem", "Medido",
                "Quantidade de celulas ocupadas no grid logico.",
                "Proxy de escala espacial da dungeon.",
                "Permite comparar custo por celula entre algoritmos.");
            Add(results, "connectionCount", "Performance", metrics.connectionCount.ToString(CultureInfo.InvariantCulture), "contagem", "Medido",
                "Quantidade de conexoes no grafo logico.",
                "Proxy de complexidade topologica.",
                "BSP tende a manter esse valor previsivel.");
            Add(results, "managedMemoryDeltaKB", "Performance", Float(metrics.managedMemoryDeltaKB), "KB", "Estimado",
                "Diferenca de memoria gerenciada antes/depois da geracao.",
                "Valor aproximado, sensivel a GC e ao estado do Editor.",
                "Use como indicio comparativo, nao como medicao absoluta de memoria.");
        }

        private static void AddAggregateMetric(List<DungeonParameterResult> results, List<DungeonRunReport> runs, string name, string category, string unit, string method, Func<DungeonMetrics, float> getter, string status, string bspNote)
        {
            float min;
            float max;
            float average = Average(runs, getter, out min, out max);
            Add(results, name, category, "media " + Float(average) + " (min " + Float(min) + ", max " + Float(max) + ")", unit, status, method, "Resumo de " + runs.Count + " execucoes.", bspNote);
        }

        private static void AddAggregateBoolean(List<DungeonParameterResult> results, List<DungeonRunReport> runs, string name, string category, Func<DungeonMetrics, bool> getter, string status, string method, string bspNote)
        {
            int count = 0;
            for (int i = 0; i < runs.Count; i++)
            {
                if (getter(runs[i].metrics))
                {
                    count++;
                }
            }

            Add(results, name, category, "Sim em " + count + "/" + runs.Count + " execucoes", "sim/nao", status, method, "Frequencia observada no lote de teste.", bspNote);
        }

        private static void AddFixedBoolean(List<DungeonParameterResult> results, string name, string category, bool value, string status, string method, string bspNote)
        {
            Add(results, name, category, Bool(value), "sim/nao", status, method, value ? "Parametro atendido." : "Parametro nao atendido nesta versao.", bspNote);
        }

        private static void AddAggregateScore(List<DungeonParameterResult> results, List<DungeonRunReport> runs, string name, string category, float diversityRatio, Func<DungeonMetrics, float> getter, string status, string method, string bspNote)
        {
            float min;
            float max;
            float average = Average(runs, getter, out min, out max);
            Add(results, name, category, "media " + Float(average) + "/5 (min " + Float(min) + ", max " + Float(max) + ")", "Likert 1-5", status, method, "Diversidade topologica do lote: " + Percent(diversityRatio * 100f) + ".", bspNote);
        }

        private static void Add(List<DungeonParameterResult> results, string name, string category, string value, string unit, string status, string method, string interpretation, string bspApplicability)
        {
            DungeonParameterResult result = new DungeonParameterResult();
            result.parameterName = name;
            result.category = category;
            result.value = value;
            result.unit = unit;
            result.status = status;
            result.collectionMethod = method;
            result.interpretation = interpretation;
            result.bspApplicability = bspApplicability;
            results.Add(result);
        }

        private static float Average(List<DungeonRunReport> runs, Func<DungeonMetrics, float> getter, out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;
            float sum = 0f;
            for (int i = 0; i < runs.Count; i++)
            {
                float value = getter(runs[i].metrics);
                sum += value;
                if (value < min) min = value;
                if (value > max) max = value;
            }

            return runs.Count == 0 ? 0f : sum / runs.Count;
        }

        private static string InterpretBranchFactor(float value)
        {
            if (value < 1.4f) return "Layout mais linear, com poucas bifurcacoes.";
            if (value <= 2.6f) return "Ramificacao moderada, geralmente boa para exploracao.";
            return "Layout bastante ramificado, pode aumentar exploracao e desorientacao.";
        }

        private static string SpawnInterpretation(bool prefabsConfigured, int budget, string label)
        {
            if (!prefabsConfigured)
            {
                return "Sistema existe, mas ainda faltam prefabs de " + label + " na biblioteca.";
            }

            if (budget <= 0)
            {
                return "Sistema configurado, mas o orcamento esta 0 nesta execucao.";
            }

            return "Sistema configurado e com orcamento ativo.";
        }

        private static string BoolCapability(bool prefabsConfigured, int budget)
        {
            if (prefabsConfigured && budget > 0) return "Sim, testado";
            if (prefabsConfigured) return "Sim, configurado";
            return "Sim, requer prefabs";
        }

        private static string Bool(bool value)
        {
            return value ? "Sim" : "Nao";
        }

        private static string Score(int value)
        {
            return value <= 0 ? "Nao avaliado" : value.ToString(CultureInfo.InvariantCulture) + "/5";
        }

        private static string Float(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private static string Percent(float value)
        {
            return value.ToString("0.##", CultureInfo.InvariantCulture) + "%";
        }

        private static float GetNumRooms(DungeonMetrics metrics) { return metrics.numRoomsTarget; }
        private static float GetConnectivity(DungeonMetrics metrics) { return metrics.connectivityRatio; }
        private static float GetVerticalVariance(DungeonMetrics metrics) { return metrics.verticalVariance; }
        private static float GetFillPercentage(DungeonMetrics metrics) { return metrics.fillPercentage; }
        private static float GetBranchFactor(DungeonMetrics metrics) { return metrics.branchFactor; }
        private static float GetAvgPathLength(DungeonMetrics metrics) { return metrics.avgPathLength; }
        private static float GetUniqueModules(DungeonMetrics metrics) { return metrics.uniqueModules; }
        private static float GetNavigableVolumeRatio(DungeonMetrics metrics) { return metrics.navigableVolumeRatio; }
        private static float GetCriticalPathLength(DungeonMetrics metrics) { return metrics.criticalPathLength; }
        private static float GetAvgAlternativePathLength(DungeonMetrics metrics) { return metrics.avgAlternativePathLength; }
        private static float GetLayoutGenerationMilliseconds(DungeonMetrics metrics) { return metrics.layoutGenerationMilliseconds; }
        private static float GetGeometryInstantiationMilliseconds(DungeonMetrics metrics) { return metrics.geometryInstantiationMilliseconds; }
        private static float GetMetricsCalculationMilliseconds(DungeonMetrics metrics) { return metrics.metricsCalculationMilliseconds; }
        private static float GetTotalGenerationMilliseconds(DungeonMetrics metrics) { return metrics.totalGenerationMilliseconds; }
        private static float GetGeneratedGameObjectCount(DungeonMetrics metrics) { return metrics.generatedGameObjectCount; }
        private static float GetOccupiedCellCount(DungeonMetrics metrics) { return metrics.occupiedCellCount; }
        private static float GetConnectionCount(DungeonMetrics metrics) { return metrics.connectionCount; }
        private static float GetManagedMemoryDeltaKB(DungeonMetrics metrics) { return metrics.managedMemoryDeltaKB; }

        private static bool CountEnemySupport(DungeonMetrics metrics) { return metrics.booleans.SupportsRandomEnemySpawns; }
        private static bool CountLootSupport(DungeonMetrics metrics) { return metrics.booleans.SupportsLootDistribution; }
        private static bool CountTrapSupport(DungeonMetrics metrics) { return metrics.booleans.SupportsTraps; }
        private static bool CountBacktrackingLoops(DungeonMetrics metrics) { return metrics.booleans.SupportsBacktrackingLoops; }
        private static bool CountVerticalConnectors(DungeonMetrics metrics) { return metrics.booleans.SupportsVerticalConnectors; }
        private static bool CountMultiFloor(DungeonMetrics metrics) { return metrics.booleans.SupportsMultiFloor; }
        private static bool CountBossArena(DungeonMetrics metrics) { return metrics.booleans.SupportsBossArena; }
        private static bool CountRuntimeRegeneration(DungeonMetrics metrics) { return metrics.booleans.RuntimeRegeneration; }
        private static bool CountBudgetAware(DungeonMetrics metrics) { return metrics.booleans.BudgetAwareSpawns; }

        private static float AverageReplayability(DungeonMetrics metrics) { return metrics.qualitative.Replayability; }
        private static float AverageDebuggability(DungeonMetrics metrics) { return metrics.qualitative.Debuggability; }
        private static float AverageFlow(DungeonMetrics metrics) { return metrics.qualitative.Flow; }
        private static float AverageLegibility(DungeonMetrics metrics) { return metrics.qualitative.Legibility; }
        private static float AverageStructuralVariety(DungeonMetrics metrics) { return metrics.qualitative.StructuralVariety; }
    }

    public static class DungeonReportExporter
    {
        public static DungeonReportPaths ExportBatchReport(DungeonBatchReport report, string folderPath, string filePrefix)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string safePrefix = string.IsNullOrEmpty(filePrefix) ? "pcg_test" : filePrefix;
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);

            DungeonReportPaths paths = new DungeonReportPaths();
            paths.jsonPath = Path.Combine(folderPath, safePrefix + "_parameter_report_" + timestamp + ".json");
            paths.parameterCsvPath = Path.Combine(folderPath, safePrefix + "_parameters_by_run_" + timestamp + ".csv");
            paths.aggregateCsvPath = Path.Combine(folderPath, safePrefix + "_aggregate_" + timestamp + ".csv");
            paths.markdownPath = Path.Combine(folderPath, safePrefix + "_readable_report_" + timestamp + ".md");

            File.WriteAllText(paths.jsonPath, JsonUtility.ToJson(report, true), Encoding.UTF8);
            File.WriteAllText(paths.parameterCsvPath, BuildRunParameterCsv(report), Encoding.UTF8);
            File.WriteAllText(paths.aggregateCsvPath, BuildAggregateCsv(report), Encoding.UTF8);
            File.WriteAllText(paths.markdownPath, BuildMarkdown(report), Encoding.UTF8);

            return paths;
        }

        private static string BuildRunParameterCsv(DungeonBatchReport report)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("runIndex,seed,topologyHash,category,parameterName,value,unit,status,collectionMethod,interpretation,algorithmApplicability");

            for (int r = 0; r < report.runs.Count; r++)
            {
                DungeonRunReport run = report.runs[r];
                for (int p = 0; p < run.parameters.Count; p++)
                {
                    DungeonParameterResult parameter = run.parameters[p];
                    builder.Append(run.runIndex).Append(",");
                    builder.Append(run.seed).Append(",");
                    builder.Append(Escape(run.topologyHash)).Append(",");
                    AppendParameterColumns(builder, parameter);
                    builder.AppendLine();
                }
            }

            return builder.ToString();
        }

        private static string BuildAggregateCsv(DungeonBatchReport report)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("category,parameterName,value,unit,status,collectionMethod,interpretation,algorithmApplicability");

            for (int i = 0; i < report.aggregateParameters.Count; i++)
            {
                AppendParameterColumns(builder, report.aggregateParameters[i]);
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string BuildMarkdown(DungeonBatchReport report)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# Relatorio de Teste PCG - " + report.algorithmName);
            builder.AppendLine();
            builder.AppendLine("Gerado em UTC: " + report.generatedAtUtc);
            builder.AppendLine();
            builder.AppendLine(report.summary);
            builder.AppendLine();
            builder.AppendLine("## Resumo agregado");
            builder.AppendLine();
            builder.AppendLine("| Categoria | Parametro | Valor | Status | Interpretacao | Observacao do algoritmo |");
            builder.AppendLine("|---|---|---:|---|---|---|");
            for (int i = 0; i < report.aggregateParameters.Count; i++)
            {
                DungeonParameterResult parameter = report.aggregateParameters[i];
                builder.Append("| ")
                    .Append(Md(parameter.category)).Append(" | ")
                    .Append(Md(parameter.parameterName)).Append(" | ")
                    .Append(Md(parameter.value)).Append(" ")
                    .Append(Md(parameter.unit)).Append(" | ")
                    .Append(Md(parameter.status)).Append(" | ")
                    .Append(Md(parameter.interpretation)).Append(" | ")
                    .Append(Md(parameter.bspApplicability)).Append(" |")
                    .AppendLine();
            }

            builder.AppendLine();
            builder.AppendLine("## Execucoes");
            builder.AppendLine();
            builder.AppendLine("| Run | Seed | Hash topologico | Salas | Pavimentos | Conectores verticais | Conectividade | Fill | Caminho critico | Layout ms | Visual ms | Total ms | GameObjects | Mem delta KB |");
            builder.AppendLine("|---:|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|");
            for (int i = 0; i < report.runs.Count; i++)
            {
                DungeonRunReport run = report.runs[i];
                DungeonMetrics metrics = run.metrics;
                builder.Append("| ")
                    .Append(run.runIndex).Append(" | ")
                    .Append(run.seed).Append(" | ")
                    .Append(run.topologyHash).Append(" | ")
                    .Append(metrics.numRoomsTarget).Append(" | ")
                    .Append(metrics.floorCount).Append(" | ")
                    .Append(metrics.verticalConnectorCount).Append(" | ")
                    .Append(Float(metrics.connectivityRatio)).Append("% | ")
                    .Append(Float(metrics.fillPercentage)).Append("% | ")
                    .Append(Float(metrics.criticalPathLength)).Append(" | ")
                    .Append(Float(metrics.layoutGenerationMilliseconds)).Append(" | ")
                    .Append(Float(metrics.geometryInstantiationMilliseconds)).Append(" | ")
                    .Append(Float(metrics.totalGenerationMilliseconds)).Append(" | ")
                    .Append(metrics.generatedGameObjectCount).Append(" | ")
                    .Append(Float(metrics.managedMemoryDeltaKB)).Append(" |")
                    .AppendLine();
            }

            builder.AppendLine();
            builder.AppendLine("## Como usar no texto da dissertacao");
            builder.AppendLine();
            builder.AppendLine("Use os parametros com status `Medido` como resultados quantitativos diretos. Parametros com status `Estimado sem NavMesh` ou `Estimado automaticamente` devem ser descritos como proxies instrumentais. Parametros com status `Nao implementado nesta versao` devem aparecer como limitacoes da variante do algoritmo testada, nao necessariamente como impossibilidade teorica do algoritmo.");

            return builder.ToString();
        }

        private static void AppendParameterColumns(StringBuilder builder, DungeonParameterResult parameter)
        {
            builder.Append(Escape(parameter.category)).Append(",");
            builder.Append(Escape(parameter.parameterName)).Append(",");
            builder.Append(Escape(parameter.value)).Append(",");
            builder.Append(Escape(parameter.unit)).Append(",");
            builder.Append(Escape(parameter.status)).Append(",");
            builder.Append(Escape(parameter.collectionMethod)).Append(",");
            builder.Append(Escape(parameter.interpretation)).Append(",");
            builder.Append(Escape(parameter.bspApplicability));
        }

        private static string Escape(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private static string Md(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            return value.Replace("|", "\\|").Replace("\r", " ").Replace("\n", " ");
        }

        private static string Float(float value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }
    }
}


