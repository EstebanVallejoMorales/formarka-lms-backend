# Formarka LMS Backend

Este es el backend del MVP de Formarka LMS, desarrollado con .NET 10 y Clean Architecture, organizado como un monorepo para microservicios.

## Estructura del Proyecto

El proyecto sigue los principios de Clean Architecture para mantener la lógica de negocio aislada de las preocupaciones de infraestructura.

- **src/Services/Courses**: Gestión de cursos, módulos, lecciones y contenido.
- **src/Services/Identity**: Gestión de perfiles de usuario y roles (Integrado con Supabase Auth).
- **src/Services/Learning**: Inscripciones, progreso del estudiante y cuestionarios (Quizzes).
- **src/Shared**: Código compartido, interfaces base e infraestructura común (como Supabase).

### Capas de cada Servicio

1. **Domain**: Entidades, enums e interfaces de repositorio (Reglas de negocio puras).
2. **Application**: Casos de uso, DTOs y manejadores de comandos/consultas (MediatR).
3. **Infrastructure**: Implementación de repositorios, integración con Supabase y persistencia.
4. **API**: Controladores ASP.NET Core y punto de entrada del servicio.

## Tecnologías

- **Lenguaje**: C# / .NET 10
- **Base de Datos**: PostgreSQL (vía Supabase)
- **Autenticación**: Supabase Auth
- **Patrones**: CQRS (con MediatR), Repository Pattern, Clean Architecture.

## Cómo Ejecutar

1. Clona el repositorio.
2. Abre la solución `FormarkaLMS.sln` en Visual Studio o VS Code.
3. Configura las variables de entorno para Supabase (URL y Key) en los archivos `appsettings.json`.
4. Ejecuta los proyectos API correspondientes.

```bash
dotnet build
```
