var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.User_Api>("user.api");

builder.Build().Run();
