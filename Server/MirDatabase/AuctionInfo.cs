using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Server.MirDatabase
    {
    public class AuctionInfo
        {
        public ulong AuctionID;
        public string NameSeller;
        public UserItem Item;
        public DateTime ConsignmentDate;
        public uint Price;

        public int CharacterIndex;
        public CharacterInfo CharacterInfo;

        public bool Expired, Sold;

        public AuctionInfo()
            {

            }
        public AuctionInfo(MySqlDataReader readerAuctionsDB)
            {
            AuctionID = Convert.ToUInt64(readerAuctionsDB["AuctionID"]);
            NameSeller = Convert.ToString(readerAuctionsDB["NameSeller"]);

            try
                {

                MySqlConnection connection = new MySqlConnection(); //star conection 
                String connectionString;
                connectionString = "Server=" + Settings.ServerIP + "; Uid=" + Settings.Uid + "; Pwd=" + Settings.Pwd + "; convert zero datetime=True";
                connection.ConnectionString = connectionString;
                connection.Open();

                MySqlCommand instruccion = connection.CreateCommand();

                instruccion.CommandText = "SELECT * FROM " + Settings.DBAccount + ".auctionsitems WHERE AuctionID = '" + AuctionID + "'";

                MySqlDataReader readerAuctions = instruccion.ExecuteReader();

                while (readerAuctions.Read())
                    {
                    UserItem AddItem = new UserItem(readerAuctions);

                    if (SMain.Envir.BindItem(AddItem))
                        {
                        Item = AddItem;
                        }
                    }

                readerAuctions.Dispose();
                if (Item != null)
                    {
                    if (Item.IsAwake)
                        {

                        MySqlCommand instruccionAwake = connection.CreateCommand();

                        instruccionAwake.CommandText = "SELECT * FROM " + Settings.DBAccount + ".awake WHERE UniqueID = '" + Item.UniqueID + "' ORDER BY Position";

                        MySqlDataReader readerAwakeDB = instruccionAwake.ExecuteReader();

                        Item.Awake = new Awake();
                        Item.Awake.type = (AwakeType)Convert.ToInt32(Item.AwakeType);

                        while (readerAwakeDB.Read())
                            {
                            Item.Awake.listAwake.Add(Convert.ToByte(readerAwakeDB["Value"]));
                            }

                        readerAwakeDB.Dispose();
                        }
                    if (Item.IsAttached)
                        {

                        MySqlCommand instruccionAttached = connection.CreateCommand();

                        instruccionAttached.CommandText = "SELECT * FROM " + Settings.DBAccount + ".auctionsitems WHERE Attached = '" + Item.UniqueID + "' ORDER BY Position";

                        MySqlDataReader readerAttachedDB = instruccionAttached.ExecuteReader();

                        Item.Awake = new Awake();
                        Item.Awake.type = (AwakeType)Convert.ToInt32(Item.AwakeType);

                        while (readerAttachedDB.Read())
                            {
                            Item.Awake.listAwake.Add(Convert.ToByte(readerAttachedDB["Value"]));
                            }

                        readerAttachedDB.Dispose();
                        }
                    }
                connection.Close();

                }
            catch (MySqlException ex)
                {
                SMain.Enqueue(ex);
                }



            ConsignmentDate = readerAuctionsDB.GetDateTime(readerAuctionsDB.GetOrdinal("ConsignmentDate"));
            Price = Convert.ToUInt32(readerAuctionsDB["Price"]);

            CharacterIndex = Convert.ToInt32(readerAuctionsDB["CharacterIndex"]);

            Expired = Convert.ToBoolean(readerAuctionsDB["Expired"]);
            Sold = Convert.ToBoolean(readerAuctionsDB["Sold"]);
            }

        public AuctionInfo(BinaryReader reader, int version, int customversion)
            {
            AuctionID = reader.ReadUInt64();

            Item = new UserItem(reader, version, customversion);
            ConsignmentDate = DateTime.FromBinary(reader.ReadInt64());
            Price = reader.ReadUInt32();

            CharacterIndex = reader.ReadInt32();

            Expired = reader.ReadBoolean();
            Sold = reader.ReadBoolean();
            }

        public void Save(BinaryWriter writer)
            {
            writer.Write(AuctionID);

            Item.Save(writer);
            writer.Write(ConsignmentDate.ToBinary());
            writer.Write(Price);

            writer.Write(CharacterIndex);

            writer.Write(Expired);
            writer.Write(Sold);

            }

        public ClientAuction CreateClientAuction(bool userMatch)
            {
            return new ClientAuction
                {
                AuctionID = AuctionID,
                Item = Item,
                Seller = userMatch ? (Sold ? "Sold" : (Expired ? "Expired" : "For Sale")) : NameSeller,
                Price = Price,
                ConsignmentDate = ConsignmentDate,
                };
            }
        }
    }
