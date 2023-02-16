using Newtonsoft.Json;
using Raylib_CsLo;

namespace IslandGen.Services;

public class GameSettings
{
    [JsonProperty] public bool DebugMode { get; set; }
    [JsonProperty] public bool Fullscreen { get; set; }
    [JsonProperty] public int TargetFrameRate { get; set; } = 60;

    /// <summary>
    ///     Applies current game settings
    /// </summary>
    public void ApplySettings()
    {
        Raylib.SetTargetFPS(TargetFrameRate);

        if (Fullscreen != Raylib.IsWindowFullscreen()) Raylib.ToggleFullscreen();
    }
}