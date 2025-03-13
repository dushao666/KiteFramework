using MediatR;

namespace Application.Commands.Menu
{
    public class DeleteMenuCommand : IRequest<bool>
    {
        public long Id { get; set; }
    }
} 