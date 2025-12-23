using Microsoft.EntityFrameworkCore;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DatabaseControl
    {
        public static List<Phone> GetPhonesForView()
        {
            using (DbAppContext context = new DbAppContext())
            {
                return context.Phones.Include(p => p.CompanyEntity).ToList();
            }
        }


        public static void AddPhone(Phone phone)
        {
            using (DbAppContext context = new DbAppContext())
            {
                context.Phones.Add(phone);
                context.SaveChanges();
            }
        }


        public static void UpdatePhone(Phone phone)
        {
            using (DbAppContext context = new DbAppContext())
            {
                Phone phone1 = context.Phones.FirstOrDefault(p => p.Id == phone.Id);

                phone1.Title = phone.Title;
                phone1.Price = phone.Price;
                phone1.CompanyId = phone.CompanyId;

                context.SaveChanges();
            }
        }
    }
}
