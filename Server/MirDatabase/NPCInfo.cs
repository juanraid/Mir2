using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Server.MirEnvir;
using MySql.Data.MySqlClient;

namespace Server.MirDatabase
{
    public class NPCInfo
    {
        public int Index;

        public string FileName = string.Empty, Name = string.Empty;

        public int MapIndex;
        public Point Location;
        public ushort Rate = 100;
        public ushort Image;
        public Color Colour;

        public bool TimeVisible = false;
        public byte HourStart = 0;
        public byte MinuteStart = 0;
        public byte HourEnd = 0;
        public byte MinuteEnd = 1;
        public short MinLev = 0;
        public short MaxLev = 0;
        public string DayofWeek = "";
        public string ClassRequired = "";
        public bool Sabuk = false;
        public int FlagNeeded = 0;
        public int Conquest;

        public bool IsDefault, IsRobot;

        public List<int> CollectQuestIndexes = new List<int>();
        public List<int> FinishQuestIndexes = new List<int>();
        
        public NPCInfo()
        { }

        public NPCInfo(MySqlDataReader readerNPCInfo)
            {

                Index = Convert.ToInt32(readerNPCInfo["IndexID"]);
                MapIndex = Convert.ToInt32(readerNPCInfo["MapIndex"]);

                //  int count = reader.ReadInt32();
                //  for (int i = 0; i < count; i++)
                //      CollectQuestIndexes.Add(reader.ReadInt32());

                //  count = reader.ReadInt32();
                //  for (int i = 0; i < count; i++)
                //     FinishQuestIndexes.Add(reader.ReadInt32());


            FileName = Convert.ToString(readerNPCInfo["FileName"]);
            Name = Convert.ToString(readerNPCInfo["Name"]);

            Location = new Point(Convert.ToInt32(readerNPCInfo["Location_X"]), Convert.ToInt32(readerNPCInfo["Location_Y"]));
            Image = Convert.ToByte(readerNPCInfo["Image"]);

            Rate = Convert.ToUInt16(readerNPCInfo["Rate"]);

                TimeVisible = Convert.ToBoolean(readerNPCInfo["TimeVisible"]);
                HourStart = Convert.ToByte(readerNPCInfo["HourStart"]);
                MinuteStart = Convert.ToByte(readerNPCInfo["MinuteStart"]);
                HourEnd = Convert.ToByte(readerNPCInfo["HourEnd"]);
                MinuteEnd = Convert.ToByte(readerNPCInfo["MinuteEnd"]);
                MinLev = Convert.ToInt16(readerNPCInfo["MinLev"]);
                MaxLev = Convert.ToInt16(readerNPCInfo["MaxLev"]);
                DayofWeek = Convert.ToString(readerNPCInfo["DayofWeek"]);
                ClassRequired = Convert.ToString(readerNPCInfo["ClassRequired"]);
                Conquest = Convert.ToInt32(readerNPCInfo["Conquest"]);
              
            }

        public static void SaveNPCInfoDB(NPCInfo InfoNPCList)
            {

            try
                {

                MySqlConnection connection = new MySqlConnection(); //star conection 
                String connectionString;
                connectionString = "Server=" + Settings.ServerIP + "; Uid=" + Settings.Uid + "; Pwd=" + Settings.Pwd + "; convert zero datetime=True";
                connection.ConnectionString = connectionString;
                connection.Open();
                string sqlCommand;
                string query = "SELECT COUNT(*) FROM  " + Settings.DBServer + ".npcinfo WHERE IndexID = " + InfoNPCList.Index;
                using (var cmd = new MySqlCommand(query, connection))
                    {
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count == 0)
                        {
                        sqlCommand = "INSERT INTO  " + Settings.DBServer + ".npcinfo (MapIndex, IndexID, FileName, Name, Location_X, Location_Y, Rate, Image, HourStart, MinuteStart, HourEnd, MinuteEnd, TimeVisible, IsDefault, MinLev, MaxLev, DayofWeek, ClassRequired, Conquest) VALUES (@MapIndex, @IndexID, @FileName, @Name, @Location_X, @Location_Y, @Rate, @Image, @HourStart, @MinuteStart, @HourEnd, @MinuteEnd, @TimeVisible, @IsDefault, @MinLev, @MaxLev, @DayofWeek, @ClassRequired, @Conquest)";
                        }
                    else
                        {
                        sqlCommand = "UPDATE  " + Settings.DBServer + ".npcinfo SET MapIndex = @MapIndex, FileName = @FileName, IndexID = @IndexID, Name = @Name, Location_X = @Location_X, Location_Y = @Location_Y, Rate = @Rate, Image = @Image,  HourStart = @HourStart, MinuteStart = @MinuteStart, HourEnd = @HourEnd, MinuteEnd = @MinuteEnd, TimeVisible = @TimeVisible, IsDefault = @IsDefault, MinLev = @MinLev, MaxLev = @MaxLev,  DayofWeek = @DayofWeek, ClassRequired = @ClassRequired, Conquest = @Conquest WHERE IndexID = " + InfoNPCList.Index;
                        }
                    }

                using (var command = new MySqlCommand(sqlCommand, connection))
                    {
                    command.Parameters.AddWithValue("@MapIndex", InfoNPCList.MapIndex);
                    command.Parameters.AddWithValue("@FileName", InfoNPCList.FileName);
                    command.Parameters.AddWithValue("@IndexID", InfoNPCList.Index);
                    command.Parameters.AddWithValue("@Name", InfoNPCList.Name);
                    command.Parameters.AddWithValue("@Location_X", InfoNPCList.Location.X);
                    command.Parameters.AddWithValue("@Location_Y", InfoNPCList.Location.Y);
                    command.Parameters.AddWithValue("@Rate", InfoNPCList.Rate);
                    command.Parameters.AddWithValue("@Image", InfoNPCList.Image);
                    command.Parameters.AddWithValue("@HourStart", InfoNPCList.HourStart);
                    command.Parameters.AddWithValue("@MinuteStart", InfoNPCList.MinuteStart);
                    command.Parameters.AddWithValue("@HourEnd", InfoNPCList.HourEnd);
                    command.Parameters.AddWithValue("@MinuteEnd", InfoNPCList.MinuteEnd);
                    command.Parameters.AddWithValue("@TimeVisible", InfoNPCList.TimeVisible);
                    command.Parameters.AddWithValue("@IsDefault", InfoNPCList.IsDefault);
                    command.Parameters.AddWithValue("@MinLev", InfoNPCList.MinLev);
                    command.Parameters.AddWithValue("@MaxLev", InfoNPCList.MaxLev);
                    command.Parameters.AddWithValue("@DayofWeek", InfoNPCList.DayofWeek);
                    command.Parameters.AddWithValue("@ClassRequired", InfoNPCList.ClassRequired);
                    command.Parameters.AddWithValue("@Conquest", InfoNPCList.Conquest);

                    command.ExecuteNonQuery();
                    command.Dispose();
                    }

                connection.Close();
                }
            catch (MySqlException ex)
                {
                SMain.Enqueue(ex);
                }

            }

