# 🍅 Tomato architecture
<img src="https://img.shields.io/badge/Jira-Pomodoro-tomato?style=flat&logo=Jira&logoColor=white"> <img src="https://img.shields.io/badge/ClickUp-Pomodoro-tomato?style=flat&logo=ClickUp&logoColor=white"> <img src="https://img.shields.io/badge/Trello-Pomodoro-tomato?style=flat&logo=Trello&logoColor=white"> <img src="https://img.shields.io/badge/Asana-Pomodoro-tomato?style=flat&logo=Asana&logoColor=white"> <img src="https://img.shields.io/badge/Redmine-Pomodoro-tomato?style=flat&logo=Redmine&logoColor=white"> <img src="https://img.shields.io/badge/Clockify-Pomodoro-tomato?style=flat&logo=clockify&logoColor=white">

<img src="https://em-content.zobj.net/source/microsoft-teams/363/tomato_1f345.png" height="77" align="right">

A **Tomato Architecture** é uma evolução pragmática das arquiteturas **Clean**, **Onion** e **Hexagonal**, criada para lidar com a realidade de sistemas modernos distribuídos, orientados a eventos, altamente integrados, com múltiplos modelos de escrita/leitura, APIs, filas, streams e domínios que mudam ao longo do tempo. É uma tentativa de pegar o melhor das arquiteturas clássicas e torná-las realmente práticas no dia a dia, evitando o excesso de abstração e o “purismo arquitetural” que muitas vezes torna Clean/Hexagonal difíceis de aplicar em escala. Se Clean/Hexagonal são teorias sobre pureza, Tomato é prática sobre sobrevivência e evolução de sistemas reais.

A Tomato Architecture nasce da percepção de que sistemas reais são “vivos”, orgânicos, mudam de forma, crescem de maneira desigual e possuem partes que amadurecem em ritmos diferentes, exatamente como um tomate crescendo em camadas, com áreas mais verdes e áreas mais vermelhas coexistindo. A ideia central é que o software não precisa nascer 100% Clean, Onion ou Hexagonal. Ele pode amadurecer aos poucos, aplicando princípios de isolamento de domínio somente onde isso realmente traz benefício concreto.

É uma arquitetura baseada em **maturidade por camadas**, e não apenas em separação por dependências. Enquanto Onion e Hexagonal colocam regras fixas para quem depende de quem, a <a href="https://www.sivalabs.in/blog/tomato-architecture-pragmatic-approach-to-software-design/">Tomato</a> introduz o conceito de **maturidade arquitetural**, permitindo que cada módulo esteja em um nível diferente, variando do mais simples ao mais sofisticado conforme sua importância no domínio.

*Como ela evolui Clean, Onion e Hexagonal*: O Clean Architecture define camadas puras e independentes. O Onion reforça o papel do domínio no centro e a separação rígida de infraestrutura. O Hexagonal enfatiza portas e adaptadores, abrindo comportamentos para o mundo externo via interfaces. A <a href="https://tomato-architecture.github.io/">Tomato</a> olha para tudo isso e diz: “Ótimo, mas e na prática?”. Então ela reduz a complexidade desnecessária e cria uma evolução mais realista:

Ela mantém a ideia de **domínio como núcleo**, mas flexibiliza o que está em volta. Ou seja, você não precisa criar dezenas de ports e adapters para operações triviais, e pode aceitar integrações diretas quando o custo de um isolamento extremo não compensa. Ela também permite que partes do sistema funcionem sem domínio formal, algo impossível no Clean tradicional, reconhecendo que em sistemas modernos alguns módulos são apenas orquestração, queries, relatórios ou transformações simples. Além disso, ela mistura CQRS, Event-Driven e Domain Events como mecanismos naturais, não como plugins opcionais — algo que Clean/Onion falam pouco.

O conceito central: **“maturidade arquitetural por tomate”**: Em vez de dividir o sistema rigidamente em camadas concêntricas (como na Onion) ou por portas/plugues (como no Hexagonal), a Tomato o divide em **fatias de maturidade**. Cada parte do sistema pode estar em um nível diferente:

<img height="266" align="right" src="https://github.com/user-attachments/assets/fdac4440-d039-4361-9a5e-27b4659d277e" />

- **Tomate Verde**: Módulos simples, CRUDs, endpoints de infraestrutura, autenticação, configurações, health checks, integração simples. Nada de complexidade desnecessária. Apenas o essencial.

- **Tomate Meio Maduro**: Serviços que começam a ter regras de negócio, leves validações, pequenas entidades, alguns eventos. Começa a se aproximar de DDD, mas sem as camadas rígidas.

- **Tomate Maduro**: Partes realmente críticas do domínio, onde invariantes são fortes, lógica complexa existe, múltiplos agregados interagem, e onde faz sentido aplicar DDD tático completo (Aggregates, Domain Services, Value Objects, Domain Events).

> [!Note]
> Lembra o **Rotten Tomatoes**, e essa associação não é coincidência, porque o próprio nome Tomato Architecture brinca com essa ideia de camadas, maturidade e gradação do mesmo jeito que o Rotten Tomatoes faz ao avaliar filmes. Mas, conceitualmente, a semelhança é ainda mais interessante do que parece à primeira vista. A lógica do Rotten Tomatoes é baseada na mistura de opiniões, contextos diferentes, níveis de qualidade distintos e perspectivas que se combinam para avaliar e criar um resultado final coerente. Isso é exatamente o espírito que a Tomato Architecture incorpora na engenharia de software. Ela parte da premissa de que um sistema grande nunca é inteiramente homogêneo, nunca nasce perfeito e nunca se mantém estático. Ele vai acumulando partes de qualidade diferente, algumas mais maduras, outras mais imaturas, e tudo isso convive ao mesmo tempo dentro da mesma aplicação — assim como os tomates verdes e vermelhos num mesmo cacho.

Isso resolve o maior problema das arquiteturas puristas: a ideia de que “tudo é Clean” ou “tudo é Hexagonal”. No Tomato, apenas o que realmente precisa ser maduro, é maduro.

Por que o nome “Tomato”? A metáfora é proposital. Um tomate tem:

* um **centro forte e uniforme** (o domínio maduro),
* camadas ao redor com texturas e resistências diferentes,
* partes verdes coexistindo com partes vermelhas,
* amadurecimento progressivo,
* e uma forma natural de crescimento, não simétrica.

A arquitetura assume que **sistemas reais são assim**:

![tomato-architecture](https://github.com/user-attachments/assets/a080719a-4aef-4bf9-ab41-f9d647082f8c)

*Como funciona na prática*: A Tomato organiza o sistema em **domínios**, cada um com seu nível de maturidade. Dentro de cada domínio:

* A API expõe comandos e queries.
* Para leitura, usa modelos diretos, rápidos e orientados a performance.
* Para escrita, pode usar agregados, serviços de domínio e eventos.
* Infrastructure não é demonizada — é utilizada de forma prática.
* Barramentos de evento são tratados como parte natural do domínio.

Aplicação completa em ASP.NET Core com Tomato Architecture (variação da Onion Architecture) aplicada a microsserviços. A estrutura terá camadas concêntricas como um tomate: `Seed (Domain) → Pulp (Application) → Flesh (Infrastructure) → Skin (API)`:
