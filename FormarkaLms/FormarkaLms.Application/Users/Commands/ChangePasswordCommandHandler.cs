using FormarkaLms.Application.Common.Interfaces;
using MediatR;

namespace FormarkaLms.Application.Users.Commands;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly ISupabaseService _supabaseService;

    public ChangePasswordCommandHandler(ISupabaseService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        return await _supabaseService.UpdateUserPasswordAsync(request.UserId, request.NewPassword);
    }
}
