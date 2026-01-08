using Serilog.Events;
using Serilog.Formatting;
using System.Text.Json;

namespace Fcg.Games.Purchase.Api.ApiConfigurations.LogsConfig;

public class NewRelicFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        var logObject = new
        {
            message = logEvent.RenderMessage(),
            timestamp = logEvent.Timestamp.UtcDateTime.ToString("o"),
            level = logEvent.Level.ToString(),
            attributes = new
            {
                app = "Fcg Purchase",
                source = logEvent.Properties.ContainsKey("SourceContext") ? logEvent.Properties["SourceContext"].ToString().Trim('"') : null,
                exception = logEvent.Exception?.ToString()
            }
        };

        string json = JsonSerializer.Serialize(logObject);
        output.WriteLine(json);
    }
}
