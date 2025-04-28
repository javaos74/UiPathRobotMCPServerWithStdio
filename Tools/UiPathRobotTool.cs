using Microsoft.VisualBasic;
using ModelContextProtocol.Server;
using System.ComponentModel;
using UiPath.Robot.Api;
using PTST.UiPath.Orchestrator.API;
using PTST.UiPath.Orchestrator.Models;
using System.Diagnostics;
namespace UiPath.Robot.MCP.Tools;

[McpServerToolType]
public class UiPathRobotTool
{
 
    [McpServerTool, Description("Get installed process list")]
    public static async Task<string> GetProcessList(
        RobotClient client)
    {
        var processes = await client.GetProcesses();
        if( processes == null || processes.Count == 0)
        {
            return "No processes found.";
        }
        return string.Join("\n---\n", processes.Select( p =>
        {
            return $"""
                    Process Name: {p.Name}
                    Process Description: {p.Description}
                    Process Key: {p.Key}
                    """;
        }));
    }

    [McpServerTool, Description("Get specific process input parameter for invocation")] 
    public static string GetProcessInputParameter(
        RobotClient client,
        [Description("Process Key to get process input parameter")] string processKey)   
    {
#if DEBUG
        Debugger.Launch();
#endif
        var helper = RobotHelper.getRobotHelper();
        var release = helper.findProcessWithKey(processKey);
        if( release == null)
        {
            return "No process found.";
        }
        else        
        {
            var inputArguments = release.Arguments.Input;
            return helper.ConvertToParameter(inputArguments);
        }   
    }

/*
    [McpServerTool, Description("Invoke process with given paramters ")]
    public static async Task<string> InvokeProcess(
        RobotClient client,
        [Description("Process Key to invoke")] string processKey,
        [Description("Input Arguments")] string inputArguments)
    {
        var process = client.GetProcesses().Result.Where( p => p.Key.ToString() == processKey).FirstOrDefault();
        if( process == null)
        {
            return "No process found.";
        }
        else
        {
            var job = process.ToJob();
            job.InputArguments = inputArguments;
            var result = await client.RunJob(job);
            return result.ToString();
        }
    }
    */

}
