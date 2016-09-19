using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Server.MirEnvir;
using MySql.Data.MySqlClient;

namespace Server.MirDatabase
{
    public class MapInfo
    {
        public int Index;
        public string FileName = string.Empty, Title = string.Empty;
        public ushort MiniMap, BigMap, Music;
        public LightSetting Light;
        public byte MapDarkLight = 0, MineIndex = 0;

        public bool NoTeleport, NoReconnect, NoRandom, NoEscape, NoRecall, NoDrug, NoPosition, NoFight,
            NoThrowItem, NoDropPlayer, NoDropMonster, NoNames, NoMount, NeedBridle, Fight, NeedHole, Fire, Lightning;

        public string NoReconnectMap = string.Empty;
        public int FireDamage, LightningDamage;

        public List<SafeZoneInfo> SafeZones = new List<SafeZoneInfo>();
        public List<MovementInfo> Movements = new List<MovementInfo>();
        public List<RespawnInfo> Respawns = new List<RespawnInfo>();
        public List<NPCInfo> NPCs = new List<NPCInfo>();
        public List<MineZone> MineZones = new List<MineZone>();
        public List<Point> ActiveCoords = new List<Point>();

        public InstanceInfo Instance;

        public MapInfo()
        {

        }
        public MapInfo(MySqlDataReader readerMapInfo)
            {
            Index = Convert.ToInt32(readerMapInfo["IndexID"]);
            FileName = Convert.ToString(readerMapInfo["FileName"]);
            Title = Convert.ToString(readerMapInfo["Title"]);
            NoReconnectMap = Convert.ToString(readerMapInfo["NoReconnectMap"]);
            MiniMap = Convert.ToUInt16(readerMapInfo["MiniMap"]);
            BigMap = Convert.ToUInt16(readerMapInfo["BigMap"]);
            Music = Convert.ToUInt16(readerMapInfo["Music"]);
            Light = (LightSetting)Convert.ToByte(readerMapInfo["Light"]);
            MapDarkLight = Convert.ToByte(readerMapInfo["MapDarkLight"]);
            FireDamage = Convert.ToInt32(readerMapInfo["FireDamage"]);
            LightningDamage = Convert.ToInt32(readerMapInfo["LightningDamage"]);
            MineIndex = Convert.ToByte(readerMapInfo["MineIndex"]);
            NoTeleport = Convert.ToBoolean(readerMapInfo["NoTeleport"]);
            NoReconnect = Convert.ToBoolean(readerMapInfo["NoReconnect"]);
            NoRandom = Convert.ToBoolean(readerMapInfo["NoRandom"]);
            NoEscape = Convert.ToBoolean(readerMapInfo["NoEscape"]);
            NoRecall = Convert.ToBoolean(readerMapInfo["NoRecall"]);
            NoDrug = Convert.ToBoolean(readerMapInfo["NoDrug"]);
            NoPosition = Convert.ToBoolean(readerMapInfo["NoPosition"]);
            NoFight = Convert.ToBoolean(readerMapInfo["NoFight"]);
            NoThrowItem = Convert.ToBoolean(readerMapInfo["NoThrowItem"]);
            NoDropPlayer = Convert.ToBoolean(readerMapInfo["NoDropPlayer"]);
            NoDropMonster = Convert.ToBoolean(readerMapInfo["NoDropMonster"]);
            NoNames = Convert.ToBoolean(readerMapInfo["NoNames"]);
            NoMount = Convert.ToBoolean(readerMapInfo["NoMount"]);
            NeedBridle = Convert.ToBoolean(readerMapInfo["NeedBridle"]);
            Fight = Convert.ToBoolean(readerMapInfo["Fight"]);
            NeedHole = Convert.ToBoolean(readerMapInfo["NeedHole"]);
            Fire = Convert.ToBoolean(readerMapInfo["Fire"]);
            Lightning = Convert.ToBoolean(readerMapInfo["Lightning"]);
            }

