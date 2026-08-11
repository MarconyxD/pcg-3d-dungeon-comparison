<a id="portuguese-version"></a>

# Seeds Experimentais

[![English version](https://img.shields.io/badge/English_version-Click_here-0969DA?style=for-the-badge)](#english-version)

Esta pasta contém o conjunto de seeds utilizado nas baterias experimentais da dissertação sobre geração procedural de dungeons tridimensionais.

O mesmo conjunto foi utilizado nos seis algoritmos avaliados:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

---

## Conjunto utilizado

As baterias comparativas utilizaram 30 seeds consecutivas:

`2000–2029`

| Parâmetro | Valor |
|---|---:|
| Primeira seed | 2000 |
| Última seed | 2029 |
| Quantidade de seeds | 30 |
| Execuções por algoritmo | 30 |
| Número de algoritmos | 6 |
| Gerações experimentais principais | 180 |

O arquivo `seeds_2000_2029.txt` contém os 30 valores, um por linha.

---

## Correspondência entre execução e seed

A seed utilizada em cada execução era definida automaticamente a partir do parâmetro `Test First Seed`.

A relação utilizada pelos geradores era:

`runSeed = testFirstSeed + i`

em que `i` corresponde ao índice da execução iniciado em zero.

Consequentemente:

| Execução | Seed |
|---:|---:|
| 1 | 2000 |
| 2 | 2001 |
| 3 | 2002 |
| 4 | 2003 |
| 5 | 2004 |
| 6 | 2005 |
| 7 | 2006 |
| 8 | 2007 |
| 9 | 2008 |
| 10 | 2009 |
| 11 | 2010 |
| 12 | 2011 |
| 13 | 2012 |
| 14 | 2013 |
| 15 | 2014 |
| 16 | 2015 |
| 17 | 2016 |
| 18 | 2017 |
| 19 | 2018 |
| 20 | 2019 |
| 21 | 2020 |
| 22 | 2021 |
| 23 | 2022 |
| 24 | 2023 |
| 25 | 2024 |
| 26 | 2025 |
| 27 | 2026 |
| 28 | 2027 |
| 29 | 2028 |
| 30 | 2029 |

---

## Utilização do mesmo conjunto entre algoritmos

Todos os algoritmos receberam o mesmo intervalo numérico de seeds.

Essa escolha fornece uma identificação uniforme para as execuções e evita que cada técnica seja avaliada utilizando um conjunto arbitrariamente diferente de valores.

Entretanto, uma mesma seed não produz necessariamente estruturas equivalentes entre algoritmos diferentes.

Cada técnica:

- utiliza procedimentos próprios de geração;
- realiza diferentes quantidades e tipos de operações pseudoaleatórias;
- consome a sequência pseudoaleatória de forma distinta.

Portanto, a seed `2000` do BSP não deve ser interpretada como equivalente espacialmente à seed `2000` do WFC, por exemplo.

A equivalência pretendida pelo protocolo está no conjunto experimental utilizado, e não na estrutura resultante.

---

## Randomização automática

Durante as baterias comparativas, a opção:

`Randomize Seed`

permaneceu desativada.

As seeds foram, portanto, determinadas explicitamente pelo procedimento experimental.

Essa configuração permite repetir uma execução específica utilizando novamente:

- o mesmo algoritmo;
- a mesma seed;
- os mesmos parâmetros;
- a mesma versão da implementação.

---

## Seed de geração manual

Os componentes presentes na cena experimental também possuíam um campo denominado:

`Seed`

configurado com o valor:

`12345`

Esse valor era utilizado em gerações individuais acionadas manualmente pelo Inspector e não fazia parte do conjunto comparativo final.

A bateria automatizada utilizava o campo:

`Test First Seed = 2000`

e gerava sequencialmente os valores até `2029`.

Por esse motivo, a seed `12345` não está incluída em `seeds_2000_2029.txt`.

---

## Verificação de reprodutibilidade

Antes das 30 execuções principais, cada gerador realizava uma verificação automática de reprodutibilidade.

A primeira seed experimental, `2000`, era executada duas vezes e os identificadores topológicos resultantes eram comparados.

Caso os dois identificadores coincidissem, a reprodução por seed era considerada aprovada.

Essas execuções de verificação:

- utilizavam a seed 2000;
- faziam parte do procedimento de validação;
- não eram contabilizadas entre as 30 execuções formais da bateria.

Assim, o conjunto principal permanece composto por exatamente 30 gerações por algoritmo.

---

## Reinícios internos do WFC

O Wave Function Collapse possui um mecanismo interno de reinício quando ocorre uma contradição ou quando o resultado não atende às condições de ocupação estabelecidas.

Essas tentativas internas não constituem novas seeds experimentais.

Elas pertencem à mesma execução formal e utilizam sequências pseudoaleatórias derivadas da seed original.

Portanto, caso a execução correspondente à seed `2000` necessite de múltiplas tentativas internas do WFC, ela continua sendo contabilizada como:

`Run 1 — Seed 2000`

O procedimento completo de tratamento de contradições está documentado em:

`../Documentation/ExperimentProtocol.md`

---

## Relação com os dados publicados

Cada execução registrada na pasta `../Data/` contém a seed correspondente.

Dessa forma, é possível relacionar diretamente:

`algoritmo → execução → seed → hash topológico → métricas`

Os arquivos `*_parameters_by_run_*.csv` e `*_parameter_report_*.json` preservam essa associação.

Os conjuntos de dados selecionados para a dissertação estão documentados em:

`../Data/README.md`

---

## Reproduzindo uma execução específica

Para reproduzir uma determinada geração, devem ser utilizados conjuntamente:

1. o algoritmo correspondente;
2. a seed registrada;
3. os parâmetros experimentais finais;
4. a configuração comum do ambiente;
5. a versão dos scripts disponibilizada neste repositório.

Os parâmetros estão disponíveis em:

- `../Configuration/AlgorithmParameters.md`;
- `../Configuration/ExperimentalConfiguration.md`.

Os scripts estão disponíveis em:

`../Scripts/`

---

## Documentação relacionada

Consulte também:

- `../Configuration/AlgorithmParameters.md` — parâmetros específicos dos algoritmos;
- `../Configuration/ExperimentalConfiguration.md` — condições comuns das baterias;
- `../Documentation/ExperimentProtocol.md` — procedimento de execução;
- `../Data/README.md` — identificação das baterias finais;
- `../Scripts/README.md` — documentação da implementação.

---

<a id="english-version"></a>

# Experimental Seeds

[![Versão em português](https://img.shields.io/badge/Vers%C3%A3o_em_portugu%C3%AAs-Voltar-1F883D?style=for-the-badge)](#portuguese-version)

This directory contains the seed set used in the experimental batches of the dissertation on procedural generation of three-dimensional dungeons.

The same set was used for all six evaluated algorithms:

- Binary Space Partitioning (BSP);
- Cellular Automata;
- Drunkard's Walk;
- Grammar-Based Generation;
- Room Graph;
- Wave Function Collapse (WFC).

---

## Seed set

The comparative batches used 30 consecutive seeds:

`2000–2029`

| Parameter | Value |
|---|---:|
| First seed | 2000 |
| Last seed | 2029 |
| Number of seeds | 30 |
| Runs per algorithm | 30 |
| Algorithms | 6 |
| Main experimental generations | 180 |

The file `seeds_2000_2029.txt` contains all 30 values, one per line.

---

## Run-to-seed mapping

The seed used in each run was automatically defined from `Test First Seed`.

The generators used:

`runSeed = testFirstSeed + i`

where `i` is the zero-based run index.

Therefore:

| Run | Seed |
|---:|---:|
| 1 | 2000 |
| 2 | 2001 |
| 3 | 2002 |
| 4 | 2003 |
| 5 | 2004 |
| 6 | 2005 |
| 7 | 2006 |
| 8 | 2007 |
| 9 | 2008 |
| 10 | 2009 |
| 11 | 2010 |
| 12 | 2011 |
| 13 | 2012 |
| 14 | 2013 |
| 15 | 2014 |
| 16 | 2015 |
| 17 | 2016 |
| 18 | 2017 |
| 19 | 2018 |
| 20 | 2019 |
| 21 | 2020 |
| 22 | 2021 |
| 23 | 2022 |
| 24 | 2023 |
| 25 | 2024 |
| 26 | 2025 |
| 27 | 2026 |
| 28 | 2027 |
| 29 | 2028 |
| 30 | 2029 |

---

## Same seed set across algorithms

All algorithms received the same numerical seed range.

This provides uniform identification of the experimental runs and prevents individual techniques from being evaluated using arbitrarily different seed sets.

However, the same seed does not necessarily produce equivalent structures across different algorithms.

Each technique:

- follows its own generation procedure;
- performs different types and numbers of pseudorandom operations;
- consumes the pseudorandom sequence differently.

Therefore, BSP seed `2000` should not be interpreted as spatially equivalent to WFC seed `2000`, for example.

The intended equivalence concerns the experimental set rather than the resulting structure.

---

## Automatic randomization

During the comparative batches:

`Randomize Seed`

remained disabled.

Seeds were therefore explicitly determined by the experimental procedure.

This makes it possible to repeat an individual run using:

- the same algorithm;
- the same seed;
- the same parameters;
- the same implementation version.

---

## Manual generation seed

Components in the experimental scene also contained a field named:

`Seed`

configured as:

`12345`

This value was used for individual generations triggered manually from the Inspector and was not part of the final comparative dataset.

The automated batch instead used:

`Test First Seed = 2000`

and sequentially generated values through `2029`.

For this reason, seed `12345` is not included in `seeds_2000_2029.txt`.

---

## Reproducibility verification

Before the 30 main runs, each generator performed an automated reproducibility check.

The first experimental seed, `2000`, was executed twice and the resulting topological identifiers were compared.

If both identifiers matched, seed reproducibility was considered successful.

These verification executions:

- used seed 2000;
- belonged to the validation procedure;
- were not counted among the 30 formal experimental runs.

The main dataset therefore remains exactly 30 generations per algorithm.

---

## Internal WFC restarts

Wave Function Collapse includes an internal restart mechanism when a contradiction occurs or when the result does not satisfy the configured occupation conditions.

These internal attempts do not constitute additional experimental seeds.

They belong to the same formal run and use pseudorandom sequences derived from the original seed.

Therefore, if the run corresponding to seed `2000` requires several internal WFC attempts, it is still recorded as:

`Run 1 — Seed 2000`

The complete contradiction-handling procedure is documented in:

`../Documentation/ExperimentProtocol.md`

---

## Relationship with published data

Every run stored under `../Data/` records its corresponding seed.

This makes it possible to trace:

`algorithm → run → seed → topological hash → metrics`

The `*_parameters_by_run_*.csv` and `*_parameter_report_*.json` files preserve this relationship.

Datasets selected for the dissertation are documented in:

`../Data/README.md`

---

## Reproducing a specific run

To reproduce a particular generation, use together:

1. the corresponding algorithm;
2. the recorded seed;
3. the final experimental parameters;
4. the shared environment configuration;
5. the script version published in this repository.

Parameter documentation is available in:

- `../Configuration/AlgorithmParameters.md`;
- `../Configuration/ExperimentalConfiguration.md`.

Source code is available under:

`../Scripts/`

---

## Related documentation

See also:

- `../Configuration/AlgorithmParameters.md` — algorithm-specific parameters;
- `../Configuration/ExperimentalConfiguration.md` — shared batch conditions;
- `../Documentation/ExperimentProtocol.md` — execution procedure;
- `../Data/README.md` — identification of final batches;
- `../Scripts/README.md` — implementation documentation.
