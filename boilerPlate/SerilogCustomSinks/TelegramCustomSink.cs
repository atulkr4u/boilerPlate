using System;
using System.Text;
using boilerPlate.Domain.Helpers;
using boilerPlate.Infra.ServiceContracts;
using Serilog.Core;
using Serilog.Events;
using Telegram.Bot;

namespace boilerPlate.SerilogCustomSinks
{
	public class TelegramCustomSink : ILogEventSink
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly LogEventLevel _minimumLevel;
        IConfigService _configService;
        public TelegramCustomSink(string botToken, LogEventLevel minimumLevel, IConfigService configService)
        {
            _telegramBotClient = new TelegramBotClient(botToken);
            _minimumLevel = minimumLevel;
            _configService = configService;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level >= _minimumLevel)
            {
                var message = logEvent.RenderMessage();
                if (logEvent.Exception != null)
                {
                    var msg = new StringBuilder();
                    msg.Append($"Environment:{_configService.CurrentEnv()}");
                    msg.Append(">>>>>>>>>>>>>>");

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

