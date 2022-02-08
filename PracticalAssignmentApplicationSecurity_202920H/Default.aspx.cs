using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PracticalAssignmentApplicationSecurity_202920H
{
    public partial class _Default : Page
    {
        string MYDB202920HConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MYOWNDB202920HConnection"].ConnectionString;
        byte[] Key;
        byte[] IV;
        
        byte[] credit = null;
        
        
        
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
                    lb_message.Text = "Congratulations! You are logged in!";
                    lb_message.ForeColor = System.Drawing.Color.Green;
                    logout_btn.Visible = true;

                    lb_thank.Text = "Thank you " + getName() + " for signing up with us!";
                    displayUserProfile(Session["LoggedIn"].ToString());

                    // maximum password age
                    if (getDateTimePasswordChanged() != "null")
                    {
                        DateTime maximumAge = Convert.ToDateTime(getDateTimePasswordChanged());
                        if (DateTime.Now >= maximumAge.AddMinutes(5))
                        {
                            Response.Redirect("ChangePassword.aspx", false);
                        }
                    }
                    

                }
            }

            else
            {
                
                Response.Redirect("Login.aspx", false);
            }

        }

        protected void LogoutAccount(object sender, EventArgs e)
        {
            updateDateTimeUserLogsOut();
                            



            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }


        }

        protected void updateDateTimeUserLogsOut()
        {


            try
            {
                using (SqlConnection con = new SqlConnection(MYDB202920HConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE AuditLog SET DateTimeUserLogsOut=@DateTimeUserLogsOut WHERE Id=@auditId"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {

                            //cmd.Parameters.AddWithValue("@EmailAddress", Session["LoggedIn"]);
                            //cmd.Parameters.AddWithValue("@DateTimeUserLogsIn", Session["DateTimeLoggedIn"]);
                            cmd.Parameters.AddWithValue("@auditId", Session["auditID"]);
                            //cmd.Parameters.AddWithValue("@previousDateTime", DBNull.Value);
                            cmd.Parameters.AddWithValue("@DateTimeUserLogsOut", DateTime.Now);


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

        protected void displayUserProfile(string email)
        {
            SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
            string sql = "select * FROM Users WHERE EmailAddress=@EmailAddress";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EmailAddress", email);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["FirstName"] != DBNull.Value)
                        {
                            db_fname.Text = reader["FirstName"].ToString();
                        }
                        if (reader["LastName"] != DBNull.Value)
                        {
                            db_lname.Text = reader["LastName"].ToString();
                        }
                        if (reader["EmailAddress"] != DBNull.Value)
                        {
                            db_email.Text = reader["EmailAddress"].ToString();
                        }
                        if (reader["CreditCardEncrypted"] != DBNull.Value)
                        {
                            credit = Convert.FromBase64String(reader["CreditCardEncrypted"].ToString());
                        }
                        if (reader["DOB"] != DBNull.Value)
                        {
                            db_dob.Text = reader["DOB"].ToString();
                        }
                        if (reader["Photo"] != DBNull.Value)
                        {
                            db_image.Visible = true;
                            
                            db_image.Attributes["src"] = ResolveUrl("~/Photos/" + reader["Photo"].ToString());
                            
                        }
                        else
                        {
                            db_image.Visible = false;
                        }
                        if (reader["IV"] != DBNull.Value)
                        {
                            IV = Convert.FromBase64String(reader["IV"].ToString());
                        }
                        if (reader["Key"] != DBNull.Value)
                        {
                            Key = Convert.FromBase64String(reader["Key"].ToString());
                        }

                    }
                    
                    db_credit.Text = decryptData(credit);
                   
                }
            } //try

            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        protected string decryptData(byte[] cipherText)
        {
            string plainText = null;

            try
            {
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.IV = IV;
                cipher.Key = Key;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decryting stream
                            // and place them in a string
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
        }
    

        protected string getName()
        {
            string theName = "";

            SqlConnection connection = new SqlConnection(MYDB202920HConnectionString);
            string query = "select FirstName,LastName from Users where EmailAddress=@EmailAddress";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@EmailAddress", Session["LoggedIn"]);

            try
            {
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["FirstName"] != null && reader["LastName"] != null)
                        {
                            if (reader["FirstName"] != DBNull.Value && reader["LastName"] != DBNull.Value)
                            {
                                theName = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
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
            return theName;

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