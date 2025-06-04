# Sistema de Agendamento Médico

Este repositório contém a implementação de um sistema de agendamento médico, desenvolvido em ASP.NET Core (MVC) com Entity Framework Core (migrations) e Razor Views. O sistema permite que médicos cadastrem seus horários disponíveis e que pacientes visualizem e agendem consultas de forma segura.

---

## Tecnologias Utilizadas

* **Backend**

  * .NET 6 (ou superior) SDK
  * ASP.NET Core MVC
  * Entity Framework Core (Code-First + Migrations)
  * C#

* **Banco de Dados**

  * SQL Server (padrão; pode ser adaptado para PostgreSQL)
  * Migrations do EF Core

* **Frontend**

  * Razor Views (ASP.NET Core)
  * Bootstrap 5 (via CDN)
  * FullCalendar 6 (via CDN)
  * JavaScript (Fetch API)

* **Autenticação e Autorização**

  * ASP.NET Core Identity
  * Roles: `Administrador`, `Médico`, `Cliente`

* **Dependências Adicionais**

  * Microsoft.EntityFrameworkCore.SqlServer
  * Microsoft.EntityFrameworkCore.Tools
  * AspNetCoreHero.ToastNotification (notificações de sucesso/erro)
---

## Pré-requisitos

1. **.NET 6 (ou superior) SDK**
   Baixar em: [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

2. **SQL Server**

   * Pode usar SQL Server Express ou Developer Edition
   * Se preferir PostgreSQL, ajuste o provider e a string de conexão em `appsettings.json`

3. **IDE/Editor**

   * Visual Studio 2022 (ou superior) ou VS Code

4. **Navegador**

   * Chrome, Firefox, Edge etc. (para testar as Views com FullCalendar e Bootstrap)

---

## Configuração do Banco de Dados

1. **String de Conexão**
   No arquivo `appsettings.json`, configure:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AgendamentoMedicoDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

   > **Observação:**
   > Este exemplo é para desenvolvimento local com LocalDB. Em produção, configure via variáveis de ambiente ou CI/CD.

2. **Migrations e Atualização do Banco**
   No Package Manager Console (projeto `AgendamentoMedico.Infra`):

   ```powershell
   Update-Database
   ```

   Isso cria o banco e aplica as migrations, incluindo o seed inicial de roles e do usuário administrador.

3. **Seed de Roles e Admin**
   No `AppDbContext.OnModelCreating` já há dados seed para:

   * Role **Administrador** (ID = `11111111-1111-1111-1111-111111111111`)
   * Role **Cliente**       (ID = `33333333-3333-3333-3333-333333333333`)
   * Usuário admin (`admin` / `SenhaForte@123`)
     Esses registros são inseridos automaticamente ao rodar `Update-Database`.

---

## Como Executar o Sistema Localmente

1. **Clone o Repositório**

   ```bash
   git clone https://github.com/SEU_USUARIO/AgendamentoMedico.git
   cd AgendamentoMedico/src/AgendamentoMedico.API
   ```

2. **Ajuste o `appsettings.json`**

   * Configure `ConnectionStrings:DefaultConnection` conforme seu ambiente.
   * Certifique-se de ter um usuário e senha válidos para SQL Server (ou outro provider).

3. **Aplicar Migrations**
   No Package Manager Console:

   ```powershell
   Update-Database
   ```

4. **Executar a Aplicação**

   * Pressione F5 (Visual Studio) ou, na pasta do projeto:

     ```bash
     dotnet run
     ```
   * Por padrão, a aplicação ficará disponível em:

     ```
     https://localhost:5001
     http://localhost:5000
     ```

5. **Acessar no Navegador**

   * Abra `https://localhost:5001`.

   * Faça login com:

     * **Usuário:** `admin`
     * **Senha:** `SenhaForte@123`

   * Para cadastrar novos médicos, use a interface de registro de funcionários (via Admin).

   * Pacientes podem se cadastrar diretamente na tela de login (link “Cadastro”).

---

## Funcionalidades Principais

### 1. Autenticação e Cadastro

* **Registrar Cliente**

  * Rota: `/Account/Register`
  * Cria registros em `Usuários` e em `Clientes`, vinculados via `UsuarioId`.

* **Login**

  * Rota: `/Account/Login`
  * Após autenticar, redireciona conforme a role (`Médico` ou `Cliente`).

* **Cadastrar Médico**

  * Geralmente feito pelo Admin (pode-se criar endpoint interno).
  * Cria registros em `Usuários` e em `Funcionarios`, vinculados via `UsuarioId`.

---

### 2. Área do Médico

* **Dashboard do Médico** (`/Funcionarios/Dashboard`)

  * **Calendário (FullCalendar)**

    * Carrega slots via AJAX:

      * `GET /Funcionarios/GetMeusHorariosDisponiveis` → JSON `{ id, start, end }` para cada horário disponível do médico.
      * `GET /Funcionarios/GetMeusAgendamentos`   → JSON `{ id, dataHora, pacienteNome, status }` para os agendamentos confirmados.
    * **Criar Horário Disponível**

      * Clique e arraste para selecionar intervalo (30 minutos).
      * Abre modal de confirmação; ao confirmar, faz `POST /Funcionarios/SalvarHorarioDisponivel` com `{ InicioConsulta, FimConsulta }`.
      * No back-end, gera `HorarioDisponivelId = Guid.NewGuid()` e salva.
    * **Remover Horário Disponível**

      * Clique num evento verde; abre modal.
      * “Remover” dispara `POST /Funcionarios/RemoverHorarioDisponivel` com `{ Id: "<GUID>" }`.
  * **Tabela “Meus Agendamentos”**

    * Preenchida por `GET /Funcionarios/GetMeusAgendamentos` (JSON → tabela via JS).
    * Colunas: Data/Hora, Paciente, Status.

* **Modal de Agendamentos Pendentes**

  * Ao carregar o dashboard, faz `GET /Funcionarios/VerificarAgendamentosPendentes`.
  * Se existirem agendamentos com `Status == Confirmado`, retorna PartialView `_AgendamentosPendentes`.
  * Modal exibe tabela de `AgendamentoViewModel` com botões “Compareceu” (Finalizado) e “Não Compareceu” (Falta).
  * Botões disparam `POST /Funcionarios/MarcarPresenca` com `{ HorarioDisponivelId, NovoStatus }`.

---

### 3. Área do Paciente

* **Marcar Consulta** (`/Home/AgendarConsulta`)

  * **Calendário (FullCalendar)**

    * Carrega todos os slots disponíveis de todos os médicos via:

      * `GET /Home/GetHorariosDisponiveis` → JSON `{ id: "<GUID>", start: "...", end: "...", titulo: "Nome do Médico" }`.
    * **Agendar Consulta**

      * Clique num evento verde; abre modal de confirmação.
      * “Confirmar” faz `POST /Home/Agendar` com `{ horarioId: "<GUID>" }`.

        * No back-end, obtém `clienteId` via `_usuarioContextService.ObterClienteLogadoAsync(User)`.
        * Chama `AgendamentoService.AgendarConsultaAsync(horarioId, clienteId)`.
      * Após sucesso:

        * `calendar.refetchEvents()` remove o slot do médico.
        * Tabela “Meus Agendamentos” (abaixo) é recarregada via `GET /Home/GetMeusAgendamentos`.
  * **Tabela “Meus Agendamentos”**

    * Populada por `GET /Home/GetMeusAgendamentos` → JSON `{ dataHora, nomeMedico, status }`.
    * Exibe data/hora formatada, nome do médico e status (Confirmado, Finalizado, Falta).

---

### 4. Administração de Cargos

* **Gerenciar Cargos** (`/Cargos/Index`)

  * Permite criar novos cargos (ex.: “Medico”, “Administrador”, etc.).
  * Adicionar ou remover cargos de usuários (Administrador pode atribuir role “Medico” a um usuário).
  
---

## Considerações de Segurança

* **Senhas** são armazenadas criptografadas (ASP.NET Identity).
* **Endpoints protegidos** por `[Authorize(Roles = "...")]`.
* **Operações de escrita** (`POST`, `PUT`, `DELETE`) usam `[ValidateAntiForgeryToken]` para prevenir CSRF.

---

## Possíveis Melhorias

* Adicionar testes unitários e de integração para controllers, services e repositórios.
* Implementar notificações em tempo real (SignalR) para alertar médicos sobre novos agendamentos.
* Refatorar Views para um SPA (Vue.js, React ou Blazor).
* Adicionar filtros, paginação e busca na tabela de agendamentos.
* Permitir cancelamento e reagendamento de consultas.
* Internacionalização (i18n) para suportar múltiplos idiomas.

---

## Autor e Contato

* **Nome:** Paulo Henrique Medeiros Bittencourt
* **E-mail:** [paulohenriquebitt@gmail.com](mailto:paulohenriquebitt@gmail.com)
* **Data de Criação:** 20/05/2025

---

**Licença:** MIT License
