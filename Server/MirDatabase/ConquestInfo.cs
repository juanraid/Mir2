using Server.MirEnvir;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MySql.Data.MySqlClient;

namespace Server.MirDatabase
{
    public class ConquestInfo
    {
        public int Index;
        public bool FullMap;
        public Point Location;
        public ushort Size;
        public string Name;
        public int MapIndex;
        public int PalaceIndex;
        public List<int> ExtraMaps = new List<int>();
        public List<ConquestArcherInfo> ConquestGuards = new List<ConquestArcherInfo>();
        public List<ConquestGateInfo> ConquestGates = new List<ConquestGateInfo>();
        public List<ConquestWallInfo> ConquestWalls = new List<ConquestWallInfo>();
        public List<ConquestSiegeInfo> ConquestSieges = new List<ConquestSiegeInfo>();
        public List<ConquestFlagInfo> ConquestFlags = new List<ConquestFlagInfo>();

        public int GuardIndex;
        public int GateIndex;
        public int WallIndex;
        public int SiegeIndex;
        public int FlagIndex;

        public byte StartHour = 0;
        public int WarLength = 60;

        private int counter;

        public ConquestType Type = ConquestType.Request;
        public ConquestGame Game = ConquestGame.CapturePalace;

        public bool Monday;
        public bool Tuesday;
        public bool Wednesday;
        public bool Thursday;
        public bool Friday;
        public bool Saturday;
        public bool Sunday;

        //King of the hill
        public Point KingLocation;
        public ushort KingSize;

        //Control points
        public List<ConquestFlagInfo> ControlPoints = new List<ConquestFlagInfo>();
        public int ControlPointIndex;

        public ConquestInfo()
        {

        }

        public ConquestInfo(MySqlDataReader readerconquestinfo)
            {
            Index = Convert.ToInt32(readerconquestinfo["IndexID"]);
            Location = new Point(Convert.ToInt32(readerconquestinfo["Location_X"]), Convert.ToInt32(readerconquestinfo["Location_Y"]));
            Size = Convert.ToUInt16(readerconquestinfo["Size"]);
            Name = Convert.ToString(readerconquestinfo["Name"]);
            MapIndex = Convert.ToInt32(readerconquestinfo["MapIndex"]);
            PalaceIndex = Convert.ToInt32(readerconquestinfo["PalaceIndex"]);
            GuardIndex = Convert.ToInt32(readerconquestinfo["GuardIndex"]);
            GateIndex = Convert.ToInt32(readerconquestinfo["GateIndex"]);
            WallIndex = Convert.ToInt32(readerconquestinfo["WallIndex"]);
            SiegeIndex = Convert.ToInt32(readerconquestinfo["SiegeIndex"]);

            StartHour = Convert.ToByte(readerconquestinfo["StartHour"]);
            WarLength = Convert.ToInt32(readerconquestinfo["WarLength"]);
            Type = (ConquestType)Convert.ToByte(readerconquestinfo["Type"]);
            Game = (ConquestGame)Convert.ToByte(readerconquestinfo["Game"]);

            Monday = Convert.ToBoolean(readerconquestinfo["Monday"]);
            Tuesday = Convert.ToBoolean(readerconquestinfo["Tuesday"]);
            Wednesday = Convert.ToBoolean(readerconquestinfo["Wednesday"]);
            Thursday = Convert.ToBoolean(readerconquestinfo["Thursday"]);
            Friday = Convert.ToBoolean(readerconquestinfo["Friday"]);
            Saturday = Convert.ToBoolean(readerconquestinfo["Saturday"]);
            Sunday = Convert.ToBoolean(readerconquestinfo["Sunday"]);

            KingLocation = new Point(Convert.ToInt32(readerconquestinfo["KingLocation_X"]), Convert.ToInt32(readerconquestinfo["KingLocation_Y"]));
            KingSize = Convert.ToUInt16(readerconquestinfo["KingSize"]);
            FullMap = Convert.ToBoolean(readerconquestinfo["FullMap"]);
            FlagIndex = Convert.ToInt32(readerconquestinfo["FlagIndex"]);
            ControlPointIndex = Convert.ToInt32(readerconquestinfo["ControlPointIndex"]);

            }

