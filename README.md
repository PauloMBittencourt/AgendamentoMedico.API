# Sistema de Agendamento Médico

Este repositório contém a implementação de um sistema de agendamento médico, desenvolvido em ASP.NET Core com Entity Framework Core (migrations) e Razor Views para o frontend. O sistema permite que médicos cadastrem seus horários disponíveis e que pacientes visualizem e agendem consultas de forma segura.

---

## Tecnologias Utilizadas

- **Backend**  
- .NET 6 (ou superior) SDK  
- ASP.NET Core MVC  
- Entity Framework Core (Code-First + Migrations)  
- C#  

- **Banco de Dados**  
- SQL Server (padrão, mas pode-se adaptar para PostgreSQL)  

- **Frontend**  
- Razor Views (ASP.NET Core)  
- Bootstrap 5 (via CDN)  
- FullCalendar 6 (via CDN)  
- JavaScript (Fetch API)  

- **Autenticação e Autorização**  
- ASP.NET Core Identity  
- Roles: `Administrador`, `Médico`, `Cliente`  

- **Dependências Adicionais**  
- Microsoft.EntityFrameworkCore.SqlServer  
- Microsoft.EntityFrameworkCore.Tools  
- AspNetCoreHero.ToastNotification (para notificações de sucesso/erro)  
- (Opcional) Swagger/Swashbuckle para documentação de API  

---

## Pré-requisitos

1. **.NET 6 (ou superior) SDK**  
 Baixar em: https://dotnet.microsoft.com/download

2. **SQL Server**  
 - Pode usar SQL Server Express ou Developer Edition  
 - Se preferir PostgreSQL, ajuste o provider e a string de conexão no `appsettings.json`.

3. **IDE/Editor**  
 - Visual Studio 2022 (ou superior) ou VS Code  

4. **Navegador**  
 - Chrome, Firefox, Edge etc. (para testar as Views com FullCalendar e Bootstrap)

---

## Configuração do Banco de Dados

1. **String de Conexão**  
 No arquivo `appsettings.json`, configure a seção `ConnectionStrings:DefaultConnection`. Por exemplo, para SQL Server LocalDB:
 ```json
 "ConnectionStrings": {
   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AgendamentoMedicoDb;Trusted_Connection=True;MultipleActiveResultSets=true"
 }
 OBS: Isso é apenas para Localhost e testes de Dev, pois não será uma falha de segurança quando subir o projeto para produção usando uma ferramenta de CI/CD como Azure podendo configurar as Appsettings por pipelines 

2. **Migrations e Atualização do Banco**
 Abra o Package Manager Console na pasta do projeto `AgendamentoMedico.Infra` e execute:

 Update-Database
```

2. **Seed de Roles e Admin**
 No `AppDbContext.OnModelCreating`, já existem seeds para:

 * Role `Administrador` (ID = `11111111-1111-1111-1111-111111111111`)
 * Role `Cliente`       (ID = `33333333-3333-3333-3333-333333333333`)
 * Usuário Admin “admin” com senha criptografada
   Ao executar `Update-Database`, esses registros são inseridos automaticamente.

 A senha do Adm tá no código para fazer os testes, favor verificar no AppDbContext.cs, não é uam falha de segurança pois eu somente usei as Seeds para ficar mais prático para vocês testarem, em um caso normal eu criaria usando SQL mesmo para melhor segurança

---

## Como Executar o Sistema Localmente

1. **Clonar o Repositório**

 ```bash
 git clone https://github.com/SeuUsuario/AgendamentoMedico.git
 cd AgendamentoMedico/src/AgendamentoMedico.API
 ```

2. **Configurar `appsettings.json`**

 * Ajuste a `ConnectionStrings:DefaultConnection` conforme seu ambiente (SQL Server ou PostgreSQL).

3. **Aplicar Migrations**

No Package Manager Console 
 ```bash
 Update-Database
 ```

5. **Executar a Aplicação**

 * Por padrão:

   * front-end em `https://localhost:5001`
   * back-end em `http://localhost:5000`

6. **Acessar no Navegador**

 * Abra `https://localhost:5001`.
 * Faça login com:
   * **Usuário:** `admin`
   * **Senha:** `SenhaForte@123`
 * Para cadastrar novos funcionários e médicos, use as rotas de registro ou endpoints dedicados.
 * Para clientes pode cadastrar na própria tela de Login no link "Cadastro"

