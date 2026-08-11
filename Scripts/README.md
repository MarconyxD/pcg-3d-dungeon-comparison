<a id="portuguese-version"></a>

# Scripts da Implementação Experimental

[![English version](https://img.shields.io/badge/English_version-Click_here-0969DA?style=for-the-badge)](#english-version)

Esta pasta contém os scripts C# utilizados na implementação experimental da dissertação sobre geração procedural de dungeons tridimensionais.

O código disponibilizado corresponde à versão empregada para implementar, executar, medir e registrar os seis algoritmos analisados no estudo:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

Além dos geradores, são incluídos os componentes compartilhados responsáveis pela representação lógica das dungeons, biblioteca de recursos tridimensionais, cálculo de métricas, geração de relatórios, exportação de mapas e interfaces personalizadas do Inspector da Unity.

---

## Objetivo desta publicação

Os scripts são disponibilizados principalmente para:

- permitir a reprodução dos experimentos descritos na dissertação;
- documentar a implementação utilizada para gerar os dados publicados;
- permitir a inspeção das métricas e procedimentos de instrumentação;
- facilitar novas análises e comparações com outras técnicas de geração procedural;
- servir como referência para futuras extensões do estudo.

Esta pasta deve ser interpretada como um **snapshot acadêmico da implementação experimental** utilizada durante a investigação.

O objetivo não é representar uma biblioteca comercial finalizada ou uma arquitetura otimizada para produção.

---

## Versão da Unity

A implementação foi desenvolvida e utilizada com:

| Elemento | Configuração |
|---|---|
| Game engine | Unity |
| Versão | `6000.3.0f1` |
| Linguagem | C# |
| Namespace principal | `Dissertation.PCG` |
| Plataforma experimental | Windows Standalone |
| Ambiente de execução dos testes | Unity Editor, fora do Play Mode |

As demais informações sobre hardware, software e configuração gráfica estão disponíveis em:

`../Configuration/HardwareAndSoftware.md`

---

## Estrutura da pasta

A organização recomendada dos scripts é:

    Scripts/
    ├── README.md
    │
    ├── Runtime/
    │   ├── Generators/
    │   │   ├── BSPDungeonGenerator.cs
    │   │   ├── CellularAutomataDungeonGenerator.cs
    │   │   ├── DrunkardWalkDungeonGenerator.cs
    │   │   ├── GrammarBasedDungeonGenerator.cs
    │   │   ├── RoomGraphDungeonGenerator.cs
    │   │   └── WFCDungeonGenerator.cs
    │   │
    │   └── Shared/
    │       ├── DungeonAssetLibrary.cs
    │       ├── DungeonLayout.cs
    │       ├── DungeonMap2DExporter.cs
    │       ├── DungeonMetrics.cs
    │       └── DungeonParameterReport.cs
    │
    └── Editor/
        ├── BSPDungeonGeneratorEditor.cs
        ├── CellularAutomataDungeonGeneratorEditor.cs
        ├── DrunkardWalkDungeonGeneratorEditor.cs
        ├── GrammarBasedDungeonGeneratorEditor.cs
        ├── RoomGraphDungeonGeneratorEditor.cs
        └── WFCDungeonGeneratorEditor.cs

---

## Geradores

Os seis arquivos localizados em `Runtime/Generators/` contêm as implementações dos algoritmos avaliados.

### BSPDungeonGenerator.cs

Implementa a variante experimental de Binary Space Partitioning utilizada no estudo.

Entre suas responsabilidades estão:

- subdivisão recursiva do espaço;
- criação e posicionamento das salas;
- geração dos corredores;
- criação de conexões adicionais para loops;
- geração de múltiplos pavimentos;
- criação de conectores verticais;
- instanciação da representação tridimensional;
- execução das baterias automatizadas;
- geração e exportação das métricas.

Os parâmetros efetivamente utilizados durante a bateria experimental estão documentados em:

`../Configuration/AlgorithmParameters.md`

---

### CellularAutomataDungeonGenerator.cs

Implementa a técnica de Cellular Automata utilizada para geração de regiões semelhantes a cavernas.

Entre suas funções estão:

- inicialização probabilística das células;
- aplicação das regras locais de vizinhança;
- execução das iterações de suavização;
- identificação de regiões abertas;
- interpretação de salas e corredores;
- construção da representação lógica;
- instanciação tridimensional;
- execução das baterias experimentais.

A configuração final utilizada no estudo foi de pavimento único.

---

### DrunkardWalkDungeonGenerator.cs

Implementa a técnica Drunkard's Walk por meio de múltiplos caminhantes.

A implementação inclui:

- deslocamento pseudoaleatório;
- controle da mudança de direção;
- escavação progressiva do espaço;
- reinício de ramificações em regiões já abertas;
- criação ocasional de áreas ampliadas;
- suporte experimental à verticalidade;
- interpretação das regiões geradas;
- instanciação tridimensional;
- execução e registro das baterias de teste.

---

### GrammarBasedDungeonGenerator.cs

Implementa uma abordagem de geração baseada em gramática.

A estrutura utiliza regras destinadas à criação de:

- caminho principal;
- ramificações;
- salas comuns;
- áreas de tesouro;
- regiões de armadilha;
- loops e atalhos;
- arena de chefe;
- conexões entre pavimentos.

A implementação também converte a estrutura derivada pela gramática em uma representação espacial tridimensional.

---

### RoomGraphDungeonGenerator.cs

Implementa a geração baseada em um grafo de salas.

Entre as etapas executadas pelo componente estão:

- definição das salas;
- criação das relações entre os nós;
- posicionamento espacial das regiões;
- criação dos corredores;
- adição de conexões extras;
- formação de loops;
- criação de conexões verticais;
- transformação do grafo abstrato em uma dungeon tridimensional.

O Room Graph foi utilizado para investigar principalmente o controle explícito da estrutura e das relações entre regiões.

---

### WFCDungeonGenerator.cs

Implementa a variante experimental de Wave Function Collapse utilizada na investigação.

A implementação inclui:

- definição de módulos;
- regras de compatibilidade;
- pesos de seleção;
- propagação das restrições;
- observações iniciais;
- estrutura inicial conectada;
- módulos de sala e corredor;
- conectores verticais;
- tratamento de contradições;
- reinícios internos do processo de colapso;
- seleção do melhor resultado disponível quando necessário;
- transformação da estrutura colapsada em layout lógico;
- instanciação tridimensional.

O comportamento específico de reinícios e falhas do WFC está documentado em:

`../Documentation/ExperimentProtocol.md`

---

## Scripts compartilhados

Os arquivos localizados em `Runtime/Shared/` são utilizados por múltiplos geradores.

### DungeonAssetLibrary.cs

Define a biblioteca compartilhada de recursos tridimensionais utilizada pelos algoritmos.

O componente mantém referências para:

- pisos;
- paredes;
- portas;
- escadas;
- marcadores;
- objetos decorativos;
- inimigos;
- loot;
- armadilhas.

A configuração utilizada no experimento está documentada em:

`../Configuration/AssetConfiguration.md`

Os assets tridimensionais originais não são incluídos neste repositório.

---

### DungeonLayout.cs

Contém as estruturas de dados compartilhadas utilizadas para representar logicamente uma dungeon.

Essa representação permite que algoritmos com mecanismos internos diferentes sejam posteriormente avaliados através de uma estrutura comum.

Entre os elementos representados encontram-se:

- dimensões gerais;
- pavimentos;
- regiões ou salas;
- conexões;
- relações verticais;
- células ocupadas;
- região inicial;
- região de objetivo.

Essa camada comum é fundamental para permitir que métricas equivalentes sejam calculadas entre diferentes técnicas.

---

### DungeonMap2DExporter.cs

Responsável pela exportação de representações bidimensionais das dungeons.

Os mapas podem ser utilizados para:

- inspeção visual;
- documentação;
- identificação de estruturas;
- comparação entre seeds;
- apoio à interpretação dos resultados.

A exportação de mapas não constitui a fonte das métricas estruturais utilizadas no estudo.

---

### DungeonMetrics.cs

Contém as estruturas e procedimentos responsáveis pelo cálculo das métricas quantitativas e de determinados parâmetros booleanos.

Entre as informações calculadas estão:

- quantidade de salas ou regiões;
- conectividade;
- percentual de preenchimento;
- fator de ramificação;
- variação vertical;
- comprimento médio dos caminhos;
- caminho crítico;
- caminhos alternativos;
- quantidade de módulos;
- estimativa lógica de volume navegável;
- presença de loops;
- conectores verticais;
- suporte a múltiplos pavimentos;
- identificação de regiões compatíveis com arena de chefe.

As medidas relacionadas a caminhos e conectividade são calculadas sobre a representação lógica da dungeon.

O parâmetro de volume navegável presente no código **não corresponde a uma validação física por NavMesh** e deve ser interpretado como uma estimativa lógica.

---

### DungeonParameterReport.cs

Contém parte importante da instrumentação utilizada para organizar, interpretar, agregar e exportar os resultados experimentais.

Entre suas responsabilidades estão:

- organização dos parâmetros quantitativos;
- organização dos parâmetros booleanos;
- aplicação das estimativas qualitativas automatizadas;
- geração dos hashes topológicos;
- consolidação das 30 execuções;
- cálculo de médias;
- cálculo de valores mínimos e máximos;
- contagem das frequências booleanas;
- geração dos relatórios experimentais.

O arquivo também contém as heurísticas utilizadas para as estimativas de:

- replayability;
- facilidade de depuração;
- fluxo;
- legibilidade;
- variedade estrutural.

Essas pontuações não devem ser interpretadas como avaliações realizadas por participantes.

A metodologia completa dessas estimativas é discutida na dissertação e no protocolo experimental.

---

## Scripts do Editor

Os arquivos presentes em `Editor/` implementam Inspetores personalizados para os seis geradores.

Eles fornecem os botões utilizados durante o desenvolvimento e a coleta experimental, como:

- geração manual da dungeon;
- execução das baterias de medição;
- exportação dos mapas;
- limpeza da dungeon gerada;
- aplicação de presets quando disponíveis.

Esses arquivos utilizam a API `UnityEditor`.

Por esse motivo, **devem permanecer dentro de uma pasta denominada `Editor` em um projeto Unity**.

Eles não devem ser colocados junto aos scripts de runtime em uma pasta comum destinada à compilação final do jogo.

---

## Como adicionar os scripts a um projeto Unity

Uma organização possível dentro de um projeto é:

    Assets/
    └── DissertationPCG/
        ├── Runtime/
        │   ├── Generators/
        │   └── Shared/
        │
        └── Editor/

Copie:

- `Scripts/Runtime/Generators/` para `Assets/DissertationPCG/Runtime/Generators/`;
- `Scripts/Runtime/Shared/` para `Assets/DissertationPCG/Runtime/Shared/`;
- `Scripts/Editor/` para `Assets/DissertationPCG/Editor/`.

Em seguida, configure na cena os componentes correspondentes aos algoritmos que deseja reproduzir.

---

## Configuração necessária

Os scripts, isoladamente, não reproduzem automaticamente o experimento apenas ao serem copiados para um novo projeto.

Para reproduzir as condições utilizadas na investigação, também é necessário consultar:

- `../Configuration/AlgorithmParameters.md`;
- `../Configuration/AssetConfiguration.md`;
- `../Configuration/ExperimentalConfiguration.md`;
- `../Configuration/HardwareAndSoftware.md`;
- `../Documentation/ExperimentProtocol.md`;
- `../Seeds/`;
- `../Data/`.

Esses arquivos documentam os valores serializados no Inspector utilizados durante as baterias finais.

---

## Valores padrão do código e configuração experimental

Alguns valores declarados diretamente nos scripts diferem dos valores efetivamente utilizados durante os experimentos.

Isso ocorre porque os componentes da Unity armazenam valores serializados no Inspector, que podem sobrescrever os valores padrão definidos no código.

Por exemplo, um campo pode possuir determinado valor inicial declarado no script e ter sido posteriormente configurado com outro valor na cena experimental.

Por esse motivo:

**os valores padrão presentes no código não devem ser interpretados como a configuração experimental final.**

A referência para reprodução das baterias utilizadas na dissertação é:

`../Configuration/AlgorithmParameters.md`

Esse documento contém os valores efetivamente configurados no Inspector para os experimentos finais.

---

## Preservação do código experimental

Os scripts são publicados preservando a implementação utilizada na investigação.

Não foram realizadas alterações posteriores com o objetivo de:

- refatorar a arquitetura;
- alterar fórmulas;
- modificar as heurísticas;
- atualizar os parâmetros padrão para coincidir artificialmente com o Inspector;
- traduzir integralmente comentários e tooltips;
- alterar os procedimentos utilizados para gerar os dados publicados.

Essa decisão busca manter correspondência entre:

`código → configuração → execução → dados → resultados da dissertação`

Melhorias técnicas futuras poderão ser desenvolvidas separadamente, mas não devem substituir silenciosamente a versão utilizada para produzir o conjunto experimental original.

---

## Idioma do código

Os scripts preservam o idioma utilizado durante o desenvolvimento original.

Por esse motivo, alguns:

- comentários;
- tooltips;
- mensagens de log;
- descrições internas;

permanecem em português.

A documentação principal deste repositório é fornecida em português e inglês.

A preservação do idioma original do código evita modificações desnecessárias na versão experimental utilizada para produzir os resultados.

---

## Assets externos

Os modelos tridimensionais utilizados durante os experimentos são provenientes do pacote KayKit utilizado no projeto.

Os arquivos dos modelos não são incluídos nesta pasta.

A origem, os nomes dos prefabs e a configuração da biblioteca experimental são documentados em:

`../Configuration/AssetConfiguration.md`

Página oficial do pacote utilizado:

[KayKit - Dungeon Pack — Kay Lousberg](https://kaylousberg.itch.io/kaykit-dungeon-pack)

---

## Arquivos .meta

Os arquivos `.meta` gerados pela Unity não são incluídos neste conjunto de scripts.

Este repositório distribui o código-fonte e a documentação necessários para reprodução metodológica, e não uma cópia integral do projeto Unity original.

Ao importar os scripts em um novo projeto, a Unity poderá gerar novos arquivos `.meta`.

Caso uma versão futura do repositório disponibilize cenas, prefabs ou ScriptableObjects com referências serializadas dependentes de GUIDs específicos, a preservação dos respectivos arquivos `.meta` deverá ser considerada.

---

## Relação com os dados experimentais

Os dados produzidos pela implementação encontram-se em:

`../Data/`

O arquivo:

`../Data/README.md`

identifica as baterias correspondentes aos resultados finais da dissertação.

Os scripts disponibilizados nesta pasta fornecem a implementação responsável pela geração e instrumentação desses dados.

---

## Limitações importantes

Ao utilizar este código, devem ser consideradas algumas características da implementação experimental:

- o volume navegável é uma estimativa lógica e não uma validação NavMesh;
- as estimativas qualitativas são heurísticas automatizadas;
- a facilidade de depuração recebeu valor fixo na instrumentação original;
- os tempos de desempenho foram obtidos no Unity Editor;
- os resultados dependem das configurações documentadas no repositório;
- diferentes versões da Unity ou diferentes hardwares podem produzir tempos absolutos distintos;
- o código representa uma implementação experimental e não um framework genérico de PCG.

Essas limitações são documentadas de forma mais detalhada na dissertação.

---

## Licença

Os scripts originais disponibilizados neste repositório estão sujeitos à licença definida no arquivo `LICENSE` ou `LICENSE.txt` localizado na raiz do repositório.

Os recursos externos, incluindo os assets KayKit, permanecem sujeitos às licenças definidas por seus respectivos autores.

---

## Documentação relacionada

Para reprodução completa do experimento, consulte:

- `../Configuration/AlgorithmParameters.md` — parâmetros finais dos algoritmos;
- `../Configuration/AssetConfiguration.md` — biblioteca de recursos tridimensionais;
- `../Configuration/ExperimentalConfiguration.md` — condições comuns das baterias;
- `../Configuration/HardwareAndSoftware.md` — ambiente experimental;
- `../Documentation/ExperimentProtocol.md` — protocolo de execução e medição;
- `../Data/README.md` — identificação dos dados finais;
- `../Seeds/` — conjunto de seeds utilizado.

---

<a id="english-version"></a>

# Experimental Implementation Scripts

[![Versão em português](https://img.shields.io/badge/Vers%C3%A3o_em_portugu%C3%AAs-Voltar-1F883D?style=for-the-badge)](#portuguese-version)

This directory contains the C# scripts used in the experimental implementation of the dissertation on procedural generation of three-dimensional dungeons.

The published code corresponds to the implementation used to develop, execute, measure, and record the six algorithms analyzed in the study:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

In addition to the generators, the repository includes shared components responsible for logical dungeon representation, 3D asset management, metric calculation, report generation, map export, and custom Unity Inspector interfaces.

---

## Purpose of this publication

The scripts are primarily published to:

- support reproduction of the experiments described in the dissertation;
- document the implementation used to generate the published data;
- allow inspection of the metrics and instrumentation procedures;
- facilitate new analyses and comparisons with other procedural generation techniques;
- provide a reference for future extensions of the research.

This directory should be understood as an **academic snapshot of the experimental implementation** used during the research.

It is not intended to represent a production-ready commercial library or an optimized software architecture.

---

## Unity version

The implementation was developed and used with:

| Element | Configuration |
|---|---|
| Game engine | Unity |
| Version | `6000.3.0f1` |
| Language | C# |
| Main namespace | `Dissertation.PCG` |
| Experimental platform | Windows Standalone |
| Test execution environment | Unity Editor, outside Play Mode |

Additional hardware, software, and graphical environment information is available in:

`../Configuration/HardwareAndSoftware.md`

---

## Directory structure

The recommended script organization is:

    Scripts/
    ├── README.md
    │
    ├── Runtime/
    │   ├── Generators/
    │   │   ├── BSPDungeonGenerator.cs
    │   │   ├── CellularAutomataDungeonGenerator.cs
    │   │   ├── DrunkardWalkDungeonGenerator.cs
    │   │   ├── GrammarBasedDungeonGenerator.cs
    │   │   ├── RoomGraphDungeonGenerator.cs
    │   │   └── WFCDungeonGenerator.cs
    │   │
    │   └── Shared/
    │       ├── DungeonAssetLibrary.cs
    │       ├── DungeonLayout.cs
    │       ├── DungeonMap2DExporter.cs
    │       ├── DungeonMetrics.cs
    │       └── DungeonParameterReport.cs
    │
    └── Editor/
        ├── BSPDungeonGeneratorEditor.cs
        ├── CellularAutomataDungeonGeneratorEditor.cs
        ├── DrunkardWalkDungeonGeneratorEditor.cs
        ├── GrammarBasedDungeonGeneratorEditor.cs
        ├── RoomGraphDungeonGeneratorEditor.cs
        └── WFCDungeonGeneratorEditor.cs

---

## Generators

The six files under `Runtime/Generators/` contain the implementations of the evaluated algorithms.

### BSPDungeonGenerator.cs

Implements the experimental Binary Space Partitioning variant used in the study.

Its responsibilities include:

- recursive space partitioning;
- room creation and placement;
- corridor generation;
- additional loop connections;
- multi-floor generation;
- vertical connectors;
- 3D representation instantiation;
- automated experimental batches;
- metric generation and export.

The parameters actually used during the experiment are documented in:

`../Configuration/AlgorithmParameters.md`

---

### CellularAutomataDungeonGenerator.cs

Implements the Cellular Automata technique used to generate cave-like regions.

Its functions include:

- probabilistic cell initialization;
- local neighborhood rules;
- smoothing iterations;
- open-region identification;
- room and corridor interpretation;
- logical structure construction;
- 3D instantiation;
- experimental batch execution.

The final experimental configuration used a single floor.

---

### DrunkardWalkDungeonGenerator.cs

Implements Drunkard's Walk using multiple walkers.

The implementation includes:

- pseudorandom movement;
- direction-change control;
- progressive space carving;
- branch restarts from previously carved regions;
- occasional enlarged-area carving;
- experimental verticality support;
- interpretation of generated regions;
- 3D instantiation;
- experimental batch execution and recording.

---

### GrammarBasedDungeonGenerator.cs

Implements a grammar-based generation approach.

Its rule system can produce:

- a main path;
- branches;
- regular rooms;
- treasure areas;
- trap areas;
- loops and shortcuts;
- a boss arena;
- connections between floors.

The implementation also transforms the grammar-derived structure into a spatial 3D representation.

---

### RoomGraphDungeonGenerator.cs

Implements room-graph-based generation.

Its stages include:

- room definition;
- node relationship generation;
- spatial room placement;
- corridor generation;
- additional connections;
- loop formation;
- vertical connections;
- conversion of the abstract graph into a three-dimensional dungeon.

Room Graph was used primarily to investigate explicit control over structural relationships between regions.

---

### WFCDungeonGenerator.cs

Implements the experimental Wave Function Collapse variant used in the research.

The implementation includes:

- module definition;
- compatibility rules;
- selection weights;
- constraint propagation;
- initial observations;
- connected backbone constraints;
- room and corridor modules;
- vertical connectors;
- contradiction handling;
- internal collapse restarts;
- preservation of the best available result when required;
- conversion of the collapsed representation into a logical layout;
- 3D instantiation.

WFC restart and failure behavior is documented in:

`../Documentation/ExperimentProtocol.md`

---

## Shared scripts

Files under `Runtime/Shared/` are used by multiple generators.

### DungeonAssetLibrary.cs

Defines the shared 3D asset library used by the algorithms.

The component stores references to:

- floors;
- walls;
- doors;
- stairs;
- markers;
- decorative objects;
- enemies;
- loot;
- traps.

The experimental configuration is documented in:

`../Configuration/AssetConfiguration.md`

Original third-party 3D assets are not included in this repository.

---

### DungeonLayout.cs

Contains the shared data structures used to logically represent a dungeon.

This representation allows algorithms with different internal generation mechanisms to be evaluated through a common structure.

Represented information includes:

- overall dimensions;
- floors;
- rooms or regions;
- connections;
- vertical relationships;
- occupied cells;
- start region;
- goal region.

This common layer is essential for calculating comparable metrics across different techniques.

---

### DungeonMap2DExporter.cs

Responsible for exporting two-dimensional representations of generated dungeons.

The maps can support:

- visual inspection;
- documentation;
- structural identification;
- seed comparison;
- interpretation of results.

Map export is not the source of the structural metrics used in the study.

---

### DungeonMetrics.cs

Contains the structures and procedures responsible for calculating quantitative metrics and selected boolean parameters.

Calculated information includes:

- number of rooms or regions;
- connectivity;
- fill percentage;
- branching factor;
- vertical variance;
- average path length;
- critical path;
- alternative paths;
- module count;
- logical navigable-volume estimate;
- loop presence;
- vertical connectors;
- multi-floor support;
- identification of regions compatible with a boss arena.

Path and connectivity measures are calculated from the logical dungeon representation.

The navigable-volume parameter in the code **does not represent physical NavMesh validation** and should be interpreted as a logical estimate.

---

### DungeonParameterReport.cs

Contains an important part of the instrumentation used to organize, interpret, aggregate, and export experimental results.

Its responsibilities include:

- organization of quantitative parameters;
- organization of boolean parameters;
- automated qualitative estimates;
- generation of topological hashes;
- aggregation of 30 runs;
- average calculation;
- minimum and maximum calculation;
- boolean-frequency counting;
- experimental report generation.

The file also contains the heuristics used to estimate:

- replayability;
- debuggability;
- flow;
- legibility;
- structural variety.

These scores should not be interpreted as evaluations performed by human participants.

Their methodology is discussed in the dissertation and experimental protocol.

---

## Editor scripts

Files under `Editor/` implement custom Inspectors for the six generators.

They provide the controls used during development and experimental collection, including:

- manual dungeon generation;
- execution of measurement batches;
- map export;
- generated-dungeon cleanup;
- preset application when available.

These files use the `UnityEditor` API.

Therefore, **they must remain inside a directory named `Editor` when used in a Unity project**.

They should not be placed together with runtime scripts in a standard directory intended for final game compilation.

---

## Adding the scripts to a Unity project

One possible organization inside a Unity project is:

    Assets/
    └── DissertationPCG/
        ├── Runtime/
        │   ├── Generators/
        │   └── Shared/
        │
        └── Editor/

Copy:

- `Scripts/Runtime/Generators/` to `Assets/DissertationPCG/Runtime/Generators/`;
- `Scripts/Runtime/Shared/` to `Assets/DissertationPCG/Runtime/Shared/`;
- `Scripts/Editor/` to `Assets/DissertationPCG/Editor/`.

The corresponding generator components can then be configured in a Unity scene.

---

## Required configuration

The scripts alone do not automatically reproduce the experiment simply by being copied into a new project.

To reproduce the conditions used in the research, also consult:

- `../Configuration/AlgorithmParameters.md`;
- `../Configuration/AssetConfiguration.md`;
- `../Configuration/ExperimentalConfiguration.md`;
- `../Configuration/HardwareAndSoftware.md`;
- `../Documentation/ExperimentProtocol.md`;
- `../Seeds/`;
- `../Data/`.

These files document the Inspector values used during the final experimental batches.

---

## Source defaults and experimental configuration

Some values declared directly in the scripts differ from those actually used during the experiments.

Unity components store serialized Inspector values that can override the default values declared in source code.

Therefore:

**default values found in the source code should not be interpreted as the final experimental configuration.**

The reference configuration for reproduction is:

`../Configuration/AlgorithmParameters.md`

This document contains the values actually configured in the Inspector for the final experimental runs.

---

## Preservation of the experimental implementation

The scripts are published while preserving the implementation used in the research.

No later modifications were introduced solely to:

- refactor the architecture;
- alter formulas;
- modify heuristics;
- change default parameters to artificially match the Inspector;
- fully translate comments or tooltips;
- alter the procedures used to generate the published data.

This decision preserves correspondence between:

`code → configuration → execution → data → dissertation results`

Future technical improvements may be developed separately but should not silently replace the version used to produce the original experimental dataset.

---

## Source-code language

The scripts preserve the language used during original development.

Therefore, some:

- comments;
- tooltips;
- log messages;
- internal descriptions;

remain in Portuguese.

The main repository documentation is provided in both Portuguese and English.

Preserving the original source language avoids unnecessary modifications to the experimental implementation.

---

## External assets

The 3D models used during the experiments are obtained from the KayKit package used in the project.

The model files themselves are not included in this directory.

Their origin, prefab names, and experimental library configuration are documented in:

`../Configuration/AssetConfiguration.md`

Official package page:

[KayKit - Dungeon Pack — Kay Lousberg](https://kaylousberg.itch.io/kaykit-dungeon-pack)

---

## .meta files

Unity-generated `.meta` files are not included in this source-code package.

This repository distributes the source code and methodological documentation required for reproduction rather than a complete copy of the original Unity project.

When imported into a new project, Unity can generate new `.meta` files.

If a future repository version distributes scenes, prefabs, or ScriptableObjects with serialized references dependent on specific GUIDs, preservation of the corresponding `.meta` files should be considered.

---

## Relationship with experimental data

Data produced by this implementation are available under:

`../Data/`

The file:

`../Data/README.md`

identifies the batches corresponding to the final dissertation results.

The scripts in this directory provide the implementation responsible for generating and instrumenting those data.

---

## Important limitations

When using this code, consider the following characteristics of the experimental implementation:

- navigable volume is a logical estimate rather than NavMesh validation;
- qualitative estimates are automated heuristics;
- debuggability received a fixed value in the original instrumentation;
- performance timings were collected inside the Unity Editor;
- results depend on the configurations documented in this repository;
- different Unity versions or hardware may produce different absolute timings;
- the code represents an experimental implementation rather than a general-purpose PCG framework.

These limitations are discussed in greater detail in the dissertation.

---

## License

Original scripts made available in this repository are subject to the license defined in `LICENSE` or `LICENSE.txt` at the repository root.

External resources, including KayKit assets, remain subject to the licenses defined by their respective authors.

---

## Related documentation

For complete experimental reproduction, see:

- `../Configuration/AlgorithmParameters.md` — final algorithm parameters;
- `../Configuration/AssetConfiguration.md` — 3D asset library;
- `../Configuration/ExperimentalConfiguration.md` — shared batch conditions;
- `../Configuration/HardwareAndSoftware.md` — experimental environment;
- `../Documentation/ExperimentProtocol.md` — execution and measurement protocol;
- `../Data/README.md` — identification of final experimental data;
- `../Seeds/` — seed set used in the experiment.
