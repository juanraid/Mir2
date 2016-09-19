using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Server.MirDatabase;
using Server.MirNetwork;
using Server.MirObjects;
using MySql.Data.MySqlClient;


namespace Server.MirEnvir
    {

    public class ADBConnect
        {
        private MySqlConnection connection;

        private bool OpenConnection()
            {
            try
                {

                string server = Settings.ServerIP;
                string uid = Settings.Uid;
                string password = Settings.Pwd;
                string connectionString;
                connectionString = "SERVER=" + server + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + "; convert zero datetime=True";
                connection = new MySqlConnection(connectionString);
                connection.Open();
                return true;

                }
            catch (MySqlException ex)
                {
                switch (ex.Number)
                    {
                    case 0:
                        SMain.Enqueue("Cannot connect to server.");
                        break;

                    case 1045:
                        SMain.Enqueue("Invalid username/password");
                        break;

                    default:
                        SMain.Enqueue(ex);
                        break;
                    }
                return false;
                }
            }

        private bool CloseConnection()
            {
            try
                {
                connection.Close();
                return true;
                }
            catch (MySqlException ex)
                {
                SMain.Enqueue(ex);
                return false;
                }
            }

        public void Insert(string query)
            {

            if (this.OpenConnection() == true)
                {
                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.ExecuteNonQuery();

                this.CloseConnection();
                }
            }

        public void Update(string query)
            {

            if (this.OpenConnection() == true)
                {

                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = connection;

                cmd.ExecuteNonQuery();

                this.CloseConnection();
                }
            }

        public void Delete(string query)
            {


            if (this.OpenConnection() == true)
                {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
                }
            }

        public int Count(string query)
            {

            int Count = -1;


            if (this.OpenConnection() == true)
                {

                MySqlCommand cmd = new MySqlCommand(query, connection);

                Count = int.Parse(cmd.ExecuteScalar() + "");

                this.CloseConnection();

                return Count;
                }
            else
                {
                return Count;
                }
            }

        public void SaveProgressDB(List<AccountInfo> AccountListDB)
            {
            var timer = Stopwatch.StartNew();
            try
                {

                MySqlConnection connectioninfo = new MySqlConnection(); //star conection 
                String connectionString;
                connectionString = "Server=" + Settings.ServerIP + "; Uid=" + Settings.Uid + "; Pwd=" + Settings.Pwd + "; convert zero datetime=True";
                connectioninfo.ConnectionString = connectionString;
                connectioninfo.Open();

                for (int iii = 0; iii < 1; iii++) { 

                    for (int i = 0; i < AccountListDB.Count; i++)
                    {

                    string sqlAccount = "UPDATE  " + Settings.DBAccount + ".account SET Gold = @Gold, Credit = @Credit WHERE AccountID = '" + AccountListDB[i].AccountID + "'";

                    using (var command = new MySqlCommand(sqlAccount, connectioninfo))
                        {
                        command.Parameters.AddWithValue("@Gold", AccountListDB[i].Gold);
                        command.Parameters.AddWithValue("@Credit", AccountListDB[i].Credit);

                        command.ExecuteNonQuery();
                        command.Dispose();
                        }

                    for (int c = 0; c < AccountListDB[i].Characters.Count; c++)
                        {
                        string sqlCharacterinfo = "UPDATE  " + Settings.DBAccount + ".characterinfo SET Level = @Level, CurrentMapIndex = @CurrentMapIndex, CurrentLocation_X = @CurrentLocation_X, CurrentLocation_Y = @CurrentLocation_Y, Direction = @Direction, BindMapIndex = @BindMapIndex, BindLocation_X = @BindLocation_X, BindLocation_Y = @BindLocation_Y, Experience = @Experience, MentorExp = @MentorExp, PKPoints = @PKPoints WHERE AccountID = '" + AccountListDB[i].AccountID + "' AND IndexID = '" + AccountListDB[i].Characters[c].Index + "'";

                        using (var command = new MySqlCommand(sqlCharacterinfo, connectioninfo))
                            {
                            command.Parameters.AddWithValue("@Level", AccountListDB[i].Characters[c].Level);
                            command.Parameters.AddWithValue("@CurrentMapIndex", AccountListDB[i].Characters[c].CurrentMapIndex);
                            command.Parameters.AddWithValue("@CurrentLocation_X", AccountListDB[i].Characters[c].CurrentLocation.X);
                            command.Parameters.AddWithValue("@CurrentLocation_Y", AccountListDB[i].Characters[c].CurrentLocation.Y);
                            command.Parameters.AddWithValue("@Direction", AccountListDB[i].Characters[c].Direction);
                            command.Parameters.AddWithValue("@BindMapIndex", AccountListDB[i].Characters[c].BindMapIndex);
                            command.Parameters.AddWithValue("@BindLocation_X", AccountListDB[i].Characters[c].BindLocation.X);
                            command.Parameters.AddWithValue("@BindLocation_Y", AccountListDB[i].Characters[c].BindLocation.Y);
                            command.Parameters.AddWithValue("@Experience", AccountListDB[i].Characters[c].Experience);
                            command.Parameters.AddWithValue("@MentorExp", AccountListDB[i].Characters[c].MentorExp);
                            command.Parameters.AddWithValue("@PKPoints", AccountListDB[i].Characters[c].PKPoints);

                                command.ExecuteNonQuery();
                            command.Dispose();
                            }

                        for (int mm = 0; mm < AccountListDB[i].Characters[c].Magics.Count; mm++)
                            {

                            if (AccountListDB[i].Characters[c].Magics[mm].Level < 3 || AccountListDB[i].Characters[c].Magics[mm].Spell == Spell.Fury || AccountListDB[i].Characters[c].Magics[mm].Spell == Spell.ImmortalSkin || AccountListDB[i].Characters[c].Magics[mm].Spell == Spell.SwiftFeet)
                                {
                                int IdSpell = Convert.ToInt32(AccountListDB[i].Characters[c].Magics[mm].Spell);

                                string sqlMagics = "UPDATE  " + Settings.DBAccount + ".magics SET Level = @Level, Experience = @Experience, Key_ = @Key_ WHERE ChName = '" + AccountListDB[i].Characters[c].Name + "' And Spell = '" + IdSpell + "' ";

                                using (var command = new MySqlCommand(sqlMagics, connectioninfo))
                                    {
                                    command.Parameters.AddWithValue("@Level", AccountListDB[i].Characters[c].Magics[mm].Level);
                                    command.Parameters.AddWithValue("@Experience", AccountListDB[i].Characters[c].Magics[mm].Experience);
                                    command.Parameters.AddWithValue("@Key_", AccountListDB[i].Characters[c].Magics[mm].Key);

                                    command.ExecuteNonQuery();
                                    command.Dispose();
                                    }
                                }
                            }

                        for (int ee = 0; ee < AccountListDB[i].Characters[c].Equipment.Length; ee++)
                            {

                            if (AccountListDB[i].Characters[c].Equipment[ee] == null) continue;

                            string sqlEquipment = "UPDATE  " + Settings.DBAccount + ".inventory SET CurrentDura = @CurrentDura, MaxDura = @MaxDura, Count = @Count, Cursed = @Cursed, ExpireInfo = @ExpireInfo  WHERE ChName = '" + AccountListDB[i].Characters[c].Name + "' AND UniqueID = '" + AccountListDB[i].Characters[c].Equipment[ee].UniqueID + "'";

                            using (var command = new MySqlCommand(sqlEquipment, connectioninfo))
                                {
                                command.Parameters.AddWithValue("@CurrentDura", AccountListDB[i].Characters[c].Equipment[ee].CurrentDura);
                                command.Parameters.AddWithValue("@MaxDura", AccountListDB[i].Characters[c].Equipment[ee].MaxDura);
                                command.Parameters.AddWithValue("@Count", AccountListDB[i].Characters[c].Equipment[ee].Count);
                                command.Parameters.AddWithValue("@Cursed", AccountListDB[i].Characters[c].Equipment[ee].Cursed);
                                command.Parameters.AddWithValue("@ExpireInfo", AccountListDB[i].Characters[c].Equipment[ee].ExpiryDate);

                                command.ExecuteNonQuery();
                                command.Dispose();
                                }

                            if (AccountListDB[i].Characters[c].Equipment[ee].IsAttached)
                                {
                                for (int sl = 0; sl < AccountListDB[i].Characters[c].Equipment[ee].Slots.Length; sl++)
                                    {
                                    if (AccountListDB[i].Characters[c].Equipment[ee].Slots[sl] == null) continue;

                                    string sqlEquipmentSlot = "UPDATE  " + Settings.DBAccount + ".inventory SET CurrentDura = @CurrentDura, MaxDura = @MaxDura, Count = @Count, Cursed = @Cursed, ExpireInfo = @ExpireInfo  WHERE ChName = '" + AccountListDB[i].Characters[c].Name + "' AND UniqueID = '" + AccountListDB[i].Characters[c].Equipment[ee].Slots[sl].UniqueID + "'";

                                    using (var command = new MySqlCommand(sqlEquipmentSlot, connectioninfo))
                                        {
                                        command.Parameters.AddWithValue("@CurrentDura", AccountListDB[i].Characters[c].Equipment[ee].Slots[sl].CurrentDura);
                                        command.Parameters.AddWithValue("@MaxDura", AccountListDB[i].Characters[c].Equipment[ee].Slots[sl].MaxDura);
                                        command.Parameters.AddWithValue("@Count", AccountListDB[i].Characters[c].Equipment[ee].Slots[sl].Count);
                                        command.Parameters.AddWithValue("@Cursed", AccountListDB[i].Characters[c].Equipment[ee].Slots[sl].Cursed);
                                        command.Parameters.AddWithValue("@ExpireInfo", AccountListDB[i].Characters[c].Equipment[ee].Slots[sl].ExpiryDate);

                                        command.ExecuteNonQuery();
                                        command.Dispose();
                                        }
                                    }
                                }
                            }

                        for (int mm = 0; mm < AccountListDB[i].Characters[c].Buffs.Count; mm++)
                            {
                            if (AccountListDB[i].Characters[c].Buffs[mm].Type == BuffType.Drop || AccountListDB[i].Characters[c].Buffs[mm].Type == BuffType.Exp || AccountListDB[i].Characters[c].Buffs[mm].Type == BuffType.General || AccountListDB[i].Characters[c].Buffs[mm].Type == BuffType.Transform)
                                {
                                int typeBuff = Convert.ToInt32(AccountListDB[i].Characters[c].Buffs[mm].Type);

                                string sqlbuff = "UPDATE  " + Settings.DBAccount + ".buff SET ExpireTime = @ExpireTime WHERE ChName = '" + AccountListDB[i].Characters[c].Name + "' AND Type = '" + typeBuff + "'";

                                using (var command = new MySqlCommand(sqlbuff, connectioninfo))
                                    {
                                    command.Parameters.AddWithValue("@ExpireTime", AccountListDB[i].Characters[c].Buffs[mm].ExpireTime);
                                    command.ExecuteNonQuery();
                                    command.Dispose();
                                    }
                                }
                            }

                        for (int it = 0; it < AccountListDB[i].Characters[c].IntelligentCreatures.Count; it++)
                            {
                            string sqlIntelligentCreatures = "UPDATE " + Settings.DBAccount + ".intelligentcreatures SET ExpireTime = @ExpireTime, BlackstoneTime = @BlackstoneTime, MaintainFoodTime = @MaintainFoodTime, CustomName = @CustomName WHERE ChName = '" + AccountListDB[i].Characters[c].Name + "'  And PetType = '" + Convert.ToInt32(AccountListDB[i].Characters[c].IntelligentCreatures[it].PetType) + "'";

                            using (var command = new MySqlCommand(sqlIntelligentCreatures, connectioninfo))
                                {
                                command.Parameters.AddWithValue("@ExpireTime", AccountListDB[i].Characters[c].IntelligentCreatures[it].ExpireTime);
                                command.Parameters.AddWithValue("@BlackstoneTime", AccountListDB[i].Characters[c].IntelligentCreatures[it].BlackstoneTime);
                                command.Parameters.AddWithValue("@MaintainFoodTime", AccountListDB[i].Characters[c].IntelligentCreatures[it].MaintainFoodTime);
                       
                                command.Parameters.AddWithValue("@CustomName", AccountListDB[i].Characters[c].IntelligentCreatures[it].CustomName);

                                command.ExecuteNonQuery();
                                command.Dispose();
                                }
                            }
                        }
                    }

                }
                connectioninfo.Close();
                AccountListDB.Clear();

                }

            catch (MySqlException ex)
                {
                SMain.Enqueue(ex);
                }

            SMain.Enqueue(string.Format("Saved, Elapsed para int.Parse: {0}", timer.Elapsed));

            }
 }



    public class SaveAccount
        {
     
        
        public void SaveProgressDB(List<AccountInfo> AccountListDB)
            {
            
                                SaveProgressAccountsDB(AccountListDB);
                                SaveProgressCharacterinfoDB(AccountListDB);
                                SaveProgressEquipmentDB(AccountListDB);
                                SaveProgressInventoryDB(AccountListDB);
                                SaveProgressStorageDB(AccountListDB);
                                SaveProgressMagicDB(AccountListDB);
            

            }


        private String AddItemDBString(UserItem item, string table, string store, int position, string name,ulong IdIndex = 0 )
            {

            // DateTime theDate = item.ExpireInfo;
            // theDate.ToString("yyyy-MM-dd H:mm:ss")
            string ID;
            string IdValue;
            string sqlInsert;
            string tipe;

            if (table.ToLower() == "storage")
                {
                ID = "AccountID"; 
                IdValue = name;
                }
            else
                {
                ID = "ChName";
                IdValue = name;
                }

            if (table != "auctionsitems" && table != "mailitems")
                {
                sqlInsert = "INSERT INTO " + Settings.DBAccount + "." + table + " (UniqueID, " + ID + ", Store, ItemIndex, Position, CurrentDura, MaxDura, Count, MC, SC, AC, DC, MAC, Accuracy, Agility, HP, MP, AttackSpeed, Luck, SoulBoundId, Identified, Cursed, Strong, MagicResist, PoisonResist, HealthRecovery, ManaRecovery, PoisonRecovery, CriticalRate, CriticalDamage, Freezing, PoisonAttack, Slots, GemCount, RefinedValue, RefineAdded, WeddingRing, ExpireInfo, IsAttached, ItemName, Attached, IsAwake, AwakeType) VALUES ('" + item.UniqueID + "', '" + IdValue + "', '" + store + "', '" + item.ItemIndex + "', '" + position + "', '" + item.CurrentDura + "', '" + item.MaxDura + "', '" + item.Count + "', '" + item.MC + "', '" + item.SC + "', '" + item.AC + "', '" + item.DC + "', '" + item.MAC + "', '" + item.Accuracy + "', '" + item.Agility + "', '" + item.HP + "', '" + item.MP + "', '" + item.AttackSpeed + "', '" + item.Luck + "', '" + item.SoulBoundId + "', '" + item.Identified + "', '" + item.Cursed + "', '" + item.Strong + "', '" + item.MagicResist + "', '" + item.PoisonResist + "', '" + item.HealthRecovery + "', '" + item.ManaRecovery + "', '" + item.PoisonRecovery + "', '" + item.CriticalRate + "', '" + item.CriticalDamage + "', '" + item.Freezing + "', '" + item.PoisonAttack + "', '" + item.Slots + "', '" + item.GemCount + "', '" + Convert.ToInt32(item.RefinedValue) + "', '" + item.RefineAdded + "', '" + Convert.ToInt32(item.WeddingRing) + "', '" + item.ExpireInfo + "', '" + Convert.ToInt32(item.IsAttached) + "','" + item.Info.Name + "','" + Convert.ToUInt64(item.Attached) + "','" + Convert.ToInt32(item.IsAwake) + "','" + Convert.ToInt32(item.AwakeType) + "')";

                return sqlInsert;
                }
            else
                {
                if (table == "auctionsitems")
                    {
                    tipe = "AuctionID";
                    }
                else
                    {
                    tipe = "MailID";
                    }
                sqlInsert = "INSERT INTO " + Settings.DBAccount + "." + table + " (UniqueID, " + ID + ", Store, ItemIndex, Position, CurrentDura, MaxDura, Count, MC, SC, AC, DC, MAC, Accuracy, Agility, HP, MP, AttackSpeed, Luck, SoulBoundId, Identified, Cursed, Strong, MagicResist, PoisonResist, HealthRecovery, ManaRecovery, PoisonRecovery, CriticalRate, CriticalDamage, Freezing, PoisonAttack, Slots, GemCount, RefinedValue, RefineAdded, WeddingRing, ExpireInfo, IsAttached, ItemName, Attached, IsAwake, AwakeType, " + tipe + ") VALUES ('" + item.UniqueID + "', '" + IdValue + "', '" + store + "', '" + item.ItemIndex + "', '" + position + "', '" + item.CurrentDura + "', '" + item.MaxDura + "', '" + item.Count + "', '" + item.MC + "', '" + item.SC + "', '" + item.AC + "', '" + item.DC + "', '" + item.MAC + "', '" + item.Accuracy + "', '" + item.Agility + "', '" + item.HP + "', '" + item.MP + "', '" + item.AttackSpeed + "', '" + item.Luck + "', '" + item.SoulBoundId + "', '" + item.Identified + "', '" + item.Cursed + "', '" + item.Strong + "', '" + item.MagicResist + "', '" + item.PoisonResist + "', '" + item.HealthRecovery + "', '" + item.ManaRecovery + "', '" + item.PoisonRecovery + "', '" + item.CriticalRate + "', '" + item.CriticalDamage + "', '" + item.Freezing + "', '" + item.PoisonAttack + "', '" + item.Slots + "', '" + item.GemCount + "', '" + Convert.ToInt32(item.RefinedValue) + "', '" + item.RefineAdded + "', '" + Convert.ToInt32(item.WeddingRing) + "', '" + item.ExpireInfo + "', '" + Convert.ToInt32(item.IsAttached) + "','" + item.Info.Name + "','" + Convert.ToUInt64(item.Attached) + "','" + Convert.ToInt32(item.IsAwake) + "','" + Convert.ToInt32(item.AwakeType) + "', '" + IdIndex + "')";

                return sqlInsert;
                }

            }

        public void GainAddItemDB(UserItem clonedItem, string table, string store, int position, string name, ulong IdIndex = 0 )
            {
            string sqlInsertDB;

            // CheckItem(item);

            // UserItem clonedItem = item.Clone();

            //  Enqueue(new S.GainedItem { Item = clonedItem }); //Cloned because we are probably going to change the amount.
            //  RefreshBagWeight();

            sqlInsertDB = AddItemDBString(clonedItem, table, store, position, name, IdIndex);

            Envir.ConnectADB.Insert(sqlInsertDB);

            if (clonedItem.IsAttached == true)
                {

                string typestore;

                if (clonedItem == null || clonedItem.Info.Type == ItemType.Mount || (clonedItem.Info.Shape == 49 && clonedItem.Info.Shape == 50))
                    {
                    typestore = "Fishing";
                    }
                else
                    {
                    typestore = "Mount";
                    }

                for (int j = 0; j < clonedItem.Slots.Length; j++)
                    {
                    if (clonedItem.Slots[j] == null) continue;
                    
                    UserItem clonedItem1 = clonedItem.Slots[j].Clone();

                    sqlInsertDB = AddItemDBString(clonedItem1, table, typestore, j, name, IdIndex);

                    Envir.ConnectADB.Insert(sqlInsertDB);

                    }
                }

            if (clonedItem.IsAwake == true)
                {
                string sqlCount = "SELECT Count(*) FROM " + Settings.DBAccount + ".awake where UniqueID = '" + clonedItem.UniqueID + "'";
                if (Envir.ConnectADB.Count(sqlCount) > 0) return;

                for (int j = 0; j < clonedItem.Awake.listAwake.Count; j++)
                    {
                    string sqlawake = "INSERT INTO " + Settings.DBAccount + ".awake (UniqueID, Value, Position) VALUES ('" + clonedItem.UniqueID + "','" + clonedItem.Awake.listAwake[j] + "','" + j + "')";
                    Envir.ConnectADB.Insert(sqlawake);
                    }

                }
            }


        public void SaveProgressStorageDB(List<AccountInfo> AccountListDB)
            {

                for (int i = 0; i < AccountListDB.Count; i++)
                    {

                    for (int s = 0; s < AccountListDB[i].Storage.Length; s++)
                        {
 if (AccountListDB[i].Storage[s] == null) continue;
                        if (AccountListDB[i].Storage[s].Awake.type != AwakeType.None)
                            {
                       
                            AccountListDB[i].Storage[s].IsAwake = true;
                            AccountListDB[i].Storage[s].AwakeType = Convert.ToInt32(AccountListDB[i].Storage[s].Awake.type);
                            }
                        
                        if (AccountListDB[i].Storage[s].Info.Type == ItemType.Mount || (AccountListDB[i].Storage[s].Info.Shape == 49 && AccountListDB[i].Storage[s].Info.Shape == 50))
                            {
                            for (int j = 0; j < AccountListDB[i].Storage[s].Slots.Length; j++)
                                {
                                if (AccountListDB[i].Storage[s].Slots[j] == null) continue;
                                AccountListDB[i].Storage[s].Slots[j].Attached = AccountListDB[i].Storage[s].UniqueID;
                                AccountListDB[i].Storage[s].IsAttached = true;
                                }

                            }

                    GainAddItemDB(AccountListDB[i].Storage[s], "storage", "Storage", s, AccountListDB[i].AccountID, 0);

                        }

                    }
            }
        
        public void SaveProgressEquipmentDB(List<AccountInfo> AccountListDB)
            {

                for (int i = 0; i < AccountListDB.Count; i++)
                    {
                    for (int ee = 0; ee < AccountListDB[i].Characters.Count; ee++) {

                        for (int eq = 0; eq < AccountListDB[i].Characters[ee].Equipment.Length; eq++)
                        {
                        if (AccountListDB[i].Characters[ee].Equipment[eq] == null) continue;
                        if (AccountListDB[i].Characters[ee].Equipment[eq].Awake.type != AwakeType.None)
                            {

                            AccountListDB[i].Characters[ee].Equipment[eq].IsAwake = true;
                            AccountListDB[i].Characters[ee].Equipment[eq].AwakeType = Convert.ToInt32(AccountListDB[i].Characters[ee].Equipment[eq].Awake.type);
                            }

                        if (AccountListDB[i].Characters[ee].Equipment[eq].Info.Type == ItemType.Mount || (AccountListDB[i].Characters[ee].Equipment[eq].Info.Shape == 49 && AccountListDB[i].Characters[ee].Equipment[eq].Info.Shape == 50))
                            {
                            for (int j = 0; j < AccountListDB[i].Characters[ee].Equipment[eq].Slots.Length; j++)
                                {
                                if (AccountListDB[i].Characters[ee].Equipment[eq].Slots[j] == null) continue;
                                AccountListDB[i].Characters[ee].Equipment[eq].Slots[j].Attached = AccountListDB[i].Characters[ee].Equipment[eq].UniqueID;
                                AccountListDB[i].Characters[ee].Equipment[eq].IsAttached = true;
                                }

                            }

                        GainAddItemDB(AccountListDB[i].Characters[ee].Equipment[eq], "inventory", "Equipment", eq, AccountListDB[i].AccountID, 0);

                        }
                    }
                    
                    }
                
            }

        public void SaveProgressInventoryDB(List<AccountInfo> AccountListDB)
            {
            for (int i = 0; i < AccountListDB.Count; i++)
                {
                for (int ee = 0; ee < AccountListDB[i].Characters.Count; ee++)
                    {

                    for (int eq = 0; eq < AccountListDB[i].Characters[ee].Inventory.Length; eq++)
                        {
                        if (AccountListDB[i].Characters[ee].Inventory[eq] == null) continue;
                        if (AccountListDB[i].Characters[ee].Inventory[eq].Awake.type != AwakeType.None)
                            {

                            AccountListDB[i].Characters[ee].Inventory[eq].IsAwake = true;
                            AccountListDB[i].Characters[ee].Inventory[eq].AwakeType = Convert.ToInt32(AccountListDB[i].Characters[ee].Inventory[eq].Awake.type);
                            }

                        if (AccountListDB[i].Characters[ee].Inventory[eq].Info.Type == ItemType.Mount || (AccountListDB[i].Characters[ee].Inventory[eq].Info.Shape == 49 && AccountListDB[i].Characters[ee].Inventory[eq].Info.Shape == 50))
                            {
                            for (int j = 0; j < AccountListDB[i].Characters[ee].Inventory[eq].Slots.Length; j++)
                                {
                                if (AccountListDB[i].Characters[ee].Inventory[eq].Slots[j] == null) continue;
                                AccountListDB[i].Characters[ee].Inventory[eq].Slots[j].Attached = AccountListDB[i].Characters[ee].Inventory[eq].UniqueID;
                                AccountListDB[i].Characters[ee].Inventory[eq].IsAttached = true;
                                }

                            }

                        GainAddItemDB(AccountListDB[i].Characters[ee].Inventory[eq], "inventory", "Inventory", eq, AccountListDB[i].AccountID, 0);

                        }
                    }

                }

            }

       public void SaveProgressCharacterinfoDB(List<AccountInfo> AccountListDB)
            {

            try
                {

                MySqlConnection connectionCharacterinfo = new MySqlConnection(); //star conection 
                String connectionString;
                connectionString = "Server=" + Settings.ServerIP + "; Uid=" + Settings.Uid + "; Pwd=" + Settings.Pwd + "; convert zero datetime=True";
                connectionCharacterinfo.ConnectionString = connectionString;
                connectionCharacterinfo.Open();

                for (int i = 0; i < AccountListDB.Count; i++)
                    {
                    string sqlCharacterinfo;

                    for (int c = 0; c < AccountListDB[i].Characters.Count; c++)
                        {

                        string query_Characters = "SELECT COUNT(*) FROM  " + Settings.DBAccount + ".characterinfo WHERE AccountID = '" + AccountListDB[i].AccountID + "' AND IndexID = '" + AccountListDB[i].Characters[c].Index + "'";
                        using (var cmd = new MySqlCommand(query_Characters, connectionCharacterinfo))
                            {

                            int count = Convert.ToInt32(cmd.ExecuteScalar());

                            if (count == 0)
                                {
                                sqlCharacterinfo = "INSERT INTO  " + Settings.DBAccount + ".characterinfo (IndexID, AccountID, Name, Level, Class, Gender, Hair, CreationIP, CreationDate, Banned, HP, MP, BanReason, ExpiryDate, LastIP, LastDate, Deleted, DeleteDate, Married, MarriedDate, Mentor, MentorDate, isMentor, MentorExp, CurrentMapIndex, CurrentLocation_X, CurrentLocation_Y, Direction, BindMapIndex, BindLocation_X, BindLocation_Y, Experience, AMode, PMode, PKPoints, Thrusting, HalfMoon, CrossHalfMoon, DoubleSlash, MentalState, AllowGroup, AllowTrade, PearlCount, CollectTime, GuildIndex) VALUES (@IndexID, @AccountID, @Name, @Level, @Class, @Gender, @Hair, @CreationIP, @CreationDate, @Banned, @HP, @MP, @BanReason, @ExpiryDate, @LastIP, @LastDate, @Deleted, @DeleteDate, @Married, @MarriedDate, @Mentor, @MentorDate, @isMentor, @MentorExp, @CurrentMapIndex, @CurrentLocation_X, @CurrentLocation_Y, @Direction, @BindMapIndex, @BindLocation_X, @BindLocation_Y, @Experience, @AMode, @PMode, @PKPoints, @Thrusting, @HalfMoon, @CrossHalfMoon, @DoubleSlash, @MentalState, @AllowGroup, @AllowTrade, @PearlCount, @CollectTime, @GuildIndex)";

                                }

                            else
                                {
                                sqlCharacterinfo = "UPDATE  " + Settings.DBAccount + ".characterinfo SET AccountID = @AccountID, Name = @Name, Level = @Level, Class = @Class, Gender = @Gender, Hair = @Hair, CreationIP = @CreationIP, CreationDate = @CreationDate, Banned = @Banned, HP = @HP, MP = @MP, BanReason = @BanReason, ExpiryDate = @ExpiryDate, LastIP = @LastIP, LastDate = @LastDate, Deleted = @Deleted, DeleteDate = @DeleteDate, Married = @Married, MarriedDate = @MarriedDate, Mentor = @Mentor, MentorDate = @MentorDate, isMentor = @isMentor, MentorExp = @MentorExp, CurrentMapIndex = @CurrentMapIndex, CurrentLocation_X = @CurrentLocation_X, CurrentLocation_Y = @CurrentLocation_Y, Direction = @Direction, BindMapIndex = @BindMapIndex, BindLocation_X = @BindLocation_X, BindLocation_Y = @BindLocation_Y, Experience = @Experience, AMode = @AMode, PMode = @PMode, PKPoints = @PKPoints, Thrusting = @Thrusting, HalfMoon = @HalfMoon, CrossHalfMoon = @CrossHalfMoon, DoubleSlash = @DoubleSlash, MentalState = @MentalState, AllowGroup = @AllowGroup, AllowTrade = @AllowTrade, PearlCount = @PearlCount, CollectTime = @CollectTime,  GuildIndex = @GuildIndex WHERE AccountID = '" + AccountListDB[i].AccountID + "' AND IndexID = '" + AccountListDB[i].Characters[c].Index + "'";
                                }
                            }

                        using (var command = new MySqlCommand(sqlCharacterinfo, connectionCharacterinfo))
                            {
                            command.Parameters.AddWithValue("@IndexID", AccountListDB[i].Characters[c].Index);
                            command.Parameters.AddWithValue("@AccountID", AccountListDB[i].AccountID);
                            command.Parameters.AddWithValue("@Name", AccountListDB[i].Characters[c].Name);
                            command.Parameters.AddWithValue("@Level", AccountListDB[i].Characters[c].Level);
                            command.Parameters.AddWithValue("@Class", AccountListDB[i].Characters[c].Class);
                            command.Parameters.AddWithValue("@Gender", AccountListDB[i].Characters[c].Gender);
                            command.Parameters.AddWithValue("@Hair", AccountListDB[i].Characters[c].Hair);

                            command.Parameters.AddWithValue("@CreationIP", AccountListDB[i].Characters[c].CreationIP);
                            command.Parameters.AddWithValue("@CreationDate", AccountListDB[i].Characters[c].CreationDate);
                            command.Parameters.AddWithValue("@Banned", AccountListDB[i].Characters[c].Banned);
                            command.Parameters.AddWithValue("@BanReason", AccountListDB[i].Characters[c].BanReason);

                            command.Parameters.AddWithValue("@ExpiryDate", AccountListDB[i].Characters[c].ExpiryDate);

                            command.Parameters.AddWithValue("@LastIP", AccountListDB[i].Characters[c].LastIP);
                            command.Parameters.AddWithValue("@LastDate", AccountListDB[i].Characters[c].LastDate);
                            command.Parameters.AddWithValue("@Deleted", AccountListDB[i].Characters[c].Deleted);
                            command.Parameters.AddWithValue("@DeleteDate", AccountListDB[i].Characters[c].DeleteDate);
                            command.Parameters.AddWithValue("@CurrentMapIndex", AccountListDB[i].Characters[c].CurrentMapIndex);
                            command.Parameters.AddWithValue("@CurrentLocation_X", AccountListDB[i].Characters[c].CurrentLocation.X);
                            command.Parameters.AddWithValue("@CurrentLocation_Y", AccountListDB[i].Characters[c].CurrentLocation.Y);
                            command.Parameters.AddWithValue("@Direction", AccountListDB[i].Characters[c].Direction);
                            command.Parameters.AddWithValue("@BindMapIndex", AccountListDB[i].Characters[c].BindMapIndex);
                            command.Parameters.AddWithValue("@BindLocation_X", AccountListDB[i].Characters[c].BindLocation.X);
                            command.Parameters.AddWithValue("@BindLocation_Y", AccountListDB[i].Characters[c].BindLocation.Y);
                            command.Parameters.AddWithValue("@HP", AccountListDB[i].Characters[c].HP);
                            command.Parameters.AddWithValue("@MP", AccountListDB[i].Characters[c].MP);

                            command.Parameters.AddWithValue("@Experience", AccountListDB[i].Characters[c].Experience);
                            command.Parameters.AddWithValue("@AMode", AccountListDB[i].Characters[c].AMode);
                            command.Parameters.AddWithValue("@PMode", AccountListDB[i].Characters[c].PMode);
                            command.Parameters.AddWithValue("@PKPoints", AccountListDB[i].Characters[c].PKPoints);
                            command.Parameters.AddWithValue("@Thrusting", AccountListDB[i].Characters[c].Thrusting);
                            command.Parameters.AddWithValue("@HalfMoon", AccountListDB[i].Characters[c].HalfMoon);
                            command.Parameters.AddWithValue("@CrossHalfMoon", AccountListDB[i].Characters[c].CrossHalfMoon);
                            command.Parameters.AddWithValue("@DoubleSlash", AccountListDB[i].Characters[c].DoubleSlash);
                            command.Parameters.AddWithValue("@MentalState", AccountListDB[i].Characters[c].MentalState);
                            command.Parameters.AddWithValue("@AllowGroup", AccountListDB[i].Characters[c].AllowGroup);
                            command.Parameters.AddWithValue("@AllowTrade", AccountListDB[i].Characters[c].AllowTrade);
                            command.Parameters.AddWithValue("@Married", AccountListDB[i].Characters[c].Married);
                            command.Parameters.AddWithValue("@MarriedDate", AccountListDB[i].Characters[c].MarriedDate);
                            command.Parameters.AddWithValue("@Mentor", AccountListDB[i].Characters[c].Mentor);
                            command.Parameters.AddWithValue("@MentorDate", AccountListDB[i].Characters[c].MentorDate);
                            command.Parameters.AddWithValue("@isMentor", AccountListDB[i].Characters[c].isMentor);
                            command.Parameters.AddWithValue("@MentorExp", AccountListDB[i].Characters[c].MentorExp);
                            command.Parameters.AddWithValue("@PearlCount", AccountListDB[i].Characters[c].PearlCount);
                            command.Parameters.AddWithValue("@CollectTime", AccountListDB[i].Characters[c].CollectTime);
                            command.Parameters.AddWithValue("@GuildIndex", AccountListDB[i].Characters[c].GuildIndex);

                            command.ExecuteNonQuery();
                            command.Dispose();
                            }
                        }
                    }
                connectionCharacterinfo.Close();

                }
            catch (MySqlException ex)
                {
                SMain.Enqueue(ex);
                }

            }

        public void SaveProgressMagicDB(List<AccountInfo> AccountListDB)
            {

            try
                {

                MySqlConnection connectionMagics = new MySqlConnection(); //star conection 
                String connectionString;
                connectionString = "Server=" + Settings.ServerIP + "; Uid=" + Settings.Uid + "; Pwd=" + Settings.Pwd + "; convert zero datetime=True";
                connectionMagics.ConnectionString = connectionString;
                connectionMagics.Open();

                for (int i = 0; i < AccountListDB.Count; i++)
                    {
                    string sqlMagics;

                    for (int c = 0; c < AccountListDB[i].Characters.Count; c++)
                        {
                        for (int mm = 0; mm < AccountListDB[i].Characters[c].Magics.Count; mm++)
                            {

                            sqlMagics = "INSERT INTO  " + Settings.DBAccount + ".magics (Spell, ChName, Level, Key_, Experience, IsTempSpell, CastTime) VALUES (@Spell, @ChName, @Level, @Key_, @Experience, @IsTempSpell, @CastTime)";

                            using (var command = new MySqlCommand(sqlMagics, connectionMagics))
                                {
                                command.Parameters.AddWithValue("@Spell", AccountListDB[i].Characters[c].Magics[mm].Spell);
                                command.Parameters.AddWithValue("@ChName", AccountListDB[i].Characters[c].Name);
                                command.Parameters.AddWithValue("@Level", AccountListDB[i].Characters[c].Magics[mm].Level);
                                command.Parameters.AddWithValue("@Key_", AccountListDB[i].Characters[c].Magics[mm].Key);
                                command.Parameters.AddWithValue("@Experience", AccountListDB[i].Characters[c].Magics[mm].Experience);
                                command.Parameters.AddWithValue("@IsTempSpell", AccountListDB[i].Characters[c].Magics[mm].IsTempSpell);
                                command.Parameters.AddWithValue("@CastTime", AccountListDB[i].Characters[c].Magics[mm].CastTime);
                                command.ExecuteNonQuery();
                                command.Dispose();
                                }

                            }

                        }
                    }
                connectionMagics.Close();

                }
            catch (MySqlException ex)
                {
                SMain.Enqueue(ex);
                }

            }

        public void SaveProgressAccountsDB(List<AccountInfo> AccountListDB)
            {

            try
                {

                MySqlConnection connection = new MySqlConnection(); //star conection 
                String connectionString;
                connectionString = "Server=" + Settings.ServerIP + "; Uid=" + Settings.Uid + "; Pwd=" + Settings.Pwd + "; convert zero datetime=True";
                connection.ConnectionString = connectionString;
                connection.Open();

                for (int i = 0; i < AccountListDB.Count; i++)
                    {
                            string sqlCommand = "INSERT INTO  " + Settings.DBAccount + ".account (AccountID, Password, UserName, BirthDate, SecretQuestion, SecretAnswer, EMailAddress, CreationIP, CreationDate, Banned, BanReason, ExpiryDate, WrongPasswordCount, LastIP, LastDate, Gold, Credit, AdminAccount) VALUES (@AccountID, @Password, @UserName, @BirthDate, @SecretQuestion, @SecretAnswer, @EMailAddress, @CreationIP, @CreationDate, @Banned, @BanReason, @ExpiryDate, @WrongPasswordCount, @LastIP, @LastDate, @Gold, @Credit, @AdminAccount)";
                    

                    using (var command = new MySqlCommand(sqlCommand, connection))
                        {
                        command.Parameters.AddWithValue("@AccountID", AccountListDB[i].AccountID);
                        command.Parameters.AddWithValue("@Password", AccountListDB[i].Password);
                        command.Parameters.AddWithValue("@UserName", AccountListDB[i].UserName);
                        command.Parameters.AddWithValue("@BirthDate", AccountListDB[i].BirthDate);
                        command.Parameters.AddWithValue("@SecretQuestion", AccountListDB[i].SecretQuestion);
                        command.Parameters.AddWithValue("@SecretAnswer", AccountListDB[i].SecretAnswer);
                        command.Parameters.AddWithValue("@EMailAddress", AccountListDB[i].EMailAddress);
                        command.Parameters.AddWithValue("@CreationIP", AccountListDB[i].CreationIP);
                        command.Parameters.AddWithValue("@CreationDate", AccountListDB[i].CreationDate);
                        command.Parameters.AddWithValue("@Banned", AccountListDB[i].Banned);
                        command.Parameters.AddWithValue("@BanReason", AccountListDB[i].BanReason);
                        command.Parameters.AddWithValue("@ExpiryDate", AccountListDB[i].ExpiryDate);
                        command.Parameters.AddWithValue("@WrongPasswordCount", AccountListDB[i].WrongPasswordCount);
                        command.Parameters.AddWithValue("@LastIP", AccountListDB[i].LastIP);
                        command.Parameters.AddWithValue("@LastDate", AccountListDB[i].LastDate);
                        command.Parameters.AddWithValue("@Gold", AccountListDB[i].Gold);
                        command.Parameters.AddWithValue("@Credit", AccountListDB[i].Credit);
                        command.Parameters.AddWithValue("@AdminAccount", AccountListDB[i].AdminAccount);

                        command.ExecuteNonQuery();
                        command.Dispose();
                        }

                    }

                connection.Close();

                }
            catch (MySqlException ex)
                {
                SMain.Enqueue(ex);
                }

            }

        }


    }
