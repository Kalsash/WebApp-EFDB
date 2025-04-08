using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace WebApp_Landing
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddLandingPages(this IServiceCollection services)
        {
            services.AddRazorPages()
                .AddApplicationPart(typeof(Pages.Pages_Index).Assembly)
                .AddApplicationPart(typeof(Pages.Pages_Error).Assembly)
                .AddApplicationPart(typeof(Pages.Pages_Privacy).Assembly) 
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddAreaFolderRouteModelConvention("Landing", "/", model =>
                    {
                        foreach (var selector in model.Selectors)
                        {
                            selector.AttributeRouteModel.Template =
                                selector.AttributeRouteModel.Template.Insert(0, "Landing/");
                        }
                    });
                });
            return services;
        }
    }
}