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

## Estrutura do Repositório

