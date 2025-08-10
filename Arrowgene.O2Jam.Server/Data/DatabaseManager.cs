// Arrowgene.O2Jam.Server/Data/DatabaseManager.cs (最终修正版)
using Microsoft.Data.Sqlite;
using System;
using System.IO;
using Arrowgene.O2Jam.Server.Common;

namespace Arrowgene.O2Jam.Server.Data
{
    public class DatabaseManager
    {
        private static readonly string DbPath = Path.Combine(Util.ExecutingDirectory(), "Data", "o2jam.db");
        private static readonly string ConnectionString = $"Data Source={DbPath}";

        public static void Initialize()
        {
            var dbDir = Path.GetDirectoryName(DbPath);
            if (dbDir != null && !Directory.Exists(dbDir))
            {
                Directory.CreateDirectory(dbDir);
            }

            if (!File.Exists(DbPath))
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();

                    // --- Accounts Table ---
                    var createAccountsTableCmd = connection.CreateCommand();
                    createAccountsTableCmd.CommandText = @"
                        CREATE TABLE Accounts (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Username TEXT NOT NULL UNIQUE,
                            Password TEXT NOT NULL
                        );";
                    createAccountsTableCmd.ExecuteNonQuery();

                    // --- Characters Table ---
                    var createCharactersTableCmd = connection.CreateCommand();
                    createCharactersTableCmd.CommandText = @"
                        CREATE TABLE Characters (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            AccountId INTEGER NOT NULL UNIQUE,
                            Name TEXT NOT NULL,
                            Level INTEGER DEFAULT 1,
                            Exp INTEGER DEFAULT 0,
                            Gems INTEGER DEFAULT 0,         -- 'Gem' is a reserved keyword in some SQL variants, using 'Gems'
                            Cash INTEGER DEFAULT 9999,      -- Added Cash field
                            Gender INTEGER DEFAULT 1,       -- Added Gender field (1 for female based on reference)
                            Instrument INTEGER DEFAULT 1429,
                            Hat INTEGER DEFAULT 1431,
                            Top INTEGER DEFAULT 1432,
                            Bottom INTEGER DEFAULT 1433,
                            Shoes INTEGER DEFAULT 1434,
                            Glasses INTEGER DEFAULT 1481,
                            Wing INTEGER DEFAULT 1343,
                            HairAccessory INTEGER DEFAULT 1185,
                            SetAccessory INTEGER DEFAULT 1541,
                            Glove INTEGER DEFAULT 0,
                            Necklace INTEGER DEFAULT 0,
                            Earring INTEGER DEFAULT 0,
                            Pet INTEGER DEFAULT 0,
                            Props INTEGER DEFAULT 0,              -- Added Props field (소품)
                            CostumeProps INTEGER DEFAULT 0,       -- Added Costume props field (의상소품)
                            InstrumentProps INTEGER DEFAULT 0,    -- Added Musical instrument props field (악기소품)
                            PenaltyCount INTEGER DEFAULT 98,    -- Added Penalty Count
                            PenaltyLevel INTEGER DEFAULT 9,     -- Added Penalty Level
                            FOREIGN KEY (AccountId) REFERENCES Accounts (Id)
                        );";
                    createCharactersTableCmd.ExecuteNonQuery();
                }
            }
        }

        public static bool RegisterAccount(string username, string password)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var checkUserCmd = connection.CreateCommand();
                checkUserCmd.CommandText = "SELECT COUNT(1) FROM Accounts WHERE Username = $username COLLATE NOCASE";
                checkUserCmd.Parameters.AddWithValue("$username", username);
                if (Convert.ToInt32(checkUserCmd.ExecuteScalar()) > 0) return false;

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var insertCmd = connection.CreateCommand();
                        insertCmd.Transaction = transaction;
                        insertCmd.CommandText = "INSERT INTO Accounts (Username, Password) VALUES ($username, $password)";
                        insertCmd.Parameters.AddWithValue("$username", username);
                        insertCmd.Parameters.AddWithValue("$password", password);
                        insertCmd.ExecuteNonQuery();

                        var getLastIdCmd = connection.CreateCommand();
                        getLastIdCmd.Transaction = transaction;
                        getLastIdCmd.CommandText = "SELECT last_insert_rowid()";
                        long accountId = (long)getLastIdCmd.ExecuteScalar();

                        var createCharCmd = connection.CreateCommand();
                        createCharCmd.Transaction = transaction;
                        createCharCmd.CommandText = @"
                            INSERT INTO Characters (AccountId, Name) 
                            VALUES ($accountId, $characterName)";
                        createCharCmd.Parameters.AddWithValue("$accountId", accountId);
                        createCharCmd.Parameters.AddWithValue("$characterName", username);
                        createCharCmd.ExecuteNonQuery();

                        transaction.Commit();
                        return true;
                    }
                    catch { transaction.Rollback(); return false; }
                }
            }
        }

        public static Account GetAccount(string username, string password)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // --- vvv 核心修正：移除了对密码的COLLATE NOCASE，进行精确比对 vvv ---
                command.CommandText = "SELECT Id, Username FROM Accounts WHERE Username = $username COLLATE NOCASE AND Password = $password";
                // --- ^^^ 核心修正结束 ^^^ ---

                command.Parameters.AddWithValue("$username", username);
                command.Parameters.AddWithValue("$password", password);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Account { Id = reader.GetInt32(0), Username = reader.GetString(1) };
                    }
                }
            }
            return null;
        }

        public static Character GetCharacterByAccountId(int accountId)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Characters WHERE AccountId = $accountId";
                command.Parameters.AddWithValue("$accountId", accountId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Character
                        {
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Level = reader.GetInt32(reader.GetOrdinal("Level")),
                            Exp = reader.GetInt32(reader.GetOrdinal("Exp")),
                            Gems = reader.GetInt32(reader.GetOrdinal("Gems")),
                            Cash = reader.GetInt32(reader.GetOrdinal("Cash")),
                            Gender = reader.GetInt32(reader.GetOrdinal("Gender")),
                            Instrument = reader.GetInt32(reader.GetOrdinal("Instrument")),
                            Hat = reader.GetInt32(reader.GetOrdinal("Hat")),
                            Top = reader.GetInt32(reader.GetOrdinal("Top")),
                            Bottom = reader.GetInt32(reader.GetOrdinal("Bottom")),
                            Shoes = reader.GetInt32(reader.GetOrdinal("Shoes")),
                            Glasses = reader.GetInt32(reader.GetOrdinal("Glasses")),
                            Wing = reader.GetInt32(reader.GetOrdinal("Wing")),
                            HairAccessory = reader.GetInt32(reader.GetOrdinal("HairAccessory")),
                            SetAccessory = reader.GetInt32(reader.GetOrdinal("SetAccessory")),
                            Glove = reader.GetInt32(reader.GetOrdinal("Glove")),
                            Necklace = reader.GetInt32(reader.GetOrdinal("Necklace")),
                            Earring = reader.GetInt32(reader.GetOrdinal("Earring")),
                            Pet = reader.GetInt32(reader.GetOrdinal("Pet")),
                            Props = reader.GetInt32(reader.GetOrdinal("Props")),
                            CostumeProps = reader.GetInt32(reader.GetOrdinal("CostumeProps")),
                            InstrumentProps = reader.GetInt32(reader.GetOrdinal("InstrumentProps")),
                            PenaltyCount = reader.GetInt32(reader.GetOrdinal("PenaltyCount")),
                            PenaltyLevel = reader.GetInt32(reader.GetOrdinal("PenaltyLevel"))
                        };
                    }
                }
            }
            return null;
        }
    }
}