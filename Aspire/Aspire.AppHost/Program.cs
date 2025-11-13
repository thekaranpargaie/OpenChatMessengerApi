var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var userdb = postgres.AddDatabase("userdb");
var chatdb = postgres.AddDatabase("chatdb");

var userApi = builder.AddProject<Projects.User_Api>("user-api")
    .WithReference(userdb);

var chatApi = builder.AddProject<Projects.Chat_Api>("chat-api")
    .WithReference(chatdb);

var presenceApi = builder.AddProject<Projects.Presence_Api>("presence-api");

var webhookApi = builder.AddProject<Projects.Webhook_Api>("webhook-api");

var archiveApi = builder.AddProject<Projects.Archive_Api>("archive-api");

var chatUI = builder.AddProject<Projects.ChatUI>("chat-ui")
    .WithReference(userApi)
    .WithReference(chatApi)
    .WithReference(presenceApi);

builder.Build().Run();
