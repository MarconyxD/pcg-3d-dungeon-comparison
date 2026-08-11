# Relatorio de Teste PCG - Wave Function Collapse

Gerado em UTC: 2026-05-19T18:48:15.6237331Z

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
| Quantitativo | uniqueModules | media 48.1 (min 44, max 49) contagem | Medido | Resumo de 30 execucoes. | WFC tende a ser forte em variedade local quando ha um tileset rico. |
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
| Performance | layoutGenerationMilliseconds | media 63.71 (min 57.245, max 83.893) ms | Medido | Resumo de 30 execucoes. | Inclui colapso, propagacao e extracao de componentes; comparar separado da instanciacao Unity. |
| Performance | geometryInstantiationMilliseconds | media 131.137 (min 123.053, max 156.231) ms | Medido | Resumo de 30 execucoes. | Custo visual depende dos prefabs/Unity, nao do WFC puro. |
| Performance | metricsCalculationMilliseconds | media 0.773 (min 0.628, max 0.94) ms | Medido | Resumo de 30 execucoes. | Custo da instrumentacao, nao do algoritmo. |
| Performance | totalGenerationMilliseconds | media 195.624 (min 183.814, max 226.393) ms | Medido | Resumo de 30 execucoes. | Inclui layout, instanciacao visual e metricas; use junto com o tempo logico. |
| Performance | generatedGameObjectCount | media 12407.7 (min 12141, max 12578) contagem | Medido | Resumo de 30 execucoes. | Reflete peso da montagem visual dos tiles colapsados. |
| Performance | occupiedCellCount | media 7917.9 (min 7800, max 7999) contagem | Medido | Resumo de 30 execucoes. | Proxy do volume ocupado pelo colapso. |
| Performance | connectionCount | media 5.7 (min 1, max 14) contagem | Medido | Resumo de 30 execucoes. | Conexoes sao inferidas apos o colapso; nao sao primitivas nativas do WFC. |
| Performance | managedMemoryDeltaKB | media 2974.133 (min -84440, max 31656) KB | Estimado | Resumo de 30 execucoes. | Estimativa sujeita ao GC da Unity; use como indicio comparativo. |

## Execucoes

