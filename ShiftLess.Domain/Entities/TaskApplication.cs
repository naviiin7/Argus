using ShiftLess.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Domain.Entities
{
    public class TaskApplication
    {
        public int Id { get; set; }

        public int TaskRequestId { get; set; }

        public int ClientId { get; set; }

        public ApplicationStatus Status { get; set; }

        public DateTime AppliedAt { get; set; }
    }
}
