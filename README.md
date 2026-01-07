# FCG Purchase Service

Este é o serviço central de processamento de pedidos e pagamentos do FCG. Ele foi projetado com uma arquitetura **Event-Driven** (Orientada a Eventos) para garantir alta escalabilidade, desacoplamento e resiliência.

## Arquitetura e Fluxo Assíncrono

O diferencial deste serviço é a implementação de um fluxo de **Event Sourcing** simplificado. O processamento de uma compra não é linear; ele é dividido em estágios independentes orquestrados via mensagens no **RabbitMQ**.

### Detalhamento do Fluxo (Deep Dive)

O ciclo de vida de uma compra passa pelas seguintes etapas no código:

#### 1. API: Recepção e Publicação (Síncrono -> Assíncrono)
- O endpoint `POST` recebe o pedido.
- Valida os dados do usuário e preços (chamadas HTTP internas).
- Gera um **Event Stream** inicial no banco de dados (Event Store) com o evento `CompraIniciadaEvent`.
- Publica este evento no barramento (RabbitMQ) via **MassTransit**.
- Retorna `200 OK` para o cliente (ID da compra), liberando a conexão HTTP.

#### 2. Worker: Processamento de Pagamento (`CompraIniciadaConsumer`)
Um consumidor dedicado escuta o evento de início.
- **Auditoria:** Grava o evento `ProcessandoPagamentoEvent` no histórico.
- **Gateway Simulado:** Executa uma lógica de simulação de pagamento (com probabilidade de 80% de aprovação e 20% de recusa).
- **Decisão:** Dependendo do resultado, ele publica um novo evento na fila: `PagamentoAprovadoEvent` ou `PagamentoRecusadoEvent`.
- *Nota: O Worker não grava o status final no banco relacional aqui, ele apenas emite o fato ocorrido.*

#### 3. Worker: Consolidação e Leitura (`AtualizaStatusCompraConsumer`)
Um segundo consumidor escuta os eventos de resultado (Aprovado/Recusado) para finalizar o processo.
- **Reconstituição de Estado:** Ao receber, por exemplo, um `PagamentoAprovadoEvent`, o worker consulta o Event Store usando o ID da transação para buscar os dados originais do pedido (jogos, valores, usuário) que estavam no evento inicial.
- **Persistência (Read Model):** Com os dados reconstituídos, ele grava o registro definitivo na tabela de leitura `TransacaoJogosEntity` com o status `Finalizado` ou `ErroProcessamento`.
- **Conclusão:** Salva o evento `CompraFinalizadaEvent` para fechar a stream.

## Endpoints e Permissões

| Recurso | Método | Rota | Permissão | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **Compras** | `POST` | `/api/v1/compras` | Autenticado | Inicia compra de jogos (Assíncrono) |
| | `GET` | `/api/v1/compras/{compraId}` | Autenticado | Consulta status da compra |
| **Transações**| `GET` | `/api/v1/transacoes/{userId}` | Público | Histórico de compras do usuário |
| **Métricas** | `GET` | `/api/v1/metricas/mais-vendidos` | Autenticado | Ranking de vendas |

## Stack Tecnológica

- **.NET 9**: Runtime de alta performance.
- **MassTransit**: Framework para orquestração de mensagens e consumers.
- **RabbitMQ**: Message Broker (Clusterizado em produção).
- **PostgreSQL**: Utilizado para duas funções:
  - **Event Store**: Tabela que armazena o histórico imutável de eventos.
  - **Read Database**: Tabelas relacionais otimizadas para consulta de status.
- **Kubernetes (HPA)**: A API e o Worker escalam de forma independente. Se a fila encher, o HPA sobe mais réplicas apenas do Worker.

## Como Executar

O repositório contém dois componentes que devem rodar simultaneamente:

1.  **API (`src/Fcg.Games.Purchase.Api`)**:
    Exposta na porta 8080.
2.  **Worker (`src/Fcg.Games.Purchasing.Worker`)**:
    Console Application (Daemon) que fica escutando o RabbitMQ.

Configurações de conexão (RabbitMQ/Postgres) devem ser ajustadas no `appsettings.json` ou via Variáveis de Ambiente.