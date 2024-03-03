var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServerContainer("CCA", "Pass@123").AddDatabase("User");
builder.AddProject<Projects.User_Api>("user.api")
    .WithReference(sqlServer);

builder.Build().Run();
