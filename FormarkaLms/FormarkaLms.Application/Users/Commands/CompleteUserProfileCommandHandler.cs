using MediatR;
using FormarkaLms.Domain.Entities;
using FormarkaLms.Domain.Enums;
using FormarkaLms.Application.Common.Interfaces;
using FormarkaLms.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace FormarkaLms.Application.Users.Commands;

public class CompleteUserProfileCommandHandler : IRequestHandler<CompleteUserProfileCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;

    public CompleteUserProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<string>> Handle(CompleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        // 1. Verificamos si el usuario ya existe en PostgreSQL para evitar duplicados
        var existingUser = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (existingUser != null)
        {
            return Result<string>.Failure(new[] { "El perfil de este usuario ya ha sido completado anteriormente." });
        }

        // 2. Parsear el rol
        if (!Enum.TryParse<UserRole>(request.Role, true, out var userRole))
        {
            userRole = UserRole.Student;
        }

        // 3. Separar nombre y apellido
        var names = request.Name.Split(' ', 2);
        var firstName = names[0];
        var lastName = names.Length > 1 ? names[1] : string.Empty;

        // 4. Creamos la entidad del Dominio utilizando el ID de Supabase
        var newUser = new User
        {
            Id = request.Id,
            Email = request.Email,
            Name = request.Name,
            FirstName = firstName,
            LastName = lastName,
            Role = userRole,
            Specialty = request.Specialty,
            PhotoUrl = request.PhotoUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // 5. Persistimos los cambios
        _context.Users.Add(newUser);

        // 6. Creamos el registro correspondiente en la tabla Student o Instructor
        if (userRole == UserRole.Student)
        {
            var student = new Student
            {
                Id = newUser.Id,
                EnrollmentDate = DateTime.UtcNow
            };
            _context.Students.Add(student);
        }
        else if (userRole == UserRole.Teacher)
        {
            var instructor = new Instructor
            {
                Id = newUser.Id,
                ProfessionalTitle = request.Specialty
            };
            _context.Instructors.Add(instructor);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(newUser.Id);
    }
}
