using MediatR;

namespace Application.Commands.System.Post
{
    public class DeletePostCommand : IRequest<bool>
    {
        public long Id { get; set; }
    }
} 