using MediatR;

namespace Application.Commands
{
    public class DeleteMenuCommand : IRequest<bool>
    {
        public long Id { get; set; }
    }
} 