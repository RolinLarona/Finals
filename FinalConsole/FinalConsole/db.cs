using System.Data.SqlClient;
using System;

namespace rental
{
    class ManageProduct
    {
        public class InsertNewProduct
        {
            private readonly string connString = "Server=localhost;Database=carrentaldb;User ID=root;Password=;";

            public void InsertData(string customerName, int vehicleid, int rentdays)
            {
                using (MySqlConnection conn = new MySqlConnection(connString))
                {
                    try
                    {
                        conn.Open();

                        string query = "INSERT INTO products (customerName, vehicleid, rentdays) VALUES (@customerName, @vehicleid, @rentdays)";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@customerName", customerName);
                            cmd.Parameters.AddWithValue("@vehicleid", vehicleid);
                            cmd.Parameters.AddWithValue("@rentdays", rentdays);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine(@"

██████ █ ██ ▄████▄ ▄████▄ ▓█████ ██████ ██████ ▐██▌ ▐██▌
▒██ ▒ ██ ▓██▒▒██▀ ▀█ ▒██▀ ▀█ ▓█ ▀ ▒██ ▒ ▒██ ▒ ▐██▌ ▐██▌
░ ▓██▄ ▓██ ▒██░▒▓█ ▄ ▒▓█ ▄▒███ ░ ▓██▄ ░ ▓██▄ ▐██▌ ▐██▌
▒ ██▒▓▓█ ░██░▒▓▓▄ ▄██▒▒▓▓▄ ▄██▒▓█ ▄ ▒ ██▒ ▒ ██▒ ▓██▒ ▓██▒
▒██████▒▒▒▒█████▓ ▒ ▓███▀ ░▒ ▓███▀ ░▒████▒▒██████▒▒▒██████▒▒ ▒▄▄ ▒▄▄
▒ ▒▓▒ ▒ ░░▒▓▒ ▒ ▒ ░ ░▒ ▒ ░░ ░▒ ▒ ░░ ▒░ ░▒ ▒▓▒ ▒ ░▒ ▒▓▒ ▒ ░ ░▀▀▒ ░▀▀▒
░ ░▒ ░ ░░░▒░ ░ ░ ░ ▒ ░ ▒ ░ ░ ░░ ░▒ ░ ░░ ░▒ ░ ░ ░ ░ ░ ░ ░
░ ░ ░░░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░
░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░
░ ░
");
                            }
                            else
                            {
                                Console.WriteLine(@"

█████▒▄▄▄ ██▓ ██▓ ▓█████ ▓█████▄ ▄▄▄█████▓ ▒█████ ██▓ ███▄ █ ██████ ▓█████ ██▀███ ▄▄▄█████▓
▓██ ▒▒████▄ ▓██▒▓██▒ ▓█ ▀ ▒██▀ ██▌ ▓ ██▒ ▓▒▒██▒ ██▒ ▓██▒ ██ ▀█ █ ▒██ ▒ ▓█ ▀ ▓██ ▒ ██▒▓ ██▒ ▓▒
▒████ ░▒██ ▀█▄ ▒██▒▒██░ ▒███ ░██ █▌ ▒ ▓██░ ▒░▒██░ ██▒ ▒██▒▓██ ▀█ ██▒░ ▓██▄ ▒███ ▓██ ░▄█ ▒▒ ▓██░ ▒░
░▓█▒ ░░██▄▄▄▄██ ░██░▒██░ ▒▓█ ▄ ░▓█▄ ▌ ░ ▓██▓ ░ ▒██ ██░ ░██░▓██▒ ▐▌██▒ ▒ ██▒▒▓█ ▄ ▒██▀▀█▄ ░ ▓██▓ ░
░▒█░ ▓█ ▓██▒░██░░██████▒░▒████▒░▒████▓ ▒██▒ ░ ░ ████▓▒░ ░██░▒██░ ▓██░▒██████▒▒░▒████▒░██▓ ▒██▒ ▒██▒ ░
▒ ░ ▒▒ ▓▒█░░▓ ░ ▒░▓ ░░░ ▒░ ░ ▒▒▓ ▒ ▒ ░░ ░ ▒░▒░▒░ ░▓ ░ ▒░ ▒ ▒ ▒ ▒▓▒ ▒ ░░░ ▒░ ░░ ▒▓ ░▒▓░ ▒ ░░
░ ▒ ▒▒ ░ ▒ ░░ ░ ▒ ░ ░ ░ ░ ░ ▒ ▒ ░ ░ ▒ ▒░ ▒ ░░ ░░ ░ ▒░░ ░▒ ░ ░ ░ ░ ░ ░▒ ░ ▒░ ░
░ ░ ░ ▒ ▒ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ▒ ▒ ░ ░ ░ ░ ░ ░ ░ ░ ░░ ░ ░
░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░ ░
░ ");
                            }
                        }
                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine($"MySQL Error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
        }
    }
}