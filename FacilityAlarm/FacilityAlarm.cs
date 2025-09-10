namespace Site12.API.Features.OtherStaff;

using System;
using System.Collections.Generic;
using System.IO;
using CommandSystem;
using Exiled.API.Features;
using Extensions;
using MEC;
using UnityEngine;
//
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class FacilityAlarm : ICommand
{
    public string Command => "facilityalarm";

    public string[] Aliases { get; } = ["falarm"];
    public string Description => "Plays the custom codes, alarms, or alarm lighting, made by KFC.";
    private static CoroutineHandle _alarmLightHandle;
    private const float AlarmCycleSeconds = 2f; // full dark -> color -> dark = 2s
    private static readonly Dictionary<string, Color> AlarmColorMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["red"]    = Color.red,
            ["blue"]   = Color.blue,
            ["orange"] = new Color(1f, 0.5f, 0f),
            ["purple"] = new Color(0.6f, 0.2f, 0.8f),
            ["yellow"] = Color.yellow,
            ["green"]  = Color.green,
            ["navy"]   = new Color(0.0f, 0.0f, 0.5f),
        };

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = "You do not have permissions to run this command.";
        if (!sender.CheckRemoteAdmin(out response))
            return false;
        
        string wrongCommandUsage =
            "This command is used to execute custom codes made by KFC.\n" +
            "----HELP COMMANDS----\n" +
            "falarm help alarm\n" +
            "falarm help code\n" +
            "falarm help alarmlight\n" +
            "----COMMAND USAGE----\n" +
            "Usage: falarm alarm (alarm name) [on/off] (default loop off)\n" +
            "Usage: falarm code (color|stop)\n" +
            "Usage: falarm alarmlight (color|stop)\n";

        var alarms = new List<string> { "Alarm1", "Alarm2" };
        string helpAlarmUsage =
            "----ALARM HELP----\n" +
            "Usage: falarm alarm (alarm name) [on/off] (default loop off)\n" +
            "Usage: falarm alarm stop\n" +
            "Available Alarms:\n" +
            string.Join(Environment.NewLine, alarms) +
            "\n--\nMore will be added later.\n" +
            "If you want to know how an Alarm sounds like, DM or ping kfc.dictator\n";

        var codeColors = new List<string> { "Red", "Blue", "White1", "White2", "Orange", "Purple1", "Purple2", "Yellow", "Green", "Gold", "Grey", "Maroon", "Navy", "Black" };
        string helpCodeUsage =
            "----CODE HELP----\n" +
            "Usage: falarm code (code name)\n" +
            "Usage: falarm code stop\n" +
            "Available Codes:\n" +
            string.Join(Environment.NewLine, codeColors) +
            "\n--\nIf you want to know how an Alarm sounds like, DM or ping kfc.dictator\n";

        var alarmColors = new List<string> { "Red", "Blue", "Orange", "Purple", "Yellow", "Green", "Navy" };
        string helpAlarmlightUsage =
            "----ALARM LIGHT HELP----\n" +
            "Usage: falarm alarmlight (color)\n" +
            "Usage: falarm alarmlight stop\n" +
            "Available Colors:\n" +
            string.Join(Environment.NewLine, alarmColors);
        
        if (arguments.Count == 0)
        {
            response = wrongCommandUsage;
            return false;
        }

        string sub = arguments.At(0)?.ToLowerInvariant();
        // ---- HELP ----
        if (sub == "help")
        {
            if (arguments.Count == 1)
            {
                response = wrongCommandUsage;
                return true;
            }

            string topic = arguments.At(1)?.ToLowerInvariant();
            switch (topic)
            {
                case "alarm":
                    response = helpAlarmUsage;
                    return true;
                case "code":
                    response = helpCodeUsage;
                    return true;
                case "alarmlight":
                    response = helpAlarmlightUsage;
                    return true;
                default:
                    response = wrongCommandUsage;
                    return false;
            }
        }
        // ---- CODE ----
        if (sub == "code")
        {
            if (arguments.Count == 1)
            {
                response = helpCodeUsage;
                return false;
            }
            if (arguments.At(1).Equals("stop", StringComparison.OrdinalIgnoreCase))
            {
                var ok = TryStopNamedAudio("KFCCode", "KFCCodeSpeaker", out var msg);
                response = ok ? "Stopped Code audio." : msg;
                return ok; // true if something was stopped, false otherwise
            }
            var audioPlayer = AudioPlayer.CreateOrGet("KFCCode", destroyWhenAllClipsPlayed: true,
                controllerId: SpeakerExtensions.GetFreeId());
                audioPlayer.AddSpeaker("KFCCodeSpeaker", new Vector3(0, 0, 0), maxDistance: 4000, minDistance: 4000,
                isSpatial: false);

            string color = arguments.At(1).ToLowerInvariant();
            switch (color)
            {
                case "blue":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCBlue"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCBlue.ogg"), "KFCBlue");
                    audioPlayer.AddClip("KFCBlue");
                    response = "Code Blue executed.";
                    return true;

                case "red":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCRed"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCRed.ogg"), "KFCRed");
                    audioPlayer.AddClip("KFCRed");
                    response = "Code Red executed.";
                    return true;

                case "white1":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCWhite1"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCWhite1.ogg"), "KFCWhite1");
                    audioPlayer.AddClip("KFCWhite1");
                    response = "Code White variant one executed.";
                    return true;

                case "white2":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCWhite2"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCWhite2.ogg"), "KFCWhite2");
                    audioPlayer.AddClip("KFCWhite2");
                    response = "Code White variant two executed.";
                    return true;

                case "orange":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCOrange"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCOrange.ogg"), "KFCOrange");
                    audioPlayer.AddClip("KFCOrange");
                    response = "Code Orange executed.";
                    return true;

                case "purple1":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCPurple1"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCPurple1.ogg"), "KFCPurple1");
                    audioPlayer.AddClip("KFCPurple1");
                    response = "Code Purple variant one executed.";
                    return true;

                case "purple2":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCPurple2"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCPurple2.ogg"), "KFCPurple2");
                    audioPlayer.AddClip("KFCPurple2");
                    response = "Code Purple variant two executed.";
                    return true;

                case "yellow":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCYellow"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCYellow.ogg"), "KFCYellow");
                    audioPlayer.AddClip("KFCYellow");
                    response = "Code Yellow executed.";
                    return true;

                case "green":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCGreen"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCGreen.ogg"), "KFCGreen");
                    audioPlayer.AddClip("KFCGreen");
                    response = "Code Green executed.";
                    return true;

                case "gold":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCGold"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCGold.ogg"), "KFCGold");
                    audioPlayer.AddClip("KFCGold");
                    response = "Code Gold executed.";
                    return true;

                case "grey":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCGrey"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCGrey.ogg"), "KFCGrey");
                    audioPlayer.AddClip("KFCGrey");
                    response = "Code Grey executed.";
                    return true;

                case "maroon":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCMaroon"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCMaroon.ogg"), "KFCMaroon");
                    audioPlayer.AddClip("KFCMaroon");
                    response = "Code Maroon executed.";
                    return true;

                case "navy":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCNavy"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCNavy.ogg"), "KFCNavy");
                    audioPlayer.AddClip("KFCNavy");
                    response = "Code Navy executed.";
                    return true;

                case "black":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCBlack"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCBlack.ogg"), "KFCBlack");
                    audioPlayer.AddClip("KFCBlack");
                    response = "Code Black executed.";
                    return true;

                default:
                    response = helpCodeUsage;
                    return false;
            }
        }
        // ---- ALARM ----
        if (sub == "alarm")
        {
            if (arguments.Count == 1)
            {
                response = helpAlarmUsage;
                return false;
            }
            if (arguments.At(1).Equals("stop", StringComparison.OrdinalIgnoreCase))
            {
                var ok = TryStopNamedAudio("KFCAlarm", "KFCAlarmSpeaker", out var msg);
                response = ok ? "Stopped Alarm audio." : msg;
                return ok;
            }
            bool loop = false;
            if (arguments.Count > 2)
            {
                var loopArg = arguments.At(2)?.ToLowerInvariant();
                switch (loopArg)
                {
                    case "on":
                    case "true":
                    case "1":
                    case "loop":
                        loop = true; break;

                    case "off":
                    case "false":
                    case "0":
                    case "noloop":
                    case "":
                        loop = false; break;

                    default:
                        response = "Invalid loop flag. Use: on/off (default off).";
                        return false;
                }
            }
            var audioPlayer = AudioPlayer.CreateOrGet("KFCAlarm", destroyWhenAllClipsPlayed: true,
                controllerId: SpeakerExtensions.GetFreeId());
            try { audioPlayer.RemoveSpeaker("KFCAlarmSpeaker"); } catch { /* ignore */ }
            audioPlayer.AddSpeaker("KFCAlarmSpeaker", new Vector3(0, 0, 0), maxDistance: 4000, minDistance: 4000, isSpatial: false);

            string name = arguments.At(1).ToLowerInvariant();
            switch (name)
            {
                case "alarm1":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCAlarm1"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCAlarm1.ogg"), "KFCAlarm1");
                    audioPlayer.AddClip("KFCAlarm1", loop: loop);
                    response = $"Alarm 1 executed. Loop: {(loop ? "on" : "off")}."; 
                    return true;
                case "alarm2":
                    if (!AudioClipStorage.AudioClips.ContainsKey("KFCAlarm2"))
                        AudioClipStorage.LoadClip(Path.Combine(Paths.Plugins, "audio", "KFCAlarm2.ogg"), "KFCAlarm2");
                    audioPlayer.AddClip("KFCAlarm2", loop: loop);
                    response = $"Alarm 2 executed. Loop: {(loop ? "on" : "off")}."; 
                    return true;
                default:
                    response = helpAlarmUsage;
                    return false;
            }
        }
        // --- Alarm light command ...
        if (sub == "alarmlight")
        {
            if (arguments.Count == 1)
            {
                response = helpAlarmlightUsage;
                return false;
            }
            string arg = arguments.At(1).ToLowerInvariant();
            if (arg == "stop")
            {
                StopAlarmLight();
                response = "Alarm light stopped. Facility lights set to black.\nUse fcolor reset to reset the lights whenever you want.";
                return true;
            }
            
            if (!AlarmColorMap.TryGetValue(arg, out var col))
            {
                response = helpAlarmlightUsage + "\nUnknown color.";
                return false;
            }
            StartAlarmLightBreathing(col);
            response = $"Alarm light breathing started: {arg} (2s cycle).";
            return true;
        }
        response = wrongCommandUsage;
        return false;
    }
    // Start/replace the breathing loop
    private static void StartAlarmLightBreathing(Color baseColor)
    {
        if (_alarmLightHandle.IsRunning)
            Timing.KillCoroutines(_alarmLightHandle);

        _alarmLightHandle = Timing.RunCoroutine(AlarmLightBreathing(baseColor, AlarmCycleSeconds));
    }
    private static void StopAlarmLight()
    {
        if (_alarmLightHandle.IsRunning)
            Timing.KillCoroutines(_alarmLightHandle);

        foreach (var room in Room.List)
            room.Color = Color.black;
    }
    private static IEnumerator<float> AlarmLightBreathing(Color baseColor, float period)
    {
        float elapsed = 0f;
        float half = Mathf.Max(0.1f, period / 2f);

        while (true)
        {
            elapsed += Timing.DeltaTime;
            float t = Mathf.PingPong(elapsed, half) / half;
            float eased = Mathf.SmoothStep(0f, 1f, t);
            Color c = new(baseColor.r * eased, baseColor.g * eased, baseColor.b * eased, 1f);
            foreach (var room in Room.List)
                room.Color = c;
            yield return Timing.WaitForSeconds(0.05f);
        }
    }
    private static bool TryStopNamedAudio(string playerName, string speakerName, out string message)
    {
        if (!AudioPlayer.AudioPlayerByName.TryGetValue(playerName, out var ap))
        {
            message = $"No active audio named '{playerName}'.";
            return false;
        }
        try { ap.RemoveSpeaker(speakerName); } catch { /* speaker might not exist, ignore */ }
        ap.Destroy();
        message = $"Stopped '{playerName}'.";
        return true;
    }
}
