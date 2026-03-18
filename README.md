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

A "Tomato Architecture" é um conceito de arquitetura de software que surge como uma evolução prática de estilos arquiteturais bem conhecidos, como a **Clean Architecture** e a **Onion Architecture** .

Com base nas informações disponíveis, que são limitadas a discussões em comunidades de desenvolvedores, explico abaixo a sua origem e os seus princípios.

🍅 Origem e Motivação

A Tomato Architecture não é um padrão formalmente definido por um autor ou instituição específica. Em vez disso, ela parece ter emergido de **críticas e reflexões práticas** de desenvolvedores sobre a aplicação das arquiteturas Clean e Onion em projetos reais . A principal motivação para o seu surgimento é a percepção de que, embora essas arquiteturas tradicionais busquem um alto nível de abstração para isolar o código de regras de negócio de detalhes externos (como bancos de dados e APIs), essa abordagem pode, por vezes, tornar-se "fora da realidade" para muitos projetos empresariais .

A crítica central que teria dado origem à Tomato Architecture é que as arquiteturas tradicionais focam demais em isolar o código para facilitar **testes de unidade** com mocks, mas acabam negligenciando a importância dos **testes de integração**, que são cruciais para garantir que o sistema funcione como um todo. A ideia por trás do "Tomato" seria, portanto, a de uma arquitetura mais "carnuda" e prática, onde a testabilidade efetiva (com integração real a componentes como bancos de dados) é um valor mais importante do que a pureza da abstração .

📐 Princípios da Tomato Architecture

Com base nessa motivação, a Tomato Architecture propõe algumas mudanças de ênfase em relação às suas antecessoras:

*   **Testes de Integração como Prioridade**: Em vez de confiar quase exclusivamente em testes de unidade com mocks, a Tomato Architecture incentiva o uso de ferramentas como **testcontainers** para executar testes de integração diretamente contra os serviços externos reais (como bancos de dados e filas). A ideia é que isso gera mais confiança no funcionamento do sistema .
*   **Pragmatismo nas Abstrações**: A arquitetura sugere evitar a criação de interfaces e camadas de abstração para "casos hipotéticos" de troca futura de tecnologias (ex: trocar de banco de dados), a menos que essa necessidade seja real e iminente. A premissa é que grandes frameworks ou provedores de infraestrutura raramente são trocados, e o esforço para abstraí-los pode não valer a pena .
*   **Camada de Negócios Mais Enxuta**: O objetivo é que a camada de negócios dependa o mínimo possível de detalhes de serviços externos, mas sem a rigidez extrema que pode tornar o desenvolvimento mais lento .

⚖️ Comparação com a Onion Architecture

A Onion Architecture, proposta por Jeffrey Palermo, organiza o código em camadas concêntricas ao redor do **Modelo de Domínio**. Sua regra fundamental é que as dependências fluem **para dentro**, ou seja, as camadas externas (Infraestrutura, UI) podem depender das camadas internas (Domínio, Aplicação), mas o núcleo nunca depende de nada externo. Isso é alcançado através do uso intenso de **interfaces** (abstrações) definidas nas camadas internas e implementadas nas externas (princípio da Inversão de Dependência) .

A Tomato Architecture pode ser vista como uma "evolução" prática ou uma variação da Onion Architecture ao **relaxar alguns de seus princípios mais rigorosos** em prol da praticidade e da efetividade dos testes. Enquanto a Onion prega a abstração total para proteger o domínio, a Tomato questiona o custo dessa abstração quando a tecnologia subjacente é estável. Enquanto a Onion viabiliza testes de unidade puros, a Tomato argumenta que testes de integração bem-feitos dão mais segurança.

💡 Observações Importantes

É fundamental entender que a "Tomato Architecture" **não é um padrão amplamente documentado ou reconhecido formalmente** na literatura de engenharia de software. As informações sobre ela circulam principalmente em fóruns, posts de blog e discussões técnicas, como a encontrada no resultado de busca . Seu nome é uma metáfora que brinca com as outras arquiteturas "comestíveis" (Cebola/Onion, Limpa/Clean), sugerindo algo mais robusto e prático.

Portanto, você deve encará-la mais como uma **filosofia ou um conjunto de boas práticas** que surgiu da experiência de desenvolvedores do que como uma receita de bolo com regras definidas.

Se você estiver interessado em explorar mais a fundo esse conceito, sugiro buscar por discussões em comunidades como o Reddit (r/programming, r/ExperiencedDevs) ou blogs de engenharia de software, onde esses tópicos são frequentemente debatidos.

A arquitetura assume que **sistemas reais são assim**:

![tomato-architecture](https://github.com/user-attachments/assets/a080719a-4aef-4bf9-ab41-f9d647082f8c)

*Como funciona na prática*: A Tomato organiza o sistema em **domínios**, cada um com seu nível de maturidade. Dentro de cada domínio:

* A API expõe comandos e queries.
* Para leitura, usa modelos diretos, rápidos e orientados a performance.
* Para escrita, pode usar agregados, serviços de domínio e eventos.
* Infrastructure não é demonizada — é utilizada de forma prática.
* Barramentos de evento são tratados como parte natural do domínio.

Aplicação completa em ASP.NET Core com Tomato Architecture (variação da Onion Architecture) aplicada a microsserviços. A estrutura terá camadas concêntricas como um tomate: `Seed (Domain) → Pulp (Application) → Flesh (Infrastructure) → Skin (API)`:
