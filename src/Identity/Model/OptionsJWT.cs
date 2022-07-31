namespace Identity.Model
{
    public class OptionsJWT
    {
        public string Secret { get;  set; }
        public string ValidIssuer { get;  set; } 
        public string ValidAudience { get;  set; } 
        public double TokenExpireTime { get;  set; } 
    }
}
