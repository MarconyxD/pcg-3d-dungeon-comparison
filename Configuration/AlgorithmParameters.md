<a id="portuguese-version"></a>

# Parâmetros dos Algoritmos

[![English version](https://img.shields.io/badge/English_version-Click_here-0969DA?style=for-the-badge)](#english-version)

Este documento registra os parâmetros específicos de cada algoritmo utilizados na configuração experimental final do estudo comparativo.

Os valores apresentados correspondem às configurações definidas no Inspector da Unity durante as baterias experimentais que originaram os resultados apresentados na dissertação. Dessa forma, estes valores devem ser considerados como a configuração experimental de referência, mesmo quando diferem dos valores padrão declarados diretamente nos scripts.

Os parâmetros compartilhados entre os geradores, incluindo dimensões do grid, posicionamento tridimensional, orçamentos de elementos semânticos, seeds, quantidade de execuções e demais condições gerais do experimento, são documentados separadamente em `ExperimentalConfiguration.md`.

---

## Binary Space Partitioning (BSP)

### Geração

| Parâmetro | Valor |
|---|---:|
| Profundidade máxima de subdivisão | 5 |
| Tamanho mínimo das salas | 5 |
| Tamanho máximo das salas | 16 |
| Número máximo de salas | 24 |
| Espaçamento interno das salas | 1 |
| Largura dos corredores | 3 |
| Folga adicional das aberturas | 1 |
| Probabilidade de subdivisão | 0,92 |
| Conexões adicionais para loops | 2 |
| Distância máxima para conexões adicionais | 26 |

### Verticalidade

| Parâmetro | Valor |
|---|---:|
| Geração multiandar | Ativada |
| Número de pavimentos | 2 |
| Conexões verticais por par de pavimentos | 1 |
| Raio de procura para conexão vertical | 10 |
| Raio da abertura vertical | 1 |
| Deslocamento frontal da abertura vertical | 1 |

### Parâmetros experimentais adicionais

| Parâmetro | Valor |
|---|---:|
| Probabilidade de decoração das salas | 0,75 |
| Área mínima para arena de chefe | 80 células |

A implementação do BSP divide recursivamente o espaço disponível, cria salas dentro das regiões resultantes, conecta a estrutura formada pelas partições e, posteriormente, adiciona as conexões extras e verticais previstas na configuração.

---

## Cellular Automata

### Regras do Cellular Automata

| Parâmetro | Valor |
|---|---:|
| Probabilidade inicial de células abertas | 0,50 |
| Iterações de suavização | 4 |
| Limite de sobrevivência de células abertas | 3 |
| Limite de nascimento de células abertas | 5 |
| Espessura da borda sólida | 1 |
| Inclusão de vizinhos diagonais | Ativada |
| Área mínima de componente de sala | 16 células |
| Limite de vizinhos cardinais para corredores | 2 |
| Limite total de vizinhos para corredores | 4 |

### Verticalidade

| Parâmetro | Valor |
|---|---:|
| Cellular Automata multiandar | Desativado |
| Parâmetro de número de pavimentos | 2 |
| Inclusão de vizinhos verticais nas regras | Ativada, mas inativa devido à geração multiandar estar desativada |
| Máximo de conectores verticais por par de pavimentos | 2, mas inativo devido à geração multiandar estar desativada |

### Parâmetros experimentais adicionais

| Parâmetro | Valor |
|---|---:|
| Probabilidade de decoração das regiões | 0,55 |
| Área mínima para arena de chefe | 120 células |

O experimento com Cellular Automata foi, portanto, realizado em uma configuração de pavimento único. Os parâmetros relacionados à verticalidade permaneceram disponíveis no componente, mas não participaram da geração experimental analisada.

---

## Drunkard's Walk

### Regras do Drunkard's Walk

| Parâmetro | Valor |
|---|---:|
| Percentual alvo de preenchimento | 0,32 |
| Número máximo de passos | 16.000 |
| Número de caminhantes | 5 |
| Raio de escavação | 1 |
| Probabilidade de mudança de direção | 0,55 |
| Probabilidade de reinício de ramificação | 0,04 |
| Probabilidade de criação de região ampliada | 0,025 |
| Raio mínimo da região ampliada | 1 |
| Raio máximo da região ampliada | 3 |
| Passos diagonais | Desativados |
| Espessura da borda sólida | 1 |
| Inclusão de vizinhos diagonais | Ativada |
| Área mínima de componente de sala | 12 células |
| Limite de vizinhos cardinais para corredores | 2 |
| Limite total de vizinhos para corredores | 4 |

### Verticalidade

| Parâmetro | Valor |
|---|---:|
| Drunkard's Walk multiandar | Ativado |
| Número de pavimentos | 2 |
| Probabilidade de passo vertical | 0,015 |
| Máximo de conectores verticais por par de pavimentos | 2 |

### Parâmetros experimentais adicionais

| Parâmetro | Valor |
|---|---:|
| Probabilidade de decoração das regiões | 0,55 |
| Área mínima para arena de chefe | 120 células |

A implementação utiliza múltiplos caminhantes que escavam progressivamente o espaço lógico até atingir o nível de ocupação pretendido ou o número máximo de passos definido.

---

## Grammar-Based Generation

### Regras da gramática

| Parâmetro | Valor |
|---|---:|
| Tamanho mínimo das salas | 5 |
| Tamanho máximo das salas | 16 |
| Número máximo de salas/símbolos espaciais | 24 |
| Espaçamento entre salas | 1 |
| Tentativas de posicionamento por sala | 350 |
| Comprimento mínimo do caminho principal | 7 |
| Comprimento máximo do caminho principal | 12 |
| Probabilidade de ramificação | 0,45 |
| Número máximo de ramificações | 5 |
| Comprimento mínimo das ramificações | 1 |
| Comprimento máximo das ramificações | 3 |
| Probabilidade de área de tesouro | 0,35 |
| Probabilidade de área de armadilha | 0,25 |
| Probabilidade de mudança de direção | 0,55 |
| Probabilidade de regra de loop | 0,35 |
| Regra de arena de chefe | Ativada |
| Tamanho mínimo da arena de chefe | 10 |
| Tamanho máximo da arena de chefe | 20 |
| Distância mínima entre etapas da gramática | 8 |
| Distância máxima entre etapas da gramática | 14 |
| Largura dos corredores | 3 |
| Folga adicional das aberturas | 1 |
| Conexões extras para loops | 2 |
| Distância máxima para conexões adicionais | 26 |

### Verticalidade

| Parâmetro | Valor |
|---|---:|
| Gramática multiandar | Ativada |
| Número de pavimentos | 2 |
| Regras verticais por par de pavimentos | 1 |
| Raio de procura para conectores verticais | 10 |
| Raio da abertura vertical | 1 |
| Deslocamento frontal da abertura vertical | 1 |

### Parâmetros experimentais adicionais

| Parâmetro | Valor |
|---|---:|
| Probabilidade de decoração das salas | 0,75 |
| Área mínima para arena de chefe | 80 células |

A gramática gera um caminho principal de progressão e ramificações opcionais, atribuindo funções como áreas de tesouro, armadilha e chefe de acordo com as probabilidades configuradas.

---

## Room Graph

### Grafo e posicionamento espacial

| Parâmetro | Valor |
|---|---:|
| Tamanho mínimo das salas | 5 |
| Tamanho máximo das salas | 16 |
| Número máximo de salas | 24 |
| Espaçamento entre salas | 1 |
| Tentativas de posicionamento | 350 |
| Largura dos corredores | 3 |
| Folga adicional das aberturas | 1 |
| Conexões adicionais para loops | 2 |
| Distância máxima para conexões adicionais | 26 |

### Verticalidade

| Parâmetro | Valor |
|---|---:|
| Room Graph multiandar | Ativado |
| Número de pavimentos | 2 |
| Conexões verticais por par de pavimentos | 1 |
| Raio de procura para conectores verticais | 10 |
| Raio da abertura vertical | 1 |
| Deslocamento frontal da abertura vertical | 1 |

### Parâmetros experimentais adicionais

| Parâmetro | Valor |
|---|---:|
| Probabilidade de decoração das salas | 0,75 |
| Área mínima para arena de chefe | 80 células |

A implementação do Room Graph define inicialmente as relações entre as salas e, posteriormente, converte o grafo resultante em uma estrutura espacial, incluindo conexões adicionais para loops e conexões verticais entre pavimentos.

---

## Wave Function Collapse (WFC)

### Configuração do colapso

| Parâmetro | Valor |
|---|---:|
| Mínimo de células ocupadas para aceitação | 600 |
| Área mínima de componente de sala | 18 células |
| Número máximo de reinícios do colapso | 40 |

### Pesos dos módulos

| Tipo de módulo | Peso |
|---|---:|
| Vazio / sólido | 0,5 |
| Sala | 5,0 |
| Corredor | 0,8 |
| Conector vertical | 0,2 |

### Observações iniciais e estrutura conectada

| Parâmetro | Valor |
|---|---:|
| Observações iniciais de sala por pavimento | 8 |
| Estrutura inicial conectada | Ativada |
| Ramificações da estrutura inicial por pavimento | 6 |
| Comprimento mínimo das ramificações | 3 |
| Comprimento máximo das ramificações | 7 |

### Verticalidade

| Parâmetro | Valor |
|---|---:|
| WFC multiandar | Ativado |
| Número de pavimentos | 2 |
| Observações de conectores verticais por par de pavimentos | 1 |

### Configuração dos macromódulos

| Parâmetro | Valor |
|---|---:|
| Raio dos módulos de sala | 1 |
| Meia largura dos módulos de corredor | 1 |
| Abertura entre células adjacentes geradas | Ativada |

### Parâmetros experimentais adicionais

| Parâmetro | Valor |
|---|---:|
| Probabilidade de decoração das regiões | 0,65 |
| Área mínima para arena de chefe | 80 células |

Quando uma tentativa de colapso encontrava uma contradição ou produzia uma ocupação insuficiente, a implementação podia reiniciar internamente o processo até o limite configurado. Esses reinícios pertenciam à mesma seed experimental e não eram contabilizados como novas execuções do experimento.

---

## Observações sobre a configuração experimental

Os algoritmos não utilizam parâmetros internos idênticos, pois cada técnica depende de mecanismos distintos para realizar a geração procedural. A padronização experimental foi aplicada às condições externas que poderiam ser compartilhadas entre os métodos, incluindo:

- área lógica de geração;
- escala espacial;
- biblioteca de recursos tridimensionais;
- orçamentos de elementos semânticos;
- quantidade de execuções;
- conjunto de seeds;
- ambiente de execução;
- procedimento de medição.

Essas condições compartilhadas são documentadas em `ExperimentalConfiguration.md`.

O campo manual `Seed` exibido no Inspector da Unity estava configurado com o valor `12345` para gerações individuais. Esse valor não foi utilizado na bateria comparativa. As execuções automatizadas utilizaram as seeds de `2000` a `2029`, conforme documentado em `ExperimentalConfiguration.md`.

---

<a id="english-version"></a>

# Algorithm Parameters

[![Versão em português](https://img.shields.io/badge/Vers%C3%A3o_em_portugu%C3%AAs-Voltar-1F883D?style=for-the-badge)](#portuguese-version)

This document records the algorithm-specific parameters used in the final experimental configuration of the comparative study.

The values presented here correspond to the settings configured in the Unity Inspector during the experimental batches that produced the results reported in the dissertation. Therefore, these values should be treated as the reference experimental configuration, even when they differ from the default values declared directly in the source code.

Parameters shared by the generators, including grid dimensions, 3D placement, semantic element budgets, seeds, number of runs, and other general experimental conditions, are documented separately in `ExperimentalConfiguration.md`.

---

## Binary Space Partitioning (BSP)

### Generation

| Parameter | Value |
|---|---:|
| Maximum split depth | 5 |
| Minimum room size | 5 |
| Maximum room size | 16 |
| Maximum rooms | 24 |
| Room padding | 1 |
| Corridor width | 3 |
| Doorway extra clearance | 1 |
| Split chance | 0.92 |
| Extra loop connections | 2 |
| Maximum extra loop distance | 26 |

### Verticality

| Parameter | Value |
|---|---:|
| Multi-floor generation | Enabled |
| Floor count | 2 |
| Vertical connections per floor pair | 1 |
| Vertical connector search radius | 10 |
| Vertical opening radius | 1 |
| Vertical opening forward offset | 1 |

### Additional experimental parameters

| Parameter | Value |
|---|---:|
| Prop room chance | 0.75 |
| Boss arena minimum area | 80 cells |

The BSP implementation recursively partitions the available space, creates rooms inside the resulting regions, connects the partition structure, and subsequently adds the configured extra loops and vertical connections.

---

## Cellular Automata

### Cellular Automata rules

| Parameter | Value |
|---|---:|
| Initial open-cell chance | 0.50 |
| Smoothing iterations | 4 |
| Survival open-neighbor limit | 3 |
| Birth open-neighbor limit | 5 |
| Solid border thickness | 1 |
| Include diagonal neighbors | Enabled |
| Minimum room component area | 16 cells |
| Corridor cardinal-neighbor limit | 2 |
| Corridor total-neighbor limit | 4 |

### Verticality

| Parameter | Value |
|---|---:|
| Multi-floor Cellular Automata | Disabled |
| Floor count parameter | 2 |
| Include vertical neighbors in rule | Enabled, but inactive because multi-floor generation was disabled |
| Maximum vertical connectors per floor pair | 2, but inactive because multi-floor generation was disabled |

### Additional experimental parameters

| Parameter | Value |
|---|---:|
| Prop room chance | 0.55 |
| Boss arena minimum area | 120 cells |

The Cellular Automata experiment was therefore performed using a single-floor configuration. Vertical parameters remained available in the component but did not participate in the analyzed experimental generation.

---

## Drunkard's Walk

### Drunkard's Walk rules

| Parameter | Value |
|---|---:|
| Target fill percentage | 0.32 |
| Maximum walker steps | 16,000 |
| Walker count | 5 |
| Walk brush radius | 1 |
| Turn chance | 0.55 |
| Branch restart chance | 0.04 |
| Room stamp chance | 0.025 |
| Minimum room stamp radius | 1 |
| Maximum room stamp radius | 3 |
| Allow diagonal walk steps | Disabled |
| Solid border thickness | 1 |
| Include diagonal neighbors | Enabled |
| Minimum room component area | 12 cells |
| Corridor cardinal-neighbor limit | 2 |
| Corridor total-neighbor limit | 4 |

### Verticality

| Parameter | Value |
|---|---:|
| Multi-floor Drunkard's Walk | Enabled |
| Floor count | 2 |
| Vertical step chance | 0.015 |
| Maximum vertical connectors per floor pair | 2 |

### Additional experimental parameters

| Parameter | Value |
|---|---:|
| Prop room chance | 0.55 |
| Boss arena minimum area | 120 cells |

The implementation uses multiple walkers that progressively carve the logical space until the target occupation level is reached or the configured maximum number of steps is exhausted.

---

## Grammar-Based Generation

### Grammar rules

| Parameter | Value |
|---|---:|
| Minimum room size | 5 |
| Maximum room size | 16 |
| Maximum rooms / spatial symbols | 24 |
| Room padding | 1 |
| Room placement attempts | 350 |
| Minimum main-path length | 7 |
| Maximum main-path length | 12 |
| Branch rule chance | 0.45 |
| Maximum grammar branches | 5 |
| Minimum branch length | 1 |
| Maximum branch length | 3 |
| Treasure rule chance | 0.35 |
| Trap rule chance | 0.25 |
| Grammar turn chance | 0.55 |
| Loop rule chance | 0.35 |
| Force boss arena rule | Enabled |
| Boss room minimum size | 10 |
| Boss room maximum size | 20 |
| Minimum grammar step | 8 |
| Maximum grammar step | 14 |
| Corridor width | 3 |
| Doorway extra clearance | 1 |
| Extra loop connections | 2 |
| Maximum extra loop distance | 26 |

### Verticality

| Parameter | Value |
|---|---:|
| Multi-floor grammar | Enabled |
| Floor count | 2 |
| Vertical rules per floor pair | 1 |
| Vertical connector search radius | 10 |
| Vertical opening radius | 1 |
| Vertical opening forward offset | 1 |

### Additional experimental parameters

| Parameter | Value |
|---|---:|
| Prop room chance | 0.75 |
| Boss arena minimum area | 80 cells |

The grammar generates a main progression path and optional branches, assigning roles such as treasure, trap, and boss areas according to the configured probabilities.

---

## Room Graph

### Graph and spatial placement

| Parameter | Value |
|---|---:|
| Minimum room size | 5 |
| Maximum room size | 16 |
| Maximum rooms | 24 |
| Room padding | 1 |
| Room placement attempts | 350 |
| Corridor width | 3 |
| Doorway extra clearance | 1 |
| Extra loop connections | 2 |
| Maximum extra loop distance | 26 |

### Verticality

| Parameter | Value |
|---|---:|
| Multi-floor Room Graph | Enabled |
| Floor count | 2 |
| Vertical connections per floor pair | 1 |
| Vertical connector search radius | 10 |
| Vertical opening radius | 1 |
| Vertical opening forward offset | 1 |

### Additional experimental parameters

| Parameter | Value |
|---|---:|
| Prop room chance | 0.75 |
| Boss arena minimum area | 80 cells |

The Room Graph implementation first defines relationships between rooms and subsequently converts the resulting graph into a spatial structure, including additional loop connections and vertical edges between floors.

---

## Wave Function Collapse (WFC)

### Collapse configuration

| Parameter | Value |
|---|---:|
| Minimum occupied cells for accepted collapse | 600 |
| Minimum room component area | 18 cells |
| Maximum collapse restarts | 40 |

### Module weights

| Module type | Weight |
|---|---:|
| Empty / solid | 0.5 |
| Room | 5.0 |
| Corridor | 0.8 |
| Vertical connector | 0.2 |

### Initial observations and connected backbone

| Parameter | Value |
|---|---:|
| Room observations per floor | 8 |
| Constrain connected backbone | Enabled |
| Backbone branches per floor | 6 |
| Minimum backbone branch length | 3 |
| Maximum backbone branch length | 7 |

### Verticality

| Parameter | Value |
|---|---:|
| Multi-floor WFC | Enabled |
| Floor count | 2 |
| Vertical connector observations per floor pair | 1 |

### Macro-module configuration

| Parameter | Value |
|---|---:|
| Room brush radius | 1 |
| Corridor half width | 1 |
| Open adjacent painted cells | Enabled |

### Additional experimental parameters

| Parameter | Value |
|---|---:|
| Prop room chance | 0.65 |
| Boss arena minimum area | 80 cells |

When a collapse attempt encountered a contradiction or produced insufficient occupation, the implementation could internally restart the process up to the configured limit. These restarts belonged to the same experimental seed and were not counted as additional experimental runs.

---

## Notes on the experimental configuration

The algorithms intentionally do not share identical internal parameters because each technique relies on different procedural generation mechanisms. Experimental standardization was instead applied to comparable external conditions, including:

- logical generation area;
- spatial scale;
- 3D asset library;
- semantic element budgets;
- number of experimental runs;
- seed set;
- execution environment;
- measurement procedure.

These shared conditions are documented in `ExperimentalConfiguration.md`.

The manual `Seed` field displayed in the Unity Inspector was set to `12345` for individual generations. This value was not used in the comparative batch. Automated experimental runs used seeds `2000` through `2029`, as documented in `ExperimentalConfiguration.md`.
