# Relatorio de Teste PCG - Cellular Automata

Gerado em UTC: 2026-05-03T00:23:05.8380623Z

Teste Cellular Automata executado com 30 seed(s). Topologias unicas: 30/30. Diversidade topologica: 100%. Reprodutibilidade por seed: aprovada.

## Resumo agregado

| Categoria | Parametro | Valor | Status | Interpretacao | Observacao do algoritmo |
|---|---|---:|---|---|---|
| Quantitativo | numRoomsTarget | media 2.133 (min 1, max 6) contagem | Medido | Resumo de 30 execucoes. | Cellular Automata nao cria salas como entidades planejadas; regioes emergem de celulas abertas. |
| Quantitativo | connectivityRatio | media 63 (min 16.667, max 100) % | Medido | Mostra se as cavernas emergentes ficaram conectadas sem flood-fill reparador, carving ou grafo-guia. | CA puro nao garante conectividade global. Se a metrica ficar baixa, isso e uma limitacao real do algoritmo nesta configuracao. |
| Quantitativo | verticalVariance | media 0 (min 0, max 0) metros | Medido | Resumo de 30 execucoes. | Verticalidade so aparece em uma variante celular 3D/multiandar; CA 2D simples nao possui altura. |
| Quantitativo | fillPercentage | media 65.219 (min 59.692, max 70.801) % | Medido | Resumo de 30 execucoes. | Densidade e controlada indiretamente por chance inicial e limites de nascimento/sobrevivencia. |
| Quantitativo | branchFactor | media 0.03 (min 0, max 0.5) media | Medido | Resumo de 30 execucoes. | Ramificacao emerge de gargalos naturais, nao de um grafo explicito. |
| Quantitativo | avgPathLength | media 1.814 (min 0, max 54.406) metros | Medido | Resumo de 30 execucoes. | Caminhos sao medidos depois da extracao; CA nao controla caminho critico diretamente. |
| Quantitativo | uniqueModules | media 16.1 (min 12, max 21) contagem | Medido | Resumo de 30 execucoes. | CA tende a variar forma organica, mas nao aumenta variedade modular sozinho. |
| Quantitativo | navigableVolumeRatio | media 100 (min 100, max 100) % | Estimado sem NavMesh | Resumo de 30 execucoes. | Proxy logico; ilhas desconectadas podem inflar area aberta sem criar navegabilidade util. |
| Quantitativo | criticalPathLength | media 1.814 (min 0, max 54.406) metros | Medido | Resumo de 30 execucoes. | CA puro nao direciona progressao inicio-fim sem uma camada adicional. |
| Quantitativo | avgAlternativePathLength | media 0 (min 0, max 0) metros | Medido | Resumo de 30 execucoes. | Loops podem emergir, mas nao sao garantidos. |
| Booleano | SupportsRandomEnemySpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Spawns usam regioes abertas extraidas; balanceamento nao e propriedade nativa do CA. |
| Booleano | SupportsLootDistribution | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Loot pode ser colocado nas regioes abertas, mas progressao por risco depende de outra camada. |
| Booleano | SupportsTraps | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Armadilhas podem usar celulas abertas/gargalos, mas sem semantica global nativa. |
| Booleano | SupportsBacktrackingLoops | Sim em 0/30 execucoes sim/nao | Medido | Parametro atendido somente quando as conexoes emergentes formam ciclos no grafo extraido. | CA pode formar ciclos emergentes, mas sem controle direto de loops. |
| Booleano | SupportsVerticalConnectors | Sim em 0/30 execucoes sim/nao | Medido | Parametro atendido apenas se o volume celular multiandar gerar alinhamentos abertos entre camadas. | Possivel apenas em uma leitura 3D/multiandar do CA, por alinhamento de celulas abertas entre camadas. |
| Booleano | SupportsMultiFloor | Sim em 0/30 execucoes sim/nao | Medido | Parametro atendido quando existe conexao vertical inferida do proprio volume celular. | CA 2D puro nao e multiandar; a variante 3D continua celular, mas precisa ser relatada como extensao volumetrica. |
| Booleano | SupportsBossArena | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Areas grandes podem emergir naturalmente, mas nao como arena semanticamente planejada. |
| Booleano | SeedReproducible | Sim sim/nao | Medido | Parametro atendido. | Resultado reproduzivel se ruido inicial e iteracoes usam a mesma seed. |
| Booleano | RuntimeRegeneration | Sim em 30/30 execucoes sim/nao | Medido | Frequencia observada no lote de teste. | Custo cresce com tamanho do grid, pavimentos e iteracoes. |
| Booleano | BudgetAwareSpawns | Sim em 30/30 execucoes sim/nao | Suportado | Frequencia observada no lote de teste. | Orcamento e aplicado apos a geracao, sobre celulas livres extraidas. |
| Qualitativo | Replayability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Alta variacao visual/organica entre seeds e esperada. |
| Qualitativo | Debuggability | media 5/5 (min 5, max 5) Likert 1-5 | Estimado automaticamente | Cellular Automata e reproduzivel e simples de parametrizar, mas o efeito de pequenas mudancas nas regras precisa ser observado por bateladas. | Regras sao simples, mas efeitos globais emergentes exigem analise por metricas. |
| Qualitativo | Flow | media 1.767/5 (min 1, max 3) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Fluxo global nao e garantido; depende de conectividade emergente. |
| Qualitativo | Legibility | media 3.367/5 (min 3, max 4) Likert 1-5 | Estimado automaticamente | A legibilidade depende do equilibrio entre areas abertas, gargalos e ilhas desconectadas. | Cavernas organicas podem ser bonitas, mas menos legiveis que salas retangulares. |
| Qualitativo | StructuralVariety | media 2/5 (min 2, max 2) Likert 1-5 | Estimado automaticamente | Diversidade topologica do lote: 100%. | Boa variedade morfologica; menor controle sobre semantica estrutural. |
| Performance | layoutGenerationMilliseconds | media 15.966 (min 10.637, max 53.75) ms | Medido | Resumo de 30 execucoes. | Inclui ruido inicial, iteracoes celulares e extracao de componentes. |
| Performance | geometryInstantiationMilliseconds | media 53.942 (min 45.558, max 66.535) ms | Medido | Resumo de 30 execucoes. | Custo visual depende dos prefabs/Unity, nao do CA puro. |
| Performance | metricsCalculationMilliseconds | media 1.756 (min 1.278, max 4.689) ms | Medido | Resumo de 30 execucoes. | Custo da instrumentacao, nao do algoritmo. |
| Performance | totalGenerationMilliseconds | media 71.669 (min 58.176, max 102.424) ms | Medido | Resumo de 30 execucoes. | Inclui layout, instanciacao visual e metricas; compare junto do tempo logico. |
| Performance | generatedGameObjectCount | media 3829.433 (min 3695, max 3961) contagem | Medido | Resumo de 30 execucoes. | Reflete peso da montagem visual das celulas abertas. |
| Performance | occupiedCellCount | media 2671.367 (min 2445, max 2900) contagem | Medido | Resumo de 30 execucoes. | Proxy do volume aberto gerado pelo CA. |
| Performance | connectionCount | media 0.067 (min 0, max 1) contagem | Medido | Resumo de 30 execucoes. | Conexoes sao inferidas apos a geracao; nao sao primitivas nativas do CA. |
| Performance | managedMemoryDeltaKB | media -383.2 (min -111312, max 8800) KB | Estimado | Resumo de 30 execucoes. | Estimativa sujeita ao GC da Unity; use como indicio comparativo. |

