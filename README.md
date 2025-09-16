# Projeto de Agendamento e Envio de E-mails

Este é um projeto acadêmico para a disciplina de Sistemas Distribuídos, demonstrando uma arquitetura de microserviços para agendar e enviar e-mails de forma assíncrona.

O sistema permite que um usuário envie os detalhes de um e-mail através de uma interface web. Esses detalhes são salvos em um banco de dados por um serviço de gerenciamento. Um segundo serviço, operando em segundo plano, monitora o banco de dados, processa os agendamentos na hora correta e envia os e-mails através de um provedor SMTP externo.

## Arquitetura do Sistema

O projeto é composto por 4 serviços independentes, orquestrados com Docker Compose:

1.  **Frontend (NGINX):** Um servidor web NGINX que serve a interface de usuário estática (HTML/JS) e atua como um proxy reverso para a API de gerenciamento.
2.  **API de Gerenciamento (.NET):** Uma API web ASP.NET Core responsável por receber os agendamentos de e-mail via endpoint REST e persisti-los no banco de dados.
3.  **Worker de Envio (.NET):** Um serviço de trabalho em segundo plano (.NET Worker Service) que monitora o banco de dados em intervalos regulares, busca por e-mails agendados que já venceram e os envia usando um serviço SMTP.
4.  **Banco de Dados (PostgreSQL):** O banco de dados relacional que serve como a fonte de verdade compartilhada entre a API de Gerenciamento e o Worker de Envio.

### Fluxo de Dados
```
[Usuário] -> [Frontend (NGINX)] -> POST /api/submit -> [API de Gerenciamento] -> Escreve no -> [Banco de Dados]
                                                                                                  ^
                                                                                                  |
                                                                                           Lê do (a cada minuto)
                                                                                                  |
                                                                                          [Worker de Envio] -> Envia E-mail -> [Servidor SMTP (Brevo)]
                                                                                                  |
                                                                                                  '-> Atualiza "IsSent=true" no -> [Banco de Dados]
```

## Tecnologias Utilizadas
* **Backend:** .NET 8, C#, ASP.NET Core (Web API, Worker Service), Entity Framework Core
* **Frontend:** HTML5, JavaScript (Vanilla), Bootstrap 5
* **Banco de Dados:** PostgreSQL
* **Infraestrutura:** Docker, Docker Compose
* **Envio de E-mail:** MailKit, Brevo (SMTP Relay)
* **Padrões de Arquitetura:** Microserviços, Repositório Compartilhado (Class Library), Injeção de Dependência, Health Checks.

## Como Executar o Projeto

### Pré-requisitos
* [Docker](https://www.docker.com/products/docker-desktop/)
* [Docker Compose](https://docs.docker.com/compose/install/)
* .NET SDK 8 (Apenas para gerar migrations ou rodar localmente fora do Docker)

### Passos para a Instalação

1.  **Clone o Repositório**
    ```bash
    git clone [https://github.com/seu-usuario/seu-repositorio.git](https://github.com/seu-usuario/seu-repositorio.git)
    cd seu-repositorio
    ```

2.  **Crie o arquivo de Configuração (`.env`)**
    Copie o arquivo de exemplo `.env.example` para criar seu próprio arquivo de configuração local.
    ```bash
    # No Windows (PowerShell)
    cp .env.example .env

    # No Linux ou macOS
    cp .env.example .env
    ```

3.  **Preencha o Arquivo `.env`**
    Abra o arquivo `.env` e preencha com suas próprias credenciais do banco de dados e do provedor SMTP (Brevo).

4.  **Inicie os Contêineres**
    Use o Docker Compose para construir as imagens e iniciar todos os serviços.
    ```bash
    docker-compose up --build
    ```
    * Adicione a flag `-d` para rodar em segundo plano (`docker-compose up --build -d`).

5.  **Acesse a Aplicação**
    Abra seu navegador e acesse `http://localhost:8080`. O formulário de agendamento de e-mail estará disponível.

## Configuração (`.env`)

As seguintes variáveis de ambiente precisam ser definidas no arquivo `.env`:

| Variável | Descrição | Exemplo |
| :--- | :--- | :--- |
| `DATABASE_HOST` | Nome do serviço do banco de dados no Docker Compose. | `db_gerenciamento` |
| `DATABASE_PORT` | Porta do PostgreSQL. | `5432` |
| `DATABASE_NAME` | Nome do banco de dados a ser criado. | `gerenciamento` |
| `DATABASE_USER` | Usuário para acessar o banco de dados. | `admin` |
| `DATABASE_PASSWORD`| Senha para o usuário do banco de dados. | `senha-forte-aqui` |
| `SmtpSettings__Host` | Endereço do servidor SMTP. | `smtp-relay.brevo.com` |
| `SmtpSettings__Port` | Porta do servidor SMTP. | `587` |
| `SmtpSettings__Username`| Usuário de login do serviço SMTP (seu e-mail da Brevo).| `seu-email@provedor.com` |
| `SmtpSettings__Password`| Senha ou Chave de API do serviço SMTP. | `sua-chave-smtp-da-brevo`|
| `SmtpSettings__SenderEmail`| E-mail do remetente (precisa ser validado na Brevo). | `seu-email@provedor.com` |
| `SmtpSettings__SenderName` | Nome do remetente que aparecerá no e-mail. | `Minha Aplicação` |

## Próximos Passos e Melhorias Futuras
- [ ] Criar a funcionalidade "Enviar Agora" com um endpoint `POST /api/schedules/{id}/send-now` para demonstrar a comunicação síncrona direta entre os serviços.
- [ ] Adicionar testes unitários e de integração.
