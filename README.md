# TaskAPI - Gerenciador de Tarefas com Chatbot 
![.NET](https://img.shields.io/badge/.NET_10-512BD4?style=flat&logo=dotnet&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-0078D4?style=flat&logo=microsoftazure&logoColor=white)
![Power Automate](https://img.shields.io/badge/Power_Automate-0066FF?style=flat&logo=powerautomate&logoColor=white)
![Copilot Studio](https://img.shields.io/badge/Copilot_Studio-7B2FBE?style=flat&logo=microsoft&logoColor=white)
![EF Core](https://img.shields.io/badge/EF_Core-512BD4?style=flat&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat&logo=microsoftsqlserver&logoColor=white)

Projeto de estudo para consolidar conhecimentos em desenvolvimento de Web APIs RESTful com C# e automação com Power Automate e Copilot Studio, com deploy na nuvem via Microsoft Azure.

## Visão Geral
APITask é um sistema de gerenciamento de tarefas construído do zero, com foco em demonstrar a integração entre diferentes tecnologias do ecossistema Microsoft:
- Uma API REST desenvolvida em C# com .NET 10 para gerenciar tarefas
- Deploy na nuvem Azure com App Service e SQL Database
- Fluxos no Power Automate que consomem a API via HTTP
- Um chatbot no Copilot Studio que integra os fluxos e permite gerenciar tarefas via chat

### Arquitetura
<img width="2900" height="230" alt="mermaid-diagram (1)" src="https://github.com/user-attachments/assets/d10e37a5-6478-4ebc-9b64-eb84bc3f6ebf" />

- O usuário interage com o TaskBot no Copilot Studio via chat
- O Copilot Studio aciona um fluxo no Power Automate conforme o tópico identificado
- O Power Automate faz uma chamada HTTP para a API hospedada no Azure
- A API processa a requisição e persiste os dados no Azure SQL Database
- A resposta retorna pelo mesmo caminho até o chat do usuário

### Stack
| Camada         | Tecnologia                         |
|----------------|------------------------------------|
| API            | C# / .NET 10 / ASP.NET Core        |
| ORM            | Entity Framework Core              |
| Banco de dados | Azure SQL Database (SQL Server)    |
| Hospedagem     | Azure App Service (Web App)        |
| Automação      | Power Automate                     |
| Chatbot        | Copilot Studio                     |

### Endpoints da API
| Método  | Rota                                       | Descrição                         |
|---------|--------------------------------------------|-----------------------------------|
| `GET`   | /api/tasks                                 | Lista todas as tarefas            |
| `GET`   | /api/tasks?status={valor}                  | Filtra por status                 |
| `GET`   | /api/tasks?priority={valor}                | Filtra por prioridade             |
| `GET`   | /api/tasks?priority={valor}&status={valor} | Filtra por prioridade e status    |
| `GET`   | /api/tasks/{id}                            | Busca tarefa por ID               |
| `POST`  | /api/tasks                                 | Cria uma nova tarefa              |
| `PATCH` | /api/tasks/{id}                            | Atualiza parcialmente uma tarefa  |
| `DELETE`| /api/tasks/{id}                            | Deleta uma tarefa                 |

### Exemplo de payload - POST /api/tasks
```json
{
  "title": "Revisar documentação",
  "description": "Revisar e atualizar os endpoints da API",
  "priority": 2
}
```
### Exemplo de resposta
```json
{
  "taskItemId": 1,
  "title": "Revisar documentação",
  "description": "Revisar e atualizar os endpoints da API",
  "status": "Pending",
  "priority": "High",
  "createdAt": "2026-04-28T18:33:33.0000000"
}
```

### Valores de status

| Valor | Descrição                     |
|-------|-------------------------------|
|   0   | Pending (Pendente)            |
|   1   | InProgress (Em progresso)     |
|   2   | Done (Concluída)              |
|   3   | Cancelled (Cancelada)         |

### Valores de prioridade

| Valor | Descrição         |
|-------|-------------------|
|   0   | Low (Baixa)       |
|   1   | Medium (Média)    |
|   2   | High (Alta)       |

##  Fluxos no Power Automate

### Flow A — Relatório diário de tarefas
- Tipo: Agendado (diário)
- Trigger: Recorrência — todo dia às 9h (Horário de Brasília)
- Ação: GET /api/tasks → filtra tarefas Pending → envia e-mail
- Destaque: Tarefas de prioridade High são exibidas em vermelho
<img width="467" height="284" alt="image" src="https://github.com/user-attachments/assets/85170eb0-5aa4-442d-9023-809e68b74bfb" />



### Flow B — Escalação automática de prioridade
- Tipo: Agendado (diário)
- Trigger: Recorrência — todo dia às 9h
- Ação: GET /api/tasks → filtra tarefas Pending com prioridade Low ou Medium criadas há mais de 3 dias → PATCH /api/tasks/{id} atualizando para High → notificação por e-mail
<img width="729" height="359" alt="image" src="https://github.com/user-attachments/assets/82aed7de-5c7e-4e99-8656-f99468e635a3" />

### Fluxos do Power Automate integrados com Copilot Studio (utilizados internamente)
Cada um dos fluxos abaixo é responsável por uma ação após ser acionado pelo chatbot via Copilot Studio

| Fluxo          | Método     | Endpoint          |
|----------------|------------|-------------------|
|   CreateTask   | `POST`     | /api/tasks        |
|   GetAllTasks  | `GET`      | /api/tasks        |
|   GetTaskById  | `GET`      | /api/tasks/{id}   |
|   PatchTask    | `PATCH`    | /api/tasks/{id}   |
|   DeleteTask   | `DELETE`   | /api/tasks/{id}   |

## Chatbot - TaskBot (Copilot Studio)
O TaskBot é um agente conversacional que permite gerenciar tarefas via chat em linguagem natural

### Funcionalidades
- Criar tarefas informando título, descrição e prioridade
- Listar todas as tarefas
- Listar tarefas aplicando filtro (status e/ou prioridade)
- Listar uma tarefa por Id 
- Atualizar o status de uma tarefa
- Deletar tarefas

### Exemplo de interação

[![Demo](https://img.youtube.com/vi/D6BxqQSE-Lg/0.jpg)](https://www.youtube.com/watch?v=D6BxqQSE-Lg)

## Rodando localmente
### Pré-requisitos
- .NET 10 SDK
- SQL Server ou Azure SQL Database
- Git

#### Clonar o repositório
```bash
git clone https://github.com/zVilanova/APITask.git
cd APITask/APITask
dotnet restore
```

#### Configuração da connection string
No appsettings.json (desenvolvimento local):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=TaskManagerDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```
Em produção, a connection string é configurada diretamente nas variáveis de ambiente do Azure App Service - sem expor credenciais no repositório

#### Migrations
```bash
dotnet ef database update (caso não tenha EF CLI instalado: dotnet tool install --global dotnet-ef)
```
#### Executar aplicação
```bash
dotnet run
```
A API estará disponível em https://localhost:7110, você pode testar os endpoints utilizando Postman

#### Publicar no Azure
Via Visual Studio:

- Clique com botão direito no projeto → Publish
- Selecione o perfil Azure App Service
- Clique em Publish

O Visual Studio compila em modo Release e publica automaticamente. As migrations são aplicadas no banco Azure durante o deploy.

## Próximas melhorias
- [ ] Autenticação e autorização com JWT
- [ ] Testes unitários nos Controllers
- [ ] Documentação interativa com Scalar
