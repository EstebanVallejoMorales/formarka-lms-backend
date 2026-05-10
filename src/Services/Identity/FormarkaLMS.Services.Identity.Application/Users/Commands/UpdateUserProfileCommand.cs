using FormarkaLMS.Services.Identity.Application.DTOs;
using FormarkaLMS.Services.Identity.Domain.Interfaces;
using MediatR;

namespace FormarkaLMS.Services.Identity.Application.Users.Commands;

public record UpdateUserProfileCommand(Guid Id, UpdateUserProfileDto UpdateDto) : IRequest<bool>;

public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IUserProfileRepository _repository;

    public UpdateUserProfileHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id);
        if (user == null) return false;

        user.FullName = request.UpdateDto.FullName;
        user.AvatarUrl = request.UpdateDto.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(user);
        return true;
    }
}
