namespace FormarkaLms.Application.Users.Commands;

public record ChangePasswordCommand(string UserId, string NewPassword) : MediatR.IRequest<bool>;
