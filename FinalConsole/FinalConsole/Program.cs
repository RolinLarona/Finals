using System;
using System.Collections.Generic;
using rental;

class Vehicle
{
    public int VehicleID { get; set; }
    public string Model { get; set; }
    public double DailyRate { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Vehicle(int id, string model, double rate)
    {
        VehicleID = id;
        Model = model;
        DailyRate = rate;
    }
}

class Rental
{
    public int RentalID { get; set; }
    public string CustomerName { get; set; }
    public Vehicle RentedVehicle { get; set; }
    public DateTime RentDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public double TotalCost { get; private set; }
    public int RentalDays { get; set; }

    public Rental(int rentalId, string customerName, Vehicle vehicle, int rentalDays)
    {
        RentalID = rentalId;
        CustomerName = customerName;
        RentedVehicle = vehicle;
        RentDate = DateTime.Now;
        RentalDays = rentalDays;
        TotalCost = rentalDays * vehicle.DailyRate;
    }

    public void ReturnVehicle()
    {
        ReturnDate = DateTime.Now;
        RentedVehicle.IsAvailable = true;
    }
}

class Reservation
{
    public int ReservationID { get; set; }
    public string CustomerName { get; set; }
    public Vehicle ReservedVehicle { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime PickupDate { get; set; }
}

class CarRentalSystem
{
    private List<Vehicle> vehicles = new List<Vehicle>();
    private List<Rental> rentalHistory = new List<Rental>();
    private List<Reservation> reservations = new List<Reservation>();
    private int rentalCounter = 1;
    private int reservationCounter = 1;
    private string connectionString = "Server=localhost;Database=CarRental;User Id=yourUsername;Password=yourPassword;"; // IMPORTANT: Change this!

    public void AddVehicle(string model, double dailyRate)
    {
        vehicles.Add(new Vehicle(vehicles.Count + 1, model, dailyRate));
    }

