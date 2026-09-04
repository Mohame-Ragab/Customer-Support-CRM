namespace CustomerSupportCRM.API.Extensions;

public static class SignalRServiceExtensions
{
    public static IServiceCollection AddChatRealtime(this IServiceCollection services)
    {
        services.AddSignalR();
        return services;
    }
}
