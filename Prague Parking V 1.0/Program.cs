namespace Prague_Parking_V_1._0
{
    internal class Program
    {
        static string?[] parkingSpots = new string?[100];
        static void Main(string[] args)
        {


            string[] menuOptions =


            {
                "Park vehicle",
                "Move vehicle",
                "Remove vehicle",
                "Search vehicle",
                "Show parking spots",
                "Exit",
            };

            bool running = true;

            while (running)
            {
                int choice = ShowMenu(menuOptions) + 1;
                switch (choice)
                {
                    case 1:
                        ParkVehicle();
                        break;
                    case 2:
                        MoveVehicle();
                        break;
                    case 3:
                        RemoveVehicle();
                        break;
                    case 4:
                        SearchVehicle();
                        break;
                    case 5:
                        ShowParkingSpots();
                        break;
                    case 6:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey(true);
                }
            }
        }

        static int ShowMenu(string[] options)
        {
            int selected = 0;
            ConsoleKey key;

            do
            {
                Console.Clear();
                Console.WriteLine("=== Prague Parking ===");
                Console.WriteLine();

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selected)
                    {
                        Console.WriteLine($"> {options[i]}");
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }
                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow)
                {
                    selected--;
                    if (selected < 0)
                    {
                        selected = options.Length - 1;
                    }
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    selected++;
                    if (selected >= options.Length)
                    {
                        selected = 0;
                    }
                }
            } while (key != ConsoleKey.Enter);
            return selected;
        }

        static void ParkVehicle()
        {
            Console.Clear();
            Console.WriteLine("=== Park Vehicle ===");
            Console.WriteLine();
            Console.Write("Enter vehicle type (CAR/MC): ");
            string? vehicleTypeInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(vehicleTypeInput))
            {
                Console.WriteLine("Invalid vehicle type. Please try again.");
                return;
            }
            string vehicleType = vehicleTypeInput.Trim().ToUpper();
            if (vehicleType != "CAR" && vehicleType != "MC")
            {
                Console.WriteLine("Invalid vehicle type. Please try again.");
                return;
            }
            Console.Write("Enter license plate: ");
            string? licensePlateInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(licensePlateInput))
            {
                Console.WriteLine("Invalid license plate. Please try again.");
                return;
            }
            string licensePlate = licensePlateInput.Trim().ToUpper();

            if (licensePlate.Length > 10)
            {
                Console.WriteLine("License plate can be at most 10 characters.");
                return;
            }
            if (licensePlate.Contains('#') || licensePlate.Contains('|'))
            {
                Console.WriteLine("License plate cannot contain '#' or '|'.");
                return;
            }
            if (FindVehicle(licensePlate) != -1)
            {
                Console.WriteLine("Vehicle with this license plate is already parked.");
                return;
            }
            int spot;
            if (vehicleType == "CAR")
            {
                spot = FindEmptySpot();
            }
            else
            {
                spot = FindSpotForMC();
            }

            if (spot == -1)
            {
                Console.WriteLine("No available parking spots.");
                return;
            }

            string vehicle = $"{vehicleType}#{licensePlate}";

            if (string.IsNullOrEmpty(parkingSpots[spot]))
            {
                parkingSpots[spot] = vehicle;
            }
            else
            {
                parkingSpots[spot] += "|" + vehicle;
            }
            Console.WriteLine($"Drive vehicle to spot {spot + 1}.");
        }

        static void MoveVehicle()
        {
            Console.Clear();
            Console.WriteLine("=== Move Vehicle ===");
            Console.WriteLine();
            Console.Write("Enter license plate: ");
            string? licensePlateInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(licensePlateInput))
            {
                Console.WriteLine("Invalid license plate. Please try again.");
                return;
            }
            string licensePlate = licensePlateInput.Trim().ToUpper();

            int oldIndex = FindVehicle(licensePlate);
            if (oldIndex == -1)
            {
                Console.WriteLine("Vehicle not found.");
                return;
            }

            Console.Write("Enter new spot (1-100): ");
            string? spotInput = Console.ReadLine();
            if (!int.TryParse(spotInput, out int newSpotNumber) || newSpotNumber < 1 || newSpotNumber > 100)
            {
                Console.WriteLine("Invalid spot. Enter a number between 1 and 100.");
                return;
            }

            int newIndex = newSpotNumber - 1;

            if (oldIndex == newIndex)
            {
                Console.WriteLine("Vehicle is already in the specified spot.");
                return;
            }

            string[] vehicles = parkingSpots[oldIndex]!.Split('|');
            string movingVehicle = "";
            string remaining = "";

            foreach (string vehicle in vehicles)
            {
                string trimmed = vehicle.Trim();
                string plate = trimmed.Split('#')[1];

                if (plate == licensePlate)
                {
                    movingVehicle = trimmed;
                }
                else
                {
                    if (remaining != "")
                    {
                        remaining += "|";
                    }
                    remaining += trimmed;
                }
            }
            string? target = parkingSpots[newIndex];
            bool canMove;

            if (string.IsNullOrEmpty(target))
            {
                canMove = true;
            }
            else if (movingVehicle.StartsWith("MC#") && target.StartsWith("MC#") && !target.Contains("|"))
            {
                canMove = true;
            }
            else
            {
                canMove = false;
            }

            if (!canMove)
            {
                Console.WriteLine($"Spot {newSpotNumber} cannot take this vehicle.");
                return;
            }

            if (string.IsNullOrEmpty(target))
            {
                parkingSpots[newIndex] = movingVehicle;
            }
            else
            {
                parkingSpots[newIndex] = target + "|" + movingVehicle;
            }

            if (remaining == "")
            {
                parkingSpots[oldIndex] = null;
            }
            else
            {
                parkingSpots[oldIndex] = remaining;
            }

            Console.WriteLine($"Vehicle {licensePlate} moved from spot {oldIndex + 1} to spot {newSpotNumber}.");
        }

        static void RemoveVehicle()
        {
            Console.Clear();
            Console.WriteLine("=== Remove Vehicle ===");
            Console.WriteLine();
            Console.Write("Enter license plate: ");
            string? licensePlateInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(licensePlateInput))
            {
                Console.WriteLine("Invalid license plate. Please try again.");
                return;
            }
            string licensePlate = licensePlateInput.Trim().ToUpper();

            int index = FindVehicle(licensePlate);

            if (index == -1)
            {
                Console.WriteLine("Vehicle not found.");
                return;
            }

            string spot = parkingSpots[index]!;
            string[] vehicles = spot.Split('|');
            string remaining = "";

            foreach (string vehicle in vehicles)
            {
                string plate = vehicle.Trim().Split('#')[1];

                if (plate != licensePlate)
                {
                    if (remaining != "")
                    {
                        remaining += "|";
                    }
                    remaining += vehicle.Trim();
                }
            }
            if (remaining == "")
            {
                parkingSpots[index] = null;
            }
            else
            {
                parkingSpots[index] = remaining;
            }

            Console.WriteLine($"Vehicle with license plate {licensePlate} removed from spot {index + 1}.");
        }

        static int FindVehicle(string licensePlate)
        {
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                string? spot = parkingSpots[i];
                if (string.IsNullOrEmpty(spot))
                {
                    continue;
                }
                string[] vehicles = spot.Split('|');
                foreach (string vehicle in vehicles)
                {
                    string plate = vehicle.Trim().Split('#')[1];
                    if (plate == licensePlate)
                    {
                        return i;
                    }

                }

            }

            return -1;

        }

        static int FindEmptySpot()
        {
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                if (string.IsNullOrEmpty(parkingSpots[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        static int FindSpotForMC()
        {
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                string? spot = parkingSpots[i];
                if (!string.IsNullOrEmpty(spot) && spot.StartsWith("MC#") && !spot.Contains("|"))
                {
                    return i;
                }

            }
            return FindEmptySpot();



        }
        static void SearchVehicle()
        {
            Console.Clear();
            Console.WriteLine("=== Search Vehicle ===");
            Console.WriteLine();
            Console.Write("Enter license plate: ");
            string? licensePlateInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(licensePlateInput))
            {
                Console.WriteLine("Invalid license plate. Please try again.");
                return;
            }
            string licensePlate = licensePlateInput.Trim().ToUpper();

            int index = FindVehicle(licensePlate);

            if (index == -1)
            {
                Console.WriteLine("Vehicle not found.");
            }
            else
            {
                Console.WriteLine($"Vehicle found at spot {index + 1}: {parkingSpots[index]}");
            }

        }
        static void ShowParkingSpots()
        {
            Console.Clear();
            Console.WriteLine("=== Parking Spots ===");
            Console.WriteLine();

            int spotsPerColumn = 20;
            int columns = parkingSpots.Length / spotsPerColumn;

            int cellWidth = 10; 
            for (int i = 0; i < parkingSpots.Length; i++)
            {
                int length = FormatSpot(i).Length;
                if (length > cellWidth)
                {
                    cellWidth = length;
                }
            }
            cellWidth += 2; 

            for (int row = 0; row < spotsPerColumn; row++) 
            {
                for (int column = 0; column < columns; column++)
                {
                    int index = column * spotsPerColumn + row;
                    Console.Write(FormatSpot(index).PadRight(cellWidth));
                }
                Console.WriteLine();
            }
        }

        static string FormatSpot(int index)
        {
            string? spot = parkingSpots[index];

            if (string.IsNullOrEmpty(spot))
            {
                return $"{index + 1,3}: -";
            }

            return $"{index + 1,3}: {spot}";
        }
    }
}
