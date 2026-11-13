using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace ShiftLess.Application.Features.Admin.Queries.GetAllUsers;

public record GetAllUsersQuery() : IRequest<List<GetAllUsersResponse>>;