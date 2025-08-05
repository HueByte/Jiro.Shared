namespace Jiro.Shared;

/// <summary>
/// Base class for polymorphic command results.
/// </summary>
public abstract class CommandResult
{
    /// <summary>
    /// Gets the type of the command result.
    /// </summary>
    public abstract CommandType Type { get; }
}

/// <summary>
/// Represents text-based command output.
/// </summary>
public class TextResult : CommandResult
{
    /// <summary>
    /// Gets the type of the command result.
    /// </summary>
    public override CommandType Type => CommandType.Text;
    
    /// <summary>
    /// Gets or sets the text response content.
    /// </summary>
    public string Response { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the text format type.
    /// </summary>
    public TextType TextType { get; set; } = TextType.Plain;
    
    /// <summary>
    /// Creates a new TextResult instance.
    /// </summary>
    /// <param name="response">The text response content.</param>
    /// <param name="textType">The text format type.</param>
    /// <returns>A new TextResult instance.</returns>
    public static TextResult Create(string response, TextType textType = TextType.Plain)
    {
        return new TextResult
        {
            Response = response,
            TextType = textType
        };
    }
}

/// <summary>
/// Represents graphical command output for data visualization.
/// </summary>
public class GraphResult : CommandResult
{
    /// <summary>
    /// Gets the type of the command result.
    /// </summary>
    public override CommandType Type => CommandType.Graph;
    
    /// <summary>
    /// Gets or sets the message associated with the graph.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the graph data as a string (typically JSON).
    /// </summary>
    public string? Data { get; set; }
    
    /// <summary>
    /// Gets or sets the units for data values.
    /// </summary>
    public Dictionary<string, string> Units { get; set; } = new();
    
    /// <summary>
    /// Gets or sets the X-axis label.
    /// </summary>
    public string XAxis { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the Y-axis label.
    /// </summary>
    public string YAxis { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets additional notes about the graph.
    /// </summary>
    public string Note { get; set; } = string.Empty;
    
    /// <summary>
    /// Creates a new GraphResult instance.
    /// </summary>
    /// <param name="message">The message associated with the graph.</param>
    /// <param name="data">The graph data.</param>
    /// <param name="units">The units for data values.</param>
    /// <param name="xAxis">The X-axis label.</param>
    /// <param name="yAxis">The Y-axis label.</param>
    /// <param name="note">Additional notes about the graph.</param>
    /// <returns>A new GraphResult instance.</returns>
    public static GraphResult Create(string message, string? data, Dictionary<string, string> units, 
        string xAxis, string yAxis, string note)
    {
        return new GraphResult
        {
            Message = message,
            Data = data,
            Units = units,
            XAxis = xAxis,
            YAxis = yAxis,
            Note = note
        };
    }
}

/// <summary>
/// Specifies the format of text data.
/// </summary>
public enum TextType
{
    /// <summary>
    /// Plain text format.
    /// </summary>
    Plain = 0,
    
    /// <summary>
    /// JSON format.
    /// </summary>
    Json = 1,
    
    /// <summary>
    /// Base64 encoded format.
    /// </summary>
    Base64 = 2,
    
    /// <summary>
    /// Markdown format.
    /// </summary>
    Markdown = 3,
    
    /// <summary>
    /// HTML format.
    /// </summary>
    Html = 4
}