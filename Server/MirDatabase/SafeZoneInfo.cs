using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Server.MirDatabase
{
    public class SafeZoneInfo
    {
        public int SafeMapIndex;
        public Point Location;
        public ushort Size;
        public bool StartPoint;

        public MapInfo Info;

        public SafeZoneInfo()
        {

        }

        public SafeZoneInfo(MySqlDataReader readerSafeZoneInfo)
            {
            SafeMapIndex = Convert.ToInt32(readerSafeZoneInfo["SafeMapIndex"]);
            Location = new Point(Convert.ToInt32(readerSafeZoneInfo["Location_X"]), Convert.ToInt32(readerSafeZoneInfo["Location_Y"]));
            Size = Convert.ToUInt16(readerSafeZoneInfo["Size"]);
            StartPoint = Convert.ToBoolean(readerSafeZoneInfo["StartPoint"]);
            }

        public SafeZoneInfo(BinaryReader reader)
        {
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Size = reader.ReadUInt16();
            StartPoint = reader.ReadBoolean();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(Size);
            writer.Write(StartPoint);
        }

        public override string ToString()
        {
            return string.Format("Map: {0}- {1}", Functions.PointToString(Location), StartPoint);
        }
    }
}
