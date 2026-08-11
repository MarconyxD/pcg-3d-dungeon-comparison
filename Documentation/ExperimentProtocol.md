<a id="portuguese-version"></a>

# Protocolo Experimental

[![English version](https://img.shields.io/badge/English_version-Click_here-0969DA?style=for-the-badge)](#english-version)

Este documento descreve o protocolo utilizado para executar, medir e registrar as baterias experimentais da dissertação.

As configurações específicas dos algoritmos encontram-se em `../Configuration/AlgorithmParameters.md`, enquanto as condições comuns do experimento são documentadas em `../Configuration/ExperimentalConfiguration.md`.

Os dados resultantes das baterias finais encontram-se em `../Data/`.

---

## Visão geral

O experimento compara seis técnicas de geração procedural de dungeons:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

Cada algoritmo foi submetido a 30 execuções utilizando as seeds de `2000` a `2029`.

O conjunto experimental principal contém:

`6 algoritmos × 30 execuções = 180 gerações`

A geração, coleta das métricas, identificação topológica e consolidação dos resultados foram realizadas automaticamente pelos sistemas implementados na Unity.

---

## Ambiente de execução

Os testes foram realizados:

- diretamente no Editor da Unity;
- fora do Play Mode;
- sem utilização de uma build compilada;
- por meio dos botões personalizados presentes no Inspector de cada gerador.

O ambiente completo de hardware e software está documentado em:

`../Configuration/HardwareAndSoftware.md`

---

## Preparação da cena

Antes da execução das baterias, os seis geradores estavam presentes na mesma cena experimental.

Cada componente utilizava:

- a configuração correspondente ao algoritmo;
- a mesma `DungeonAssetLibrary`;
- as mesmas dimensões gerais de geração;
- os mesmos orçamentos comuns para inimigos, loot e armadilhas;
- o mesmo conjunto experimental de seeds.

A opção `Clear Before Generate` permanecia ativada para impedir que objetos pertencentes à geração anterior permanecessem na cena.

---

## Início da bateria

A bateria era iniciada pelo botão de medição disponível no Inspector do respectivo gerador.

Dependendo do algoritmo, o botão possuía denominação equivalente a:

`Run Measurement Test`

ou uma variação específica contendo o nome do método.

Após o acionamento do botão, as etapas seguintes eram realizadas automaticamente.

---

## Verificação de reprodutibilidade

Antes das 30 execuções principais, o sistema verificava a reprodutibilidade da primeira seed experimental.

A seed `2000` era gerada duas vezes pelo mesmo algoritmo.

Em cada geração era calculado um identificador da topologia produzida.

Quando os dois identificadores coincidiam, a reprodução por seed era considerada aprovada.

Essas duas gerações de verificação não eram contabilizadas entre as 30 execuções da bateria comparativa.

---

## Sequência das seeds

A bateria utilizava:

`Test First Seed = 2000`

Para cada execução, a seed era definida automaticamente por:

`runSeed = testFirstSeed + i`

Assim:

| Execução | Seed |
|---:|---:|
| 1 | 2000 |
| 2 | 2001 |
| 3 | 2002 |
| ... | ... |
| 30 | 2029 |

O mesmo intervalo foi utilizado pelos seis algoritmos.

---

## Procedimento por execução

Para cada seed, o sistema realizava a seguinte sequência geral:

1. preparava o estado interno do gerador;
2. aplicava a seed correspondente;
3. construía o layout lógico da dungeon;
4. estabelecia salas ou regiões e suas conexões;
5. atribuía início e objetivo;
6. instanciava a geometria tridimensional quando a medição visual estava habilitada;
7. calculava as métricas estruturais;
8. registrava os parâmetros booleanos;
9. calculava as medidas de desempenho;
10. gerava o identificador topológico;
11. armazenava o resultado daquela execução.

Após essa sequência, o sistema avançava automaticamente para a próxima seed.

---

## Identificador topológico

Cada dungeon recebia um identificador calculado a partir de sua estrutura.

O identificador considerava informações como:

- dimensões das regiões;
- posições das regiões;
- pavimento;
- relações entre salas ou regiões;
- conexões verticais;
- conexões adicionais identificadas como loops.

Esse valor permitia comparar estruturas geradas em diferentes execuções sem depender apenas de inspeção visual.

Também foi utilizado para:

- verificar a reprodutibilidade por seed;
- contar topologias únicas;
- calcular a diversidade topológica da bateria.

---

## Diversidade topológica

Ao longo das 30 execuções, os identificadores topológicos eram armazenados em um conjunto de valores únicos.

A diversidade da bateria era calculada pela relação:

`diversidade = topologias únicas / número de execuções`

Como cada bateria continha 30 execuções:

`diversidade = topologias únicas / 30`

Esse resultado também serviu como base para a estimativa automatizada de replayability.

A replayability documentada neste projeto representa, portanto, diversidade estrutural entre seeds e não uma avaliação direta da intenção de jogadores de repetir a experiência.

---

## Medição de desempenho

Os tempos de execução foram medidos por meio de `System.Diagnostics.Stopwatch`.

O procedimento separava diferentes etapas para evitar que o custo lógico do algoritmo fosse confundido com o custo de representação tridimensional.

### Tempo de geração lógica

Representa o período necessário para que o algoritmo construa seu layout procedural e suas relações estruturais.

Esse valor corresponde ao componente mais diretamente relacionado ao custo computacional da técnica de geração.

### Tempo de instanciação visual

Representa o período utilizado pela Unity para criar os objetos tridimensionais que materializam o layout.

Essa etapa pode incluir:

- pisos;
- paredes;
- portas;
- escadas;
- objetos decorativos;
- elementos adicionais associados à dungeon.

O custo visual é apresentado separadamente porque depende não apenas do algoritmo, mas também da quantidade de objetos instanciados e do ambiente da Unity.

### Tempo de cálculo das métricas

Representa o período utilizado pelo sistema de instrumentação para analisar o layout produzido e calcular os indicadores utilizados na pesquisa.

Essa etapa não faz parte do funcionamento essencial do algoritmo procedural, sendo específica do experimento.

### Tempo total

Quando todas as etapas eram medidas conjuntamente, o tempo total compreendia:

`geração lógica + instanciação visual + cálculo das métricas`

Os tempos são registrados em milissegundos.

---

## Memória gerenciada

O sistema também registrava uma estimativa da variação da memória gerenciada.

A memória era consultada antes e depois da geração utilizando o mecanismo de memória gerenciada do ambiente .NET utilizado pela Unity.

Esse valor foi mantido como indicador complementar.

Ele não deve ser interpretado como consumo absoluto de memória do algoritmo, pois pode sofrer influência de:

- coleta de lixo;
- estado anterior do Editor;
- objetos temporários;
- comportamento interno da Unity.

---

## Medição visual

Para os resultados de desempenho visual utilizados na dissertação, a opção:

`Measure Visual Instantiation In Tests`

foi utilizada para registrar o tempo de instanciação da geometria tridimensional.

Para BSP, Cellular Automata, Drunkard's Walk e Grammar-Based Generation, os dados finais utilizados na comparação encontram-se nas respectivas baterias selecionadas em `../Data/`.

Room Graph e WFC exigem uma observação adicional.

---

## Procedimento complementar do Room Graph

Os dados preservados do Room Graph incluem duas baterias.

### Bateria estrutural

Timestamp:

`20260502_190613`

Essa bateria constitui a referência para:

- métricas estruturais;
- parâmetros booleanos;
- estimativas qualitativas;
- tempo lógico;
- cálculo das métricas.

### Bateria de instanciação visual

Timestamp:

`20260519_183913`

Essa bateria foi utilizada para obter o custo de instanciação tridimensional.

As duas baterias utilizaram as seeds `2000–2029`.

Durante a auditoria dos resultados, os 30 identificadores topológicos foram comparados entre as duas baterias e apresentaram correspondência seed por seed.

Assim, a bateria complementar reproduziu as mesmas topologias da coleta estrutural.

Quando o desempenho do Room Graph é apresentado de forma conjunta na dissertação, os componentes devem ser interpretados como uma composição das médias correspondentes e não como um único cronômetro contínuo pertencente a uma única bateria.

---

## Procedimento complementar do WFC

O WFC também possui duas baterias preservadas.

### Bateria estrutural

Timestamp:

`20260502_204944`

Essa bateria constitui a referência para:

- métricas estruturais;
- parâmetros booleanos;
- estimativas qualitativas;
- tempo lógico;
- cálculo das métricas.

### Bateria de instanciação visual

Timestamp:

`20260519_184822`

Essa bateria foi utilizada para obter o custo de instanciação tridimensional.

As duas baterias utilizam as mesmas seeds `2000–2029`.

Os identificadores topológicos das 30 execuções também apresentaram correspondência seed por seed.

Portanto, a medição complementar representa as mesmas estruturas utilizadas na análise lógica.

---

## Tratamento de resultados desconectados

Mapas desconectados não eram automaticamente considerados inválidos.

A conectividade constitui uma das variáveis analisadas no experimento.

Consequentemente, uma geração com:

- regiões isoladas;
- conectividade reduzida;
- ausência de loops;
- ausência de verticalidade;
- baixa ramificação;

continuava fazendo parte da bateria experimental.

Não era utilizada uma nova seed apenas porque o resultado apresentava uma característica desfavorável.

Esse procedimento permite que limitações dos algoritmos permaneçam visíveis nos dados.

---

## Ausência de seleção manual

Não foi empregado um procedimento de escolha das melhores dungeons.

As 30 seeds definidas para cada algoritmo formavam a bateria experimental independentemente da qualidade aparente da estrutura produzida.

Os resultados não eram substituídos por novas seeds com o objetivo de melhorar médias ou frequências.

Inspeções visuais foram utilizadas como apoio à interpretação dos ambientes, mas não como critério para excluir gerações da amostra.

---

## Tratamento específico de falhas no WFC

O Wave Function Collapse possui um comportamento particular devido à possibilidade de contradições durante o processo de colapso.

Uma tentativa podia falhar quando as restrições de compatibilidade deixavam uma região sem opções válidas.

Nessa situação, o WFC podia reiniciar internamente o processo.

Na configuração experimental final:

`Maximum Collapse Restarts = 40`

Esses reinícios faziam parte da mesma execução experimental e não eram contabilizados como novas seeds.

Cada reinício utilizava uma sequência pseudoaleatória derivada da seed experimental original.

---

## Ocupação mínima no WFC

A configuração experimental do WFC também estabelecia:

`Minimum Occupied Cells for Accepted Collapse = 600`

Quando uma tentativa produzia uma solução válida com ocupação suficiente, ela era aceita.

Caso as tentativas não atingissem o mínimo, mas pelo menos um colapso válido tivesse sido produzido, a implementação podia preservar o melhor resultado encontrado.

Caso todas as tentativas terminassem em contradição, uma estrutura vazia poderia ser retornada.

Esse comportamento mantinha a limitação observável nas métricas em vez de substituir silenciosamente a seed por outra geração.

---

## Diferença entre reinício interno e repetição experimental

É importante distinguir:

### Reinício interno

Faz parte do funcionamento do algoritmo.

Exemplo:

- uma tentativa de WFC encontra uma contradição;
- o algoritmo reinicia o colapso internamente;
- a seed experimental continua sendo a mesma;
- continua sendo uma única execução da bateria.

### Nova execução experimental

Representa uma nova seed do conjunto:

- seed 2000;
- seed 2001;
- seed 2002;
- etc.

As tentativas internas do WFC não aumentam o número de execuções formais do experimento.

---

## Resultados booleanos

Para os parâmetros booleanos, o sistema registrava a ocorrência da característica em cada execução.

Ao final da bateria, o resultado era apresentado no formato:

`x/30`

Exemplos:

- loops presentes em 30/30 execuções;
- conectores verticais presentes em 2/30 execuções;
- determinada característica ausente em 0/30 execuções.

Esse procedimento preserva a frequência observada em vez de reduzir o resultado apenas a "sim" ou "não".

---

## Resultados quantitativos

Para as métricas quantitativas, o sistema armazenava os valores individuais das 30 execuções.

Na consolidação eram calculados:

- média;
- valor mínimo;
- valor máximo.

Esses valores formam a base das tabelas comparativas da dissertação.

---

## Estimativas qualitativas

As pontuações qualitativas não foram atribuídas manualmente.

Elas foram calculadas automaticamente por regras heurísticas utilizando as métricas estruturais.

As pontuações individuais variam de 1 a 5.

Os critérios incluem:

- replayability;
- facilidade de depuração;
- fluxo;
- legibilidade;
- variedade estrutural.

Essas pontuações representam indicadores indiretos e não avaliações realizadas por participantes.

As definições metodológicas correspondentes são discutidas na dissertação.

---

## Volume navegável

O parâmetro de volume navegável presente nos relatórios é uma estimativa baseada na estrutura lógica.

Ele não corresponde a uma validação física realizada por NavMesh.

Consequentemente, o valor deve ser interpretado como uma indicação de ocupação logicamente considerada navegável pelo sistema experimental.

Não demonstra que todas essas áreas seriam percorríveis por qualquer personagem, agente ou configuração física de jogo.

---

## Consolidação final

Após as 30 execuções, o sistema realizava:

1. contagem das topologias únicas;
2. cálculo da diversidade topológica;
3. aplicação das heurísticas qualitativas;
4. cálculo das médias, mínimos e máximos;
5. contagem das frequências booleanas;
6. criação do resumo agregado;
7. exportação dos relatórios.

A última dungeon podia ser instanciada novamente após a consolidação para inspeção visual.

Essa instanciação posterior não era adicionada às 30 execuções da bateria.

---

## Arquivos exportados

Cada bateria podia produzir:

- relatório completo em JSON;
- parâmetros individuais em CSV;
- resultados agregados em CSV;
- relatório legível em Markdown.

Os conjuntos utilizados na dissertação encontram-se em:

`../Data/`

O arquivo `../Data/README.md` identifica exatamente quais baterias correspondem aos resultados finais.

---

## Sequência resumida do experimento

| Etapa | Procedimento |
|---:|---|
| 1 | Configurar o gerador |
| 2 | Iniciar a bateria pelo Inspector |
| 3 | Verificar a reprodutibilidade da primeira seed |
| 4 | Executar seed 2000 |
| 5 | Gerar o layout lógico |
| 6 | Instanciar a geometria quando habilitado |
| 7 | Calcular as métricas |
| 8 | Gerar o identificador topológico |
| 9 | Registrar os resultados |
| 10 | Repetir até a seed 2029 |
| 11 | Calcular diversidade topológica |
| 12 | Aplicar estimativas qualitativas |
| 13 | Consolidar médias e frequências |
| 14 | Exportar os relatórios |
| 15 | Instanciar a última dungeon para inspeção, quando configurado |

---

## Documentação relacionada

Consulte também:

- `../Configuration/AlgorithmParameters.md` — parâmetros específicos dos algoritmos;
- `../Configuration/AssetConfiguration.md` — biblioteca tridimensional;
- `../Configuration/ExperimentalConfiguration.md` — configuração geral;
- `../Configuration/HardwareAndSoftware.md` — ambiente de execução;
- `../Data/README.md` — identificação dos conjuntos de dados finais;
- `../Seeds/` — conjunto de seeds experimentais.

---

<a id="english-version"></a>

# Experimental Protocol

[![Versão em português](https://img.shields.io/badge/Vers%C3%A3o_em_portugu%C3%AAs-Voltar-1F883D?style=for-the-badge)](#portuguese-version)

This document describes the protocol used to execute, measure, and record the experimental batches of the dissertation.

Algorithm-specific settings are documented in `../Configuration/AlgorithmParameters.md`, while shared experimental conditions are documented in `../Configuration/ExperimentalConfiguration.md`.

The datasets resulting from the final experimental batches are available in `../Data/`.

---

## Overview

The experiment compares six procedural dungeon generation techniques:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

Each algorithm was submitted to 30 runs using seeds `2000` through `2029`.

The main experimental set therefore contains:

`6 algorithms × 30 runs = 180 generations`

Generation, metric collection, topological identification, and result aggregation were performed automatically by the systems implemented in Unity.

---

## Execution environment

Tests were performed:

- directly inside the Unity Editor;
- outside Play Mode;
- without using a compiled build;
- through custom buttons available in each generator's Inspector.

The complete hardware and software environment is documented in:

`../Configuration/HardwareAndSoftware.md`

---

## Scene preparation

Before running the batches, all six generators were present in the same experimental scene.

Each component used:

- its corresponding algorithm configuration;
- the same `DungeonAssetLibrary`;
- the same general generation dimensions;
- the same common enemy, loot, and trap budgets;
- the same experimental seed set.

`Clear Before Generate` remained enabled to prevent objects belonging to a previous generation from remaining in the scene.

---

## Starting a batch

A batch was started using the measurement button available in the Inspector of the corresponding generator.

Depending on the algorithm, the button was named:

`Run Measurement Test`

or a method-specific variation containing the name of the algorithm.

After activation, the following stages were performed automatically.

---

## Reproducibility verification

Before the 30 main runs, the system verified the reproducibility of the first experimental seed.

Seed `2000` was generated twice by the same algorithm.

A topological identifier was calculated for each generated structure.

If the two identifiers matched, seed reproducibility was considered successful.

These verification generations were not included among the 30 comparative runs.

---

## Seed sequence

The batch used:

`Test First Seed = 2000`

For every run, the seed was automatically defined as:

`runSeed = testFirstSeed + i`

Therefore:

| Run | Seed |
|---:|---:|
| 1 | 2000 |
| 2 | 2001 |
| 3 | 2002 |
| ... | ... |
| 30 | 2029 |

The same interval was used for all six algorithms.

---

## Per-run procedure

For each seed, the system followed this general sequence:

1. prepare the internal generator state;
2. apply the corresponding seed;
3. construct the logical dungeon layout;
4. establish rooms or regions and their connections;
5. assign start and goal;
6. instantiate 3D geometry when visual measurement was enabled;
7. calculate structural metrics;
8. record boolean parameters;
9. calculate performance measurements;
10. generate the topological identifier;
11. store the result of that run.

The system then automatically proceeded to the next seed.

---

## Topological identifier

Each generated dungeon received an identifier calculated from its structure.

The identifier considered information including:

- region dimensions;
- region positions;
- floor;
- relationships between rooms or regions;
- vertical connections;
- connections identified as loops.

This value allowed structures from different runs to be compared without relying only on visual inspection.

It was also used to:

- verify seed reproducibility;
- count unique topologies;
- calculate batch topological diversity.

---

## Topological diversity

During the 30 runs, topological identifiers were stored in a set of unique values.

Batch diversity was calculated as:

`diversity = unique topologies / number of runs`

Since each batch contained 30 runs:

`diversity = unique topologies / 30`

This result also served as the basis for the automated replayability estimate.

Replayability documented in this project therefore represents structural diversity between seeds and not a direct evaluation of players' intention to replay the experience.

---

## Performance measurement

Execution times were measured using `System.Diagnostics.Stopwatch`.

The procedure separated different stages so that the logical cost of the algorithm would not be confused with the cost of its 3D representation.

### Logical generation time

Represents the period required for the algorithm to construct its procedural layout and structural relationships.

This value is the component most directly associated with the computational cost of the generation technique.

### Visual instantiation time

Represents the time used by Unity to create the 3D objects that materialize the layout.

This stage may include:

- floors;
- walls;
- doors;
- stairs;
- decorative objects;
- additional dungeon elements.

Visual cost is reported separately because it depends not only on the algorithm but also on the number of instantiated objects and the Unity environment.

### Metric calculation time

Represents the time required by the instrumentation system to analyze the generated layout and calculate the indicators used in the research.

This stage is specific to the experiment and is not an essential part of a procedural generator in a conventional game.

### Total time

When all stages were measured together, total time included:

`logical generation + visual instantiation + metric calculation`

Times are recorded in milliseconds.

---

## Managed memory

The system also recorded an estimate of managed-memory variation.

Memory was inspected before and after generation using the managed-memory mechanism available in the .NET environment used by Unity.

This value was retained as a complementary indicator.

It should not be interpreted as absolute algorithm memory consumption because it may be affected by:

- garbage collection;
- previous Editor state;
- temporary objects;
- internal Unity behavior.

---

## Visual measurement

For the visual-performance results used in the dissertation, the option:

`Measure Visual Instantiation In Tests`

was used to record the cost of instantiating 3D geometry.

For BSP, Cellular Automata, Drunkard's Walk, and Grammar-Based Generation, the final datasets used in the comparison are available in their respective selected batches under `../Data/`.

Room Graph and WFC require an additional methodological note.

---

## Complementary Room Graph procedure

The preserved Room Graph data include two batches.

### Structural batch

Timestamp:

`20260502_190613`

This batch is the reference for:

- structural metrics;
- boolean parameters;
- qualitative estimates;
- logical generation time;
- metric calculation.

### Visual-instantiation batch

Timestamp:

`20260519_183913`

This batch was used to obtain the 3D instantiation cost.

Both batches used seeds `2000–2029`.

During data auditing, the 30 topological identifiers were compared between the batches and matched seed by seed.

The complementary batch therefore reproduced the same topologies used in the structural collection.

When Room Graph performance is presented jointly in the dissertation, the components should be interpreted as a composition of the corresponding averages rather than a single continuous stopwatch measurement from one batch.

---

## Complementary WFC procedure

WFC also contains two preserved batches.

### Structural batch

Timestamp:

`20260502_204944`

This batch is the reference for:

- structural metrics;
- boolean parameters;
- qualitative estimates;
- logical generation time;
- metric calculation.

### Visual-instantiation batch

Timestamp:

`20260519_184822`

This batch was used to obtain the 3D instantiation cost.

Both batches use the same seeds `2000–2029`.

The topological identifiers of all 30 runs also matched seed by seed.

The complementary visual measurement therefore represents the same structures used in the logical analysis.

---

## Handling disconnected results

Disconnected maps were not automatically considered invalid.

Connectivity is one of the variables analyzed in the experiment.

Therefore, a generation containing:

- isolated regions;
- reduced connectivity;
- no loops;
- no verticality;
- limited branching;

remained part of the experimental batch.

A new seed was not used simply because a result exhibited an unfavorable characteristic.

This procedure ensures that algorithm limitations remain visible in the data.

---

## No manual best-result selection

No best-dungeon selection procedure was applied.

The 30 seeds assigned to each algorithm formed the experimental batch regardless of the apparent quality of the generated structure.

Results were not replaced by new seeds to improve averages or frequencies.

Visual inspections supported interpretation of the generated environments but were not used as criteria for excluding runs from the sample.

---

## WFC-specific failure handling

Wave Function Collapse has a particular behavior because contradictions may occur during the collapse process.

An attempt could fail when compatibility constraints left a region with no valid possibilities.

In this situation, WFC could internally restart the process.

The final experimental configuration used:

`Maximum Collapse Restarts = 40`

These restarts belonged to the same experimental run and were not counted as new seeds.

Each internal restart used a pseudorandom sequence derived from the original experimental seed.

---

## Minimum WFC occupation

The experimental WFC configuration also used:

`Minimum Occupied Cells for Accepted Collapse = 600`

When an attempt produced a valid solution with sufficient occupation, it was accepted.

If no attempt reached the minimum but at least one valid collapse had been produced, the implementation could preserve the best available result.

If all attempts ended in contradiction, an empty structure could be returned.

This behavior kept the limitation visible in the metrics instead of silently replacing the experimental seed.

---

## Internal restart versus experimental repetition

It is important to distinguish between:

### Internal restart

Part of the algorithm's behavior.

Example:

- a WFC attempt encounters a contradiction;
- the algorithm internally restarts the collapse;
- the experimental seed remains unchanged;
- it remains one experimental batch run.

### New experimental run

Represents another seed in the set:

- seed 2000;
- seed 2001;
- seed 2002;
- etc.

Internal WFC attempts do not increase the number of formal experimental runs.

---

## Boolean results

For boolean parameters, the system recorded whether the characteristic occurred in each run.

At the end of the batch, the result was represented as:

`x/30`

Examples:

- loops in 30/30 runs;
- vertical connectors in 2/30 runs;
- a feature absent in 0/30 runs.

This procedure preserves observed frequency instead of reducing the result to a simple yes/no classification.

---

## Quantitative results

For quantitative metrics, the system stored individual values from all 30 runs.

During aggregation, the following were calculated:

- average;
- minimum;
- maximum.

These values form the basis of the dissertation's comparative tables.

---

## Qualitative estimates

Qualitative scores were not manually assigned.

They were automatically calculated by heuristic rules using structural metrics.

Individual scores range from 1 to 5.

Criteria include:

- replayability;
- debuggability;
- flow;
- legibility;
- structural variety.

These scores represent indirect indicators rather than evaluations performed by participants.

Their methodological definitions are discussed in the dissertation.

---

## Navigable volume

The navigable-volume parameter contained in the reports is an estimate based on the logical structure.

It does not correspond to physical NavMesh validation.

The value should therefore be interpreted as an indication of occupation considered logically navigable by the experimental system.

It does not demonstrate that all such areas would be traversable by any game character, agent, or physical configuration.

---

## Final aggregation

After all 30 runs, the system performed:

1. unique-topology counting;
2. topological-diversity calculation;
3. application of qualitative heuristics;
4. calculation of averages, minima, and maxima;
5. counting of boolean frequencies;
6. creation of the aggregated summary;
7. report export.

The final dungeon could then be instantiated again for visual inspection.

This post-batch instantiation was not added to the 30 experimental runs.

---

## Exported files

Each batch could produce:

- a complete JSON report;
- individual parameters in CSV;
- aggregated results in CSV;
- a human-readable Markdown report.

The datasets used in the dissertation are available in:

`../Data/`

`../Data/README.md` identifies exactly which batches correspond to the final results.

---

## Experiment summary

| Stage | Procedure |
|---:|---|
| 1 | Configure generator |
| 2 | Start batch from Inspector |
| 3 | Verify reproducibility of first seed |
| 4 | Execute seed 2000 |
| 5 | Generate logical layout |
| 6 | Instantiate geometry when enabled |
| 7 | Calculate metrics |
| 8 | Generate topological identifier |
| 9 | Store results |
| 10 | Repeat through seed 2029 |
| 11 | Calculate topological diversity |
| 12 | Apply qualitative estimates |
| 13 | Aggregate averages and frequencies |
| 14 | Export reports |
| 15 | Instantiate final dungeon for inspection when configured |

---

## Related documentation

See also:

- `../Configuration/AlgorithmParameters.md` — algorithm-specific parameters;
- `../Configuration/AssetConfiguration.md` — 3D asset library;
- `../Configuration/ExperimentalConfiguration.md` — general experimental configuration;
- `../Configuration/HardwareAndSoftware.md` — execution environment;
- `../Data/README.md` — identification of final datasets;
- `../Seeds/` — experimental seed set.