## Execucoes

| Run | Seed | Hash topologico | Salas | Pavimentos | Conectores verticais | Conectividade | Fill | Caminho critico | Layout ms | Visual ms | Total ms | GameObjects | Mem delta KB |
|---:|---:|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 1 | 2000 | 391DA0F2 | 4 | 1 | 0 | 25% | 64.893% | 0 | 16.111 | 48.552 | 66.084 | 3786 | 8280 |
| 2 | 2001 | C1DC5FDF | 2 | 1 | 0 | 50% | 63.916% | 0 | 12.788 | 58.098 | 72.274 | 3830 | 8160 |
| 3 | 2002 | 456495B1 | 2 | 1 | 0 | 50% | 63.916% | 0 | 12.088 | 53.86 | 67.296 | 3762 | 8160 |
| 4 | 2003 | 55DEF7DB | 1 | 1 | 0 | 100% | 62.573% | 0 | 15.218 | 66.535 | 86.446 | 3794 | 7996 |
| 5 | 2004 | B0F99AED | 3 | 1 | 0 | 33.333% | 65.649% | 0 | 49.575 | 51.15 | 102.168 | 3802 | -105628 |
| 6 | 2005 | 748282CC | 1 | 1 | 0 | 100% | 66.064% | 0 | 12.855 | 51.346 | 65.863 | 3834 | 3684 |
| 7 | 2006 | 6E2E32B8 | 2 | 1 | 0 | 50% | 65.259% | 0 | 12.418 | 54.818 | 68.845 | 3776 | 6508 |
| 8 | 2007 | A1A85A89 | 2 | 1 | 0 | 50% | 63.843% | 0 | 14.017 | 55.273 | 71.294 | 3886 | 6700 |
| 9 | 2008 | 67438BF3 | 1 | 1 | 0 | 100% | 65.308% | 0 | 12.238 | 55.884 | 70.363 | 3821 | 6976 |
| 10 | 2009 | 9C4051C4 | 4 | 1 | 0 | 25% | 59.692% | 0 | 13.574 | 49.446 | 64.448 | 3695 | 6404 |
| 11 | 2010 | BAA29ACC | 1 | 1 | 0 | 100% | 62.5% | 0 | 14.241 | 57.002 | 73.606 | 3788 | 6580 |
| 12 | 2011 | 618E880B | 2 | 1 | 0 | 50% | 65.967% | 0 | 19.939 | 54.158 | 75.709 | 3814 | 7856 |
| 13 | 2012 | AD76F455 | 2 | 1 | 0 | 50% | 68.018% | 0 | 14.38 | 57.526 | 73.344 | 3912 | 8460 |
| 14 | 2013 | 561C3505 | 2 | 1 | 0 | 50% | 65.356% | 0 | 12.016 | 50.62 | 64.707 | 3814 | 8152 |
| 15 | 2014 | 882865B4 | 1 | 1 | 0 | 100% | 70.801% | 0 | 13.926 | 52.191 | 67.471 | 3890 | 8800 |
| 16 | 2015 | CA7BCD04 | 1 | 1 | 0 | 100% | 63.623% | 0 | 13.633 | 50.077 | 65.083 | 3800 | 7908 |
| 17 | 2016 | 6BEC6FF5 | 2 | 1 | 0 | 50% | 69.043% | 0 | 15.423 | 48.361 | 65.42 | 3961 | 8588 |
| 18 | 2017 | DFB128DF | 1 | 1 | 0 | 100% | 62.939% | 0 | 11.756 | 49.633 | 63.027 | 3814 | 7864 |
| 19 | 2018 | 9C08D84E | 3 | 1 | 0 | 33.333% | 64.624% | 0 | 11.214 | 45.558 | 58.176 | 3809 | 8204 |
| 20 | 2019 | 2EA31189 | 3 | 1 | 0 | 33.333% | 65.503% | 0 | 11.559 | 62.656 | 76.706 | 3859 | 8392 |
| 21 | 2020 | EF577186 | 3 | 1 | 0 | 33.333% | 66.333% | 0 | 53.75 | 47.249 | 102.424 | 3837 | -111312 |
| 22 | 2021 | 5ED0D661 | 6 | 1 | 0 | 16.667% | 61.035% | 0 | 10.637 | 52.16 | 64.365 | 3816 | 2888 |
| 23 | 2022 | 44E568B3 | 2 | 1 | 0 | 50% | 66.919% | 0 | 11.555 | 50.596 | 63.777 | 3841 | 6620 |
| 24 | 2023 | DB8290D7 | 1 | 1 | 0 | 100% | 65.918% | 0 | 13.755 | 50.625 | 65.661 | 3866 | 6884 |
| 25 | 2024 | 00FF1EC5 | 1 | 1 | 0 | 100% | 65.747% | 0 | 14.878 | 55.343 | 72 | 3801 | 7028 |
| 26 | 2025 | 8CFA022E | 5 | 1 | 0 | 40% | 64.258% | 54.406 | 13.632 | 58.241 | 73.661 | 3908 | 6852 |
| 27 | 2026 | C7720854 | 2 | 1 | 0 | 50% | 67.847% | 0 | 12.922 | 59.8 | 74.573 | 3869 | 7124 |
| 28 | 2027 | 53F32175 | 2 | 1 | 0 | 50% | 63.794% | 0 | 12.051 | 57.443 | 70.83 | 3782 | 7544 |
| 29 | 2028 | 72C7435E | 1 | 1 | 0 | 100% | 68.555% | 0 | 12.912 | 53.107 | 67.495 | 3822 | 8516 |
| 30 | 2029 | ED7EC54B | 1 | 1 | 0 | 100% | 66.675% | 0 | 13.931 | 60.956 | 76.951 | 3894 | 8316 |

## Como usar no texto da dissertacao

Use os parametros com status `Medido` como resultados quantitativos diretos. Parametros com status `Estimado sem NavMesh` ou `Estimado automaticamente` devem ser descritos como proxies instrumentais. Parametros com status `Nao implementado nesta versao` devem aparecer como limitacoes da variante do algoritmo testada, nao necessariamente como impossibilidade teorica do algoritmo.
