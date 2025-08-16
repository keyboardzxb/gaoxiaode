using Arrowgene.Logging;
using Arrowgene.O2Jam.Server.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;

namespace Arrowgene.O2Jam.Server.Data
{
    public class DatabaseManager
    {
        private static readonly ILogger Logger = LogProvider.Logger<DatabaseManager>();
        public static string ConnectionString { get; set; }

        private static O2JamDbContext CreateDbContext()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new System.InvalidOperationException("DatabaseManager.ConnectionString must be set before use.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<O2JamDbContext>();
            optionsBuilder.UseSqlServer(ConnectionString);

            return new O2JamDbContext(optionsBuilder.Options);
        }

        public static Account GetAccount(string username, string password)
        {
            using (var context = CreateDbContext())
            {
                // Step 1: Find user by username only.
                var member = context.Members
                    .FirstOrDefault(m => m.UserId.Trim() == username);

                // Step 2: If no user found, log it and return null.
                if (member == null)
                {
                    Logger.Info($"Login Error: User '{username}' not found in the database.");
                    return null;
                }

                // Step 3: Log user details to prove database connection is working.
                Logger.Info($"Login Info: User '{username}' found. Nick: '{member.UserNick.Trim()}', Registered: '{member.RegisterDate}'. Now checking password.");

                // Step 4: Check password.
                if (member.Password.Trim().ToLower() != password.ToLower())
                {
                    Logger.Info($"Login Error: Password mismatch for user '{username}'.");
                    return null;
                }

                Logger.Info($"Login Info: Password correct for user '{username}'. Proceeding with login.");

                // Find corresponding user info
                var user = context.Users
                    .FirstOrDefault(u => u.UserIndexId == member.Id);

                if (user == null)
                {
                    // Data inconsistency: Member exists but UserInfo does not.
                    // We will create the missing data on the fly.
                    Logger.Info($"Login Info: UserInfo for '{username}' not found. Creating it now.");
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            long newId = member.Id;
                            string usernameStr = member.UserId.Trim();
                            string usernickStr = member.UserNick.Trim();

                            // 1. Create UserInfo
                            var newUserInfo = new UserEntity
                            {
                                UserIndexId = (int)newId,
                                UserId = usernameStr,
                                UserNickname = usernickStr,
                                Sex = member.Sex ? "m" : "f",
                                CreateTime = System.DateTime.UtcNow
                            };
                            context.Users.Add(newUserInfo);

                            // 2. Create Character Info (PlayerEntity)
                            var newPlayer = new PlayerEntity
                            {
                                UserIndexId = (int)newId,
                                UserId = usernameStr,
                                UserNickname = usernickStr,
                                Sex = member.Sex,
                                Level = 1,
                                Experience = 0,
                                AdminLevel = 0,
                                Battle = 0,
                                Win = 0,
                                Draw = 0,
                                Lose = 0
                            };
                            context.Players.Add(newPlayer);

                            // 3. Create Default Cash
                            var newCash = new CashEntity
                            {
                                UserIndexId = (int)newId,
                                Gem = 10000,
                                Mcash = 1000,
                                O2cash = 0,
                                Musiccash = 0,
                                Itemcash = 0
                            };
                            context.Cashes.Add(newCash);

                            // 4. Create Default Items
                            var newItems = new ItemEntity
                            {
                                UserIndexId = (int)newId,
                                // Default items based on P_o2jam_create_user for male/female
                                Equip2 = member.Sex ? 7 : 4,   // Hair
                                Equip6 = member.Sex ? 79 : 76,  // Jacket
                                Equip7 = member.Sex ? 106 : 103, // Pants
                                Equip12 = member.Sex ? 35 : 2   // Face
                            };
                            context.Items.Add(newItems);

                            context.SaveChanges();
                            transaction.Commit();

                            // Set the user to the newly created user info
                            user = newUserInfo;
                            Logger.Info($"Login Info: Successfully created missing data for '{username}'.");
                        }
                        catch (System.Exception ex)
                        {
                            var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                            Logger.Error($"Login Error: Failed to create missing data for '{username}'. Error: {errorMsg}");
                            transaction.Rollback();
                            // If we fail to create the data, we cannot log the user in.
                            return null;
                        }
                    }
                }

