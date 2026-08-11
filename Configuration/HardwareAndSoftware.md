<a id="portuguese-version"></a>

# Hardware e Software do Ambiente Experimental

[![English version](https://img.shields.io/badge/English_version-Click_here-0969DA?style=for-the-badge)](#english-version)

Este documento registra as principais características de hardware, software e configuração gráfica do ambiente utilizado nos experimentos da dissertação.

Essas informações complementam `ExperimentalConfiguration.md` e permitem interpretar os resultados de desempenho considerando as condições em que as baterias foram executadas.

---

## Equipamento utilizado

Os experimentos foram realizados em um computador portátil Lenovo IdeaPad Gaming 3 15IHU6.

| Componente | Configuração |
|---|---|
| Modelo | Lenovo IdeaPad Gaming 3 15IHU6 |
| Processador | 11th Gen Intel Core i5-11300H |
| Frequência nominal do processador | 3,10 GHz |
| Memória RAM instalada | 24 GB |
| Memória RAM utilizável reportada pelo sistema | 23,8 GB |
| GPU dedicada instalada | NVIDIA GeForce RTX 3050 Laptop GPU |
| Memória da GPU dedicada | 4 GB |
| GPU integrada instalada | Intel Iris Xe Graphics |
| Arquitetura do sistema | 64 bits, processador baseado em x64 |

O equipamento possuía simultaneamente uma GPU dedicada NVIDIA e uma GPU integrada Intel.

Os registros preservados confirmam a presença de ambas, mas não identificam de forma conclusiva qual adaptador gráfico foi selecionado pela Unity durante cada execução experimental. Por esse motivo, este documento registra o hardware disponível sem atribuir os resultados exclusivamente a uma das duas GPUs.

---

## Sistema operacional

| Parâmetro | Valor |
|---|---|
| Sistema operacional | Windows 11 Home Single Language |
| Versão | 25H2 |
| Compilação registrada | 26200.8973 |
| Tipo de sistema | 64 bits |

Informações de identificação individual do equipamento, como ID do dispositivo, ID do produto e número de série, não são reproduzidas neste repositório por não serem necessárias para a reprodução metodológica.

---

## Unity

O protótipo experimental foi desenvolvido e executado na Unity.

| Parâmetro | Valor |
|---|---|
| Game engine | Unity |
| Versão | 6000.3.0f1 |
| Linguagem de programação | C# |
| Build Target configurado | Windows Standalone |
| Render Pipeline | Universal Render Pipeline (URP) |
| Perfil de qualidade ativo | PC |

Os seis geradores procedurais foram implementados dentro da mesma cena experimental.

---

## Modo de execução dos experimentos

As baterias não foram executadas em uma build compilada.

Os testes foram realizados diretamente no Editor da Unity e fora do `Play Mode`.

A execução ocorria através dos botões personalizados presentes no Inspector de cada gerador, incluindo as rotinas responsáveis pelas baterias de medição.

O procedimento experimental foi, portanto:

- abrir a cena experimental na Unity;
- manter o Editor fora do `Play Mode`;
- selecionar o componente correspondente ao algoritmo;
- iniciar a bateria através do botão de medição disponível no Inspector;
- permitir que o sistema processasse automaticamente as seeds configuradas;
- registrar e exportar os resultados produzidos pela instrumentação.

O protocolo completo é documentado em `ExperimentProtocol.md`.

Essa condição deve ser considerada ao interpretar os tempos registrados. Os resultados representam a execução dentro do ambiente do Editor da Unity e não devem ser tratados como valores absolutos de desempenho de uma build final otimizada.

---

## Resolução da Game View

A janela `Game` da Unity estava configurada para:

| Parâmetro | Valor |
|---|---:|
| Resolução | 1920 × 1080 |
| Perfil exibido | Full HD |
| Display | Display 1 |

Essa resolução foi mantida como referência visual do ambiente experimental.

---

## Perfil de qualidade

O nível de qualidade ativo durante a configuração experimental era denominado `PC`.

| Parâmetro | Configuração |
|---|---|
| Quality Level | PC |
| Build Target | Windows Standalone |
| Render Pipeline Asset | `PC_RPAsset` |
| Resolution Scaling Fixed DPI Factor | 1 |
| Realtime GI CPU Usage | Unlimited |
| VSync Count | Don't Sync |
| Global Mipmap Limit | Full Resolution |
| Anisotropic Textures | Forced On |
| Mipmap Streaming | Desativado |

A sincronização vertical permaneceu desativada.

---

## Universal Render Pipeline

O projeto utilizava o asset `PC_RPAsset` da Universal Render Pipeline.

### Configuração de renderização

| Parâmetro | Valor |
|---|---|
| Renderer | `PC_Renderer` |
| Depth Texture | Ativada |
| Opaque Texture | Ativada |
| Opaque Downsampling | 2x Bilinear |
| Terrain Holes | Ativado |
| GPU Resident Drawer | Disabled |

### Qualidade

| Parâmetro | Valor |
|---|---|
| HDR | Ativado |
| Anti Aliasing (MSAA) | Desativado |
| Render Scale | 1 |
| Upscaling Filter | Automatic |
| LOD Cross Fade | Ativado |
| LOD Cross Fade Dithering | Blue Noise |

A escala de renderização igual a 1 indica que a imagem era renderizada na resolução de referência, sem redução proporcional definida por esse parâmetro.

---

## Iluminação e sombras

### Luz principal

| Parâmetro | Valor |
|---|---|
| Main Light Rendering | Per Pixel |
| Cast Shadows | Ativado |
| Shadow Resolution | 2048 |

### Luzes adicionais

| Parâmetro | Valor |
|---|---|
| Additional Lights | Per Pixel |
| Per Object Limit | 4 |
| Cast Shadows | Ativado |
| Shadow Atlas Resolution | 2048 |

### Sombras

| Parâmetro | Valor |
|---|---|
| Maximum Shadow Distance | 50 |
| Cascade Count | 4 |
| Soft Shadows | Ativado |
| Soft Shadow Quality | High |
| Depth Bias | 0,1 |
| Normal Bias | 0,5 |

Essas configurações permaneceram associadas ao mesmo perfil gráfico utilizado pelos diferentes geradores.

---

## Pós-processamento

O projeto utilizava um `Global Volume` com o perfil `SampleSceneProfile`.

### Configurações gerais

| Parâmetro | Valor |
|---|---|
| Grading Mode | Low Dynamic Range |
| LUT Size | 32 |
| Volume Update Mode | Every Frame |

### Tonemapping

| Parâmetro | Valor |
|---|---|
| Tonemapping | Ativado |
| Mode | Neutral |

### Bloom

| Parâmetro | Valor |
|---|---:|
| Bloom | Ativado |
| Threshold | 1 |
| Intensity | 0,25 |
| Scatter | 0,5 |
| High Quality Filtering | Ativado |

### Motion Blur

| Parâmetro | Valor |
|---|---|
| Motion Blur | Desativado |

Embora parâmetros internos do componente permanecessem visíveis no perfil, o efeito estava desativado e, portanto, não contribuía para a representação final.

### Vignette

| Parâmetro | Valor |
|---|---:|
| Vignette | Ativada |
| Intensity | 0,2 |

O pós-processamento permaneceu comum à cena experimental, não sendo alterado entre as baterias dos diferentes algoritmos.

---

## Relação com as medições de desempenho

As configurações apresentadas neste documento são particularmente relevantes para os valores relacionados à instanciação visual.

A comparação experimental separa:

- geração do layout lógico;
- instanciação da geometria tridimensional;
- cálculo das métricas;
- tempo total medido.

O tempo lógico representa principalmente o processamento necessário para que cada algoritmo construa sua estrutura procedural.

O tempo de instanciação visual, por outro lado, também pode ser influenciado pelo ambiente da Unity, pela quantidade de objetos criados, pelos recursos tridimensionais e pelas condições gráficas descritas neste documento.

Por esse motivo, os resultados da dissertação apresentam o custo lógico e o custo visual separadamente.

---

## Considerações sobre reprodutibilidade

A reprodução dos valores de desempenho em outro equipamento não exige que os tempos obtidos sejam numericamente idênticos.

Diferenças de processador, memória, GPU, versão do sistema operacional, versão da Unity e estado do Editor podem alterar os tempos absolutos.

Para fins de reprodução, o objetivo principal é manter:

- a mesma versão da Unity, quando possível;
- os mesmos scripts;
- os mesmos parâmetros dos algoritmos;
- as mesmas seeds;
- a mesma configuração de geração;
- o mesmo protocolo de medição.

Os valores registrados nesta investigação devem ser interpretados principalmente como uma comparação interna entre algoritmos executados sob um mesmo ambiente experimental.

As configurações dos algoritmos encontram-se em `AlgorithmParameters.md`, enquanto as condições compartilhadas da bateria são documentadas em `ExperimentalConfiguration.md`.

---

## Resumo do ambiente

| Elemento | Configuração |
|---|---|
| Computador | Lenovo IdeaPad Gaming 3 15IHU6 |
| CPU | Intel Core i5-11300H @ 3,10 GHz |
| RAM | 24 GB |
| GPU dedicada disponível | NVIDIA GeForce RTX 3050 Laptop GPU 4 GB |
| GPU integrada disponível | Intel Iris Xe Graphics |
| Sistema operacional | Windows 11 Home Single Language 64 bits |
| Versão do Windows | 25H2 |
| Unity | 6000.3.0f1 |
| Execução | Unity Editor |
| Play Mode | Não utilizado |
| Build compilada | Não utilizada |
| Resolução da Game View | 1920 × 1080 |
| Build Target | Windows Standalone |
| Render Pipeline | Universal Render Pipeline |
| Quality Level | PC |
| Render Scale | 1 |
| HDR | Ativado |
| MSAA | Desativado |
| VSync | Desativado |
| Main Shadow Resolution | 2048 |
| Soft Shadows | High |
| Tonemapping | Neutral |
| Bloom | Ativado, intensidade 0,25 |
| Motion Blur | Desativado |
| Vignette | Ativada, intensidade 0,2 |

---

<a id="english-version"></a>

# Experimental Hardware and Software

[![Versão em português](https://img.shields.io/badge/Vers%C3%A3o_em_portugu%C3%AAs-Voltar-1F883D?style=for-the-badge)](#portuguese-version)

This document records the main hardware, software, and graphical configuration characteristics of the environment used for the dissertation experiments.

This information complements `ExperimentalConfiguration.md` and allows the performance results to be interpreted in relation to the conditions under which the experimental batches were executed.

---

## Experimental computer

The experiments were performed on a Lenovo IdeaPad Gaming 3 15IHU6 laptop.

| Component | Configuration |
|---|---|
| Model | Lenovo IdeaPad Gaming 3 15IHU6 |
| Processor | 11th Gen Intel Core i5-11300H |
| Nominal processor frequency | 3.10 GHz |
| Installed RAM | 24 GB |
| Usable RAM reported by the system | 23.8 GB |
| Installed dedicated GPU | NVIDIA GeForce RTX 3050 Laptop GPU |
| Dedicated GPU memory | 4 GB |
| Installed integrated GPU | Intel Iris Xe Graphics |
| System architecture | 64-bit, x64-based processor |

The system contained both a dedicated NVIDIA GPU and an integrated Intel GPU.

The preserved records confirm that both adapters were installed but do not conclusively identify which graphics adapter Unity selected during each experimental execution. Therefore, this document records the available hardware without attributing the results exclusively to either GPU.

---

## Operating system

| Parameter | Value |
|---|---|
| Operating system | Windows 11 Home Single Language |
| Version | 25H2 |
| Recorded OS build | 26200.8973 |
| System type | 64-bit |

Individual device-identification information such as device ID, product ID, and serial number is intentionally excluded because it is not required for methodological reproduction.

---

## Unity

The experimental prototype was developed and executed using Unity.

| Parameter | Value |
|---|---|
| Game engine | Unity |
| Version | 6000.3.0f1 |
| Programming language | C# |
| Configured Build Target | Windows Standalone |
| Render Pipeline | Universal Render Pipeline (URP) |
| Active Quality Profile | PC |

All six procedural generators were implemented within the same experimental Unity scene.

---

## Experimental execution mode

The experimental batches were not executed using a compiled build.

Tests were performed directly inside the Unity Editor and outside `Play Mode`.

Execution was triggered through the custom buttons available in the Inspector of each generator, including the routines responsible for the measurement batches.

The experimental procedure therefore consisted of:

- opening the experimental scene in Unity;
- keeping the Editor outside `Play Mode`;
- selecting the component corresponding to the algorithm;
- starting the batch through the measurement button available in the Inspector;
- allowing the system to process the configured seeds automatically;
- recording and exporting the results produced by the instrumentation system.

The complete procedure is documented in `ExperimentProtocol.md`.

This condition should be considered when interpreting the recorded execution times. The results represent execution within the Unity Editor and should not be interpreted as absolute performance values for an optimized final build.

---

## Game View resolution

The Unity `Game` window was configured as:

| Parameter | Value |
|---|---:|
| Resolution | 1920 × 1080 |
| Displayed profile | Full HD |
| Display | Display 1 |

This resolution was maintained as the visual reference for the experimental environment.

---

## Quality profile

The active quality level was named `PC`.

| Parameter | Configuration |
|---|---|
| Quality Level | PC |
| Build Target | Windows Standalone |
| Render Pipeline Asset | `PC_RPAsset` |
| Resolution Scaling Fixed DPI Factor | 1 |
| Realtime GI CPU Usage | Unlimited |
| VSync Count | Don't Sync |
| Global Mipmap Limit | Full Resolution |
| Anisotropic Textures | Forced On |
| Mipmap Streaming | Disabled |

Vertical synchronization remained disabled.

---

## Universal Render Pipeline

The project used the `PC_RPAsset` Universal Render Pipeline asset.

### Rendering configuration

| Parameter | Value |
|---|---|
| Renderer | `PC_Renderer` |
| Depth Texture | Enabled |
| Opaque Texture | Enabled |
| Opaque Downsampling | 2x Bilinear |
| Terrain Holes | Enabled |
| GPU Resident Drawer | Disabled |

### Quality

| Parameter | Value |
|---|---|
| HDR | Enabled |
| Anti Aliasing (MSAA) | Disabled |
| Render Scale | 1 |
| Upscaling Filter | Automatic |
| LOD Cross Fade | Enabled |
| LOD Cross Fade Dithering | Blue Noise |

A render scale of 1 means that the reference resolution was used without proportional reduction through this parameter.

---

## Lighting and shadows

### Main light

| Parameter | Value |
|---|---|
| Main Light Rendering | Per Pixel |
| Cast Shadows | Enabled |
| Shadow Resolution | 2048 |

### Additional lights

| Parameter | Value |
|---|---|
| Additional Lights | Per Pixel |
| Per Object Limit | 4 |
| Cast Shadows | Enabled |
| Shadow Atlas Resolution | 2048 |

### Shadows

| Parameter | Value |
|---|---|
| Maximum Shadow Distance | 50 |
| Cascade Count | 4 |
| Soft Shadows | Enabled |
| Soft Shadow Quality | High |
| Depth Bias | 0.1 |
| Normal Bias | 0.5 |

These settings remained associated with the same graphical profile used by the different generators.

---

## Post-processing

The project used a `Global Volume` with the `SampleSceneProfile` profile.

### General configuration

| Parameter | Value |
|---|---|
| Grading Mode | Low Dynamic Range |
| LUT Size | 32 |
| Volume Update Mode | Every Frame |

### Tonemapping

| Parameter | Value |
|---|---|
| Tonemapping | Enabled |
| Mode | Neutral |

### Bloom

| Parameter | Value |
|---|---:|
| Bloom | Enabled |
| Threshold | 1 |
| Intensity | 0.25 |
| Scatter | 0.5 |
| High Quality Filtering | Enabled |

### Motion Blur

| Parameter | Value |
|---|---|
| Motion Blur | Disabled |

Although internal parameters remained visible in the profile, the effect itself was disabled and therefore did not contribute to the final representation.

### Vignette

| Parameter | Value |
|---|---:|
| Vignette | Enabled |
| Intensity | 0.2 |

Post-processing remained common to the experimental scene and was not changed between the batches of different algorithms.

---

## Relationship with performance measurements

The settings documented here are particularly relevant to the values associated with visual instantiation.

The experimental comparison separately records:

- logical layout generation;
- 3D geometry instantiation;
- metric calculation;
- total measured time.

Logical generation time mainly represents the computation required for each algorithm to construct its procedural structure.

Visual instantiation time, on the other hand, may also be influenced by the Unity environment, the number of created objects, the 3D resources, and the graphical conditions described in this document.

For this reason, the dissertation reports logical and visual costs separately.

---

## Reproducibility considerations

Reproducing the performance measurements on another computer does not require the resulting execution times to be numerically identical.

Differences in processor, memory, GPU, operating-system version, Unity version, and Editor state may affect absolute timing values.

For reproduction purposes, the primary objective is to preserve:

- the same Unity version whenever possible;
- the same scripts;
- the same algorithm parameters;
- the same seeds;
- the same generation configuration;
- the same measurement protocol.

The values obtained in this study should primarily be interpreted as an internal comparison between algorithms executed under the same experimental environment.

Algorithm settings are documented in `AlgorithmParameters.md`, while shared batch conditions are documented in `ExperimentalConfiguration.md`.

---

## Environment summary

| Element | Configuration |
|---|---|
| Computer | Lenovo IdeaPad Gaming 3 15IHU6 |
| CPU | Intel Core i5-11300H @ 3.10 GHz |
| RAM | 24 GB |
| Available dedicated GPU | NVIDIA GeForce RTX 3050 Laptop GPU 4 GB |
| Available integrated GPU | Intel Iris Xe Graphics |
| Operating system | Windows 11 Home Single Language 64-bit |
| Windows version | 25H2 |
| Unity | 6000.3.0f1 |
| Execution | Unity Editor |
| Play Mode | Not used |
| Compiled build | Not used |
| Game View resolution | 1920 × 1080 |
| Build Target | Windows Standalone |
| Render Pipeline | Universal Render Pipeline |
| Quality Level | PC |
| Render Scale | 1 |
| HDR | Enabled |
| MSAA | Disabled |
| VSync | Disabled |
| Main Shadow Resolution | 2048 |
| Soft Shadows | High |
| Tonemapping | Neutral |
| Bloom | Enabled, intensity 0.25 |
| Motion Blur | Disabled |
| Vignette | Enabled, intensity 0.2 |
