using System;
using System.Text;
using boilerPlate.Domain.Helpers;
using Serilog.Core;
using Serilog.Events;
using Telegram.Bot;

namespace boilerPlate.SerilogCustomSinks
{
	public class TelegramCustomSink : ILogEventSink
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly LogEventLevel _minimumLevel;

        public TelegramCustomSink(string botToken, LogEventLevel minimumLevel)
        {
            _telegramBotClient = new TelegramBotClient(botToken);
            _minimumLevel = minimumLevel;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level >= _minimumLevel)
            {
                var message = logEvent.RenderMessage();
                if (logEvent.Exception != null)
                {
                    var msg = new StringBuilder();
                    if (logEvent.Properties.ContainsKey("RequestPath"))
                    {
                        msg.Append($"RequestPath:{logEvent.Properties["RequestPath"]}");
                        msg.Append(">>>>>>>>>>>>>>");
                    }
                    if (!string.IsNullOrEmpty(logEvent.Exception.Message))
                    {
                        msg.Append($"ExceptionMessage:{logEvent.Exception.Message}");
                        msg.Append(">>>>>>>>>>>>>>");
                    }
                    if (!string.IsNullOrEmpty(logEvent.Exception.StackTrace))
                    {
                        msg.Append($"StackTrace:{logEvent.Exception.StackTrace}");
                    }

                    message = msg.ToString();
                }
                _telegramBotClient.SendTextMessageAsync("607833984", message.TakeTillLength(4000)).GetAwaiter().GetResult();
             }
        }
    }
}

