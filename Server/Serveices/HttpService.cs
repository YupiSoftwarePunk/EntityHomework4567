using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Serveices
{
    public class HttpService
    {
        private static HttpListener listener = new HttpListener();

        public static void Start()
        {
            listener.Prefixes.Add("http://localhost:5050/api/");
            listener.Start();
            Console.WriteLine($"HTTP сервер запущен на порту 5050");

            Task.Run(async () =>
            {
                while (true)
                {
                    var ctx = await listener.GetContextAsync();
                    await HandleRequest(ctx);
                }
            });
        }

        private static async Task HandleRequest(HttpListenerContext ctx)
        {
            string path = ctx.Request.Url.AbsolutePath.ToLower();
            string method = ctx.Request.HttpMethod;

            using var db = new DbAppContext();

            if (path == "/api/phones" && method == "GET")
            {
                var phones = db.Phones.ToList();
                string json = System.Text.Json.JsonSerializer.Serialize(phones);
                await WriteResponse(ctx, json);
            }
            else if (path == "/api/phones" && method == "POST")
            {
                using var reader = new StreamReader(ctx.Request.InputStream);
                string body = await reader.ReadToEndAsync();
                var phone = System.Text.Json.JsonSerializer.Deserialize<Phone>(body);

                db.Phones.Add(phone);
                db.SaveChanges();

                await WriteResponse(ctx, "{\"status\":\"added\"}");
            }
            else if (path.StartsWith("/api/phones/") && method == "DELETE")
            {
                int id = int.Parse(path.Replace("/api/phones/", ""));
                var phone = db.Phones.Find(id);
                if (phone != null)
                {
                    db.Phones.Remove(phone);
                    db.SaveChanges();
                    await WriteResponse(ctx, "{\"status\":\"deleted\"}");
                }
                else
                {
                    await WriteResponse(ctx, "{\"error\":\"not found\"}", 404);
                }
            }
            else if (path.StartsWith("/api/phones/") && method == "PUT")
            {
                int id = int.Parse(path.Replace("/api/phones/", ""));

                using var reader = new StreamReader(ctx.Request.InputStream);
                string body = await reader.ReadToEndAsync();
                var updatedPhone = System.Text.Json.JsonSerializer.Deserialize<Phone>(body);

                var phone = db.Phones.Find(id);
                if (phone == null)
                {
                    await WriteResponse(ctx, "{\"error\":\"not found\"}", 404);
                    return;
                }

                phone.CompanyId = updatedPhone.CompanyId;
                phone.Title = updatedPhone.Title;
                phone.Price = updatedPhone.Price;

                db.SaveChanges();

                await WriteResponse(ctx, "{\"status\":\"updated\"}");
            }
            else if (path.StartsWith("/api/companies/") && method == "GET")
            {
                var companies = db.Companies.ToList();
                string json = System.Text.Json.JsonSerializer.Serialize(companies);
                await WriteResponse(ctx, json);
            }
            else
            {
                await WriteResponse(ctx, "{\"error\":\"unknown route\"}", 404);
            }

        }


        private static async Task WriteResponse(HttpListenerContext ctx, string body, int status = 200)
        {
            ctx.Response.StatusCode = status;
            byte[] buffer = Encoding.UTF8.GetBytes(body);
            ctx.Response.ContentType = "application/json";
            await ctx.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            ctx.Response.Close();
        }
    }
}
