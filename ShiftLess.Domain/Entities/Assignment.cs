using System;
using System.Collections.Generic;
using System.Text;
using ShiftLess.Domain.Enums;

namespace ShiftLess.Domain.Entities
{
    public class Assignment
    {
        public int Id { get; set; }

        public int TaskRequestId { get; set; }

        public int ClientId { get; set; }

        public AssignmentStatus Status { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}
