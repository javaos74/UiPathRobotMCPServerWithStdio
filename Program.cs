using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;
using System.Net.Http.Headers;
using UiPath.Robot.Api;
using UiPath.Robot.MCP.Tools;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<UiPathRobotTool>();

builder.Services.AddSingleton(_ =>
{
    var client = new RobotClient();
    return client;
});
var app = builder.Build();

RobotHelper helper = RobotHelper.getRobotHelper();
//Console.WriteLine( helper.findProcessWithKey("1537e5fa-b770-4a67-92e9-38bcc5a23089"));

await app.RunAsync();