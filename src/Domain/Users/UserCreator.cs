namespace IWantApp.Domain.Users
{
    public class UserCreator
    {
        public readonly UserManager<IdentityUser> _userManager;

        public UserCreator(UserManager<IdentityUser> userManager)
        {
           _userManager = userManager;
        }

        public async Task<(IdentityResult, string)> Create(string email, string password, List<Claim> claims)
        {
            var user = new IdentityUser { UserName = email, Email =email };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return (result, string.Empty);
            }

            return (await _userManager.AddClaimsAsync(user, claims), user.Id);

        }
    }
}
