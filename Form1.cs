using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace ListView_Small_Project
{

    public partial class Form1 : Form
    {

        short CountContact = 0;
        List<ListViewItem> LsContactsItems = new List<ListViewItem>();

        string filePath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "Contacts.txt");

        public Form1()
        {
            InitializeComponent();

            LoadContacts();

            imageList1.Images[0] = Properties.Resources.icons8_user_16;
            imageList2.Images[0] = Properties.Resources.icons8_user_32;
        }

        bool IsEmptyText(string Text)
        {
            return string.IsNullOrEmpty(Text);
        }

        void AddSubItemToListView(ListViewItem Item, string TextToAdd)
        {
            Item.SubItems.Add(TextToAdd.Trim());
        }

        private void AddContact()
        {
            ListViewItem item = new ListViewItem(txtFullName.Text.Trim());
            item.ImageIndex = 0;

            AddSubItemToListView(item, txtPhNumber.Text);
            AddSubItemToListView(item, txtEmail.Text);
            AddSubItemToListView(item, txtCity.Text);


            AddSubItemToListView(item, rbMale.Checked ? rbMale.Text : rbFemale.Text);

            LsContactsItems.Add(item);
            lvContacts.Items.Add(item);

            CountContact++;
            lbTotalContact.Text = CountContact.ToString();


            SetAddModeAndResetInfo();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (IsEmptyText(txtFullName.Text) || IsEmptyText(txtEmail.Text)
             || IsEmptyText(txtCity.Text) || IsEmptyText(txtPhNumber.Text)
             || (!rbMale.Checked && !rbFemale.Checked)) return;

            if (btnAdd.Tag.ToString() == "Edit")
            {
                AcceptChange();
                SetAddModeAndResetInfo();

                btnEdit.Enabled = true;

                MessageBox.Show("Edit Completed.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return;
            }

            AddContact();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvContacts.SelectedItems.Count == 0)
                return;

            ListViewItem item = lvContacts.SelectedItems[0];

            LsContactsItems.Remove(item);
            lvContacts.Items.Remove(item);

            CountContact--;
            lbTotalContact.Text = CountContact.ToString();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (lvContacts.Items.Count > 0)
            {
                LsContactsItems.Clear();
                lvContacts.Items.Clear();
                CountContact = 0;
                lbTotalContact.Text = "0";

                File.Delete(filePath);
            }
        }

        private void cbDetails_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbDetails.SelectedIndex)
            {
                case 0:
                    lvContacts.View = View.Details;
                    break;
                case 1:
                    lvContacts.View = View.LargeIcon;
                    break;
                case 2:
                    lvContacts.View = View.SmallIcon;
                    break;
                case 3:
                    lvContacts.View = View.List;
                    break;
                case 4:
                    lvContacts.View = View.Tile;
                    break;
                default:
                    lvContacts.View = View.Details;
                    break;
            }
        }

        private void AddToViewContactsSearchResults()
        {
            lvContacts.Items.Clear();

            foreach (ListViewItem item in LsContactsItems)
            {
                if (item.Text == txtSearch.Text)
                {
                    lvContacts.Items.Add(item);
                }
            }
        }

        private void AddToViewContactsLastContacts()
        {
            lvContacts.Items.Clear();

            foreach (ListViewItem item in LsContactsItems)
            {
                lvContacts.Items.Add(item);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
                AddToViewContactsSearchResults();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
                AddToViewContactsLastContacts();
        }

        private void SetEditModeAndSetInfo()
        {
            btnEdit.Enabled = false;

            btnAdd.Image = Properties.Resources.icons8_edit_32;
            btnAdd.Text = "         Edit";
            ListViewItem item = lvContacts.SelectedItems[0];

            txtFullName.Text = item.Text;
            txtPhNumber.Text = item.SubItems[1].Text;
            txtEmail.Text = item.SubItems[2].Text;
            txtCity.Text = item.SubItems[3].Text;

            if (item.SubItems[4].Text == "Male")
                rbMale.Checked = true;
            else
                rbFemale.Checked = true;

            btnAdd.Tag = "Edit";
        }

        private void SetAddModeAndResetInfo()
        {
            btnAdd.Image = Properties.Resources.icons8_add_user_48;
            btnAdd.Text = "          Add";

            txtFullName.Clear();
            txtPhNumber.Clear();
            txtEmail.Clear();
            txtCity.Clear();
            rbFemale.Checked = false;
            rbMale.Checked = false;
            txtFullName.Focus();

            btnAdd.Tag = "Add";
        }

        private void AcceptChange()
        {
            ListViewItem item = lvContacts.SelectedItems[0];

            item.SubItems[0].Text = txtFullName.Text;
            item.SubItems[1].Text = txtPhNumber.Text;
            item.SubItems[2].Text = txtEmail.Text;
            item.SubItems[3].Text = txtCity.Text;

            item.SubItems[4].Text = rbMale.Checked ? "Male" : "Female";

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvContacts.SelectedItems.Count == 0)
                return;

            if (btnAdd.Tag.ToString() == "Add")
            {
                SetEditModeAndSetInfo();
            }
        }

        private void SaveContacts()
        {
            List<string> lstLines = new List<string>();

            foreach (ListViewItem item in LsContactsItems)
            {
                string Line = string.Join("|", item.Text, item.SubItems[1].Text
                    , item.SubItems[2].Text, item.SubItems[3].Text, item.SubItems[4].Text.Trim()); ;

                lstLines.Add(Line);
            }

            File.WriteAllLines(filePath, lstLines);
        }

        private void LoadContacts()
        {
            if (!File.Exists(filePath)) return;

            string[] lstLines = File.ReadAllLines(filePath);

            lvContacts.Items.Clear();
            LsContactsItems.Clear();


            foreach (string L in lstLines)
            {
                string[] Info = L.Split('|');

                ListViewItem item = new ListViewItem(Info[0]);

                item.SubItems.Add(Info[1]);
                item.SubItems.Add(Info[2]);
                item.SubItems.Add(Info[3]);
                item.SubItems.Add(Info[4]);

                lvContacts.Items.Add(item);
                LsContactsItems.Add(item);
            }

            CountContact = (short)LsContactsItems.Count;
            lbTotalContact.Text = CountContact.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveContacts();

            MessageBox.Show("Contacts Saved Successfully", "",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtFullName_Validating(object sender, CancelEventArgs e)
        {
            TextBox text = (TextBox)sender;
            if (string.IsNullOrEmpty(text.Text))
            {
                errorProv.SetError(text, "You Should Enter a Value.");
            }
            else
            errorProv.SetError(text, "");
        }

        private void LbGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LbGitHub.LinkVisited = true;
            System.Diagnostics.Process.Start(@"https://github.com/Aboudx");
        }

        private void lbLinkedIn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lbLinkedIn.LinkVisited = true;
            System.Diagnostics.Process.Start(@"https://www.linkedin.com/in/abdullatif-al-muhammad-134425418");
        }

        private void btnGitHub_Click(object sender, EventArgs e)
        {
            LbGitHub.LinkVisited = true;
            System.Diagnostics.Process.Start(@"https://github.com/Aboudx");
        }

        private void btnLinkedIn_Click(object sender, EventArgs e)
        {
            lbLinkedIn.LinkVisited = true;
            System.Diagnostics.Process.Start(@"https://www.linkedin.com/in/abdullatif-al-muhammad-134425418");
        }
    }
}