        public static void SaveMapInfoDB(MapInfo InfoMapList)
            {
            try
                {
                MySqlConnection connection = new MySqlConnection(); //star conection 
                String connectionString;
                connectionString = "Server=" + Settings.ServerIP + "; Uid=" + Settings.Uid + "; Pwd=" + Settings.Pwd + "; convert zero datetime=True";
                connection.ConnectionString = connectionString;
                connection.Open();


                string query = "SELECT COUNT(*) FROM " + Settings.DBServer + ".mapinfo WHERE IndexID = '" + InfoMapList.Index + "'";
                string sqlCommand;
                using (var cmdd = new MySqlCommand(query, connection))
                    {
                    int countt = Convert.ToInt32(cmdd.ExecuteScalar());
                    if (countt == 0)
                        {
                        sqlCommand = "INSERT INTO  " + Settings.DBServer + ".mapinfo (IndexID, Title, FileName, NoReconnectMap, MiniMap, BigMap, Music, Light, MapDarkLight, FireDamage, LightningDamage, MineIndex, NoTeleport, NoReconnect, NoRandom, NoEscape, NoRecall, NoDrug, NoPosition, NoFight, NoThrowItem, NoDropPlayer, NoDropMonster, NoNames, NoMount, NeedBridle, Fight, NeedHole, Fire, Lightning) VALUES (@IndexID, @Title, @FileName, @NoReconnectMap, @MiniMap, @BigMap, @Music, @Light, @MapDarkLight, @FireDamage, @LightningDamage, @MineIndex, @NoTeleport, @NoReconnect, @NoRandom, @NoEscape, @NoRecall, @NoDrug, @NoPosition, @NoFight, @NoThrowItem, @NoDropPlayer, @NoDropMonster, @NoNames, @NoMount, @NeedBridle, @Fight, @NeedHole, @Fire, @Lightning)";

                        }
                    else
                        {
                        sqlCommand = "UPDATE  " + Settings.DBServer + ".mapinfo SET IndexID = @IndexID, Title = @Title, FileName = @FileName, NoReconnectMap = @NoReconnectMap, MiniMap = @MiniMap, BigMap = @BigMap, Music = @Music, Light = @Light, MapDarkLight = @MapDarkLight, FireDamage = @FireDamage, LightningDamage = @LightningDamage, MineIndex = @MineIndex, NoTeleport = @NoTeleport, NoReconnect = @NoReconnect, NoRandom = @NoRandom, NoEscape = @NoEscape, NoRecall = @NoRecall, NoDrug = @NoDrug, NoPosition = @NoPosition, NoFight = @NoFight, NoThrowItem = @NoThrowItem, NoDropPlayer = @NoDropPlayer, NoDropMonster = @NoDropMonster, NoNames = @NoNames, NoMount = @NoMount, NeedBridle = @NeedBridle, Fight = @Fight, NeedHole = @NeedHole, Fire = @Fire, Lightning = @Lightning WHERE IndexID = " + InfoMapList.Index;

                        }
                    using (var Update = new MySqlCommand(sqlCommand, connection))
                        {
                        Update.Parameters.AddWithValue("@IndexID", InfoMapList.Index);
                        Update.Parameters.AddWithValue("@FileName", InfoMapList.FileName);
                        Update.Parameters.AddWithValue("@Title", InfoMapList.Title);
                        Update.Parameters.AddWithValue("@NoReconnectMap", InfoMapList.NoReconnectMap);
                        Update.Parameters.AddWithValue("@MiniMap", InfoMapList.MiniMap);
                        Update.Parameters.AddWithValue("@BigMap", InfoMapList.BigMap);
                        Update.Parameters.AddWithValue("@Music", InfoMapList.Music);
                        Update.Parameters.AddWithValue("@Light", InfoMapList.Light);
                        Update.Parameters.AddWithValue("@MapDarkLight", InfoMapList.MapDarkLight);
                        Update.Parameters.AddWithValue("@FireDamage", InfoMapList.FireDamage);
                        Update.Parameters.AddWithValue("@LightningDamage", InfoMapList.LightningDamage);
                        Update.Parameters.AddWithValue("@MineIndex", InfoMapList.MineIndex);
                        Update.Parameters.AddWithValue("@NoTeleport", InfoMapList.NoTeleport);
                        Update.Parameters.AddWithValue("@NoReconnect", InfoMapList.NoReconnect);
                        Update.Parameters.AddWithValue("@NoRandom", InfoMapList.NoRandom);
                        Update.Parameters.AddWithValue("@NoEscape", InfoMapList.NoEscape);
                        Update.Parameters.AddWithValue("@NoRecall", InfoMapList.NoRecall);
                        Update.Parameters.AddWithValue("@NoDrug", InfoMapList.NoDrug);
                        Update.Parameters.AddWithValue("@NoPosition", InfoMapList.NoPosition);
                        Update.Parameters.AddWithValue("@NoFight", InfoMapList.NoFight);
                        Update.Parameters.AddWithValue("@NoThrowItem", InfoMapList.NoThrowItem);
                        Update.Parameters.AddWithValue("@NoDropPlayer", InfoMapList.NoDropPlayer);
                        Update.Parameters.AddWithValue("@NoDropMonster", InfoMapList.NoDropMonster);
                        Update.Parameters.AddWithValue("@NoNames", InfoMapList.NoNames);
                        Update.Parameters.AddWithValue("@NoMount", InfoMapList.NoMount);
                        Update.Parameters.AddWithValue("@NeedBridle", InfoMapList.NeedBridle);
                        Update.Parameters.AddWithValue("@Fight", InfoMapList.Fight);
                        Update.Parameters.AddWithValue("@NeedHole", InfoMapList.NeedHole);
                        Update.Parameters.AddWithValue("@Fire", InfoMapList.Fire);
                        Update.Parameters.AddWithValue("@Lightning", InfoMapList.Lightning);

                        Update.ExecuteNonQuery();
                        Update.Dispose();
                        }
                    cmdd.Dispose();
                    }

                var sqlCommand_mov = "SELECT COUNT(*) FROM " + Settings.DBServer + ".movements WHERE IndexIDTied =" + InfoMapList.Index;

                using (var cmdd = new MySqlCommand(sqlCommand_mov, connection))
                    {
                    int count = Convert.ToInt32(cmdd.ExecuteScalar());
                    if (count != 0)
                        {
                        var query_respanw = "DELETE FROM  " + Settings.DBServer + ".movements WHERE IndexIDTied =" + InfoMapList.Index;

                        using (var Remove = new MySqlCommand(query_respanw, connection))
                            {
                            Remove.ExecuteNonQuery();
                            Remove.Dispose();

                            }
                        }
                    cmdd.Dispose();
                    }

                for (int i = 0; i < InfoMapList.Movements.Count; i++)
                    {

                    var sqlCommand_respanw = "INSERT INTO  " + Settings.DBServer + ".movements (IndexIDTied, MapIndex, Source_X, Source_Y, Destination_X, Destination_Y, ConquestIndex, NeedHole, NeedMove) VALUES (@IndexIDTied, @MapIndex, @Source_X, @Source_Y, @Destination_X, @Destination_Y, @ConquestIndex, @NeedHole, @NeedMove)";

                    using (var command = new MySqlCommand(sqlCommand_respanw, connection))
                        {
                        command.Parameters.AddWithValue("@IndexIDTied", InfoMapList.Index);
                        command.Parameters.AddWithValue("@MapIndex", InfoMapList.Movements[i].MapIndex);
                        command.Parameters.AddWithValue("@Source_X", InfoMapList.Movements[i].Source.X);
                        command.Parameters.AddWithValue("@Source_Y", InfoMapList.Movements[i].Source.Y);
                        command.Parameters.AddWithValue("@Destination_X", InfoMapList.Movements[i].Destination.X);
                        command.Parameters.AddWithValue("@Destination_Y", InfoMapList.Movements[i].Destination.Y);
                        command.Parameters.AddWithValue("@ConquestIndex", InfoMapList.Movements[i].ConquestIndex);
                        command.Parameters.AddWithValue("@NeedHole", InfoMapList.Movements[i].NeedHole);
                        command.Parameters.AddWithValue("@NeedMove", InfoMapList.Movements[i].NeedMove);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        }
                    }

                var sqlCommand_respanw_check = "SELECT COUNT(*) FROM " + Settings.DBServer + ".respawnInfo WHERE MapRespawnIndex =" + InfoMapList.Index;

                using (var cmdd = new MySqlCommand(sqlCommand_respanw_check, connection))
                    {
                    int count = Convert.ToInt32(cmdd.ExecuteScalar());
                    if (count != 0)
                        {
                        var query_respanw = "DELETE FROM  " + Settings.DBServer + ".respawnInfo WHERE MapRespawnIndex =" + InfoMapList.Index;

                        using (var Remove = new MySqlCommand(query_respanw, connection))
                            {
                            Remove.ExecuteNonQuery();
                            Remove.Dispose();

                            }
                        }
                    cmdd.Dispose();
                    }

                for (int i = 0; i < InfoMapList.Respawns.Count; i++)
                    {
                    var sqlCommand_respanw = "INSERT INTO  " + Settings.DBServer + ".respawnInfo (MapRespawnIndex, RespawnIndex, MonsterIndex, Location_X, Location_Y, RoutePath, Direction, SaveRespawnTime, Count, Spread, Delay, RandomDelay, RespawnTicks) VALUES (@MapRespawnIndex, @RespawnIndex, @MonsterIndex, @Location_X, @Location_Y, @RoutePath, @Direction, @SaveRespawnTime, @Count, @Spread, @Delay, @RandomDelay, @RespawnTicks)";

                    using (var command = new MySqlCommand(sqlCommand_respanw, connection))
                        {
                        command.Parameters.AddWithValue("@MapRespawnIndex", InfoMapList.Index);
                        command.Parameters.AddWithValue("@RespawnIndex", InfoMapList.Respawns[i].RespawnIndex);
                        command.Parameters.AddWithValue("@MonsterIndex", InfoMapList.Respawns[i].MonsterIndex);
                        command.Parameters.AddWithValue("@Location_X", InfoMapList.Respawns[i].Location.X);
                        command.Parameters.AddWithValue("@Location_Y", InfoMapList.Respawns[i].Location.Y);
                        command.Parameters.AddWithValue("@RoutePath", InfoMapList.Respawns[i].RoutePath);
                        command.Parameters.AddWithValue("@Direction", InfoMapList.Respawns[i].Direction);
                        command.Parameters.AddWithValue("@SaveRespawnTime", InfoMapList.Respawns[i].SaveRespawnTime);
                        command.Parameters.AddWithValue("@Count", InfoMapList.Respawns[i].Count);
                        command.Parameters.AddWithValue("@Spread", InfoMapList.Respawns[i].Spread);
                        command.Parameters.AddWithValue("@Delay", InfoMapList.Respawns[i].Delay);
                        command.Parameters.AddWithValue("@RandomDelay", InfoMapList.Respawns[i].RandomDelay);
                        command.Parameters.AddWithValue("@RespawnTicks", InfoMapList.Respawns[i].RespawnTicks);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        }
                    }


                var sqlCommand_safe_check = "SELECT COUNT(*) FROM " + Settings.DBServer + ".safezoneinfo WHERE SafeMapIndex =" + InfoMapList.Index;

                using (var cmdd = new MySqlCommand(sqlCommand_safe_check, connection))
                    {
                    int count = Convert.ToInt32(cmdd.ExecuteScalar());
                    if (count != 0)
                        {
                        var query_respanw = "DELETE FROM  " + Settings.DBServer + ".safezoneinfo WHERE SafeMapIndex =" + InfoMapList.Index;

                        using (var Remove = new MySqlCommand(query_respanw, connection))
                            {
                            Remove.ExecuteNonQuery();
                            Remove.Dispose();

                            }
                        }
                    cmdd.Dispose();
                    }

                for (int i = 0; i < InfoMapList.SafeZones.Count; i++)
                    {
                    var sqlCommand_respanw = "INSERT INTO  " + Settings.DBServer + ".safezoneinfo (SafeMapIndex, Size, StartPoint, Location_X, Location_Y) VALUES (@SafeMapIndex, @Size, @StartPoint, @Location_X, @Location_Y)";

                    using (var command = new MySqlCommand(sqlCommand_respanw, connection))
                        {
                        command.Parameters.AddWithValue("@SafeMapIndex", InfoMapList.Index);
                        command.Parameters.AddWithValue("@Size", InfoMapList.SafeZones[i].Size);
                        command.Parameters.AddWithValue("@StartPoint", InfoMapList.SafeZones[i].StartPoint);
                        command.Parameters.AddWithValue("@Location_X", InfoMapList.SafeZones[i].Location.X);
                        command.Parameters.AddWithValue("@Location_Y", InfoMapList.SafeZones[i].Location.Y);

                        command.ExecuteNonQuery();
                        command.Dispose();
                        }
                    }

                var sqlCommand_mine_check = "SELECT COUNT(*) FROM " + Settings.DBServer + ".minezone WHERE MapMineIndex =" + InfoMapList.Index;

                using (var cmdd = new MySqlCommand(sqlCommand_mine_check, connection))
                    {
                    int count = Convert.ToInt32(cmdd.ExecuteScalar());
                    if (count != 0)
                        {
                        var query_respanw = "DELETE FROM  " + Settings.DBServer + ".minezone WHERE MapMineIndex =" + InfoMapList.Index;

                        using (var Remove = new MySqlCommand(query_respanw, connection))
                            {
                            Remove.ExecuteNonQuery();
                            Remove.Dispose();

                            }
                        }
                    cmdd.Dispose();
                    }

                for (int i = 0; i < InfoMapList.MineZones.Count; i++)
                    {
                    var sqlCommand_respanw = "INSERT INTO  " + Settings.DBServer + ".minezone (MapMineIndex, Size, Mine, Location_X, Location_Y) VALUES (@MapMineIndex, @Size, @Mine, @Location_X, @Location_Y)";

                    using (var command = new MySqlCommand(sqlCommand_respanw, connection))
                        {
                        command.Parameters.AddWithValue("@MapMineIndex", InfoMapList.Index);
                        command.Parameters.AddWithValue("@Size", InfoMapList.MineZones[i].Size);
                        command.Parameters.AddWithValue("@Mine", InfoMapList.MineZones[i].Mine);
                        command.Parameters.AddWithValue("@Location_X", InfoMapList.MineZones[i].Location.X);
                        command.Parameters.AddWithValue("@Location_Y", InfoMapList.MineZones[i].Location.Y);

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

        public MapInfo(BinaryReader reader)
        {
            Index = reader.ReadInt32();
            FileName = reader.ReadString();
            Title = reader.ReadString();
            MiniMap = reader.ReadUInt16();
            Light = (LightSetting) reader.ReadByte();

            if (Envir.LoadVersion >= 3) BigMap = reader.ReadUInt16();

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                SafeZones.Add(new SafeZoneInfo(reader) { Info = this });

            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                Respawns.Add(new RespawnInfo(reader, Envir.LoadVersion, Envir.LoadCustomVersion));

            if (Envir.LoadVersion <= 33)
            {
                count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                    NPCs.Add(new NPCInfo(reader));
            }

            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                Movements.Add(new MovementInfo(reader));

            if (Envir.LoadVersion < 14) return;

            NoTeleport = reader.ReadBoolean();
            NoReconnect = reader.ReadBoolean();
            NoReconnectMap = reader.ReadString();
            NoRandom = reader.ReadBoolean();
            NoEscape = reader.ReadBoolean();
            NoRecall = reader.ReadBoolean();
            NoDrug = reader.ReadBoolean();
            NoPosition = reader.ReadBoolean();
            NoThrowItem = reader.ReadBoolean();
            NoDropPlayer = reader.ReadBoolean();
            NoDropMonster = reader.ReadBoolean();
            NoNames = reader.ReadBoolean();
            Fight = reader.ReadBoolean();
            if (Envir.LoadVersion == 14) NeedHole = reader.ReadBoolean();
            Fire = reader.ReadBoolean();
            FireDamage = reader.ReadInt32();
            Lightning = reader.ReadBoolean();
            LightningDamage = reader.ReadInt32();
            if (Envir.LoadVersion < 23) return;
            MapDarkLight = reader.ReadByte();
            if (Envir.LoadVersion < 26) return;
            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
                MineZones.Add(new MineZone(reader));
            if (Envir.LoadVersion < 27) return;
            MineIndex = reader.ReadByte();

            if (Envir.LoadVersion < 33) return;
            NoMount = reader.ReadBoolean();
            NeedBridle = reader.ReadBoolean();

            if (Envir.LoadVersion < 42) return;
            NoFight = reader.ReadBoolean();

            if (Envir.LoadVersion < 53) return;
                Music = reader.ReadUInt16(); 

        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(FileName);
            writer.Write(Title);
            writer.Write(MiniMap);
            writer.Write((byte)Light);
            writer.Write(BigMap);
            writer.Write(SafeZones.Count);

            for (int i = 0; i < SafeZones.Count; i++)
                SafeZones[i].Save(writer);

            writer.Write(Respawns.Count);
            for (int i = 0; i < Respawns.Count; i++)
                Respawns[i].Save(writer);

            writer.Write(Movements.Count);
            for (int i = 0; i < Movements.Count; i++)
                Movements[i].Save(writer);

            writer.Write(NoTeleport);
            writer.Write(NoReconnect);
            writer.Write(NoReconnectMap);
            writer.Write(NoRandom);
            writer.Write(NoEscape);
            writer.Write(NoRecall);
            writer.Write(NoDrug);
            writer.Write(NoPosition);
            writer.Write(NoThrowItem);
            writer.Write(NoDropPlayer);
            writer.Write(NoDropMonster);
            writer.Write(NoNames);
            writer.Write(Fight);
            writer.Write(Fire);
            writer.Write(FireDamage);
            writer.Write(Lightning);
            writer.Write(LightningDamage);
            writer.Write(MapDarkLight);
            writer.Write(MineZones.Count);
            for (int i = 0; i < MineZones.Count; i++)
                MineZones[i].Save(writer);
            writer.Write(MineIndex);

            writer.Write(NoMount);
            writer.Write(NeedBridle);

            writer.Write(NoFight);

            writer.Write(Music);

            
        }


        public void CreateMap()
        {
            for (int j = 0; j < SMain.Envir.NPCInfoList.Count; j++)
            {
                if (SMain.Envir.NPCInfoList[j].MapIndex != Index) continue;

                NPCs.Add(SMain.Envir.NPCInfoList[j]);
            }

            Map map = new Map(this);

            if(map.Info.FileName == "orc25")
            {

            }

            if (!map.Load()) return;

            SMain.Envir.MapList.Add(map);

            if (Instance == null)
            {
                Instance = new InstanceInfo(this, map);
            }

            for (int i = 0; i < SafeZones.Count; i++)
                if (SafeZones[i].StartPoint)
                    SMain.Envir.StartPoints.Add(SafeZones[i]);
        }

        public void CreateInstance()
        {
            if (Instance.MapList.Count == 0) return;

            Map map = new Map(this);
            if (!map.Load()) return;

            SMain.Envir.MapList.Add(map);

            Instance.AddMap(map);
        }

        public void CreateSafeZone()
        {
            SafeZones.Add(new SafeZoneInfo { Info = this });
        }

        public void CreateRespawnInfo()
        {
            Respawns.Add(new RespawnInfo { RespawnIndex = ++SMain.EditEnvir.RespawnIndex });
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Index, Title);
        }

        public void CreateNPCInfo()
        {
            NPCs.Add(new NPCInfo());
        }

        public void CreateMovementInfo()
        {
            Movements.Add(new MovementInfo());
        }

        public static void FromText(string text)
        {
            string[] data = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (data.Length < 8) return;

            MapInfo info = new MapInfo {FileName = data[0], Title = data[1]};


            if (!ushort.TryParse(data[2], out info.MiniMap)) return;

            if (!Enum.TryParse(data[3], out info.Light)) return;
            int sziCount, miCount, riCount, npcCount;

            if (!int.TryParse(data[4], out sziCount)) return;
            if (!int.TryParse(data[5], out miCount)) return;
            if (!int.TryParse(data[6], out riCount)) return;
            if (!int.TryParse(data[7], out npcCount)) return;


            int start = 8;

            for (int i = 0; i < sziCount; i++)
            {
                SafeZoneInfo temp = new SafeZoneInfo { Info = info };
                int x, y;

                if (!int.TryParse(data[start + (i * 4)], out x)) return;
                if (!int.TryParse(data[start + 1 + (i * 4)], out y)) return;
                if (!ushort.TryParse(data[start + 2 + (i * 4)], out temp.Size)) return;
                if (!bool.TryParse(data[start + 3 + (i * 4)], out temp.StartPoint)) return;

                temp.Location = new Point(x, y);
                info.SafeZones.Add(temp);
            }
            start += sziCount * 4;



            for (int i = 0; i < miCount; i++)
            {
                MovementInfo temp = new MovementInfo();
                int x, y;

                if (!int.TryParse(data[start + (i * 5)], out x)) return;
                if (!int.TryParse(data[start + 1 + (i * 5)], out y)) return;
                temp.Source = new Point(x, y);

                if (!int.TryParse(data[start + 2 + (i * 5)], out temp.MapIndex)) return;

                if (!int.TryParse(data[start + 3 + (i * 5)], out x)) return;
                if (!int.TryParse(data[start + 4 + (i * 5)], out y)) return;
                temp.Destination = new Point(x, y);

                info.Movements.Add(temp);
            }
            start += miCount * 5;


            for (int i = 0; i < riCount; i++)
            {
                RespawnInfo temp = new RespawnInfo();
                int x, y;

                if (!int.TryParse(data[start + (i * 7)], out temp.MonsterIndex)) return;
                if (!int.TryParse(data[start + 1 + (i * 7)], out x)) return;
                if (!int.TryParse(data[start + 2 + (i * 7)], out y)) return;

                temp.Location = new Point(x, y);

                if (!ushort.TryParse(data[start + 3 + (i * 7)], out temp.Count)) return;
                if (!ushort.TryParse(data[start + 4 + (i * 7)], out temp.Spread)) return;
                if (!ushort.TryParse(data[start + 5 + (i * 7)], out temp.Delay)) return;
                if (!byte.TryParse(data[start + 6 + (i * 7)], out temp.Direction)) return;
                if (!int.TryParse(data[start + 7 + (i * 7)], out temp.RespawnIndex)) return;
                if (!bool.TryParse(data[start + 8 + (i * 7)], out temp.SaveRespawnTime)) return;
                if (!ushort.TryParse(data[start + 9 + (i * 7)], out temp.RespawnTicks)) return;

                info.Respawns.Add(temp);
            }
            start += riCount * 7;


            for (int i = 0; i < npcCount; i++)
            {
                NPCInfo temp = new NPCInfo { FileName = data[start + (i * 6)], Name = data[start + 1 + (i * 6)] };
                int x, y;

                if (!int.TryParse(data[start + 2 + (i * 6)], out x)) return;
                if (!int.TryParse(data[start + 3 + (i * 6)], out y)) return;

                temp.Location = new Point(x, y);

                if (!ushort.TryParse(data[start + 4 + (i * 6)], out temp.Rate)) return;
                if (!ushort.TryParse(data[start + 5 + (i * 6)], out temp.Image)) return;

                info.NPCs.Add(temp);
            }



            info.Index = ++SMain.EditEnvir.MapIndex;
            SMain.EditEnvir.MapInfoList.Add(info);
        }
    }

    public class InstanceInfo
    {
        //Constants
        public int PlayerCap = 2;
        public int MaxInstanceCount = 10;

        //
        public MapInfo MapInfo;
        public List<Map> MapList = new List<Map>();

        /*
         Notes
         Create new instance from here if all current maps are full
         Destroy maps when instance is empty - process loop in map or here?
         Change NPC INSTANCEMOVE to move and create next available instance

        */

        public InstanceInfo(MapInfo mapInfo, Map map)
        {
            MapInfo = mapInfo;
            AddMap(map);
        }

        public void AddMap(Map map)
        {
            MapList.Add(map);
        }

        public void RemoveMap(Map map)
        {
            MapList.Remove(map);
        }

        public Map GetFirstAvailableInstance()
        {
            for (int i = 0; i < MapList.Count; i++)
            {
                Map m = MapList[i];

                if (m.Players.Count < PlayerCap) return m;
            }

            return null;
        }

        public void CreateNewInstance()
        {
            MapInfo.CreateInstance();
        }
    }
}