// File: ArtilleryMaster_BuildFinal/Form1.cs
// Welcome to Form1.cs! If you find a bug, it was probably the blue dot's fault. Or mine. But mostly the blue dot.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.IO;

namespace ArtilleryMaster_BuildFinal
{
    // This is Form1. It's like Form2, but cooler. (Actually, there is no Form2.)
    public class Form1 : Form
    {
        // --- Designer fields ---
        private System.ComponentModel.IContainer components = null;
        private PictureBox pictureBoxMap;
        private OutlinedPanel panelRight;
        private Label labelElevation;
        private Label labelAzimuth;
        private Label labelDistance;
        private Label labelTimeOfFlight;
        private TextBox valueElevation;
        private TextBox valueAzimuth;
        private TextBox valueDistance;
        private TextBox valueTimeOfFlight;
        private Label labelProjectile;
        private ComboBox comboProjectile;
        private Label labelMap;
        private ComboBox comboMap;
        private OutlinedPanel panel1;
        private Button buttonAddMap;
        private Button buttonAddProjectile;
        private Label Target_Name;
        private HintTextBox TarResPoi;
        private Button saveTRP;
        private OutlinedPanel panel2;
        private Label label3;
        private Label label2;
        private Label label1;
        private HintTextBox textBox1;
        private HintTextBox textBox2;
        private ComboBox mphorkph;
        private Button button1;
        private Label labelInstructions;
        private Button button3;
        private Button button2;
        private Button EditMenu;
        private TextBox textBoxImagePath;

        // Replace panelEditProjectile and its child controls with the new UserControl
        private EditProjectilePanel editProjectilePanel;

        // Dictionary for map image paths. Because remembering stuff is hard.
        private Dictionary<string, string> mapImagePaths = new Dictionary<string, string>();
        private Dictionary<string, double> mapGridSizes = new Dictionary<string, double>(); // <-- Add this line

        // Fields for map interaction
        private PointF? weaponMapPos = null; // Blue dot (left click)
        private PointF? targetMapPos = null; // Red dot (right click)
        private float mapZoom = 1.0f;
        private Point mapPan = Point.Empty;
        private Point? panStart = null;
        private Image mapImage = null;
        private double currentGridSize = 100.0;
        private Button delTRP;
        private ComboBox Target_Point;

        // List of projectiles. Because one is never enough.
        private List<ProjectileData> projectiles = new List<ProjectileData>();

        // List of TRPs. Not to be confused with RPGs.
        private List<TargetRegistrationPoint> trpList = new List<TargetRegistrationPoint>();
        
        // this directs to the save file, where everything is saved.
        private const string SaveFilePath = "artillerymaster_save.txt";

        public Form1()
        {
            InitializeComponent();
            // Hide the bottom panel on startup. Out of sight, out of mind.
            panel2.Visible = false;

            // Create and add the EditProjectilePanel. Because editing is caring.
            editProjectilePanel = new EditProjectilePanel();
            editProjectilePanel.Location = new Point(800, 540);
            editProjectilePanel.Visible = false;
            this.Controls.Add(editProjectilePanel);

            // Wire up events. Like a mad scientist, but with less electricity.
            editProjectilePanel.SaveClicked += btnEditSave_Click;
            editProjectilePanel.RemoveClicked += btnEditRemove_Click;
            editProjectilePanel.CancelClicked += btnEditCancel_Click;
            editProjectilePanel.BrowseClicked += btnEditBrowse_Click;
            editProjectilePanel.SelectedProjectileChanged += (s, e) => UpdateEditPanelFields();

            // Wire up map selection event
            comboMap.SelectedIndexChanged += ComboMap_SelectedIndexChanged;

            // Wire up PictureBox events for map interaction
            pictureBoxMap.Paint += PictureBoxMap_Paint;
            pictureBoxMap.MouseDown += PictureBoxMap_MouseDown;
            pictureBoxMap.MouseWheel += PictureBoxMap_MouseWheel;
            pictureBoxMap.MouseMove += PictureBoxMap_MouseMove;
            pictureBoxMap.MouseUp += PictureBoxMap_MouseUp;
            pictureBoxMap.Focus();
            pictureBoxMap.TabStop = true;
            pictureBoxMap.MouseEnter += (s, e) => pictureBoxMap.Focus();

            // Style buttons (do this after InitializeComponent)
            StyleButton(this.button2);
            StyleButton(this.button3);
            StyleButton(this.EditMenu);

            // Wire up projectile selection event
            comboProjectile.SelectedIndexChanged += comboProjectile_SelectedIndexChanged;

            // Wire up TRP events
            saveTRP.Click += saveTRP_Click;
            Target_Point.SelectedIndexChanged += Target_Point_SelectedIndexChanged;
            delTRP.Click += delTRP_Click;

            // Style ComboBoxes
            StyleComboBox(this.comboProjectile);
            StyleComboBox(this.comboMap);

            // Load data on startup
            LoadAllData();

            // Save data on close
            this.FormClosing += (s, e) => SaveAllData();
        }
        