| Run | Seed | Hash topologico | Salas | Pavimentos | Conectores verticais | Conectividade | Fill | Caminho critico | Layout ms | Visual ms | Total ms | GameObjects | Mem delta KB |
|---:|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 2000 | AD449757 | 41 | 2 | 0 | 2.439% | 97.339% | 0 | 61.505 | 130.186 | 192.489 | 12432 | 27228 |
| 2 | 2001 | 8075E94A | 41 | 2 | 0 | 2.439% | 96.387% | 0 | 60.133 | 124.559 | 185.366 | 12436 | 30388 |
| 3 | 2002 | 0409C706 | 47 | 2 | 0 | 2.128% | 97.644% | 0 | 57.681 | 125.406 | 183.814 | 12533 | 30744 |
| 4 | 2003 | 4D03B9CD | 43 | 2 | 0 | 2.326% | 96.631% | 0 | 83.447 | 127.481 | 211.686 | 12481 | -82708 |
| 5 | 2004 | C3330D14 | 50 | 2 | 0 | 6% | 96.582% | 62 | 59.446 | 123.915 | 184.229 | 12342 | 26992 |
| 6 | 2005 | 1D5C36F8 | 49 | 2 | 0 | 4.082% | 96.68% | 14 | 61.618 | 123.053 | 185.477 | 12249 | 31656 |
| 7 | 2006 | 005077BE | 46 | 2 | 0 | 2.174% | 96.46% | 0 | 58.692 | 132.782 | 192.319 | 12392 | 30892 |
| 8 | 2007 | 28F01ED3 | 39 | 2 | 0 | 2.564% | 96.338% | 0 | 83.893 | 127.26 | 211.993 | 12432 | -84440 |
| 9 | 2008 | C8B19DCD | 41 | 2 | 0 | 2.439% | 96.313% | 0 | 58.402 | 127.385 | 186.588 | 12404 | 26088 |
| 10 | 2009 | 5998891E | 40 | 2 | 0 | 2.5% | 96.741% | 0 | 57.771 | 128.838 | 187.272 | 12564 | 30252 |
| 11 | 2010 | DEEDE1F5 | 50 | 2 | 0 | 8% | 96.228% | 34 | 60.239 | 123.18 | 184.296 | 12222 | 31116 |
| 12 | 2011 | 8ED1D2ED | 46 | 2 | 0 | 2.174% | 96.704% | 0 | 80.812 | 125.519 | 207.102 | 12415 | -83740 |
| 13 | 2012 | 6B05695C | 33 | 2 | 0 | 3.03% | 96.338% | 0 | 58.634 | 125.483 | 184.748 | 12370 | 26188 |
| 14 | 2013 | B1512C89 | 42 | 2 | 0 | 2.381% | 96.265% | 0 | 63.348 | 146.049 | 210.127 | 12471 | 30368 |
| 15 | 2014 | 817CEF4F | 45 | 2 | 0 | 6.667% | 96.375% | 24 | 62.131 | 133.158 | 196.057 | 12376 | 30800 |
| 16 | 2015 | 63748D78 | 41 | 2 | 0 | 2.439% | 97.644% | 0 | 79.483 | 146.188 | 226.393 | 12578 | -83984 |
| 17 | 2016 | A8F968F1 | 45 | 2 | 0 | 2.222% | 96.985% | 0 | 65.33 | 130.308 | 196.478 | 12289 | 26620 |
| 18 | 2017 | 9FDE84EE | 47 | 2 | 1 | 2.128% | 97.156% | 0 | 58.766 | 127.088 | 186.67 | 12518 | 30444 |
| 19 | 2018 | 66DD4175 | 47 | 2 | 0 | 2.128% | 95.74% | 0 | 57.843 | 132.091 | 190.864 | 12357 | 30316 |
| 20 | 2019 | 369E40D0 | 41 | 2 | 0 | 2.439% | 96.997% | 0 | 77.319 | 127.139 | 205.091 | 12517 | -84416 |
| 21 | 2020 | 66C89E16 | 42 | 2 | 0 | 2.381% | 95.215% | 0 | 59.631 | 124.054 | 184.491 | 12141 | 26520 |
| 22 | 2021 | 0FFAED1A | 36 | 2 | 0 | 2.778% | 96.228% | 0 | 57.245 | 128.927 | 186.826 | 12544 | 29972 |
| 23 | 2022 | B0B7507B | 45 | 2 | 0 | 4.444% | 96.887% | 76 | 60.026 | 155.834 | 216.684 | 12334 | -71556 |
| 24 | 2023 | B39E4532 | 42 | 2 | 0 | 7.143% | 96.655% | 12 | 66.308 | 129.671 | 196.782 | 12361 | 20508 |
| 25 | 2024 | 4C3E4583 | 46 | 2 | 1 | 6.522% | 96.899% | 72 | 60.256 | 126.132 | 187.24 | 12317 | 25884 |
| 26 | 2025 | 759A0080 | 37 | 2 | 0 | 2.703% | 97.034% | 0 | 59.035 | 130.612 | 190.321 | 12463 | 30000 |
| 27 | 2026 | 7C3A4C6C | 36 | 2 | 0 | 2.778% | 96.973% | 0 | 59.523 | 156.231 | 216.515 | 12542 | -72780 |
| 28 | 2027 | BA6E3743 | 37 | 2 | 0 | 2.703% | 96.765% | 0 | 61.764 | 144.311 | 206.776 | 12375 | 24740 |
| 29 | 2028 | FFC8DBE3 | 46 | 2 | 0 | 2.174% | 96.802% | 0 | 61.544 | 126.12 | 188.606 | 12368 | 25884 |
| 30 | 2029 | 95F53982 | 48 | 2 | 0 | 4.167% | 96.619% | 16 | 59.486 | 125.144 | 185.43 | 12408 | 29248 |

## Como usar no texto da dissertacao

Use os parametros com status `Medido` como resultados quantitativos diretos. Parametros com status `Estimado sem NavMesh` ou `Estimado automaticamente` devem ser descritos como proxies instrumentais. Parametros com status `Nao implementado nesta versao` devem aparecer como limitacoes da variante do algoritmo testada, nao necessariamente como impossibilidade teorica do algoritmo.
