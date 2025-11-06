using ConsoleApp30;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
public class Program
{
    private static LinkedList<Player> collection = new LinkedList<Player>();
    private static Dictionary<Guid, LinkedListNode<Player>> idToNode = new Dictionary<Guid, LinkedListNode<Player>>();
    private static DateTime initTime;
    private static string fileName;

    public static void Main(string[] args)
    {

        fileName = GetFileName(args);

        LoadFromFile();

        initTime = DateTime.Now;

        while (true)
        {
            try
            {
                Console.Write("> ");
                string command = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(command)) continue;

                string[] parts = command.Split(' ', 2);
                string cmd = parts[0].ToLower();

                switch (cmd)
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "info":
                        ShowInfo();
                        break;
                    case "show":
                        ShowCollection();
                        break;
                    case "insert":
                        InsertPlayer();
                        break;
                    case "update":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Ошибка: укажите id. Использование: update <id> {element}");
                            break;
                        }
                        UpdatePlayer(parts[1]);
                        break;
                    case "remove_key":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Ошибка: укажите id. Использование: remove_key <id>");
                            break;
                        }
                        RemovePlayer(parts[1]);
                        break;
                    case "clear":
                        ClearCollection();
                        break;
                    case "save":
                        SaveToFile();
                        break;
                    case "execute_script":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Ошибка: укажите имя файла. Использование: execute_script <file_name>");
                            break;
                        }
                        ExecuteScript(parts[1]);
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Неизвестная команда. Введите 'help' для справки.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private static string GetFileName(string[] args)
    {

        if (args.Length > 0)
        {
            return args[0];
        }


        string envFile = Environment.GetEnvironmentVariable("COLLECTION_FILE");
        if (!string.IsNullOrEmpty(envFile))
        {
            return envFile;
        }


        Console.Write("Введите имя файла для коллекции: ");
        return Console.ReadLine()?.Trim() ?? "collection.json";
    }

    private static void LoadFromFile()
    {
        if (!File.Exists(fileName)) return;

        try
        {
            string json = File.ReadAllText(fileName);
            var players = JsonSerializer.Deserialize<List<Player>>(json);
            if (players != null)
            {
                foreach (var player in players)
                {
                    var node = collection.AddLast(player);
                    idToNode[player.PlayerID] = node;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки файла: {ex.Message}");
        }
    }

    private static void SaveToFile()
    {
        try
        {
            var players = collection.ToList();
            string json = JsonSerializer.Serialize(players, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fileName, json);
            Console.WriteLine("Коллекция сохранена.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения: {ex.Message}");
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Доступные команды:");
        Console.WriteLine("help - вывести справку");
        Console.WriteLine("info - информация о коллекции");
        Console.WriteLine("show - показать все элементы");
        Console.WriteLine("insert - добавить новый элемент");
        Console.WriteLine("update <id> - обновить элемент по id");
        Console.WriteLine("remove_key <id> - удалить элемент по id");
        Console.WriteLine("clear - очистить коллекцию");
        Console.WriteLine("save - сохранить в файл");
        Console.WriteLine("execute_script <file_name> - выполнить команды из файла");
        Console.WriteLine("exit - выход");
    }

    private static void ShowInfo()
    {
        Console.WriteLine($"Тип коллекции: LinkedList<Player>");
        Console.WriteLine($"Дата инициализации: {initTime}");
        Console.WriteLine($"Количество элементов: {collection.Count}");
    }

    private static void ShowCollection()
    {
        if (collection.Count == 0)
        {
            Console.WriteLine("Коллекция пуста.");
            return;
        }
        foreach (var player in collection)
        {
            Console.WriteLine(player);
        }
    }

    private static void InsertPlayer()
    {
        try
        {
            string name = ReadStringField("PlayerName");
            string clan = ReadStringField("Clan");

            var player = new ConsoleApp30.Player(name, clan);
            var node = collection.AddLast(player);
            idToNode[player.PlayerID] = node;
            Console.WriteLine("Игрок добавлен.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка добавления: {ex.Message}");
        }
    }

    private static void UpdatePlayer(string idStr)
    {
        try
        {
            Guid id = Guid.Parse(idStr);
            if (!idToNode.ContainsKey(id))
            {
                Console.WriteLine("Игрок с таким ID не найден.");
                return;
            }

            var oldNode = idToNode[id];
            var oldPlayer = oldNode.Value;

            string name = ReadStringField("PlayerName");
            string clan = ReadStringField("Clan");

            Player newPlayer = new Player(name, clan, oldPlayer.PlayerID, oldPlayer.CreatedAt, oldPlayer.Status, oldPlayer.PlayerTeam, oldPlayer.Score, oldPlayer.Accuracy, oldPlayer.Speed);
            oldNode.Value = newPlayer;
            Console.WriteLine("Игрок обновлен.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обновления: {ex.Message}");
        }
    }

    private static void RemovePlayer(string idStr)
    {
        try
        {
            Guid id = Guid.Parse(idStr);
            if (!idToNode.ContainsKey(id))
            {
                Console.WriteLine("Игрок с таким ID не найден.");
                return;
            }

            var node = idToNode[id];
            collection.Remove(node);
            idToNode.Remove(id);
            Console.WriteLine("Игрок удален.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка удаления: {ex.Message}");
        }
    }

    private static void ClearCollection()
    {
        collection.Clear();
        idToNode.Clear();
        Console.WriteLine("Коллекция очищена.");
    }

    private static void ExecuteScript(string scriptFile)
    {
        if (!File.Exists(scriptFile))
        {
            Console.WriteLine("Файл скрипта не найден.");
            return;
        }

        try
        {
            var lines = File.ReadAllLines(scriptFile);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                Console.WriteLine($"> {line}");

                string[] parts = line.Split(' ', 2);
                string cmd = parts[0].ToLower();
                switch (cmd)
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "info":
                        ShowInfo();
                        break;
                    case "show":
                        ShowCollection();
                        break;
                    case "insert":
                        InsertPlayer();
                        break;
                    case "update":
                        if (parts.Length < 2) break;
                        UpdatePlayer(parts[1]);
                        break;
                    case "remove_key":
                        if (parts.Length < 2) break;
                        RemovePlayer(parts[1]);
                        break;
                    case "clear":
                        ClearCollection();
                        break;
                    case "save":
                        SaveToFile();
                        break;
                    case "execute_script":

                        Console.WriteLine("Рекурсивные скрипты не поддерживаются.");
                        break;
                    case "exit":
                        return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения скрипта: {ex.Message}");
        }
    }

    private static string ReadStringField(string fieldName)
    {
        Console.Write($"Введите {fieldName} (пустая строка для null): ");
        string input = Console.ReadLine();
        return string.IsNullOrEmpty(input) ? null : input;
    }

    private static int ReadIntField(string fieldName)
    {
        while (true)
        {
            Console.Write($"Введите {fieldName}: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int value))
            {
                return value;
            }
            Console.WriteLine("Некорректный ввод. Повторите.");
        }
    }

    private static float ReadFloatField(string fieldName)
    {
        while (true)
        {
            Console.Write($"Введите {fieldName}: ");
            string input = Console.ReadLine();
            if (float.TryParse(input, out float value))
            {
                return value;
            }
            Console.WriteLine("Некорректный ввод. Повторите.");
        }
    }
}
