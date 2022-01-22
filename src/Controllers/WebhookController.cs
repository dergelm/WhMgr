﻿namespace WhMgr.Controllers
{
    using System.Collections.Generic;
    using System.Threading;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using WhMgr.Services.Webhook;

    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IWebhookProcessorService _webhookService;

        public WebhookController(
            ILogger<WebhookController> logger,
            IWebhookProcessorService webhookService)
        {
            _logger = logger;
            _webhookService = webhookService;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            _logger.LogDebug($"Endpoint GET / hit");
            return Content($"{Strings.BotName} {Strings.BotVersion} is running...");
        }

        [HttpPost("/")]
        public IActionResult HandleData(List<WebhookPayload> data)
        {
            ThreadPool.QueueUserWorkItem(async x => await _webhookService.ParseDataAsync(data));
            return Ok();
        }
    }
}