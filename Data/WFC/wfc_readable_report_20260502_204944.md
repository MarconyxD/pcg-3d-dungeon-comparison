# Relatorio de Teste PCG - Wave Function Collapse

Gerado em UTC: 2026-05-02T20:49:41.9729662Z

Teste Wave Function Collapse executado com 30 seed(s). Topologias unicas: 30/30. Diversidade topologica: 100%. Reprodutibilidade por seed: aprovada.

## Resumo agregado

| Categoria | Parametro | Valor | Status | Interpretacao | Observacao do algoritmo |
|---|---|---:|---|---|---|
| Quantitativo | numRoomsTarget | media 42.967 (min 33, max 50) contagem | Medido | Resumo de 30 execucoes. | WFC nao controla salas como entidade global; salas emergem de componentes de tiles. |
| Quantitativo | connectivityRatio | media 3.35 (min 2.128, max 8) % | Medido | Mostra se o WFC produziu uma dungeon conectada sem reparo global posterior. | WFC puro nao garante conectividade global por padrao; esta variante usa uma espinha dorsal observada como restricao inicial do proprio WFC, nao como reparo posterior. |
| Quantitativo | verticalVariance | media 1.981 (min 1.915, max 2) metros | Medido | Resumo de 30 execucoes. | WFC pode representar verticalidade se o tileset tiver sockets verticais. |
| Quantitativo | fillPercentage | media 96.654 (min 95.215, max 97.644) % | Medido | Resumo de 30 execucoes. | Densidade e resultado de pesos e compatibilidades locais, nao de controle global direto. |
| Quantitativo | branchFactor | media 0.26 (min 0.051, max 0.609) media | Medido | Resumo de 30 execucoes. | Ramificacao emerge das conexoes locais; controlar grau medio diretamente exige tileset/pesos bem calibrados. |
| Quantitativo | avgPathLength | media 8.156 (min 0, max 76) metros | Medido | Resumo de 30 execucoes. | Caminhos sao medidos apos extrair um grafo do colapso; nao sao objetivo nativo do WFC. |
| Quantitativo | uniqueModules | media 25.8 (min 24, max 26) contagem | Medido | Resumo de 30 execucoes. | WFC tende a ser forte em variedade local quando ha um tileset rico. |
| Quantitativo | navigableVolumeRatio | media 100 (min 100, max 100) % | Estimado sem NavMesh | Resumo de 30 execucoes. | Proxy logico; validacao final ainda depende de NavMesh e colisores. |
| Quantitativo | criticalPathLength | media 10.333 (min 0, max 76) metros | Medido | Resumo de 30 execucoes. | Caminho critico nao e controlado diretamente pelo WFC puro. |
| Quantitativo | avgAlternativePathLength | media 15.744 (min 0, max 110) metros | Medido | Resumo de 30 execucoes. | Loops podem emergir, mas nao sao garantidos sem regras locais que favorecam ciclos. |
| Booleano | SupportsRandomEnemySpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Spawns usam celulas de sala extraidas; balanceamento e camada de gameplay. |
| Booleano | SupportsLootDistribution | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Loot pode ser colocado sobre componentes de sala, mas progressao por risco nao e nativa do WFC. |
| Booleano | SupportsTraps | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Armadilhas podem usar celulas/corredores resultantes; sem semantica global nativa. |
| Booleano | SupportsBacktrackingLoops | Sim em 0/30 execucoes sim/nao | Medido | Parametro atendido quando o resultado WFC contem ciclos inferidos no grafo extraido. | WFC pode criar loops emergentes; se a metrica vier baixa, isso e uma limitacao da configuracao pura testada. |
| Booleano | SupportsVerticalConnectors | Sim em 2/30 execucoes sim/nao | Medido | Parametro atendido somente quando os modulos verticais do WFC aparecem conectados a salas/corredores. | Suportado quando tiles verticais e observacoes iniciais fazem parte do proprio tileset WFC, sem BSP/Room Graph. |
| Booleano | SupportsMultiFloor | Sim em 2/30 execucoes sim/nao | Medido | Parametro atendido quando ha pavimentos conectados por sockets verticais do proprio WFC. | Multiandar e possivel em WFC 3D com sockets verticais, mas nao e garantido em WFC 2D simples. |
| Booleano | SupportsBossArena | Sim em 28/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Arenas grandes nao sao naturais no WFC local; exigem tiles/padroes que favorecam areas amplas. |
| Booleano | SeedReproducible | Sim sim/nao | Medido | Parametro atendido. | O colapso e reproduzivel se todas as escolhas usam a mesma seed. |
| Booleano | RuntimeRegeneration | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Custo depende do tamanho do volume, quantidade de modulos e numero de contradicoes/reinicios. |
| Booleano | BudgetAwareSpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Orcamento e aplicado depois sobre celulas livres extraidas; nao e propriedade nativa do WFC. |
| Qualitativo | Replayability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | WFC costuma variar bem em padroes locais; diversidade topologica depende do tileset. |
| Qualitativo | Debuggability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | WFC e deterministico por seed, mas contradicoes e propagacao de entropia tornam a depuracao menos direta que BSP/Room Graph. | Mais dificil de depurar que BSP/Room Graph, pois erros aparecem como contradicoes de restricao. |
| Qualitativo | Flow | media 1.267/5 (min 1, max 2) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Fluxo global nao e garantido por restricoes locais; precisa ser medido apos o colapso. |
| Qualitativo | Legibility | media 3/5 (min 3, max 3) Likert 1-5 | Estimado automaticamente | A legibilidade depende muito do conjunto de tiles e das restricoes locais usadas. | Boa legibilidade exige tileset com padroes claros; ruido local pode reduzir leitura espacial. |
| Qualitativo | StructuralVariety | media 3/5 (min 3, max 3) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Ponto forte potencial do WFC, principalmente na variedade local de tiles. |
| Performance | layoutGenerationMilliseconds | media 71.234 (min 58.21, max 138.634) ms | Medido | Resumo de 30 execucoes. | Inclui colapso, propagacao e extracao de componentes; comparar separado da instanciacao Unity. |
| Performance | geometryInstantiationMilliseconds | media 0 (min 0, max 0) ms | Medido | Resumo de 30 execucoes. | Custo visual depende dos prefabs/Unity, nao do WFC puro. |
| Performance | metricsCalculationMilliseconds | media 0.968 (min 0.605, max 4.536) ms | Medido | Resumo de 30 execucoes. | Custo da instrumentacao, nao do algoritmo. |
| Performance | totalGenerationMilliseconds | media 72.203 (min 59.292, max 140.012) ms | Medido | Resumo de 30 execucoes. | Inclui layout, instanciacao visual e metricas; use junto com o tempo logico. |
| Performance | generatedGameObjectCount | media 0 (min 0, max 0) contagem | Medido | Resumo de 30 execucoes. | Reflete peso da montagem visual dos tiles colapsados. |
| Performance | occupiedCellCount | media 7917.9 (min 7800, max 7999) contagem | Medido | Resumo de 30 execucoes. | Proxy do volume ocupado pelo colapso. |
| Performance | connectionCount | media 5.7 (min 1, max 14) contagem | Medido | Resumo de 30 execucoes. | Conexoes sao inferidas apos o colapso; nao sao primitivas nativas do WFC. |
| Performance | managedMemoryDeltaKB | media -219.467 (min -97332, max 16964) KB | Estimado | Resumo de 30 execucoes. | Estimativa sujeita ao GC da Unity; use como indicio comparativo. |

