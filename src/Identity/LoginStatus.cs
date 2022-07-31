namespace Identity
{
    public enum LoginStatus
    {
        IsNotAllowed,
        IsLockedOut,
        RequiresTwoFactor,
        Succeeded
    }
}
