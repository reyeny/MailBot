using Microsoft.AspNetCore.Identity;

public class NoPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
{
    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
    {
        return Task.FromResult(IdentityResult.Success);
    }
}