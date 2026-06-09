namespace FormarkaLms.Application.Comments.DTOs;

public class CommentDto
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string UserId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string? UserAvatar { get; set; }
    public string Content { get; set; } = default!;
    public int Likes { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ParentId { get; set; }
    public List<CommentDto> Replies { get; set; } = new List<CommentDto>();
}
