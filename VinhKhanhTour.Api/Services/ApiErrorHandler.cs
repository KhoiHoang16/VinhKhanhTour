using System;
using Microsoft.Extensions.Logging;
using VinhKhanhTour.Shared.Services;

namespace VinhKhanhTour.Api.Services
{
    public class ApiErrorHandler : IErrorHandler
    {
        private readonly ILogger<ApiErrorHandler> _logger;

        public ApiErrorHandler(ILogger<ApiErrorHandler> logger)
        {
            _logger = logger;
        }

        public void HandleError(Exception ex)
        {
            _logger.LogError(ex, "An error occurred in the repository");
        }
    }
}
