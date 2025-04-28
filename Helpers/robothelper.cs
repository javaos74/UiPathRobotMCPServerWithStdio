using PTST.UiPath.Orchestrator.API;
using PTST.UiPath.Orchestrator.Models;
using dotenv.net;
using dotenv.net.Utilities;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Text.Json;

public class RobotHelper
{
    static public RobotHelper _instance;
    private Orchestrator _orch;
    private IEnumerable<Folder>? _folders;   
    public RobotHelper() 
    {
        DotEnv.Load();
        // Constructor logic here
        _orch = new Orchestrator( EnvReader.GetStringValue("UIPATH_ENDPOINT"),
            EnvReader.GetStringValue("UIPATH_APPID"),
            EnvReader.GetStringValue("UIPATH_APPSECRET"),
            EnvReader.GetStringValue("UIPATH_APPSCOPE"));
        var _names = EnvReader.GetStringValue("UIPATH_FOLDERS").Split(';'); // default Shared folder 
        if (_names != null && _names.Length > 0)
        {
            _folders = _orch.GetAll<Folder>().Result.Where(f => _names.Contains(f.DisplayName));
        }
        else
        {
            _folders = _orch.GetAll<Folder>().Result.Where(f => f.DisplayName == "Shared");
        }
    }

    public Release? findProcessWithKey(string processKey)
    {
        if( _folders == null)
        {
            return null;
        } 
        else
        {
            Release release = null;
            foreach (var _folder in _folders)
            {
                release = _orch.GetAll<Release>(_folder.Id).Result.Where(r => r.Key.ToString() == processKey).FirstOrDefault();
                if (release != null)
                {
                    break;
                }
            }
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
            properties.Add(jobj["name"]?.ToString(),  new Dictionary<string, string>() { { "type", _GetTypeName(jobj["type"]?.ToString()) } });
            if (jobj["required"]?.ToString() == "true")
            {
                required.Add(jobj["name"]?.ToString());
            }
        }
        result.Add("properties", properties);
        result.Add("required", required);
        return JsonSerializer.Serialize(result);
    }

    public string _GetTypeName(string val) {
        string? _type= val?.Split(',')[0];
        switch (_type)
        {
            case "System.String":
                _type = "string";
                break;
            case "System.Int64":
            case "System.UInt64":
            case "System.Int32":
            case "System.UInt32":
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