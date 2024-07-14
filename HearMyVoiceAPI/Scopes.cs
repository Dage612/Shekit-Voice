using HeayMyVoiceBusinness;
using HeayMyVoiceCommon.Contracts;

namespace HearMyVoiceAPI
{
    public class Scopes
    {
        public static IServiceCollection ConfigureScopes(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IHearService, HearBusiness>();


            return services;
        }
    }
}
