var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.People>("people");

builder.Build().Run();
