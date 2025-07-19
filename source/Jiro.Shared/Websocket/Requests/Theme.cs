namespace Jiro.Shared.Websocket.Requests;

/// <summary>
/// Represents a theme.
/// </summary>
public class Theme
{
	/// <summary>
	/// Gets or sets the theme name.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the theme description.
	/// </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the theme color scheme.
	/// </summary>
	public string JsonColorScheme { get; set; } = string.Empty;
}
