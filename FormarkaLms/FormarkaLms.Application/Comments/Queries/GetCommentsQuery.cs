using MediatR;
using Microsoft.EntityFrameworkCore;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Application.Comments.DTOs;

namespace FormarkaLms.Application.Comments.Queries;

public record GetCommentsQuery(int LessonId) : IRequest<List<CommentDto>>;

public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, List<CommentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCommentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Include(c => c.Replies)
                .ThenInclude(r => r.User)
            .Where(c => c.LessonId == request.LessonId && c.ParentId == null)
            .OrderByDescending(c => c.Id)
            .Select(c => MapToDto(c))
            .ToListAsync(cancellationToken);

        return comments;
    }

    private static CommentDto MapToDto(FormarkaLms.Domain.Entities.Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            LessonId = comment.LessonId,
            UserId = comment.UserId,
            UserName = comment.User != null ? comment.User.FullName : "Unknown",
            UserAvatar = comment.User != null ? comment.User.ProfilePictureUrl : null,
            Content = comment.Content,
            Likes = comment.Likes,
            CreatedAt = comment.CreatedAt,
            ParentId = comment.ParentId,
            Replies = comment.Replies.Select(MapToDto).ToList()
        };
    }
}
