using MediatR;

namespace Application.Commands.System.Post
{
    public class UpdatePostStatusCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public int Status { get; set; }
    }
} 