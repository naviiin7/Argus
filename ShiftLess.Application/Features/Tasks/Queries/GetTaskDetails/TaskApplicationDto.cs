using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Tasks.Queries.GetTaskDetails
{
    public class TaskApplicantDto
    {
        public int ApplicationId { get; set; }

        public int WorkerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
