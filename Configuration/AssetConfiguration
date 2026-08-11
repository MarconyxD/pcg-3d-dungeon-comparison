<a id="portuguese-version"></a>

# Configuração dos Recursos Tridimensionais

[![English version](https://img.shields.io/badge/English_version-Click_here-0969DA?style=for-the-badge)](#english-version)

Este documento registra a configuração da biblioteca de recursos tridimensionais utilizada nos experimentos da dissertação.

Todos os seis algoritmos analisados utilizaram a mesma `DungeonAssetLibrary`, garantindo que diferenças observadas entre as dungeons não fossem causadas pela utilização de conjuntos distintos de modelos tridimensionais.

A biblioteca reúne elementos estruturais, marcadores e recursos semânticos utilizados na representação visual das dungeons geradas.

---

## Biblioteca compartilhada

| Configuração | Valor |
|---|---|
| Biblioteca utilizada | `DungeonAssetLibrary` |
| Utilização pelos algoritmos | Compartilhada pelos seis geradores |
| Instanciação de geometria | Ativada |
| Fallback por primitivas | Ativado |
| Material de fallback para piso | Nenhum material específico configurado |
| Material de fallback para parede | Nenhum material específico configurado |

A opção de fallback por primitivas permitia que elementos estruturais básicos fossem representados por primitivas da Unity caso um prefab necessário não estivesse disponível. Durante os experimentos, entretanto, os principais recursos estruturais encontravam-se configurados na biblioteca.

---

## Prefabs estruturais

Os elementos responsáveis pela construção básica das dungeons foram configurados da seguinte forma:

| Função | Prefab configurado |
|---|---|
| Piso | `floor_dirt_small_D` |
| Parede | `wall` |
| Porta / abertura | `wall_doorway` |
| Escada de subida | `stairs` |
| Escada de descida | `stairs` |

Esses mesmos recursos foram utilizados pelos diferentes algoritmos sempre que a estrutura produzida exigia a representação correspondente.

O uso de uma biblioteca estrutural comum contribuiu para que a comparação permanecesse concentrada nas diferenças de organização espacial e comportamento dos algoritmos, e não nas características de modelos tridimensionais distintos.

---

## Marcadores

Dois prefabs foram utilizados como marcadores auxiliares:

| Função | Prefab configurado |
|---|---|
| Marcador de início | `coin` |
| Marcador de objetivo | `coin_stack_small` |

Esses elementos permitiam identificar visualmente pontos relevantes da estrutura gerada durante a inspeção das dungeons.

---

## Objetos decorativos

A biblioteca continha dez prefabs destinados à decoração procedural das regiões geradas.

| Índice | Prefab |
|---:|---|
| 0 | `bucket_pickaxes` |
| 1 | `box_large` |
| 2 | `box_stacked` |
| 3 | `bucket` |
| 4 | `chair` |
| 5 | `crate_large_decorated` |
| 6 | `crates_stacked` |
| 7 | `keg` |
| 8 | `stool` |
| 9 | `table_medium_decorated_A` |

A quantidade efetivamente posicionada em cada dungeon dependia das regiões disponíveis, da probabilidade de decoração configurada para cada algoritmo e dos limites mínimo e máximo definidos no gerador correspondente.

A lista de prefabs, entretanto, permaneceu a mesma em todas as técnicas.

---

## Inimigos

Quatro prefabs de inimigos foram disponibilizados para os sistemas de distribuição procedural:

| Índice | Prefab |
|---:|---|
| 0 | `Skeleton_Minion` |
| 1 | `Skeleton_Warrior` |
| 2 | `Skeleton_Rogue` |
| 3 | `Skeleton_Mage` |

O orçamento experimental comum foi definido em `Enemy Budget = 10`.

Esse valor representa o limite utilizado pelo sistema de distribuição durante a geração e não significa necessariamente que dez inimigos seriam posicionados em todas as dungeons. A quantidade efetiva também dependia da existência de regiões consideradas adequadas para spawn.

---

## Loot

A biblioteca de recompensas foi composta por quatro prefabs:

| Índice | Prefab |
|---:|---|
| 0 | `coin` |
| 1 | `coin_stack_small` |
| 2 | `coin_stack_medium` |
| 3 | `coin_stack_large` |

O orçamento experimental comum foi definido em `Loot Budget = 6`.

Da mesma forma que no caso dos inimigos, esse valor representa o limite disponível para o sistema de distribuição e não uma quantidade obrigatória de objetos em cada geração.

---

## Armadilhas

Uma única opção de armadilha foi utilizada:

| Índice | Prefab |
|---:|---|
| 0 | `floor_tile_big_spikes` |

O orçamento experimental comum foi definido em `Trap Budget = 4`.

A presença efetiva das armadilhas dependia da existência de posições válidas nas regiões geradas.

---

## Configuração espacial dos prefabs

Além da seleção dos modelos tridimensionais, os geradores utilizaram uma configuração espacial comum para sua instanciação:

| Parâmetro | Valor |
|---|---:|
| Tile Size | 2 |
| Floor Height | 4 |
| Wall Height | 3 |
| Wall Thickness | 0,25 |
| Wall Yaw Offset | 0 |
| Wall Y Offset | 0 |
| Prefab Instance Scale | 1 × 1 × 1 |
| Center On Origin | Ativado |

Esses valores foram mantidos constantes para que os seis algoritmos utilizassem a mesma escala espacial ao transformar seus layouts lógicos em ambientes tridimensionais.

---

## Origem dos recursos

Os modelos tridimensionais utilizados na representação das dungeons pertencem ao conjunto de assets KayKit utilizado no projeto experimental.

Os arquivos originais desses assets não são redistribuídos neste repositório. Este repositório documenta apenas:

- os nomes dos prefabs utilizados;
- sua função dentro do experimento;
- as configurações necessárias para reproduzir sua utilização.

Os direitos e condições de uso dos recursos de terceiros permanecem vinculados às respectivas licenças de seus autores e distribuidores.

Consequentemente, a licença deste repositório se aplica ao código e aos materiais originais disponibilizados pelo autor da investigação, não alterando a licença dos assets externos utilizados pelo projeto.

---

## Relação com os algoritmos

A `DungeonAssetLibrary` foi compartilhada pelas implementações de:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

Embora todos os algoritmos tivessem acesso à mesma biblioteca, a quantidade e a organização dos objetos instanciados podiam variar de acordo com a estrutura produzida.

Dessa forma, diferenças observadas na representação final resultam principalmente das características espaciais de cada algoritmo e das regiões disponíveis para posicionamento, e não da utilização de bibliotecas visuais diferentes.

---

<a id="english-version"></a>

# 3D Asset Configuration

[![Versão em português](https://img.shields.io/badge/Vers%C3%A3o_em_portugu%C3%AAs-Voltar-1F883D?style=for-the-badge)](#portuguese-version)

This document records the configuration of the 3D asset library used in the dissertation experiments.

All six analyzed algorithms used the same `DungeonAssetLibrary`, ensuring that differences observed between generated dungeons were not caused by the use of different sets of 3D models.

The library contains structural elements, markers, and semantic resources used for the visual representation of the generated dungeons.

---

## Shared library

| Configuration | Value |
|---|---|
| Library | `DungeonAssetLibrary` |
| Use by algorithms | Shared by all six generators |
| Geometry instantiation | Enabled |
| Primitive fallback | Enabled |
| Fallback floor material | No specific material configured |
| Fallback wall material | No specific material configured |

The primitive fallback option allowed basic structural elements to be represented using Unity primitives if a required prefab was unavailable. During the experiments, however, the main structural assets were configured in the library.

---

## Structural prefabs

The elements responsible for the basic construction of the dungeons were configured as follows:

| Function | Configured prefab |
|---|---|
| Floor | `floor_dirt_small_D` |
| Wall | `wall` |
| Door / opening | `wall_doorway` |
| Stairs up | `stairs` |
| Stairs down | `stairs` |

The same resources were used by the different algorithms whenever the generated structure required the corresponding representation.

Using a common structural library helped keep the comparison focused on differences in spatial organization and algorithm behavior rather than on differences between 3D model sets.

---

## Markers

Two prefabs were used as auxiliary markers:

| Function | Configured prefab |
|---|---|
| Start marker | `coin` |
| Goal marker | `coin_stack_small` |

These elements provided a visual indication of relevant points in the generated structure during dungeon inspection.

---

## Decorative props

The library contained ten prefabs intended for procedural decoration of generated regions.

| Index | Prefab |
|---:|---|
| 0 | `bucket_pickaxes` |
| 1 | `box_large` |
| 2 | `box_stacked` |
| 3 | `bucket` |
| 4 | `chair` |
| 5 | `crate_large_decorated` |
| 6 | `crates_stacked` |
| 7 | `keg` |
| 8 | `stool` |
| 9 | `table_medium_decorated_A` |

The number of objects actually placed in each dungeon depended on the available regions, the decoration probability configured for each algorithm, and the minimum and maximum limits defined in the corresponding generator.

The prefab list itself remained identical for all techniques.

---

## Enemies

Four enemy prefabs were available to the procedural distribution systems:

| Index | Prefab |
|---:|---|
| 0 | `Skeleton_Minion` |
| 1 | `Skeleton_Warrior` |
| 2 | `Skeleton_Rogue` |
| 3 | `Skeleton_Mage` |

The common experimental budget was defined as `Enemy Budget = 10`.

This value represents the limit used by the distribution system during generation and does not necessarily mean that ten enemies were placed in every dungeon. The actual number also depended on the existence of regions considered suitable for spawning.

---

## Loot

The reward library contained four prefabs:

| Index | Prefab |
|---:|---|
| 0 | `coin` |
| 1 | `coin_stack_small` |
| 2 | `coin_stack_medium` |
| 3 | `coin_stack_large` |

The common experimental budget was defined as `Loot Budget = 6`.

As with enemies, this value represents the limit available to the distribution system rather than a mandatory number of objects for each generation.

---

## Traps

A single trap option was used:

| Index | Prefab |
|---:|---|
| 0 | `floor_tile_big_spikes` |

The common experimental budget was defined as `Trap Budget = 4`.

The actual presence of traps depended on the availability of valid placement positions in the generated regions.

---

## Prefab spatial configuration

In addition to the selection of 3D models, the generators used a common spatial configuration for their instantiation:

| Parameter | Value |
|---|---:|
| Tile Size | 2 |
| Floor Height | 4 |
| Wall Height | 3 |
| Wall Thickness | 0.25 |
| Wall Yaw Offset | 0 |
| Wall Y Offset | 0 |
| Prefab Instance Scale | 1 × 1 × 1 |
| Center On Origin | Enabled |

These values were kept constant so that all six algorithms used the same spatial scale when transforming their logical layouts into 3D environments.

---

## Asset origin

The 3D models used to represent the dungeons belong to the KayKit asset collection used in the experimental project.

The original third-party asset files are not redistributed in this repository. This repository documents only:

- the names of the prefabs used;
- their role in the experiment;
- the configuration required to reproduce their use.

The rights and terms of use of third-party resources remain subject to the licenses provided by their respective authors and distributors.

Consequently, the license of this repository applies to the original code and materials made available by the author of the research and does not modify the licenses of external assets used by the project.

---

## Relationship with the algorithms

The `DungeonAssetLibrary` was shared by the implementations of:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

Although all algorithms had access to the same library, the number and organization of instantiated objects could vary according to the generated structure.

Therefore, differences observed in the final representation primarily result from the spatial characteristics of each algorithm and the regions available for placement rather than from the use of different visual libraries.