    public void DisplayAvailableVehicles()
    {
        Console.Clear();
        Console.WriteLine("==================================================================================");
        Console.WriteLine(@"           

 █████╗ ██╗   ██╗ █████╗ ██╗██╗      █████╗ ██████╗ ██╗     ███████╗    ██╗   ██╗███████╗██╗  ██╗██╗ ██████╗██╗     ███████╗███████╗
██╔══██╗██║   ██║██╔══██╗██║██║     ██╔══██╗██╔══██╗██║     ██╔════╝    ██║   ██║██╔════╝██║  ██║██║██╔════╝██║     ██╔════╝██╔════╝
███████║██║   ██║███████║██║██║     ███████║██████╔╝██║     █████╗      ██║   ██║█████╗  ███████║██║██║     ██║     █████╗  ███████╗
██╔══██║╚██╗ ██╔╝██╔══██║██║██║     ██╔══██║██╔══██╗██║     ██╔══╝      ╚██╗ ██╔╝██╔══╝  ██╔══██║██║██║     ██║     ██╔══╝  ╚════██║
██║  ██║ ╚████╔╝ ██║  ██║██║███████╗██║  ██║██████╔╝███████╗███████╗     ╚████╔╝ ███████╗██║  ██║██║╚██████╗███████╗███████╗███████║
╚═╝  ╚═╝  ╚═══╝  ╚═╝  ╚═╝╚═╝╚══════╝╚═╝  ╚═╝╚═════╝ ╚══════╝╚══════╝      ╚═══╝  ╚══════╝╚═╝  ╚═╝╚═╝ ╚═════╝╚══════╝╚══════╝╚══════╝
                                                                                                                                                                                                  ");
        Console.WriteLine("==================================================================================");
        if (vehicles.Count == 0 || vehicles.TrueForAll(v => !v.IsAvailable))
        {
            Console.WriteLine("No vehicles are currently available for rent.");
        }
        else
        {
            foreach (var vehicle in vehicles)
            {
                if (vehicle.IsAvailable)
                {
                    Console.WriteLine($"[ID: {vehicle.VehicleID}] {vehicle.Model} - P{vehicle.DailyRate:F2}/day");
                }
            }
        }
        Console.WriteLine("==================================================================================\n");
    }

    public void RentVehicle(string customerName, int vehicleID)
    {
        Vehicle vehicle = vehicles.Find(v => v.VehicleID == vehicleID && v.IsAvailable);
        if (vehicle != null)
        {
            Console.Write("Enter number of rental days: ");
            if (int.TryParse(Console.ReadLine(), out int rentalDays) && rentalDays > 0)
            {
                vehicle.IsAvailable = false;
                Rental rental = new Rental(rentalCounter++, customerName, vehicle, rentalDays);
                rentalHistory.Add(rental);

                // SQL Server Insertion
                using (SqlConnection conn = new SqlConnection(connectionString)) // Use the connection string
                {
                    conn.Open();
                    string query = "INSERT INTO Rentals (RentalID, CustomerName, VehicleID, RentDate, RentalDays, TotalCost) " +  // Corrected table name to Rentals
                                   "VALUES (@RentalID, @CustomerName, @VehicleID, @RentDate, @RentalDays, @TotalCost)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RentalID", rental.RentalID);
                        cmd.Parameters.AddWithValue("@CustomerName", rental.CustomerName);
                        cmd.Parameters.AddWithValue("@VehicleID", vehicle.VehicleID); // Use vehicle.VehicleID
                        cmd.Parameters.AddWithValue("@RentDate", rental.RentDate);
                        cmd.Parameters.AddWithValue("@RentalDays", rental.RentalDays);
                        cmd.Parameters.AddWithValue("@TotalCost", rental.TotalCost);
                        cmd.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("\n==================================================================================");
                Console.WriteLine($"✅ {customerName} rented '{vehicle.Model}' for {rentalDays} days.");
                Console.WriteLine($"Rent Date: {rental.RentDate:g}");
                Console.WriteLine($"Total Cost: P{rental.TotalCost:F2}");
                Console.WriteLine("==================================================================================\n");
            }
            else
            {
                Console.WriteLine("\n❌ Invalid input for rental days.");
            }
        }
        else
        {
            Console.WriteLine("\n❌ Vehicle not available or invalid ID.");
        }
    }

    public void ReturnVehicle(int rentalID)
    {
        Rental rental = rentalHistory.Find(r => r.RentalID == rentalID && r.ReturnDate == null);
        if (rental != null)
        {
            rental.ReturnVehicle();
            using (SqlConnection conn = new SqlConnection(connectionString)) // Use the connection string
            {
                conn.Open();
                string query = "UPDATE Rentals SET ReturnDate = @ReturnDate WHERE RentalID = @RentalID";  // Corrected table name to Rentals
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ReturnDate", rental.ReturnDate);
                    cmd.Parameters.AddWithValue("@RentalID", rental.RentalID);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("\n==================================================================================");
            Console.WriteLine($"✅ {rental.CustomerName} returned '{rental.RentedVehicle.Model}'.");
            Console.WriteLine($"Total Cost: P{rental.TotalCost:F2}");
            Console.WriteLine("==================================================================================\n");
        }
        else
        {
            Console.WriteLine("\n❌ Invalid Rental ID or vehicle already returned.");
        }
    }

    public void ViewRentalHistory()
    {
        Console.Clear();
        Console.WriteLine("==================================================================================");
        Console.WriteLine(@" 
         ______ _____ _   _ _____ ___   _       _   _ _____ _____ _____ _____________   __
        | ___ \  ___| \ | |_   _/ _ \ | |     | | | |_   _/  ___|_   _|  _  | ___ \ \ / /
        | |_/ / |__ |  \| | | |/ /_\ \| |     | |_| | | | \ `--.  | | | | | | |_/ /\ V / 
        |    /|  __|| . ` | | ||  _  || |     |  _  | | |  `--. \ | | | | | |    /  \ /  
        | |\ \| |___| |\  | | || | | || |____ | | | |_| |_/\__/ / | | \ \_/ / |\ \  | |  
        \_| \_\____/\_| \_/ \_/\_| |_/\_____/ \_| |_/\___/\____/  \_/  \___/\_| \_| \_/  
                                                                                                                     ");
        Console.WriteLine("==================================================================================");
        if (rentalHistory.Count == 0)
        {
            Console.WriteLine(@"
        ╔╗╔╔═╗  ╦═╗╔═╗╔╗╔╔╦╗╔═╗╦    ╦ ╦╦╔═╗╔╦╗╔═╗╦═╗╦ ╦  ╔═╗╦  ╦╔═╗╦╦  ╔═╗╔╗ ╦  ╔═╗
        ║║║║ ║  ╠╦╝║╣ ║║║ ║ ╠═╣║    ╠═╣║╚═╗ ║ ║ ║╠╦╝╚╦╝  ╠═╣╚╗╔╝╠═╣║║  ╠═╣╠╩╗║  ║╣ 
        ╝╚╝╚═╝  ╩╚═╚═╝╝╚╝ ╩ ╩ ╩╩═╝  ╩ ╩╩╚═╝ ╩ ╚═╝╩╚═ ╩   ╩ ╩ ╚╝ ╩ ╩╩╩═╝╩ ╩╚═╝╩═╝╚═╝");
        }
        else
        {
            foreach (var rental in rentalHistory)
            {
                string status = rental.ReturnDate == null ? "Not Returned" : $"Returned on {rental.ReturnDate:g}";
                Console.WriteLine($"[ID: {rental.RentalID}] {rental.CustomerName} | {rental.RentedVehicle.Model} | P{rental.TotalCost:F2} | Rented on {rental.RentDate:g} | {status}");
            }
        }
        Console.WriteLine("==================================================================================\n");
    }

    public void ReserveVehicle(string customerName, int vehicleID, DateTime pickupDate)
    {
        Vehicle vehicle = vehicles.Find(v => v.VehicleID == vehicleID && v.IsAvailable);
        if (vehicle != null)
        {
            Reservation reservation = new Reservation
            {
                ReservationID = reservationCounter++,
                CustomerName = customerName,
                ReservedVehicle = vehicle,
                ReservationDate = DateTime.Now,
                PickupDate = pickupDate
            };
            reservations.Add(reservation);
            vehicle.IsAvailable = false;

            using (SqlConnection conn = new SqlConnection(connectionString)) // Use the connection string
            {
                conn.Open();
                string query = "INSERT INTO Reservations (ReservationID, CustomerName, VehicleID, ReservationDate, PickupDate) " +
                               "VALUES (@ReservationID, @CustomerName, @VehicleID, @ReservationDate, @PickupDate)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ReservationID", reservation.ReservationID);
                    cmd.Parameters.AddWithValue("@CustomerName", reservation.CustomerName);
                    cmd.Parameters.AddWithValue("@VehicleID", vehicle.VehicleID);
                    cmd.Parameters.AddWithValue("@ReservationDate", reservation.ReservationDate);
                    cmd.Parameters.AddWithValue("@PickupDate", reservation.PickupDate);
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("\n==================================================================================");
            Console.WriteLine(@"

                      ██████  █    ██  ▄████▄   ▄████▄ ▓█████   ██████   ██████  ▐██▌ ▐██▌ 
                     ▒██    ▒  ██  ▓██▒▒██▀ ▀█  ▒██▀ ▀█ ▓█   ▀ ▒██    ▒ ▒██    ▒  ▐██▌ ▐██▌ 
                     ░ ▓██▄   ▓██  ▒██░▒▓█    ▄ ▒▓█    ▄▒███   ░ ▓██▄   ░ ▓██▄    ▐██▌ ▐██▌  
                       ▒   ██▒▓▓█  ░██░▒▓▓▄ ▄██▒▒▓▓▄ ▄██▒▓█  ▄   ▒   ██▒  ▒   ██▒ ▓██▒ ▓██▒ 
                     ▒██████▒▒▒▒█████▓ ▒ ▓███▀ ░▒ ▓███▀ ░▒████▒▒██████▒▒▒██████▒▒ ▒▄▄  ▒▄▄  
                     ▒ ▒▓▒ ▒ ░░▒▓▒ ▒ ▒ ░ ░▒ ▒  ░░ ░▒ ▒  ░░ ▒░ ░▒ ▒▓▒ ▒ ░▒ ▒▓▒ ▒ ░ ░▀▀▒ ░▀▀▒ 
                     ░ ░▒  ░ ░░░▒░ ░ ░   ░  ▒     ░  ▒   ░ ░  ░░ ░▒  ░ ░░ ░▒  ░ ░ ░  ░ ░  ░ ░ 
                     ░  ░   ░░░ ░ ░ ░        ░          ░   ░  ░  ░  ░  ░      ░    ░ 
                        ░     ░     ░ ░      ░ ░        ░  ░      ░        ░   ░    ░  
                                    ░        ░                                          
");
            Console.WriteLine($"✅ {customerName} reserved '{vehicle.Model}' for pickup on {pickupDate:g}.");
            Console.WriteLine($"Reservation ID: {reservation.ReservationID}");
            Console.WriteLine("==================================================================================\n");
        }
        else
        {
            Console.WriteLine("\n❌ Vehicle not available or invalid ID for reservation.");
        }
    }

    public void ViewReservations()
    {
        Console.Clear();
        Console.WriteLine(@" ==================================================================================

          ______ _______ _______ _______  ______ _    _ _______      __   __  _____  _     _  ______      _    _ _______ _     _ _____ _______        _______
         |_____/ |______ |______ |______ |_____/  \  /  |______        \_/   |     | |     | |_____/       \  /  |______ |_____|   |   |       |      |______
         |    \_ |______ ______| |______ |    \_   \/   |______         |    |_____| |_____| |    \_        \/   |______ |     | __|__ |_____  |_____ |______
                                                                                                                                                     
");
        Console.WriteLine("==================================================================================");
        if (reservations.Count == 0)
        {
            Console.WriteLine(@"
███╗   ██╗ ██████╗     ██████╗ ███████╗███████╗███████╗██████╗ ██╗   ██╗ █████╗ ████████╗██╗ ██████╗ ███╗   ██╗    ███████╗ ██████╗ ██╗   ██╗███╗   ██╗██████╗ ██╗
████╗  ██║██╔═══██╗    ██╔══██╗██╔════╝██╔════╝██╔════╝██╔══██╗██║   ██║██╔══██╗╚══██╔══╝██║██╔═══██╗████╗  ██║    ██╔════╝██╔═══██╗██║   ██║████╗  ██║██╔══██╗██║
██╔██╗ ██║██║   ██║    ██████╔╝█████╗  ███████╗█████╗  ██████╔╝██║   ██║███████║   ██║   ██║██║   ██║██╔██╗ ██║    █████╗ ██║   ██║██║   ██║██╔██╗ ██║██║  ██║██║
██║╚██╗██║██║   ██║    ██╔══██╗██╔══╝  ╚════██║██╔══╝  ██╔══██╗╚██╗ ██╔╝██╔══██║   ██║   ██║██║   ██║██║╚██╗██║    ██╔══╝  ██║   ██║██║   ██║██║╚██╗██║██║  ██║╚═╝
██║ ╚████║╚██████╔╝    ██║  ██║███████╗███████║███████╗██║  ██║ ╚████╔╝ ██║  ██║   ██║   ██║╚██████╔╝██║ ╚████║    ██║     ╚██████╔╝╚██████╔╝██║ ╚████║██████╔╝██╗
╚═╝  ╚═══╝ ╚═════╝     ╚═╝  ╚═╝╚══════╝╚══════╝╚══════╝╚═╝  ╚═╝  ╚═══╝  ╚═╝  ╚═╝   ╚═╝   ╚═╝ ╚═════╝ ╚═╝  ╚═══╝    ╚═╝      ╚═════╝  ╚═════╝ ╚═╝  ╚═══╝╚═════╝ ╚═╝
                                                                                                                                                                                                                                                           ");
        }
        else
        {
            foreach (var reservation in reservations)
            {
                Console.WriteLine($"[ID: {reservation.ReservationID}] {reservation.CustomerName} | {reservation.ReservedVehicle.Model} | Pickup on: {reservation.PickupDate:g} | Reserved on: {reservation.ReservationDate:g}");
            }
        }
        Console.WriteLine("==================================================================================\n");
    }
}

class Program
{
    private static bool Login()
    {
        string username = "admin";
        string password = "admin";

        Console.Write("Enter username: ");
        string inputUsername = Console.ReadLine();
        Console.Write("Enter password: ");
        string inputPassword = Console.ReadLine();

        if (inputUsername == username && inputPassword == password)
        {
            Console.WriteLine("\n✅ Login successful!");
            return true;
        }
        else
        {
            Console.WriteLine("\n❌ Invalid credentials. Try again.");
            return false;
        }
    }

    static void Main()
    {
        CarRentalSystem rentalSystem = new CarRentalSystem();

        rentalSystem.AddVehicle("Toyota HiAce", 1000);
        rentalSystem.AddVehicle("Nissan Escapade", 1000);
        rentalSystem.AddVehicle("Toyota Vios", 700);
        rentalSystem.AddVehicle("Toyota Innova", 700);
        rentalSystem.AddVehicle("Toyota Avanza", 700);
        rentalSystem.AddVehicle("Mitsubishi Mirage", 700);

        while (!Login()) { }

        while (true)
        {
            Console.Clear();

            Console.WriteLine(@"
                       
                                 ---------------------------.
                               `/""""/""""/|""|'|""||""|   ' \.
                               /    /    / |__| |__||__|      |
                              /----------=====================|
                              | \  /V\  /    _.               |
                               ()\ \W/ /()   _            _   |
                              |   \   /     / \          / \  |-( )
                              =C========C==_| ) |--------| ) _/==] _-{_}_)
                                \_\_/__..  \_\_/_ \_\_/ \_\_/__.__.    
        ╔═╗╔═╗╦═╗  ╦═╗╔═╗╔╗╔╔╦╗╔═╗╦    ╔╦╗╔═╗╔╗╔╔═╗╔═╗╔═╗╔╦╗╔═╗╔╗╔╔╦╗  ╔═╗╦ ╦╔═╗╔╦╗╔═╗╔╦╗
        ║  ╠═╣╠╦╝  ╠╦╝║╣ ║║║ ║ ╠═╣║    ║║║╠═╣║║║╠═╣║ ╦║╣ ║║║║╣ ║║║ ║   ╚═╗╚╦╝╚═╗ ║ ║╣ ║║║
        ╚═╝╩ ╩╩╚═  ╩╚═╚═╝╝╚╝ ╩ ╩ ╩╩═╝  ╩ ╩╩ ╩╝╚╝╩ ╩╚═╝╚═╝╩ ╩╚═╝╝╚╝ ╩   ╚═╝ ╩ ╚═╝ ╩ ╚═╝╩ ╩ ");
            Console.WriteLine("=================================================================================================\n");
            Console.WriteLine("[1] View Available Vehicles\n[2] Rent a Vehicle\n[3] Return a Vehicle\n[4] View Rental History\n[5] Reserve a Car/Van\n[6] View Reservations\n[7] Exit");
            Console.Write("Select an option: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    rentalSystem.DisplayAvailableVehicles();
                    break;
                case "2":
                    Console.Write("\nEnter Customer Name: ");
                    string customerName = Console.ReadLine();
                    rentalSystem.DisplayAvailableVehicles();
                    Console.Write("Enter Vehicle ID to Rent: ");
                    if (int.TryParse(Console.ReadLine(), out int vehicleID))
                    {
                        rentalSystem.RentVehicle(customerName, vehicleID);
                    }
                    else
                    {
                        Console.WriteLine("\n❌ Invalid input.");
                    }
                    break;
                case "3":
                    Console.Write("\nEnter Rental ID to Return: ");
                    if (int.TryParse(Console.ReadLine(), out int rentalID))
                    {
                        rentalSystem.ReturnVehicle(rentalID);
                    }
                    else
                    {
                        Console.WriteLine("\n❌ Invalid input.");
                    }
                    break;
                case "4":
                    rentalSystem.ViewRentalHistory();
                    break;
                case "5":
                    Console.Write("\nEnter Customer Name: ");
                    string resCustomerName = Console.ReadLine();
                    rentalSystem.DisplayAvailableVehicles();
                    Console.Write("Enter Vehicle ID to Reserve: ");
                    if (int.TryParse(Console.ReadLine(), out int resVehicleID))
                    {
                        Console.Write("Enter Pickup Date (YYYY-MM-DD HH:MM): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime pickupDate))
                        {
                            rentalSystem.ReserveVehicle(resCustomerName, resVehicleID, pickupDate);
                        }
                        else
                        {
                            Console.WriteLine("\n❌ Invalid date format.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("\n❌ Invalid input.");
                    }
                    break;
                case "6":
                    rentalSystem.ViewReservations();
                    break;
                case "7":
                    Console.WriteLine("\n👋 Exiting program...");
                    return;
                default:
                    Console.WriteLine("\n❌ Invalid option. Try again.");
                    break;
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}

