## Database

To create a migration:

```bash
dotnet ef migrations add MigrationName -s ../Ethos.Web.Host/Ethos.Web.Host.csproj
```

To update the database:

```bash
dotnet ef database update -s ../Ethos.Web.Host/Ethos.Web.Host.csproj
```
