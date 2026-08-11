# Relatorio de Teste PCG - Grammar-Based Generation

Gerado em UTC: 2026-05-03T05:27:17.7824991Z

Teste Grammar-Based Generation executado com 30 seed(s). Topologias unicas: 30/30. Diversidade topologica: 100%. Reprodutibilidade por seed: aprovada.

## Resumo agregado

| Categoria | Parametro | Valor | Status | Interpretacao | Observacao do algoritmo |
|---|---|---:|---|---|---|
| Quantitativo | numRoomsTarget | media 27.1 (min 20, max 32) contagem | Medido | Indica quantos simbolos terminais da gramatica viraram salas no grid. | A gramatica controla quantidade por regras de caminho principal, ramificacoes e terminais; a interpretacao espacial pode descartar simbolos se nao houver espaco. |
| Quantitativo | connectivityRatio | media 97.159 (min 87.5, max 100) % | Medido | Mostra se a derivacao sequencial e suas ramificacoes permaneceram conectadas. | A sequencia derivada tende a ser conectada porque cada simbolo nasce de um simbolo anterior; falhas indicam limitacao de embedding espacial. |
| Quantitativo | verticalVariance | media 1.975 (min 1.901, max 2) metros | Medido | Valor acima de 0 indica que regras verticais geraram pavimentos conectados. | Verticalidade e suportada quando a gramatica inclui regras entre pavimentos. |
| Quantitativo | fillPercentage | media 43.337 (min 33.24, max 51.587) % | Medido | Resumo de 30 execucoes. | A densidade depende do numero de simbolos terminais, tamanhos das salas e comprimento dos corredores. |
| Quantitativo | branchFactor | media 1.978 (min 1.75, max 2.16) media | Medido | Valores maiores indicam mais derivacoes laterais, atalhos e variacao de fluxo. | Ramificacao e controlada por regras laterais e por regras de loop. |
| Quantitativo | avgPathLength | media 207.859 (min 118.091, max 356.148) metros | Medido | Resumo de 30 execucoes. | Caminhos sao consequencia da ordem de derivacao, nao de busca posterior ou reparo. |
| Quantitativo | uniqueModules | media 42.833 (min 39, max 45) contagem | Medido | Resumo de 30 execucoes. | A variedade vem dos simbolos de entrada, sala, ramificacao, tesouro, armadilha, arena, saida e dos assets configurados. |
| Quantitativo | navigableVolumeRatio | media 100 (min 100, max 100) % | Estimado sem NavMesh | Resumo de 30 execucoes. | A navegabilidade e estimada no grid logico; validacao fisica final ainda exige NavMesh. |
| Quantitativo | criticalPathLength | media 409.533 (min 260, max 630) metros | Medido | Aproxima o percurso entre o simbolo inicial e o terminal mais distante. | A gramatica pode controlar um caminho principal longo por regras sequenciais. |
| Quantitativo | avgAlternativePathLength | media 20.456 (min 0, max 45) metros | Medido | Resumo de 30 execucoes. | Loops sao suportados apenas quando a gramatica inclui regras de retorno/atalho. |
| Booleano | SupportsRandomEnemySpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Spawns usam salas/celulas derivadas; balanceamento de combate ainda e camada de gameplay. |
| Booleano | SupportsLootDistribution | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Loot pode usar simbolos Treasure ou salas derivadas, mas progressao economica exige regra complementar. |
| Booleano | SupportsTraps | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Armadilhas podem usar simbolos Trap ou gargalos, mas pacing de desafio exige camada semantica. |
| Booleano | SupportsBacktrackingLoops | Sim em 15/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Suporte nativo quando a gramatica inclui producoes de loop; nao precisa misturar outro algoritmo. |
| Booleano | SupportsVerticalConnectors | Sim em 30/30 execucoes sim/nao | Medido | Parametro atendido quando regras verticais da propria gramatica foram materializadas. | Implementado por regras verticais da propria gramatica, sem BSP, Room Graph, WFC ou outro metodo. |
| Booleano | SupportsMultiFloor | Sim em 30/30 execucoes sim/nao | Medido | Parametro atendido quando a gramatica derivou pavimentos conectados. | Implementado por simbolos em pavimentos diferentes e producoes verticais entre eles. |
| Booleano | SupportsBossArena | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Suportado por uma regra terminal especifica de BossArena, com tamanho configuravel. |
| Booleano | SeedReproducible | Sim sim/nao | Medido | Parametro atendido. | O Grammar-Based Generation e deterministico quando todas as escolhas usam a mesma seed. |
| Booleano | RuntimeRegeneration | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | O custo cresce com quantidade de simbolos, tentativas de embedding e numero de pavimentos. |
| Booleano | BudgetAwareSpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Orcamentos sao aplicados apos a derivacao, sobre celulas livres. |
| Qualitativo | Replayability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | A variacao entre seeds depende da escolha das producoes e da interpretacao espacial. |
| Qualitativo | Debuggability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | A derivacao por regras facilita explicar por que cada sala existe. | A gramatica e facil de documentar porque cada resultado pode ser explicado por uma sequencia de regras. |
| Qualitativo | Flow | media 4/5 (min 3, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | O controle de fluxo e uma vantagem forte, pois caminho principal, ramificacoes e loops sao regras explicitas. |
| Qualitativo | Legibility | media 4.033/5 (min 3, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | A legibilidade depende da interpretacao espacial: regras claras podem gerar mapas confusos se o embedding ficar apertado. |
| Qualitativo | StructuralVariety | media 4.5/5 (min 4, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | A variedade estrutural depende do repertorio de producoes; aumentar a gramatica aumenta expressividade. |
| Performance | layoutGenerationMilliseconds | media 0.784 (min 0.473, max 2.67) ms | Medido | Representa o custo da expansao e interpretacao da gramatica. | Mede custo de expandir regras, posicionar simbolos e abrir corredores; compare separado da instanciacao Unity. |
| Performance | geometryInstantiationMilliseconds | media 60.177 (min 41.359, max 87.386) ms | Medido | Resumo de 30 execucoes. | Custo visual depende dos prefabs/Unity, nao do Grammar-Based Generation puro. |
| Performance | metricsCalculationMilliseconds | media 1.926 (min 1.313, max 3.509) ms | Medido | Resumo de 30 execucoes. | Custo da instrumentacao, nao do algoritmo. |
| Performance | totalGenerationMilliseconds | media 62.891 (min 43.152, max 90.83) ms | Medido | Resumo de 30 execucoes. | Inclui layout, instanciacao visual e metricas; use junto com o tempo logico. |
| Performance | generatedGameObjectCount | media 4798.1 (min 3667, max 5694) contagem | Medido | Resumo de 30 execucoes. | Reflete peso da montagem visual da cena derivada. |
| Performance | occupiedCellCount | media 3550.2 (min 2723, max 4226) contagem | Medido | Resumo de 30 execucoes. | Proxy de escala espacial resultante dos simbolos e corredores materializados. |
| Performance | connectionCount | media 26.767 (min 21, max 31) contagem | Medido | Reflete a complexidade topologica resultante das regras de producao. | Metrica natural para a gramatica, pois corresponde ao numero de relacoes materializadas entre simbolos. |
| Performance | managedMemoryDeltaKB | media 2951.467 (min 484, max 4132) KB | Estimado | Resumo de 30 execucoes. | Estimativa sujeita ao GC da Unity; use como indicio comparativo. |

## Execucoes

| Run | Seed | Hash topologico | Salas | Pavimentos | Conectores verticais | Conectividade | Fill | Caminho critico | Layout ms | Visual ms | Total ms | GameObjects | Mem delta KB |
|---:|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 2000 | 8DD730AB | 27 | 2 | 1 | 96.296% | 45.496% | 388 | 0.54 | 87.386 | 90.83 | 5000 | 484 |
| 2 | 2001 | 9043074E | 25 | 2 | 1 | 100% | 45.764% | 442 | 1.008 | 75.197 | 78.21 | 4991 | 1160 |
| 3 | 2002 | 74953930 | 32 | 2 | 1 | 93.75% | 48.303% | 410 | 2.67 | 80.415 | 86.598 | 5393 | 1148 |
| 4 | 2003 | 244BD791 | 20 | 2 | 1 | 100% | 35.779% | 330 | 0.553 | 52.204 | 54.321 | 3906 | 876 |
| 5 | 2004 | 4D7DBB81 | 23 | 2 | 1 | 91.304% | 39.856% | 388 | 0.573 | 62.7 | 64.781 | 4449 | 1356 |
| 6 | 2005 | BEA32F90 | 27 | 2 | 1 | 96.296% | 40.417% | 332 | 0.607 | 53.241 | 55.539 | 4467 | 1608 |
| 7 | 2006 | 608FECB5 | 26 | 2 | 1 | 92.308% | 42.175% | 490 | 0.554 | 53.61 | 55.863 | 4687 | 1992 |
| 8 | 2007 | A6FF72C5 | 25 | 2 | 1 | 100% | 38.635% | 370 | 0.565 | 49.463 | 51.511 | 4360 | 2120 |
| 9 | 2008 | BD69CF32 | 30 | 2 | 1 | 100% | 50.146% | 522 | 0.857 | 66.424 | 69.439 | 5387 | 4060 |
| 10 | 2009 | 376A255C | 27 | 2 | 1 | 100% | 44.275% | 360 | 0.65 | 57.036 | 59.406 | 4910 | 3548 |
| 11 | 2010 | 0C238E0C | 28 | 2 | 1 | 100% | 44.116% | 540 | 0.629 | 57.514 | 59.782 | 4924 | 3624 |
| 12 | 2011 | 6E52D178 | 29 | 2 | 1 | 100% | 44.385% | 368 | 0.676 | 51.503 | 54.105 | 4872 | 3568 |
| 13 | 2012 | 232D74F7 | 28 | 2 | 1 | 96.429% | 46.216% | 314 | 0.633 | 55.333 | 57.915 | 5076 | 3724 |
| 14 | 2013 | 0A8F9487 | 23 | 2 | 1 | 100% | 33.24% | 260 | 0.473 | 41.359 | 43.152 | 3667 | 2696 |
| 15 | 2014 | 7A6F0E26 | 32 | 2 | 1 | 87.5% | 51.587% | 630 | 0.835 | 61.993 | 64.933 | 5694 | 4132 |
| 16 | 2015 | 452D61ED | 27 | 2 | 1 | 100% | 38.464% | 326 | 0.56 | 51.318 | 53.449 | 4362 | 3168 |
| 17 | 2016 | C7143CFE | 27 | 2 | 1 | 96.296% | 47.131% | 518 | 0.778 | 57.105 | 59.761 | 5217 | 3824 |
| 18 | 2017 | 01CF1927 | 25 | 2 | 1 | 100% | 35.95% | 450 | 0.559 | 56.539 | 59.593 | 3996 | 2908 |
| 19 | 2018 | 3C215FB6 | 28 | 2 | 1 | 100% | 47.437% | 358 | 0.749 | 57.928 | 60.418 | 5240 | 3788 |
| 20 | 2019 | 695EA350 | 27 | 2 | 1 | 100% | 38.184% | 432 | 0.835 | 47.269 | 49.471 | 4390 | 3108 |
| 21 | 2020 | 5A5BECCF | 24 | 2 | 1 | 100% | 37.878% | 334 | 0.663 | 47.283 | 49.666 | 4159 | 3088 |
| 22 | 2021 | 8818AC49 | 29 | 2 | 1 | 100% | 47.192% | 410 | 0.693 | 57.942 | 60.87 | 5146 | 3776 |
| 23 | 2022 | 48C9E73F | 29 | 2 | 1 | 93.103% | 46.838% | 490 | 0.763 | 61.237 | 63.839 | 5179 | 3732 |
| 24 | 2023 | DF9DE621 | 28 | 2 | 1 | 96.429% | 48.633% | 536 | 0.746 | 61.932 | 64.736 | 5338 | 3872 |
| 25 | 2024 | FC2B611E | 29 | 2 | 1 | 100% | 41.785% | 346 | 0.814 | 57.042 | 59.522 | 4653 | 3380 |
| 26 | 2025 | 346189DF | 29 | 2 | 1 | 96.552% | 46.68% | 378 | 0.786 | 66.413 | 69.501 | 5171 | 3804 |
| 27 | 2026 | BE3921AB | 31 | 2 | 1 | 90.323% | 46.375% | 390 | 0.967 | 74.118 | 76.951 | 5195 | 3748 |
| 28 | 2027 | D43E90D0 | 23 | 2 | 1 | 95.652% | 37.146% | 400 | 0.872 | 59.297 | 62.322 | 4105 | 3012 |
| 29 | 2028 | CE954DE7 | 23 | 2 | 1 | 95.652% | 40.93% | 348 | 0.605 | 67.219 | 69.563 | 4508 | 3280 |
| 30 | 2029 | ED361641 | 32 | 2 | 1 | 96.875% | 49.109% | 426 | 1.323 | 77.303 | 80.674 | 5501 | 3960 |

## Como usar no texto da dissertacao

Use os parametros com status `Medido` como resultados quantitativos diretos. Parametros com status `Estimado sem NavMesh` ou `Estimado automaticamente` devem ser descritos como proxies instrumentais. Parametros com status `Nao implementado nesta versao` devem aparecer como limitacoes da variante do algoritmo testada, nao necessariamente como impossibilidade teorica do algoritmo.
