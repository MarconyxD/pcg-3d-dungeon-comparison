<a id="portuguese-version"></a>

# Configuração Experimental

[![English version](https://img.shields.io/badge/English_version-Click_here-0969DA?style=for-the-badge)](#english-version)

Este documento registra as condições experimentais compartilhadas pelos seis algoritmos de geração procedural analisados na dissertação.

O objetivo desta configuração comum foi reduzir diferenças externas entre as técnicas e permitir que os resultados fossem comparados sob condições equivalentes sempre que os parâmetros fossem compatíveis entre os métodos.

Os parâmetros específicos de cada algoritmo são apresentados separadamente em `AlgorithmParameters.md`, enquanto a biblioteca de recursos tridimensionais utilizada é documentada em `AssetConfiguration.md`.

---

## Algoritmos avaliados

O conjunto experimental foi composto pelas seguintes técnicas:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

Cada algoritmo foi implementado como um gerador independente dentro da mesma cena experimental da Unity.

---

## Espaço lógico de geração

Todos os algoritmos utilizaram como referência um espaço lógico de `64 × 64` células.

| Parâmetro | Valor |
|---|---:|
| Map Width | 64 células |
| Map Depth | 64 células |

Essa padronização forneceu uma área horizontal equivalente para as técnicas analisadas, mesmo quando seus processos internos de geração e interpretação estrutural eram diferentes.

O número de pavimentos não foi obrigatoriamente igual entre todas as técnicas, pois dependia das características da implementação experimental de cada algoritmo. A configuração correspondente é documentada em `AlgorithmParameters.md`.

---

## Configuração tridimensional compartilhada

A transformação dos layouts lógicos em ambientes tridimensionais utilizou os seguintes parâmetros:

| Parâmetro | Valor |
|---|---:|
| Instantiate Geometry | Ativado |
| Center On Origin | Ativado |
| Tile Size | 2 |
| Floor Height | 4 |
| Wall Height | 3 |
| Wall Thickness | 0,25 |
| Wall Yaw Offset | 0 |
| Wall Y Offset | 0 |
| Prefab Instance Scale | 1 × 1 × 1 |
| Use Primitive Fallbacks | Ativado |

O `Tile Size` estabelece a escala espacial utilizada na conversão das células lógicas para unidades da Unity.

O `Floor Height` define a distância vertical entre pavimentos, enquanto `Wall Height` e `Wall Thickness` estabelecem as dimensões utilizadas para paredes primitivas de fallback quando necessário.

Os prefabs foram mantidos em escala `1 × 1 × 1`.

A opção `Center On Origin` permaneceu ativada para que as dungeons fossem posicionadas em torno da origem da cena.

---

## Biblioteca tridimensional

Todos os algoritmos utilizaram a mesma `DungeonAssetLibrary`.

A biblioteca continha:

- prefabs estruturais para pisos, paredes, portas e escadas;
- marcadores de início e objetivo;
- dez prefabs decorativos;
- quatro prefabs de inimigos;
- quatro prefabs de loot;
- um prefab de armadilha.

A configuração completa da biblioteca é apresentada em `AssetConfiguration.md`.

O compartilhamento da mesma biblioteca entre os seis métodos evitou que diferenças nos recursos visuais disponíveis fossem introduzidas como uma variável adicional na comparação.

---

## Distribuição de elementos semânticos

Os geradores utilizavam uma camada comum para distribuição de elementos de gameplay.

Os seguintes limites foram mantidos constantes:

| Parâmetro | Valor |
|---|---:|
| Minimum Props Per Room | 0 |
| Maximum Props Per Room | 5 |
| Enemy Budget | 10 |
| Loot Budget | 6 |
| Trap Budget | 4 |

A probabilidade de uma região receber elementos decorativos variava entre os algoritmos e, por esse motivo, é documentada individualmente em `AlgorithmParameters.md`.

Os valores de orçamento representam limites máximos utilizados pelo sistema de distribuição. Eles não determinam que a quantidade total configurada seria necessariamente utilizada em todas as dungeons, pois o posicionamento também dependia da existência de regiões válidas.

---

## Seeds experimentais

A opção `Randomize Seed` permaneceu desativada durante as baterias comparativas.

Cada algoritmo foi executado utilizando o mesmo conjunto de 30 seeds consecutivas:

`2000` a `2029`.

| Parâmetro | Valor |
|---|---:|
| Test Run Count | 30 |
| Test First Seed | 2000 |
| Última seed utilizada | 2029 |
| Quantidade de seeds por algoritmo | 30 |
| Número de algoritmos | 6 |
| Total de gerações experimentais principais | 180 |
| Randomize Seed | Desativado |

A seed utilizada em cada execução era definida automaticamente pela relação:

`runSeed = testFirstSeed + i`

em que `i` corresponde à posição da execução dentro da bateria.

Assim:

- execução 1 → seed 2000;
- execução 2 → seed 2001;
- ...
- execução 30 → seed 2029.

O mesmo intervalo foi aplicado aos seis algoritmos.

Isso não significa que uma mesma seed produza estruturas equivalentes entre técnicas diferentes. Cada algoritmo possui seu próprio processo de geração e utiliza a sequência pseudoaleatória de maneira distinta. O uso do mesmo intervalo permite, entretanto, manter um conjunto experimental uniforme e facilmente identificável.

---

## Seed de geração manual

Os componentes presentes na cena também possuíam um campo manual denominado `Seed`, configurado com o valor:

`12345`

Esse valor era utilizado para gerações individuais realizadas pelo botão de geração normal do Inspector.

Ele não corresponde às seeds utilizadas na bateria comparativa final.

Para os experimentos automatizados, a referência era o campo `Test First Seed`, configurado com o valor `2000`.

---

## Quantidade de execuções

Cada algoritmo foi submetido a 30 execuções formais.

O conjunto principal foi, portanto:

`6 algoritmos × 30 execuções = 180 gerações`

Cada geração era registrada individualmente antes da consolidação dos resultados.

A utilização de múltiplas seeds permitiu observar:

- variação estrutural;
- conectividade;
- estabilidade;
- diversidade topológica;
- comportamento das métricas;
- frequência dos parâmetros booleanos;
- desempenho computacional.

---

## Verificação de reprodutibilidade

Antes da bateria principal, o sistema verificava automaticamente a reprodutibilidade da primeira seed experimental.

A mesma seed era utilizada duas vezes pelo mesmo algoritmo.

A estrutura resultante de cada execução era transformada em um identificador topológico. Caso os dois identificadores fossem iguais, a reprodutibilidade por seed era considerada aprovada.

Essa verificação não fazia parte das 30 gerações contabilizadas como amostra comparativa.

---

## Identificação topológica

Cada dungeon gerada recebia um identificador associado à sua estrutura.

Esse identificador considerava informações relacionadas a:

- dimensões e posições das regiões;
- pavimento correspondente;
- relações entre regiões;
- conexões verticais;
- conexões identificadas como loops.

Os identificadores eram utilizados para distinguir topologias diferentes entre as seeds e calcular a diversidade estrutural observada durante cada bateria.

---

## Limite experimental de regeneração

Foi adotado o seguinte valor como limite para o parâmetro de regeneração:

| Parâmetro | Valor |
|---|---:|
| Runtime Regeneration Max Milliseconds | 250 ms |

Uma execução dentro desse limite era considerada compatível com o critério experimental de regeneração.

Esse parâmetro funciona como uma referência definida para o estudo e não representa um limite universal para aplicações comerciais ou para todos os tipos de jogos.

---

## Instanciação visual durante a medição

A medição do custo de instanciação tridimensional foi realizada com a opção:

`Measure Visual Instantiation In Tests = Enabled`

Essa configuração permitiu registrar separadamente:

- o tempo utilizado para gerar o layout lógico;
- o tempo utilizado para instanciar a representação tridimensional;
- o tempo utilizado para calcular as métricas;
- o tempo total da operação medida.

No caso de Room Graph e WFC, os registros experimentais preservados incluem uma bateria estrutural e uma bateria complementar destinada à medição visual.

As baterias utilizaram o mesmo conjunto de seeds e produziram correspondência topológica seed por seed. O procedimento específico dessas duas técnicas é detalhado no documento `ExperimentProtocol.md`.

---

## Instanciação após a bateria

A opção `Instantiate Last Test Dungeon` permaneceu ativada.

Após a conclusão e consolidação de uma bateria, o sistema podia instanciar novamente a dungeon correspondente à última seed utilizada.

Essa etapa servia para inspeção visual e eventual exportação do mapa final.

A geração realizada nessa etapa ocorria depois da consolidação dos dados e não era adicionada como uma nova execução experimental.

---

## Exportação das métricas

Os sistemas de instrumentação foram configurados para armazenar os resultados na pasta:

`PCGMetrics`

A opção de exportação automática de métricas encontrava-se ativada.

Os relatórios de bateria podiam ser produzidos nos seguintes formatos:

- JSON;
- CSV com dados individuais por execução;
- CSV com resultados agregados;
- Markdown.

Os dados utilizados na dissertação foram posteriormente identificados e separados dos arquivos produzidos durante testes de desenvolvimento.

Os conjuntos experimentais finais são disponibilizados na pasta `Data` deste repositório.

---

## Exportação de mapas 2D

Os componentes também possuíam suporte para representação bidimensional das dungeons.

A configuração comum utilizada incluía:

| Parâmetro | Valor |
|---|---:|
| Map Export Subfolder Name | `Maps` |
| Map Pixels Per Cell | 10 |
| Map Include Grid | Ativado |
| Map Include Legend | Ativado |
| Export 2D Maps During Measurement Test | Desativado |
| Export 2D Map For Last Test Dungeon | Ativado |

A exportação de um mapa para cada uma das 30 seeds permaneceu desativada durante as baterias, evitando a criação desnecessária de grandes quantidades de imagens.

Entretanto, a última dungeon da bateria podia ser exportada para fins de inspeção e documentação.

Alguns algoritmos também possuíam representações específicas adicionais, como mapas da massa celular ou da área percorrida pelos caminhantes.

---

## Estado anterior às execuções

Os geradores estavam configurados com a opção `Clear Before Generate` ativada.

Isso permitia remover a dungeon anterior antes da criação de uma nova estrutura.

O procedimento evitava que elementos pertencentes a gerações anteriores permanecessem na cena e interferissem na próxima execução.

O estado interno dos geradores também era preparado novamente para a seed correspondente antes da construção do novo layout.

---

## Critério de padronização

A comparação não procurou atribuir valores internos idênticos a todos os algoritmos, pois cada técnica depende de mecanismos próprios.

Por exemplo:

- BSP utiliza subdivisões;
- Cellular Automata utiliza regras de vizinhança;
- Drunkard's Walk utiliza caminhantes;
- Grammar-Based Generation utiliza regras de produção;
- Room Graph utiliza nós e conexões;
- WFC utiliza módulos e restrições de compatibilidade.

A padronização foi aplicada às condições externas comparáveis.

Os principais elementos comuns foram:

- espaço lógico de 64 × 64 células;
- mesma escala espacial;
- mesma biblioteca de recursos tridimensionais;
- mesmos orçamentos de inimigos, loot e armadilhas;
- mesmo conjunto de seeds;
- 30 execuções por algoritmo;
- mesmo ambiente de desenvolvimento;
- mesmo sistema de instrumentação;
- mesmo procedimento de consolidação.

Os parâmetros internos específicos são documentados em `AlgorithmParameters.md`.

---

## Resumo da configuração experimental

| Elemento | Configuração |
|---|---|
| Algoritmos | 6 |
| Execuções por algoritmo | 30 |
| Gerações experimentais principais | 180 |
| Seeds | 2000–2029 |
| Randomização automática das seeds | Desativada |
| Grid | 64 × 64 |
| Tile Size | 2 |
| Floor Height | 4 |
| Wall Height | 3 |
| Wall Thickness | 0,25 |
| Prefab Scale | 1 × 1 × 1 |
| DungeonAssetLibrary | Compartilhada |
| Enemy Budget | 10 |
| Loot Budget | 6 |
| Trap Budget | 4 |
| Runtime Regeneration Limit | 250 ms |
| Medição da instanciação visual | Ativada para a coleta de desempenho |
| Pasta de métricas | `PCGMetrics` |

---

<a id="english-version"></a>

# Experimental Configuration

[![Versão em português](https://img.shields.io/badge/Vers%C3%A3o_em_portugu%C3%AAs-Voltar-1F883D?style=for-the-badge)](#portuguese-version)

This document records the experimental conditions shared by the six procedural generation algorithms analyzed in the dissertation.

The purpose of this common configuration was to reduce external differences between the techniques and allow their results to be compared under equivalent conditions whenever the parameters were compatible across methods.

Algorithm-specific parameters are documented separately in `AlgorithmParameters.md`, while the 3D resource library is documented in `AssetConfiguration.md`.

---

## Evaluated algorithms

The experimental set consisted of:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

Each algorithm was implemented as an independent generator within the same Unity experimental scene.

---

## Logical generation space

All algorithms used a `64 × 64` cell logical space as their reference.

| Parameter | Value |
|---|---:|
| Map Width | 64 cells |
| Map Depth | 64 cells |

This standardization provided an equivalent horizontal area for the analyzed techniques, even though their internal generation and structural interpretation processes differed.

The number of floors was not necessarily identical for every technique because it depended on the experimental implementation of each algorithm. The corresponding configurations are documented in `AlgorithmParameters.md`.

---

## Shared 3D configuration

The conversion of logical layouts into 3D environments used the following parameters:

| Parameter | Value |
|---|---:|
| Instantiate Geometry | Enabled |
| Center On Origin | Enabled |
| Tile Size | 2 |
| Floor Height | 4 |
| Wall Height | 3 |
| Wall Thickness | 0.25 |
| Wall Yaw Offset | 0 |
| Wall Y Offset | 0 |
| Prefab Instance Scale | 1 × 1 × 1 |
| Use Primitive Fallbacks | Enabled |

`Tile Size` defines the spatial scale used when converting logical cells into Unity units.

`Floor Height` defines the vertical distance between floors, while `Wall Height` and `Wall Thickness` define the dimensions used by primitive fallback walls when required.

Prefabs were kept at their original `1 × 1 × 1` scale.

`Center On Origin` remained enabled so that the generated dungeons were positioned around the scene origin.

---

## 3D asset library

All algorithms used the same `DungeonAssetLibrary`.

The library contained:

- structural prefabs for floors, walls, doors, and stairs;
- start and goal markers;
- ten decorative prefabs;
- four enemy prefabs;
- four loot prefabs;
- one trap prefab.

The complete library configuration is documented in `AssetConfiguration.md`.

Sharing the same asset library across all six methods prevented differences in the available visual resources from becoming an additional variable in the comparison.

---

## Semantic element distribution

The generators used a common layer for distributing gameplay-related elements.

The following limits were kept constant:

| Parameter | Value |
|---|---:|
| Minimum Props Per Room | 0 |
| Maximum Props Per Room | 5 |
| Enemy Budget | 10 |
| Loot Budget | 6 |
| Trap Budget | 4 |

The probability of a region receiving decorative elements varied between algorithms and is therefore documented individually in `AlgorithmParameters.md`.

Budget values represent upper limits used by the distribution system. They do not imply that the full configured quantity had to be placed in every dungeon because placement also depended on the availability of valid regions.

---

## Experimental seeds

`Randomize Seed` remained disabled during the comparative batches.

Each algorithm was executed using the same set of 30 consecutive seeds:

`2000` through `2029`.

| Parameter | Value |
|---|---:|
| Test Run Count | 30 |
| Test First Seed | 2000 |
| Last seed used | 2029 |
| Seeds per algorithm | 30 |
| Algorithms | 6 |
| Main experimental generations | 180 |
| Randomize Seed | Disabled |

The seed used for each run was automatically defined by:

`runSeed = testFirstSeed + i`

where `i` represents the position of the execution within the batch.

Therefore:

- run 1 → seed 2000;
- run 2 → seed 2001;
- ...
- run 30 → seed 2029.

The same interval was applied to all six algorithms.

This does not mean that the same seed produces equivalent structures across different techniques. Each algorithm has its own generation process and consumes the pseudorandom sequence differently. Using the same interval nevertheless provides a uniform and easily identifiable experimental set.

---

## Manual generation seed

The components in the experimental scene also contained a manual `Seed` field configured as:

`12345`

This value was used for individual generations triggered by the standard generation button in the Inspector.

It does not correspond to the seeds used in the final comparative batch.

For the automated experiments, the relevant field was `Test First Seed`, configured as `2000`.

---

## Number of runs

Each algorithm was submitted to 30 formal runs.

The main experimental set therefore contained:

`6 algorithms × 30 runs = 180 generations`

Each generation was recorded individually before the results were aggregated.

Using multiple seeds made it possible to observe:

- structural variation;
- connectivity;
- stability;
- topological diversity;
- metric behavior;
- frequency of boolean parameters;
- computational performance.

---

## Reproducibility verification

Before the main batch, the system automatically verified the reproducibility of the first experimental seed.

The same seed was used twice by the same algorithm.

The structure produced in each execution was transformed into a topological identifier. If the two identifiers matched, seed reproducibility was considered successful.

This verification was not included among the 30 generations counted in the comparative sample.

---

## Topological identification

Each generated dungeon received an identifier associated with its structure.

The identifier considered information related to:

- dimensions and positions of regions;
- corresponding floor;
- relationships between regions;
- vertical connections;
- connections identified as loops.

These identifiers were used to distinguish different topologies across seeds and calculate the structural diversity observed during each batch.

---

## Experimental regeneration limit

The following value was adopted as the regeneration criterion:

| Parameter | Value |
|---|---:|
| Runtime Regeneration Max Milliseconds | 250 ms |

A run completed within this limit was considered compatible with the experimental regeneration criterion.

This parameter is a reference established for this study and should not be interpreted as a universal performance threshold for commercial applications or every game genre.

---

## Visual instantiation during measurement

The cost of 3D instantiation was measured using:

`Measure Visual Instantiation In Tests = Enabled`

This configuration allowed separate recording of:

- logical layout generation time;
- 3D geometry instantiation time;
- metric calculation time;
- total measured operation time.

For Room Graph and WFC, the preserved experimental records include an initial structural batch and a later complementary batch dedicated to visual instantiation measurement.

Both batches used the same seed set and produced seed-by-seed topological correspondence. The specific procedure for these two techniques is documented in `ExperimentProtocol.md`.

---

## Instantiation after the batch

`Instantiate Last Test Dungeon` remained enabled.

After a batch was completed and its results had been consolidated, the system could instantiate the dungeon corresponding to the final seed again.

This step was intended for visual inspection and optional export of the final map.

The generation performed at this stage occurred after data consolidation and was not added as another experimental run.

---

## Metrics export

The instrumentation systems stored their results in:

`PCGMetrics`

Automatic metrics export was enabled.

Batch reports could be produced in:

- JSON;
- CSV containing individual run data;
- CSV containing aggregated results;
- Markdown.

The data used in the dissertation were subsequently identified and separated from files produced during development testing.

The final experimental datasets are made available in the `Data` directory of this repository.

---

## 2D map export

The components also supported 2D representations of generated dungeons.

The common configuration included:

| Parameter | Value |
|---|---:|
| Map Export Subfolder Name | `Maps` |
| Map Pixels Per Cell | 10 |
| Map Include Grid | Enabled |
| Map Include Legend | Enabled |
| Export 2D Maps During Measurement Test | Disabled |
| Export 2D Map For Last Test Dungeon | Enabled |

Exporting one map for every experimental seed remained disabled during the batches to avoid generating unnecessary large quantities of images.

The final dungeon in a batch could nevertheless be exported for inspection and documentation.

Some algorithms also supported additional representations, such as cellular-mass maps or walker-path masks.

---

## State before each generation

The generators were configured with `Clear Before Generate` enabled.

This removed the previous dungeon before a new structure was created.

The procedure prevented elements belonging to earlier generations from remaining in the scene and interfering with the next run.

The internal generator state was also prepared again for the corresponding seed before construction of the new layout.

---

## Standardization criterion

The comparison did not attempt to assign identical internal values to all algorithms because each technique relies on different generation mechanisms.

For example:

- BSP uses recursive partitions;
- Cellular Automata uses neighborhood rules;
- Drunkard's Walk uses walkers;
- Grammar-Based Generation uses production rules;
- Room Graph uses nodes and edges;
- WFC uses modules and compatibility constraints.

Standardization was therefore applied to comparable external conditions.

The primary common elements were:

- 64 × 64 logical generation area;
- same spatial scale;
- same 3D asset library;
- same enemy, loot, and trap budgets;
- same seed set;
- 30 runs per algorithm;
- same development environment;
- same instrumentation system;
- same aggregation procedure.

Algorithm-specific internal parameters are documented in `AlgorithmParameters.md`.

---

## Experimental configuration summary

| Element | Configuration |
|---|---|
| Algorithms | 6 |
| Runs per algorithm | 30 |
| Main experimental generations | 180 |
| Seeds | 2000–2029 |
| Automatic seed randomization | Disabled |
| Grid | 64 × 64 |
| Tile Size | 2 |
| Floor Height | 4 |
| Wall Height | 3 |
| Wall Thickness | 0.25 |
| Prefab Scale | 1 × 1 × 1 |
| DungeonAssetLibrary | Shared |
| Enemy Budget | 10 |
| Loot Budget | 6 |
| Trap Budget | 4 |
| Runtime Regeneration Limit | 250 ms |
| Visual instantiation measurement | Enabled for performance collection |
| Metrics directory | `PCGMetrics` |
