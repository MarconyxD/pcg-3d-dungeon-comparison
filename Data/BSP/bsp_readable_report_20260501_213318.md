# Relatorio de Teste PCG - Binary Space Partitioning

Gerado em UTC: 2026-05-01T21:33:16.5861873Z

Teste executado com 30 seed(s). Topologias unicas: 30/30. Diversidade topologica: 100%. Reprodutibilidade por seed: aprovada.

## Resumo agregado

| Categoria | Parametro | Valor | Status | Interpretacao | Observacao BSP |
|---|---|---:|---|---|---|
| Quantitativo | numRoomsTarget | media 45.167 (min 39, max 48) contagem | Medido | Resumo de 30 execucoes. | BSP controla esse valor por particionamento e pelo limite Max Rooms. |
| Quantitativo | connectivityRatio | media 100 (min 100, max 100) % | Medido | Resumo de 30 execucoes. | BSP deve ficar proximo de 100% porque cada particao e conectada no retorno da arvore. |
| Quantitativo | verticalVariance | media 1.996 (min 1.977, max 2) metros | Medido | Resumo de 30 execucoes. | BSP puro pode medir verticalidade quando a variante multiandar esta ativa. |
| Quantitativo | fillPercentage | media 44.528 (min 38, max 48.34) % | Medido | Resumo de 30 execucoes. | Mostra o quanto do espaco disponivel virou sala ou corredor. |
| Quantitativo | branchFactor | media 2.133 (min 2.125, max 2.154) media | Medido | Resumo de 30 execucoes. | Indica linearidade ou ramificacao da dungeon. |
| Quantitativo | avgPathLength | media 201.565 (min 116.468, max 314.727) metros | Medido | Resumo de 30 execucoes. | Aproximacao por grafo logico; pode ser refinada com NavMesh. |
| Quantitativo | uniqueModules | media 26.7 (min 25, max 27) contagem | Medido | Resumo de 30 execucoes. | No BSP, a variedade estrutural vem de salas, corredores, conectores verticais e assets configurados. |
| Quantitativo | navigableVolumeRatio | media 100 (min 100, max 100) % | Estimado sem NavMesh | Resumo de 30 execucoes. | Para medicao fisica final, integrar com NavMeshSurface e amostragem de pontos. |
| Quantitativo | criticalPathLength | media 385.4 (min 246, max 618) metros | Medido | Resumo de 30 execucoes. | Representa o percurso principal aproximado. |
| Quantitativo | avgAlternativePathLength | media 32.817 (min 21, max 48) metros | Medido | Resumo de 30 execucoes. | Se Extra Loop Connections for 0, tende a ficar 0. |
| Booleano | SupportsRandomEnemySpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | BSP aceita spawns por sala/celula sem depender do algoritmo estrutural. |
| Booleano | SupportsLootDistribution | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | BSP aceita distribuicao por sala/celula. |
| Booleano | SupportsTraps | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | BSP aceita marcadores de armadilha em areas navegaveis. |
| Booleano | SupportsBacktrackingLoops | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | BSP suporta loops quando Extra Loop Connections cria arestas extras. |
| Booleano | SupportsVerticalConnectors | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Implementado como extensao BSP pura: salas de pavimentos adjacentes sao conectadas por escadas/rampas. |
| Booleano | SupportsMultiFloor | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Implementado sem algoritmo auxiliar, usando BSP por pavimento e conectores verticais internos. |
| Booleano | SupportsBossArena | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | BSP pode gerar arenas se Max Room Size permitir salas grandes. |
| Booleano | SeedReproducible | Sim sim/nao | Medido | Parametro atendido. | BSP deterministico e adequado para testes reprodutiveis. |
| Booleano | RuntimeRegeneration | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | BSP tende a ser rapido; instanciacao visual pode custar mais que o layout logico. |
| Booleano | BudgetAwareSpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | O controle de orcamento e independente da estrutura BSP. |
| Qualitativo | Replayability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | BSP tem boa variacao quando os parametros permitem particoes diferentes. |
| Qualitativo | Debuggability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Arvore BSP facilita depuracao. |
| Qualitativo | Flow | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | BSP tende a criar fluxo claro por salas e corredores. |
| Qualitativo | Legibility | media 4.5/5 (min 4, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | BSP geralmente produz mapas legiveis por ter salas retangulares. |
| Qualitativo | StructuralVariety | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | BSP oferece variedade moderada; WFC e grammar-based podem superar em variedade local. |
| Performance | layoutGenerationMilliseconds | media 0.628 (min 0.514, max 0.878) ms | Medido | Resumo de 30 execucoes. | Esta e a metrica mais justa para comparar o custo do BSP com outros algoritmos. |
| Performance | geometryInstantiationMilliseconds | media 46.627 (min 39.349, max 65.123) ms | Medido | Resumo de 30 execucoes. | Depende muito dos assets Unity; use separado do custo algoritmico. |
| Performance | metricsCalculationMilliseconds | media 0.168 (min 0.146, max 0.264) ms | Medido | Resumo de 30 execucoes. | Ajuda a separar custo do algoritmo e custo da instrumentacao. |
| Performance | totalGenerationMilliseconds | media 47.426 (min 40.068, max 65.974) ms | Medido | Resumo de 30 execucoes. | Inclui layout, instanciacao visual habilitada e calculo de metricas. |
| Performance | generatedGameObjectCount | media 5315.067 (min 4686, max 5700) contagem | Medido | Resumo de 30 execucoes. | Quando o teste logico esta ativo, tende a 0; quando visual esta ativo, reflete o peso de instanciacao. |
| Performance | occupiedCellCount | media 3647.733 (min 3113, max 3960) contagem | Medido | Resumo de 30 execucoes. | Proxy direto do tamanho espacial processado pelo algoritmo. |
| Performance | connectionCount | media 48.167 (min 42, max 51) contagem | Medido | Resumo de 30 execucoes. | Proxy de complexidade topologica. |
| Performance | managedMemoryDeltaKB | media 1635.467 (min 1252, max 1996) KB | Estimado | Resumo de 30 execucoes. | Indicador aproximado; valores podem variar por coleta de lixo e comportamento interno da Unity. |

## Execucoes

| Run | Seed | Hash topologico | Salas | Pavimentos | Conectores verticais | Conectividade | Fill | Caminho critico | Layout ms | Visual ms | Total ms | GameObjects | Mem delta KB |
|---:|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 2000 | A1717E5F | 48 | 2 | 1 | 100% | 46.96% | 246 | 0.665 | 65.123 | 65.974 | 5579 | 1312 |
| 2 | 2001 | D3B1144D | 42 | 2 | 1 | 100% | 45.972% | 576 | 0.878 | 57.335 | 58.361 | 5401 | 1252 |
| 3 | 2002 | 7611272D | 45 | 2 | 1 | 100% | 44.592% | 276 | 0.681 | 44.38 | 45.243 | 5303 | 1252 |
| 4 | 2003 | A5228F41 | 45 | 2 | 1 | 100% | 48.34% | 270 | 0.646 | 52.007 | 52.826 | 5563 | 1284 |
| 5 | 2004 | 9C10FF72 | 48 | 2 | 1 | 100% | 44.983% | 352 | 0.629 | 44.653 | 45.459 | 5462 | 1324 |
| 6 | 2005 | BDB44203 | 44 | 2 | 1 | 100% | 44.788% | 370 | 0.601 | 42.392 | 43.149 | 5250 | 1460 |
| 7 | 2006 | 09F4B3F4 | 45 | 2 | 1 | 100% | 44.055% | 368 | 0.598 | 44.348 | 45.107 | 5361 | 1564 |
| 8 | 2007 | 5E1594CF | 48 | 2 | 1 | 100% | 42.126% | 348 | 0.638 | 43.538 | 44.352 | 5050 | 1488 |
| 9 | 2008 | 5A099FEE | 46 | 2 | 1 | 100% | 43.86% | 312 | 0.592 | 43.704 | 44.46 | 5308 | 1536 |
| 10 | 2009 | CC79E927 | 45 | 2 | 1 | 100% | 43.518% | 332 | 0.605 | 44.334 | 45.106 | 5145 | 1488 |
| 11 | 2010 | D808F03F | 48 | 2 | 1 | 100% | 46.326% | 340 | 0.639 | 46.405 | 47.225 | 5528 | 1596 |
| 12 | 2011 | F240EEF8 | 44 | 2 | 1 | 100% | 42.468% | 404 | 0.576 | 42.484 | 43.215 | 5147 | 1492 |
| 13 | 2012 | AE6E74BD | 42 | 2 | 1 | 100% | 45.056% | 302 | 0.549 | 44.057 | 44.776 | 5376 | 1532 |
| 14 | 2013 | 002979B4 | 46 | 2 | 1 | 100% | 45.618% | 316 | 0.605 | 46.543 | 47.313 | 5387 | 1544 |
| 15 | 2014 | 12078910 | 48 | 2 | 1 | 100% | 44.885% | 370 | 0.632 | 46.311 | 47.11 | 5384 | 1560 |
| 16 | 2015 | 10B2C4D7 | 42 | 2 | 1 | 100% | 45.105% | 352 | 0.557 | 48.238 | 48.949 | 5409 | 1692 |
| 17 | 2016 | 1E447580 | 48 | 2 | 1 | 100% | 47.449% | 618 | 0.632 | 46.98 | 47.777 | 5673 | 1960 |
| 18 | 2017 | 498B8187 | 48 | 2 | 1 | 100% | 46.68% | 466 | 0.637 | 48.035 | 48.853 | 5599 | 1936 |
| 19 | 2018 | D73568F8 | 45 | 2 | 1 | 100% | 47.034% | 614 | 0.648 | 50.33 | 51.142 | 5582 | 1984 |
| 20 | 2019 | 04D63519 | 43 | 2 | 1 | 100% | 45.007% | 324 | 0.684 | 47.391 | 48.237 | 5322 | 1820 |
| 21 | 2020 | 7254045A | 44 | 2 | 1 | 100% | 47.131% | 432 | 0.659 | 51.478 | 52.306 | 5510 | 1904 |
| 22 | 2021 | F322DBE4 | 45 | 2 | 1 | 100% | 42.114% | 272 | 0.633 | 51.171 | 52.07 | 5062 | 1760 |
| 23 | 2022 | 6C446DED | 39 | 2 | 1 | 100% | 38% | 440 | 0.514 | 41.794 | 42.458 | 4686 | 1640 |
| 24 | 2023 | 6631B00A | 43 | 2 | 1 | 100% | 39.185% | 466 | 0.563 | 39.349 | 40.068 | 4769 | 1672 |
| 25 | 2024 | 24DFB3AD | 47 | 2 | 1 | 100% | 45.544% | 306 | 0.721 | 45.896 | 46.796 | 5385 | 1864 |
| 26 | 2025 | 27E34810 | 43 | 2 | 1 | 100% | 42.444% | 366 | 0.595 | 43.295 | 44.045 | 5080 | 1796 |
| 27 | 2026 | 1B9B1099 | 48 | 2 | 1 | 100% | 47.644% | 518 | 0.657 | 47.645 | 48.497 | 5700 | 1996 |
| 28 | 2027 | 3B854335 | 48 | 2 | 1 | 100% | 43.433% | 458 | 0.633 | 42.628 | 43.423 | 5180 | 1812 |
| 29 | 2028 | C9039CCE | 40 | 2 | 1 | 100% | 45.544% | 308 | 0.549 | 44.458 | 45.159 | 5341 | 1816 |
| 30 | 2029 | 87702613 | 48 | 2 | 1 | 100% | 39.978% | 440 | 0.627 | 42.521 | 43.313 | 4910 | 1728 |

## Como usar no texto da dissertacao

Use os parametros com status `Medido` como resultados quantitativos diretos. Parametros com status `Estimado sem NavMesh` ou `Estimado automaticamente` devem ser descritos como proxies instrumentais. Parametros com status `Nao implementado nesta versao` devem aparecer como limitacoes da variante BSP testada, nao necessariamente como impossibilidade teorica do algoritmo.
