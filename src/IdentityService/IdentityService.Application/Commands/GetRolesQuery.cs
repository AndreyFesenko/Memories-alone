using MediatR;
using System.Collections.Generic;
using IdentityService.Application.DTOs;

namespace IdentityService.Application.Commands;

public class GetRolesQuery : IRequest<List<RoleDto>> { }
