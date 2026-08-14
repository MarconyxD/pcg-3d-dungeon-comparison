<a id="portuguese-version"></a>

# 3D Procedural Dungeon Generation — Comparative Study

[![English version](https://img.shields.io/badge/English_version-Click_here-0969DA?style=for-the-badge)](#english-version)

![Unity](https://img.shields.io/badge/Unity-6000.3.0f1-000000?style=flat-square&logo=unity)
![C Sharp](https://img.shields.io/badge/C%23-Experimental_Implementation-512BD4?style=flat-square)
![Runs](https://img.shields.io/badge/Experimental_Runs-180-1F883D?style=flat-square)
![License](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)

## Sobre este repositório

Este repositório reúne os materiais de reprodutibilidade, scripts e dados experimentais utilizados em uma investigação comparativa sobre técnicas de geração procedural de dungeons tridimensionais.

O estudo compara seis abordagens de Procedural Content Generation (PCG) implementadas em um mesmo ambiente experimental na Unity:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

O objetivo deste repositório é permitir que as condições experimentais sejam verificadas, compreendidas e reproduzidas, preservando a relação entre:

`implementação → configuração → execução → dados → resultados`

---

## Dissertação

Este repositório acompanha a investigação de mestrado de **Marcony Montini de Oliveira Lima** sobre geração procedural de dungeons em jogos digitais 3D.

Os materiais disponibilizados foram organizados especificamente para documentar a implementação e as baterias experimentais utilizadas na análise comparativa.

O repositório não corresponde a um jogo completo nem a um framework comercial de geração procedural. Ele deve ser entendido como um **pacote acadêmico de reprodutibilidade** da implementação experimental.

---

## Visão geral do experimento

| Elemento | Configuração |
|---|---|
| Algoritmos analisados | 6 |
| Execuções por algoritmo | 30 |
| Total de gerações experimentais principais | 180 |
| Seeds | 2000–2029 |
| Grid lógico | 64 × 64 |
| Game engine | Unity 6000.3.0f1 |
| Linguagem | C# |
| Ambiente de execução | Unity Editor |
| Play Mode | Não utilizado |
| Build compilada | Não utilizada |
| Render Pipeline | Universal Render Pipeline (URP) |
| Resolução de referência | 1920 × 1080 |

Todos os algoritmos utilizaram o mesmo conjunto de 30 seeds e, sempre que aplicável, compartilharam as mesmas condições externas de geração, escala espacial, biblioteca tridimensional, instrumentação e procedimento de coleta.

---

## Estrutura do repositório

    pcg-3d-dungeon-comparison/
    │
    ├── Configuration/
    │   ├── AlgorithmParameters.md
    │   ├── AssetConfiguration.md
    │   ├── ExperimentalConfiguration.md
    │   └── HardwareAndSoftware.md
    │
    ├── Data/
    │   ├── BSP/
    │   ├── CellularAutomata/
    │   ├── DrunkardsWalk/
    │   ├── GrammarBased/
    │   ├── RoomGraph/
    │   ├── WFC/
    │   └── README.md
    │
    ├── Documentation/
    │   └── ExperimentProtocol.md
    │
    ├── Scripts/
    │   ├── Editor/
    │   ├── Runtime/
    │   │   ├── Generators/
    │   │   └── Shared/
    │   └── README.md
    │
    ├── Seeds/
    │   ├── README.md
    │   └── seeds_2000_2029.txt
    │
    ├── CITATION.cff
    ├── LICENSE.txt
    └── README.md

---

## Navegação rápida

| Conteúdo | Documento |
|---|---|
| Parâmetros finais dos seis algoritmos | [AlgorithmParameters.md](Configuration/AlgorithmParameters.md) |
| Biblioteca e assets tridimensionais | [AssetConfiguration.md](Configuration/AssetConfiguration.md) |
| Configuração geral do experimento | [ExperimentalConfiguration.md](Configuration/ExperimentalConfiguration.md) |
| Hardware, software e configuração gráfica | [HardwareAndSoftware.md](Configuration/HardwareAndSoftware.md) |
| Protocolo completo de execução | [ExperimentProtocol.md](Documentation/ExperimentProtocol.md) |
| Dados experimentais finais | [Data/README.md](Data/README.md) |
| Documentação dos scripts | [Scripts/README.md](Scripts/README.md) |
| Seeds experimentais | [Seeds/README.md](Seeds/README.md) |
| Metadados para citação | [CITATION.cff](CITATION.cff) |
| Licença | [LICENSE.txt](LICENSE.txt) |

---

## Algoritmos

### Binary Space Partitioning

Divide recursivamente o espaço em regiões, posiciona salas nas partições resultantes e estabelece corredores entre elas. A implementação experimental também inclui loops adicionais e conexões verticais entre pavimentos.

### Cellular Automata

Utiliza regras locais de vizinhança aplicadas iterativamente a uma grade para produzir estruturas orgânicas semelhantes a cavernas. A configuração experimental utiliza dois pavimentos, com vizinhança vertical ativada e conexões verticais inferidas entre regiões abertas alinhadas nas diferentes camadas.

### Drunkard's Walk

Utiliza múltiplos caminhantes pseudoaleatórios para escavar progressivamente o espaço disponível, produzindo caminhos e regiões de formato menos regular. A implementação inclui suporte experimental à movimentação entre pavimentos.

### Grammar-Based Generation

Constrói a dungeon através de regras de produção responsáveis por definir progressão, ramificações e funções espaciais, incluindo áreas de tesouro, armadilhas e arena de chefe.

### Room Graph

Representa inicialmente a organização da dungeon através de um grafo de salas e conexões, posteriormente convertido em uma estrutura espacial tridimensional.

### Wave Function Collapse

Utiliza módulos e regras de compatibilidade para realizar um processo de colapso e propagação de restrições. A implementação experimental inclui observações iniciais, estrutura conectada, módulos verticais e tratamento interno de contradições.

---

## Configuração experimental

Os valores efetivamente utilizados nas baterias finais estão documentados em:

[Configuration/AlgorithmParameters.md](Configuration/AlgorithmParameters.md)

Essa distinção é importante porque determinados valores padrão declarados nos scripts podem diferir dos valores serializados que estavam configurados no Inspector durante o experimento.

Para fins de reprodução, **a documentação da pasta `Configuration` deve ser utilizada como referência para os parâmetros experimentais finais**.

As condições compartilhadas incluem, entre outras:

- grid lógico de 64 × 64 células;
- Tile Size igual a 2;
- mesma biblioteca tridimensional;
- Enemy Budget igual a 10;
- Loot Budget igual a 6;
- Trap Budget igual a 4;
- 30 execuções por algoritmo;
- seeds de 2000 a 2029.

---

## Seeds

O conjunto de seeds utilizado está disponível em:

[Seeds/seeds_2000_2029.txt](Seeds/seeds_2000_2029.txt)

Cada algoritmo foi executado com os mesmos 30 valores:

`2000–2029`

A utilização da mesma seed não implica que algoritmos diferentes produzam estruturas equivalentes. Cada técnica utiliza a sequência pseudoaleatória segundo seus próprios procedimentos.

O objetivo do conjunto comum é manter uma identificação experimental uniforme entre os métodos.

---

## Executando o experimento

Este repositório disponibiliza os scripts e a documentação de configuração, mas **não contém uma cópia integral do projeto Unity original**.

Para reconstruir o ambiente experimental:

1. utilize preferencialmente a Unity `6000.3.0f1`;
2. copie `Scripts/Runtime/` para uma pasta de runtime do projeto;
3. mantenha `Scripts/Editor/` dentro de uma pasta denominada `Editor`;
4. obtenha e configure os recursos tridimensionais descritos em [AssetConfiguration.md](Configuration/AssetConfiguration.md);
5. adicione à cena o gerador correspondente ao algoritmo desejado;
6. configure os valores documentados em [AlgorithmParameters.md](Configuration/AlgorithmParameters.md);
7. aplique as condições comuns descritas em [ExperimentalConfiguration.md](Configuration/ExperimentalConfiguration.md);
8. mantenha o Editor fora do Play Mode;
9. execute a bateria através do botão de medição disponibilizado pelo Inspector do gerador;
10. compare os relatórios produzidos com os dados preservados em `Data/`.

O procedimento detalhado, incluindo medição de desempenho, tratamento de falhas e consolidação, encontra-se em:

[Documentation/ExperimentProtocol.md](Documentation/ExperimentProtocol.md)

---

## Métricas

A instrumentação experimental registra três grupos principais de parâmetros.

### Métricas quantitativas

Incluem propriedades como:

- número de salas ou regiões;
- conectividade;
- percentual de preenchimento;
- fator de ramificação;
- variação vertical;
- comprimento médio dos caminhos;
- caminho crítico;
- caminhos alternativos;
- quantidade de módulos;
- estimativa lógica do volume navegável;
- tempos de execução.

### Parâmetros booleanos

Registram características como:

- presença de loops;
- conectores verticais;
- múltiplos pavimentos;
- presença de região compatível com arena de chefe;
- reprodutibilidade por seed;
- atendimento ao limite experimental de regeneração;
- compatibilidade com os sistemas de inimigos, loot e armadilhas.

### Estimativas qualitativas automatizadas

A implementação também calcula pontuações heurísticas para:

- replayability;
- facilidade de depuração;
- fluxo;
- legibilidade;
- variedade estrutural.

Esses valores são **estimativas instrumentais automatizadas**. Eles não correspondem a avaliações realizadas por participantes ou a medições diretas da experiência de jogadores.

Da mesma forma, o parâmetro de volume navegável é uma **estimativa baseada na representação lógica** e não uma validação física por NavMesh.

---

## Dados experimentais

Os dados finais utilizados na análise estão disponíveis em:

[Data/](Data/)

Cada bateria preservada inclui:

- relatório completo em JSON;
- parâmetros individuais por execução em CSV;
- resultados agregados em CSV;
- relatório legível em Markdown.

As baterias selecionadas são:

| Algoritmo | Bateria | Finalidade |
|---|---|---|
| BSP | `20260501_213318` | Bateria experimental final |
| Cellular Automata | `20260814_035905` | Bateria experimental final |
| Drunkard's Walk | `20260503_012255` | Bateria experimental final |
| Grammar-Based Generation | `20260503_052720` | Bateria experimental final |
| Room Graph | `20260502_190613` | Dados estruturais e lógicos |
| Room Graph | `20260519_183913` | Instanciação visual complementar |
| WFC | `20260502_204944` | Dados estruturais e lógicos |
| WFC | `20260519_184822` | Instanciação visual complementar |

Room Graph e WFC possuem duas baterias preservadas porque a medição de instanciação visual utilizada na comparação final foi obtida em uma coleta complementar.

Nas duas técnicas, as baterias correspondentes utilizaram as mesmas seeds e apresentaram correspondência dos identificadores topológicos seed por seed.

Mais detalhes:

[Data/README.md](Data/README.md)

---

## Tratamento de resultados desfavoráveis

Não foi utilizado um procedimento de escolha manual das melhores dungeons.

Resultados com:

- baixa conectividade;
- regiões desconectadas;
- ausência de loops;
- baixa ramificação;
- ausência ou limitação de verticalidade;

permaneceram nas baterias experimentais quando correspondiam às seeds previstas.

Esses comportamentos constituem parte dos resultados e são necessários para representar as limitações das técnicas avaliadas.

O WFC possui reinícios internos próprios de sua implementação quando encontra contradições. Esses reinícios pertencem à mesma execução experimental e não representam substituição de uma seed por outra.

---

## Assets de terceiros

A representação tridimensional utiliza recursos do **KayKit Dungeon Remastered Pack**, criado por **Kay Lousberg** e atualmente distribuído como **KayKit - Dungeon Pack**.

Página oficial:

[KayKit - Dungeon Pack](https://kaylousberg.itch.io/kaykit-dungeon-pack)

Os arquivos tridimensionais originais não são redistribuídos neste repositório.

A documentação registra:

- o pacote utilizado;
- os prefabs selecionados;
- sua função no experimento;
- a configuração espacial utilizada.

O pacote KayKit é distribuído sob licença CC0. A licença MIT deste repositório aplica-se apenas aos códigos e materiais originais disponibilizados neste projeto.

Mais informações:

[Configuration/AssetConfiguration.md](Configuration/AssetConfiguration.md)

---

## Limitações conhecidas

Ao interpretar ou reproduzir o experimento, devem ser consideradas as seguintes limitações:

- os testes foram executados diretamente no Unity Editor e não em uma build compilada;
- o estudo utilizou um único ambiente principal de hardware e software;
- a escala experimental utilizada não representa testes sistemáticos em diferentes tamanhos de dungeon;
- o volume navegável é uma estimativa lógica e não uma validação NavMesh;
- as pontuações qualitativas são heurísticas automatizadas;
- não foram utilizados participantes para validar diretamente replayability, legibilidade, fluxo ou experiência de exploração;
- os valores absolutos de desempenho podem variar em outros computadores ou versões da Unity;
- este repositório não contém a cena Unity original nem os assets externos;
- os valores padrão presentes nos scripts não substituem os parâmetros finais documentados em `Configuration/`.

Essas limitações não impedem a comparação interna realizada no estudo, mas delimitam o alcance da generalização dos resultados.

---

## Citação

Este repositório contém um arquivo:

[CITATION.cff](CITATION.cff)

O GitHub pode utilizar esses metadados para gerar automaticamente uma referência através da opção **Cite this repository**.

Ao utilizar os scripts, dados ou documentação em trabalhos acadêmicos, recomenda-se citar este repositório e, quando aplicável, a dissertação associada à investigação.

---

## Licença

O código e os materiais originais disponibilizados neste repositório são distribuídos sob a:

[MIT License](LICENSE.txt)

A licença MIT não altera as condições de utilização de recursos externos.

Os assets KayKit permanecem sujeitos à licença definida por seu autor.

---

## Autor

**Marcony Montini de Oliveira Lima**

Repositório acadêmico para documentação e reprodução da investigação comparativa sobre geração procedural de dungeons tridimensionais.

---

<a id="english-version"></a>

# 3D Procedural Dungeon Generation — Comparative Study

[![Versão em português](https://img.shields.io/badge/Vers%C3%A3o_em_portugu%C3%AAs-Voltar-1F883D?style=for-the-badge)](#portuguese-version)

![Unity](https://img.shields.io/badge/Unity-6000.3.0f1-000000?style=flat-square&logo=unity)
![C Sharp](https://img.shields.io/badge/C%23-Experimental_Implementation-512BD4?style=flat-square)
![Runs](https://img.shields.io/badge/Experimental_Runs-180-1F883D?style=flat-square)
![License](https://img.shields.io/badge/License-MIT-yellow?style=flat-square)

## About this repository

This repository contains the reproducibility materials, source scripts, and experimental data used in a comparative investigation of procedural generation techniques for three-dimensional dungeons.

The study compares six Procedural Content Generation (PCG) approaches implemented within the same Unity experimental environment:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

The purpose of this repository is to allow the experimental conditions to be inspected, understood, and reproduced while preserving the relationship between:

`implementation → configuration → execution → data → results`

---

## Dissertation

This repository accompanies the master's research of **Marcony Montini de Oliveira Lima** on procedural generation of dungeons in 3D digital games.

The published materials were organized specifically to document the implementation and experimental batches used in the comparative analysis.

This repository is not a complete game or a production-ready procedural-generation framework. It should be understood as an **academic reproducibility package** for the experimental implementation.

---

## Experiment overview

| Element | Configuration |
|---|---|
| Algorithms | 6 |
| Runs per algorithm | 30 |
| Main experimental generations | 180 |
| Seeds | 2000–2029 |
| Logical grid | 64 × 64 |
| Game engine | Unity 6000.3.0f1 |
| Language | C# |
| Execution environment | Unity Editor |
| Play Mode | Not used |
| Compiled build | Not used |
| Render Pipeline | Universal Render Pipeline (URP) |
| Reference resolution | 1920 × 1080 |

All algorithms used the same set of 30 seeds and, whenever applicable, shared the same external generation conditions, spatial scale, 3D asset library, instrumentation system, and collection procedure.

---

## Repository structure

    pcg-3d-dungeon-comparison/
    │
    ├── Configuration/
    │   ├── AlgorithmParameters.md
    │   ├── AssetConfiguration.md
    │   ├── ExperimentalConfiguration.md
    │   └── HardwareAndSoftware.md
    │
    ├── Data/
    │   ├── BSP/
    │   ├── CellularAutomata/
    │   ├── DrunkardsWalk/
    │   ├── GrammarBased/
    │   ├── RoomGraph/
    │   ├── WFC/
    │   └── README.md
    │
    ├── Documentation/
    │   └── ExperimentProtocol.md
    │
    ├── Scripts/
    │   ├── Editor/
    │   ├── Runtime/
    │   │   ├── Generators/
    │   │   └── Shared/
    │   └── README.md
    │
    ├── Seeds/
    │   ├── README.md
    │   └── seeds_2000_2029.txt
    │
    ├── CITATION.cff
    ├── LICENSE.txt
    └── README.md

---

## Quick navigation

| Content | Document |
|---|---|
| Final parameters for all six algorithms | [AlgorithmParameters.md](Configuration/AlgorithmParameters.md) |
| 3D asset library and configuration | [AssetConfiguration.md](Configuration/AssetConfiguration.md) |
| Shared experimental configuration | [ExperimentalConfiguration.md](Configuration/ExperimentalConfiguration.md) |
| Hardware, software, and rendering environment | [HardwareAndSoftware.md](Configuration/HardwareAndSoftware.md) |
| Complete execution protocol | [ExperimentProtocol.md](Documentation/ExperimentProtocol.md) |
| Final experimental data | [Data/README.md](Data/README.md) |
| Script documentation | [Scripts/README.md](Scripts/README.md) |
| Experimental seeds | [Seeds/README.md](Seeds/README.md) |
| Citation metadata | [CITATION.cff](CITATION.cff) |
| License | [LICENSE.txt](LICENSE.txt) |

---

## Algorithms

### Binary Space Partitioning

Recursively divides the available space into regions, places rooms inside the resulting partitions, and establishes corridors between them. The experimental implementation also includes additional loops and vertical connections between floors.

### Cellular Automata

Uses local neighborhood rules iteratively applied to a grid to produce organic cave-like structures. The experimental configuration uses two floors, with vertical neighborhood rules enabled and vertical connections inferred between aligned open regions across the different layers.

### Drunkard's Walk

Uses multiple pseudorandom walkers to progressively carve the available space, producing less regular paths and regions. The implementation includes experimental support for movement between floors.

### Grammar-Based Generation

Constructs the dungeon through production rules responsible for defining progression, branches, and spatial roles, including treasure areas, traps, and a boss arena.

### Room Graph

Initially represents the dungeon organization as a graph of rooms and connections, which is subsequently converted into a three-dimensional spatial structure.

### Wave Function Collapse

Uses modules and compatibility rules to perform constraint collapse and propagation. The experimental implementation includes initial observations, a connected backbone, vertical modules, and internal contradiction handling.

---

## Experimental configuration

The values actually used in the final experimental batches are documented in:

[Configuration/AlgorithmParameters.md](Configuration/AlgorithmParameters.md)

This distinction is important because some default values declared in the source files may differ from the serialized values configured in the Unity Inspector during the experiments.

For reproduction purposes, **the documentation under `Configuration/` should be treated as the reference for the final experimental parameters**.

Shared conditions include, among others:

- 64 × 64 logical grid;
- Tile Size of 2;
- common 3D asset library;
- Enemy Budget of 10;
- Loot Budget of 6;
- Trap Budget of 4;
- 30 runs per algorithm;
- seeds 2000 through 2029.

---

## Seeds

The experimental seed set is available at:

[Seeds/seeds_2000_2029.txt](Seeds/seeds_2000_2029.txt)

Each algorithm was executed using the same 30 values:

`2000–2029`

Using the same seed does not imply that different algorithms produce equivalent structures. Each technique consumes the pseudorandom sequence according to its own procedures.

The common set provides uniform experimental identification across methods.

---

## Running the experiment

This repository provides the source scripts and configuration documentation but **does not contain a complete copy of the original Unity project**.

To reconstruct the experimental environment:

1. preferably use Unity `6000.3.0f1`;
2. copy `Scripts/Runtime/` into a runtime directory in the project;
3. keep `Scripts/Editor/` inside a directory named `Editor`;
4. obtain and configure the 3D resources documented in [AssetConfiguration.md](Configuration/AssetConfiguration.md);
5. add the corresponding generator component to the experimental scene;
6. apply the values documented in [AlgorithmParameters.md](Configuration/AlgorithmParameters.md);
7. apply the shared conditions described in [ExperimentalConfiguration.md](Configuration/ExperimentalConfiguration.md);
8. keep the Unity Editor outside Play Mode;
9. run the batch through the measurement button provided by the generator Inspector;
10. compare the generated reports with the preserved datasets under `Data/`.

The detailed procedure, including performance measurement, failure handling, and aggregation, is available in:

[Documentation/ExperimentProtocol.md](Documentation/ExperimentProtocol.md)

---

## Metrics

The experimental instrumentation records three primary groups of parameters.

### Quantitative metrics

These include properties such as:

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
- execution times.

### Boolean parameters

These record characteristics such as:

- loop presence;
- vertical connectors;
- multiple floors;
- presence of a region compatible with a boss arena;
- seed reproducibility;
- compliance with the experimental regeneration threshold;
- compatibility with enemy, loot, and trap systems.

### Automated qualitative estimates

The implementation also calculates heuristic scores for:

- replayability;
- debuggability;
- flow;
- legibility;
- structural variety.

These values are **automated instrumental estimates**. They do not represent evaluations performed by participants or direct measurements of player experience.

Likewise, navigable volume is a **logical estimate** and not a physical NavMesh validation.

---

## Experimental data

The final datasets used in the analysis are available under:

[Data/](Data/)

Each preserved batch includes:

- a complete JSON report;
- per-run parameters in CSV;
- aggregated results in CSV;
- a human-readable Markdown report.

The selected batches are:

| Algorithm | Batch | Purpose |
|---|---|---|
| BSP | `20260501_213318` | Final experimental batch |
| Cellular Automata | `20260814_035905` | Final experimental batch |
| Drunkard's Walk | `20260503_012255` | Final experimental batch |
| Grammar-Based Generation | `20260503_052720` | Final experimental batch |
| Room Graph | `20260502_190613` | Structural and logical data |
| Room Graph | `20260519_183913` | Complementary visual instantiation |
| WFC | `20260502_204944` | Structural and logical data |
| WFC | `20260519_184822` | Complementary visual instantiation |

Room Graph and WFC contain two preserved batches because the visual-instantiation measurement used in the final comparison was collected in a complementary batch.

For both techniques, corresponding batches used the same seeds and presented seed-by-seed matching topological identifiers.

More details:

[Data/README.md](Data/README.md)

---

## Handling unfavorable results

No manual best-dungeon selection procedure was used.

Results containing:

- low connectivity;
- disconnected regions;
- no loops;
- limited branching;
- absent or limited verticality;

remained in the experimental batches when they corresponded to the predefined seeds.

These behaviors are part of the results and are necessary to represent limitations of the evaluated techniques.

WFC includes internal restarts when contradictions occur. These restarts belong to the same experimental run and do not represent replacement of one experimental seed with another.

---

## Third-party assets

The three-dimensional representation uses assets from the **KayKit Dungeon Remastered Pack**, created by **Kay Lousberg** and currently distributed as **KayKit - Dungeon Pack**.

Official page:

[KayKit - Dungeon Pack](https://kaylousberg.itch.io/kaykit-dungeon-pack)

The original 3D asset files are not redistributed in this repository.

The documentation records:

- the package used;
- selected prefabs;
- their experimental role;
- spatial configuration.

The KayKit package is distributed under CC0. The MIT license of this repository applies only to original code and materials published as part of this project.

More information:

[Configuration/AssetConfiguration.md](Configuration/AssetConfiguration.md)

---

## Known limitations

The following limitations should be considered when interpreting or reproducing the experiment:

- tests were executed directly inside the Unity Editor rather than in a compiled build;
- the study used a single primary hardware and software environment;
- the experimental scale does not represent systematic testing across multiple dungeon sizes;
- navigable volume is a logical estimate rather than NavMesh validation;
- qualitative scores are automated heuristics;
- no participants were used to directly validate replayability, legibility, flow, or exploration experience;
- absolute performance values may differ on other computers or Unity versions;
- this repository does not contain the original Unity scene or external assets;
- default values found in the scripts do not replace the final parameters documented under `Configuration/`.

These limitations do not invalidate the internal comparison performed in the study, but they delimit the extent to which the results can be generalized.

---

## Citation

This repository contains:

[CITATION.cff](CITATION.cff)

GitHub can use these metadata to automatically generate citation information through **Cite this repository**.

When using the scripts, data, or documentation in academic work, please cite this repository and, when applicable, the associated dissertation.

---

## License

Original source code and materials published in this repository are distributed under the:

[MIT License](LICENSE.txt)

The MIT license does not modify the terms applicable to external resources.

KayKit assets remain subject to the license defined by their original author.

---

## Author

**Marcony Montini de Oliveira Lima**

Academic repository for documenting and reproducing the comparative investigation of procedural generation techniques for three-dimensional dungeons.
