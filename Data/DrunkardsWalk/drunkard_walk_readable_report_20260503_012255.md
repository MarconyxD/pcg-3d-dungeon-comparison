# Relatorio de Teste PCG - Drunkard Walk

Gerado em UTC: 2026-05-03T01:22:53.1935784Z

Teste Drunkard Walk executado com 30 seed(s). Topologias unicas: 30/30. Diversidade topologica: 100%. Reprodutibilidade por seed: aprovada.

## Resumo agregado

| Categoria | Parametro | Valor | Status | Interpretacao | Observacao do algoritmo |
|---|---|---:|---|---|---|
| Quantitativo | numRoomsTarget | media 3.3 (min 2, max 5) contagem | Medido | Resumo de 30 execucoes. | Drunkard Walk nao cria salas planejadas; regioes amplas aparecem quando o passeio se auto-intersecta ou recebe stamps locais. |
| Quantitativo | connectivityRatio | media 87.833 (min 60, max 100) % | Medido | Mostra se as regioes escavadas permaneceram conectadas sem flood-fill reparador, BSP, grafo-guia ou abertura posterior de tuneis. | Com caminhantes reiniciando apenas em celulas ja escavadas, a tendencia e conectividade alta; se falhar, e limitacao da extracao ou da variante multiandar. |
| Quantitativo | verticalVariance | media 1.857 (min 1.6, max 2) metros | Medido | Resumo de 30 execucoes. | Verticalidade so aparece quando a opcao multiandar permite passos verticais do proprio caminhante. |
| Quantitativo | fillPercentage | media 30.045 (min 30.029, max 30.127) % | Medido | Resumo de 30 execucoes. | Densidade e controlada por meta de preenchimento, passos maximos, raio do pincel e stamps de sala. |
| Quantitativo | branchFactor | media 1.082 (min 0.8, max 1.333) media | Medido | Resumo de 30 execucoes. | Ramificacao emerge de reinicios em celulas ja escavadas e mudancas de direcao, nao de um grafo explicito. |
| Quantitativo | avgPathLength | media 29.712 (min 8, max 51.572) metros | Medido | Resumo de 30 execucoes. | Caminhos sao medidos depois da extracao; o passeio nao escolhe objetivo final semanticamente. |
| Quantitativo | uniqueModules | media 19.433 (min 16, max 24) contagem | Medido | Resumo de 30 execucoes. | O algoritmo varia forma do caminho; variedade modular depende da biblioteca e da camada de instanciacao. |
| Quantitativo | navigableVolumeRatio | media 100 (min 100, max 100) % | Estimado sem NavMesh | Resumo de 30 execucoes. | Proxy logico; deve ser confirmado com NavMesh/colisao em uma validacao fisica. |
| Quantitativo | criticalPathLength | media 41.859 (min 8, max 71.941) metros | Medido | Resumo de 30 execucoes. | Pode gerar caminho longo por caminhada sinuosa, mas nao controla pacing ou progressao de missao sozinho. |
| Quantitativo | avgAlternativePathLength | media 0 (min 0, max 0) metros | Medido | Resumo de 30 execucoes. | Loops podem emergir por auto-interseccao do passeio, mas nao sao garantidos por planejamento global. |
| Booleano | SupportsRandomEnemySpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Spawns usam regioes escavadas; balanceamento nao e propriedade nativa do Drunkard Walk. |
| Booleano | SupportsLootDistribution | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Loot pode ser colocado nas regioes escavadas, mas progressao por risco depende de outra camada. |
| Booleano | SupportsTraps | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Armadilhas podem usar celulas escavadas/gargalos, mas sem semantica global nativa. |
| Booleano | SupportsBacktrackingLoops | Sim em 0/30 execucoes sim/nao | Medido | Parametro atendido somente quando as conexoes emergentes formam ciclos no grafo extraido. | Loops podem aparecer por auto-interseccao, mas nao ha garantia de ciclo jogavel em toda seed. |
| Booleano | SupportsVerticalConnectors | Sim em 30/30 execucoes sim/nao | Medido | Parametro atendido apenas se o proprio caminhante puder subir/descer e produzir alinhamentos entre pavimentos. | Possivel apenas na extensao multiandar em que o proprio caminhante faz passos verticais. |
| Booleano | SupportsMultiFloor | Sim em 30/30 execucoes sim/nao | Medido | Parametro atendido quando existe conexao vertical inferida do passeio multiandar. | Drunkard Walk 2D puro nao e multiandar; o passeio 3D continua puro, mas deve ser relatado como extensao volumetrica. |
| Booleano | SupportsBossArena | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Areas grandes podem surgir por stamps ou sobreposicao de passos, mas nao sao arenas semanticamente planejadas. |
| Booleano | SeedReproducible | Sim sim/nao | Medido | Parametro atendido. | Resultado reproduzivel quando o passeio usa a mesma seed. |
| Booleano | RuntimeRegeneration | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Custo cresce com tamanho do grid, quantidade de passos, caminhantes e raio de escavacao. |
| Booleano | BudgetAwareSpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Orcamento e aplicado apos a geracao, sobre celulas livres extraidas. |
| Qualitativo | Replayability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Alta variacao de percurso entre seeds e esperada. |
| Qualitativo | Debuggability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Drunkard Walk e reproduzivel e simples de depurar, pois o caminho do caminhante explica diretamente a area escavada. | O caminho do caminhante e facil de repetir e inspecionar, mas metricas ajudam a quantificar sinuosidade e loops. |
| Qualitativo | Flow | media 3.633/5 (min 2, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Fluxo tende a ser continuo, mas pode ficar sinuoso demais ou pouco ramificado. |
| Qualitativo | Legibility | media 4.633/5 (min 4, max 5) Likert 1-5 | Estimado automaticamente | A legibilidade depende do equilibrio entre corredor escavado, areas abertas e excesso de sinuosidade. | Corredores escavados podem ser claros, mas excesso de curvas reduz orientacao. |
| Qualitativo | StructuralVariety | media 2/5 (min 2, max 2) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Boa variedade de trajetos; menor controle sobre categorias semanticas de sala. |
| Performance | layoutGenerationMilliseconds | media 11.811 (min 8.613, max 42.137) ms | Medido | Resumo de 30 execucoes. | Inclui passeio aleatorio, escavacao e extracao de componentes. |
| Performance | geometryInstantiationMilliseconds | media 40.634 (min 34.319, max 71.546) ms | Medido | Resumo de 30 execucoes. | Custo visual depende dos prefabs/Unity, nao do Drunkard Walk puro. |
| Performance | metricsCalculationMilliseconds | media 1.664 (min 1.183, max 6.418) ms | Medido | Resumo de 30 execucoes. | Custo da instrumentacao, nao do algoritmo. |
| Performance | totalGenerationMilliseconds | media 54.111 (min 44.992, max 95.727) ms | Medido | Resumo de 30 execucoes. | Inclui layout, instanciacao visual e metricas; compare junto do tempo logico. |
| Performance | generatedGameObjectCount | media 3206.733 (min 3064, max 3341) contagem | Medido | Resumo de 30 execucoes. | Reflete peso da montagem visual das celulas abertas. |
| Performance | occupiedCellCount | media 2461.267 (min 2460, max 2468) contagem | Medido | Resumo de 30 execucoes. | Proxy do volume escavado pelo Drunkard Walk. |
| Performance | connectionCount | media 1.733 (min 1, max 2) contagem | Medido | Resumo de 30 execucoes. | Conexoes sao inferidas apos a geracao; o algoritmo nativo e um passeio, nao um grafo explicito. |
| Performance | managedMemoryDeltaKB | media -554.667 (min -115944, max 7800) KB | Estimado | Resumo de 30 execucoes. | Estimativa sujeita ao GC da Unity; use como indicio comparativo. |

## Execucoes

| Run | Seed | Hash topologico | Salas | Pavimentos | Conectores verticais | Conectividade | Fill | Caminho critico | Layout ms | Visual ms | Total ms | GameObjects | Mem delta KB |
|---:|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 2000 | 27C29434 | 2 | 2 | 1 | 100% | 30.029% | 8 | 10.264 | 44.525 | 56.189 | 3076 | 7684 |
| 2 | 2001 | EFE326B1 | 3 | 2 | 2 | 100% | 30.029% | 36.802 | 18.299 | 71.546 | 95.727 | 3261 | 7720 |
| 3 | 2002 | 0F2BCA01 | 2 | 2 | 1 | 100% | 30.078% | 12.944 | 42.137 | 42.604 | 86.064 | 3115 | -100776 |
| 4 | 2003 | C2A886F7 | 3 | 2 | 2 | 100% | 30.029% | 71.815 | 9.961 | 39.289 | 50.741 | 3064 | 3332 |
| 5 | 2004 | BEBF0A25 | 3 | 2 | 2 | 100% | 30.054% | 60.886 | 9.555 | 38.552 | 49.45 | 3239 | 6444 |
| 6 | 2005 | B772FADF | 5 | 2 | 2 | 60% | 30.029% | 36.249 | 9.501 | 40.103 | 50.817 | 3244 | 6492 |
| 7 | 2006 | 99B52482 | 5 | 2 | 2 | 60% | 30.042% | 54.99 | 8.786 | 38.835 | 48.888 | 3341 | 7608 |
| 8 | 2007 | 263D94BF | 3 | 2 | 2 | 100% | 30.127% | 57.852 | 9.876 | 38.883 | 50.044 | 3221 | 7684 |
| 9 | 2008 | 42479BBF | 4 | 2 | 2 | 75% | 30.029% | 35.305 | 9.564 | 38.163 | 49.026 | 3220 | 7516 |
| 10 | 2009 | 9FE48A5C | 2 | 2 | 1 | 100% | 30.042% | 8.472 | 9.842 | 39.748 | 50.895 | 3235 | 7624 |
| 11 | 2010 | FEFF574D | 3 | 2 | 2 | 100% | 30.029% | 56 | 10.174 | 38.922 | 50.34 | 3261 | 7576 |
| 12 | 2011 | A721EC9C | 5 | 2 | 2 | 60% | 30.054% | 60.143 | 11.922 | 40.248 | 54.125 | 3283 | 7596 |
| 13 | 2012 | F02A3EF9 | 3 | 2 | 2 | 100% | 30.029% | 67.781 | 10.213 | 42.586 | 54.021 | 3216 | 7568 |
| 14 | 2013 | A3F8EB77 | 2 | 2 | 1 | 100% | 30.09% | 16.649 | 9.029 | 37.529 | 47.973 | 3180 | 7624 |
| 15 | 2014 | CE09E272 | 2 | 2 | 1 | 100% | 30.042% | 17.416 | 9.972 | 39.294 | 50.998 | 3121 | 7600 |
| 16 | 2015 | EADE0115 | 2 | 2 | 1 | 100% | 30.042% | 14 | 9.828 | 39.516 | 50.655 | 3295 | 7748 |
| 17 | 2016 | E0D7541D | 3 | 2 | 2 | 100% | 30.042% | 30.077 | 9.685 | 39.722 | 50.772 | 3173 | 7780 |
| 18 | 2017 | DC8816D6 | 5 | 2 | 2 | 60% | 30.054% | 31.857 | 9.404 | 38.633 | 49.429 | 3250 | 7800 |
| 19 | 2018 | 7EA88459 | 3 | 2 | 2 | 100% | 30.029% | 56.038 | 14.918 | 53.24 | 74.579 | 3231 | 7756 |
| 20 | 2019 | 6DFC6BC8 | 2 | 2 | 1 | 100% | 30.029% | 8.472 | 27.519 | 39.186 | 68.113 | 3099 | -115944 |
| 21 | 2020 | 1C845AB4 | 5 | 2 | 2 | 60% | 30.042% | 52.703 | 10.337 | 36.421 | 48.027 | 3183 | 3272 |
| 22 | 2021 | 8147B425 | 4 | 2 | 2 | 75% | 30.029% | 67.246 | 11.552 | 40.298 | 53.097 | 3260 | 6348 |
| 23 | 2022 | C0FB3C55 | 4 | 2 | 2 | 75% | 30.042% | 57.666 | 8.668 | 44.132 | 54.091 | 3272 | 6468 |
| 24 | 2023 | B0B50AA2 | 4 | 2 | 2 | 75% | 30.078% | 52.166 | 8.869 | 38.467 | 48.522 | 3267 | 7232 |
| 25 | 2024 | 69DA7134 | 3 | 2 | 2 | 100% | 30.042% | 58.626 | 8.613 | 36.026 | 45.899 | 3236 | 7664 |
| 26 | 2025 | 1FDF03CD | 2 | 2 | 1 | 100% | 30.029% | 8 | 9.104 | 40.106 | 50.794 | 3148 | 7548 |
| 27 | 2026 | 553FB569 | 5 | 2 | 2 | 60% | 30.029% | 58.361 | 9.456 | 34.319 | 44.992 | 3086 | 7532 |
| 28 | 2027 | 81B4784A | 3 | 2 | 2 | 100% | 30.042% | 44.889 | 8.725 | 36.48 | 46.429 | 3229 | 7644 |
| 29 | 2028 | F8BC19CF | 4 | 2 | 2 | 75% | 30.042% | 71.941 | 9.41 | 36.424 | 47.024 | 3219 | 7592 |
| 30 | 2029 | AE02B6E0 | 3 | 2 | 2 | 100% | 30.042% | 42.419 | 9.136 | 35.216 | 45.605 | 3177 | 7628 |

## Como usar no texto da dissertacao

Use os parametros com status `Medido` como resultados quantitativos diretos. Parametros com status `Estimado sem NavMesh` ou `Estimado automaticamente` devem ser descritos como proxies instrumentais. Parametros com status `Nao implementado nesta versao` devem aparecer como limitacoes da variante do algoritmo testada, nao necessariamente como impossibilidade teorica do algoritmo.
