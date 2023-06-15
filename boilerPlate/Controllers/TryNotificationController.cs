using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using boilerPlate.Controllers;
using boilerPlate.Infra.ServiceContracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace boilerPlate.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TryNotificationController : Controller
    {
        private readonly ILogger<TryNotificationController> _logger;
        private readonly IMediator _mediator;
        public TryNotificationController(ILogger<TryNotificationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public string PublishNews(NewsItem newsItem)
        {
            var notification = new NewsNotification { NewsItem = newsItem };
            _mediator.Publish(notification);

            return "Message Published";
        }
        
    }
    [DataContract]
    public class NewsItem
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string FullNews { get; set; }
    }
    public class NewsNotification : INotification
    {
        public NewsItem NewsItem { get; set; }
    }
    public class NewsNotificationHandler : INotificationHandler<NewsNotification>
    {
        private readonly ILogger<NewsNotificationHandler> _logger;
        public NewsNotificationHandler(ILogger<NewsNotificationHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(NewsNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogError($"Title:{notification.NewsItem.Title}>>>>>{notification.NewsItem.FullNews}");
            // Perform any required processing here
            return Task.CompletedTask;
        }
    }
    public class NewsNotificationHandler2 : INotificationHandler<NewsNotification>
    {
        private readonly ILogger<NewsNotificationHandler2> _logger;
        public NewsNotificationHandler2(ILogger<NewsNotificationHandler2> logger)
        {
            _logger = logger;
        }
        public Task Handle(NewsNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogError($"Title:{notification.NewsItem.Title}------{notification.NewsItem.FullNews}");
            // Perform any required processing here
            return Task.CompletedTask;
        }
    }
}


