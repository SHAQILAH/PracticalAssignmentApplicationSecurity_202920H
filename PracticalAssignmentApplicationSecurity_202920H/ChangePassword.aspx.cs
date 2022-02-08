using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PracticalAssignmentApplicationSecurity_202920H
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        string MYDB202920HConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYOWNDB202920HConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedIn"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }

                else
                {
                    // minimum password age
                    if (getDateTimePasswordChanged() != "null")
                    {
                        DateTime minimumAge = Convert.ToDateTime(getDateTimePasswordChanged());
                        if (DateTime.Now <= minimumAge.AddMinutes(2))
                        {
                            tb_currentpassword.Visible = false;
                            tb_newpassword.Visible = false;
                            save_btn.Visible = false;
                            lb_currentpassword.Visible = false;
                            lb_newpassword.Visible = false;
                            lb_change.Text = "Minimum Password Age has not yet reached!<br> You can't change your password now. <br>Try again later.";
                        }
                        else
                        {
                            tb_currentpassword.Visible = true;
                            tb_newpassword.Visible = true;
                            save_btn.Visible = true;
                            lb_change.Text = "Change Password";

                        }
                    }

                }
            }

            else
            {

                Response.Redirect("Login.aspx", false);
            }
        }

        protected void save_btnClick(object sender, EventArgs e)
        {
            

            




            //if (!String.IsNullOrEmpty(tb_newpassword.Text) && !String.IsNullOrEmpty(tb_currentpassword.Text))
            //{
                


            
                newPassword();
            //}
            
                

                

            




















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

        protected void newPassword()
        {
            string currentpassword = tb_currentpassword.Text.ToString().Trim();
            string email = Session["LoggedIn"].ToString();
            //fb_error.Text = email;

            SHA512Managed hashing = new SHA512Managed();
            string dbHash = getDBHash(email);
            string dbSalt = getDBSalt(email);
            try
            {
                if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                {
                    string currentPasswordWithSalt = currentpassword + dbSalt;
                    byte[] currentPasswordHashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(currentPasswordWithSalt));
                    string userHash = Convert.ToBase64String(currentPasswordHashWithSalt);

                    

                    // check if password in db currently and the one in the textbox - whether they the same
                    if (userHash.Equals(dbHash))
                    {
                        // since current password in db = current password in textbox, just check if the new password is not the same with current
                        // so wont reuse old password (so i guess is 1 password history only? idk better than nothing)
                        if (tb_currentpassword.Text != tb_newpassword.Text)
                        {
                            //int scores = passwordChecker(fb_newpassword.Text);
                            //fb_error.Text = scores.ToString();
                            //if (scores > 0)
                            //{
                                updatePassword();
                                updateDateTimePasswordChanged();
                                fb_save.Text = "Your password has been updated successfully!";
                                fb_save.ForeColor = Color.Green;
                            //}
                            //else
                            //{

                            //    fb_save.Text = "Please ensure your password has met all of the following requirements to be considered as a strong password:<br /><ul><li>Minimum 12 characters</li><li>At least 1 lowercase</li><li>At least 1 uppercase</li><li>At least 1 number</li><li>At least 1 special character</li></ul>";
                            //    fb_save.ForeColor = Color.Red;
                            //}
                            
                        }
                        else
                        {
                            fb_save.Text = "Current and new passwords are the same! No changes made!";
                            fb_save.ForeColor = Color.Red;
                        }

                    }
                    else
                    {
                        fb_save.Text = "Incorrect current password!";
                        fb_save.ForeColor = Color.Red;

                    }
                }
                else
                {
                    fb_save.Text = "HELP";
                }
            }

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            finally { }

        }

        protected void updatePassword()
        {
            // securing user data and passwords
            string password = tb_newpassword.Text.ToString().Trim();

            RNGCryptoServiceProvider randomSalt = new RNGCryptoServiceProvider();
            byte[] saltByte = new byte[8];

            randomSalt.GetBytes(saltByte);
            salt = Convert.ToBase64String(saltByte);

            SHA512Managed generateHash = new SHA512Managed();

            string passwordWithSalt = password + salt;
            byte[] plainHash = generateHash.ComputeHash(Encoding.UTF8.GetBytes(password));
            byte[] passwordHashWithSalt = generateHash.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));

            finalHash = Convert.ToBase64String(passwordHashWithSalt);

            RijndaelManaged cipher = new RijndaelManaged();
            cipher.GenerateKey();
            Key = cipher.Key;
            IV = cipher.IV;

            try
            {
                using (SqlConnection con = new SqlConnection(MYDB202920HConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Users SET PasswordHash=@PasswordHash, PasswordSalt=@PasswordSalt WHERE EmailAddress=@EmailAddress"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {

                            cmd.Parameters.AddWithValue("@EmailAddress", Session["LoggedIn"]);
                            cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                            cmd.Parameters.AddWithValue("@PasswordSalt", salt);


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

        protected void updateDateTimePasswordChanged()
        {


            try
            {
                using (SqlConnection con = new SqlConnection(MYDB202920HConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE Users SET DateTimePasswordChanged=@DateTimePasswordChanged WHERE EmailAddress=@EmailAddress"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {

                            cmd.Parameters.AddWithValue("@EmailAddress", Session["LoggedIn"]);
                            
                            cmd.Parameters.AddWithValue("@DateTimePasswordChanged", DateTime.Now);


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
        private string generalPasswordChecker(string password)
        {
            string result = "";


            if (String.IsNullOrEmpty(password))
            {
                result = "This field is required!";
            }



            return result;
        }
        private int passwordChecker(string password)
        {
            int score;
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{12,}"))
            {
                score = 1;
            }
            else
            {
                score = 0;
            }

            


            //if (password.Length >= 12)
            //{
            //    score = 1;
            //}

            //else
            //{
            //    return 0;
            //}


            //if (Regex.IsMatch(password, "[a-z]"))
            //{
            //    score++;
            //}

            //if (Regex.IsMatch(password, "[A-Z]"))
            //{
            //    score++;
            //}

            //if (Regex.IsMatch(password, "[0-9]"))
            //{
            //    score++;
            //}

            //if (Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            //{
            //    score++;
            //}
            return score;
        }

        protected string getDateTimePasswordChanged()
        {
            string theDateTime = "";

            SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
            string query = "select DateTimePasswordChanged FROM Users WHERE EmailAddress=@EmailAddress";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmailAddress", Session["LoggedIn"].ToString());

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["DateTimePasswordChanged"] != null)
                        {
                            if (reader["DateTimePasswordChanged"] != DBNull.Value)
                            {
                                theDateTime = reader["DateTimePasswordChanged"].ToString();
                            }
                            else
                            {
                                theDateTime = "null";
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
            return theDateTime;

        }


    }
}