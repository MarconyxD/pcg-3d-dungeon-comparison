# Relatorio de Teste PCG - Room Graph

Gerado em UTC: 2026-05-19T18:39:10.8238090Z

Teste Room Graph executado com 30 seed(s). Topologias unicas: 30/30. Diversidade topologica: 100%. Reprodutibilidade por seed: aprovada.

## Resumo agregado

| Categoria | Parametro | Valor | Status | Interpretacao | Observacao do algoritmo |
|---|---|---:|---|---|---|
| Quantitativo | numRoomsTarget | media 46.167 (min 42, max 48) contagem | Medido | Indica quantos nos do grafo foram efetivamente posicionados como salas. | Room Graph controla salas como nos do grafo; a etapa de embedding pode gerar menos salas se nao houver espaco sem sobreposicao. |
| Quantitativo | connectivityRatio | media 100 (min 100, max 100) % | Medido | Mostra se as salas ficaram conectadas no componente principal do grafo. | Room Graph e forte neste parametro: a arvore de conexoes garante componente principal conectado quando ha pelo menos duas salas. |
| Quantitativo | verticalVariance | media 1.999 (min 1.986, max 2) metros | Medido | Valor acima de 0 indica variacao vertical gerada por pavimentos e arestas verticais. | Room Graph puro suporta verticalidade ao criar arestas verticais entre nos de pavimentos adjacentes. |
| Quantitativo | fillPercentage | media 52.422 (min 48.694, max 55.933) % | Medido | Resumo de 30 execucoes. | A densidade depende do numero de nos, tamanhos de sala e sucesso do posicionamento espacial do grafo. |
| Quantitativo | branchFactor | media 2.043 (min 2.042, max 2.048) media | Medido | Valores maiores indicam mais bifurcacoes e loops no grafo. | O fator de ramificacao e diretamente controlado por arvore inicial e arestas extras de loop. |
| Quantitativo | avgPathLength | media 134.036 (min 76.468, max 196.304) metros | Medido | Resumo de 30 execucoes. | Caminhos sao naturais para Room Graph, pois o algoritmo ja trabalha com grafo explicito. |
| Quantitativo | uniqueModules | media 26.233 (min 24, max 27) contagem | Medido | Resumo de 30 execucoes. | A variedade vem das categorias logicas do grafo, conectores, corredores e assets configurados. |
| Quantitativo | navigableVolumeRatio | media 100 (min 100, max 100) % | Estimado sem NavMesh | Resumo de 30 execucoes. | A navegabilidade e estimada no grid logico; validacao fisica final ainda exige NavMesh. |
| Quantitativo | criticalPathLength | media 251.533 (min 142, max 360) metros | Medido | Aproxima o percurso principal entre inicio e ponto mais distante no grafo. | Room Graph mede bem percurso principal porque distancias no grafo sao parte central do modelo. |
| Quantitativo | avgAlternativePathLength | media 32.567 (min 11, max 45) metros | Medido | Resumo de 30 execucoes. | Loops e atalhos sao uma vantagem direta do Room Graph, bastando adicionar arestas extras. |
| Booleano | SupportsRandomEnemySpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Room Graph oferece salas/nos claros para spawns; balanceamento de combate e camada de gameplay. |
| Booleano | SupportsLootDistribution | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Loot pode ser distribuido por nos/salas ou por distancia no grafo. |
| Booleano | SupportsTraps | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Armadilhas podem ser colocadas em salas, corredores ou arestas especificas do grafo. |
| Booleano | SupportsBacktrackingLoops | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Suporte nativo quando o grafo recebe arestas extras alem da arvore de conectividade. |
| Booleano | SupportsVerticalConnectors | Sim em 30/30 execucoes sim/nao | Medido | Parametro atendido quando existem arestas verticais Room Graph entre pavimentos. | Implementado como arestas verticais do proprio Room Graph, sem usar BSP ou outro algoritmo auxiliar. |
| Booleano | SupportsMultiFloor | Sim em 30/30 execucoes sim/nao | Medido | Parametro atendido quando ha pavimentos conectados dentro do mesmo grafo. | Implementado por nos distribuidos em pavimentos e arestas verticais entre camadas. |
| Booleano | SupportsBossArena | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Suportado se algum no/sala for grande o suficiente; pode exigir parametros de tamanho ou uma regra de no especial. |
| Booleano | SeedReproducible | Sim sim/nao | Medido | Parametro atendido. | O Room Graph e deterministico quando todas as escolhas usam a mesma seed. |
| Booleano | RuntimeRegeneration | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Room Graph tende a ser rapido; a etapa mais sensivel e o posicionamento sem sobreposicao das salas. |
| Booleano | BudgetAwareSpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Orcamentos sao controlados por camada de spawn sobre os nos/salas gerados. |
| Qualitativo | Replayability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | A variacao entre seeds depende da combinacao entre grafo, embedding espacial e arestas extras. |
| Qualitativo | Debuggability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | A estrutura de nos e arestas facilita inspecao, depuracao e reproducao por seed. | Room Graph e facil de depurar porque a topologia pode ser inspecionada como nos e arestas. |
| Qualitativo | Flow | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | O controle de fluxo e uma das maiores vantagens do Room Graph, pois caminhos e ramificacoes sao explicitos. |
| Qualitativo | Legibility | media 4/5 (min 4, max 4) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | A legibilidade depende do embedding: o grafo e claro, mas corredores podem cruzar se o layout espacial ficar apertado. |
| Qualitativo | StructuralVariety | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | A variedade estrutural e boa em macrofluxo; variedade local ainda depende de regras de decoracao/assets. |
| Performance | layoutGenerationMilliseconds | media 1.54 (min 1.013, max 3.014) ms | Medido | Representa o custo algoritmico principal do Room Graph. | Mede custo de criar nos, posiciona-los e conectar o grafo; compare separado da instanciacao Unity. |
| Performance | geometryInstantiationMilliseconds | media 60.128 (min 48.873, max 87.217) ms | Medido | Resumo de 30 execucoes. | Custo visual depende dos prefabs/Unity, nao do Room Graph puro. |
| Performance | metricsCalculationMilliseconds | media 3.959 (min 1.694, max 23.17) ms | Medido | Resumo de 30 execucoes. | Custo da instrumentacao, nao do algoritmo. |
| Performance | totalGenerationMilliseconds | media 65.63 (min 52.069, max 112.881) ms | Medido | Resumo de 30 execucoes. | Inclui layout, instanciacao visual e metricas; use junto com o tempo logico. |
| Performance | generatedGameObjectCount | media 5977.533 (min 5691, max 6281) contagem | Medido | Resumo de 30 execucoes. | Reflete peso da montagem visual da cena gerada pelo grafo. |
| Performance | occupiedCellCount | media 4294.4 (min 3989, max 4582) contagem | Medido | Resumo de 30 execucoes. | Proxy de escala espacial resultante do embedding dos nos e corredores. |
| Performance | connectionCount | media 47.167 (min 43, max 49) contagem | Medido | Reflete diretamente a complexidade topologica do grafo. | Metrica muito natural para Room Graph, pois corresponde diretamente ao numero de arestas. |
| Performance | managedMemoryDeltaKB | media -3674.4 (min -215748, max 4560) KB | Estimado | Resumo de 30 execucoes. | Estimativa sujeita ao GC da Unity; use como indicio comparativo. |