---

## Funcionalidades Principais

### 1. Registro e Login

* **Registrar Cliente**

* Rota: `/Account/Register`
* Cria registros em `Usuarios` e `Clientes`, vinculados via `UsuarioId`.

* **Login**

* Rota: `/Account/Login`
* Após autenticar, redireciona conforme role (`Médico` ou `Cliente`).

* **Cadastrar Médico**

* Geralmente feito pelo Admin ou via endpoint específico (não público).
* Cria registros em `Usuarios` e em `Funcionarios`, vinculados via `UsuarioId`.

---

### 2. Área do Médico

* **Dashboard do Médico** (`/Medico/Dashboard`)

* **Calendário (FullCalendar):**

  * Carrega via AJAX:

    * `GET /Medico/GetMeusHorariosDisponiveis` → retorna JSON com `{ id, start, end }` para cada slot disponível.
    * `GET /Medico/GetMeusAgendamentos`   → retorna JSON com `{ id, dataHora, pacienteNome, status }` para agendamentos já feitos.
  * **Criar Horário Disponível:**

    * Clique e arraste para selecionar intervalo (30 minutos).
    * Abre modal de confirmação; “Confirmar” faz `POST /Medico/SalvarHorarioDisponivel` com `{ start: "...", end: "..." }`.
  * **Remover Horário Disponível:**

    * Clique num evento verde; abre modal.
    * “Remover” faz `POST /Medico/RemoverHorarioDisponivel` com `{ id: "<GUID>" }`.

* **Tabela de Agendamentos:**

  * Atualizada via `GET /Medico/GetMeusAgendamentos` (JSON → HTML via JS).
  * Colunas: Data/Hora, Paciente, Status.

---

### 3. Área do Paciente

* **Marcar Consulta** (`/Home/AgendarConsulta`)

* **Calendário (FullCalendar):**

  * Carrega todos os slots disponíveis (de todos os médicos) via:

    * `GET /Home/GetHorariosDisponiveis` → retorna JSON com `{ id: "<GUID>", start: "...", end: "...", title: "Nome do Médico" }`.
  * **Agendar Consulta:**

    * Clique num evento verde; abre modal de confirmação.
    * “Confirmar” faz `POST /Home/Agendar` com `{ horarioId: "<GUID>" }`.

      * Backend usa `_usuarioContextService.ObterClienteLogadoAsync(User)` para obter `Cliente.Id`.
      * Chama `AgendamentoService.AgendarConsultaAsync(horarioId, clienteId)`.
    * Após sucesso:

      * `calendar.refetchEvents()` remove o slot agendado.
      * Tabela “Meus Agendamentos” (abaixo) é recarregada via `GET /Home/GetMeusAgendamentos`.

* **Tabela Meus Agendamentos:**

* Populada por `GET /Home/GetMeusAgendamentos` → JSON com `{ dataHora, nomeMedico, status }`.
* Exibe data/hora formatada, nome do médico e status.
---

## Considerações Finais

* **Segurança**

* Senhas criptografadas via Identity.
* Endpoints protegidos por `[Authorize(Roles = "...")]`.
* Todas as operações de escrita (`POST`, `PUT`, `DELETE`) exigem `[ValidateAntiForgeryToken]`.

* **Escalabilidade**

* EF Core com Code-First + Migrations facilita evolução da base.
* Frontend leve (CDNs para Bootstrap e FullCalendar).
* Possível migrar para PostgreSQL ou outro provider apenas trocando provider e string de conexão.

* **Possíveis Melhorias**

* Adicionar testes unitários/integrados para controllers, services e repositórios.
* Implementar notificações em tempo real (SignalR) para avisar médicos sobre novos agendamentos instantaneamente.
* Refatorar Views em componentes front-end (Vue.js, React ou Blazor).
* Adicionar paginação e filtros nas tabelas de agendamentos.
* Implementar cancelamento e reagendamento de consultas.

---

## Autor e Contato

* **Nome:** Paulo Henrique Medeiros Bittencourt
* **E-mail:** [paulohenriquebitt@gmail.com](paulohenriquebitt@gmail.com)
* **Data de Criação:** 20/05/2025

---

**Licença:** MIT License
