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
        private readonly string _chatId=null;
        private readonly string _telegramToken = null;
        public TelegramCustomSink(LogEventLevel minimumLevel, IConfigService configService)
        {
            _configService = configService;
            _telegramToken = _configService.Get("TelegramToken");
            _telegramBotClient = new TelegramBotClient(_telegramToken);
            _minimumLevel = minimumLevel;
            _chatId = _configService.Get("TelegramChatId");
            // "TelegramToken": "6192549985:AAElMvWhByCK0saq8rth3CJ-KEBzr9iBmsk",
            //"TelegramChatId": "607833984"

        }

        public void Dispose(string key)
        {
            throw new NotImplementedException();
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
                _telegramBotClient.SendTextMessageAsync(_chatId, message.TakeTillLength(4000)).GetAwaiter().GetResult();
             }
        }
    }
}

