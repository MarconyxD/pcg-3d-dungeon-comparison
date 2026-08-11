# Relatorio de Teste PCG - Room Graph

Gerado em UTC: 2026-05-02T19:06:13.1400324Z

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
| Quantitativo | uniqueModules | media 4 (min 4, max 4) contagem | Medido | Resumo de 30 execucoes. | A variedade vem das categorias logicas do grafo, conectores, corredores e assets configurados. |
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
| Performance | layoutGenerationMilliseconds | media 2.514 (min 0.987, max 12.169) ms | Medido | Representa o custo algoritmico principal do Room Graph. | Mede custo de criar nos, posiciona-los e conectar o grafo; compare separado da instanciacao Unity. |
| Performance | geometryInstantiationMilliseconds | media 0 (min 0, max 0) ms | Medido | Resumo de 30 execucoes. | Custo visual depende dos prefabs/Unity, nao do Room Graph puro. |
| Performance | metricsCalculationMilliseconds | media 6.564 (min 1.605, max 35.719) ms | Medido | Resumo de 30 execucoes. | Custo da instrumentacao, nao do algoritmo. |
| Performance | totalGenerationMilliseconds | media 9.079 (min 2.645, max 38.503) ms | Medido | Resumo de 30 execucoes. | Inclui layout, instanciacao visual e metricas; use junto com o tempo logico. |
| Performance | generatedGameObjectCount | media 0 (min 0, max 0) contagem | Medido | Resumo de 30 execucoes. | Reflete peso da montagem visual da cena gerada pelo grafo. |
| Performance | occupiedCellCount | media 4294.4 (min 3989, max 4582) contagem | Medido | Resumo de 30 execucoes. | Proxy de escala espacial resultante do embedding dos nos e corredores. |
| Performance | connectionCount | media 47.167 (min 43, max 49) contagem | Medido | Reflete diretamente a complexidade topologica do grafo. | Metrica muito natural para Room Graph, pois corresponde diretamente ao numero de arestas. |
| Performance | managedMemoryDeltaKB | media -6122.8 (min -200664, max 1080) KB | Estimado | Resumo de 30 execucoes. | Estimativa sujeita ao GC da Unity; use como indicio comparativo. |

## Execucoes

