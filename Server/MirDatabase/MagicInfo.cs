using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Server.MirEnvir;
using S = ServerPackets;
using MySql.Data.MySqlClient;

namespace Server.MirDatabase
{
    public class MagicInfo
    {
        public string Name;
        public Spell Spell;
        public byte BaseCost, LevelCost, Icon;
        public byte Level1, Level2, Level3;
        public ushort Need1, Need2, Need3;
        public uint DelayBase = 1800, DelayReduction;
        public ushort PowerBase, PowerBonus;
        public ushort MPowerBase, MPowerBonus;
        public float MultiplierBase = 1.0f, MultiplierBonus;
        public byte Range = 9;

        public override string ToString()
        {
            return Name;
        }

        public MagicInfo()
        {

        }
        public MagicInfo(MySqlDataReader readerMagicInfo, int version = int.MaxValue, int Customversion = int.MaxValue)
            {

            Need2 = Convert.ToUInt16(readerMagicInfo["Need2"]);
            Name = readerMagicInfo["Name"].ToString();
            Level2 = Convert.ToByte(readerMagicInfo["Level2"]);
            Need3 = Convert.ToUInt16(readerMagicInfo["Need3"]);
            Need1 = Convert.ToUInt16(readerMagicInfo["Need1"]);
            Range = Convert.ToByte(readerMagicInfo["Range_"]);
            DelayReduction = Convert.ToUInt32(readerMagicInfo["DelayReduction"]);
            MultiplierBonus = Convert.ToInt32(readerMagicInfo["MultiplierBonus"]);
            PowerBonus = Convert.ToUInt16(readerMagicInfo["PowerBonus"]);
            MultiplierBase = Convert.ToInt32(readerMagicInfo["MultiplierBase"]);
            PowerBase = Convert.ToUInt16(readerMagicInfo["PowerBase"]);
            MPowerBase = Convert.ToUInt16(readerMagicInfo["MPowerBase"]);
            MPowerBonus = Convert.ToUInt16(readerMagicInfo["MPowerBonus"]);

            Spell = (Spell)Convert.ToInt32(readerMagicInfo["Spell"]);

            BaseCost = Convert.ToByte(readerMagicInfo["BaseCost"]);
            DelayBase = Convert.ToUInt32(readerMagicInfo["DelayBase"]);
            LevelCost = Convert.ToByte(readerMagicInfo["LevelCost"]);
            Level3 = Convert.ToByte(readerMagicInfo["Level3"]);
            Level1 = Convert.ToByte(readerMagicInfo["Level1"]);
            Icon = Convert.ToByte(readerMagicInfo["Icon"]);
            }

