using MediatR;

namespace FormarkaLms.Application.Users.Queries;

public record CheckUserExistsQuery(string Id) : IRequest<bool>;