        protected override void Dispose(bool disposing)
        {// Dispose of the form and its components. Because we don't want to leave a mess behind.
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pictureBoxMap = new System.Windows.Forms.PictureBox();
            this.labelInstructions = new System.Windows.Forms.Label();
            this.panelRight = new ArtilleryMaster_BuildFinal.OutlinedPanel();
            this.labelElevation = new System.Windows.Forms.Label();
            this.valueElevation = new System.Windows.Forms.TextBox();
            this.labelAzimuth = new System.Windows.Forms.Label();
            this.valueAzimuth = new System.Windows.Forms.TextBox();
            this.labelDistance = new System.Windows.Forms.Label();
            this.valueDistance = new System.Windows.Forms.TextBox();
            this.labelTimeOfFlight = new System.Windows.Forms.Label();
            this.valueTimeOfFlight = new System.Windows.Forms.TextBox();
            this.labelProjectile = new System.Windows.Forms.Label();
            this.comboProjectile = new System.Windows.Forms.ComboBox();
            this.labelMap = new System.Windows.Forms.Label();
            this.comboMap = new System.Windows.Forms.ComboBox();
            this.panel1 = new ArtilleryMaster_BuildFinal.OutlinedPanel();
            this.delTRP = new System.Windows.Forms.Button();
            this.Target_Point = new System.Windows.Forms.ComboBox();
            this.saveTRP = new System.Windows.Forms.Button();
            this.TarResPoi = new ArtilleryMaster_BuildFinal.HintTextBox();
            this.Target_Name = new System.Windows.Forms.Label();
            this.buttonAddProjectile = new System.Windows.Forms.Button();
            this.buttonAddMap = new System.Windows.Forms.Button();
            this.panel2 = new ArtilleryMaster_BuildFinal.OutlinedPanel();
            this.EditMenu = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBoxImagePath = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.mphorkph = new System.Windows.Forms.ComboBox();
            this.textBox2 = new ArtilleryMaster_BuildFinal.HintTextBox();
            this.textBox1 = new ArtilleryMaster_BuildFinal.HintTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMap)).BeginInit();
            this.panelRight.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxMap
            //
            this.pictureBoxMap.BackColor = System.Drawing.Color.DimGray;
            this.pictureBoxMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxMap.Location = new System.Drawing.Point(60, 40);
            this.pictureBoxMap.Name = "pictureBoxMap";
            this.pictureBoxMap.Size = new System.Drawing.Size(700, 600);
            this.pictureBoxMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxMap.TabIndex = 0;
            this.pictureBoxMap.TabStop = false;
            // 
            // labelInstructions
            // 
            this.labelInstructions.AutoSize = true;
            this.labelInstructions.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelInstructions.ForeColor = System.Drawing.Color.LightGray;
            this.labelInstructions.Location = new System.Drawing.Point(60, 660);
            this.labelInstructions.Name = "labelInstructions";
            this.labelInstructions.Size = new System.Drawing.Size(647, 15);
            this.labelInstructions.TabIndex = 2;
            this.labelInstructions.Text = "LMB to set your position. RMB to set the target\'s position. Hold MMB to move the " +
    "map around, and scroll wheel to zoom.";
            // 
            // panelRight
            // 
            this.panelRight.BackColor = System.Drawing.Color.Black;
            this.panelRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelRight.Controls.Add(this.labelElevation);
            this.panelRight.Controls.Add(this.valueElevation);
            this.panelRight.Controls.Add(this.labelAzimuth);
            this.panelRight.Controls.Add(this.valueAzimuth);
            this.panelRight.Controls.Add(this.labelDistance);
            this.panelRight.Controls.Add(this.valueDistance);
            this.panelRight.Controls.Add(this.labelTimeOfFlight);
            this.panelRight.Controls.Add(this.valueTimeOfFlight);
            this.panelRight.Controls.Add(this.labelProjectile);
            this.panelRight.Controls.Add(this.comboProjectile);
            this.panelRight.Controls.Add(this.labelMap);
            this.panelRight.Controls.Add(this.comboMap);
            this.panelRight.Location = new System.Drawing.Point(800, 40);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(400, 300);
            this.panelRight.TabIndex = 1;
            // 
            // labelElevation
            // 
            this.labelElevation.AutoSize = true;
            this.labelElevation.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelElevation.ForeColor = System.Drawing.Color.White;
            this.labelElevation.Location = new System.Drawing.Point(20, 17);
            this.labelElevation.Name = "labelElevation";
            this.labelElevation.Size = new System.Drawing.Size(70, 19);
            this.labelElevation.TabIndex = 0;
            this.labelElevation.Text = "Elevation";
            // 
            // valueElevation
            // 
            this.valueElevation.BackColor = System.Drawing.Color.Black;
            this.valueElevation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valueElevation.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.valueElevation.ForeColor = System.Drawing.Color.White;
            this.valueElevation.Location = new System.Drawing.Point(180, 17);
            this.valueElevation.Name = "valueElevation";
            this.valueElevation.ReadOnly = true;
            this.valueElevation.Size = new System.Drawing.Size(180, 25);
            this.valueElevation.TabIndex = 1;
            this.valueElevation.Text = "-";
            // 
            // labelAzimuth
            // 
            this.labelAzimuth.AutoSize = true;
            this.labelAzimuth.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelAzimuth.ForeColor = System.Drawing.Color.White;
            this.labelAzimuth.Location = new System.Drawing.Point(21, 53);
            this.labelAzimuth.Name = "labelAzimuth";
            this.labelAzimuth.Size = new System.Drawing.Size(64, 19);
            this.labelAzimuth.TabIndex = 1;
            this.labelAzimuth.Text = "Azimuth";
            // 
            // valueAzimuth
            // 
            this.valueAzimuth.BackColor = System.Drawing.Color.Black;
            this.valueAzimuth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valueAzimuth.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.valueAzimuth.ForeColor = System.Drawing.Color.White;
            this.valueAzimuth.Location = new System.Drawing.Point(180, 53);
            this.valueAzimuth.Name = "valueAzimuth";
            this.valueAzimuth.ReadOnly = true;
            this.valueAzimuth.Size = new System.Drawing.Size(180, 25);
            this.valueAzimuth.TabIndex = 2;
            this.valueAzimuth.Text = "-";
            // 
            // labelDistance
            // 
            this.labelDistance.AutoSize = true;
            this.labelDistance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelDistance.ForeColor = System.Drawing.Color.White;
            this.labelDistance.Location = new System.Drawing.Point(21, 89);
            this.labelDistance.Name = "labelDistance";
            this.labelDistance.Size = new System.Drawing.Size(65, 19);
            this.labelDistance.TabIndex = 2;
            this.labelDistance.Text = "Distance";
            // 
            // valueDistance
            // 
            this.valueDistance.BackColor = System.Drawing.Color.Black;
            this.valueDistance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valueDistance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.valueDistance.ForeColor = System.Drawing.Color.White;
            this.valueDistance.Location = new System.Drawing.Point(180, 89);
            this.valueDistance.Name = "valueDistance";
            this.valueDistance.ReadOnly = true;
            this.valueDistance.Size = new System.Drawing.Size(180, 25);
            this.valueDistance.TabIndex = 3;
            this.valueDistance.Text = "-";
            // 
            // labelTimeOfFlight
            // 
            this.labelTimeOfFlight.AutoSize = true;
            this.labelTimeOfFlight.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelTimeOfFlight.ForeColor = System.Drawing.Color.White;
            this.labelTimeOfFlight.Location = new System.Drawing.Point(21, 125);
            this.labelTimeOfFlight.Name = "labelTimeOfFlight";
            this.labelTimeOfFlight.Size = new System.Drawing.Size(99, 19);
            this.labelTimeOfFlight.TabIndex = 3;
            this.labelTimeOfFlight.Text = "Time of flight";
            // 
            // valueTimeOfFlight
            // 
            this.valueTimeOfFlight.BackColor = System.Drawing.Color.Black;
            this.valueTimeOfFlight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.valueTimeOfFlight.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.valueTimeOfFlight.ForeColor = System.Drawing.Color.White;
            this.valueTimeOfFlight.Location = new System.Drawing.Point(180, 125);
            this.valueTimeOfFlight.Name = "valueTimeOfFlight";
            this.valueTimeOfFlight.ReadOnly = true;
            this.valueTimeOfFlight.Size = new System.Drawing.Size(180, 25);
            this.valueTimeOfFlight.TabIndex = 4;
            this.valueTimeOfFlight.Text = "-";
            // 
            // labelProjectile
            // 
            this.labelProjectile.AutoSize = true;
            this.labelProjectile.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelProjectile.ForeColor = System.Drawing.Color.White;
            this.labelProjectile.Location = new System.Drawing.Point(21, 161);
            this.labelProjectile.Name = "labelProjectile";
            this.labelProjectile.Size = new System.Drawing.Size(73, 19);
            this.labelProjectile.TabIndex = 8;
            this.labelProjectile.Text = "Projectile";
            // 
            // comboProjectile
            // 
            this.comboProjectile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboProjectile.FormattingEnabled = true;
            this.comboProjectile.Items.AddRange(new object[] {
            "3BK-10"});
            this.comboProjectile.Location = new System.Drawing.Point(180, 161);
            this.comboProjectile.Name = "comboProjectile";
            this.comboProjectile.Size = new System.Drawing.Size(120, 21);
            this.comboProjectile.TabIndex = 9;
            // Set DrawMode and subscribe to DrawItem for custom rendering
            this.comboProjectile.DrawMode = DrawMode.OwnerDrawFixed;
            this.comboProjectile.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboProjectile.DrawItem += ComboBox_DrawItem;
            // 
            // labelMap
            // 
            this.labelMap.AutoSize = true;
            this.labelMap.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelMap.ForeColor = System.Drawing.Color.White;
            this.labelMap.Location = new System.Drawing.Point(21, 202);
            this.labelMap.Name = "labelMap";
            this.labelMap.Size = new System.Drawing.Size(39, 19);
            this.labelMap.TabIndex = 10;
            this.labelMap.Text = "Map";
            // 
            // comboMap
            // 
            this.comboMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboMap.FormattingEnabled = true;
            this.comboMap.Items.AddRange(new object[] {
            "Muddy Fields"});
            this.comboMap.Location = new System.Drawing.Point(180, 202);
            this.comboMap.Name = "comboMap";
            this.comboMap.Size = new System.Drawing.Size(120, 21);
            this.comboMap.TabIndex = 11;
            // Set DrawMode and subscribe to DrawItem for custom rendering
            this.comboMap.DrawMode = DrawMode.OwnerDrawFixed;
            this.comboMap.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboMap.DrawItem += ComboBox_DrawItem;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.delTRP);
            this.panel1.Controls.Add(this.Target_Point);
            this.panel1.Controls.Add(this.saveTRP);
            this.panel1.Controls.Add(this.TarResPoi);
            this.panel1.Controls.Add(this.Target_Name);
            this.panel1.Controls.Add(this.buttonAddProjectile);
            this.panel1.Controls.Add(this.buttonAddMap);
            this.panel1.Location = new System.Drawing.Point(800, 346);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 135);
            this.panel1.TabIndex = 3;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // delTRP
            // 
            this.delTRP.BackColor = System.Drawing.Color.Black;
            this.delTRP.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.delTRP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.delTRP.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.delTRP.ForeColor = System.Drawing.Color.White;
            this.delTRP.Location = new System.Drawing.Point(152, 100);
            this.delTRP.Name = "delTRP";
            this.delTRP.Size = new System.Drawing.Size(107, 27);
            this.delTRP.TabIndex = 6;
            this.delTRP.Text = "Delete TRP";
            this.delTRP.UseVisualStyleBackColor = false;
            // 
            // Target_Point
            // 
            this.Target_Point.BackColor = System.Drawing.Color.Black;
            this.Target_Point.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Target_Point.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Target_Point.ForeColor = System.Drawing.Color.White;
            this.Target_Point.FormattingEnabled = true;
            this.Target_Point.Location = new System.Drawing.Point(25, 100);
            this.Target_Point.Name = "Target_Point";
            this.Target_Point.Size = new System.Drawing.Size(121, 25);
            this.Target_Point.TabIndex = 5;
            // 
            // saveTRP
            // 
            this.saveTRP.BackColor = System.Drawing.Color.Black;
            this.saveTRP.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.saveTRP.ForeColor = System.Drawing.Color.White;
            this.saveTRP.Location = new System.Drawing.Point(24, 67);
            this.saveTRP.Name = "saveTRP";
            this.saveTRP.Size = new System.Drawing.Size(238, 27);
            this.saveTRP.TabIndex = 4;
            this.saveTRP.Text = "Save Target Registration Point";
            this.saveTRP.UseVisualStyleBackColor = false;
            // 
            // TarResPoi
            // 
            this.TarResPoi.BackColor = System.Drawing.Color.Black;
            this.TarResPoi.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TarResPoi.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.TarResPoi.ForeColor = System.Drawing.Color.White;
            this.TarResPoi.Hint = "Target Name";
            this.TarResPoi.Location = new System.Drawing.Point(120, 36);
            this.TarResPoi.Name = "TarResPoi";
            this.TarResPoi.Size = new System.Drawing.Size(142, 25);
            this.TarResPoi.TabIndex = 3;
            this.TarResPoi.TextChanged += new System.EventHandler(this.TarResPoi_TextChanged);
            // 
            // Target_Name
            // 
            this.Target_Name.AutoSize = true;
            this.Target_Name.BackColor = System.Drawing.Color.Black;
            this.Target_Name.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.Target_Name.ForeColor = System.Drawing.Color.White;
            this.Target_Name.Location = new System.Drawing.Point(24, 38);
            this.Target_Name.Name = "Target_Name";
            this.Target_Name.Size = new System.Drawing.Size(96, 19);
            this.Target_Name.TabIndex = 2;
            this.Target_Name.Text = "Target Name";
            // 
            // buttonAddProjectile
            // 
            this.buttonAddProjectile.BackColor = System.Drawing.Color.Black;
            this.buttonAddProjectile.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonAddProjectile.ForeColor = System.Drawing.Color.White;
            this.buttonAddProjectile.Location = new System.Drawing.Point(132, 3);
            this.buttonAddProjectile.Name = "buttonAddProjectile";
            this.buttonAddProjectile.Size = new System.Drawing.Size(130, 27);
            this.buttonAddProjectile.TabIndex = 1;
            this.buttonAddProjectile.Text = "Add Projectile";
            this.buttonAddProjectile.UseVisualStyleBackColor = false;
            this.buttonAddProjectile.Click += new System.EventHandler(this.buttonAddProjectile_Click);
            // 
            // buttonAddMap
            // 
            this.buttonAddMap.BackColor = System.Drawing.Color.Black;
            this.buttonAddMap.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.buttonAddMap.ForeColor = System.Drawing.Color.White;
            this.buttonAddMap.Location = new System.Drawing.Point(24, 3);
            this.buttonAddMap.Name = "buttonAddMap";
            this.buttonAddMap.Size = new System.Drawing.Size(102, 27);
            this.buttonAddMap.TabIndex = 0;
            this.buttonAddMap.Text = "Add Map";
            this.buttonAddMap.UseVisualStyleBackColor = false;
            this.buttonAddMap.Click += new System.EventHandler(this.buttonAddMap_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.EditMenu);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.textBoxImagePath);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.mphorkph);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(800, 487);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(400, 159);
            this.panel2.TabIndex = 5;
            // 
            // EditMenu
            // 
            this.EditMenu.BackColor = System.Drawing.Color.Black;
            this.EditMenu.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.EditMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EditMenu.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.EditMenu.ForeColor = System.Drawing.Color.White;
            this.EditMenu.Location = new System.Drawing.Point(270, 118);
            this.EditMenu.Name = "EditMenu";
            this.EditMenu.Size = new System.Drawing.Size(75, 33);
            this.EditMenu.TabIndex = 11;
            this.EditMenu.Text = "Edit";
            this.EditMenu.UseVisualStyleBackColor = false;
            this.EditMenu.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Black;
            this.button3.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button3.ForeColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(152, 118);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 33);
            this.button3.TabIndex = 10;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Black;
            this.button2.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(25, 118);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 33);
            this.button2.TabIndex = 9;
            this.button2.Text = "Confirm/Add";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBoxImagePath
            // 
            this.textBoxImagePath.BackColor = System.Drawing.Color.Black;
            this.textBoxImagePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxImagePath.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBoxImagePath.ForeColor = System.Drawing.Color.White;
            this.textBoxImagePath.Location = new System.Drawing.Point(160, 79);
            this.textBoxImagePath.Name = "textBoxImagePath";
            this.textBoxImagePath.ReadOnly = true;
            this.textBoxImagePath.Size = new System.Drawing.Size(200, 25);
            this.textBoxImagePath.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Black;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(79, 67);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 33);
            this.button1.TabIndex = 7;
            this.button1.Text = "Browse...";
            this.button1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // mphorkph
            // 
            this.mphorkph.BackColor = System.Drawing.Color.Black;
            this.mphorkph.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mphorkph.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.mphorkph.ForeColor = System.Drawing.Color.White;
            this.mphorkph.FormattingEnabled = true;
            this.mphorkph.Items.AddRange(new object[] {
            "m/h",
            "k/h",
            "m/s"});
            this.mphorkph.Location = new System.Drawing.Point(295, 34);
            this.mphorkph.Name = "mphorkph";
            this.mphorkph.Size = new System.Drawing.Size(50, 25);
            this.mphorkph.TabIndex = 6;
            this.mphorkph.SelectedIndexChanged += new System.EventHandler(this.mphorkph_SelectedIndexChanged);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.Black;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBox2.ForeColor = System.Drawing.Color.White;
            this.textBox2.Hint = "Projectile\'s Velocity";
            this.textBox2.Location = new System.Drawing.Point(120, 34);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(139, 25);
            this.textBox2.TabIndex = 4;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            this.textBox2.GotFocus += new System.EventHandler(this.textBox2_GotFocus);
            this.textBox2.LostFocus += new System.EventHandler(this.textBox2_LostFocus);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.Black;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Hint = "Projectile/Weapon\'s Name";
            this.textBox1.Location = new System.Drawing.Point(120, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(176, 25);
            this.textBox1.TabIndex = 3;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.GotFocus += new System.EventHandler(this.textBox1_GotFocus);
            this.textBox1.LostFocus += new System.EventHandler(this.textBox1_LostFocus);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Black;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(3, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 19);
            this.label3.TabIndex = 2;
            this.label3.Text = "Image";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Black;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(3, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "Velocity";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            this.label1.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1264, 761);
            this.Controls.Add(this.pictureBoxMap);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.labelInstructions);
            this.Name = "Form1";
            this.Text = "Artillery Master";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMap)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        // Add Map button click: opens dialog to add a new map
        private void buttonAddMap_Click(object sender, EventArgs e)
        {
            // Open the addMappingDialog. Because maps don't add themselves.
            using (var dlg = new addMappingDialog())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    string name = dlg.MapName;
                    string imagePath = dlg.ImagePath;
                    double gridSize = dlg.GridSize; // <-- Get grid size
                    // If you don't name your map, it doesn't get to play.
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        if (!comboMap.Items.Contains(name))
                            comboMap.Items.Add(name);
                        comboMap.SelectedItem = name;
                        // Store imagePath in the dictionary. Because we can't trust our memory.
                        if (!string.IsNullOrWhiteSpace(imagePath))
                            mapImagePaths[name] = imagePath;
                        mapGridSizes[name] = gridSize; // <-- Store grid size
                    }
                }
            }
        }

        // Add Projectile button click: toggles the projectile add/edit panel
        private void buttonAddProjectile_Click(object sender, EventArgs e)
        {
            // Toggle the bottom panel's visibility. Hide and seek, but with UI.
            panel2.Visible = !panel2.Visible;
        }

        private void Target_Name_Click(object sender, EventArgs e)
        {

        }

        private void TarResPoi_TextChanged(object sender, EventArgs e)
        {

        }

        // Save TRP button click: saves the current blue/red positions as a TRP
        private void saveTRP_Click(object sender, EventArgs e)
        {
            if (!weaponMapPos.HasValue || !targetMapPos.HasValue)
            {
                MessageBox.Show("Set both weapon (blue) and target (red) positions on the map first.", "TRP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string trpName = TarResPoi.Text.Trim();
            if (string.IsNullOrWhiteSpace(trpName))
            {
                MessageBox.Show("Enter a name for the TRP.", "TRP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Overwrite if exists
            var existing = trpList.FirstOrDefault(t => t.Name == trpName);
            if (existing != null)
            {
                existing.WeaponPos = weaponMapPos.Value;
                existing.TargetPos = targetMapPos.Value;
            }
            else
            {
                trpList.Add(new TargetRegistrationPoint
                {
                    Name = trpName,
                    WeaponPos = weaponMapPos.Value,
                    TargetPos = targetMapPos.Value
                });
                Target_Point.Items.Add(trpName);
            }
            Target_Point.SelectedItem = trpName;
        }

        // TRP ComboBox selection changed: sets blue/red dots to saved TRP positions
        private void Target_Point_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Target_Point.SelectedItem == null) return;
            string trpName = Target_Point.SelectedItem.ToString();
            var trp = trpList.FirstOrDefault(t => t.Name == trpName);
            if (trp != null)
            {
                weaponMapPos = trp.WeaponPos;
                targetMapPos = trp.TargetPos;
                UpdateProjectileStats();
                pictureBoxMap.Invalidate();
            }
        }

        // Delete TRP button click: removes the selected TRP
        private void delTRP_Click(object sender, EventArgs e)
        {
            if (Target_Point.SelectedItem == null) return;
            string trpName = Target_Point.SelectedItem.ToString();
            var trp = trpList.FirstOrDefault(t => t.Name == trpName);
            if (trp != null)
            {
                trpList.Remove(trp);
                Target_Point.Items.Remove(trpName);
                Target_Point.SelectedIndex = Target_Point.Items.Count > 0 ? 0 : -1;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            UpdateProjectileStats();
        }

        private void mphorkph_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the velocity label to reflect the selected unit
            string selectedUnit = mphorkph.SelectedItem as string;
            if (selectedUnit == "k/h" || selectedUnit == "km/h")
            {
                label2.Text = "Velocity (km/h)";
            }
            else if (selectedUnit == "m/h" || selectedUnit == "m/h")
            {
                label2.Text = "Velocity (m/h)";
            }
            else
            {
                label2.Text = "Velocity (m/s)";
            }
            // No PlaceholderText usage (not supported in .NET Framework 4.7.2)
            UpdateProjectileStats();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Projectile Image";
                dlg.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textBoxImagePath.Text = dlg.FileName;
                }
            }
        }

        private double GetVelocityInMetersPerSecond()
        {
            if (comboProjectile.SelectedItem == null)
                return 0.0;
            string selectedName = comboProjectile.SelectedItem.ToString();
            var proj = projectiles.Find(p => p.Name == selectedName);
            return proj != null ? proj.Velocity : 0.0;
        }

        private void textBox1_GotFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == "Projectile/Weapon's Name")
                textBox1.Text = "";
        }

        private void textBox2_GotFocus(object sender, EventArgs e)
        {
            if (textBox2.Text == "Projectile's Velocity")
                textBox2.Text = "";
        }

        private void textBox1_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
                textBox1.Text = "Projectile/Weapon's Name";
        }

        private void textBox2_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
                textBox2.Text = "Projectile's Velocity";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Confirm/Add button logic
            string name = textBox1.Text.Trim();
            string velocityText = textBox2.Text.Trim();
            string imagePath = textBoxImagePath.Text.Trim();
            string unit = mphorkph.SelectedItem as string ?? "m/s";

            // Validate name and velocity
            if (string.IsNullOrWhiteSpace(name) || name == "Projectile/Weapon's Name")
            {
                MessageBox.Show("Please enter a valid projectile name.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            double velocity;
            if (!double.TryParse(velocityText, out velocity) || velocityText == "Projectile's Velocity")
            {
                MessageBox.Show("Please enter a valid velocity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Convert velocity to m/s if needed
            if (unit == "k/h" || unit == "km/h")
                velocity = velocity / 3.6;
            else if (unit == "m/h" || unit == "mph")
                velocity = velocity * 0.44704;

            // Add to ComboBox if not already present
            if (!comboProjectile.Items.Contains(name))
                comboProjectile.Items.Add(name);

            comboProjectile.SelectedItem = name;

            // Store projectile data in the list (replace if exists)
            var existing = projectiles.FindIndex(p => p.Name == name);
            if (existing >= 0)
                projectiles[existing] = new ProjectileData { Name = name, Velocity = velocity, ImagePath = imagePath, Unit = "m/s" };
            else
                projectiles.Add(new ProjectileData { Name = name, Velocity = velocity, ImagePath = imagePath, Unit = "m/s" });

            // Reset fields and hide panel
            textBox1.Text = "Projectile/Weapon's Name";
            textBox2.Text = "Projectile's Velocity";
            textBoxImagePath.Text = "";
            mphorkph.SelectedIndex = -1;
            panel2.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Cancel button logic: just hide and reset the panel
            textBox1.Text = "Projectile/Weapon's Name";
            textBox2.Text = "Projectile's Velocity";
            textBoxImagePath.Text = "";
            mphorkph.SelectedIndex = -1;
            panel2.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Populate the edit panel with current projectiles
            var combo = editProjectilePanel.ComboEditProjectile;
            combo.Items.Clear();
            foreach (var item in comboProjectile.Items)
                combo.Items.Add(item);

            if (comboProjectile.SelectedItem != null)
                combo.SelectedItem = comboProjectile.SelectedItem;
            else if (combo.Items.Count > 0)
                combo.SelectedIndex = 0;

            // Populate fields if a projectile is selected
            UpdateEditPanelFields();

            editProjectilePanel.Visible = true;
            panel2.Visible = false;
        }

        private void UpdateEditPanelFields()
        {
            var combo = editProjectilePanel.ComboEditProjectile;
            if (combo.SelectedItem == null) return;
            string name = combo.SelectedItem.ToString();
            editProjectilePanel.EditName.Text = name;
            // For demo: velocity and image path are not stored, so leave blank or implement a dictionary if needed
            editProjectilePanel.EditVelocity.Text = "";
            editProjectilePanel.EditImagePath.Text = "";
            editProjectilePanel.EditMphorkph.SelectedIndex = -1;
        }

        private void btnEditSave_Click(object sender, EventArgs e)
        {
            var combo = editProjectilePanel.ComboEditProjectile;
            if (combo.SelectedItem == null) return;
            string oldName = combo.SelectedItem.ToString();
            string newName = editProjectilePanel.EditName.Text.Trim();
            string velocityText = editProjectilePanel.EditVelocity.Text.Trim();
            string imagePath = editProjectilePanel.EditImagePath.Text.Trim();
            string unit = editProjectilePanel.EditMphorkph.SelectedItem as string ?? "m/s";

            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Please enter a valid projectile name.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            double velocity;
            if (!double.TryParse(velocityText, out velocity))
            {
                MessageBox.Show("Please enter a valid velocity.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (unit == "k/h" || unit == "km/h")
                velocity = velocity / 3.6;
            else if (unit == "m/h" || unit == "mph")
                velocity = velocity * 0.44704;

            // Update ComboBox
            int idx = comboProjectile.Items.IndexOf(oldName);
            if (idx >= 0)
            {
                comboProjectile.Items[idx] = newName;
                combo.Items[idx] = newName;
                comboProjectile.SelectedItem = newName;
            }

            // Update projectile data
            var projIdx = projectiles.FindIndex(p => p.Name == oldName);
            if (projIdx >= 0)
                projectiles[projIdx] = new ProjectileData { Name = newName, Velocity = velocity, ImagePath = imagePath, Unit = "m/s" };

            editProjectilePanel.Visible = false;
        }

        private void btnEditRemove_Click(object sender, EventArgs e)
        {
            var combo = editProjectilePanel.ComboEditProjectile;
            if (combo.SelectedItem == null) return;
            string name = combo.SelectedItem.ToString();
            int idx = comboProjectile.Items.IndexOf(name);
            if (idx >= 0)
            {
                comboProjectile.Items.RemoveAt(idx);
                combo.Items.RemoveAt(idx);
            }
            // Remove from projectile data list
            projectiles.RemoveAll(p => p.Name == name);

            if (comboProjectile.Items.Count > 0)
                comboProjectile.SelectedIndex = 0;
            editProjectilePanel.Visible = false;
        }

        private void btnEditCancel_Click(object sender, EventArgs e)
        {
            editProjectilePanel.Visible = false;
        }

        private void btnEditBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Select Projectile Image";
                dlg.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
                dlg.RestoreDirectory = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    editProjectilePanel.EditImagePath.Text = dlg.FileName;
                }
            }
        }

        private void StyleButton(Button btn)
        {
            btn.BackColor = Color.Black;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.FlatAppearance.BorderColor = Color.White;
        }

        private void StyleComboBox(ComboBox combo)
        {
            combo.FlatStyle = FlatStyle.Flat;
            combo.BackColor = Color.Black;
            combo.ForeColor = Color.White;
            combo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            // Optional: add a white border by handling DrawItem or Paint if you want to go extra fancy
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        // Add this method to handle map selection changes
        private void ComboMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMap = comboMap.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedMap) && mapImagePaths.ContainsKey(selectedMap))
            {
                try
                {
                    mapImage = Image.FromFile(mapImagePaths[selectedMap]);
                }
                catch
                {
                    mapImage = null;
                }
            }
            else
            {
                mapImage = null;
            }
            // Set grid size for selected map
            if (!string.IsNullOrEmpty(selectedMap) && mapGridSizes.ContainsKey(selectedMap))
                currentGridSize = mapGridSizes[selectedMap];
            else
                currentGridSize = 100.0;
            UpdateProjectileStats();
            pictureBoxMap.Invalidate();
        }

        // Mouse event handlers: update positions and trigger calculation
        private void PictureBoxMap_MouseDown(object sender, MouseEventArgs e)
        {
            // Left click: set weapon. Right click: set target. Middle click: go on a pan-ic.
            if (e.Button == MouseButtons.Left)
            {
                var mapPt = ScreenToMap(e.Location);
                weaponMapPos = ClampToMapBounds(mapPt);
                UpdateProjectileStats();
                pictureBoxMap.Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                var mapPt = ScreenToMap(e.Location);
                targetMapPos = ClampToMapBounds(mapPt);
                UpdateProjectileStats();
                pictureBoxMap.Invalidate();
            }
            else if (e.Button == MouseButtons.Middle)
            {
                panStart = e.Location;
                pictureBoxMap.Cursor = Cursors.SizeAll;
            }
        }

        private void PictureBoxMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                panStart = null;
                pictureBoxMap.Cursor = Cursors.Default;
            }
        }

        private void PictureBoxMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (panStart.HasValue && e.Button == MouseButtons.Middle)
            {
                var delta = new Point(e.X - panStart.Value.X, e.Y - panStart.Value.Y);
                mapPan = new Point(mapPan.X + delta.X, mapPan.Y + delta.Y);
                panStart = e.Location;
                pictureBoxMap.Invalidate();
            }
        }

        private void PictureBoxMap_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldZoom = mapZoom;
            if (e.Delta > 0) mapZoom *= 1.1f;
            else mapZoom /= 1.1f;
            if (mapZoom < 0.1f) mapZoom = 0.1f;
            if (mapZoom > 10f) mapZoom = 10f;
            pictureBoxMap.Invalidate();
        }

        // Helper: convert screen (PictureBox) to map coordinates
        private PointF ScreenToMap(Point pt)
        {
            float x = (pt.X - mapPan.X - pictureBoxMap.Width / 2f) / mapZoom + (mapImage != null ? mapImage.Width / 2f : 0);
            float y = (pt.Y - mapPan.Y - pictureBoxMap.Height / 2f) / mapZoom + (mapImage != null ? mapImage.Height / 2f : 0);
            return new PointF(x, y);
        }

        // Helper: convert map to screen (PictureBox) coordinates
        private Point MapToScreen(PointF pt)
        {
            float x = (pt.X - (mapImage != null ? mapImage.Width / 2f : 0)) * mapZoom + pictureBoxMap.Width / 2f + mapPan.X;
            float y = (pt.Y - (mapImage != null ? mapImage.Height / 2f : 0)) * mapZoom + pictureBoxMap.Height / 2f + mapPan.Y;
            return new Point((int)x, (int)y);
        }

        // Paint event: draw map, grid, dots, line, and distance
        private void PictureBoxMap_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.DimGray);

            // Draw map image
            if (mapImage != null)
            {
                var imgRect = new Rectangle(
                    (int)(pictureBoxMap.Width / 2f - mapImage.Width / 2f * mapZoom + mapPan.X),
                    (int)(pictureBoxMap.Height / 2f - mapImage.Height / 2f * mapZoom + mapPan.Y),
                    (int)(mapImage.Width * mapZoom),
                    (int)(mapImage.Height * mapZoom)
                );
                g.DrawImage(mapImage, imgRect);
            }

            // Draw grid
            if (mapImage != null && currentGridSize > 0)
            {
                Pen gridPen = new Pen(Color.FromArgb(120, 200, 200, 200), 1f);
                float gridPx = (float)(currentGridSize * mapZoom);
                int nX = (int)Math.Ceiling(mapImage.Width / currentGridSize);
                int nY = (int)Math.Ceiling(mapImage.Height / currentGridSize);

                for (int i = 0; i <= nX; ++i)
                {
                    float gx = i * (float)currentGridSize;
                    var p1 = MapToScreen(new PointF(gx, 0));
                    var p2 = MapToScreen(new PointF(gx, mapImage.Height));
                    g.DrawLine(gridPen, p1, p2);
                }
                for (int j = 0; j <= nY; ++j)
                {
                    float gy = j * (float)currentGridSize;
                    var p1 = MapToScreen(new PointF(0, gy));
                    var p2 = MapToScreen(new PointF(mapImage.Width, gy));
                    g.DrawLine(gridPen, p1, p2);
                }
                gridPen.Dispose();
            }

            // Draw weapon (blue) and target (red) dots
            if (weaponMapPos.HasValue)
            {
                var pt = MapToScreen(weaponMapPos.Value);
                g.FillEllipse(Brushes.Blue, pt.X - 6, pt.Y - 6, 12, 12);
                g.DrawEllipse(Pens.Black, pt.X - 6, pt.Y - 6, 12, 12);
            }
            if (targetMapPos.HasValue)
            {
                var pt = MapToScreen(targetMapPos.Value);
                g.FillEllipse(Brushes.Red, pt.X - 6, pt.Y - 6, 12, 12);
                g.DrawEllipse(Pens.Black, pt.X - 6, pt.Y - 6, 12, 12);
            }

            // Draw line and distance
            if (weaponMapPos.HasValue && targetMapPos.HasValue)
            {
                var p1 = MapToScreen(weaponMapPos.Value);
                var p2 = MapToScreen(targetMapPos.Value);
                g.DrawLine(new Pen(Color.White, 2), p1, p2);

                // Distance in map units (meters)
                double dx = weaponMapPos.Value.X - targetMapPos.Value.X;
                double dy = weaponMapPos.Value.Y - targetMapPos.Value.Y;
                double dist = Math.Sqrt(dx * dx + dy * dy);

                // Draw distance label under the line midpoint
                var mid = new PointF((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2 + 10);
                string distText = $"{dist:F1} m";
                var textSize = g.MeasureString(distText, this.Font);
                g.DrawString(distText, this.Font, Brushes.White, mid.X - textSize.Width / 2, mid.Y);
            }
        }

        // Add this method to update the stats panel
        private void UpdateProjectileStats()
        {
            if (weaponMapPos.HasValue && targetMapPos.HasValue && comboProjectile.SelectedItem != null)
            {
                double velocity = GetVelocityInMetersPerSecond();
                if (velocity > 0)
                {
                    // Calculate distance in map units (meters)
                    double dx = weaponMapPos.Value.X - targetMapPos.Value.X;
                    double dy = weaponMapPos.Value.Y - targetMapPos.Value.Y;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    // Use C# static class for calculations
                    double azimuth = ArtilleryCalculator.CalculateAzimuth(
                        weaponMapPos.Value.X, weaponMapPos.Value.Y,
                        targetMapPos.Value.X, targetMapPos.Value.Y);

                    double elevLow = ArtilleryCalculator.CalculateElevation(distance, velocity, false);
                    double elevHigh = ArtilleryCalculator.CalculateElevation(distance, velocity, true);

                    double tofLow = ArtilleryCalculator.CalculateTimeOfFlight(distance, velocity, false);
                    double tofHigh = ArtilleryCalculator.CalculateTimeOfFlight(distance, velocity, true);

                    valueElevation.Text = $"{elevLow:F2}° or {elevHigh:F2}°";
                    valueAzimuth.Text = $"{azimuth:F2}°";
                    valueDistance.Text = $"{distance:F1} m / {(distance / 1609.34):F2} mi";
                    valueTimeOfFlight.Text = $"{tofLow:F2} or {tofHigh:F2} s";
                }
                else
                {
                    valueElevation.Text = "-";
                    valueAzimuth.Text = "-";
                    valueDistance.Text = "-";
                    valueTimeOfFlight.Text = "-";
                }
            }
            else
            {
                valueElevation.Text = "-";
                valueAzimuth.Text = "-";
                valueDistance.Text = "-";
                valueTimeOfFlight.Text = "-";
            }
            panelRight.Refresh();
        }

        // Add this event handler anywhere in the Form1 class (e.g., after other event handlers)
        private void comboProjectile_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateProjectileStats();
        }

        // Add this helper method to clamp a PointF to the map image bounds
        private PointF ClampToMapBounds(PointF pt)
        {
            // ClampToMapBounds: because dots can't go on vacation off the map!
            if (mapImage == null)
                return pt;
            float x = Math.Max(0, Math.Min(mapImage.Width, pt.X));
            float y = Math.Max(0, Math.Min(mapImage.Height, pt.Y));
            return new PointF(x, y);
        }

        // Custom DrawItem handler for fancy ComboBoxes
        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            e.DrawBackground();

            // Set background and text color
            Color backColor = Color.Black;
            Color foreColor = Color.White;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                backColor = Color.FromArgb(40, 40, 40);
                foreColor = Color.Cyan;
            }

            using (SolidBrush bg = new SolidBrush(backColor))
                e.Graphics.FillRectangle(bg, e.Bounds);

            if (e.Index >= 0 && combo.Items.Count > 0)
            {
                string text = combo.Items[e.Index].ToString();
                using (SolidBrush brush = new SolidBrush(foreColor))
                    e.Graphics.DrawString(text, combo.Font, brush, e.Bounds.Left + 2, e.Bounds.Top + 1);
            }

            // Draw white border around the dropdown item
            using (Pen borderPen = new Pen(Color.White, 1))
                e.Graphics.DrawRectangle(borderPen, e.Bounds.Left, e.Bounds.Top, e.Bounds.Width - 1, e.Bounds.Height - 1);

            e.DrawFocusRectangle();
        }

        // --- Save/Load Methods ---

        private void SaveAllData()
        {
            try
            {
                using (var sw = new StreamWriter(SaveFilePath, false))
                {
                    // Save Projectiles
                    sw.WriteLine("[Projectiles]");
                    foreach (var p in projectiles)
                        sw.WriteLine($"{p.Name}|{p.Velocity}|{p.ImagePath}|{p.Unit}");

                    // Save Maps
                    sw.WriteLine("[Maps]");
                    foreach (var mapObj in comboMap.Items)
                    {
                        string name = mapObj.ToString();
                        string img = mapImagePaths.ContainsKey(name) ? mapImagePaths[name] : "";
                        double grid = mapGridSizes.ContainsKey(name) ? mapGridSizes[name] : 100.0; // <-- Use stored grid size
                        sw.WriteLine($"{name}|{img}|{grid}");
                    }

                    // Save TRPs
                    sw.WriteLine("[TRP]");
                    foreach (var trp in trpList)
                        sw.WriteLine($"{trp.Name}|{trp.WeaponPos.X}|{trp.WeaponPos.Y}|{trp.TargetPos.X}|{trp.TargetPos.Y}");
                }
            }
            catch { /* Optionally log or show error */ }
        }

        private void LoadAllData()
        {
            if (!File.Exists(SaveFilePath))
                return;

            try
            {
                string section = "";
                foreach (var line in File.ReadAllLines(SaveFilePath))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        section = line;
                        continue;
                    }

                    if (section == "[Projectiles]")
                    {
                        var parts = line.Split('|');
                        if (parts.Length >= 4)
                        {
                            var p = new ProjectileData
                            {
                                Name = parts[0],
                                Velocity = double.TryParse(parts[1], out double v) ? v : 0,
                                ImagePath = parts[2],
                                Unit = parts[3]
                            };
                            projectiles.Add(p);
                            if (!comboProjectile.Items.Contains(p.Name))
                                comboProjectile.Items.Add(p.Name);
                        }
                    }
                    else if (section == "[Maps]")
                    {
                        var parts = line.Split('|');
                        if (parts.Length >= 3)
                        {
                            string name = parts[0];
                            string img = parts[1];
                            double grid = 100.0;
                            double.TryParse(parts[2], out grid);
                            if (!comboMap.Items.Contains(name))
                                comboMap.Items.Add(name);
                            if (!string.IsNullOrWhiteSpace(img))
                                mapImagePaths[name] = img;
                            mapGridSizes[name] = grid; // <-- Restore grid size
                        }
                    }
                    else if (section == "[TRP]")
                    {
                        var parts = line.Split('|');
                        if (parts.Length >= 5)
                        {
                            var trp = new TargetRegistrationPoint
                            {
                                Name = parts[0],
                                WeaponPos = new PointF(float.Parse(parts[1]), float.Parse(parts[2])),
                                TargetPos = new PointF(float.Parse(parts[3]), float.Parse(parts[4]))
                            };
                            trpList.Add(trp);
                            if (!Target_Point.Items.Contains(trp.Name))
                                Target_Point.Items.Add(trp.Name);
                        }
                    }
                }
                // Optionally select first items
                if (comboProjectile.Items.Count > 0 && comboProjectile.SelectedIndex == -1)
                    comboProjectile.SelectedIndex = 0;
                if (comboMap.Items.Count > 0 && comboMap.SelectedIndex == -1)
                    comboMap.SelectedIndex = 0;
                if (Target_Point.Items.Count > 0 && Target_Point.SelectedIndex == -1)
                    Target_Point.SelectedIndex = 0;
            }
            catch
            {
                // Optionally log or show error
            }
        }
    }

    // ProjectileData: because every projectile has a story.
    public class ProjectileData
    {
        public string Name { get; set; }
        public double Velocity { get; set; }
        public string ImagePath { get; set; }
        public string Unit { get; set; } // "m/s", "km/h", "mph"
    }

    // ArtilleryCalculator: does the math so you don't have to.
    public static class ArtilleryCalculator
    {
        // Returns elevation angle in degrees for given distance, velocity, and arch type
        public static double CalculateElevation(double distance, double velocity, bool highArch)
        {
            // Physics! Now with 100% more gravity.
            const double g = 9.80665;
            double v2 = velocity * velocity;
            double arg = (g * distance) / v2;
            if (arg > 1.0) return 0.0; // out of range, like my GPA
            double theta = 0.5 * Math.Asin(arg);
            if (highArch)
                theta = Math.PI / 2 - theta;
            return theta * 180.0 / Math.PI;
        }

        public static double CalculateAzimuth(double x0, double y0, double x1, double y1)
        {
            // Azimuth: which way is that again?
            double dx = x1 - x0;
            double dy = y1 - y0;
            double angle = Math.Atan2(dx, -dy) * 180.0 / Math.PI;
            if (angle < 0) angle += 360.0;
            return angle;
        }

        public static double CalculateTimeOfFlight(double distance, double velocity, bool highArch)
        {            
            // Time of flight: how long until you regret this shot?
            double theta = CalculateElevation(distance, velocity, highArch) * Math.PI / 180.0;
            if (velocity == 0) return 0.0; // Don't divide by zero, that's illegal in most countries
            return distance / (velocity * Math.Cos(theta));
        }
    }

    // Add this C# class at the namespace level (outside Form1)
    public class TargetRegistrationPoint
    {
        public string Name { get; set; }
        public PointF WeaponPos { get; set; }
        public PointF TargetPos { get; set; }
    }
}
