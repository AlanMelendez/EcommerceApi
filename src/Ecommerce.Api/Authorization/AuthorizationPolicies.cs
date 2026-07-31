namespace Ecommerce.Api.Authorization
{

    //This belongs to the API Layer because policies are used by HTTP endpoints.
    public static class AuthorizationPolicies
    {
        public const string AdminOnly = "AdminOnly";
        public const string CustomerOnly = "CustomerOnly";

        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AdminOnly, policy =>
                {
                    policy.RequireRole("Admin");
                });
                options.AddPolicy(CustomerOnly, policy =>
                {
                    policy.RequireRole("Customer");
                });
            });
        }
    }
}
