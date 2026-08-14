# Relatorio de Teste PCG - Cellular Automata

Gerado em UTC: 2026-08-14T03:59:00.5124536Z

Teste Cellular Automata executado com 30 seed(s). Topologias unicas: 30/30. Diversidade topologica: 100%. Reprodutibilidade por seed: aprovada.

## Resumo agregado

| Categoria | Parametro | Valor | Status | Interpretacao | Observacao do algoritmo |
|---|---|---:|---|---|---|
| Quantitativo | numRoomsTarget | media 2.133 (min 2, max 4) contagem | Medido | Resumo de 30 execucoes. | Cellular Automata nao cria salas como entidades planejadas; regioes emergem de celulas abertas. |
| Quantitativo | connectivityRatio | media 99.167 (min 75, max 100) % | Medido | Mostra se as cavernas emergentes ficaram conectadas sem flood-fill reparador, carving ou grafo-guia. | CA puro nao garante conectividade global. Se a metrica ficar baixa, isso e uma limitacao real do algoritmo nesta configuracao. |
| Quantitativo | verticalVariance | media 1.983 (min 1.732, max 2) metros | Medido | Resumo de 30 execucoes. | Verticalidade so aparece em uma variante celular 3D/multiandar; CA 2D simples nao possui altura. |
| Quantitativo | fillPercentage | media 84.485 (min 82.446, max 86.316) % | Medido | Resumo de 30 execucoes. | Densidade e controlada indiretamente por chance inicial e limites de nascimento/sobrevivencia. |
| Quantitativo | branchFactor | media 1.022 (min 1, max 1.333) media | Medido | Resumo de 30 execucoes. | Ramificacao emerge de gargalos naturais, nao de um grafo explicito. |
| Quantitativo | avgPathLength | media 9.698 (min 6, max 46.897) metros | Medido | Resumo de 30 execucoes. | Caminhos sao medidos depois da extracao; CA nao controla caminho critico diretamente. |
| Quantitativo | uniqueModules | media 19.067 (min 16, max 24) contagem | Medido | Resumo de 30 execucoes. | CA tende a variar forma organica, mas nao aumenta variedade modular sozinho. |
| Quantitativo | navigableVolumeRatio | media 100 (min 100, max 100) % | Estimado sem NavMesh | Resumo de 30 execucoes. | Proxy logico; ilhas desconectadas podem inflar area aberta sem criar navegabilidade util. |
| Quantitativo | criticalPathLength | media 13.397 (min 6, max 87.795) metros | Medido | Resumo de 30 execucoes. | CA puro nao direciona progressao inicio-fim sem uma camada adicional. |
| Quantitativo | avgAlternativePathLength | media 0 (min 0, max 0) metros | Medido | Resumo de 30 execucoes. | Loops podem emergir, mas nao sao garantidos. |
| Booleano | SupportsRandomEnemySpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Spawns usam regioes abertas extraidas; balanceamento nao e propriedade nativa do CA. |
| Booleano | SupportsLootDistribution | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Loot pode ser colocado nas regioes abertas, mas progressao por risco depende de outra camada. |
| Booleano | SupportsTraps | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Armadilhas podem usar celulas abertas/gargalos, mas sem semantica global nativa. |
| Booleano | SupportsBacktrackingLoops | Sim em 0/30 execucoes sim/nao | Medido | Parametro atendido somente quando as conexoes emergentes formam ciclos no grafo extraido. | CA pode formar ciclos emergentes, mas sem controle direto de loops. |
| Booleano | SupportsVerticalConnectors | Sim em 30/30 execucoes sim/nao | Medido | Parametro atendido apenas se o volume celular multiandar gerar alinhamentos abertos entre camadas. | Possivel apenas em uma leitura 3D/multiandar do CA, por alinhamento de celulas abertas entre camadas. |
| Booleano | SupportsMultiFloor | Sim em 30/30 execucoes sim/nao | Medido | Parametro atendido quando existe conexao vertical inferida do proprio volume celular. | CA 2D puro nao e multiandar; a variante 3D continua celular, mas precisa ser relatada como extensao volumetrica. |
| Booleano | SupportsBossArena | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Areas grandes podem emergir naturalmente, mas nao como arena semanticamente planejada. |
| Booleano | SeedReproducible | Sim sim/nao | Medido | Parametro atendido. | Resultado reproduzivel se ruido inicial e iteracoes usam a mesma seed. |
| Booleano | RuntimeRegeneration | Sim em 29/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Custo cresce com tamanho do grid, pavimentos e iteracoes. |
| Booleano | BudgetAwareSpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Orcamento e aplicado apos a geracao, sobre celulas livres extraidas. |
| Qualitativo | Replayability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Alta variacao visual/organica entre seeds e esperada. |
| Qualitativo | Debuggability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Cellular Automata e reproduzivel e simples de parametrizar, mas o efeito de pequenas mudancas nas regras precisa ser observado por bateladas. | Regras sao simples, mas efeitos globais emergentes exigem analise por metricas. |
| Qualitativo | Flow | media 4/5 (min 2, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Fluxo global nao e garantido; depende de conectividade emergente. |
| Qualitativo | Legibility | media 3.967/5 (min 3, max 4) Likert 1-5 | Estimado automaticamente | A legibilidade depende do equilibrio entre areas abertas, gargalos e ilhas desconectadas. | Cavernas organicas podem ser bonitas, mas menos legiveis que salas retangulares. |
| Qualitativo | StructuralVariety | media 2/5 (min 2, max 2) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Boa variedade morfologica; menor controle sobre semantica estrutural. |
| Performance | layoutGenerationMilliseconds | media 32.646 (min 27.885, max 88.289) ms | Medido | Resumo de 30 execucoes. | Inclui ruido inicial, iteracoes celulares e extracao de componentes. |
| Performance | geometryInstantiationMilliseconds | media 97.243 (min 86.109, max 189.353) ms | Medido | Resumo de 30 execucoes. | Custo visual depende dos prefabs/Unity, nao do CA puro. |
| Performance | metricsCalculationMilliseconds | media 3.751 (min 3.164, max 12.796) ms | Medido | Resumo de 30 execucoes. | Custo da instrumentacao, nao do algoritmo. |
| Performance | totalGenerationMilliseconds | media 133.643 (min 117.827, max 281.001) ms | Medido | Resumo de 30 execucoes. | Inclui layout, instanciacao visual e metricas; compare junto do tempo logico. |
| Performance | generatedGameObjectCount | media 8117.067 (min 8052, max 8209) contagem | Medido | Resumo de 30 execucoes. | Reflete peso da montagem visual das celulas abertas. |
| Performance | occupiedCellCount | media 6921 (min 6754, max 7071) contagem | Medido | Resumo de 30 execucoes. | Proxy do volume aberto gerado pelo CA. |
| Performance | connectionCount | media 1.1 (min 1, max 2) contagem | Medido | Resumo de 30 execucoes. | Conexoes sao inferidas apos a geracao; nao sao primitivas nativas do CA. |
| Performance | managedMemoryDeltaKB | media -345.467 (min -199868, max 22004) KB | Estimado | Resumo de 30 execucoes. | Estimativa sujeita ao GC da Unity; use como indicio comparativo. |

## Execucoes

| Run | Seed | Hash topologico | Salas | Pavimentos | Conectores verticais | Conectividade | Fill | Caminho critico | Layout ms | Visual ms | Total ms | GameObjects | Mem delta KB |
|---:|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 2000 | 7B040F57 | 2 | 2 | 1 | 100% | 84.705% | 6 | 29.235 | 87.234 | 119.829 | 8091 | 17764 |
| 2 | 2001 | 40998080 | 2 | 2 | 1 | 100% | 82.861% | 6 | 28.784 | 90.063 | 122.239 | 8064 | 20444 |
| 3 | 2002 | 13628002 | 2 | 2 | 1 | 100% | 84.204% | 6 | 34.775 | 95.232 | 133.375 | 8157 | 20736 |
| 4 | 2003 | 826AA19C | 2 | 2 | 1 | 100% | 83.276% | 6 | 28.488 | 89.565 | 121.458 | 8168 | 20524 |
| 5 | 2004 | 35C022D2 | 2 | 2 | 1 | 100% | 83.765% | 6 | 28.668 | 93.238 | 126.253 | 8111 | 20636 |
| 6 | 2005 | 229D8E5E | 2 | 2 | 1 | 100% | 85.547% | 6 | 53.69 | 89.283 | 146.338 | 8090 | -103396 |
| 7 | 2006 | DF105D80 | 2 | 2 | 1 | 100% | 83.032% | 6 | 30.497 | 87.262 | 121.023 | 8096 | 14340 |
| 8 | 2007 | 4A3DC9E5 | 2 | 2 | 1 | 100% | 85.12% | 6 | 28.275 | 88.492 | 120.171 | 8209 | 20672 |
| 9 | 2008 | A96948D8 | 2 | 2 | 1 | 100% | 84.204% | 6 | 28.053 | 86.569 | 117.827 | 8120 | 20748 |
| 10 | 2009 | 27D8EB1C | 2 | 2 | 1 | 100% | 82.471% | 6 | 27.885 | 88.584 | 119.637 | 8111 | 20324 |
| 11 | 2010 | B39DD387 | 2 | 2 | 1 | 100% | 83.704% | 6 | 27.987 | 90.223 | 121.539 | 8083 | 20616 |
| 12 | 2011 | D4F6830C | 2 | 2 | 1 | 100% | 84.741% | 6 | 32.659 | 117.35 | 153.463 | 8126 | -99236 |
| 13 | 2012 | EB2853EC | 2 | 2 | 1 | 100% | 84.766% | 6 | 28.38 | 86.605 | 118.301 | 8155 | 9420 |
| 14 | 2013 | E52E6DC8 | 4 | 2 | 2 | 75% | 84.155% | 86.368 | 28.294 | 86.623 | 118.539 | 8103 | 18080 |
| 15 | 2014 | D90353C5 | 2 | 2 | 1 | 100% | 85.242% | 6 | 28.368 | 86.465 | 118.113 | 8153 | 20988 |
| 16 | 2015 | D45C77DB | 2 | 2 | 1 | 100% | 85.364% | 6 | 28.285 | 87.779 | 119.346 | 8131 | 21032 |
| 17 | 2016 | 25226FE4 | 2 | 2 | 1 | 100% | 85.059% | 6 | 28.748 | 86.109 | 118.111 | 8075 | 20940 |
| 18 | 2017 | 95C5D4F7 | 2 | 2 | 1 | 100% | 86.047% | 6 | 28.641 | 93.996 | 127.132 | 8191 | 21184 |
| 19 | 2018 | 54727B2A | 2 | 2 | 1 | 100% | 84.375% | 6 | 52.042 | 87.325 | 142.77 | 8106 | -108464 |
| 20 | 2019 | A2701895 | 2 | 2 | 1 | 100% | 85.779% | 6 | 28.879 | 87.773 | 120.203 | 8096 | 13828 |
| 21 | 2020 | 3B515BAB | 3 | 2 | 2 | 100% | 83.862% | 65.741 | 28.32 | 97.092 | 128.672 | 8089 | 19148 |
| 22 | 2021 | 0C5C3E5F | 2 | 2 | 1 | 100% | 84.094% | 6 | 29.876 | 103.867 | 136.986 | 8117 | 20720 |
| 23 | 2022 | EA4A7189 | 2 | 2 | 1 | 100% | 84.778% | 6 | 28.979 | 90.733 | 123.028 | 8113 | 20880 |
| 24 | 2023 | 61EF832E | 2 | 2 | 1 | 100% | 85.4% | 6 | 28.575 | 87.566 | 119.572 | 8106 | 21020 |
| 25 | 2024 | EC1FD171 | 3 | 2 | 2 | 100% | 84.937% | 87.795 | 29.077 | 185.249 | 227.126 | 8122 | 22004 |
| 26 | 2025 | 05A90EFF | 2 | 2 | 1 | 100% | 84.924% | 6 | 88.289 | 189.353 | 281.001 | 8122 | -199868 |
| 27 | 2026 | B25367FC | 2 | 2 | 1 | 100% | 84.399% | 6 | 28.978 | 92.227 | 124.981 | 8070 | 13152 |
| 28 | 2027 | 6D787A36 | 2 | 2 | 1 | 100% | 82.446% | 6 | 28.099 | 87.41 | 118.709 | 8052 | 19220 |
| 29 | 2028 | 9AF97D5B | 2 | 2 | 1 | 100% | 84.973% | 6 | 29.264 | 88.698 | 121.38 | 8114 | 20932 |
| 30 | 2029 | E936FF63 | 2 | 2 | 1 | 100% | 86.316% | 6 | 29.292 | 89.319 | 122.17 | 8171 | 21248 |

## Como usar no texto da dissertacao

Use os parametros com status `Medido` como resultados quantitativos diretos. Parametros com status `Estimado sem NavMesh` ou `Estimado automaticamente` devem ser descritos como proxies instrumentais. Parametros com status `Nao implementado nesta versao` devem aparecer como limitacoes da variante do algoritmo testada, nao necessariamente como impossibilidade teorica do algoritmo.
