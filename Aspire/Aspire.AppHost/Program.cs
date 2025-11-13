var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("userdb");

builder.AddProject<Projects.User_Api>("user-api")
    .WithReference(postgres);

builder.Build().Run();
