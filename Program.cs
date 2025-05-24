using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;
using ModelContextProtocol.Protocol.Types;
using System.Net.Http.Headers;
using UiPath.Robot.Api;
using UiPath.Robot.MCP.Tools;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

HashSet<string> subscriptions = [];

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<UiPathRobotTool>()
    .WithListResourcesHandler(async (ctx, ct) =>
    {
        return new ListResourcesResult
        {
            Resources = 
            [
                new ModelContextProtocol.Protocol.Types.Resource
                {
                    Uri = "uipath://processes/list",
                    Name = "Installed Processes",
                    Description = "List of installed processes in robot.",
                    MimeType = "text/plain",
                },
            ]
        };
    })
    .WithReadResourceHandler(async (ctx, ct) =>
    {
        var uri = ctx.Params?.Uri;
        if (uri == "uipath://processes/list")
        {
            return new ReadResourceResult
            {
                Contents = [ new TextResourceContents
                {
                    Text = "test",
                    MimeType = "text/plain",
                    Uri = uri,
                }]
            };
        }
        else
        {
            throw new NotSupportedException($"Unknown resource: {uri}");
        }
    });

builder.Services.AddSingleton(subscriptions);
builder.Services.AddHostedService<SubscriptionMessageSender>();

builder.Services.AddSingleton(_ =>
{
    var client = new RobotClient();
    return client;
});
var app = builder.Build();

RobotHelper helper = RobotHelper.getRobotHelper();
//Console.WriteLine( helper.findProcessWithKey("1537e5fa-b770-4a67-92e9-38bcc5a23089"));

await app.RunAsync();