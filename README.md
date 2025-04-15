# Projekt z programowania obiektowego w c #

---
## Before you start ##
Before running program you should create an .env file with variables:

- `DB_SERVER`
- `DB_PORT`
- `DB_USER`
- `DB_PASSWORD`
- `DB_NAME`

### Additionaly you can add
- `ADMIN_LOGIN`
- `ADMIN_PASSWORD`

If you don't add them, default values will be used that are "Admin" for both login and password.

---
## How to run program ##

1. Open terminal in **projekt_po** directory
2. If you have docker you can use ```docker-compose up``` to start database container or you can use your own database (you should create .env file with database variables)
3. Run ```dotnet ef database update```  to be sure that database migrations are up to date
4. Run ```dotnet run``` to start program
5. cd into **Tests** directory and run ```dotnet test``` to run tests

### Additional commands ###
- ```dotnet run seed``` - to seed database with test data
- ```dotnet run clean``` - to clean all data
  - ```dotnet run clean --database``` - to clean all data from database
  - ```dotnet run clean --log``` - to clean all data from log file

## Database structure ##

!['Database schema'](./documentation/DATABASE%20STRUCTURE.png)

## resources ##

><https://learn.microsoft.com/en-us/ef/core/>
><https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli>
><https://spectreconsole.net/prompts/selection>
