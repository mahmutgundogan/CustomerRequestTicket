using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RealProject.Models;
using RealProject.Classes;
using System.Data.SqlClient;

namespace RealProject.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(Customer user)
        {
            if (ModelState.IsValid)
            {
                using(UserEntities db = new UserEntities())
                {
                    var usr = db.Customer.Where(u => u.Email == user.Email && u.Password == user.Password).FirstOrDefault();
                    if(usr == null)
                    {
                        using(AdminEntities AdminDb = new AdminEntities())
                        {
                            var admin = AdminDb.Admin.Where(u => u.Email == user.Email && u.Password == user.Password).FirstOrDefault();
                            if(admin == null)
                            {
                                ModelState.AddModelError("", "Username or password is wrong");
                                return View("Login");
                            }
                            else
                            {
                                Session["AdminId"] = admin.id;
                                Session["AdminName"] = admin.Name;
                                return RedirectToAction("AdminLogin");
                            }
                        }
                    }
                    else
                    {
                        Session["UserId"] = usr.id;
                        Session["Username"] = usr.Name;
                        Session["Surname"] = usr.Surname;
                        return RedirectToAction("CustomerLogin");
                    }
                }
            }
            else
            {
                return View("Login");
            }
        }

        public ActionResult AdminLogin()
        {
            if (Session["AdminId"] != null)
            {
                TicketEntities tktdb = new TicketEntities();
                return View(tktdb.Ticket.AsEnumerable());
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Details(int ?id)
        {
            TicketEntities ticketdb = new TicketEntities();
            foreach (var item in ticketdb.Ticket)
            {
                if (item.id == id)
                {
                    return View(item);
                }
            }
            return View("error");
        }

        public ActionResult CustomerLogin()
        {
            if (Session["UserId"] != null)
            {
                TicketEntities ticketdb = new TicketEntities();
                List<Ticket> tkt = new List<Ticket>();
                foreach (var item in ticketdb.Ticket)
                {
                    if(item.SenderId == (int)Session["UserId"])
                    {
                        tkt.Add(item);
                    }
                }
                return View(tkt.AsEnumerable());
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Answer(int id)
        {
            TicketEntities ticketdb = new TicketEntities();
            foreach (var item in ticketdb.Ticket)
            {
                if (item.id == id)
                {
                    return View(item);
                }
            }
            return View("error");
        }

        [HttpPost]
        public ActionResult Answer(Ticket tkt, int id)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = "Server=SAMSUNG-SAMSUNG\\SQLEXPRESS;Database=Test;Trusted_Connection=True;";
            conn.Open();
            string query = "update Ticket SET okundu = @value where id = " + id;
            SqlCommand myCommand = new SqlCommand(query,conn);
            myCommand.Parameters.AddWithValue("@value", 1);
            myCommand.ExecuteNonQuery();
            //MailSender.SendMail("gundoganm@itu.edu.tr", tkt.konu, tkt.problem);
            return RedirectToAction("AdminLogin");
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Create(Ticket tkt)
        {
            if (ModelState.IsValid)
            {
                MailSender.SendMail("gundoganm@itu.edu.tr", tkt.konu, tkt.problem);

                using (TicketEntities TicketDb = new TicketEntities())
                {
                    tkt.name = Session["Username"].ToString(); ;
                    tkt.surname = Session["Surname"].ToString();
                    tkt.SenderId = (int) Session["UserId"];
                    tkt.tarih = DateTime.Now;
                    tkt.okundu = 0;
                    TicketDb.Ticket.Add(tkt);
                    TicketDb.SaveChanges();
                }
                ModelState.Clear();
                return RedirectToAction("CustomerLogin");
            }
            else
            {
                return View("CustomerLogin");
            }
        }

    }
}