using PTST.UiPath.Orchestrator.API;
using PTST.UiPath.Orchestrator.Models;
using dotenv.net;
using dotenv.net.Utilities;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

public class RobotHelper
{
    static public RobotHelper _instance;
    private Orchestrator _orch;
    private Folder? _folder;   
    public RobotHelper() 
    {
        DotEnv.Load();
        // Constructor logic here
        _orch = new Orchestrator( EnvReader.GetStringValue("UIPATH_ENDPOINT"),
            EnvReader.GetStringValue("UIPATH_APPID"),
            EnvReader.GetStringValue("UIPATH_APPSECRET"),
            EnvReader.GetStringValue("UIPATH_APPSCOPE"));
        _folder = _orch.GetAll<Folder>().Result.Where(f => f.DisplayName == "Agentic Demo").FirstOrDefault();
        //Console.WriteLine($"Folder Name: {_folder.DisplayName}, Id: {_folder.Id}");
    }

    public Release? findProcessWithKey(string processKey)
    {
        if( _folder == null)
        {
            return null;
        } 
        else
        {
            var release = _orch.GetAll<Release>(_folder.Id ).Result.Where(r => r.Key.ToString() == processKey).FirstOrDefault();
            //Console.WriteLine($"Release Name: {release.Name}, Id: {release.Id}");
            return release;
        }   
    }

    public static RobotHelper getRobotHelper()
    {
        if( _instance == null)
        {
            _instance = new RobotHelper();
        }
        return _instance;

    }


    public string ConvertToParameter(string inputArguments)
    {
        Dictionary<string, object> result = new Dictionary<string, object>();
        Dictionary<string, object> properties = new Dictionary<string, object>();
        result.Add("type", "object");
        List<string> required = new List<string>();

        JArray jarr = JArray.Parse(inputArguments);
        foreach( var jobj in jarr)
        {
            properties.Add(jobj["name"]?.ToString(),  new JObject( "type", _GetTypeName(jobj["type"]?.ToString())));
            if (jobj["required"]?.ToString() == "true")
            {
                required.Add(jobj["name"]?.ToString());
            }
        }
        result.Add("properties", properties);
        result.Add("required", required);
        return result.ToString();
    }

    public string _GetTypeName(string val) {
        string? _type= val?.Split(',')[0];
        switch (_type)
        {
            case "System.String":
                _type = "string";
                break;
            case "System.Int64":
            case "System.Int32":
            case "System.Single":
            case "System.Double":
                _type = "number";
                break;
            case "System.Boolean":
                _type = "boolean";
                break;
        }
        return _type?? string.Empty;
    }

}