        public static void SaveMagicInfoDB(MagicInfo info)
            {
            try
                {
                MySqlConnection connection = new MySqlConnection(); //star conection 
                String connectionString;
                connectionString = "Server=" + Settings.ServerIP + "; Uid=" + Settings.Uid + "; Pwd=" + Settings.Pwd + "; convert zero datetime=True";
                connection.ConnectionString = connectionString;
                connection.Open();
                    string sqlCommand;
                    string queryy = "SELECT COUNT(*) FROM " + Settings.DBServer + ".magicinfo WHERE Spell = '" + Convert.ToInt32(info.Spell) + "'";

                    using (var cmdd = new MySqlCommand(queryy, connection))
                        {
                        int countt = Convert.ToInt32(cmdd.ExecuteScalar());
                        if (countt == 0)
                            {
                             sqlCommand = "INSERT INTO  " + Settings.DBServer + ".magicinfo (Name, Spell, BaseCost, LevelCost, Icon, Level1,Level2, Level3, Need1, Need2, Need3, DelayBase, DelayReduction, PowerBase, PowerBonus, MPowerBase, MPowerBonus, MultiplierBase, MultiplierBonus, Range_) VALUES (@Name, @Spell, @BaseCost, @LevelCost, @Icon, @Level1, @Level2, @Level3, @Need1, @Need2, @Need3, @DelayBase, @DelayReduction, @PowerBase, @PowerBonus, @MPowerBase, @MPowerBonus,@MultiplierBase, @MultiplierBonus, @Range_)";

                            }
                        else
                            {
                            sqlCommand = "UPDATE  " + Settings.DBServer + ".magicinfo SET Spell = @Spell, BaseCost = @BaseCost, LevelCost = @LevelCost, Icon = @Icon, Level1 = @Level1, Level2 = @Level2, Level3 = @Level3, Need1 = @Need1, Need2 = @Need2, Need3 = @Need3, DelayBase = @DelayBase, DelayReduction = @DelayReduction, PowerBase = @PowerBase, PowerBonus = @PowerBonus, MPowerBase = @MPowerBase, MPowerBonus = @MPowerBonus, MultiplierBase = @MultiplierBase, MultiplierBonus = @MultiplierBonus, Range_ = @Range_, Name = @Name WHERE Spell = '" + Convert.ToInt32(info.Spell) + "'";
                        }

                            using (var Update = new MySqlCommand(sqlCommand, connection))
                                {

                                Update.Parameters.AddWithValue("@Need2", info.Need2);
                                Update.Parameters.AddWithValue("@Name", info.Name);
                                Update.Parameters.AddWithValue("@Level2", info.Level2);
                                Update.Parameters.AddWithValue("@Need3", info.Need3);
                                Update.Parameters.AddWithValue("@Need1", info.Need1);
                                Update.Parameters.AddWithValue("@Range_", info.Range);
                                Update.Parameters.AddWithValue("@DelayReduction", info.DelayReduction);
                                Update.Parameters.AddWithValue("@MultiplierBonus", info.MultiplierBonus);
                                Update.Parameters.AddWithValue("@PowerBonus", info.PowerBonus);
                                Update.Parameters.AddWithValue("@MultiplierBase", info.MultiplierBase);
                                Update.Parameters.AddWithValue("@PowerBase", info.PowerBase);
                                Update.Parameters.AddWithValue("@MPowerBase", info.MPowerBase);
                                Update.Parameters.AddWithValue("@MPowerBonus", info.MPowerBonus);
                                Update.Parameters.AddWithValue("@Spell", info.Spell);
                                Update.Parameters.AddWithValue("@BaseCost", info.BaseCost);
                                Update.Parameters.AddWithValue("@DelayBase", info.DelayBase);
                                Update.Parameters.AddWithValue("@LevelCost", info.LevelCost);
                                Update.Parameters.AddWithValue("@Level3", info.Level3);
                                Update.Parameters.AddWithValue("@Level1", info.Level1);
                                Update.Parameters.AddWithValue("@Icon", info.Icon);

                                Update.ExecuteNonQuery();
                                Update.Dispose();
                                }

                            }

                connection.Close();
                }
            catch (MySqlException ex)
                {
                SMain.Enqueue(ex);
                }
            }

        public MagicInfo (BinaryReader reader, int version = int.MaxValue, int Customversion = int.MaxValue)
        {
            Name = reader.ReadString();
            Spell = (Spell)reader.ReadByte();
            BaseCost = reader.ReadByte();
            LevelCost = reader.ReadByte();
            Icon = reader.ReadByte();
            Level1 = reader.ReadByte();
            Level2 = reader.ReadByte();
            Level3 = reader.ReadByte();
            Need1 = reader.ReadUInt16();
            Need2 = reader.ReadUInt16();
            Need3 = reader.ReadUInt16();
            DelayBase = reader.ReadUInt32();
            DelayReduction = reader.ReadUInt32();
            PowerBase = reader.ReadUInt16();
            PowerBonus = reader.ReadUInt16();
            MPowerBase = reader.ReadUInt16();
            MPowerBonus = reader.ReadUInt16();

            if (version > 66)
                Range = reader.ReadByte();
            if (version > 70)
            {
                MultiplierBase = reader.ReadSingle();
                MultiplierBonus = reader.ReadSingle();
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write((byte)Spell);
            writer.Write(BaseCost);
            writer.Write(LevelCost);
            writer.Write(Icon);
            writer.Write(Level1);
            writer.Write(Level2);
            writer.Write(Level3);
            writer.Write(Need1);
            writer.Write(Need2);
            writer.Write(Need3);
            writer.Write(DelayBase);
            writer.Write(DelayReduction);
            writer.Write(PowerBase);
            writer.Write(PowerBonus);
            writer.Write(MPowerBase);
            writer.Write(MPowerBonus);
            writer.Write(Range);
            writer.Write(MultiplierBase);
            writer.Write(MultiplierBonus);
        }
    }

