using MediatR;

namespace Application.Command.Menu
{
    public class DeleteMenuCommand : IRequest<bool>
    {
        public long Id { get; set; }
    }
} 