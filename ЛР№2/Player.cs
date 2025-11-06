using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp30
{

    // enum статус игрока
    public enum PlayerStatus
    {
        Active,
        Inactive
    }

    // enum команда
    public enum Team
    {
        Red,
        Blue,
    }

    // struct для хар-к оружия
    public struct WeaponAttributes
    {
        public int Damage;
        public int AmmoCount;
        public float Range;

        public WeaponAttributes(int damage, int ammoCount, float range)
        {
            Damage = damage;
            AmmoCount = ammoCount;
            Range = range;
        }

        public override string ToString()
        {
            return $"Damage: {Damage}, Ammo: {AmmoCount}, Range: {Range}";
        }
    }

    // второй класс связанный с игроком
    public class Legend
    {
        public string LegendName { get; set; }
        public int Level { get; set; }

        public Legend(string name, int level)
        {
            LegendName = name;
            Level = level;
        }

        public override string ToString()
        {
            return $"Legend: {LegendName}, Level: {Level}";
        }
    }

    public class Player
    {
        // строковые поля
        public string PlayerName { get; set; }
        public string Clan { get; set; }

        // enum поля
        public PlayerStatus Status { get; set; }
        public Team PlayerTeam { get; set; }

        // struct поле
        public WeaponAttributes WeaponStats { get; set; }

        // другой класс
        public Legend LegendInfo { get; set; }

        // числовые поля
        public int Score { get; set; }
        public double Accuracy { get; set; }
        public float Speed { get; set; }

        // автоматическое генерируемое поле
        public Guid PlayerID { get; set; }

        // дата создания
        public DateTime CreatedAt { get; set; }
        public string Name { get; }

        // Конструктор для создания нового игрока (генерирует ID и дату)
        public Player(string name, string clan, Guid playerID, DateTime createdAt, PlayerStatus status, Legend legend, WeaponAttributes weaponAttributes)
        {
            PlayerName = string.IsNullOrEmpty(name) ? null : name;
            Clan = string.IsNullOrEmpty(clan) ? null : clan;
            LegendInfo = legend;
            WeaponStats = weaponAttributes;

            // работа автоматических полей
            Status = PlayerStatus.Active;
            PlayerTeam = Team.Red;
            Score = 0;
            Accuracy = 0.0;
            Speed = 5.0f;

            PlayerID = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        // Конструктор для загрузки из файла или update (с заданными ID и датой)
        public Player(string name, string clan, Legend legend, WeaponAttributes weaponAttributes, Guid id, DateTime createdAt, PlayerStatus status = PlayerStatus.Active, Team team = Team.Red, int score = 0, double accuracy = 0.0, float speed = 5.0f)
        {
            PlayerName = string.IsNullOrEmpty(name) ? null : name;
            Clan = string.IsNullOrEmpty(clan) ? null : clan;
            LegendInfo = legend;
            WeaponStats = weaponAttributes;
            Status = status;
            PlayerTeam = team;
            Score = score;
            Accuracy = accuracy;
            Speed = speed;
            PlayerID = id;
            CreatedAt = createdAt;
        }

        public Player(string name, string clan, Guid playerID)
        {
            Name = name;
            Clan = clan;
        }

        public Player(string name, string clan, Guid playerID, DateTime createdAt, PlayerStatus status, Team playerTeam, int score, double accuracy, float speed) : this(name, clan, playerID)
        {
        }

        public Player(string name, string clan)
        {
            Name = name;
            Clan = clan;
        }

        public override string ToString()
        {
            return $"ID: {PlayerID}, Name: {PlayerName ?? "null"}, Clan: {Clan ?? "null"}, {LegendInfo}, Weapon: {WeaponStats}, Status: {Status}, Team: {PlayerTeam}, Score: {Score}, Accuracy: {Accuracy}, Speed: {Speed}, Created: {CreatedAt}";
        }
    }
}