| Run | Seed | Hash topologico | Salas | Pavimentos | Conectores verticais | Conectividade | Fill | Caminho critico | Layout ms | Visual ms | Total ms | GameObjects | Mem delta KB |
|---:|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 2000 | E13ADCFD | 43 | 2 | 1 | 100% | 51.709% | 338 | 2.049 | 0 | 11.116 | 0 | 1004 |
| 2 | 2001 | B440E37B | 45 | 2 | 1 | 100% | 49.67% | 276 | 12.169 | 0 | 23.726 | 0 | 952 |
| 3 | 2002 | 258AD17C | 48 | 2 | 1 | 100% | 51.538% | 148 | 4.432 | 0 | 13.471 | 0 | 988 |
| 4 | 2003 | 89D1B882 | 46 | 2 | 1 | 100% | 53.088% | 218 | 4.442 | 0 | 16.659 | 0 | 1024 |
| 5 | 2004 | 08E96317 | 48 | 2 | 1 | 100% | 53.613% | 214 | 6.709 | 0 | 18.847 | 0 | 1056 |
| 6 | 2005 | 6B27D7DE | 47 | 2 | 1 | 100% | 51.209% | 200 | 4.515 | 0 | 15.01 | 0 | 968 |
| 7 | 2006 | 6087AACA | 47 | 2 | 1 | 100% | 51.758% | 334 | 3.7 | 0 | 12.468 | 0 | 1024 |
| 8 | 2007 | D6B4C89C | 47 | 2 | 1 | 100% | 51.526% | 272 | 2.796 | 0 | 12.022 | 0 | 1008 |
| 9 | 2008 | 80993A22 | 45 | 2 | 1 | 100% | 55.933% | 178 | 2.707 | 0 | 10.994 | 0 | 1080 |
| 10 | 2009 | 49706998 | 47 | 2 | 1 | 100% | 54.773% | 328 | 3.108 | 0 | 11.229 | 0 | 1048 |
| 11 | 2010 | 7443C078 | 42 | 2 | 1 | 100% | 55.554% | 142 | 4.03 | 0 | 14.295 | 0 | 1052 |
| 12 | 2011 | 528E1BFE | 46 | 2 | 1 | 100% | 51.526% | 192 | 2.647 | 0 | 16.631 | 0 | 980 |
| 13 | 2012 | B8409E72 | 45 | 2 | 1 | 100% | 52.075% | 360 | 2.783 | 0 | 38.503 | 0 | -200664 |
| 14 | 2013 | F838A5E2 | 47 | 2 | 1 | 100% | 54.382% | 338 | 1.257 | 0 | 4.598 | 0 | 56 |
| 15 | 2014 | B5824A82 | 46 | 2 | 1 | 100% | 52.612% | 322 | 1.218 | 0 | 3.957 | 0 | 56 |
| 16 | 2015 | 105EFFE6 | 45 | 2 | 1 | 100% | 50.818% | 306 | 1.252 | 0 | 3.711 | 0 | 56 |
| 17 | 2016 | C0C6C1AE | 47 | 2 | 1 | 100% | 54.15% | 286 | 1.209 | 0 | 4.158 | 0 | 56 |
| 18 | 2017 | 83B00461 | 47 | 2 | 1 | 100% | 51.282% | 258 | 1.33 | 0 | 3.838 | 0 | 60 |
| 19 | 2018 | 821E844A | 45 | 2 | 1 | 100% | 52.332% | 184 | 1.333 | 0 | 4.006 | 0 | 372 |
| 20 | 2019 | 3984DB4C | 46 | 2 | 1 | 100% | 53.589% | 236 | 1.284 | 0 | 4.144 | 0 | 348 |
| 21 | 2020 | 3EDF9924 | 47 | 2 | 1 | 100% | 51.428% | 256 | 1.067 | 0 | 2.817 | 0 | 332 |
| 22 | 2021 | 6CA9F9DE | 46 | 2 | 1 | 100% | 52.185% | 172 | 1.034 | 0 | 2.864 | 0 | 340 |
| 23 | 2022 | DA5B5F56 | 46 | 2 | 1 | 100% | 52.197% | 178 | 1.071 | 0 | 2.882 | 0 | 344 |
| 24 | 2023 | 5BBEC674 | 48 | 2 | 1 | 100% | 53.918% | 212 | 1.095 | 0 | 2.937 | 0 | 352 |
| 25 | 2024 | A3B541B9 | 46 | 2 | 1 | 100% | 51.453% | 262 | 0.987 | 0 | 3.279 | 0 | 336 |
| 26 | 2025 | 1E5F4323 | 47 | 2 | 1 | 100% | 51.355% | 246 | 1.004 | 0 | 2.807 | 0 | 332 |
| 27 | 2026 | E866F0C1 | 47 | 2 | 1 | 100% | 48.694% | 298 | 1.04 | 0 | 2.645 | 0 | 312 |
| 28 | 2027 | 4A86B6A8 | 46 | 2 | 1 | 100% | 51.892% | 288 | 1.058 | 0 | 2.874 | 0 | 340 |
| 29 | 2028 | 71CD5570 | 45 | 2 | 1 | 100% | 55.017% | 196 | 1.061 | 0 | 3.114 | 0 | 360 |
| 30 | 2029 | 3A54F7C7 | 48 | 2 | 1 | 100% | 51.379% | 308 | 1.026 | 0 | 2.767 | 0 | 744 |

## Como usar no texto da dissertacao

Use os parametros com status `Medido` como resultados quantitativos diretos. Parametros com status `Estimado sem NavMesh` ou `Estimado automaticamente` devem ser descritos como proxies instrumentais. Parametros com status `Nao implementado nesta versao` devem aparecer como limitacoes da variante do algoritmo testada, nao necessariamente como impossibilidade teorica do algoritmo.
