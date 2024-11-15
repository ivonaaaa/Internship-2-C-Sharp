using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Globalization;

class Program
{
    //lista svih korisnika
    static List<Dictionary<string, object>> users = new List<Dictionary<string, object>>();

    //globalne varijable za izbornike
    static Dictionary<string, Action> MainMenu = new Dictionary<string, Action>();
    static Dictionary<string, Action> UsersMenu = new Dictionary<string, Action>();
    static Dictionary<string, Action> AccountsMenu = new Dictionary<string, Action>();

    //GLAVNI PROGRAM MAIN
    static void Main()
    {
        InitializeData();

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
            { "Odaberi korisnika", SelectUser },
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

    }//ShowMenu

    static void SelectUser()
    {
        Console.WriteLine("Trenutni korisnici:");
        foreach (var existingUsers in users)
        {
            Console.WriteLine($"{existingUsers["Name"]} {existingUsers["Surname"]} (ID: {existingUsers["Id"]})");
        }

        Console.WriteLine("\nUnesite ime korisnika: ");
        string name = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("Unesite prezime korisnika: ");
        string surname = Console.ReadLine() ?? string.Empty;

        var user = users.FirstOrDefault(u => (string)u["Name"] == name && (string)u["Surname"] == surname);
        if (user == null)
        {
            Console.WriteLine("Korisnik nije pronađen!");
            Console.ReadKey();
            return;
        }

        var accounts = user["Accounts"] as List<Dictionary<string, object>>;
        if (accounts == null)
        {
            Console.WriteLine("Greška pri dohvaćanju računa korisnika.");
            return;
        }

        Console.WriteLine($"Računi za korisnika {user["Name"]} {user["Surname"]}:");
        int i = 1;

        foreach (var account in accounts)
        {
            Console.WriteLine($"{i}. {account["AccountType"]}: {account["Balance"]} EUR");
            i++;
        }

        Console.WriteLine("Odaberite račun za upravljanje:");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= accounts.Count)
        {
            var selectedAccount = accounts[choice - 1];
            var accountOptionsMenu = new Dictionary<string, Action>
            {
                { "Unos nove transakcije", () => AddTransaction(user, selectedAccount) },
                { "Brisanje transakcije", () => DeleteTransaction(user, selectedAccount) },
                { "Uređivanje transakcije", () => EditTransaction(user, selectedAccount) },
                { "Pregled transakcija", () => ShowTransactions(user, selectedAccount) },
                { "Financijsko izvješće", () => FinancialReport(user, selectedAccount) },
                { "Povratak", () => ShowMenu(AccountsMenu) }
            };

            ShowMenu(accountOptionsMenu);

        }
        else
        {
            Console.WriteLine("Neispravan odabir računa.");
            Console.ReadKey();
        }

    }//SelectUser

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

        var accounts = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object> { { "AccountType", "tekući" }, { "Balance", 100.00m }, { "Transactions", new List<Dictionary<string, object>>() } },
            new Dictionary<string, object> { { "AccountType", "žiro" }, { "Balance", 0.00m }, { "Transactions", new List<Dictionary<string, object>>() } },
            new Dictionary<string, object> { { "AccountType", "prepaid" }, { "Balance", 0.00m }, { "Transactions", new List<Dictionary<string, object>>() } }
        };

        var newUser = new Dictionary<string, object>
        {
            { "Id", id },
            { "Name", name },
            { "Surname", surname },
            { "DateOfBirth", dateOfBirth },
            { "Accounts", accounts }
        };
        users.Add(newUser);

        Console.WriteLine("Korisnik uspješno dodan!");
        Console.ReadKey();

    }//AddUser

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

    }//DeleteUser

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

            Console.WriteLine($"Unesite novi datum rođenja korisnika {userToEdit["Name"]} (dd/mm/yyyy): ");
            DateTime dateOfBirth;
            if (!DateTime.TryParse(Console.ReadLine(), out dateOfBirth))
            {
                Console.WriteLine("Neispravan datum. Pokušajte ponovno.");
                Console.ReadKey();
                return;
            }
            userToEdit["DateOfBirth"] = dateOfBirth;

            Console.WriteLine("Korisnik uspješno uređen!");
        }
        else
        {
            Console.WriteLine("Korisnik s tim ID-om nije pronađen!");
        }
        Console.ReadKey();

    }//EditUser

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
            var usersWithNegativeBalance = users.Where(u => ((List<Dictionary<string, object>>)u["Accounts"]).Any(a => (double)a["Balance"] < 0)).ToList();

            foreach (var user in usersWithNegativeBalance)
            {
                Console.WriteLine($"{user["Id"]} - {user["Name"]} {user["Surname"]}");

                var accounts = (List<Dictionary<string, object>>)user["Accounts"];
                foreach (var account in accounts)
                {
                    Console.WriteLine($"Stanje na računu: {account["Balance"]}");
                }
            }
            Console.ReadKey();
        }
        Console.ReadKey();

    }//ShowUsers

    static void AddTransaction(Dictionary<string, object> user, Dictionary<string, object> selectedAccount)
    {
        Console.WriteLine("Odaberite tip transakcije:");
        Console.WriteLine("a) Trenutna transakcija (automatski unos trenutnog datuma i vremena)");
        Console.WriteLine("b) Ranije izvršena transakcija (unos datuma i vremena)");
        string transactionTypeChoice = Console.ReadLine()?.ToLower() ?? string.Empty;

        DateTime transactionDateTime;
        if (transactionTypeChoice == "a")
        {
            transactionDateTime = DateTime.Now;
        }
        else if (transactionTypeChoice == "b")
        {
            Console.WriteLine("Unesite datum i vrijeme transakcije (dd/mm/yyyy hh:mm):");
            if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out transactionDateTime))
            {
                Console.WriteLine("Neispravan datum i vrijeme. Pokušajte ponovno.");
                Console.ReadKey();
                return;
            }
        }
        else
        {
            Console.WriteLine("Nevažeći odabir. Pokušajte ponovno.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Unesite ID transakcije:");
        if (!int.TryParse(Console.ReadLine(), out int transactionId))
        {
            Console.WriteLine("Neispravan ID. Pokušajte ponovno.");
            Console.ReadKey();
            return;
        }

        var transactions = selectedAccount.ContainsKey("Transactions")
            ? selectedAccount["Transactions"] as List<Dictionary<string, object>>
            : new List<Dictionary<string, object>>();

        // Ensure that "Transactions" is not null and contains valid dictionaries
        if (transactions != null && transactions.Any(t => t.ContainsKey("Id") && (int)t["Id"] == transactionId))
        {
            Console.WriteLine("Transakcija s tim ID-om već postoji. Pokušajte s drugim ID-em.");
            Console.ReadKey();
            return;
        }


        Console.WriteLine("Unesite iznos transakcije (pozitivno za prihod, negativno za rashod):");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Nevažeći iznos. Pokušajte ponovno.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Unesite opis transakcije (po defaultu 'standardna transakcija' ako samo pritisnete Enter):");
        string description = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(description))
        {
            description = "standardna transakcija";
        }

        Console.WriteLine("Unesite tip transakcije (prihod/rashod):");
        string type = Console.ReadLine()?.ToLower(CultureInfo.InvariantCulture) ?? string.Empty;
        if (type != "prihod" && type != "rashod")
        {
            Console.WriteLine("Neispravan tip transakcije. Pokušajte ponovno.");
            Console.ReadKey();
            return;
        }

        List<string> categories = new List<string>();
        if (type == "prihod")
        {
            categories = new List<string> { "Plaća", "Stipendija", "Poklon", "Bonus", "Uplata" };
        }
        else if (type == "rashod")
        {
            categories = new List<string> { "Hrana", "Struja", "Voda", "Članarina", "Telefon" };
        }

        Console.WriteLine("Odaberite kategoriju transakcije:");
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i]}");
        }

        string categoryChoice = Console.ReadLine() ?? string.Empty;
        if (!int.TryParse(categoryChoice, out int categoryIndex) || categoryIndex < 1 || categoryIndex > categories.Count)
        {
            Console.WriteLine("Neispravan odabir kategorije. Pokušajte ponovno.");
            Console.ReadKey();
            return;
        }

        string selectedCategory = categories[categoryIndex - 1];

        var transaction = new Dictionary<string, object>
        {
            { "Id", transactionId },
            { "Amount", amount },
            { "Description", description },
            { "Type", type },
            { "Category", selectedCategory },
            { "Date", transactionDateTime }
        };

        transactions.Add(transaction);
        selectedAccount["Transactions"] = transactions;

        Console.WriteLine("Transakcija uspješno dodana!");
        Console.ReadKey();

    }//AddTransaction


    static void DeleteTransaction(Dictionary<string, object> user, Dictionary<string, object> selectedAccount)
    {
        var transactions = (List<Dictionary<string, object>>)selectedAccount["Transactions"];

        Console.WriteLine("Odaberite način brisanja transakcije: ");
        Console.WriteLine("a) Po ID-u");
        Console.WriteLine("b) Ispod unesenog iznosa");
        Console.WriteLine("c) Iznad unesenog iznosa");
        Console.WriteLine("d) Svi prihodi");
        Console.WriteLine("e) Svi rashodi");
        Console.WriteLine("f) Po kategoriji");
        string choice = Console.ReadLine()?.ToLower() ?? string.Empty;

        if (choice == "a")
        {
            Console.WriteLine("Unesite ID transakcije koju želite izbrisati: ");
            if (!int.TryParse(Console.ReadLine(), out int transactionId))
            {
                Console.WriteLine("Neispravan ID. Pokušajte ponovno.");
                Console.ReadKey();
                return;
            }

            var transactionToDelete = transactions.FirstOrDefault(t => (int)t["Id"] == transactionId);
            if (transactionToDelete != null)
            {
                transactions.Remove(transactionToDelete);
                Console.WriteLine("Transakcija uspješno obrisana!");
            }
            else
            {
                Console.WriteLine("Transakcija s tim ID-om nije pronađena.");
            }
        }
        else if (choice == "b")
        {
            Console.WriteLine("Unesite iznos ispod kojeg želite obrisati transakcije: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
            {
                Console.WriteLine("Neispravan iznos. Pokušajte ponovno.");
                Console.ReadKey();
                return;
            }

            var transactionsToDelete = transactions.Where(t => (decimal)t["Amount"] < amount).ToList();
            if (transactionsToDelete.Any())
            {
                foreach (var transaction in transactionsToDelete)
                {
                    transactions.Remove(transaction);
                }
                Console.WriteLine($"Obrisane su sve transakcije ispod {amount} EUR.");
            }
            else
            {
                Console.WriteLine("Nema transakcija ispod tog iznosa.");
            }
        }
        else if (choice == "c")
        {
            Console.WriteLine("Unesite iznos iznad kojeg želite obrisati transakcije: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal upperAmount))
            {
                Console.WriteLine("Neispravan iznos. Pokušajte ponovno.");
                Console.ReadKey();
                return;
            }

            var transactionsToDelete = transactions.Where(t => (decimal)t["Amount"] > upperAmount).ToList();
            if (transactionsToDelete.Any())
            {
                foreach (var transaction in transactionsToDelete)
                {
                    transactions.Remove(transaction);
                }
                Console.WriteLine($"Obrisane su sve transakcije iznad {upperAmount} EUR.");
            }
            else
            {
                Console.WriteLine("Nema transakcija iznad tog iznosa.");
            }
        }
        else if (choice == "d")
        {
            var transactionsToDelete = transactions.Where(t => (string)t["Type"] == "prihod").ToList();
            if (transactionsToDelete.Any())
            {
                foreach (var transaction in transactionsToDelete)
                {
                    transactions.Remove(transaction);
                }
                Console.WriteLine("Obrisane su sve transakcije prihoda.");
            }
            else
            {
                Console.WriteLine("Nema transakcija prihoda.");
            }
        }
        else if (choice == "e")
        {
            var transactionsToDelete = transactions.Where(t => (string)t["Type"] == "rashod").ToList();
            if (transactionsToDelete.Any())
            {
                foreach (var transaction in transactionsToDelete)
                {
                    transactions.Remove(transaction);
                }
                Console.WriteLine("Obrisane su sve transakcije rashoda.");
            }
            else
            {
                Console.WriteLine("Nema transakcija rashoda.");
            }
        }
        else if (choice == "f")
        {
            Console.WriteLine("Unesite kategoriju transakcija koje želite obrisati: ");
            string category = Console.ReadLine()?.ToLower() ?? string.Empty;

            var transactionsToDelete = transactions
                .Where(t => ((string)t["Category"]).ToLower(CultureInfo.InvariantCulture) == category.ToLower(CultureInfo.InvariantCulture))
                .ToList(); if (transactionsToDelete.Any())
            {
                foreach (var transaction in transactionsToDelete)
                {
                    transactions.Remove(transaction);
                }
                Console.WriteLine($"Obrisane su sve transakcije za kategoriju {category}.");
            }
            else
            {
                Console.WriteLine("Nema transakcija za tu kategoriju.");
            }
        }
        else
        {
            Console.WriteLine("Nevažeći odabir. Pokušajte ponovno.");
            Console.ReadKey();
        }

    }//DeleteTransaction

    static void EditTransaction(Dictionary<string, object> user, Dictionary<string, object> selectedAccount)
    {
        var transactions = selectedAccount.ContainsKey("Transactions") ? selectedAccount["Transactions"] as List<Dictionary<string, object>> : new List<Dictionary<string, object>>();

        int transactionId = -1;
        while (transactionId < 0)
        {
            Console.WriteLine("Unesite ID transakcije koju želite urediti:");
            if (!int.TryParse(Console.ReadLine(), out transactionId))
            {
                Console.WriteLine("Neispravan unos ID-a. Pokušajte ponovno.");
                continue;
            }

            var transactionToEdit = transactions.FirstOrDefault(t => (int)t["Id"] == transactionId);
            if (transactionToEdit == null)
            {
                Console.WriteLine("Transakcija s tim ID-em ne postoji. Pokušajte ponovno.");
                transactionId = -1;
            }
        }

        var transaction = transactions.First(t => (int)t["Id"] == transactionId);

        Console.WriteLine($"Trenutni iznos transakcije: {transaction["Amount"]} EUR");
        Console.WriteLine("Unesite novi iznos transakcije (pozitivno za prihod, negativno za rashod):");
        if (decimal.TryParse(Console.ReadLine(), out decimal newAmount))
        {
            transaction["Amount"] = newAmount;
        }
        else
        {
            Console.WriteLine("Neispravan iznos. Zadržava se prethodni.");
        }

        Console.WriteLine($"Trenutni opis transakcije: {transaction["Description"]}");
        Console.WriteLine("Unesite novi opis transakcije (po defaultu 'standardna transakcija' ako samo pritisnete Enter):");
        string newDescription = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newDescription))
        {
            newDescription = "standardna transakcija";
        }
        transaction["Description"] = newDescription;

        Console.WriteLine($"Trenutni tip transakcije: {transaction["Type"]}");
        Console.WriteLine("Unesite novi tip transakcije (prihod/rashod):");
        string newType = Console.ReadLine()?.ToLower(CultureInfo.InvariantCulture) ?? string.Empty;
        if (newType == "prihod" || newType == "rashod")
        {
            transaction["Type"] = newType;
        }
        else
        {
            Console.WriteLine("Neispravan tip. Zadržava se prethodni.");
        }

        List<string> categories = new List<string>();
        if (newType == "prihod")
        {
            categories = new List<string> { "Plaća", "Stipendija", "Poklon", "Bonus", "Udio u dobiti" };
        }
        else if (newType == "rashod")
        {
            categories = new List<string> { "Hrana", "Struja", "Voda", "Članarina", "Telefon" };
        }

        Console.WriteLine($"Trenutna kategorija transakcije: {transaction["Category"]}");
        Console.WriteLine("Odaberite novu kategoriju transakcije:");
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i]}");
        }
        string categoryChoice = Console.ReadLine();
        if (int.TryParse(categoryChoice, out int categoryIndex) && categoryIndex >= 1 && categoryIndex <= categories.Count)
        {
            transaction["Category"] = categories[categoryIndex - 1];
        }
        else
        {
            Console.WriteLine("Neispravan odabir kategorije. Zadržava se prethodna.");
        }

        Console.WriteLine($"Trenutni datum i vrijeme transakcije: {transaction["Date"]}");
        Console.WriteLine("Unesite novi datum i vrijeme transakcije (dd/mm/yyyy hh:mm):");
        if (DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDateTime))
        {
            transaction["Date"] = newDateTime;
        }
        else
        {
            Console.WriteLine("Neispravan datum i vrijeme. Zadržava se prethodni.");
        }

        Console.WriteLine("Transakcija uspješno uređena!");
        Console.ReadKey();

    }//EditTransaction

    static void ShowTransactions(Dictionary<string, object> user, Dictionary<string, object> selectedAccount)
    {
        var transactions = selectedAccount.ContainsKey("Transactions") ? selectedAccount["Transactions"] as List<Dictionary<string, object>> : new List<Dictionary<string, object>>();

        if (transactions.Count == 0)
        {
            Console.WriteLine("Nema pohranjenih transakcija!");
            return;
        }

        Console.WriteLine("Odaberite opciju za pregled transakcija:");
        Console.WriteLine("a) Sve transakcije");
        Console.WriteLine("b) Sortirane transakcije po iznosu (uzlazno)");
        Console.WriteLine("c) Sortirane transakcije po iznosu (silazno)");
        Console.WriteLine("d) Sortirane transakcije po opisu (abecedno)");
        Console.WriteLine("e) Sortirane transakcije po datumu (uzlazno)");
        Console.WriteLine("f) Sortirane transakcije po datumu (silazno)");
        Console.WriteLine("g) Svi prihodi");
        Console.WriteLine("h) Svi rashodi");
        Console.WriteLine("i) Transakcije za odabranu kategoriju");
        Console.WriteLine("j) Transakcije za odabrani tip i kategoriju");

        string choice = Console.ReadLine()?.ToLower();

        switch (choice)
        {
            case "a":
                DisplayTransactions(transactions);
                break;
            case "b":
                var sortedByAmountAsc = transactions.OrderBy(t => t["Amount"]).ToList();
                DisplayTransactions(sortedByAmountAsc);
                break;
            case "c":
                var sortedByAmountDesc = transactions.OrderByDescending(t => t["Amount"]).ToList();
                DisplayTransactions(sortedByAmountDesc);
                break;
            case "d":
                var sortedByDescription = transactions.OrderBy(t => t["Description"].ToString()).ToList();
                DisplayTransactions(sortedByDescription);
                break;
            case "e":
                var sortedByDateAsc = transactions.OrderBy(t => t["Date"]).ToList();
                DisplayTransactions(sortedByDateAsc);
                break;
            case "f":
                var sortedByDateDesc = transactions.OrderByDescending(t => t["Date"]).ToList();
                DisplayTransactions(sortedByDateDesc);
                break;
            case "g":
                var incomeTransactions = transactions.Where(t => t["Type"].ToString() == "prihod").ToList();
                DisplayTransactions(incomeTransactions);
                break;
            case "h":
                var expenseTransactions = transactions.Where(t => t["Type"].ToString() == "rashod").ToList();
                DisplayTransactions(expenseTransactions);
                break;
            case "i":
                Console.WriteLine("Unesite naziv kategorije:");
                string category = Console.ReadLine();
                var transactionsByCategory = transactions.Where(t => t["Category"].ToString().Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
                DisplayTransactions(transactionsByCategory);
                break;
            case "j":
                Console.WriteLine("Unesite tip transakcije (prihod/rashod):");
                string type = Console.ReadLine()?.ToLower();
                if (type != "prihod" && type != "rashod")
                {
                    Console.WriteLine("Neispravan tip. Pokušajte ponovno.");
                    break;
                }

                Console.WriteLine("Unesite naziv kategorije:");
                string selectedCategory = Console.ReadLine();
                var transactionsByTypeAndCategory = transactions.Where(t =>
                    t["Type"].ToString() == type && t["Category"].ToString().Equals(selectedCategory, StringComparison.OrdinalIgnoreCase)).ToList();
                DisplayTransactions(transactionsByTypeAndCategory);
                break;
            default:
                Console.WriteLine("Neispravan odabir. Pokušajte ponovno.");
                break;
        }
    }

    static void DisplayTransactions(List<Dictionary<string, object>> transactions)
    {
        if (transactions.Count == 0)
        {
            Console.WriteLine("Nema transakcija koje odgovaraju odabranom kriteriju.");
            return;
        }

        foreach (var transaction in transactions)
        {
            Console.WriteLine($"{transaction["Type"]} - {transaction["Amount"]} EUR - {transaction["Description"]} - {transaction["Category"]} - {((DateTime)transaction["Date"]).ToString("dd/MM/yyyy HH:mm")}");
        }
        Console.WriteLine();

    }//ShowTransactions

    static void FinancialReport(Dictionary<string, object> user, Dictionary<string, object> selectedAccount)
    {
        var transactions = selectedAccount.ContainsKey("Transactions") ? selectedAccount["Transactions"] as List<Dictionary<string, object>> : new List<Dictionary<string, object>>();

        Console.WriteLine("Odaberite opciju za financijsko izvješće:");
        Console.WriteLine("a) Trenutno stanje računa");
        Console.WriteLine("b) Broj ukupnih transakcija");
        Console.WriteLine("c) Ukupan iznos prihoda i rashoda za odabrani mjesec i godinu");
        Console.WriteLine("d) Postotak udjela rashoda za odabranu kategoriju");
        Console.WriteLine("e) Prosječni iznos transakcije za odabrani mjesec i godinu");
        Console.WriteLine("f) Prosječni iznos transakcije za odabranu kategoriju");
        string choice = Console.ReadLine()?.ToLower();

        switch (choice)
        {
            case "a":
                ShowAccountBalance(transactions);
                break;
            case "b":
                ShowTotalTransactions(transactions);
                break;
            case "c":
                ShowIncomeAndExpenseForMonthYear(transactions);
                break;
            case "d":
                ShowExpenseCategoryPercentage(transactions);
                break;
            case "e":
                ShowAverageTransactionForMonthYear(transactions);
                break;
            case "f":
                ShowAverageTransactionForCategory(transactions);
                break;
            default:
                Console.WriteLine("Neispravan odabir. Pokušajte ponovno.");
                break;
        }

    }//FinancialReport

    // a) Trenutno stanje računa
    static void ShowAccountBalance(List<Dictionary<string, object>> transactions)
    {
        decimal totalIncome = transactions.Where(t => (string)t["Type"] == "prihod").Sum(t => (decimal)t["Amount"]);
        decimal totalExpense = transactions.Where(t => (string)t["Type"] == "rashod").Sum(t => (decimal)t["Amount"]);
        decimal balance = totalIncome - totalExpense;

        Console.WriteLine($"Trenutno stanje računa: {balance} EUR");
        Console.WriteLine($"Ukupni prihodi: {totalIncome} EUR");
        Console.WriteLine($"Ukupni rashodi: {totalExpense} EUR");

        if (balance < 0)
        {
            Console.WriteLine("Upozorenje: Račun je u minusu!");
        }
    }

    // b) Broj ukupnih transakcija
    static void ShowTotalTransactions(List<Dictionary<string, object>> transactions)
    {
        int totalTransactions = transactions.Count;
        Console.WriteLine($"Ukupan broj transakcija: {totalTransactions}");
    }

    // c) Ukupan iznos prihoda i rashoda za odabrani mjesec i godinu
    static void ShowIncomeAndExpenseForMonthYear(List<Dictionary<string, object>> transactions)
    {
        Console.WriteLine("Unesite mjesec (1-12):");
        int month = int.Parse(Console.ReadLine() ?? "1");

        Console.WriteLine("Unesite godinu (yyyy):");
        int year = int.Parse(Console.ReadLine() ?? "2024");

        decimal totalIncome = transactions.Where(t =>
            (string)t["Type"] == "prihod" && ((DateTime)t["Date"]).Month == month && ((DateTime)t["Date"]).Year == year)
            .Sum(t => (decimal)t["Amount"]);
        decimal totalExpense = transactions.Where(t =>
            (string)t["Type"] == "rashod" && ((DateTime)t["Date"]).Month == month && ((DateTime)t["Date"]).Year == year)
            .Sum(t => (decimal)t["Amount"]);

        Console.WriteLine($"Ukupni prihodi za {month}/{year}: {totalIncome} EUR");
        Console.WriteLine($"Ukupni rashodi za {month}/{year}: {totalExpense} EUR");
    }

    // d) Postotak udjela rashoda za odabranu kategoriju
    static void ShowExpenseCategoryPercentage(List<Dictionary<string, object>> transactions)
    {
        Console.WriteLine("Unesite kategoriju rashoda:");
        string category = Console.ReadLine()?.ToLower() ?? string.Empty;

        decimal totalExpenses = transactions.Where(t => (string)t["Type"] == "rashod").Sum(t => (decimal)t["Amount"]);
        decimal categoryExpenses = transactions.Where(t =>
            (string)t["Type"] == "rashod" && ((string)t["Category"]).ToLower() == category)
            .Sum(t => (decimal)t["Amount"]);

        if (totalExpenses == 0)
        {
            Console.WriteLine("Nema rashoda za izračun.");
            return;
        }

        decimal percentage = (categoryExpenses / totalExpenses) * 100;
        Console.WriteLine($"Postotak rashoda za kategoriju '{category}': {percentage:F2}%");
    }

    // e) Prosječni iznos transakcije za odabrani mjesec i godinu
    static void ShowAverageTransactionForMonthYear(List<Dictionary<string, object>> transactions)
    {
        Console.WriteLine("Unesite mjesec (1-12):");
        int month = int.Parse(Console.ReadLine() ?? "1");

        Console.WriteLine("Unesite godinu (yyyy):");
        int year = int.Parse(Console.ReadLine() ?? "2024");

        var transactionsForMonthYear = transactions.Where(t =>
            ((DateTime)t["Date"]).Month == month && ((DateTime)t["Date"]).Year == year).ToList();

        if (!transactionsForMonthYear.Any())
        {
            Console.WriteLine("Nema transakcija za odabrani mjesec i godinu.");
            return;
        }

        decimal average = transactionsForMonthYear.Average(t => (decimal)t["Amount"]);
        Console.WriteLine($"Prosječni iznos transakcije za {month}/{year}: {average:F2} EUR");
    }

    // f) Prosječni iznos transakcije za odabranu kategoriju
    static void ShowAverageTransactionForCategory(List<Dictionary<string, object>> transactions)
    {
        Console.WriteLine("Unesite kategoriju:");
        string category = Console.ReadLine()?.ToLower() ?? string.Empty;

        var transactionsForCategory = transactions.Where(t =>
            ((string)t["Category"]).ToLower() == category).ToList();

        if (!transactionsForCategory.Any())
        {
            Console.WriteLine("Nema transakcija za odabranu kategoriju.");
            return;
        }

        decimal average = transactionsForCategory.Average(t => (decimal)t["Amount"]);
        Console.WriteLine($"Prosječni iznos transakcije za kategoriju '{category}': {average:F2} EUR");

    }

    //INICIJALNI PODACI
    static void InitializeData()
    {
        //user 1
        var user1 = new Dictionary<string, object>
        {
            {"Id", 18},
            {"Name", "Ivona"},
            {"Surname", "Ercegovac"},
            {"DateOfBirth", new DateTime(2001, 12, 7)},
            {"Accounts", new List<Dictionary<string, object>>()}
        };

        var account1 = new Dictionary<string, object>
        {
            {"AccountType", "tekući"},
            {"Balance", 5000.75},
            {"Transactions", new List<Dictionary<string, object>>()}
        };

        var account2 = new Dictionary<string, object>
        {
            {"AccountType", "žiro"},
            {"Balance", 30.00},
            {"Transactions", new List<Dictionary<string, object>>()}
        };

        var account3 = new Dictionary<string, object>
        {
            {"AccountType", "prepaid"},
            {"Balance", 70.00},
            {"Transactions", new List<Dictionary<string, object>>()}
        };

        var transaction1 = new Dictionary<string, object>
        {
            {"Id", 101},
            {"Amount", 2000.00},
            {"Description", "Nekakava plaća"},
            {"Type", "Prihod"},
            {"Category", "Plaća"},
            {"Date", DateTime.Now.AddMonths(-1)}
        };

        var transaction2 = new Dictionary<string, object>
        {
            {"Id", 102},
            {"Amount", -150.50},
            {"Description", "Trošak na hranu"},
            {"Type", "Rashod"},
            {"Category", "Hrana"},
            {"Date", DateTime.Now.AddDays(-10)}
        };

        ((List<Dictionary<string, object>>)account2["Transactions"]).Add(transaction1);
        ((List<Dictionary<string, object>>)account1["Transactions"]).Add(transaction2);
        ((List<Dictionary<string, object>>)user1["Accounts"]).Add(account1);
        ((List<Dictionary<string, object>>)user1["Accounts"]).Add(account2);
        ((List<Dictionary<string, object>>)user1["Accounts"]).Add(account3);
        users.Add(user1);

        //user 2
        var user2 = new Dictionary<string, object>
        {
            {"Id", 19},
            {"Name", "Ivo"},
            {"Surname", "Ivica"},
            {"DateOfBirth", new DateTime(1990, 11, 22)},
            {"Accounts", new List<Dictionary<string, object>>()}
        };

        var account4 = new Dictionary<string, object>
        {
            {"AccountType", "tekući"},
            {"Balance", 899.00},
            {"Transactions", new List<Dictionary<string, object>>()}
        };

        var account5 = new Dictionary<string, object>
        {
            {"AccountType", "žiro"},
            {"Balance", 1500.00},
            {"Transactions", new List<Dictionary<string, object>>()}
        };

        var account6 = new Dictionary<string, object>
        {
            {"AccountType", "prepaid"},
            {"Balance", 00.00},
            {"Transactions", new List<Dictionary<string, object>>()}
        };

        var transaction3 = new Dictionary<string, object>
        {
            {"Id", 103},
            {"Amount", 500.00},
            {"Description", "Rođendanski poklon"},
            {"Type", "Prihod"},
            {"Category", "Poklon"},
            {"Date", DateTime.Now.AddDays(-38)}
        };

        ((List<Dictionary<string, object>>)account4["Transactions"]).Add(transaction3);
        ((List<Dictionary<string, object>>)user2["Accounts"]).Add(account4);
        ((List<Dictionary<string, object>>)user2["Accounts"]).Add(account5);
        ((List<Dictionary<string, object>>)user2["Accounts"]).Add(account6);
        users.Add(user2);

        //user 3
        var user3 = new Dictionary<string, object>
        {
            {"Id", 20},
            {"Name", "Jure"},
            {"Surname", "Jurica"},
            {"DateOfBirth", new DateTime(1998, 5, 14)},
            {"Accounts", new List<Dictionary<string, object>>()}
        };

        var account7 = new Dictionary<string, object>
        {
            {"AccountType", "tekući"},
            {"Balance", 2000.00},
            {"Transactions", new List<Dictionary<string, object>>()}
        };

        var account8 = new Dictionary<string, object>
        {
            {"AccountType", "žiro"},
            {"Balance", -700.00},
            {"Transactions", new List<Dictionary<string, object>>()}
        };

        var account9 = new Dictionary<string, object>
        {
            {"AccountType", "prepaid"},
            {"Balance", 145.00},
            {"Transactions", new List<Dictionary<string, object>>()}
        };

        var transaction4 = new Dictionary<string, object>
        {
            {"Id", 724},
            {"Amount", 45.00},
            {"Description", "Uplaćen bonus u iznosu od 45.00 EUR"},
            {"Type", "Prihod"},
            {"Category", "Bonus"},
            {"Date", DateTime.Now.AddDays(-60)}
        };

        var transaction5 = new Dictionary<string, object>
        {
            {"Id", 405},
            {"Amount", 20.00},
            {"Description", "Poklon za Božić"},
            {"Type", "Prihod"},
            {"Category", "Poklon"},
            {"Date", DateTime.Now.AddDays(-13)}
        };

        ((List<Dictionary<string, object>>)account8["Transactions"]).Add(transaction4);
        ((List<Dictionary<string, object>>)account9["Transactions"]).Add(transaction5);
        ((List<Dictionary<string, object>>)user3["Accounts"]).Add(account7);
        ((List<Dictionary<string, object>>)user3["Accounts"]).Add(account8);
        ((List<Dictionary<string, object>>)user3["Accounts"]).Add(account9);
        users.Add(user3);

    }//InitializeData


}//class Program