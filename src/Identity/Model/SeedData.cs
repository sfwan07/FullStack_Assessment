namespace Identity.Model
{
    public class SeedData
    {
        public List<UserModel> User { get; set; }
        public List<string> Role { get; set; }
        public List<UserRoleModel> UserRole { get; set; }

        public class UserModel
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class UserRoleModel
        {
            public string User { get; set; }
            public string Role { get; set; }
        }
    }
}
