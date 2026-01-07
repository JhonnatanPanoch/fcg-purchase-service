using Amazon.Lambda.Core;
using System.Text.Json;
using System.Text;
using Amazon.SimpleEmailV2.Model;
using Amazon.SimpleEmailV2;
using Amazon;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Fcg.Games.Purchase.Infra.Lambda;

public class Function
{
    private static readonly IAmazonSimpleEmailServiceV2 sesClient =
        new AmazonSimpleEmailServiceV2Client(RegionEndpoint.SAEast1);

    public async Task FunctionHandler(JsonElement mqEvent, ILambdaContext context)
    {
        try
        {
            context.Logger.LogLine("[DEBUG] Evento recebido: " + mqEvent.GetRawText());

            if (mqEvent.TryGetProperty("rmqMessagesByQueue", out var rmqByQueue) && rmqByQueue.ValueKind == JsonValueKind.Object)
            {
                foreach (var queueProperty in rmqByQueue.EnumerateObject())
                {
                    var queueName = queueProperty.Name;
                    var msgsArray = queueProperty.Value;

                    if (msgsArray.ValueKind != JsonValueKind.Array)
                        continue;

                    foreach (var msg in msgsArray.EnumerateArray())
                    {
                        if (msg.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.String)
                        {
                            var base64 = dataProp.GetString();
                            var raw = Convert.FromBase64String(base64 ?? "");
                            var body = Encoding.UTF8.GetString(raw);

                            context.Logger.LogLine($"[INFO] Mensagem recebida: {body}");

                            var evento = DesserializeBody(body, context);
                            if (evento != null)
                            {
                                context.Logger.LogLine($"[INFO] CompraId: {evento.compraId}, Email: {evento.emailUsuario}");
                                await ProcessMessageAsync(evento, context);
                            }
                            else
                            {
                                context.Logger.LogLine("[WARN] Falha ao desserializar mensagem para PagamentoAprovadoEvent.");
                            }
                        }
                        else
                        {
                            context.Logger.LogLine($"[WARN] Mensagem sem campo 'data' ou não-string (fila {queueName}).");
                        }
                    }
                }
                return;
            }
        }
        catch (Exception ex)
        {
            context.Logger.LogLine($"[ERROR] Erro geral no handler: {ex}");
            throw;
        }
    }

    private static PagamentoAprovadoEvent DesserializeBody(string body, ILambdaContext context)
    {
        if (!string.IsNullOrEmpty(body))
        {
            context.Logger.LogLine($"[INFO] Mensagem recebida: {body}");

            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("message", out var msgElement))
                {
                    var evento = msgElement.Deserialize<PagamentoAprovadoEvent>();
                    context.Logger.LogLine($"[INFO] Mensagem recebida via MASSTRANSIT: {evento}");

                    return evento;
                }
                else
                {
                    // Caso 2: Mensagem crua (publicada direto no RabbitMQ)
                    var evento = JsonSerializer.Deserialize<PagamentoAprovadoEvent>(body);
                    context.Logger.LogLine($"[INFO] Mensagem recebida via RabbitMQ manager: {evento}");

                    return evento;
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"[ERROR] Falha ao processar mensagem: {ex}");
            }
        }

        throw new ArgumentException("Não foi possível desserializar o body do evento.");
    }

    private async Task ProcessMessageAsync(PagamentoAprovadoEvent evento, ILambdaContext context)
    {
        try
        {
            var messageId = await SendConfirmationEmailAsync(evento, context);
            context.Logger.LogLine($"[INFO] E-mail enviado com MessageId: {messageId}");
        }
        catch (Exception emailEx)
        {
            context.Logger.LogLine($"[ERROR] Erro ao enviar e-mail: {emailEx}");
        }
    }

    private async Task<string> SendConfirmationEmailAsync(PagamentoAprovadoEvent evento, ILambdaContext context)
    {
        if (string.IsNullOrWhiteSpace(evento.emailUsuario))
            throw new ArgumentException("EmailUsuario vazio.");

        var request = new SendEmailRequest
        {
            FromEmailAddress = "jhonnatan.jp@gmail.com",
            Destination = new Destination
            {
                ToAddresses = new List<string> { evento.emailUsuario }
            },
            Content = new EmailContent
            {
                Simple = new Message
                {
                    Subject = new Content { Data = $"Compra {evento.compraId} confirmada" },
                    Body = new Body
                    {
                        Html = new Content { Data = $"<p>Olá,</p><p>Sua compra <strong>{evento.compraId}</strong> foi confirmada.</p><p>ID transação: {evento.compraId}</p><p>Obrigado!</p>" },
                        Text = new Content { Data = $"Sua compra {evento.compraId} foi confirmada. ID transação: {evento.compraId}" }
                    }
                }
            }
        };
        
        context.Logger.LogLine("[DEBUG] Iniciando envio de e-mail...");
        var response = await sesClient.SendEmailAsync(request);
        context.Logger.LogLine($"[DEBUG] Resposta SES: {response.HttpStatusCode} - MessageId: {response.MessageId}");
        
        return response.MessageId;
    }
}

public sealed record PagamentoAprovadoEvent(
    Guid compraId,
    string emailUsuario) : EventBase
{
    public override Guid streamId { get; init; } = compraId;
}

public abstract record EventBase
{
    public Guid eventId { get; init; } = Guid.NewGuid();
    public DateTime timestamp { get; init; } = DateTime.UtcNow;
    public abstract Guid streamId { get; init; }
}
