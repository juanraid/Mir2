using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Server
    {

    public partial class AccountDB : Form
        {
        private MySqlConnection connection;
        private MySqlDataAdapter mySqlDataAdapter;

        public AccountDB()
            {
            InitializeComponent();
            }
        private bool OpenConnection()
            {
            try
                {
                connection.Open();
                return true;
                }
            catch (MySqlException ex)
                {
                switch (ex.Number)
                    {
                    case 0:
                        MessageBox.Show("Cannot connect to server. Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                    default:
                        MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
                return false;
                }
            }


        private void AccountSearchButton_Click(object sender, EventArgs e)
            {
            string connectionString = "Server=" + Settings.ServerIP + "; Uid=" + Settings.Uid + "; Pwd=" + Settings.Pwd + "; convert zero datetime=True";

            connection = new MySqlConnection(connectionString);

            if (this.OpenConnection() == true)
                {
                mySqlDataAdapter = new MySqlDataAdapter("SELECT * FROM " + Settings.DBAccount + ".account where AccountID = '" + AccountFilterTextBox.Text + "'; SELECT Name FROM " + Settings.DBAccount + ".characterinfo where AccountID = '" + AccountFilterTextBox.Text + "'", connection);
                DataSet DS = new DataSet();

                 mySqlDataAdapter.Fill(DS);

                AccountInfoRow.DataSource = DS.Tables[0];
                DataGridViewRow rows = AccountInfoRow.Rows[0];
                AccountIDTextBox.Text = Convert.ToString(rows.Cells["AccountID"].Value);
                PasswordTextBox.Text = Convert.ToString(rows.Cells["Password"].Value);
                UserNameTextBox.Text = Convert.ToString(rows.Cells["UserName"].Value);
                BirthDateTextBox.Text = Convert.ToString(rows.Cells["BirthDate"].Value);
                QuestionTextBox.Text = Convert.ToString(rows.Cells["SecretQuestion"].Value);
                AnswerTextBox.Text = Convert.ToString(rows.Cells["SecretAnswer"].Value);
                EMailTextBox.Text = Convert.ToString(rows.Cells["EMailAddress"].Value);
                CreationIPTextBox.Text = Convert.ToString(rows.Cells["CreationIP"].Value);
                CreationDateTextBox.Text = Convert.ToString(rows.Cells["CreationDate"].Value);
                LastIPTextBox.Text = Convert.ToString(rows.Cells["LastIP"].Value);
                LastDateTextBox.Text = Convert.ToString(rows.Cells["LastDate"].Value);
                BanReasonTextBox.Text = Convert.ToString(rows.Cells["BanReason"].Value);
                BannedCheckBox.Checked = Convert.ToBoolean(rows.Cells["Banned"].Value);
                ExpiryDateTextBox.Text = Convert.ToString(rows.Cells["ExpiryDate"].Value);
                AdminCheckBox.Checked = Convert.ToBoolean(rows.Cells["AdminAccount"].Value);
                
                SelecCharBox.ValueMember = "Name";
                SelecCharBox.DisplayMember = "Name";
                SelecCharBox.DataSource = DS.Tables[1];
                DataAccount.Enabled = true;
                this.CloseConnection();
                }
            }

        private void SaveAccountButton_Click(object sender, EventArgs e)
            {
            DataTable changes = ((DataTable)AccountInfoRow.DataSource).GetChanges();

            if (changes != null)
                {
                MySqlCommandBuilder mcb = new MySqlCommandBuilder(mySqlDataAdapter);
                mySqlDataAdapter.UpdateCommand = mcb.GetUpdateCommand();
                mySqlDataAdapter.Update(changes);
                ((DataTable)AccountInfoRow.DataSource).AcceptChanges();
                }
            }

        private void ShowButton_Click(object sender, EventArgs e)
            {
                string connectionString = "Server=" + Settings.ServerIP + "; Uid=" + Settings.Uid + "; Pwd=" + Settings.Pwd + "; convert zero datetime=True";

            connection = new MySqlConnection(connectionString);

            if (this.OpenConnection() == true)
                {
  
                        mySqlDataAdapter = new MySqlDataAdapter("SELECT * FROM " + Settings.DBAccount + ".inventory where ChName = '" + SelecCharBox.SelectedValue.ToString() + "'; SELECT * FROM " + Settings.DBAccount + ".magics where ChName = '" + SelecCharBox.SelectedValue.ToString() + "'; SELECT * FROM " + Settings.DBAccount + ".storage WHERE AccountID = '" + AccountFilterTextBox.Text + "';SELECT * FROM " + Settings.DBAccount + ".characterinfo where Name = '" + SelecCharBox.SelectedValue.ToString() + "'", connection);
                        DataSet DS = new DataSet();
                        mySqlDataAdapter.Fill(DS);
                
                InventoryGridView1.DataSource = DS.Tables[0];
                MagiasdataGridView1.DataSource = DS.Tables[1];
                StorageGridView1.DataSource = DS.Tables[2];
                CharacterGridView1.DataSource = DS.Tables[3];
                CharacterGrid();

                MagiasdataGridView1.Columns["ChName"].Visible = false;

                for (int i = 0; i < InventoryGridView1.Columns.Count; i++)
                    {
                    if (InventoryGridView1.Columns[i].Name != "ItemName" && InventoryGridView1.Columns[i].Name != "ItemIndex" && InventoryGridView1.Columns[i].Name != "Store")
                        InventoryGridView1.Columns[i].Visible = false;
                    }

                InventoryGridView1.AutoResizeColumns();
                InventoryGridView1.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.AllCells;

                for (int i = 0; i < StorageGridView1.Columns.Count; i++)
                    {
                    if (StorageGridView1.Columns[i].Name != "ItemName" && StorageGridView1.Columns[i].Name != "ItemIndex" && StorageGridView1.Columns[i].Name != "Store")
                        StorageGridView1.Columns[i].Visible = false;
                    }

                StorageGridView1.AutoResizeColumns();
                StorageGridView1.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.AllCells;

                this.CloseConnection();
                }

            tabControl1.Enabled = true;
           
            }
        private void InventoryGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
            {
            
            DataGridViewRow rows = InventoryGridView1.CurrentRow;
            NameInventtextBox.Text = Convert.ToString(rows.Cells["ItemName"].Value);
            UniqueIDTextBox.Text = Convert.ToString(rows.Cells["UniqueID"].Value);
            ItemIndexTextBox.Text = Convert.ToString(rows.Cells["ItemIndex"].Value);
            CurrentDuraTextBox.Text = Convert.ToString(rows.Cells["CurrentDura"].Value);
            MaxDuraTextBox.Text = Convert.ToString(rows.Cells["MaxDura"].Value);
            CountTextBox.Text = Convert.ToString(rows.Cells["Count"].Value);
            ACTextBox.Text = Convert.ToString(rows.Cells["AC"].Value);
            MACTextBox.Text = Convert.ToString(rows.Cells["MAC"].Value);
            DCTextBox.Text = Convert.ToString(rows.Cells["DC"].Value);
            MCTextBox.Text = Convert.ToString(rows.Cells["MC"].Value);
            SCTextBox.Text = Convert.ToString(rows.Cells["SC"].Value);
            AccuracyTextBox.Text = Convert.ToString(rows.Cells["Accuracy"].Value);
            AgilityTextBox.Text = Convert.ToString(rows.Cells["Agility"].Value);
            HPTextBox.Text = Convert.ToString(rows.Cells["HP"].Value);
            MPTextBox.Text = Convert.ToString(rows.Cells["MP"].Value);
            AttackSpeedTextBox.Text = Convert.ToString(rows.Cells["AttackSpeed"].Value);
            LuckTextBox.Text = Convert.ToString(rows.Cells["Luck"].Value);
            SoulBoundIdTextBox.Text = Convert.ToString(rows.Cells["SoulBoundId"].Value);
            StrongTextBox.Text = Convert.ToString(rows.Cells["Strong"].Value);
            MagicResistTextBox.Text = Convert.ToString(rows.Cells["MagicResist"].Value);
            PoisonResistTextBox.Text = Convert.ToString(rows.Cells["PoisonResist"].Value);
            HealthRecoveryTextBox.Text = Convert.ToString(rows.Cells["HealthRecovery"].Value);
            ManaRecoveryTextBox.Text = Convert.ToString(rows.Cells["ManaRecovery"].Value);
            PoisonRecoveryTextBox.Text = Convert.ToString(rows.Cells["PoisonRecovery"].Value);
            CriticalRateTextBox.Text = Convert.ToString(rows.Cells["CriticalRate"].Value);
            CriticalDamageTextBox.Text = Convert.ToString(rows.Cells["CriticalDamage"].Value);
            FreezingTextBox.Text = Convert.ToString(rows.Cells["Freezing"].Value);
            PoisonAttackTextBox.Text = Convert.ToString(rows.Cells["PoisonAttack"].Value);
            IdentifiedtextBox.Text = Convert.ToString(rows.Cells["Identified"].Value);
            CursedtextBox.Text = Convert.ToString(rows.Cells["Cursed"].Value);

            GemCountTextBox.Text = Convert.ToString(rows.Cells["GemCount"].Value);
            RefinedValueTextBox.Text = Convert.ToString(rows.Cells["RefinedValue"].Value);
            RefineAddedextBox.Text = Convert.ToString(rows.Cells["RefineAdded"].Value);
            WeddingRingTextBox.Text = Convert.ToString(rows.Cells["WeddingRing"].Value);

            IsAwaketextBox.Checked = Convert.ToBoolean(rows.Cells["IsAwake"].Value);
            AwakeTypetextBox.Text = Convert.ToString(rows.Cells["AwakeType"].Value);
            IsAttachedcheckbox.Checked = Convert.ToBoolean(rows.Cells["IsAttached"].Value);
            AttachedTextBox.Text = Convert.ToString(rows.Cells["Attached"].Value);
            PositionTextBox.Text = Convert.ToString(rows.Cells["Position"].Value);
            StoreTextBox.Text = Convert.ToString(rows.Cells["Store"].Value);

            }

        private void StorageGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
            {

            DataGridViewRow rows = StorageGridView1.CurrentRow;
            NameInventtextBox1.Text = Convert.ToString(rows.Cells["ItemName"].Value);
            UniqueIDTextBox1.Text = Convert.ToString(rows.Cells["UniqueID"].Value);
            ItemIndexTextBox1.Text = Convert.ToString(rows.Cells["ItemIndex"].Value);
            CurrentDuraTextBox1.Text = Convert.ToString(rows.Cells["CurrentDura"].Value);
            MaxDuraTextBox1.Text = Convert.ToString(rows.Cells["MaxDura"].Value);
            CountTextBox1.Text = Convert.ToString(rows.Cells["Count"].Value);
            ACTextBox1.Text = Convert.ToString(rows.Cells["AC"].Value);
            MACTextBox1.Text = Convert.ToString(rows.Cells["MAC"].Value);
            DCTextBox1.Text = Convert.ToString(rows.Cells["DC"].Value);
            MCTextBox1.Text = Convert.ToString(rows.Cells["MC"].Value);
            SCTextBox1.Text = Convert.ToString(rows.Cells["SC"].Value);
            AccuracyTextBox1.Text = Convert.ToString(rows.Cells["Accuracy"].Value);
            AgilityTextBox1.Text = Convert.ToString(rows.Cells["Agility"].Value);
            HPTextBox1.Text = Convert.ToString(rows.Cells["HP"].Value);
            MPTextBox1.Text = Convert.ToString(rows.Cells["MP"].Value);
            AttackSpeedTextBox1.Text = Convert.ToString(rows.Cells["AttackSpeed"].Value);
            LuckTextBox1.Text = Convert.ToString(rows.Cells["Luck"].Value);
            SoulBoundIdTextBox1.Text = Convert.ToString(rows.Cells["SoulBoundId"].Value);
            StrongTextBox1.Text = Convert.ToString(rows.Cells["Strong"].Value);
            MagicResistTextBox1.Text = Convert.ToString(rows.Cells["MagicResist"].Value);
            PoisonResistTextBox1.Text = Convert.ToString(rows.Cells["PoisonResist"].Value);
            HealthRecoveryTextBox1.Text = Convert.ToString(rows.Cells["HealthRecovery"].Value);
            ManaRecoveryTextBox1.Text = Convert.ToString(rows.Cells["ManaRecovery"].Value);
            PoisonRecoveryTextBox1.Text = Convert.ToString(rows.Cells["PoisonRecovery"].Value);
            CriticalRateTextBox1.Text = Convert.ToString(rows.Cells["CriticalRate"].Value);
            CriticalDamageTextBox1.Text = Convert.ToString(rows.Cells["CriticalDamage"].Value);
            FreezingTextBox1.Text = Convert.ToString(rows.Cells["Freezing"].Value);
            PoisonAttackTextBox1.Text = Convert.ToString(rows.Cells["PoisonAttack"].Value);
            IdentifiedtextBox1.Text = Convert.ToString(rows.Cells["Identified"].Value);
            CursedtextBox1.Text = Convert.ToString(rows.Cells["Cursed"].Value);

            GemCountTextBox1.Text = Convert.ToString(rows.Cells["GemCount"].Value);
            RefinedValueTextBox1.Text = Convert.ToString(rows.Cells["RefinedValue"].Value);
            RefineAddedextBox1.Text = Convert.ToString(rows.Cells["RefineAdded"].Value);
            WeddingRingTextBox1.Text = Convert.ToString(rows.Cells["WeddingRing"].Value);

            IsAwaketextBox1.Checked = Convert.ToBoolean(rows.Cells["IsAwake"].Value);
            AwakeTypetextBox1.Text = Convert.ToString(rows.Cells["AwakeType"].Value);
            IsAttachedcheckbox.Checked = Convert.ToBoolean(rows.Cells["IsAttached"].Value);
            AttachedTextBox1.Text = Convert.ToString(rows.Cells["Attached"].Value);
            PositionTextBox1.Text = Convert.ToString(rows.Cells["Position"].Value);
            StoreTextBox1.Text = Convert.ToString(rows.Cells["Store"].Value);

            }

        private void CharacterGrid()
            {

            DataGridViewRow rows = CharacterGridView1.Rows[0];
            
            IndexIDTextBox.Text = Convert.ToString(rows.Cells["IndexID"].Value);
            NameTextBox.Text = Convert.ToString(rows.Cells["Name"].Value);
            LevelTextBox.Text = Convert.ToString(rows.Cells["Level"].Value);
            ClassComboBox.ValueMember = "Class";
            ClassComboBox.DisplayMember = "Class";
            GenderComboBox.ValueMember = "Gender";
            GenderComboBox.DisplayMember = "Gender";
            HairtextBox.Text = Convert.ToString(rows.Cells["Hair"].Value);
            InventoryResizetextBox.Text = Convert.ToString(rows.Cells["InventoryResize"].Value);
            CreationIPtextBox1.Text = Convert.ToString(rows.Cells["CreationIP"].Value);
            CreationDatetextBox1.Text = Convert.ToString(rows.Cells["CreationDate"].Value);
            BannedcheckBox1.Checked = Convert.ToBoolean(rows.Cells["Banned"].Value);
            BanReasontextBox1.Text = Convert.ToString(rows.Cells["BanReason"].Value);
            ExpiryDatetextBox1.Text = Convert.ToString(rows.Cells["ExpiryDate"].Value);
            LastIPtextBox1.Text = Convert.ToString(rows.Cells["LastIP"].Value);
            LastDatetextBox1.Text = Convert.ToString(rows.Cells["LastDate"].Value);
            DeletedcheckBox.Checked = Convert.ToBoolean(rows.Cells["Deleted"].Value);
            DeleteDatetextBox.Text = Convert.ToString(rows.Cells["DeleteDate"].Value);
            CurrentMapIndexTextBox.Text = Convert.ToString(rows.Cells["CurrentMapIndex"].Value);
            CurrentLocation_XTextBox.Text = Convert.ToString(rows.Cells["CurrentLocation_X"].Value);
            CurrentLocation_YTextBox.Text = Convert.ToString(rows.Cells["CurrentLocation_Y"].Value);
            DirectiontextBox.Text = Convert.ToString(rows.Cells["Direction"].Value);
            BindMapIndex.Text = Convert.ToString(rows.Cells["BindMapIndex"].Value);
            BindLocation_X.Text = Convert.ToString(rows.Cells["BindLocation_X"].Value);
            BindLocation_Y.Text = Convert.ToString(rows.Cells["BindLocation_Y"].Value);
            HPtextBox11.Text = Convert.ToString(rows.Cells["HP"].Value);
            MPtextBox11.Text = Convert.ToString(rows.Cells["MP"].Value);
            ExperiencieTextBox.Text = Convert.ToString(rows.Cells["Experience"].Value);
            AModetextBox.Text = Convert.ToString(rows.Cells["AMode"].Value);
            PModetextBox1.Text = Convert.ToString(rows.Cells["PMode"].Value);
            PKPointsTextBox.Text = Convert.ToString(rows.Cells["PKPoints"].Value);
            ThrustingcheckBox.Checked = Convert.ToBoolean(rows.Cells["Thrusting"].Value);
            HalfMooncheckBox.Checked = Convert.ToBoolean(rows.Cells["HalfMoon"].Value);
            CrossHalfMooncheckBox.Checked = Convert.ToBoolean(rows.Cells["CrossHalfMoon"].Value);
            DoubleSlashcheckBox.Checked = Convert.ToBoolean(rows.Cells["DoubleSlash"].Value);
            MentalStatetextBox.Text = Convert.ToString(rows.Cells["MentalState"].Value);
            AllowGroupcheckBox.Checked = Convert.ToBoolean(rows.Cells["AllowGroup"].Value);
            AllowTradecheckBox.Checked = Convert.ToBoolean(rows.Cells["AllowTrade"].Value);
            MarriedtextBox.Text = Convert.ToString(rows.Cells["Married"].Value);
            MarriedDatetextBox.Text = Convert.ToString(rows.Cells["MarriedDate"].Value);
            MarriedNametextBox.Text = Convert.ToString(rows.Cells["MarriedName"].Value);
            MentorTextBox.Text = Convert.ToString(rows.Cells["Mentor"].Value);
            MentorDateTextBox.Text = Convert.ToString(rows.Cells["MentorDate"].Value);
            isMentorcheckBox.Checked = Convert.ToBoolean(rows.Cells["isMentor"].Value);
            MentorExptextBox.Text = Convert.ToString(rows.Cells["MentorExp"].Value);
            MentorNametextBox.Text = Convert.ToString(rows.Cells["MentorName"].Value);
            PearlCounttextBox.Text = Convert.ToString(rows.Cells["PearlCount"].Value);
            CollectTimetextBox.Text = Convert.ToString(rows.Cells["CollectTime"].Value);
            GuildIndextextBox1.Text = Convert.ToString(rows.Cells["GuildIndex"].Value);

            }

        private void groupBox1_Enter(object sender, EventArgs e)
            {

            }
        }
    }