        public ConquestInfo(BinaryReader reader)
        {
            Index = reader.ReadInt32();

            if(Envir.LoadVersion > 73)
            {
                FullMap = reader.ReadBoolean();
            }

            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Size = reader.ReadUInt16();
            Name = reader.ReadString();
            MapIndex = reader.ReadInt32();
            PalaceIndex = reader.ReadInt32();
            GuardIndex = reader.ReadInt32();
            GateIndex = reader.ReadInt32();
            WallIndex = reader.ReadInt32();
            SiegeIndex = reader.ReadInt32();

            if (Envir.LoadVersion > 72)
            {
                FlagIndex = reader.ReadInt32();
            }

            counter = reader.ReadInt32();
            for (int i = 0; i < counter; i++)
            {
                ConquestGuards.Add(new ConquestArcherInfo(reader));
            }
            counter = reader.ReadInt32();
            for (int i = 0; i < counter; i++)
            {
                ExtraMaps.Add(reader.ReadInt32());
            }
            counter = reader.ReadInt32();
            for (int i = 0; i < counter; i++)
            {
                ConquestGates.Add(new ConquestGateInfo(reader));
            }
            counter = reader.ReadInt32();
            for (int i = 0; i < counter; i++)
            {
                ConquestWalls.Add(new ConquestWallInfo(reader));
            }
            counter = reader.ReadInt32();
            for (int i = 0; i < counter; i++)
            {
                ConquestSieges.Add(new ConquestSiegeInfo(reader));
            }

            if (Envir.LoadVersion > 72)
            {
                counter = reader.ReadInt32();
                for (int i = 0; i < counter; i++)
                {
                    ConquestFlags.Add(new ConquestFlagInfo(reader));
                }
            }

            StartHour = reader.ReadByte();
            WarLength = reader.ReadInt32();
            Type = (ConquestType)reader.ReadByte();
            Game = (ConquestGame)reader.ReadByte();

            Monday = reader.ReadBoolean();
            Tuesday = reader.ReadBoolean();
            Wednesday = reader.ReadBoolean();
            Thursday = reader.ReadBoolean();
            Friday = reader.ReadBoolean();
            Saturday = reader.ReadBoolean();
            Sunday = reader.ReadBoolean();

            KingLocation = new Point(reader.ReadInt32(), reader.ReadInt32());
            KingSize = reader.ReadUInt16();

            if (Envir.LoadVersion > 74)
            {
                ControlPointIndex = reader.ReadInt32();
                counter = reader.ReadInt32();
                for (int i = 0; i < counter; i++)
                {
                    ControlPoints.Add(new ConquestFlagInfo(reader));
                }
            }
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(FullMap);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(Size);
            writer.Write(Name);
            writer.Write(MapIndex);
            writer.Write(PalaceIndex);
            writer.Write(GuardIndex);
            writer.Write(GateIndex);
            writer.Write(WallIndex);
            writer.Write(SiegeIndex);
            writer.Write(FlagIndex);

            writer.Write(ConquestGuards.Count);
            for (int i = 0; i < ConquestGuards.Count; i++)
            {
                ConquestGuards[i].Save(writer);
            }
            writer.Write(ExtraMaps.Count);
            for (int i = 0; i < ExtraMaps.Count; i++)
            {
                writer.Write(ExtraMaps[i]);
            }
            writer.Write(ConquestGates.Count);
            for (int i = 0; i < ConquestGates.Count; i++)
            {
                ConquestGates[i].Save(writer);
            }
            writer.Write(ConquestWalls.Count);
            for (int i = 0; i < ConquestWalls.Count; i++)
            {
                ConquestWalls[i].Save(writer);
            }
            writer.Write(ConquestSieges.Count);
            for (int i = 0; i < ConquestSieges.Count; i++)
            {
                ConquestSieges[i].Save(writer);
            }

            writer.Write(ConquestFlags.Count);
            for (int i = 0; i < ConquestFlags.Count; i++)
            {
                ConquestFlags[i].Save(writer);
            }
            writer.Write(StartHour);
            writer.Write(WarLength);
            writer.Write((byte)Type);
            writer.Write((byte)Game);

            writer.Write(Monday);
            writer.Write(Tuesday);
            writer.Write(Wednesday);
            writer.Write(Thursday);
            writer.Write(Friday);
            writer.Write(Saturday);
            writer.Write(Sunday);

            writer.Write(KingLocation.X);
            writer.Write(KingLocation.Y);
            writer.Write(KingSize);

            writer.Write(ControlPointIndex);
            writer.Write(ControlPoints.Count);
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                ControlPoints[i].Save(writer);
            }

        }

