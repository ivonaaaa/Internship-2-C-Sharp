using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static List<User> users = new List<User>(); //instanca klase korisnici

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
        if (users.Any(u => u.Id == id))
        {
            Console.WriteLine("Korisnik s tim ID-om već postoji!");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Unesite ime korisnika: ");
        string name = Console.ReadLine();

        Console.WriteLine("Unesite prezime korisnika: ");
        string surname = Console.ReadLine();

        Console.WriteLine("Unesite datum rođenja korisnika (dd/mm/yyyy): ");
        DateTime dateOfBirth;
        if (!DateTime.TryParse(Console.ReadLine(), out dateOfBirth))
        {
            Console.WriteLine("Neispravan datum. Pokušajte ponovno.");
            Console.ReadKey();
            return;
        }

        var newUser = new User(id, name, surname, dateOfBirth);
        users.Add(newUser);

        Console.WriteLine("Korisnik uspješno dodan!");
        Console.ReadKey();
    }

    static void DeleteUser()
    {
        Console.WriteLine("Odaberite način brisanja korisnika: ");
        Console.WriteLine("a) Po ID-u");
        Console.WriteLine("b) Po imenu i prezimenu");
        string choice = Console.ReadLine();

        if (choice == "a" || choice == "A")
        {
            Console.WriteLine("Unesite ID korisnika kojeg želite izbrisati: ");
            int id = int.Parse(Console.ReadLine());
            var userToDelete = users.FirstOrDefault(u => u.Id == id);
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
            string name = Console.ReadLine();
            Console.WriteLine("Unesite prezime korisnika za brisanje:");
            string surname = Console.ReadLine();

            var userToDelete = users.FirstOrDefault(u => u.Name == name && u.Surname == surname);
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
        Console.WriteLine("Odaberite način uređivanja korisnika: ");
        Console.WriteLine("a) Po ID-u");
        string choice = Console.ReadLine();

        if (choice == "a" || choice == "A")
        {
            Console.WriteLine("Unesite ID korisnika kojeg želite urediti:");
            int id = int.Parse(Console.ReadLine());
            var userToEdit = users.FirstOrDefault(u => u.Id == id);
            if (userToEdit != null)
            {
                Console.WriteLine($"Unesite novo ime za korisnika {userToEdit.Name}: ");
                userToEdit.Name = Console.ReadLine();

                Console.WriteLine($"Unesite novo prezime za korisnika {userToEdit.Surname}: ");
                userToEdit.Surname = Console.ReadLine();

                Console.WriteLine("Korisnik uspješno uređen!");
            }
            else
            {
                Console.WriteLine("Korisnik s tim ID-om nije pronađen!");
            }
        }

        Console.ReadKey();
    }

    static void ShowUsers()
    {
        Console.WriteLine("Odaberite opciju pregleda korisnika: ");
        Console.WriteLine("a) Svi korisnici abecedno po prezimenu");
        Console.WriteLine("b) Svi korisnici stariji od 30 godina");
        Console.WriteLine("c) Svi korisnici koji imaju barem jedan račun u minusu");
        string choice = Console.ReadLine();

        if (choice == "a" || choice == "A")
        {
            var sortedUsers = users.OrderBy(u => u.Surname).ToList();
            foreach (var user in sortedUsers)
            {
                Console.WriteLine($"{user.Id} - {user.Name} - {user.Surname} - {user.DateOfBirth}");
            }
        }
        else if (choice == "b" || choice == "B")
        {
            var olderThan30 = users.Where(u => u.Age > 30).ToList();
            foreach (var user in olderThan30)
            {
                Console.WriteLine($"{user.Id} - {user.Name} {user.Surname}, {user.Age} godina");
            }
        }
        else if (choice == "c" || choice == "C")
        {
            var usersWithNegativeBalance = users.Where(u => u.Accounts.Any(a => a.Balance < 0)).ToList();
            foreach (var user in usersWithNegativeBalance)
            {
                Console.WriteLine($"{user.Id} - {user.Name} {user.Surname}"); //ode prikazat stanje na racunu jos?
            }
        }

        Console.ReadKey();
    }

}//class Program


//OSTALE KLASE
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime DateOfBirth { get; set; }
    public List<Account> Accounts { get; set; } = new List<Account>();

    public int Age => DateTime.Now.Year - DateOfBirth.Year;

    //konstruktor
    public User(int id, string name, string surname, DateTime dateOfBirth)
    {
        Id = id;
        Name = name;
        Surname = surname;
        DateOfBirth = dateOfBirth;
    }

}//User

public class Account
{
    public int AccountNumber { get; set; }
    public decimal Balance { get; set; }

    //konstruktor
    public Account(int accountNumber, decimal balance)
    {
        AccountNumber = accountNumber;
        Balance = balance;
    }

}//Account