using PTST.UiPath.Orchestrator.API;
using PTST.UiPath.Orchestrator.Models;
using dotenv.net;
using dotenv.net.Utilities;

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

}