                return new Account
                {
                    Id = user.UserIndexId,
                    Username = user.UserId
                };
            }
        }

        public static bool RegisterAccount(string username, string password, Setting setting)
        {
            using (var context = CreateDbContext())
            {
                // 1. Check if user already exists in the member table
                if (context.Members.Any(m => m.UserId == username))
                {
                    return false; // User already exists
                }

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // 2. Create and save the new member to get an ID
                        string passwordForDb = password;
                        if (string.Equals(setting.PasswordHash, "MD5", System.StringComparison.OrdinalIgnoreCase))
                        {
                            using (var md5 = System.Security.Cryptography.MD5.Create())
                            {
                                var inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
                                var hashBytes = md5.ComputeHash(inputBytes);
                                passwordForDb = System.Convert.ToHexString(hashBytes).ToLower();
                            }
                        }

                        var newMember = new MemberEntity
                        {
                            UserId = username,
                            UserNick = username, // Default nickname to username
                            Password = passwordForDb,
                            Sex = true, // Default to male
                            RegisterDate = System.DateTime.UtcNow,
                            Id9you = "0", // Default value
                            Vip = 0
                        };
                        context.Members.Add(newMember);
                        context.SaveChanges(); // Save to generate the ID

                        long newId = newMember.Id;

                        // 3. Create UserInfo
                        var newUserInfo = new UserEntity
                        {
                            UserIndexId = (int)newId,
                            UserId = username,
                            UserNickname = username,
                            Sex = "m", // Default to male
                            CreateTime = System.DateTime.UtcNow
                        };
                        context.Users.Add(newUserInfo);

                        // 4. Create Character Info
                        var newPlayer = new PlayerEntity
                        {
                            UserIndexId = (int)newId,
                            UserId = username,
                            UserNickname = username,
                            Sex = true, // Default to male
                            Level = 1,
                            Experience = 0,
                            AdminLevel = 0,
                            Battle = 0,
                            Win = 0,
                            Draw = 0,
                            Lose = 0
                        };
                        context.Players.Add(newPlayer);

                        // 5. Create Default Cash
                        var newCash = new CashEntity
                        {
                            UserIndexId = (int)newId,
                            Gem = 10000,
                            Mcash = 1000,
                            O2cash = 0,
                            Musiccash = 0,
                            Itemcash = 0
                        };
                        context.Cashes.Add(newCash);

                        // 6. Create Default Items
                        var newItems = new ItemEntity
                        {
                            UserIndexId = (int)newId,
                            // Default items based on P_o2jam_create_user for male
                            Equip2 = 7,   // Hair
                            Equip6 = 79,  // Jacket
                            Equip7 = 106, // Pants
                            Equip12 = 35  // Face
                        };
                        context.Items.Add(newItems);

                        // 7. Save all changes
                        context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        public static Character GetCharacterByAccountId(int accountId)
        {
            using (var context = CreateDbContext())
            {
                var player = context.Players.Find(accountId);
                if (player == null)
                {
                    return null;
                }

                var items = context.Items.Find(accountId);
                var cash = context.Cashes.Find(accountId);

                // If items or cash are missing, we can still proceed with default values
                // but the character will be incomplete.
                if (items == null || cash == null)
                {
                    // Consider logging a warning here about incomplete character data.
                }

                return new Character
                {
                    Name = player.UserNickname,
                    Level = player.Level,
                    Exp = player.Experience,
                    Gender = player.Sex ? 1 : 0, // Assuming true is male (1), false is female (0)

                    // From CashEntity
                    Gems = cash?.Gem ?? 0,
                    Cash = cash?.Mcash ?? 0,

                    // From ItemEntity (Equipped items)
                    Instrument = items?.Equip1 ?? 0,
                    Hat = items?.Equip2 ?? 0,
                    Top = items?.Equip6 ?? 0,
                    Bottom = items?.Equip7 ?? 0,
                    Shoes = items?.Equip5 ?? 0, // Note: SQL had shoes before top/bottom, mapping to EQUIP5
                    Glasses = items?.Equip4 ?? 0,
                    Wing = items?.Equip11 ?? 0,
                    HairAccessory = items?.Equip3 ?? 0,
                    SetAccessory = items?.Equip10 ?? 0,
                    Glove = items?.Equip8 ?? 0,
                    Necklace = items?.Equip9 ?? 0,
                    Earring = items?.Equip12 ?? 0, // Note: Mapped to Equip12, might need adjustment
                    Pet = items?.Equip13 ?? 0,
                    Props = items?.Equip14 ?? 0,
                    CostumeProps = items?.Equip15 ?? 0,
                    InstrumentProps = items?.Equip16 ?? 0,

                    // These were in the old Character model but not in the new schema directly
                    // They might be calculated or stored elsewhere. Defaulting to 0 for now.
                    PenaltyCount = 0,
                    PenaltyLevel = 0
                };
            }
        }
    }
}