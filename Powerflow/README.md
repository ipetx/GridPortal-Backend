<!-- ### If you want to inssert to the db
```
dotnet ef migrations add UpdateModel --context CimDbContext
dotnet ef database update --context CimDbContext
dotnet run
``` -->

# Instructions
### Install extentions
If you do not have the dependent extensions installed, you should first install them. ![dependencies](img/dependencies.png)

### Install EF Core
```
dotnet add package Microsoft.EntityFrameworkCore --version 9.0.4
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.4
dotnet add package Microsoft.Extensions.DependencyInjection
```
### Add EF Core Migrations
```
dotnet tool install --global dotnet-ef
```
If this is your first time add EF Core, you need to reopen VSCode to make it work.

### Create and start the database
```
sqllocaldb i mssqllocaldb
sqllocaldb create mssqllocaldb
sqllocaldb start mssqllocaldb
```
![create_database](img/create_database.png)

### Update or create the database
Creat the database
```
& "$env:USERPROFILE\.dotnet\tools\dotnet-ef" migrations add InitialCreate
```
or simply
```
dotnet ef migrations add InitialCreate
```
![add_migrations](img/add_migrations.png)


Update the database
```
& "$env:USERPROFILE\.dotnet\tools\dotnet-ef" database update
```
or simply
```
dotnet ef database update
```
![update_database](img/update_database.png)

If you want to create a new migrations you need to first remove the previous migrations then add a new migrations, then update the database.

```
& "$env:USERPROFILE\.dotnet\tools\dotnet-ef" migrations remove
& "$env:USERPROFILE\.dotnet\tools\dotnet-ef" migrations add RebuildModel
& "$env:USERPROFILE\.dotnet\tools\dotnet-ef" database update
```

### Drop database:
```
dotnet ef database drop
```
![drop_database](img/drop_database.png)

### Then update database and insert
```
dotnet ef migrations remove
dotnet ef migrations add IntialCreate
dotnet ef database update
dotnet run
```

### To insert to an existing database
```
dotnet run --project RdfMerger.csproj path/to/new_instance_file.xml
```