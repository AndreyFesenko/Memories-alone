using MediatR;
using System.Collections.Generic;
using IdentityService.Application.DTOs;

namespace IdentityService.Application.Commands;

public class GetUsersQuery : IRequest<List<UserDto>>
{
    // Можно добавить фильтры/пагинацию позже
}