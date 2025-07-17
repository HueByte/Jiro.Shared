namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents the response for themes.
/// </summary>
public class ThemesResponse : SyncResponse
{
    /// <summary>
    /// Gets or sets the list of available themes.
    /// </summary>
    public List<Theme> Themes { get; set; } = new();
}
