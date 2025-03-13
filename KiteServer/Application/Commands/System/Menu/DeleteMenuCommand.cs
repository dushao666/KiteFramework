using MediatR;

namespace Application.Commands.System.Menu
{
    public class DeleteMenuCommand : IRequest<bool>
    {
        public long Id { get; set; }
    }
} 