using MediatR;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Entities;
using FormarkaLms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLms.Application.Resources.Commands;

public record AddResourceCommand(int LessonId, string Title, string Url, string Type) : IRequest<ResourceDto>;

public class ResourceDto
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Title { get; set; } = default!;
    public string Url { get; set; } = default!;
    public string Type { get; set; } = default!;
}

public class AddResourceCommandHandler : IRequestHandler<AddResourceCommand, ResourceDto>
{
    private readonly IApplicationDbContext _context;

    public AddResourceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResourceDto> Handle(AddResourceCommand request, CancellationToken cancellationToken)
    {
        var resource = new Resource
        {
            LessonId = request.LessonId,
            Title = request.Title,
            Url = request.Url,
            Type = Enum.Parse<ResourceType>(request.Type, true),
            CreatedAt = DateTime.UtcNow
        };

        _context.Resources.Add(resource);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResourceDto
        {
            Id = resource.Id,
            LessonId = resource.LessonId,
            Title = resource.Title,
            Url = resource.Url,
            Type = resource.Type.ToString().ToLower()
        };
    }
}
