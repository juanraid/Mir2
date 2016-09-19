using Server.MirDatabase;
using Server.MirObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace Server.MirEnvir
{
    public class MailInfo
    {
        public ulong MailID;

        public string Sender;

        public int RecipientIndex;
        public CharacterInfo RecipientInfo;

        public string Message = string.Empty;
        public uint Gold = 0;
        public List<UserItem> Items = new List<UserItem>();
        public int ItemCount;
        public DateTime DateSent, DateOpened;

        public bool Sent
        {
            get { return DateSent > DateTime.MinValue; }
        }

        public bool Opened
        {
            get { return DateOpened > DateTime.MinValue; }
        }

        public bool Locked;

        public bool Collected;

        public bool Parcel //parcel if item contains gold or items.
        {
            get { return Gold > 0 || Items.Count > 0; }
        }

        public bool CanReply;

        public MailInfo(int recipientIndex, bool canReply = false)
        {
            MailID = ++SMain.Envir.NextMailID;
            RecipientIndex = recipientIndex;

            CanReply = canReply;

            string Update = "UPDATE " + Settings.DBAccount + ".generalcount SET NextMailID = '" + MailID + "'  WHERE IndexID = '1'";

            Envir.ConnectADB.Update(Update);

            }
        public MailInfo(MySqlDataReader readerMail)
            {
            MailID = Convert.ToUInt32(readerMail["MailID"]);
            Sender = readerMail["Sender"].ToString();
            RecipientIndex = Convert.ToInt32(readerMail["RecipientIndex"]);
            Message = readerMail["Message"].ToString();
            Gold = Convert.ToUInt32(readerMail["Gold"]);
            ItemCount = Convert.ToInt32(readerMail["ItemCount"]);
            if (ItemCount > 0) { 
            try
                {

                MySqlConnection connection = new MySqlConnection(); //star conection 
                String connectionString;
                connectionString = "Server=" + Settings.ServerIP + "; Uid=" + Settings.Uid + "; Pwd=" + Settings.Pwd + "; convert zero datetime=True";
                connection.ConnectionString = connectionString;
                connection.Open();

                MySqlCommand instruccion = connection.CreateCommand();

                instruccion.CommandText = "SELECT * FROM " + Settings.DBAccount + ".mailitems WHERE MailID = '" + MailID + "'";

                MySqlDataReader readerAuctions = instruccion.ExecuteReader();

                while (readerAuctions.Read())
                    {
                    UserItem AddItem = new UserItem(readerAuctions);

                    if (SMain.Envir.BindItem(AddItem))
                        {
                            Items.Add(AddItem);
                        }
                    }

                readerAuctions.Dispose();

                    for (int i = 0; i < Items.Count; i++)
                        {
                    if (Items[i].IsAwake)
                        {

                        MySqlCommand instruccionAwake = connection.CreateCommand();

                        instruccionAwake.CommandText = "SELECT * FROM " + Settings.DBAccount + ".mailitems WHERE UniqueID = '" + Items[i].UniqueID + "' ORDER BY Position";

                        MySqlDataReader readerAwakeDB = instruccionAwake.ExecuteReader();

                            Items[i].Awake = new Awake();
                            Items[i].Awake.type = (AwakeType)Convert.ToInt32(Items[i].AwakeType);

                        while (readerAwakeDB.Read())
                            {
                                Items[i].Awake.listAwake.Add(Convert.ToByte(readerAwakeDB["Value"]));
                            }

                        readerAwakeDB.Dispose();
                        }
                    if (Items[i].IsAttached)
                        {

                        MySqlCommand instruccionAttached = connection.CreateCommand();

                        instruccionAttached.CommandText = "SELECT * FROM " + Settings.DBAccount + ".mailitems WHERE Attached = '" + Items[i].UniqueID + "' ORDER BY Position";

                        MySqlDataReader readerAttachedDB = instruccionAttached.ExecuteReader();

                            Items[i].Awake = new Awake();
                            Items[i].Awake.type = (AwakeType)Convert.ToInt32(Items[i].AwakeType);

                        while (readerAttachedDB.Read())
                            {
                                Items[i].Awake.listAwake.Add(Convert.ToByte(readerAttachedDB["Value"]));
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

                }
        

            DateSent = readerMail.GetDateTime(readerMail.GetOrdinal("DateSent"));
            DateOpened = readerMail.GetDateTime(readerMail.GetOrdinal("DateOpened"));

            Locked = Convert.ToBoolean(readerMail["Locked"]);
            Collected = Convert.ToBoolean(readerMail["Collected"]);
            CanReply = Convert.ToBoolean(readerMail["CanReply"]);
            }

        public MailInfo(BinaryReader reader, int version, int customversion)
        {
            MailID = reader.ReadUInt64();
            Sender = reader.ReadString();
            RecipientIndex = reader.ReadInt32();
            Message = reader.ReadString();
            Gold = reader.ReadUInt32();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                UserItem item = new UserItem(reader, version, customversion);
                if (SMain.Envir.BindItem(item))
                    Items.Add(item);
            }

            DateSent = DateTime.FromBinary(reader.ReadInt64());
            DateOpened = DateTime.FromBinary(reader.ReadInt64());

            Locked = reader.ReadBoolean();
            Collected = reader.ReadBoolean();
            CanReply = reader.ReadBoolean();
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(MailID);
            writer.Write(Sender);
            writer.Write(RecipientIndex);
            writer.Write(Message);
            writer.Write(Gold);

            writer.Write(Items.Count);
            for (int i = 0; i < Items.Count; i++)
                Items[i].Save(writer);

            writer.Write(DateSent.ToBinary());
            writer.Write(DateOpened.ToBinary());

            writer.Write(Locked);
            writer.Write(Collected);
            writer.Write(CanReply);
        }

        public void Send()
        {
            if (Sent) return;

            Collected = true;

            if (Parcel)
            {
                if(Items.Count > 0 && Gold > 0)
                {
                    if(!Settings.MailAutoSendGold || !Settings.MailAutoSendItems)
                    {
                        Collected = false;
                    }
                }
                if(Items.Count > 0)
                {
                    if (!Settings.MailAutoSendItems)
                    {
                        Collected = false;
                    }
                }
                else
                {
                    if (!Settings.MailAutoSendGold)
                    {
                        Collected = false;
                    }
                }
            }

            if (SMain.Envir.Mail.Contains(this)) return;

            SMain.Envir.Mail.Add(this); //add to postbox

            DateSent = DateTime.Now;
            DateTime theDate = DateSent;
            DateTime theDate1 = DateOpened;

            string sqlMail = "INSERT INTO  " + Settings.DBAccount + ".mail ( MailID, Sender, RecipientIndex, Message, Gold, DateSent, DateOpened, Locked, Collected, CanReply, ItemCount) VALUES ('" + MailID + "', '" + Sender + "', '" + RecipientIndex + "', '" + Message + "', '" + Gold + "', '" + theDate.ToString("yyyy-MM-dd H:mm:ss").ToString() + "', '" + theDate1.ToString("yyyy-MM-dd H:mm:ss").ToString() + "', '" + Convert.ToInt32(Locked) + "', '" + Convert.ToInt32(Collected) + "', '" + Convert.ToInt32(CanReply) + "', '"+ Items.Count +"')";
            Envir.ConnectADB.Insert(sqlMail);
            
            }

        public bool Receive()
        {
            if (!Sent) return false; //mail not sent yet

            if (RecipientInfo == null)
            {
                RecipientInfo = SMain.Envir.GetCharacterInfo(RecipientIndex);

                if (RecipientInfo == null) return false;
            }

            RecipientInfo.Mail.Add(this); //add to players inbox
            
            if(RecipientInfo.Player != null)
            {
                RecipientInfo.Player.NewMail = true; //notify player of new mail  --check in player process
            }

            SMain.Envir.Mail.Remove(this); //remove from postbox

            return true;
        }

        public ClientMail CreateClientMail()
        {
            return new ClientMail
            {
                MailID = MailID,
                SenderName = Sender,
                Message = Message,
                Locked = Locked,
                CanReply = CanReply,
                Gold = Gold,
                Items = Items,
                Opened = Opened,
                Collected = Collected,
                DateSent = DateSent
            };
        }
    }

    // player bool NewMail (process in envir loop) - send all mail on login

    // Send mail from player (auto from player)
    // Send mail from Envir (mir administrator)
}
