using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using NLog;
using Identity.Service.Application.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Service.Web.Middlewares
{
    public class RequestResponseMiddleware
    {
        private readonly RequestDelegate next;
        private readonly Logger logger;
        public RequestResponseMiddleware(RequestDelegate next)
        {
            this.next = next;
            logger = LogManager.GetLogger("RequestResponseMiddleware");
        }

        public async Task Invoke(HttpContext context)
        {
            IServiceProvider services = context.RequestServices;

            NLogTrack logTrack = services.GetRequiredService<NLogTrack>();
            logTrack.TraceIdentifier = context.TraceIdentifier;
            string endpoint = context.Request.GetDisplayUrl();
            string methodType = context.Request.Method;
            logger.Info(logTrack.GetLogMessage($"API call start:Type:{methodType} EndPoint:{endpoint}"));
            if (methodType != "GET" && context.Request.ContentType != null && !context.Request.ContentType.Contains("multipart"))
            {
                dynamic requestData = await GetRequestData(context);
                logger.Info(logTrack.GetLogMessage($"Request body {JsonConvert.SerializeObject(requestData)}"));
            }
            else if (context.Request.ContentType != null && context.Request.ContentType.Contains("multipart"))
            {
                var formFieldsDictionary = GetRequestFormData(context);
                if (formFieldsDictionary.Count>0)
                {
                    string formFieldsJson = JsonConvert.SerializeObject(formFieldsDictionary);
                    logger.Info(logTrack.GetLogMessage($"Request body Form fields: {formFieldsJson}"));
                }
                context.Request.EnableBuffering();
            }
            else
            {
                // Do Nothing Sonar issue
            }
            var originalResponseBody = context.Response.Body;

            using MemoryStream newResponseBody = new();
            context.Response.Body = newResponseBody;
            try
            {
                await next(context);
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                logger.Info(logTrack.GetLogMessage($"Response body {JsonConvert.SerializeObject(responseBodyText)}"));
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await newResponseBody.CopyToAsync(originalResponseBody);
            }
            catch
            {
                context.Response.Body = originalResponseBody;
                throw;
            }
        }

        private async Task<dynamic> GetRequestData(HttpContext context)
        {
            dynamic requestData = new ExpandoObject();
            requestData.RouteValues = context.Request.RouteValues.ToList();
            requestData.QueryValues = context.Request.Query.ToList();
            context.Request.EnableBuffering();
            int count = 0;
            try { count = context.Request.Form.Files.Count; }
            catch { /*files not exists in body*/ }
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            if (count == 0)
                requestData.JsonBody = JsonConvert.DeserializeObject(await new StreamReader(context.Request.Body).ReadToEndAsync());
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            requestData = ReplaceSensitiveData(requestData);
            return requestData;
        }
        private dynamic ReplaceSensitiveData(dynamic jsonData)
        {
            string[] sensitiveKeys = { "password", "oldpassword", "newpassword" };
            try
            {
                sensitiveKeys.Where(key => jsonData.JsonBody.ContainsKey(key))
                  .ToList()
                  .ForEach(key => jsonData.JsonBody[key] = "****");
            }
            catch
            {
                logger.Info("Unable to mask sensitive data");
            }
            return jsonData;
        }

        private static Dictionary<string, string> GetRequestFormData(HttpContext context)
        {
            var formFieldsDictionary = new Dictionary<string, string>();

            AddFormFields(context, formFieldsDictionary);
            AddFormFiles(context, formFieldsDictionary);

            return formFieldsDictionary;
        }

        private static void AddFormFields(HttpContext context, Dictionary<string, string> dict)
        {
            foreach (var field in context.Request.Form)
            {
                string key = field.Key ?? string.Empty;
                string value = field.Value.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    dict[key] = value;
                }
            }
        }

        private static void AddFormFiles(HttpContext context, Dictionary<string, string> dict)
        {
            foreach (var file in context.Request.Form.Files)
            {
                string key = file.FileName ?? string.Empty;
                string value = file.ContentType ?? string.Empty;

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    dict[key] = value;
                }
            }
        }


    }
}