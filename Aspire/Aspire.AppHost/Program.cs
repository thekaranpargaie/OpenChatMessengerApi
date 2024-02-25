var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.People>("people");

builder.AddProject<Projects.Chat>("chat");

builder.Build().Run();