        public override string ToString()
        {
            return string.Format("{0}- {1}", Index, Name);
        }
    }

    public class ConquestSiegeInfo
    {
        public int Index;
        public Point Location;
        public int MobIndex;
        public string Name;
        public uint RepairCost;

        public ConquestSiegeInfo()
        {

        }
        public ConquestSiegeInfo(MySqlDataReader readerConquestSieges)
            {
            Index = Convert.ToInt32(readerConquestSieges["IndexID"]);
            Location = new Point(Convert.ToInt32(readerConquestSieges["Location_X"]), Convert.ToInt32(readerConquestSieges["Location_Y"]));
            MobIndex = Convert.ToInt32(readerConquestSieges["MobIndex"]);
            Name = Convert.ToString(readerConquestSieges["Name"]);
            RepairCost = Convert.ToUInt32(readerConquestSieges["RepairCost"]);
            }

        public ConquestSiegeInfo(BinaryReader reader)
        {
            Index = reader.ReadInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            MobIndex = reader.ReadInt32();
            Name = reader.ReadString();
            RepairCost = reader.ReadUInt32();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(MobIndex);
            writer.Write(Name);
            writer.Write(RepairCost);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})", Index, Name, Location);
        }


    }

    public class ConquestWallInfo
    {
        public int Index;
        public Point Location;
        public int MobIndex;
        public string Name;
        public uint RepairCost;

        public ConquestWallInfo()
        {

        }
        public ConquestWallInfo(MySqlDataReader readerConquestWalls)
            {
            Index = Convert.ToInt32(readerConquestWalls["IndexID"]);
            Location = new Point(Convert.ToInt32(readerConquestWalls["Location_X"]), Convert.ToInt32(readerConquestWalls["Location_Y"]));
            MobIndex = Convert.ToInt32(readerConquestWalls["MobIndex"]);
            Name = Convert.ToString(readerConquestWalls["Name"]);
            RepairCost = Convert.ToUInt32(readerConquestWalls["RepairCost"]);
            }
        public ConquestWallInfo(BinaryReader reader)
        {
            Index = reader.ReadInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            MobIndex = reader.ReadInt32();
            Name = reader.ReadString();
            RepairCost = reader.ReadUInt32();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(MobIndex);
            writer.Write(Name);
            writer.Write(RepairCost);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})", Index, Name, Location);
        }


    }

    public class ConquestGateInfo
    {
        public int Index;
        public Point Location;
        public int MobIndex;
        public string Name;
        public uint RepairCost;

        public ConquestGateInfo()
        {

        }
        public ConquestGateInfo(MySqlDataReader readerConquestGates)
            {
            Index = Convert.ToInt32(readerConquestGates["IndexID"]);
            Location = new Point(Convert.ToInt32(readerConquestGates["Location_X"]), Convert.ToInt32(readerConquestGates["Location_Y"]));
            MobIndex = Convert.ToInt32(readerConquestGates["MobIndex"]);
            Name = Convert.ToString(readerConquestGates["Name"]);
            RepairCost = Convert.ToUInt32(readerConquestGates["RepairCost"]);
            }
        public ConquestGateInfo(BinaryReader reader)
        {
            Index = reader.ReadInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            MobIndex = reader.ReadInt32();
            Name = reader.ReadString();
            RepairCost = reader.ReadUInt32();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(MobIndex);
            writer.Write(Name);
            writer.Write(RepairCost);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})", Index, Name, Location);
        }


    }

    public class ConquestArcherInfo
    {
        public int Index;
        public Point Location;
        public int MobIndex;
        public string Name;
        public uint RepairCost;

        public ConquestArcherInfo()
        {

        }
        public ConquestArcherInfo(MySqlDataReader readerConquestGuards)
            {
            Index = Convert.ToInt32(readerConquestGuards["IndexID"]);
            Location = new Point(Convert.ToInt32(readerConquestGuards["Location_X"]), Convert.ToInt32(readerConquestGuards["Location_Y"]));
            MobIndex = Convert.ToInt32(readerConquestGuards["MobIndex"]);
            Name = Convert.ToString(readerConquestGuards["Name"]);
            RepairCost = Convert.ToUInt32(readerConquestGuards["RepairCost"]);
            }

        public ConquestArcherInfo(BinaryReader reader)
        {
            Index = reader.ReadInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            MobIndex = reader.ReadInt32();
            Name = reader.ReadString();
            RepairCost = reader.ReadUInt32();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(MobIndex);
            writer.Write(Name);
            writer.Write(RepairCost);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})", Index, Name, Location);
        }


    }

    public class ConquestFlagInfo
    {
        public int Index;
        public Point Location;
        public string Name;
        public string FileName = string.Empty;

        public ConquestFlagInfo()
        {

        }

        public ConquestFlagInfo(MySqlDataReader readerConquestFlag)
            {
            Index = Convert.ToInt32(readerConquestFlag["IndexID"]);
            Location = new Point(Convert.ToInt32(readerConquestFlag["Location_X"]), Convert.ToInt32(readerConquestFlag["Location_Y"]));
            Name = readerConquestFlag["Name"].ToString();
            FileName = readerConquestFlag["FileName"].ToString();
            }

        public ConquestFlagInfo(BinaryReader reader)
        {
            Index = reader.ReadInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Name = reader.ReadString();
            FileName = reader.ReadString();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(Name);
            writer.Write(FileName);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})", Index, Name, Location);
        }
    }
}
