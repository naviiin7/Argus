using ShiftLess.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Domain.Entities
{
    public class TaskApplication
    {
        public int TaskApplicationId { get; set; }

        public int TaskRequestId { get; set; }

        public TaskRequest TaskRequest { get; set; } = null!;

        public int WorkerId { get; set; }

        public User Worker { get; set; } = null!;

        public DateTime AppliedAt { get; set; }

        public ApplicationStatus Status { get; set; }
    }
}
