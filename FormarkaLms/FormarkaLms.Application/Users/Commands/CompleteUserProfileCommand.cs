using FormarkaLms.Application.Common.Models;
using MediatR;

namespace FormarkaLms.Application.Users.Commands;

public record CompleteUserProfileCommand(
    string Id, 
    string Email, 
    string Name, 
    string Role, 
    string Specialty, 
    string PhotoUrl
) : IRequest<Result<string>>;
