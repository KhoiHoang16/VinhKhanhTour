using System;
using Microsoft.Extensions.Logging;
using VinhKhanhTour.Shared.Services;

namespace VinhKhanhTour.CMS.Services
{
    public class CmsErrorHandler : IErrorHandler
    {
        private readonly ILogger<CmsErrorHandler> _logger;

        public CmsErrorHandler(ILogger<CmsErrorHandler> logger)
        {
            _logger = logger;
        }

        public void HandleError(Exception ex)
        {
            _logger.LogError(ex, "An error occurred in the repository");
        }
    }
}
