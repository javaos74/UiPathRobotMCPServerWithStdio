using Microsoft.VisualBasic;
using ModelContextProtocol.Server;
using System.ComponentModel;
using UiPath.Robot.Api;
using PTST.UiPath.Orchestrator.API;
using PTST.UiPath.Orchestrator.Models;
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

    [McpServerTool, Description("Get Process Detail for invocation with process key")] 
    public static async Task<string> GetProcessDetail(
        RobotClient client,
        [Description("Process Key to get process detail")] string processKey)   
    {
        var helper = RobotHelper.getRobotHelper();
        var release = helper.findProcessWithKey(processKey);
        if( release == null)
        {
            return "No process found.";
        }
        else        
        {
            return $"""
                    Process Name: {release.Name}
                    Process Description: {release.Description}
                    Process Key: {release.Key}
                    Process Input Arguments: {release.Arguments.Input}
                    """;
        }   
    }
}
