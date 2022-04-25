# movie-catalog-rental-service

🎬 Movie catalog and rental service API written in C#

# Pre requisites

- [.NET 6.0](https://dotnet.microsoft.com/en-us/download): to run, build, test the project locally;
- [dotnet-ef tool](https://docs.microsoft.com/en-us/ef/core/cli/dotnet): to create new migrations;
- [Docker](https://www.docker.com/products/docker-desktop/): to run everything inside containers.

# Running commands

To up all the app and dependencies containerized, type the following command in
the [src](./src) folder:

```shell
docker-compose up --build
```

> **NOTE:** the command above will up the web application, SqlServer and will execute all the existing migrations.
> That's enough to run everything, but even though, if you want to run the app locally using containerized dependencies,
> you must try the commands below.

To run the SqlServer, type the following command in
the [src](./src) folder:

```shell
docker-compose up sql-server-database
```

To up the existent migrations in SqlServer, type the following command in
the [src](./src) folder:

```shell
docker-compose up --build migrations
```

If you want to create a new migration (after an entity model update, for example), type the following command in
the [root](./) folder:

```shell
dotnet ef migrations add <migration-name> --project src/RentMovie.Infrastructure/RentMovie.Infrastructure.csproj --startup-project src/RentMovie.Web/RentMovie.Web.csproj --context RentMovieContext --verbose
```

To run all the automated test, type the following command in the [src](./src) folder:

```shell
dotnet test
```

## Design and Architecture decisions

- Rich Domain Models (validations and behaviors inside the domain entities)

- [Hexagonal architecture](https://alistair.cockburn.us/hexagonal-architecture/)

![hexagonal architecture](./assets/hexagonal.png)

> Font: [Hexagonal Architecture, there are always two sides to
> every story](https://medium.com/ssense-tech/hexagonal-architecture-there-are-always-two-sides-to-every-story-bc0780ed7d9c)