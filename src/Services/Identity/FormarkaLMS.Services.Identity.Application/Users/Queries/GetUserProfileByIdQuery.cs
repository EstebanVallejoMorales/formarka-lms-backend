using FormarkaLMS.Services.Identity.Application.DTOs;
using FormarkaLMS.Services.Identity.Domain.Interfaces;
using MediatR;

namespace FormarkaLMS.Services.Identity.Application.Users.Queries;

public record GetUserProfileByIdQuery(Guid Id) : IRequest<UserProfileDto?>;

public class GetUserProfileByIdHandler : IRequestHandler<GetUserProfileByIdQuery, UserProfileDto?>
{
    private readonly IUserProfileRepository _repository;

    public GetUserProfileByIdHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserProfileDto?> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id);
        if (user == null) return null;

        return new UserProfileDto(
            user.Id,
            user.Email,
            user.FullName,
            user.AvatarUrl,
            user.Role
        );
    }
}
