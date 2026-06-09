using MediatR;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Domain.Entities;
using FormarkaLms.Application.Comments.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLms.Application.Comments.Commands;

public record AddCommentCommand(int LessonId, string UserId, string Content, int? ParentId = null) : IRequest<CommentDto>;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentDto>
{
    private readonly IApplicationDbContext _context;

    public AddCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = new Comment
        {
            LessonId = request.LessonId,
            UserId = request.UserId,
            Content = request.Content,
            ParentId = request.ParentId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync(cancellationToken);

        // Fetch with user info for DTO
        var savedComment = await _context.Comments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == comment.Id, cancellationToken);

        return new CommentDto
        {
            Id = savedComment!.Id,
            LessonId = savedComment.LessonId,
            UserId = savedComment.UserId,
            UserName = savedComment.User?.FullName ?? "Unknown",
            UserAvatar = savedComment.User?.ProfilePictureUrl,
            Content = savedComment.Content,
            Likes = savedComment.Likes,
            CreatedAt = savedComment.CreatedAt,
            ParentId = savedComment.ParentId
        };
    }
}
