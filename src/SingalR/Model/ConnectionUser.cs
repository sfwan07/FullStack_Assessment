namespace SingalR.API.Model
{
    public class ConnectedUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ConnectionId { get; set; }
    }
}
