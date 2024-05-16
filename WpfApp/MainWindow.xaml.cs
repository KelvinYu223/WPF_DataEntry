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
            List<int> list = new List<int>();

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                list.Add(1);
                MessageBox.Show("Please fill in the name!", "Empty Name", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                list.Add(2);
                MessageBox.Show("Please fill in the location!", "Empty Location", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (string.IsNullOrEmpty(txtEmail.Text))
            {
                list.Add(3);
                MessageBox.Show("Please fill in the email!", "Empty Email", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (names.Items.Contains(txtName.Text))
            {
                list.Add(4);
                MessageBox.Show("Name already exists!", "Duplicate Name", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (names.Items.Contains(txtEmail.Text))
            {
                list.Add(5);
                MessageBox.Show("Email already exists!", "Duplicate Email", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if(list.Count == 0)
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
                        if (names.Items.Count > 0)
                        {
                            MessageBoxButton button = MessageBoxButton.YesNo;
                            MessageBoxResult result;
                            result = MessageBox.Show("Are you sure?", "Delete All Data", button, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                names.Items.Clear();
                                deletecommand.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            MessageBox.Show("List is Empty", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
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
