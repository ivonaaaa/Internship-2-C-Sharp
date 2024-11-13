using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
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
            { "Prikaži sve korisnike", ShowUsers },
            { "Povratak", () => ShowMenu(MainMenu) }
        };

        AccountsMenu = new Dictionary<string, Action>
        {
            //popravit opcije
            { "Pregled računa", ShowAccounts },
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
        Console.WriteLine("Dodavanje korisnika...");
        Console.ReadKey();
    }

    static void DeleteUser()
    {
        Console.WriteLine("Brisanje korisnika...");
        Console.ReadKey();
    }

    static void ShowUsers()
    {
        Console.WriteLine("Prikaz svih korisnika...");
        Console.ReadKey();
    }

    static void ShowAccounts()
    {
        Console.WriteLine("Pregled svih računa...");
        Console.ReadKey();
    }

}//class Program