       public NPCInfo(BinaryReader reader)
        {
            if (Envir.LoadVersion > 33)
            {
                Index = reader.ReadInt32();
                MapIndex = reader.ReadInt32();

                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                    CollectQuestIndexes.Add(reader.ReadInt32());

                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                    FinishQuestIndexes.Add(reader.ReadInt32());
            }

            FileName = reader.ReadString();
            Name = reader.ReadString();

            Location = new Point(reader.ReadInt32(), reader.ReadInt32());

            if (Envir.LoadVersion >= 72)
            {
                Image = reader.ReadUInt16();
            }
            else
            {
                Image = reader.ReadByte();
            }
            
            Rate = reader.ReadUInt16();

            if (Envir.LoadVersion >= 64)
            {
                TimeVisible = reader.ReadBoolean();
                HourStart = reader.ReadByte();
                MinuteStart = reader.ReadByte();
                HourEnd = reader.ReadByte();
                MinuteEnd = reader.ReadByte();
                MinLev = reader.ReadInt16();
                MaxLev = reader.ReadInt16();
                DayofWeek = reader.ReadString();
                ClassRequired = reader.ReadString();
                if (Envir.LoadVersion >= 66)
                    Conquest = reader.ReadInt32();
                else
                    Sabuk = reader.ReadBoolean();
                FlagNeeded = reader.ReadInt32();
            }
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(MapIndex);

            writer.Write(CollectQuestIndexes.Count());
            for (int i = 0; i < CollectQuestIndexes.Count; i++)
                writer.Write(CollectQuestIndexes[i]);

            writer.Write(FinishQuestIndexes.Count());
            for (int i = 0; i < FinishQuestIndexes.Count; i++)
                writer.Write(FinishQuestIndexes[i]);

            writer.Write(FileName);
            writer.Write(Name);

            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(Image);
            writer.Write(Rate);

            writer.Write(TimeVisible);
            writer.Write(HourStart);
            writer.Write(MinuteStart);
            writer.Write(HourEnd);
            writer.Write(MinuteEnd);
            writer.Write(MinLev);
            writer.Write(MaxLev);
            writer.Write(DayofWeek);
            writer.Write(ClassRequired);
            writer.Write(Conquest);
            writer.Write(FlagNeeded);
        }

        public static void FromText(string text)
        {
            string[] data = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (data.Length < 6) return;

            NPCInfo info = new NPCInfo { Name = data[0] };

            int x, y;

            info.FileName = data[0];
            info.MapIndex = SMain.EditEnvir.MapInfoList.Where(d => d.FileName == data[1]).FirstOrDefault().Index;

            if (!int.TryParse(data[2], out x)) return;
            if (!int.TryParse(data[3], out y)) return;

            info.Location = new Point(x, y);

            info.Name = data[4];

            if (!ushort.TryParse(data[5], out info.Image)) return;
            if (!ushort.TryParse(data[6], out info.Rate)) return;

            info.Index = ++SMain.EditEnvir.NPCIndex;
            SMain.EditEnvir.NPCInfoList.Add(info);
        }
        public string ToText()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6}",
                FileName, SMain.EditEnvir.MapInfoList.Where(d => d.Index == MapIndex).FirstOrDefault().FileName, Location.X, Location.Y, Name, Image, Rate);
        }

        public override string ToString()
        {
            return string.Format("{0}:   {1}", FileName, Functions.PointToString(Location));
        }
    }
}
