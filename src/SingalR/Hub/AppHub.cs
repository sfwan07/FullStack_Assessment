using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SingalR.API.Data;
using SingalR.API.Model;

namespace SingalR.API
{
    [Authorize]
    public class AppHub : Hub
    {
        private readonly DataContext _dataContext;
        private readonly HttpContext _httpContext;

        public AppHub(DataContext dataContext, HttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task SendToUser(SendUserModel model)
        {
            IReadOnlyList<string> connsetions =await _dataContext.ConnectedUsers
                .Where(x => x.UserId == model.userId).Select(s => s.ConnectionId).ToListAsync();
            await Clients.Clients(connsetions).SendAsync("SendToUser", model.message);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var connection = await _dataContext.ConnectedUsers.SingleOrDefaultAsync(c => c.ConnectionId == Context.ConnectionId);
                if (connection is not null)
                {
                    _dataContext.ConnectedUsers.Remove(connection);
                }

            }
            catch (Exception)
            {
            }
            await base.OnDisconnectedAsync(exception);
        }


        public override async Task OnConnectedAsync()
        {
            try
            {
                Guid userId = Guid.Parse(_httpContext.User.FindFirst("SID").Value);
                if (Guid.Empty != userId)
                {
                    _dataContext.ConnectedUsers.Add(new Model.ConnectedUser
                    {
                        ConnectionId = Context.ConnectionId,
                        UserId = userId
                    });
                    await _dataContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
            }

            await base.OnConnectedAsync();
        }
    }
}
