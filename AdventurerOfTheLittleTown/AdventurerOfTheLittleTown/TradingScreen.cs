using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;

namespace AdventurerOfTheLittleTown
{
    public partial class TradingScreen : Form
    {
        public Player CurrentPlayer { get; set; }

        private Player _currentPlayer;

        public TradingScreen(Player player)
        {
            _currentPlayer = player;

            InitializeComponent();

            // Style, to display numeric column values
            DataGridViewCellStyle rightAlignedCellStyle = new DataGridViewCellStyle();
            rightAlignedCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Populate the datagrid for the player's inventory
            dgvMyItems.RowHeadersVisible = false;
            dgvMyItems.AutoGenerateColumns = false;

            // This hidden column holds the item ID, so we know which item to sell
            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 85,
                DataPropertyName = "Description"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Qty",
                Width = 30,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Quantity"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                Width = 30,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Price"
            });

            dgvMyItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Weight",
                Width = 30,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Weight"
            });

            dgvMyItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Sell 1",
                UseColumnTextForButtonValue = true,
                Width = 40,
                DataPropertyName = "ItemID"
            });
            
            // Bind the player's inventory to the datagridview
            dgvMyItems.DataSource = _currentPlayer.Inventory;

            // When the user clicks on a row, call this function
            dgvMyItems.CellClick += dgvMyItems_CellClick;


            // Populate the datagrid for the vendor's inventory
            dgvVendorItems.RowHeadersVisible = false;
            dgvVendorItems.AutoGenerateColumns = false;

            // This hidden column holds the item ID, so we know which item to sell
            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ItemID",
                Visible = false
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 100,
                DataPropertyName = "Description"
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Price",
                Width = 45,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Price"
            });

            dgvVendorItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Weight",
                Width = 45,
                DefaultCellStyle = rightAlignedCellStyle,
                DataPropertyName = "Weight"
            });

            dgvVendorItems.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "Buy 1",
                UseColumnTextForButtonValue = true,
                Width = 50,
                DataPropertyName = "ItemID"
            });

            // Bind the vendor's inventory to the datagridview
            dgvVendorItems.DataSource = _currentPlayer.CurrentLocation.VendorWorkingHere.Inventory;

            // When the user clicks on a row, call this function
            dgvVendorItems.CellClick += dgvVendorItems_CellClick;
        }

        private void dgvMyItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // The first column of a datagridview has a ColumnIndex = 0
            // This is known as a "zero-based" array/collection/list.
            // You start counting with 0.
            //
            // The 5th column (ColumnIndex = 4) is the column with the button.
            // So, if the player clicked the button column, we will sell an item from that row.
            if (e.ColumnIndex == 5)
            {
                // This gets the ID value of the item, from the hidden 1st column
                // Remember, ColumnIndex = 0, for the first column
                var itemID = dgvMyItems.Rows[e.RowIndex].Cells[0].Value;

                // Get the Item object for the selected item row
                Item itemBeingSold = World.ItemByID(Convert.ToInt32(itemID));

                if (itemBeingSold.Price == World.UNSELLABLE_ITEM_PRICE)
                {
                    MessageBox.Show("You cannot sell the " + itemBeingSold.Name);
                }
                else
                {
                    // Remove one of these items from the player's inventory
                    _currentPlayer.RemoveItemFromInventory(itemBeingSold);

                    // Give the player the gold for the item being sold.
                    _currentPlayer.Gold += itemBeingSold.Price;

                    // Update Player's current weight
                    PlayerCurrentWeight();
                }               
            }                       
        }

        private void dgvVendorItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // The 4th column (ColumnIndex = 3) has the "Buy 1" button.
            if (e.ColumnIndex == 4)
            {
                if (_currentPlayer.CurrentWeight >= _currentPlayer.MaxWeight)
                {
                    MessageBox.Show("You don't have enough space to buy items!");
                }
                else
                {
                    // This gets the ID value of the item, from the hidden 1st column
                    var itemID = dgvVendorItems.Rows[e.RowIndex].Cells[0].Value;

                    // Get the Item object for the selected item row
                    Item itemBeingBought = World.ItemByID(Convert.ToInt32(itemID));

                    // Check if the player has enough gold to buy the item
                    if (_currentPlayer.Gold >= itemBeingBought.Price)
                    {
                        // Add one of the items to the player's inventory
                        _currentPlayer.AddItemToInventory(itemBeingBought);

                        // Remove the gold to pay for the item
                        _currentPlayer.Gold -= itemBeingBought.Price;

                        // Update Player's current weight
                        PlayerCurrentWeight();
                    }
                    else
                    {
                        MessageBox.Show("You do not have enough gold to buy the " + itemBeingBought.Name);
                    }
                }
            }
        }

        public int _newWeight;
        public void PlayerCurrentWeight()
        {
            for (int i = 0; i < _currentPlayer.Inventory.Count; i++)
            {
                _newWeight += _currentPlayer.Inventory[i].Quantity * _currentPlayer.Inventory[i].Weight;
            }
            _currentPlayer.CurrentWeight = _newWeight;
            _newWeight = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
       
        }
    }

