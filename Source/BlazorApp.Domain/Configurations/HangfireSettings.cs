namespace BlazorApp.Domain.Configurations
{
    public class HangfireSettings
    {
        public TimeSpan QueuePollInterval { get; set; }

        public TimeSpan InvisibilityTimeout { get; set; }

        public TimeSpan HeartbeatInterval { get; set; }

        public List<string>? Queues { get; set; }

        public TimeSpan SchedulePollingInterval { get; set; }

        public TimeSpan ServerCheckInterval { get; set; }
        
        public string? ServerName { get; set; }

        public TimeSpan ServerTimeout { get; set; }

        public TimeSpan ShutdownTimeout { get; set; }

        public int WorkerCount { get; set; }

        public string? Route { get; set; }

        public string? AppPath { get; set; }

        public int StatsPollingInterval { get; set; }

        public string? DashboardTitle { get; set; }
    }
}
