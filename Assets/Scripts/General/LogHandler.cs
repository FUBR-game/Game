using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public enum LogTag
{
    Network,
    Game,
    Damage,
    Attack,
    Dead,
    Kill
}

public class LogHandler
{
    public static LogHandler Instance => _instance ?? (_instance = new LogHandler());

    private static LogHandler _instance;
    private string _location;


    private LogHandler()
    {
        _location = Application.dataPath + @"\game.log";
    }

    public void Log(LogTag Tag, string[] Content)
    {
        new Task(() => { WriteLog(Tag, Content); }).Start();
    }

    private void WriteLog(LogTag Tag, string[] Content)
    {
        var line = "";

        switch (Tag)
        {
            case LogTag.Network:
                line += "NETWORK; " + string.Join("; ", Content);
                break;
            case LogTag.Game:
                line += "GAME; " + string.Join("; ", Content);
                break;
            case LogTag.Damage:
                line += "DAMAGE; " + string.Join("; ", Content);
                break;
            case LogTag.Attack:
                line += "ATTACK; " + string.Join("; ", Content);
                break;
            case LogTag.Dead:
                line += "DEAD; " + string.Join("; ", Content);
                break;
            case LogTag.Kill:
                line += "KILL; " + string.Join("; ", Content);
                break;
            default:
                line += "UNKNOWN";
                break;
        }

        using (StreamWriter writer = File.AppendText(_location))
        {
            writer.WriteLine(line);
        }
    }

    public async void UploadLog(string gameId)
    {
        using (var client = new HttpClient())
        using (var content = new MultipartFormDataContent())
        using (Stream stream = File.Open(_location, FileMode.Open, FileAccess.ReadWrite))
        {
            content.Add(new StringContent(gameId), "game_id");
            content.Add(new StreamContent(stream), "game_log", "game.log");
        
            await client.PostAsync("https://lumen.arankieskamp.com/api/log_upload", content);
        }
    }
}