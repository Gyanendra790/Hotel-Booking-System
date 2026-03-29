## Migrations

Run the following commands to generate and apply migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

This folder will be populated after running the above commands.
The database schema is defined in `Data/ApplicationDbContext.cs`.
