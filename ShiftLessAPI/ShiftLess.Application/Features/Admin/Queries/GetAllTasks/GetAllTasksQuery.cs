using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace ShiftLess.Application.Features.Admin.Queries.GetAllTasks;

public record GetAllTasksQuery() : IRequest<List<GetAllTasksResponse>>;