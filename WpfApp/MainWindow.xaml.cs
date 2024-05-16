using System.Windows;
using Microsoft.Data.SqlClient;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string name, email, location;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtName.Text) && !names.Items.Contains(txtName.Text) && !string.IsNullOrWhiteSpace(txtLocation.Text) && !names.Items.Contains(txtLocation.Text) && !string.IsNullOrWhiteSpace(txtEmail.Text) && !names.Items.Contains(txtEmail.Text))
            {
                name = txtName.Text;
                location = txtLocation.Text;
                email = txtEmail.Text;

                try
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                    builder.DataSource = "MTM-NB20160010\\MSSQLSERVER01";
                    builder.InitialCatalog = "TutorialDB";
                    builder.IntegratedSecurity = true;
                    builder.TrustServerCertificate = true;

                    // Connect to SQL
                    Console.Write("Connecting to SQL Server ... \n");
                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        connection.Open();
                        Console.WriteLine("Connected to the server.");
                        string querysql = "SELECT * FROM Customers";
                        string insertsql = "INSERT INTO Customers (Name, Location, Email) VALUES (@name, @location, @email)";

                        SqlCommand querycommand = new SqlCommand(querysql, connection);
                        SqlCommand insertcommand = new SqlCommand(insertsql, connection);

                        try
                        {
                            SqlDataReader reader = querycommand.ExecuteReader();
                            while (reader.Read())
                            {
                                names.Items.Add(reader["Name"]);
                                names.Items.Add(reader["Location"]);
                                names.Items.Add(reader["Email"]);

                            }

                            connection.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                        names.Items.Add(name);
                        names.Items.Add(location);
                        names.Items.Add(email);



                        try
                        {
                            connection.Open();
                            insertcommand.Parameters.AddWithValue("@name", name);
                            insertcommand.Parameters.AddWithValue("@location", location);
                            insertcommand.Parameters.AddWithValue("@email", email);
                            int rowAffected = insertcommand.ExecuteNonQuery();
                            Console.WriteLine($"Inserted {rowAffected} row(s)!");
                            connection.Close();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                txtName.Clear();
                txtLocation.Clear();
                txtEmail.Clear();
            }
            else
            {
                MessageBox.Show("Please fill in all the details!");
            }
        }
        private void OnClickNuke(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = "MTM-NB20160010\\MSSQLSERVER01";
                builder.InitialCatalog = "TutorialDB";
                builder.IntegratedSecurity = true;
                builder.TrustServerCertificate = true;

                // Connect to SQL

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();

                    string deletesql = "TRUNCATE TABLE Customers";

                    SqlCommand deletecommand = new SqlCommand(deletesql, connection);

                    try
                    {
                        if (names.Items.Count>0) {
                            names.Items.Clear();
                            deletecommand.ExecuteNonQuery(); 
                        }else
                        {
                            MessageBox.Show("List is Empty");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to delete " + ex.ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
