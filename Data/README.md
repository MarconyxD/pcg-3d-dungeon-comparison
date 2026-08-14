<a id="portuguese-version"></a>

# Dados Experimentais

[![English version](https://img.shields.io/badge/English_version-Click_here-0969DA?style=for-the-badge)](#english-version)

Esta pasta contém os dados experimentais selecionados que sustentam os resultados apresentados na dissertação sobre a comparação entre técnicas de geração procedural de dungeons tridimensionais.

O conteúdo foi extraído dos relatórios produzidos automaticamente pelos sistemas de instrumentação implementados na Unity.

A pasta original `PCGMetrics` também continha gerações manuais, testes intermediários, baterias preliminares, mapas e arquivos produzidos durante o desenvolvimento. Esses materiais não fazem parte deste conjunto de dados publicado.

Foram preservadas aqui apenas as baterias identificadas como correspondentes aos resultados utilizados na versão final da análise.

---

## Estrutura da pasta

A organização dos dados segue os seis algoritmos avaliados:

    Data/
    ├── README.md
    ├── BSP/
    ├── CellularAutomata/
    ├── DrunkardsWalk/
    ├── GrammarBased/
    ├── RoomGraph/
    └── WFC/

Cada subpasta contém os arquivos gerados para a bateria experimental correspondente.

---

## Conjunto experimental

O conjunto principal utiliza:

| Elemento | Configuração |
|---|---:|
| Algoritmos | 6 |
| Execuções por algoritmo | 30 |
| Seeds por algoritmo | 30 |
| Intervalo de seeds | 2000–2029 |
| Total de gerações principais | 180 |

As mesmas 30 seeds foram utilizadas pelos seis algoritmos.

A utilização da mesma seed em algoritmos diferentes não implica a geração da mesma dungeon, pois cada técnica utiliza seus próprios procedimentos e consome a sequência pseudoaleatória de maneira distinta.

O conjunto comum permite, entretanto, manter a identificação das execuções uniforme entre os métodos.

---

## Formatos disponíveis

Para cada bateria experimental foram preservados quatro tipos principais de arquivos.

| Formato | Conteúdo |
|---|---|
| `*_parameter_report_*.json` | Relatório completo da bateria, contendo as execuções individuais, métricas, parâmetros, hashes topológicos e informações agregadas |
| `*_parameters_by_run_*.csv` | Parâmetros organizados por execução e seed, adequado para análise em planilhas, Python, R ou outras ferramentas |
| `*_aggregate_*.csv` | Resultados consolidados da bateria, incluindo médias, mínimos, máximos e frequências |
| `*_readable_report_*.md` | Relatório textual legível gerado automaticamente pelo sistema de instrumentação |

Os arquivos mantêm seus nomes e timestamps originais para preservar a rastreabilidade entre os registros produzidos durante o experimento.

O arquivo JSON representa a forma mais completa do relatório experimental. Os arquivos CSV facilitam análises externas e o relatório Markdown fornece uma representação de leitura direta do mesmo conjunto de informações.

---

## Binary Space Partitioning (BSP)

A bateria utilizada para os resultados finais do BSP é identificada pelo timestamp:

`20260501_213318`

Arquivos:

- `bsp_parameter_report_20260501_213318.json`
- `bsp_parameters_by_run_20260501_213318.csv`
- `bsp_aggregate_20260501_213318.csv`
- `bsp_readable_report_20260501_213318.md`

Esta bateria contém as 30 execuções correspondentes às seeds de 2000 a 2029.

---

## Cellular Automata

A bateria utilizada para os resultados finais do Cellular Automata é identificada pelo timestamp:

`20260814_035905`

Arquivos:

- `cellular_automata_parameter_report_20260814_035905.json`
- `cellular_automata_parameters_by_run_20260814_035905.csv`
- `cellular_automata_aggregate_20260814_035905.csv`
- `cellular_automata_readable_report_20260814_035905.md`

Esta bateria contém as 30 execuções correspondentes às seeds de 2000 a 2029 e utiliza a configuração experimental multiandar documentada em `../Configuration/AlgorithmParameters.md`, com dois pavimentos e vizinhança vertical ativada.

---

## Drunkard's Walk

A bateria utilizada para os resultados finais do Drunkard's Walk é identificada pelo timestamp:

`20260503_012255`

Arquivos:

- `drunkard_walk_parameter_report_20260503_012255.json`
- `drunkard_walk_parameters_by_run_20260503_012255.csv`
- `drunkard_walk_aggregate_20260503_012255.csv`
- `drunkard_walk_readable_report_20260503_012255.md`

Esta bateria contém as 30 execuções correspondentes ao conjunto experimental final.

---

## Grammar-Based Generation

A bateria utilizada para os resultados finais do Grammar-Based Generation é identificada pelo timestamp:

`20260503_052720`

Arquivos:

- `grammar_based_parameter_report_20260503_052720.json`
- `grammar_based_parameters_by_run_20260503_052720.csv`
- `grammar_based_aggregate_20260503_052720.csv`
- `grammar_based_readable_report_20260503_052720.md`

Esta bateria contém as execuções empregadas na comparação estrutural, funcional e de desempenho apresentada na dissertação.

---

## Room Graph

O Room Graph possui duas baterias preservadas porque o procedimento utilizado para os resultados finais envolveu uma coleta estrutural e uma coleta complementar de instanciação visual.

### Bateria estrutural

Timestamp:

`20260502_190613`

Arquivos:

- `room_graph_parameter_report_20260502_190613.json`
- `room_graph_parameters_by_run_20260502_190613.csv`
- `room_graph_aggregate_20260502_190613.csv`
- `room_graph_readable_report_20260502_190613.md`

Esta bateria constitui a referência para os resultados estruturais e lógicos utilizados na dissertação.

### Bateria complementar de instanciação visual

Timestamp:

`20260519_183913`

Arquivos:

- `room_graph_parameter_report_20260519_183913.json`
- `room_graph_parameters_by_run_20260519_183913.csv`
- `room_graph_aggregate_20260519_183913.csv`
- `room_graph_readable_report_20260519_183913.md`

A segunda bateria foi utilizada para registrar o custo de instanciação dos elementos tridimensionais.

As duas baterias utilizam as mesmas seeds, de 2000 a 2029.

Durante a auditoria dos dados, os hashes topológicos das 30 execuções foram comparados entre as duas baterias e apresentaram correspondência seed por seed. Dessa forma, a coleta complementar de desempenho visual reproduziu as mesmas topologias utilizadas na bateria estrutural.

Na comparação final da dissertação:

- os valores estruturais e de geração lógica são provenientes da bateria `20260502_190613`;
- o custo de instanciação visual é proveniente da bateria `20260519_183913`.

Quando esses componentes são apresentados conjuntamente, o tempo resultante deve ser interpretado como uma composição das médias das etapas correspondentes, e não como um único cronômetro contínuo pertencente à mesma bateria.

---

## Wave Function Collapse (WFC)

O WFC também possui duas baterias preservadas pelo mesmo motivo metodológico.

### Bateria estrutural

Timestamp:

`20260502_204944`

Arquivos:

- `wfc_parameter_report_20260502_204944.json`
- `wfc_parameters_by_run_20260502_204944.csv`
- `wfc_aggregate_20260502_204944.csv`
- `wfc_readable_report_20260502_204944.md`

Esta bateria constitui a referência para os resultados estruturais e lógicos do WFC.

### Bateria complementar de instanciação visual

Timestamp:

`20260519_184822`

Arquivos:

- `wfc_parameter_report_20260519_184822.json`
- `wfc_parameters_by_run_20260519_184822.csv`
- `wfc_aggregate_20260519_184822.csv`
- `wfc_readable_report_20260519_184822.md`

A segunda bateria foi utilizada para registrar o custo de instanciação da representação tridimensional.

Assim como no Room Graph, as duas baterias utilizam as mesmas seeds de 2000 a 2029.

A auditoria dos relatórios confirmou correspondência dos hashes topológicos das 30 execuções entre as duas baterias. Portanto, a medição visual complementar corresponde às mesmas topologias produzidas na bateria estrutural.

Na comparação final:

- os resultados estruturais e o tempo lógico são provenientes da bateria `20260502_204944`;
- o custo de instanciação visual é proveniente da bateria `20260519_184822`.

A composição desses valores deve ser interpretada segundo o procedimento descrito acima.

---

## Resumo das baterias selecionadas

| Algoritmo | Bateria | Finalidade |
|---|---|---|
| BSP | `20260501_213318` | Bateria experimental final |
| Cellular Automata | 20260814_035905 | Bateria experimental final
| Drunkard's Walk | `20260503_012255` | Bateria experimental final |
| Grammar-Based Generation | `20260503_052720` | Bateria experimental final |
| Room Graph | `20260502_190613` | Dados estruturais e lógicos |
| Room Graph | `20260519_183913` | Instanciação visual complementar |
| WFC | `20260502_204944` | Dados estruturais e lógicos |
| WFC | `20260519_184822` | Instanciação visual complementar |

---

## Dados não incluídos

A pasta original `PCGMetrics` continha outros arquivos produzidos durante o desenvolvimento do sistema.

Eles foram deliberadamente excluídos deste conjunto público quando não contribuíram para os valores finais apresentados na dissertação.

Entre os materiais não incluídos estão:

- gerações manuais individuais;
- arquivos associados à seed manual `12345`;
- baterias preliminares de teste;
- baterias posteriormente substituídas por uma configuração final;
- testes destinados à calibração de parâmetros;
- arquivos produzidos após a coleta original;
- relatórios utilizados apenas durante desenvolvimento e depuração;
- mapas que não fazem parte do conjunto quantitativo final.

Esses arquivos não foram removidos porque apresentassem resultados desfavoráveis.

A seleção foi realizada com base na correspondência entre as baterias experimentais e os valores efetivamente utilizados na análise final da dissertação.

Resultados desfavoráveis existentes dentro das baterias selecionadas, incluindo baixa conectividade, ausência de loops, limitações de verticalidade ou outras características estruturais, permanecem preservados no conjunto de dados.

---

## Resultados desconectados ou desfavoráveis

Não foi aplicado um procedimento de seleção das melhores dungeons.

Cada uma das 30 seeds previstas para uma bateria permaneceu associada à sua execução experimental.

Uma dungeon desconectada, pouco ramificada ou que não apresentasse determinada característica não era automaticamente descartada.

Esses comportamentos constituem parte dos resultados comparativos e são necessários para analisar as limitações das técnicas.

O caso específico dos reinícios internos do WFC faz parte do funcionamento da implementação e é documentado no protocolo experimental. Esses reinícios internos não representam substituição de uma seed experimental por outra.

---

## Dados brutos e resultados agregados

É importante distinguir dois níveis de informação disponíveis nesta pasta.

### Dados por execução

Os arquivos `*_parameters_by_run_*.csv` e os registros individuais presentes nos JSONs permitem analisar cada uma das 30 seeds separadamente.

Eles possibilitam identificar, entre outros elementos:

- seed;
- hash topológico;
- métricas estruturais;
- parâmetros booleanos;
- estimativas qualitativas;
- medidas de desempenho.

### Dados agregados

Os arquivos `*_aggregate_*.csv` apresentam a consolidação da bateria.

Dependendo da natureza do parâmetro, o sistema registra:

- média;
- valor mínimo;
- valor máximo;
- frequência de ocorrência;
- média das pontuações estimadas.

Os valores agregados constituem a principal base das tabelas comparativas apresentadas na dissertação.

---

## Interpretação dos dados

Nem todos os parâmetros possuem a mesma natureza.

Os relatórios distinguem resultados diretamente medidos de estimativas ou características suportadas pela implementação.

Em particular:

- métricas estruturais como conectividade, preenchimento e caminhos são calculadas sobre o layout lógico;
- o volume navegável é uma estimativa lógica e não uma validação física por NavMesh;
- as pontuações de replayability, fluxo, legibilidade e variedade estrutural são estimativas heurísticas automatizadas;
- suporte a inimigos, loot e armadilhas representa compatibilidade com a camada comum de distribuição implementada para o experimento.

As definições e limitações dessas métricas são discutidas na dissertação e na documentação metodológica do repositório.

---

## Documentação relacionada

Para reproduzir ou interpretar corretamente os dados, consulte também:

- `../Configuration/AlgorithmParameters.md` — parâmetros específicos dos seis algoritmos;
- `../Configuration/AssetConfiguration.md` — recursos tridimensionais utilizados;
- `../Configuration/ExperimentalConfiguration.md` — condições comuns das baterias;
- `../Configuration/HardwareAndSoftware.md` — ambiente de hardware, software e renderização;
- `../Documentation/ExperimentProtocol.md` — procedimento de execução, medição, falhas e consolidação;
- `../Seeds/` — conjunto de seeds utilizado no experimento.

---

## Objetivo desta publicação

Este conjunto de dados é disponibilizado para permitir:

- verificação dos resultados apresentados na dissertação;
- reprodução das baterias experimentais;
- realização de novas análises sobre os dados coletados;
- comparação com novas implementações ou algoritmos;
- continuidade da investigação sobre geração procedural de dungeons.

Os timestamps e nomes originais foram mantidos sempre que possível para preservar a relação entre os dados publicados e os relatórios produzidos originalmente pelo sistema experimental.

---

<a id="english-version"></a>

# Experimental Data

[![Versão em português](https://img.shields.io/badge/Vers%C3%A3o_em_portugu%C3%AAs-Voltar-1F883D?style=for-the-badge)](#portuguese-version)

This directory contains the selected experimental data supporting the results reported in the dissertation comparing procedural generation techniques for three-dimensional dungeons.

The contents were extracted from reports automatically produced by the instrumentation systems implemented in Unity.

The original `PCGMetrics` directory also contained manual generations, intermediate tests, preliminary batches, maps, and files produced during development. Those materials are not part of this published dataset.

Only the batches identified as corresponding to the results used in the final dissertation analysis are preserved here.

---

## Directory structure

The data are organized according to the six evaluated algorithms:

    Data/
    ├── README.md
    ├── BSP/
    ├── CellularAutomata/
    ├── DrunkardsWalk/
    ├── GrammarBased/
    ├── RoomGraph/
    └── WFC/

Each subdirectory contains the files generated for the corresponding experimental batch.

---

## Experimental dataset

The main experimental set uses:

| Element | Configuration |
|---|---:|
| Algorithms | 6 |
| Runs per algorithm | 30 |
| Seeds per algorithm | 30 |
| Seed range | 2000–2029 |
| Main generations | 180 |

The same 30 seeds were used for all six algorithms.

Using the same seed across different algorithms does not imply generation of the same dungeon because each technique follows its own procedures and consumes the pseudorandom sequence differently.

The common set nevertheless provides uniform identification of runs across the evaluated methods.

---

## Available formats

Four main file types are preserved for each experimental batch.

| Format | Contents |
|---|---|
| `*_parameter_report_*.json` | Complete batch report containing individual runs, metrics, parameters, topological hashes, and aggregated information |
| `*_parameters_by_run_*.csv` | Parameters organized by run and seed for analysis using spreadsheets, Python, R, or other tools |
| `*_aggregate_*.csv` | Consolidated batch results including averages, minima, maxima, and frequencies |
| `*_readable_report_*.md` | Human-readable report automatically generated by the instrumentation system |

Files retain their original names and timestamps to preserve traceability between the records produced during the experiment.

The JSON file represents the most complete form of the experimental report. CSV files facilitate external analysis, while the Markdown report provides a directly readable representation of the same information.

---

## Binary Space Partitioning (BSP)

The batch used for the final BSP results is identified by:

`20260501_213318`

Files:

- `bsp_parameter_report_20260501_213318.json`
- `bsp_parameters_by_run_20260501_213318.csv`
- `bsp_aggregate_20260501_213318.csv`
- `bsp_readable_report_20260501_213318.md`

This batch contains the 30 runs corresponding to seeds 2000 through 2029.

---

## Cellular Automata

The batch used for the final Cellular Automata results is identified by:

`20260814_035905`

Files:

- `cellular_automata_parameter_report_20260814_035905.json`
- `cellular_automata_parameters_by_run_20260814_035905.csv`
- `cellular_automata_aggregate_20260814_035905.csv`
- `cellular_automata_readable_report_20260814_035905.md`

This batch contains the 30 runs corresponding to seeds 2000 through 2029 and uses the multi-floor experimental configuration documented in `../Configuration/AlgorithmParameters.md`, with two floors and vertical neighborhood rules enabled.

---

## Drunkard's Walk

The batch used for the final Drunkard's Walk results is identified by:

`20260503_012255`

Files:

- `drunkard_walk_parameter_report_20260503_012255.json`
- `drunkard_walk_parameters_by_run_20260503_012255.csv`
- `drunkard_walk_aggregate_20260503_012255.csv`
- `drunkard_walk_readable_report_20260503_012255.md`

This batch contains the 30 runs corresponding to the final experimental set.

---

## Grammar-Based Generation

The batch used for the final Grammar-Based Generation results is identified by:

`20260503_052720`

Files:

- `grammar_based_parameter_report_20260503_052720.json`
- `grammar_based_parameters_by_run_20260503_052720.csv`
- `grammar_based_aggregate_20260503_052720.csv`
- `grammar_based_readable_report_20260503_052720.md`

This batch contains the runs used in the structural, functional, and performance comparison reported in the dissertation.

---

## Room Graph

Room Graph contains two preserved batches because the final procedure involved a structural collection and a complementary visual-instantiation collection.

### Structural batch

Timestamp:

`20260502_190613`

Files:

- `room_graph_parameter_report_20260502_190613.json`
- `room_graph_parameters_by_run_20260502_190613.csv`
- `room_graph_aggregate_20260502_190613.csv`
- `room_graph_readable_report_20260502_190613.md`

This batch is the reference for structural and logical results used in the dissertation.

### Complementary visual-instantiation batch

Timestamp:

`20260519_183913`

Files:

- `room_graph_parameter_report_20260519_183913.json`
- `room_graph_parameters_by_run_20260519_183913.csv`
- `room_graph_aggregate_20260519_183913.csv`
- `room_graph_readable_report_20260519_183913.md`

The second batch was used to record the cost of instantiating the three-dimensional elements.

Both batches use the same seeds, from 2000 through 2029.

During data auditing, the topological hashes of all 30 runs were compared between the two batches and matched seed by seed. The complementary performance collection therefore reproduced the same topologies used in the structural batch.

In the final dissertation comparison:

- structural and logical-generation values come from batch `20260502_190613`;
- visual-instantiation cost comes from batch `20260519_183913`.

When these components are presented together, the resulting time should be understood as a composition of the corresponding stage averages rather than a single continuous stopwatch measurement belonging to one batch.

---

## Wave Function Collapse (WFC)

WFC also contains two preserved batches for the same methodological reason.

### Structural batch

Timestamp:

`20260502_204944`

Files:

- `wfc_parameter_report_20260502_204944.json`
- `wfc_parameters_by_run_20260502_204944.csv`
- `wfc_aggregate_20260502_204944.csv`
- `wfc_readable_report_20260502_204944.md`

This batch is the reference for the structural and logical WFC results.

### Complementary visual-instantiation batch

Timestamp:

`20260519_184822`

Files:

- `wfc_parameter_report_20260519_184822.json`
- `wfc_parameters_by_run_20260519_184822.csv`
- `wfc_aggregate_20260519_184822.csv`
- `wfc_readable_report_20260519_184822.md`

The second batch was used to record the cost of instantiating the three-dimensional representation.

As with Room Graph, both batches use seeds 2000 through 2029.

The audit of the reports confirmed matching topological hashes for all 30 seeds between both batches. Therefore, the complementary visual measurement corresponds to the same topologies produced in the structural batch.

In the final comparison:

- structural results and logical time come from batch `20260502_204944`;
- visual-instantiation cost comes from batch `20260519_184822`.

The composition of these values should be interpreted according to the procedure described above.

---

## Selected batch summary

| Algorithm | Batch | Purpose |
|---|---|---|
| BSP | `20260501_213318` | Final experimental batch |
| Cellular Automata | 20260814_035905 | Final experimental batch
| Drunkard's Walk | `20260503_012255` | Final experimental batch |
| Grammar-Based Generation | `20260503_052720` | Final experimental batch |
| Room Graph | `20260502_190613` | Structural and logical data |
| Room Graph | `20260519_183913` | Complementary visual instantiation |
| WFC | `20260502_204944` | Structural and logical data |
| WFC | `20260519_184822` | Complementary visual instantiation |

---

## Excluded data

The original `PCGMetrics` directory contained additional files produced during system development.

They were deliberately excluded from this public dataset when they did not contribute to the final values reported in the dissertation.

Excluded materials include:

- individual manual generations;
- files associated with the manual seed `12345`;
- preliminary test batches;
- batches later superseded by a final configuration;
- parameter-calibration tests;
- files produced after the original experimental collection;
- reports used only during development and debugging;
- maps not belonging to the final quantitative dataset.

These files were not excluded because they contained unfavorable results.

Selection was based on the correspondence between experimental batches and the values actually used in the final dissertation analysis.

Unfavorable results contained within the selected batches, including low connectivity, absence of loops, limited verticality, or other structural limitations, remain preserved in the dataset.

---

## Disconnected or unfavorable results

No best-dungeon selection procedure was applied.

Each of the 30 seeds assigned to a batch remained associated with its experimental run.

A disconnected, weakly branched, or otherwise limited dungeon was not automatically discarded.

These behaviors are part of the comparative results and are necessary for analyzing the limitations of the techniques.

The specific case of internal WFC restarts belongs to the implementation behavior and is documented in the experimental protocol. These internal attempts do not represent replacement of one experimental seed by another.

---

## Raw and aggregated data

Two levels of information are available in this directory.

### Per-run data

The `*_parameters_by_run_*.csv` files and the individual records contained in the JSON reports allow each of the 30 seeds to be analyzed separately.

They include, among other information:

- seed;
- topological hash;
- structural metrics;
- boolean parameters;
- qualitative estimates;
- performance measurements.

### Aggregated data

The `*_aggregate_*.csv` files contain the consolidated batch results.

Depending on parameter type, the system records:

- average;
- minimum;
- maximum;
- occurrence frequency;
- average estimated score.

Aggregated values form the primary basis of the comparative tables reported in the dissertation.

---

## Data interpretation

Not all parameters have the same methodological nature.

The reports distinguish directly measured results from estimates or implementation-supported features.

In particular:

- structural metrics such as connectivity, fill percentage, and path measures are calculated on the logical layout;
- navigable volume is a logical estimate rather than a physical NavMesh validation;
- replayability, flow, legibility, and structural-variety scores are automated heuristic estimates;
- enemy, loot, and trap support represents compatibility with the common distribution layer implemented for the experiment.

Definitions and limitations of these metrics are discussed in the dissertation and in the methodological documentation included in this repository.

---

## Related documentation

For correct reproduction and interpretation of these data, see also:

- `../Configuration/AlgorithmParameters.md` — specific parameters for all six algorithms;
- `../Configuration/AssetConfiguration.md` — 3D resources used in the experiment;
- `../Configuration/ExperimentalConfiguration.md` — common batch conditions;
- `../Configuration/HardwareAndSoftware.md` — hardware, software, and rendering environment;
- `../Documentation/ExperimentProtocol.md` — execution, measurement, failure handling, and aggregation procedures;
- `../Seeds/` — seed set used in the experiment.

---

## Purpose of publication

This dataset is published to support:

- verification of results reported in the dissertation;
- reproduction of experimental batches;
- additional analyses of the collected data;
- comparison with new implementations or algorithms;
- continued research on procedural dungeon generation.

Original timestamps and filenames have been preserved whenever possible to maintain traceability between the published data and the reports originally produced by the experimental system.