## Execucoes

| Run | Seed | Hash topologico | Salas | Pavimentos | Conectores verticais | Conectividade | Fill | Caminho critico | Layout ms | Visual ms | Total ms | GameObjects | Mem delta KB |
|---:|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 2000 | E13ADCFD | 43 | 2 | 1 | 100% | 51.709% | 338 | 1.04 | 57.332 | 60.314 | 5850 | 3692 |
| 2 | 2001 | B440E37B | 45 | 2 | 1 | 100% | 49.67% | 276 | 1.153 | 52.175 | 55.269 | 5728 | 3572 |
| 3 | 2002 | 258AD17C | 48 | 2 | 1 | 100% | 51.538% | 148 | 1.176 | 52.569 | 55.784 | 5999 | 3960 |
| 4 | 2003 | 89D1B882 | 46 | 2 | 1 | 100% | 53.088% | 218 | 1.153 | 54.341 | 57.67 | 6014 | 4364 |
| 5 | 2004 | 08E96317 | 48 | 2 | 1 | 100% | 53.613% | 214 | 1.212 | 54.306 | 57.727 | 6094 | 4432 |
| 6 | 2005 | 6B27D7DE | 47 | 2 | 1 | 100% | 51.209% | 200 | 1.821 | 53.828 | 57.511 | 5849 | 4192 |
| 7 | 2006 | 6087AACA | 47 | 2 | 1 | 100% | 51.758% | 334 | 1.12 | 48.873 | 52.069 | 5924 | 4296 |
| 8 | 2007 | D6B4C89C | 47 | 2 | 1 | 100% | 51.526% | 272 | 1.16 | 56.265 | 59.834 | 5901 | 4280 |
| 9 | 2008 | 80993A22 | 45 | 2 | 1 | 100% | 55.933% | 178 | 1.268 | 54.87 | 59.828 | 6281 | 4560 |
| 10 | 2009 | 49706998 | 47 | 2 | 1 | 100% | 54.773% | 328 | 1.128 | 66.755 | 74.855 | 6201 | 4512 |
| 11 | 2010 | 7443C078 | 42 | 2 | 1 | 100% | 55.554% | 142 | 2.432 | 75.556 | 85.022 | 6251 | 4528 |
| 12 | 2011 | 528E1BFE | 46 | 2 | 1 | 100% | 51.526% | 192 | 3.014 | 75.369 | 85.628 | 5969 | 4284 |
| 13 | 2012 | B8409E72 | 45 | 2 | 1 | 100% | 52.075% | 360 | 2.35 | 75.38 | 85.146 | 5909 | 4300 |
| 14 | 2013 | F838A5E2 | 47 | 2 | 1 | 100% | 54.382% | 338 | 2.7 | 74.046 | 84.029 | 6170 | 4480 |
| 15 | 2014 | B5824A82 | 46 | 2 | 1 | 100% | 52.612% | 322 | 2.613 | 73.963 | 83.7 | 5954 | 4324 |
| 16 | 2015 | 105EFFE6 | 45 | 2 | 1 | 100% | 50.818% | 306 | 2.702 | 73.658 | 83.015 | 5828 | 4216 |
| 17 | 2016 | C0C6C1AE | 47 | 2 | 1 | 100% | 54.15% | 286 | 2.49 | 87.217 | 112.881 | 6185 | -215748 |
| 18 | 2017 | 83B00461 | 47 | 2 | 1 | 100% | 51.282% | 258 | 1.656 | 56.262 | 60.079 | 5855 | 56 |
| 19 | 2018 | 821E844A | 45 | 2 | 1 | 100% | 52.332% | 184 | 1.441 | 54.474 | 57.987 | 5873 | 1324 |
| 20 | 2019 | 3984DB4C | 46 | 2 | 1 | 100% | 53.589% | 236 | 1.186 | 55.447 | 58.736 | 6045 | 2612 |
| 21 | 2020 | 3EDF9924 | 47 | 2 | 1 | 100% | 51.428% | 256 | 1.168 | 57.108 | 60.306 | 5921 | 2540 |
| 22 | 2021 | 6CA9F9DE | 46 | 2 | 1 | 100% | 52.185% | 172 | 1.174 | 58.011 | 61.223 | 5960 | 2776 |
| 23 | 2022 | DA5B5F56 | 46 | 2 | 1 | 100% | 52.197% | 178 | 1.23 | 55.607 | 58.986 | 5996 | 3476 |
| 24 | 2023 | 5BBEC674 | 48 | 2 | 1 | 100% | 53.918% | 212 | 1.226 | 54.876 | 58.091 | 6141 | 3568 |
| 25 | 2024 | A3B541B9 | 46 | 2 | 1 | 100% | 51.453% | 262 | 1.013 | 59.54 | 62.372 | 5869 | 3420 |
| 26 | 2025 | 1E5F4323 | 47 | 2 | 1 | 100% | 51.355% | 246 | 1.082 | 54.176 | 57.083 | 5882 | 3416 |
| 27 | 2026 | E866F0C1 | 47 | 2 | 1 | 100% | 48.694% | 298 | 1.028 | 51.343 | 54.067 | 5691 | 3268 |
| 28 | 2027 | 4A86B6A8 | 46 | 2 | 1 | 100% | 51.892% | 288 | 1.07 | 49.3 | 52.26 | 5879 | 3480 |
| 29 | 2028 | 71CD5570 | 45 | 2 | 1 | 100% | 55.017% | 196 | 1.352 | 60.519 | 63.879 | 6183 | 3888 |
| 30 | 2029 | 3A54F7C7 | 48 | 2 | 1 | 100% | 51.379% | 308 | 1.043 | 50.668 | 53.537 | 5924 | 3700 |

## Como usar no texto da dissertacao

Use os parametros com status `Medido` como resultados quantitativos diretos. Parametros com status `Estimado sem NavMesh` ou `Estimado automaticamente` devem ser descritos como proxies instrumentais. Parametros com status `Nao implementado nesta versao` devem aparecer como limitacoes da variante do algoritmo testada, nao necessariamente como impossibilidade teorica do algoritmo.
