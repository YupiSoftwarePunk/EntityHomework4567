using Microsoft.EntityFrameworkCore;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
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
            string path = ctx.Request.Url.AbsolutePath.ToLower().TrimEnd('/');
            string method = ctx.Request.HttpMethod;

            Console.WriteLine($"PATH = '{path}', METHOD = '{method}'");

            using var db = new DbAppContext();

            if (path == "/api/phones" && method == "GET")
            {
                var phones = db.Phones
                    .Include(p => p.CompanyEntity)
                    .Select(p => new
                    {
                        p.Id,
                        p.Title,
                        p.Price,
                        p.CompanyId,
                        p.Description,
                        p.Image,
                        CompanyEntity = new
                        {
                            p.CompanyEntity.Id,
                            p.CompanyEntity.Title
                        }
                    })
                    .ToList();

                string json = JsonSerializer.Serialize(phones);
                await WriteResponse(ctx, json);
                return;
            }

            else if (path == "/api/phones" && method == "POST")
            {
                using var reader = new StreamReader(ctx.Request.InputStream);
                string body = await reader.ReadToEndAsync();
                var phone = JsonSerializer.Deserialize<Phone>(body);

                if (string.IsNullOrWhiteSpace(phone.Image))
                {
                    phone.Image = "C:\\Users\\Denis\\Documents\\projects C#\\EntityHomework4567\\Server\\noimage.jpg";
                }


                db.Phones.Add(phone);
                db.SaveChanges();

                await WriteResponse(ctx, "{\"status\":\"added\"}");
                return;
            }
            else if (path.StartsWith("/api/phones/") && method == "DELETE")
            {
                if (int.TryParse(path.Replace("/api/phones/", ""), out int id))
                {
                    var phone = db.Phones.Find(id);
                    if (phone == null)
                    {
                        await WriteResponse(ctx, "{\"error\":\"not found\"}", 404);
                        return;
                    }

                    db.Phones.Remove(phone);
                    db.SaveChanges();
                    await WriteResponse(ctx, "{\"status\":\"deleted\"}");
                    return;
                }
            }
            else if (path.StartsWith("/api/phones/") && method == "PUT")
            {
                if (int.TryParse(path.Replace("/api/phones/", ""), out int id))
                {
                    using var reader = new StreamReader(ctx.Request.InputStream);
                    string body = await reader.ReadToEndAsync();
                    var updatedPhone = JsonSerializer.Deserialize<Phone>(body);

                    var phone = db.Phones.Find(id);
                    if (phone == null)
                    {
                        await WriteResponse(ctx, "{\"error\":\"not found\"}", 404);
                        return;
                    }

                    phone.Title = updatedPhone.Title;
                    phone.Price = updatedPhone.Price;
                    phone.CompanyId = updatedPhone.CompanyId;
                    phone.Description = updatedPhone.Description;

                    if (string.IsNullOrWhiteSpace(updatedPhone.Image))
                    {
                        updatedPhone.Image = "C:\\Users\\Denis\\Documents\\projects C#\\EntityHomework4567\\Server\\noimage.jpg";
                    }

                    phone.Image = updatedPhone.Image;

                    db.SaveChanges();
                    await WriteResponse(ctx, "{\"status\":\"updated\"}");
                    return;
                }
            }
            else if (path == "/api/companies" && method == "GET")
            {
                var companies = db.Companies
                    .Select(c => new
                    {
                        c.Id,
                        c.Title
                    })
                    .ToList();

                string json = JsonSerializer.Serialize(companies);
                await WriteResponse(ctx, json);
                return;
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
