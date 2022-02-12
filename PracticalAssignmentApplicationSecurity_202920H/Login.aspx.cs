using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PracticalAssignmentApplicationSecurity_202920H
{

    public class MyObject
    {
        public string success { get; set; }
        public List<string> ErrorMsg { get; set; }
    }
    public partial class Login : System.Web.UI.Page
    {
        string MYDB202920HConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYOWNDB202920HConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;


        protected void Page_Load(object sender, EventArgs e)
        {
            // account recovery (after 2 minutes, account is no longer locked out)
            if (DateTime.Now >= getDateTimeAccountLock().AddMinutes(2))
            {
                
                getAutomaticAccountRecovery();
            }
            
        }

        protected void submit_btnClick(object sender, EventArgs e)
        {
            int counter;


            string password = tb_password.Text.ToString().Trim();
            string email = tb_email.Text.ToString().Trim();

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);

            if (ValidateCaptcha())
            {



                try
                {
                    if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                    {
                        string passwordWithSalt = password + dbSalt;
                        byte[] passwordHashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                        string userHash = Convert.ToBase64String(passwordHashWithSalt);

                        if (userHash.Equals(dbHash))
                        {
                            if (checkAccountLockout() == false)
                            {
                                insertSignIn();
                                Session["LoggedIn"] = tb_email.Text.Trim();

                                Session["auditID"] = getAuditId();


                                //Session["DateTimeLoggedIn"] = getDateTimeLoggedIn();

                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;

                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                Response.Redirect("Default.aspx", false);
                            }
                            else
                            {
                                error_message.Text = "Your account has been lockout!";
                                error_message.ForeColor = Color.Red;

                            }

                        }

                        else
                        {
                            increaseCounter();
                            counter = getCounter();

                            error_message.Text = "Email or password is not valid. Please try again.";
                            error_message.ForeColor = Color.Red;


                            if (counter >= 3)
                            {
                                SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
                                string sql = "select AccountLockout from Users where EmailAddress=@EmailAddress";
                                SqlCommand command = new SqlCommand(sql, connection);
                                command.Parameters.AddWithValue("@EmailAddress", tb_email.Text.ToString().Trim());

                                try
                                {
                                    connection.Open();

                                    using (SqlDataReader reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            if (reader["AccountLockout"] != null)
                                            {
                                                if (reader["AccountLockout"] != DBNull.Value)
                                                {
                                                    try
                                                    {
                                                        using (SqlConnection con = new SqlConnection(MYDB202920HConnectionString))
                                                        {
                                                            using (SqlCommand cmd = new SqlCommand("UPDATE Users SET AccountLockout=@AccountLockout, DateTimeAccountLock=@DateTimeAccountLock WHERE EmailAddress=@EmailAddress"))
                                                            {
                                                                using (SqlDataAdapter sda = new SqlDataAdapter())
                                                                {
                                                                    cmd.CommandType = CommandType.Text;
                                                                    cmd.Parameters.AddWithValue("@AccountLockout", 1);
                                                                    cmd.Parameters.AddWithValue("@DateTimeAccountLock", DateTime.Now);
                                                                    cmd.Parameters.AddWithValue("@EmailAddress", HttpUtility.HtmlEncode(tb_email.Text.Trim()));

                                                                    cmd.Connection = con;
                                                                    con.Open();
                                                                    cmd.ExecuteNonQuery();
                                                                    con.Close();

                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        throw new Exception(ex.ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                catch (Exception ex)
                                {
                                    throw new Exception(ex.ToString());
                                }
                                finally { connection.Close(); }
                            }
                            //Response.Redirect("Login.aspx");
                        }
                    }
                    else
                    {
                        error_message.Text = "Email or password is not valid. Please try again.";
                        error_message.ForeColor = Color.Red;
                    }
                }

                catch (Exception ex)
                {
                    throw new Exception(ex.ToString());
                }

                finally { }


            }
            else
            {
                error_captcha.Text = "Validate captcha to prove that you are a human.";
                error_captcha.ForeColor = Color.Red;
            }



        }

        protected string getDBHash(string email)
        {
            string theHash = null;

            SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
            string query = "select PasswordHash FROM Users WHERE EmailAddress=@EmailAddress";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmailAddress", email);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordHash"] != null)
                        {
                            if (reader["PasswordHash"] != DBNull.Value)
                            {
                                theHash = reader["PasswordHash"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return theHash;

        }

        protected string getDBSalt(string email)
        {
            string theSalt = null;

            SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
            string query = "select PasswordSalt from Users where EmailAddress=@EmailAddress";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmailAddress", email);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["PasswordSalt"] != null)
                        {
                            if (reader["PasswordSalt"] != DBNull.Value)
                            {
                                theSalt = reader["PasswordSalt"].ToString();
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return theSalt;
        }

        protected byte[] encryptData(string data)
        {
            byte[] cipherText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();
                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);

            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { }
            return cipherText;
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            string captchaResponse = Request.Form["g-recaptcha-response"];
            
            // the secret key is removed as stated in the assignment submission folder

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                (" https://www.google.com/recaptcha/api/siteverify?secret= &response=" + captchaResponse);

            try
            {

                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {

                        string jsonResponse = readStream.ReadToEnd();


                        lb_gScore.Text = jsonResponse.ToString();
                        lb_gScore.ForeColor = Color.Green;

                        JavaScriptSerializer js = new JavaScriptSerializer();


                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);


                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }
                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        protected int getAuditId()
        {
            int auditId = 0;

            SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
            string query = "select Id FROM AuditLog WHERE EmailAddress=@EmailAddress";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmailAddress", HttpUtility.HtmlEncode(tb_email.Text.Trim()));

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Id"] != null)
                        {
                            if (reader["Id"] != DBNull.Value)
                            {

                                auditId = Convert.ToInt32(reader["Id"]);
                                

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return auditId;
            

        }

        protected bool checkAccountLockout()
        {
            bool theResult = false;

            SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
            string query = "select AccountLockout FROM Users WHERE EmailAddress=@EmailAddress";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmailAddress", HttpUtility.HtmlEncode(tb_email.Text.Trim()));

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["AccountLockout"] != null)
                        {
                            if (reader["AccountLockout"] != DBNull.Value)
                            {
                                if (Convert.ToInt32(reader["AccountLockout"]) != 0)
                                {
                                    theResult = true;
                                }
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { connection.Close(); }
            return theResult;

        }

        protected int getCounter()
        {
            int theCounter = 0;

            SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
            string query = "select Counter from Users where EmailAddress=@EmailAddress";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmailAddress", HttpUtility.HtmlEncode(tb_email.Text.ToString().Trim()));

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Counter"] != null)
                        {
                            //if (reader["Counter"] != DBNull.Value)
                            //{
                                theCounter = Convert.ToInt32(reader["Counter"]);
                            //}
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { connection.Close(); }
            return theCounter;

        }

        protected int increaseCounter()
        {
            int theCounter = getCounter();
            
            

            SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
            string query = "select Counter from Users where EmailAddress=@EmailAddress";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmailAddress", HttpUtility.HtmlEncode(tb_email.Text.ToString().Trim()));

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["Counter"] != null)
                        {
                            
                                try
                                {
                                    using (SqlConnection con = new SqlConnection(MYDB202920HConnectionString))
                                    {
                                        using (SqlCommand cmd = new SqlCommand("UPDATE Users SET Counter=@Counter WHERE EmailAddress=@EmailAddress"))
                                        {
                                            using (SqlDataAdapter sda = new SqlDataAdapter())
                                            {
                                                cmd.CommandType = CommandType.Text;
                                                cmd.Parameters.AddWithValue("@Counter", theCounter + 1);
                                                cmd.Parameters.AddWithValue("@EmailAddress", HttpUtility.HtmlEncode(tb_email.Text.Trim()));

                                                cmd.Connection = con;
                                                con.Open();
                                                cmd.ExecuteNonQuery();
                                                con.Close();

                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(ex.ToString());
                                }
                            }
                       
                        }
                    }
                
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { connection.Close(); }
            return theCounter;

        }

        protected void insertSignIn()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(MYDB202920HConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO AuditLog VALUES(@EmailAddress,@DateTimeUserLogsIn,@DateTimeUserLogsOut)"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.Text;
                            
                            cmd.Parameters.AddWithValue("@EmailAddress", HttpUtility.HtmlEncode(tb_email.Text.Trim()));
                           
                            cmd.Parameters.AddWithValue("@DateTimeUserLogsIn", DateTime.Now);
                            
                            cmd.Parameters.AddWithValue("@DateTimeUserLogsOut", DBNull.Value);
                            
                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
           





        }

        protected DateTime getDateTimeAccountLock()
        {
            DateTime dateTimeAccountLock = DateTime.Now;

            SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
            string query = "select DateTimeAccountLock from Users where EmailAddress=@EmailAddress";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmailAddress", HttpUtility.HtmlEncode(tb_email.Text.ToString().Trim()));

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["DateTimeAccountLock"] != null)
                        {
                            if (reader["DateTimeAccountLock"] != DBNull.Value)
                            {
                                dateTimeAccountLock = Convert.ToDateTime(reader["DateTimeAccountLock"]);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { connection.Close(); }
            return dateTimeAccountLock;

        }

        protected void getAutomaticAccountRecovery()
        {

            try
            {
                using (SqlConnection con = new SqlConnection(MYDB202920HConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Users SET DateTimeAccountLock=@DateTimeAccountLock, AccountLockout=@AccountLockout, Counter=@Counter WHERE EmailAddress=@EmailAddress"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {

                            
                            cmd.Parameters.AddWithValue("@EmailAddress", HttpUtility.HtmlEncode(tb_email.Text.ToString().Trim()));
                            cmd.Parameters.AddWithValue("@DateTimeAccountLock", DBNull.Value);
                            cmd.Parameters.AddWithValue("@AccountLockout", 0);
                            cmd.Parameters.AddWithValue("@Counter", 0);


                            cmd.Connection = con;
                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            

        }

    }

    



}
