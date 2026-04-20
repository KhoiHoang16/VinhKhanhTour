using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace VinhKhanhTour.CMS.Services
{
    public class ActiveSessionTracker
    {
        private readonly ConcurrentDictionary<string, UserSessionDto> _activeSessions = new();

        public void Ping(UserSessionDto session)
        {
            session.LastActive = DateTime.Now;
            var key = $"{session.UserType}_{session.Identifier}";
            _activeSessions.AddOrUpdate(key, session, (_, existing) =>
            {
                existing.LastActive = DateTime.Now;
                existing.IpAddress = session.IpAddress ?? existing.IpAddress;
                existing.UserAgent = session.UserAgent ?? existing.UserAgent;
                existing.DisplayName = session.DisplayName ?? existing.DisplayName;
                existing.Avatar = session.Avatar ?? existing.Avatar;
                existing.Id = session.Id;
                return existing;
            });
        }

        public List<UserSessionDto> GetActiveSessions(int withinSeconds = 300) // Default 5 phút
        {
            var cutoff = DateTime.Now.AddSeconds(-withinSeconds);
            
            // Clean up old sessions
            var expiredKeys = _activeSessions.Where(kv => kv.Value.LastActive < cutoff).Select(kv => kv.Key).ToList();
            foreach (var key in expiredKeys)
            {
                _activeSessions.TryRemove(key, out _);
            }

            return _activeSessions.Values
                .Where(s => s.LastActive >= cutoff)
                .OrderByDescending(s => s.LastActive)
                .ToList();
        }
    }

    public class UserSessionDto
    {
        public int Id { get; set; }
        public string UserType { get; set; } = string.Empty;
        public string Identifier { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime LastActive { get; set; }
    }
}
