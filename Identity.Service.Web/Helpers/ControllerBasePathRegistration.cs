using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Identity.Service.Web.Helpers;

public class ControllerBasePathRegistration : IApplicationModelConvention
{
    private readonly AttributeRouteModel _routePrefix;

    public ControllerBasePathRegistration(string prefix)
    {
        _routePrefix = new AttributeRouteModel(new RouteAttribute(prefix));
    }

    public void Apply(ApplicationModel application)
    {
        var allSelectors = application.Controllers
            .SelectMany(controller => controller.Selectors)
            .ToList();

        foreach (var selector in allSelectors)
        {
            selector.AttributeRouteModel = selector.AttributeRouteModel != null
                ? AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel)
                : _routePrefix;
        }
    }
}
