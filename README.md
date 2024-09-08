## Useful commands in .net console CLI

Install tooling

~~~bash
dotnet tool update -g dotnet-ef
dotnet tool update -g dotnet-aspnet-codegenerator
~~~

## EF Core migrations

Run from solution folder

~~~bash
dotnet ef migrations --project App.DAL.EF --startup-project WebApp add Initial
dotnet ef database   --project App.DAL.EF --startup-project WebApp update
~~~

## Docker

~~~bash
docker compose up
~~~

## Run project

~~~bash
dotnet run --project WebApp
~~~