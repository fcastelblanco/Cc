﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Isn.Common.ExtensionMethods;
using Isn.Common.LogHelper;
using Isn.Upt.Business.Definitions;
using Isn.Upt.Domain;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace Cc.Upt.Web.AuthenticationApi
{
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        /// <summary>
        ///     Get user information and find it in the DataBase
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            else
            {
                var ip = GetClientIpAddress(actionContext.Request);
                var action = actionContext.ActionDescriptor.ActionName;
                var controller = actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;

                var authToken = actionContext.Request.Headers.Authorization.Parameter;
                var decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authToken));
                var userName = decodedToken.Substring(0, decodedToken.IndexOf(":", StringComparison.Ordinal));
                var password = decodedToken.Substring(decodedToken.IndexOf(":", StringComparison.Ordinal) + 1);

                IUserService userService;
                string userPassword;
                User user;

                if (action == "GetCurrentUserData" &&
                    controller == "UserApi")
                {
                    userService = DependencyResolver.Current.GetService<IUserService>();
                    userPassword = password.Encode();
                    user = userService
                        .FindBy(u => u.UserName == userName && u.Password == userPassword).FirstOrDefault();

                    HttpContext.Current.User = new GenericPrincipal(new ApiIdentity(user), new string[] { });
                    if (user != null)
                        Log.Instance.Info("Se acepta petición desde: " + ip + ", controlador: " + controller + ", acción: " +
                                          action + ", identificador del usuario autorizado: " + user.Id);
                    base.OnActionExecuting(actionContext);
                    return;
                }

                var companyToken = actionContext.Request.Headers.GetValues("CompanyId").First();

                if (companyToken == null)
                {
                    Log.Instance.Info("Se acepta petición desde: " + ip + ", controlador: " + controller + ", acción: " +
                                      action + ", se deniega respuesta por ausencia del Id de la compañia en el header");
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    return;
                }

                var companyIdAsString = Encoding.UTF8.GetString(Convert.FromBase64String(companyToken));
                Guid companyId;
                if (!Guid.TryParse(companyIdAsString, out companyId))
                {
                    Log.Instance.Info("Se acepta petición desde: " + ip + ", controlador: " + controller + ", acción: " +
                                      action +
                                      ", se deniega respuesta por imposibilidad de convertir el identificador de la compañia");
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    return;
                }

                userService = DependencyResolver.Current.GetService<IUserService>();
                userPassword = password.Encode();
                user = userService
                    .FindBy(u => u.UserName == userName && u.Password == userPassword).FirstOrDefault();

                if (user != null)
                {
                    if (user.CompanyId != companyId)
                    {
                        Log.Instance.Info("Se acepta petición desde: " + ip + ", controlador: " + controller +
                                          ", acción: " + action + ", identificador del usuario autorizado: " + user.Id +
                                          ", se deniega respuesta porque el usuario no pertenece a la compañia");
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        return;
                    }

                    HttpContext.Current.User = new GenericPrincipal(new ApiIdentity(user), new string[] {});
                    Log.Instance.Info("Se acepta petición desde: " + ip + ", controlador: " + controller + ", acción: " +
                                      action + ", identificador del usuario autorizado: " + user.Id);
                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    Log.Instance.Info("Se acepta petición desde: " + ip + ", controlador: " + controller + ", acción: " +
                                      action + ", se deniega respuesta por credenciales del usuario");
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }
        }

        private static string GetClientIpAddress(HttpRequestMessage request)
        {
            if (!request.Properties.ContainsKey("MS_HttpContext")) return string.Empty;

            var httpContextBase = request.Properties["MS_HttpContext"] as HttpContextBase;

            if (httpContextBase?.Request.UserHostAddress != null)
                return
                    IPAddress.Parse(httpContextBase.Request.UserHostAddress)
                        .ToString();

            return string.Empty;
        }
    }
}