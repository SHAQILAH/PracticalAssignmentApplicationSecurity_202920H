using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace PracticalAssignmentApplicationSecurity_202920H
{
    public partial class Registration : System.Web.UI.Page
    {
        string MYDB202920HConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYOWNDB202920HConnection"].ConnectionString;
        static string finalHash;
        static string salt;
        byte[] Key;
        byte[] IV;

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void submit_btnClick(object sender, EventArgs e)
        {

            // server side input validation
            fb_fname.Text = fnameChecker(tb_fname.Text);
            fb_fname.ForeColor = Color.Red;

            fb_lname.Text = lnameChecker(tb_lname.Text);
            fb_lname.ForeColor = Color.Red;

            fb_emailGeneral.Text = generalEmailChecker(tb_email.Text);
            fb_emailGeneral.ForeColor = Color.Red;

            fb_credit.Text = creditCardChecker(tb_credit.Text);
            fb_credit.ForeColor = Color.Red;

            fb_passwordGeneral.Text = generalPasswordChecker(tb_password.Text);
            fb_passwordGeneral.ForeColor = Color.Red;

            fb_dob.Text = dobChecker(tb_birthdate.Text);
            fb_dob.ForeColor = Color.Red;



            // server-based password check

            // if password isnt empty
            if (fb_passwordGeneral.Text == "")
            {


                int scores = passwordChecker(tb_password.Text);
                string rating = "";
                string requirements = "";
                switch (scores)
                {
                    case 0:
                        rating = "Very Weak";
                        requirements = "Please ensure your password has met all of the following requirements to be considered as a strong password:<br /><ul><li>Minimum 12 characters</li><li>At least 1 lowercase</li><li>At least 1 uppercase</li><li>At least 1 number</li><li>At least 1 special character</li></ul>";
                        break;
                    case 1:
                        rating = "Weak";
                        requirements = "Please ensure your password has met all of the following requirements to be considered as a strong password:<br /><ul><li>Minimum 12 characters</li><li>At least 1 lowercase</li><li>At least 1 uppercase</li><li>At least 1 number</li><li>At least 1 special character</li></ul>";
                        break;
                    case 2:
                        rating = "Medium";
                        requirements = "Please ensure your password has met all of the following requirements to be considered as a strong password:<br /><ul><li>Minimum 12 characters</li><li>At least 1 lowercase</li><li>At least 1 uppercase</li><li>At least 1 number</li><li>At least 1 special character</li></ul>";
                        break;
                    case 3:
                        rating = "Strong";
                        requirements = "Please ensure your password has met all of the following requirements to be considered as a strong password:<br /><ul><li>Minimum 12 characters</li><li>At least 1 lowercase</li><li>At least 1 uppercase</li><li>At least 1 number</li><li>At least 1 special character</li></ul>";
                        break;
                    case 4:
                        rating = "Very Strong";
                        requirements = "Please ensure your password has met all of the following requirements to be considered as a strong password:<br /><ul><li>Minimum 12 characters</li><li>At least 1 lowercase</li><li>At least 1 uppercase</li><li>At least 1 number</li><li>At least 1 special character</li></ul>";
                        break;
                    case 5:
                        rating = "Excellent";
                        requirements = "Your password has met all of the following requirements to be considered as a strong password:<br /><ul><li>Minimum 12 characters</li><li>At least 1 lowercase</li><li>At least 1 uppercase</li><li>At least 1 number</li><li>At least 1 special character</li></ul>Good job!";
                        break;
                    default:
                        break;
                }
                fb_password.Text = "Password Rating : " + rating + "<br />" + requirements;
                if (scores < 4)
                {
                    fb_password.ForeColor = Color.Red;
                    return;
                }
                fb_password.ForeColor = Color.Green;

                // Using ASP.NET Validator control for Regular expression for Password
                //lb_error.Text = "Password Complexity - Excellent!";

            }




            // securing user data and passwords
            string password = tb_password.Text.ToString().Trim();

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

            if (fu_photo.HasFile)
            {
                string extension = Path.GetExtension(fu_photo.PostedFile.FileName).ToLower();


                if (extension == ".jpeg" || extension == ".jpg" || extension == ".png" || extension == ".gif")
                {
                    
                }
                else
                {
                    fb_photo.Text = "You are uploading a file with " + extension + " extension. Only jpeg, jpg, png and gif files are accepted!";
                    fb_photo.ForeColor = Color.Red;
                }
            }


            if (fb_fname.Text == "" && fb_lname.Text == "" && fb_emailGeneral.Text == "" && fb_credit.Text == "" && fb_passwordGeneral.Text == "" && fb_dob.Text == "")
            {
                if (fu_photo.HasFile)
                {
                    string extension = Path.GetExtension(fu_photo.PostedFile.FileName).ToLower();


                    if (extension == ".jpeg" || extension == ".jpg" || extension == ".png" || extension == ".gif")
                    {
                        createUser();
                    }
                    else
                    {
                        fb_photo.Text = "You are uploading a file with " + extension + " extension. Only jpeg, jpg, png and gif files are accepted!";
                        fb_photo.ForeColor = Color.Red;
                    }
                }
                else
                {
                    createUser();
                }
                
            }
                   

            

        }

        protected void createUser()
        {
            var emailUnique = emailChecker(tb_email.Text);
            if (emailUnique == true)
            {


                try
                {
                    using (SqlConnection con = new SqlConnection(MYDB202920HConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("INSERT INTO Users VALUES(@FirstName,@LastName,@EmailAddress,@CreditCardEncrypted,@PasswordHash,@PasswordSalt,@DOB,@Photo,@DateTimeRegistered,@AccountLockout,@Counter,@DateTimeAccountLock,@DateTimePasswordChanged,@IV,@Key)"))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.AddWithValue("@FirstName", HttpUtility.HtmlEncode(tb_fname.Text.Trim()));
                                cmd.Parameters.AddWithValue("@LastName", HttpUtility.HtmlEncode(tb_lname.Text.Trim()));
                                cmd.Parameters.AddWithValue("@EmailAddress", HttpUtility.HtmlEncode(tb_email.Text.Trim()));
                                cmd.Parameters.AddWithValue("@CreditCardEncrypted", Convert.ToBase64String(encryptDataFields(HttpUtility.HtmlEncode(tb_credit.Text.Trim()))));
                                cmd.Parameters.AddWithValue("@PasswordHash", finalHash);
                                cmd.Parameters.AddWithValue("@PasswordSalt", salt);
                                cmd.Parameters.AddWithValue("@DOB", HttpUtility.HtmlEncode(tb_birthdate.Text.Trim()));

                                if (fu_photo.HasFile)
                                {
                                    
                                    string photoName = HttpUtility.HtmlEncode(fu_photo.FileName.ToString());
                                    fu_photo.PostedFile.SaveAs(Server.MapPath("~/Photos/") + photoName);
                                    cmd.Parameters.AddWithValue("@Photo", photoName);
                                    
                                    
                                    
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@Photo", DBNull.Value);
                                }
                                cmd.Parameters.AddWithValue("@DateTimeRegistered", DateTime.Now);
                                cmd.Parameters.AddWithValue("@AccountLockout", 0);
                                cmd.Parameters.AddWithValue("@Counter", 0);
                                cmd.Parameters.AddWithValue("@DateTimeAccountLock", DBNull.Value);
                                cmd.Parameters.AddWithValue("@DateTimePasswordChanged", DBNull.Value);
                                //cmd.Parameters.AddWithValue("@EmailVerified", DBNull.Value);
                                cmd.Parameters.AddWithValue("@IV", Convert.ToBase64String(IV));
                                cmd.Parameters.AddWithValue("@Key", Convert.ToBase64String(Key));
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
                Response.Redirect("Login.aspx", false);
            }
            else
            {
                fb_email.Text = "Email is already in use!";
                fb_email.ForeColor = Color.Red;
            }
        }

        protected byte[] encryptDataFields(string data)
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

        private int passwordChecker(string password)
        {
            int score;


            if (password.Length < 12)
            {
                return 0;
            }

            else
            {
                score = 1;
            }


            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            {
                score++;
            }
            return score;
        }

        private string fnameChecker(string fname)
        {
            string result = "";


            if (String.IsNullOrEmpty(fname))
            {
                result = "This field is required!";
            }

            else
            {
                if (Regex.IsMatch(fname, "[^a-zA-Z]"))
                {
                    result = "Only alphabets (lowercase or uppercase) are accepted!";
                }
            }

            return result;     
        }

        private string lnameChecker(string lname)
        {
            string result = "";


            if (String.IsNullOrEmpty(lname))
            {
                result = "This field is required!";
            }

            else
            {
                if (Regex.IsMatch(lname, "[^a-zA-Z]"))
                {
                    result = "Only alphabets (lowercase or uppercase) are accepted!";
                }
            }

            return result;
        }

        private string generalEmailChecker(string email)
        {
            string result = "";


            if (String.IsNullOrEmpty(email))
            {
                result = "This field is required!";
            }

            else
            {
                string pattern = @"^\w+[\+\.\w-]*@([\w-]+\.)*\w+[\w-]*\.([a-z]{2,4}|\d+)$";
                if (Regex.IsMatch(email, pattern))
                {
                    result = "";
                }
                else
                {
                    result = "Invalid email address used!";
                }
            }

            return result;
        }

        private string creditCardChecker(string creditcard)
        {
            string result = "";


            if (String.IsNullOrEmpty(creditcard))
            {
                result = "This field is required!";
            }

            else
            {
                string pattern = @"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$";
                if (Regex.IsMatch(creditcard, pattern))
                {
                    result = "";
                }
                else
                {
                    result = "Invalid credit card credentials entered!";
                }
                
            }

            return result;
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

        private string dobChecker(string dob)
        {
            string result = "";


            if (String.IsNullOrEmpty(dob))
            {
                result = "This field is required!";
            }

            else
            {
                // restrict future dates
                if (DateTime.Parse(dob) >= DateTime.Now)
                {
                    result = "You can only pick dates from the past till " + DateTime.Now + "!";
                }

            }

            return result;
        }

        private Boolean emailChecker(string email)
        {
            var uniqueEmail = true;
            

            SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
            string query = "select EmailAddress from Users";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                        if (reader["EmailAddress"] != null)
                        {
                            if (reader["EmailAddress"] != DBNull.Value)
                            {
                                if (reader["EmailAddress"].ToString() == email)
                                {
                                    uniqueEmail = false;
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
           
            return uniqueEmail;

                //    using (SqlConnection con = new SqlConnection(MYDB202920HConnectionString))
                //    {
                //        using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users where EmailAddress = @EmailAddress"))
                //        {
                //            using (SqlDataAdapter sda = new SqlDataAdapter())
                //            {
                //                cmd.CommandType = CommandType.Text;
                //                cmd.Parameters.AddWithValue("@EmailAddress", email);
                //                cmd.Connection = con;
                //                con.Open();
                //                SqlDataReader reader = cmd.ExecuteReader();
                //                if (reader.HasRows)
                //                {
                //                    while (reader.Read())
                //                    {
                //                        uniqueEmail = false;

                //                    }
                //                }
                //                con.Close();
                //                return uniqueEmail;

                //            }
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception(ex.ToString());
                //}
            }
    }

    

}