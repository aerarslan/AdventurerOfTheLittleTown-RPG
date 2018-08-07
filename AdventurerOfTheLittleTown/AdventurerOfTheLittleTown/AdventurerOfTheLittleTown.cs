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
using System.IO; // write to, read from disk

namespace AdventurerOfTheLittleTown
{
    public partial class AdventurerOfTheLittleTown : Form
    {
        private Player _player;
        private Monster _currentMonster;
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";

        public AdventurerOfTheLittleTown()
        {
            InitializeComponent();

            if (File.Exists(PLAYER_DATA_FILE_NAME))
            {
                _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
            }
            else
            {
                _player = Player.CreateDefaultPlayer();
            }

            // Player is new. Show the guide
            if (_player.NewPlayer == 1)
            {
                Guide guideScreen = new Guide();
                guideScreen.StartPosition = FormStartPosition.CenterParent;
                guideScreen.ShowDialog(this);
            }
            // Close the guide until save is removed
            _player.NewPlayer = 0;

            lblHitPoints.DataBindings.Add("Text", _player, "CurrentHitPoints");
            lblGold.DataBindings.Add("Text", _player, "Gold");
            lblExperience.DataBindings.Add("Text", _player, "ExperiencePoints");
            lblLevel.DataBindings.Add("Text", _player, "Level");
            lblDeathCounter.DataBindings.Add("Text", _player, "DeathCounter");
            lblCurrentWeight.DataBindings.Add("Text", _player, "CurrentWeight");
            lblMaxWeight.DataBindings.Add("Text", _player, "MaxWeight");

            dgvInventory.RowHeadersVisible = false;
            dgvInventory.AutoGenerateColumns = false;

            dgvInventory.DataSource = _player.Inventory;

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 140,
                DataPropertyName = "Description"
            });

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Quantity",
                DataPropertyName = "Quantity"
            });

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Weight",
                DataPropertyName = "Weight"
            });

            dgvQuests.RowHeadersVisible = false;
            dgvQuests.AutoGenerateColumns = false;

            dgvQuests.DataSource = _player.Quests;

            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 197,
                DataPropertyName = "Name"
            });

            dgvQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Done?",
                DataPropertyName = "IsCompleted"
            });

            cboWeapons.DataSource = _player.Weapons;
            cboWeapons.DisplayMember = "Name";
            cboWeapons.ValueMember = "Id";
           
            if (_player.CurrentWeapon != null)
            {
                cboWeapons.SelectedItem = _player.CurrentWeapon;
            }

            cboWeapons.SelectedIndexChanged += cboWeapons_SelectedIndexChanged;

            cboPotions.DataSource = _player.Potions;
            cboPotions.DisplayMember = "Name";
            cboPotions.ValueMember = "Id";

            _player.PropertyChanged += PlayerOnPropertyChanged;

            MoveTo(_player.CurrentLocation);

            pic_2_0.Image = Properties.Resources.mapHome;
            pic_2_1.Image = Properties.Resources.mapTownSquare;
            pic_1_1.Image = Properties.Resources.mapFarmHouse;
            pic_0_1.Image = Properties.Resources.mapFarmersField;
            pic_3_1.Image = Properties.Resources.mapAlchemistHut;
            pic_4_1.Image = Properties.Resources.mapAlchemistGarden;
            pic_2_2.Image = Properties.Resources.mapGuardPost;
            pic_2_3.Image = Properties.Resources.mapBridge;
            pic_2_4.Image = Properties.Resources.mapSpiderForest;
            pic_2_5.Image = Properties.Resources.mapSpiderCave;
        }

        private void pbNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);           
        }

        private void pbEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        private void pbSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }

        private void pbWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }
     
        private void MoveTo(Location newLocation)
        {
            PlayerCurrentWeight();
            //if player walk through a location contains quest and don't click the button, hide the buttons.
            btnQuest2.Visible = false;
            btnQuest.Visible = false;

            //Does the location have any required items
            if (!_player.HasRequiredItemToEnterThisLocation(newLocation))
            {
                rtbMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location. \n" 
                    + "Come back after you get it!" + Environment.NewLine;
                
                return;
            }

            // Update the player's current location
            _player.CurrentLocation = newLocation;

            // Show/hide available movement buttons
            pbNorth.Visible = (newLocation.LocationToNorth != null);
            pbEast.Visible = (newLocation.LocationToEast != null);
            pbSouth.Visible = (newLocation.LocationToSouth != null);
            pbWest.Visible = (newLocation.LocationToWest != null);

            // Display current location name and description
            rtbLocation.Text = "Current Location = " + newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;
                     
            // Does the location have a quest?
            if (newLocation.QuestAvailableHere != null)
            {               
                // See if the player already has the quest, and if they've completed it
                bool playerAlreadyHasQuest = _player.HasThisQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompletedQuest = _player.CompletedThisQuest(newLocation.QuestAvailableHere);

                // See if the player already has the quest
                if (playerAlreadyHasQuest)
                {
                   
                    // If the player has not completed the quest yet
                    if (!playerAlreadyCompletedQuest)
                    {
                        btnQuest2.Visible = true;                      
                    }
                }                                          
                else
                {
                    btnQuest.Visible = true;                   
                }
            }
           
            // Does the location have a monster?
            if (newLocation.MonsterLivingHere != null)
            {
                rtbMessages.Text += "◕_◕ You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine + Environment.NewLine;
                // Make a new monster, using the values from the standard monster in the World.Monster list
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                    standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaximumHitPoints);

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                cboWeapons.Visible = _player.Weapons.Any();
                cboPotions.Visible = _player.Potions.Any();
                btnUseWeapon.Visible = _player.Weapons.Any();
                btnUsePotion.Visible = _player.Potions.Any();
                if (_player.CurrentWeapon == null)
                {
                    lblAction.Visible = false;
                }
                else
                {
                    lblAction.Visible = true;
                }
            }
            else
            {
                _currentMonster = null;

                cboWeapons.Visible = false;
                cboPotions.Visible = false;
                btnUseWeapon.Visible = false;
                btnUsePotion.Visible = false;
                lblAction.Visible = false;
            }

            btnTrade.Visible = (_player.CurrentLocation.VendorWorkingHere != null);
            MapDefault(_player.CurrentLocation);
            MapLocation(_player.CurrentLocation);            
        }
        private void MapLocation(Location location)
        {
            if (location == World.LocationByID(World.LOCATION_ID_HOME))
            {
                MapDetails(pic_2_0);
            }
            else if (location == World.LocationByID(World.LOCATION_ID_TOWN_SQUARE))
            {
                MapDetails(pic_2_1);
            }
            else if (location == World.LocationByID(World.LOCATION_ID_FARMHOUSE))
            {
                MapDetails(pic_1_1);
            }               
            else if (location == World.LocationByID(World.LOCATION_ID_FARM_FIELD))
            {
                MapDetails(pic_0_1);
            }              
            else if (location == World.LocationByID(World.LOCATION_ID_ALCHEMIST_HUT))
            {
                MapDetails(pic_3_1);
            }
            else if (location == World.LocationByID(World.LOCATION_ID_ALCHEMISTS_GARDEN))
            {
                MapDetails(pic_4_1);
            }
            else if (location == World.LocationByID(World.LOCATION_ID_GUARD_POST))
            {
                MapDetails(pic_2_2);
            }
            else if (location == World.LocationByID(World.LOCATION_ID_BRIDGE))
            {
                MapDetails(pic_2_3);
            }
            else if (location == World.LocationByID(World.LOCATION_ID_SPIDER_FOREST))
            {
                MapDetails(pic_2_4);               
            }
            else if (location == World.LocationByID(World.LOCATION_ID_SPIDER_CAVE))
            {
                MapDetails(pic_2_5);
            }
        }

        private void MapDetails(PictureBox pictureBox)
        {
            pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            pictureBox.BackColor = Color.FromArgb(127, 255, 0);
        }

        private void MapDefault(Location location)
        {
            pic_2_0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pic_2_0.BackColor = Color.FromArgb(143, 188, 143);
            pic_2_1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pic_2_1.BackColor = Color.FromArgb(143, 188, 143);
            pic_1_1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pic_1_1.BackColor = Color.FromArgb(143, 188, 143);
            pic_0_1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pic_0_1.BackColor = Color.FromArgb(143, 188, 143);
            pic_3_1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pic_3_1.BackColor = Color.FromArgb(143, 188, 143);
            pic_4_1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pic_4_1.BackColor = Color.FromArgb(143, 188, 143);
            pic_2_2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pic_2_2.BackColor = Color.FromArgb(143, 188, 143);
            pic_2_3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pic_2_3.BackColor = Color.FromArgb(143, 188, 143);
            pic_2_4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pic_2_4.BackColor = Color.FromArgb(143, 188, 143);
            pic_2_5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pic_2_5.BackColor = Color.FromArgb(143, 188, 143);
        }

        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            // Get the currently selected weapon from the cboWeapons ComboBox
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            // Determine the amount of damage to do to the monster
            int damageToMonster = RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);

            // Apply the damage to the monster's CurrentHitPoints
            _currentMonster.CurrentHitPoints -= damageToMonster;

            // Display message
            rtbMessages.Text += "▬▬ι═══════- You hit the " + _currentMonster.Name + " for " + damageToMonster.ToString() + " points." + Environment.NewLine;

            // Check if the monster is dead
            if (_currentMonster.CurrentHitPoints <= 0)
            {
                // Monster is dead
                rtbMessages.Text += "☠ You defeated the " + _currentMonster.Name + Environment.NewLine;

                // Give player experience points for killing the monster
                _player.AddExperiencePoints(_currentMonster.RewardExperiencePoints);
                rtbMessages.Text += "► You receive " + _currentMonster.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;

                // Give player gold for killing the monster 
                _player.Gold += _currentMonster.RewardGold;
                rtbMessages.Text += "► You receive " + _currentMonster.RewardGold.ToString() + " gold" + Environment.NewLine;

                // Get random loot items from the monster
                List<InventoryItem> lootedItems = new List<InventoryItem>();

                // Add items to the lootedItems list, comparing a random number to the drop percentage
                foreach (LootItem lootItem in _currentMonster.LootTable)
                {
                    if (RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }

                // If no items were randomly selected, then add the default loot item(s).
                if (lootedItems.Count == 0)
                {
                    foreach (LootItem lootItem in _currentMonster.LootTable)
                    {
                        if (lootItem.IsDefaultItem)
                        {
                            lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                        }
                    }
                }

                // Add the looted items to the player's inventory
                foreach (InventoryItem inventoryItem in lootedItems)
                {
                    if (_player.CurrentWeight >= _player.MaxWeight)
                    {
                        MessageBox.Show("You don't have enough space to carry more items! You couldn't loot  "+inventoryItem.Details.Name);
                    }
                    else
                    {
                        _player.AddItemToInventory(inventoryItem.Details);

                        if (inventoryItem.Quantity == 1)
                        {
                            rtbMessages.Text += "►►► You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.Name + Environment.NewLine;
                        }
                        else
                        {
                            rtbMessages.Text += "►►► You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.NamePlural + Environment.NewLine;
                        }
                    }
                }
                                          
                //Heal the player
                _player.CurrentHitPoints = _player.MaximumHitPoints;
                if (_player.CurrentHitPoints > _player.MaximumHitPoints)
                {
                    _player.CurrentHitPoints = _player.MaximumHitPoints;
                }

                // Add a blank line to the messages box, just for appearance.
                rtbMessages.Text += Environment.NewLine;

                // Move player to current location (to heal player and create a new monster to fight)
                MoveTo(_player.CurrentLocation);
               
            }
            else
            {
                // Monster is still alive

                // Determine the amount of damage the monster does to the player
                int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);

                // Display message
                rtbMessages.Text += "-═══════ι▬▬ The " + _currentMonster.Name + " dealed " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;

                // Subtract damage from player
                _player.CurrentHitPoints -= damageToPlayer;

             
              
                  if (_player.CurrentHitPoints <= 0)
                  {
                      var confirmResult = MessageBox.Show("Do you want to revive? You will lose 3 maximum Hitpoints !!!",
                                    "(x_x) The " + _currentMonster.Name + " killed you.",
                                    MessageBoxButtons.YesNo);                    
                      if (confirmResult == DialogResult.Yes)
                      {                                               
                              // Move player to "Home"
                              MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                              // Death Count
                              _player.DeathCounter++;
                              // Max hit points
                              _player.MaximumHitPoints = ((_player.Level * 10) - (_player.DeathCounter * 3));
                              // Heal the player
                              _player.CurrentHitPoints = _player.MaximumHitPoints;
                              if (_player.CurrentHitPoints <= 0)
                              {
                                  MessageBox.Show("You don't have enough HitPoints to revive!");
                                  ResetThePlayer();
                              }
                      }
                      else // Didn't revive
                      {
                          ResetThePlayer();
                      }                                               
                 }
                                                     
            }
        }

        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            // Get the currently selected potion from the combobox
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

            // Add healing amount to the player's current hit points
            _player.CurrentHitPoints = (_player.CurrentHitPoints + potion.AmountToHeal);

            // CurrentHitPoints cannot exceed player's MaximumHitPoints
            if (_player.CurrentHitPoints > _player.MaximumHitPoints)
            {
                _player.CurrentHitPoints = _player.MaximumHitPoints;
            }

            // Remove the potion from the player's inventory
            _player.RemoveItemFromInventory(potion, 1);

            // Display message
            rtbMessages.Text += "You drink a " + potion.Name + Environment.NewLine;

            // Monster gets their turn to attack

            // Determine the amount of damage the monster does to the player
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);

            // Display message
            rtbMessages.Text += "The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;

            // Subtract damage from player
            _player.CurrentHitPoints -= damageToPlayer;

            if (_player.CurrentHitPoints <= 0)
            {
                // Display message
                rtbMessages.Text += "The " + _currentMonster.Name + " killed you." + Environment.NewLine;

                // Move player to "Home"
                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }
                     
        }

        private void btnTrade_Click(object sender, EventArgs e)
        {
            TradingScreen tradingScreen = new TradingScreen(_player);
            tradingScreen.StartPosition = FormStartPosition.CenterParent;
            tradingScreen.ShowDialog(this);
        }

        private void PlayerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Weapons")
            {
                Weapon previouslySelectedWeapon = _player.CurrentWeapon;

                cboWeapons.DataSource = _player.Weapons;
               
                if (previouslySelectedWeapon != null &&
                    _player.Weapons.Exists(w => w.ID == previouslySelectedWeapon.ID))
                {                   
                    cboWeapons.Text = previouslySelectedWeapon.Name;                  
                    cboWeapons.SelectedItem = previouslySelectedWeapon;
                }

                if (!_player.Weapons.Any())
                {
                    cboWeapons.Visible = false;
                    btnUseWeapon.Visible = false;
                }
            }

            if (propertyChangedEventArgs.PropertyName == "Potions")
            {
                cboPotions.DataSource = _player.Potions;

                if (!_player.Potions.Any())
                {
                    cboPotions.Visible = false;
                    btnUsePotion.Visible = false;
                }
            }
        }

            // Scroll to bottom automatically
        private void rtbMessages_TextChanged(object sender, EventArgs e)
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }
       
        private void cboWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            _player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
        }

        private void btnMap_Click(object sender, EventArgs e)
        {           
            if (Width == 735)
                Width = 1280;
            else Width = 735;
        }

        private void AdventurerOfTheLittleTown_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Reset your progress?",
                                     "Are you sure?",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                ResetThePlayer();
            }
            else
            {
                MessageBox.Show("Be careful next time !!");
            }
            
        }

        private void ResetThePlayer() 
        {
            Player newPlayer = Player.CreateDefaultPlayer();           
            _player = newPlayer;           
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            
            rtbMessages.Clear();

            lblHitPoints.DataBindings.Clear();
            lblGold.DataBindings.Clear();
            lblExperience.DataBindings.Clear();
            lblLevel.DataBindings.Clear();
            lblDeathCounter.DataBindings.Clear();
            lblCurrentWeight.DataBindings.Clear();
            lblMaxWeight.DataBindings.Clear();

            lblHitPoints.DataBindings.Add("Text", _player, "CurrentHitPoints");
            lblGold.DataBindings.Add("Text", _player, "Gold");
            lblExperience.DataBindings.Add("Text", _player, "ExperiencePoints");
            lblLevel.DataBindings.Add("Text", _player, "Level");
            lblDeathCounter.DataBindings.Add("Text", _player, "DeathCounter");
            lblCurrentWeight.DataBindings.Add("Text", _player, "CurrentWeight");
            lblMaxWeight.DataBindings.Add("Text", _player, "MaxWeight");

            cboWeapons.DataSource = _player.Weapons;
            cboWeapons.DisplayMember = "Name";
            cboWeapons.ValueMember = "Id";

            if (_player.CurrentWeapon != null)
            {
                cboWeapons.SelectedItem = _player.CurrentWeapon;
            }

            cboWeapons.SelectedIndexChanged += cboWeapons_SelectedIndexChanged;

            cboPotions.DataSource = _player.Potions;
            cboPotions.DisplayMember = "Name";
            cboPotions.ValueMember = "Id";

            _player.PropertyChanged += PlayerOnPropertyChanged;

            dgvInventory.DataSource = _player.Inventory;
            dgvQuests.DataSource = _player.Quests;

            // player is not new.
            _player.NewPlayer = 0;
                
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {           
            if (dgvInventory.Visible == false)
            {              
                dgvInventory.Visible = true;
            }
            else
            {
                dgvInventory.Visible = false;
            }
        }

        private void btnQuestList_Click(object sender, EventArgs e)
        {
            if (dgvQuests.Visible == false)
            {
                dgvQuests.Visible = true;
            }
            else
            {
                dgvQuests.Visible = false;
            }
        }

        private void pbNorth_MouseHover(object sender, EventArgs e)
        {
            pbNorth.Image = Properties.Resources.upArrowHover;
        }

        private void pbNorth_MouseLeave(object sender, EventArgs e)
        {
            pbNorth.Image = Properties.Resources.upArrow;
        }

        private void pbEast_MouseHover(object sender, EventArgs e)
        {
            pbEast.Image = Properties.Resources.rightArrowHover;
        }

        private void pbEast_MouseLeave(object sender, EventArgs e)
        {
            pbEast.Image = Properties.Resources.rightArrow;
        }

        private void pbSouth_MouseHover(object sender, EventArgs e)
        {
            pbSouth.Image = Properties.Resources.downArrowHover;
        }

        private void pbSouth_MouseLeave(object sender, EventArgs e)
        {
            pbSouth.Image = Properties.Resources.downArrow;
        }

        private void pbWest_MouseHover(object sender, EventArgs e)
        {
            pbWest.Image = Properties.Resources.leftArrowHover;
        }

        private void pbWest_MouseLeave(object sender, EventArgs e)
        {
            pbWest.Image = Properties.Resources.leftArrow;
        }
        
        private void btnGuide_Click(object sender, EventArgs e)
        {
            Guide guideScreen = new Guide();
            guideScreen.StartPosition = FormStartPosition.CenterParent;
            guideScreen.ShowDialog(this);
        }

        private void btnQuest_Click(object sender, EventArgs e)
        {
            // The player does not already have the quest

            // Display the messages
            rtbMessages.Text += "You receive the " + _player.CurrentLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
            rtbMessages.Text += _player.CurrentLocation.QuestAvailableHere.Description + Environment.NewLine;
            rtbMessages.Text += "To complete it, return with:" + Environment.NewLine;
            foreach (QuestCompletionItem qci in _player.CurrentLocation.QuestAvailableHere.QuestCompletionItems)
            {
                if (qci.Quantity == 1)
                {
                    rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                }
                else
                {
                    rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                }
            }
            rtbMessages.Text += Environment.NewLine;

            // Add the quest to the player's quest list
            _player.Quests.Add(new PlayerQuest(_player.CurrentLocation.QuestAvailableHere));
            btnQuest.Visible = false;
        }

        private void btnQuest2_Click(object sender, EventArgs e)
        {
            _player.HasAllQuestCompletionItems(_player.CurrentLocation.QuestAvailableHere);

            // The player has all items required to complete the quest
            if (_player.HasAllQuestCompletionItems(_player.CurrentLocation.QuestAvailableHere))
            {
                // Display message
                rtbMessages.Text += Environment.NewLine;
                rtbMessages.Text += "You complete the '" + _player.CurrentLocation.QuestAvailableHere.Name + "' quest. \n" + Environment.NewLine;

                // Remove quest items from inventory
                _player.RemoveQuestCompletionItems(_player.CurrentLocation.QuestAvailableHere);

                // Give quest rewards
                rtbMessages.Text += "► You receive: " + Environment.NewLine;
                rtbMessages.Text += _player.CurrentLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                rtbMessages.Text += _player.CurrentLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                rtbMessages.Text += _player.CurrentLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                rtbMessages.Text += Environment.NewLine;

                _player.AddExperiencePoints(_player.CurrentLocation.QuestAvailableHere.RewardExperiencePoints);
                _player.Gold += _player.CurrentLocation.QuestAvailableHere.RewardGold;

                // Add the reward item to the player's inventory
                _player.AddItemToInventory(_player.CurrentLocation.QuestAvailableHere.RewardItem);
                // Mark the quest as completed
                _player.MarkQuestCompleted(_player.CurrentLocation.QuestAvailableHere);

                PlayerCurrentWeight();
                btnQuest2.Visible = false;
            }
        }

        
        public int _newWeight;
        public void PlayerCurrentWeight()
        {          
            for (int i = 0; i < _player.Inventory.Count; i++)
            {              
                 _newWeight += _player.Inventory[i].Quantity * _player.Inventory[i].Weight;
            }
            _player.CurrentWeight = _newWeight;
            _newWeight = 0;
        }
                
    }
}