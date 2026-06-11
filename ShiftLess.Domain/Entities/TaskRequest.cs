using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Domain.Entities
{
    public class TaskRequest
    {
        public int Id { get; set; }

        public int ManagerId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Budget { get; set; }

        public int RequiredClients { get; set; }

        public DateTime Deadline { get; set; }

        public TaskStatus Status { get; set; }
    }
}
