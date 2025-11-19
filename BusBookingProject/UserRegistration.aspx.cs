using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace BusBookingProject
{
    public partial class UserRegistration : System.Web.UI.Page
    {
        // Get the connection string from Web.config
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["OnlineBusBookingConnectionString"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {
            // No code needed here for this page
        }

        // Renamed function to follow C# conventions
        private int RegisterUser()
        {
            int registrationStatus = 0;

            // Use 'using' blocks to automatically manage and close connections
            // This prevents resource leaks and is much safer.
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand("ispUserRegistration", con))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    sqlCmd.Parameters.AddWithValue("@FName", txtFirstName.Text);
                    sqlCmd.Parameters.AddWithValue("@LName", txtLastName.Text);
                    sqlCmd.Parameters.AddWithValue("@EmailId", txtEmailID.Text);
                    sqlCmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    sqlCmd.Parameters.AddWithValue("@City", txtCity.Text);
                    sqlCmd.Parameters.AddWithValue("@PinCode", txtPincode.Text);
                    sqlCmd.Parameters.AddWithValue("@ContactNo", txtMobileNo.Text);
                    sqlCmd.Parameters.AddWithValue("@Password", txtPassword.Text);

                    // Create and add the output parameter
                    SqlParameter outParam = new SqlParameter("@Ret_Val", SqlDbType.BigInt)
                    {
                        Direction = ParameterDirection.Output
                    };
                    sqlCmd.Parameters.Add(outParam);

                    // Open connection and execute
                    con.Open();
                    sqlCmd.ExecuteNonQuery();

                    // Get the output value
                    registrationStatus = Convert.ToInt32(sqlCmd.Parameters["@Ret_Val"].Value);

                    // The 'using' block will automatically close the connection here,
                    // even if an error occurs.
                }
            }
            return registrationStatus;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                int registrationStatus = RegisterUser();

                if (registrationStatus > 0)
                {
                    ShowAlert("User Registration has been done successfully");
                    ClearForm();
                }
                else if (registrationStatus == -1)
                {
                    ShowAlert("Mobile No already exists. Please try with another mobile no");
                }
                else
                {
                    ShowAlert("An unknown error occurred.");
                }
            }
            catch (Exception ex)
            {
                // If an error happens (like the 'TrustServerCertificate' error), show it.
                ShowAlert("ERROR: " + ex.Message);
            }
        }

        // Helper function to show alerts
        private void ShowAlert(string message)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", $"alert('{message}');", true);
        }

        // Helper function to clear the form
        private void ClearForm()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtAddress.Text = "";
            txtMobileNo.Text = "";
            txtPincode.Text = "";
            txtCity.Text = "";
            txtPassword.Text = "";
            txtEmailID.Text = "";
        }
    }
}