## Execucoes

| Run | Seed | Hash topologico | Salas | Pavimentos | Conectores verticais | Conectividade | Fill | Caminho critico | Layout ms | Visual ms | Total ms | GameObjects | Mem delta KB |
|---:|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 2000 | AD449757 | 41 | 2 | 0 | 2.439% | 97.339% | 0 | 61.789 | 0 | 62.499 | 0 | 16204 |
| 2 | 2001 | 8075E94A | 41 | 2 | 0 | 2.439% | 96.387% | 0 | 63.526 | 0 | 64.258 | 0 | 16272 |
| 3 | 2002 | 0409C706 | 47 | 2 | 0 | 2.128% | 97.644% | 0 | 64.056 | 0 | 64.917 | 0 | 16164 |
| 4 | 2003 | 4D03B9CD | 43 | 2 | 0 | 2.326% | 96.631% | 0 | 65.234 | 0 | 65.993 | 0 | 16004 |
| 5 | 2004 | C3330D14 | 50 | 2 | 0 | 6% | 96.582% | 62 | 109.501 | 0 | 110.341 | 0 | -83924 |
| 6 | 2005 | 1D5C36F8 | 49 | 2 | 0 | 4.082% | 96.68% | 14 | 64.81 | 0 | 65.726 | 0 | 6088 |
| 7 | 2006 | 005077BE | 46 | 2 | 0 | 2.174% | 96.46% | 0 | 63.186 | 0 | 64.01 | 0 | 10976 |
| 8 | 2007 | 28F01ED3 | 39 | 2 | 0 | 2.564% | 96.338% | 0 | 63.869 | 0 | 64.656 | 0 | 13696 |
| 9 | 2008 | C8B19DCD | 41 | 2 | 0 | 2.439% | 96.313% | 0 | 67.25 | 0 | 68.088 | 0 | 15764 |
| 10 | 2009 | 5998891E | 40 | 2 | 0 | 2.5% | 96.741% | 0 | 67.733 | 0 | 68.46 | 0 | 15792 |
| 11 | 2010 | DEEDE1F5 | 50 | 2 | 0 | 8% | 96.228% | 34 | 69.602 | 0 | 70.53 | 0 | 16940 |
| 12 | 2011 | 8ED1D2ED | 46 | 2 | 0 | 2.174% | 96.704% | 0 | 67.018 | 0 | 68.492 | 0 | 16536 |
| 13 | 2012 | 6B05695C | 33 | 2 | 0 | 3.03% | 96.338% | 0 | 107.479 | 0 | 108.184 | 0 | -93512 |
| 14 | 2013 | B1512C89 | 42 | 2 | 0 | 2.381% | 96.265% | 0 | 68.093 | 0 | 68.848 | 0 | 5716 |
| 15 | 2014 | 817CEF4F | 45 | 2 | 0 | 6.667% | 96.375% | 24 | 62.312 | 0 | 63.047 | 0 | 12328 |
| 16 | 2015 | 63748D78 | 41 | 2 | 0 | 2.439% | 97.644% | 0 | 61.989 | 0 | 62.594 | 0 | 13716 |
| 17 | 2016 | A8F968F1 | 45 | 2 | 0 | 2.222% | 96.985% | 0 | 64.914 | 0 | 65.831 | 0 | 16268 |
| 18 | 2017 | 9FDE84EE | 47 | 2 | 1 | 2.128% | 97.156% | 0 | 59.175 | 0 | 59.947 | 0 | 15988 |
| 19 | 2018 | 66DD4175 | 47 | 2 | 0 | 2.128% | 95.74% | 0 | 58.627 | 0 | 59.292 | 0 | 16116 |
| 20 | 2019 | 369E40D0 | 41 | 2 | 0 | 2.439% | 96.997% | 0 | 58.21 | 0 | 62.748 | 0 | 16368 |
| 21 | 2020 | 66C89E16 | 42 | 2 | 0 | 2.381% | 95.215% | 0 | 87.277 | 0 | 88.046 | 0 | -95076 |
| 22 | 2021 | 0FFAED1A | 36 | 2 | 0 | 2.778% | 96.228% | 0 | 60.655 | 0 | 61.345 | 0 | 5548 |
| 23 | 2022 | B0B7507B | 45 | 2 | 0 | 4.444% | 96.887% | 76 | 59.407 | 0 | 60.192 | 0 | 13676 |
| 24 | 2023 | B39E4532 | 42 | 2 | 0 | 7.143% | 96.655% | 12 | 59.279 | 0 | 60.037 | 0 | 13752 |
| 25 | 2024 | 4C3E4583 | 46 | 2 | 1 | 6.522% | 96.899% | 72 | 59.925 | 0 | 60.707 | 0 | 16032 |
| 26 | 2025 | 759A0080 | 37 | 2 | 0 | 2.703% | 97.034% | 0 | 62.588 | 0 | 63.216 | 0 | 16592 |
| 27 | 2026 | 7C3A4C6C | 36 | 2 | 0 | 2.778% | 96.973% | 0 | 58.709 | 0 | 59.342 | 0 | 16024 |
| 28 | 2027 | BA6E3743 | 37 | 2 | 0 | 2.703% | 96.765% | 0 | 95.619 | 0 | 97.361 | 0 | 16964 |
| 29 | 2028 | FFC8DBE3 | 46 | 2 | 0 | 2.174% | 96.802% | 0 | 138.634 | 0 | 140.012 | 0 | -97332 |
| 30 | 2029 | 95F53982 | 48 | 2 | 0 | 4.167% | 96.619% | 16 | 86.55 | 0 | 87.386 | 0 | 7736 |

## Como usar no texto da dissertacao

Use os parametros com status `Medido` como resultados quantitativos diretos. Parametros com status `Estimado sem NavMesh` ou `Estimado automaticamente` devem ser descritos como proxies instrumentais. Parametros com status `Nao implementado nesta versao` devem aparecer como limitacoes da variante do algoritmo testada, nao necessariamente como impossibilidade teorica do algoritmo.
