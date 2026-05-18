# 🤝 Contributing Guide

## Welcome!

Thank you for your interest in contributing to the Motocross Tracking Platform! This guide will help you get started.

## Development Setup

### Prerequisites

- Node.js 20+
- .NET 8 SDK
- PostgreSQL 15+
- Git
- VS Code (recommended)

### Local Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/YOUR_USERNAME/motocross-tracking.git
   cd motocross-tracking
   ```

2. **Install frontend dependencies:**
   ```bash
   cd frontend
   npm install
   ```

3. **Install backend dependencies:**
   ```bash
   cd ../backend
   dotnet restore
   ```

4. **Set up environment variables:**
   - Copy `.env.example` to `.env.local` in frontend/
   - Update `appsettings.Development.json` in backend/src/Api/

5. **Start PostgreSQL:**
   ```bash
   # Via Docker
   docker run --name postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:15

   # Or use local PostgreSQL installation
   ```

6. **Run database migrations:**
   ```bash
   cd backend
   dotnet ef database update --project src/Infrastructure --startup-project src/Api
   ```

7. **Start the backend:**
   ```bash
   cd backend/src/Api
   dotnet run
   ```

8. **Start the frontend:**
   ```bash
   cd frontend
   npm run dev
   ```

9. **Access the application:**
   - Frontend: http://localhost:5173
   - Backend API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger

## Project Structure

```
motocross-tracking/
├── frontend/          # React + TypeScript frontend
├── backend/           # ASP.NET Core backend
├── docs/              # Documentation
└── .github/           # GitHub Actions workflows
```

## Code Standards

### Frontend (TypeScript/React)

- Use TypeScript for type safety
- Follow React hooks best practices
- Use functional components
- Implement proper error handling
- Write meaningful component names
- Keep components small and focused
- Use custom hooks for reusable logic

**Example:**
```typescript
// ✅ Good
export const SessionCard: React.FC<{ session: Session }> = ({ session }) => {
  const { data, isLoading } = useQuery(['session', session.id], fetchSession);
  
  if (isLoading) return <Loading />;
  
  return <div>...</div>;
};

// ❌ Bad
export const Card = (props: any) => {
  return <div>{props.data}</div>;
};
```

### Backend (C#/.NET)

- Follow Clean Architecture principles
- Keep controllers thin
- Put business logic in domain/application layers
- Use async/await consistently
- Implement proper exception handling
- Use meaningful variable names
- Follow SOLID principles

**Example:**
```csharp
// ✅ Good
public class SessionService : ISessionService
{
    private readonly ISessionRepository _repository;
    
    public async Task<SessionDto> CreateSessionAsync(CreateSessionCommand command)
    {
        var session = new Session(command.Name, command.Description);
        await _repository.AddAsync(session);
        return MapToDto(session);
    }
}

// ❌ Bad
public class SessionController
{
    public async Task<Session> Create(string name)
    {
        var s = new Session { Name = name };
        context.Sessions.Add(s);
        await context.SaveChangesAsync();
        return s;
    }
}
```

## Git Workflow

### Branching Strategy

- `main` - Production-ready code
- `develop` - Development branch
- `feature/*` - New features
- `bugfix/*` - Bug fixes
- `hotfix/*` - Production hotfixes

### Commit Messages

Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding tests
- `chore`: Maintenance tasks

**Examples:**
```bash
feat(tracking): add GPS position smoothing algorithm
fix(api): resolve SignalR connection timeout issue
docs(readme): update deployment instructions
refactor(domain): extract lap detection to separate service
```

### Pull Request Process

1. **Create a feature branch:**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes:**
   - Write code
   - Add tests (when applicable)
   - Update documentation

3. **Commit your changes:**
   ```bash
   git add .
   git commit -m "feat(scope): description"
   ```

4. **Push to your fork:**
   ```bash
   git push origin feature/your-feature-name
   ```

5. **Create a Pull Request:**
   - Go to GitHub
   - Click "New Pull Request"
   - Fill in the template
   - Request review

6. **Address review feedback:**
   - Make requested changes
   - Push updates
   - Respond to comments

7. **Merge:**
   - Once approved, squash and merge
   - Delete the feature branch

## Testing

### Frontend Tests

```bash
cd frontend
npm run test
```

### Backend Tests

```bash
cd backend
dotnet test
```

## Code Review Guidelines

### For Authors

- Keep PRs small and focused
- Write clear descriptions
- Add screenshots for UI changes
- Ensure CI/CD passes
- Respond to feedback promptly

### For Reviewers

- Be constructive and respectful
- Focus on code quality, not style preferences
- Ask questions, don't demand changes
- Approve when ready
- Test locally for complex changes

## Documentation

- Update README.md for user-facing changes
- Update API.md for API changes
- Update ARCHITECTURE.md for architectural changes
- Add inline code comments for complex logic
- Update DEPLOYMENT.md for deployment changes

## Release Process

1. **Version bump:**
   - Update package.json (frontend)
   - Update .csproj files (backend)

2. **Changelog:**
   - Update CHANGELOG.md
   - List all changes since last release

3. **Create release:**
   ```bash
   git tag -a v1.0.0 -m "Release v1.0.0"
   git push origin v1.0.0
   ```

4. **GitHub Release:**
   - Go to Releases
   - Create new release
   - Add release notes
   - Attach binaries (if applicable)

## Getting Help

- **Questions:** Open a GitHub Discussion
- **Bugs:** Open a GitHub Issue
- **Chat:** Join our Discord (link in README)
- **Email:** support@example.com

## Code of Conduct

- Be respectful and inclusive
- Welcome newcomers
- Focus on constructive feedback
- No harassment or discrimination
- Follow GitHub Community Guidelines

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Recognition

Contributors will be added to CONTRIBUTORS.md and recognized in release notes.

---

Thank you for contributing! 🏁
