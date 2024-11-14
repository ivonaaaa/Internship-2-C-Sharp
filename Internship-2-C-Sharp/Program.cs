using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static List<Dictionary<string, object>> users = new List<Dictionary<string, object>>();

    //GLOBALNE varijable za izbornike
    static Dictionary<string, Action> MainMenu = new Dictionary<string, Action>();
    static Dictionary<string, Action> UsersMenu = new Dictionary<string, Action>();
    static Dictionary<string, Action> AccountsMenu = new Dictionary<string, Action>();

    static void Main()
    {
        //MAIN
        //napraviti dictionary za svaki "izbornik"
        MainMenu = new Dictionary<string, Action>
        {
            { "Korisnici", () => ShowMenu(UsersMenu) },
            { "Računi", () => ShowMenu(AccountsMenu) },
            { "Izlaz", () => Environment.Exit(0) }
        };

        UsersMenu = new Dictionary<string, Action>
        {
            { "Dodaj korisnika", AddUser },
            { "Izbriši korisnika", DeleteUser },
            { "Uredi korisnika", EditUser },
            { "Prikaži sve korisnike", ShowUsers },
            { "Povratak", () => ShowMenu(MainMenu) }
        };

        AccountsMenu = new Dictionary<string, Action>
        {
            //popravit opcije
            { "Povratak", () => ShowMenu(MainMenu) }
        };

        //pozivanje prikaza glavnog izbornika
        ShowMenu(MainMenu);

    }//Main


    //FUNKCIJE
    static void ShowMenu(Dictionary<string, Action> MainMenu)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Izbornik:");

            int i = 1;
            foreach (var option in MainMenu.Keys)
            {
                Console.WriteLine($"{i} - {option}");
                i++;
            }
            Console.WriteLine("\nOdaberite opciju: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= MainMenu.Count)
            {
                var action = MainMenu.Values.ElementAt(choice - 1);
                action.Invoke();
            }
            else
            {
                Console.WriteLine("Nevažeća opcija, pokušajte ponovno.");
                Console.ReadKey();
            }
        }
    }

    static void AddUser()
    {
        Console.WriteLine("Unesite ID korisnika: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Neispravan ID. Pokušajte ponovno.");
            Console.ReadKey();
            return;
        }
        if (users.Any(u => (int)u["Id"] == id))
        {
            Console.WriteLine("Korisnik s tim ID-om već postoji!");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Unesite ime korisnika: ");
        string name = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Unesite prezime korisnika: ");
        string surname = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Unesite datum rođenja korisnika (dd/mm/yyyy): ");
        DateTime dateOfBirth;
        if (!DateTime.TryParse(Console.ReadLine(), out dateOfBirth))
        {
            Console.WriteLine("Neispravan datum. Pokušajte ponovno.");
            Console.ReadKey();
            return;
        }

        var newUser = new Dictionary<string, object>
        {
            { "Id", id },
            { "Name", name },
            { "Surname", surname },
            { "DateOfBirth", dateOfBirth },
            { "Accounts", new List<Dictionary<string, object>>() }
        };
        users.Add(newUser);

        Console.WriteLine("Korisnik uspješno dodan!");
        Console.ReadKey();
    }

    static void DeleteUser()
    {
        Console.WriteLine("Odaberite način brisanja korisnika: ");
        Console.WriteLine("a) Po ID-u");
        Console.WriteLine("b) Po imenu i prezimenu");
        string choice = Console.ReadLine() ?? string.Empty;

        if (choice == "a" || choice == "A")
        {
            Console.WriteLine("Unesite ID korisnika kojeg želite izbrisati: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Neispravan ID.");
                Console.ReadKey();
                return;
            }
            var userToDelete = users.FirstOrDefault(u => (int)u["Id"] == id);
            if (userToDelete != null)
            {
                users.Remove(userToDelete);
                Console.WriteLine("Korisnik uspješno obrisan!");
            }
            else
            {
                Console.WriteLine("Korisnik s tim ID-om nije pronađen!");
            }
        }
        else if (choice == "b" || choice == "B")
        {
            Console.WriteLine("Unesite ime korisnika za brisanje:");
            string name = Console.ReadLine() ?? string.Empty;
            Console.WriteLine("Unesite prezime korisnika za brisanje:");
            string surname = Console.ReadLine() ?? string.Empty;

            var userToDelete = users.FirstOrDefault(u => (string)u["Name"] == name && (string)u["Surname"] == surname);
            if (userToDelete != null)
            {
                users.Remove(userToDelete);
                Console.WriteLine("Korisnik uspješno obrisan!");
            }
            else
            {
                Console.WriteLine("Korisnik s tim imenom i prezimenom nije pronađen!");
            }
        }

        Console.ReadKey();
    }

    static void EditUser()
    {
        Console.WriteLine("Unesite ID korisnika kojeg želite urediti:");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Neispravan ID.");
            Console.ReadKey();
            return;
        }
        var userToEdit = users.FirstOrDefault(u => (int)u["Id"] == id);
        if (userToEdit != null)
        {
            Console.WriteLine($"Unesite novo ime za korisnika {userToEdit["Name"]}: ");
            userToEdit["Name"] = Console.ReadLine() ?? string.Empty;

            Console.WriteLine($"Unesite novo prezime za korisnika {userToEdit["Surname"]}: ");
            userToEdit["Surname"] = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Korisnik uspješno uređen!");
        }
        else
        {
            Console.WriteLine("Korisnik s tim ID-om nije pronađen!");
        }

        Console.ReadKey();
    }

    static void ShowUsers()
    {
        Console.WriteLine("Odaberite opciju pregleda korisnika: ");
        Console.WriteLine("a) Svi korisnici abecedno po prezimenu");
        Console.WriteLine("b) Svi korisnici stariji od 30 godina");
        Console.WriteLine("c) Svi korisnici koji imaju barem jedan račun u minusu");
        string choice = Console.ReadLine() ?? string.Empty;

        if (choice == "a" || choice == "A")
        {
            var sortedUsers = users.OrderBy(u => (string)u["Surname"]).ToList();
            foreach (var user in sortedUsers)
            {
                Console.WriteLine($"{user["Id"]} - {user["Name"]} - {user["Surname"]} - {user["DateOfBirth"]}");
            }
        }
        else if (choice == "b" || choice == "B")
        {
            foreach (var u in users)
            {
                int age = DateTime.Now.Year - ((DateTime)u["DateOfBirth"]).Year;
                if (age > 30)
                {
                    Console.WriteLine($"{u["Id"]} - {u["Name"]} {u["Surname"]}, {age} years old");
                }
            }
        }
        else if (choice == "c" || choice == "C")
        {
            var usersWithNegativeBalance = users.Where(u => ((List<Dictionary<string, object>>)u["Accounts"]).Any(a => (decimal)a["Balance"] < 0)).ToList();

            foreach (var user in usersWithNegativeBalance)
            {
                Console.WriteLine($"{user["Id"]} - {user["Name"]} {user["Surname"]}");

                var accounts = (List<Dictionary<string, object>>)user["Accounts"];
                foreach (var account in accounts)
                {
                    Console.WriteLine($"  Account Number: {account["AccountNumber"]}, Balance: {account["Balance"]}");
                }
            }
            Console.ReadKey();
        }

        Console.ReadKey();
    }

}//class Program