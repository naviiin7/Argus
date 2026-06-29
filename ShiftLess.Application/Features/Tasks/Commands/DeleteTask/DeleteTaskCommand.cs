using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShiftLess.Application.Features.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommand : IRequest<DeleteTaskResponse>
    {
        public DeleteTaskCommand(int id, int managerId)
        {
            Id = id;
            ManagerId = managerId;
        }

        public int Id { get; }
        public int ManagerId { get; }
    }
}
