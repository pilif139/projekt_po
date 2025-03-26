# Projekt z programowania obiektowego w c #

Before running program you should create an .env file with variables:

- `DB_SERVER`
- `DB_PORT`
- `DB_USER`
- `DB_PASSWORD`
- `DB_NAME`

## How to run program ##

1. Open terminal in **projekt_po** directory
2. If you have docker you can use ```docker-compose up``` to start database container or you can use your own database (you should create .env file with database variables)
3. Run ```dotnet ef database update```  to be sure that database migrations are up to date
4. Run ```dotnet run``` to start program
5. cd into **Tests** directory and run ```dotnet test``` to run tests

## resources ##

><https://learn.microsoft.com/en-us/ef/core/>
><https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli>
><https://spectreconsole.net/prompts/selection>