    public class UserMagic
    {
        public Spell Spell;
        public MagicInfo Info;

        public byte Level, Key;
        public ushort Experience;
        public bool IsTempSpell;
        public long CastTime;

        private MagicInfo GetMagicInfo(Spell spell)
        {
            for (int i = 0; i < SMain.Envir.MagicInfoList.Count; i++)
            {
                MagicInfo info = SMain.Envir.MagicInfoList[i];
                if (info.Spell != spell) continue;
                return info;
            }
            return null;
        }

        public UserMagic(Spell spell)
        {
            Spell = spell;
            
            Info = GetMagicInfo(Spell);
        }

        public UserMagic(MySqlDataReader readerMagicsDB)
            {
            Spell = (Spell)Convert.ToInt32(readerMagicsDB["Spell"]);
            Info = GetMagicInfo(Spell);

            Level = Convert.ToByte(readerMagicsDB["Level"]);
            Key = Convert.ToByte(readerMagicsDB["Key_"]);
            Experience = Convert.ToUInt16(readerMagicsDB["Experience"]);

            IsTempSpell = Convert.ToBoolean(readerMagicsDB["IsTempSpell"]);
            CastTime = Convert.ToInt64(readerMagicsDB["CastTime"]);

            }

        public UserMagic(BinaryReader reader)
        {
            Spell = (Spell) reader.ReadByte();
            Info = GetMagicInfo(Spell);

            Level = reader.ReadByte();
            Key = reader.ReadByte();
            Experience = reader.ReadUInt16();

            if (Envir.LoadVersion < 15) return;
            IsTempSpell = reader.ReadBoolean();

            if (Envir.LoadVersion < 65) return;
            CastTime = reader.ReadInt64();
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write((byte) Spell);

            writer.Write(Level);
            writer.Write(Key);
            writer.Write(Experience);
            writer.Write(IsTempSpell);
            writer.Write(CastTime);
        }

        public Packet GetInfo()
        {
            return new S.NewMagic
                {
                    Magic = CreateClientMagic()
                };
        }

        public ClientMagic CreateClientMagic()
        {
            return new ClientMagic
                {
                    Spell = Spell,
                    BaseCost = Info.BaseCost,
                    LevelCost = Info.LevelCost,
                    Icon = Info.Icon,
                    Level1 = Info.Level1,
                    Level2 = Info.Level2,
                    Level3 = Info.Level3,
                    Need1 = Info.Need1,
                    Need2 = Info.Need2,
                    Need3 = Info.Need3,
                    Level = Level,
                    Key = Key,
                    Experience = Experience,
                    IsTempSpell = IsTempSpell,
                    Delay = GetDelay(),
                    Range = Info.Range,
                    CastTime = (CastTime != 0) && (SMain.Envir.Time > CastTime)? SMain.Envir.Time - CastTime: 0
            };
        }

        public int GetDamage(int DamageBase)
        {
            return (int)((DamageBase + GetPower()) * GetMultiplier());
        }

        public float GetMultiplier()
        {
            return (Info.MultiplierBase + (Level * Info.MultiplierBonus));
        }

        public int GetPower()
        {
            return (int)Math.Round((MPower() / 4F) * (Level + 1) + DefPower());
        }

        public int MPower()
        {
            if (Info.MPowerBonus > 0)
            {
                return SMain.Envir.Random.Next(Info.MPowerBase, Info.MPowerBonus + Info.MPowerBase);
            }
            else
                return Info.MPowerBase;
        }
        public int DefPower()
        {
            if (Info.PowerBonus > 0)
            {
                return SMain.Envir.Random.Next(Info.PowerBase, Info.PowerBonus + Info.PowerBase);
            }
            else
                return Info.PowerBase;
        }

        public int GetPower(int power)
        {
            return (int)Math.Round(power / 4F * (Level + 1) + DefPower());
        }

        public long GetDelay()
        {
            return Info.DelayBase - (Level * Info.DelayReduction);
        }
    }
}
