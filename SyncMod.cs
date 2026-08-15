using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;

[assembly: AssemblyTitle("HyperMC Executor ModSync")]
[assembly: AssemblyDescription("HyperMC Executor ModSync v2.0.0 - Modpack Synchronization Tool for Minecraft")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("HyperMC")]
[assembly: AssemblyProduct("HyperMC Executor ModSync")]
[assembly: AssemblyCopyright("Copyright © 2026 c0devX-fonq")]
[assembly: AssemblyTrademark("HyperMC")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
[assembly: AssemblyInformationalVersion("2.0.0")]

namespace HyperMC.ModSync
{
    public class ModSyncForm : Form
    {
        // UI Layout Panels
        private Panel pnlSidebar;
        private Panel pnlMainContent;
        private Panel pnlProfileHeader;

        // UI Controls
        private Label lblAppTitle;
        private Label lblSubtitle;
        private Label lblSidebarHeader;
        private Label lblPresetTitle;
        private Label lblUrlTitle;
        private Label lblPathTitle;
        private Label lblLogTitle;
        private Label lblStatus;
        private ProgressBar progressBar;
        private Label lblProgressText;
        private ComboBox cbPathPresets;
        private TextBox txtModpackUrl;
        private Button btnPasteUrl;
        private TextBox txtPath;
        private Button btnBrowse;
        private Button btnSync;
        private Button btnOpenMods;
        private Button btnCleanBackups;
        private Button btnLangToggle;
        private RichTextBox txtLog;

        // Launcher Sidebar Buttons List & Active Indicator
        private List<Button> sidebarLauncherBtns = new List<Button>();
        private Button activeSidebarBtn = null;
        private Panel pnlActiveIndicator;

        // Config & State variables
        private string userSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HyperMCModSync", "settings.json");
        private string modpackUrl = "https://drive.google.com/uc?export=download&id=YOUR_FILE_ID";
        private string serverName = "HyperMC";
        private string authorCredit = "Tool by c0devX-fonq";
        private string targetVersion = "Fabric 1.21.1";
        private string minecraftPath = "";
        private bool isVietnamese = true; // Default language: Vietnamese
        private bool hasPatched = false; // Tracks if patched at least once in session
        private bool isDownloading = false; // Tracks active download status
        private WebClient currentWebClient = null; // Active WebClient reference for cancellation
        private bool isInitializing = true; // Prevents premature event handling during initialization
        private bool isBtnSyncPressed = false; // Tracks 3D button click/pressed state

        // Minecraft Launcher Palette
        private readonly Color bgSidebar = Color.FromArgb(24, 25, 29);         // #18191D (Dark Minecraft Launcher Sidebar)
        private readonly Color bgSidebarHover = Color.FromArgb(38, 40, 46);    // #26282E
        private readonly Color bgSidebarActive = Color.FromArgb(46, 48, 56);   // #2E3038
        private readonly Color bgMain = Color.FromArgb(16, 17, 20);           // #101114 (Main Canvas)
        private readonly Color bgCard = Color.FromArgb(28, 30, 36);           // #1C1E24 (Card Panels)
        private readonly Color bgInput = Color.FromArgb(20, 21, 26);          // #14151A
        private readonly Color borderCol = Color.FromArgb(48, 52, 62);         // #30343E
        private readonly Color mcGreen = Color.FromArgb(46, 160, 67);          // #2EA043 (Minecraft Launcher Play Green)
        private readonly Color textMain = Color.FromArgb(240, 242, 245);       // #F0F2F5
        private readonly Color textMuted = Color.FromArgb(145, 148, 160);      // #9194A0
        private readonly Color btnCancel = Color.FromArgb(160, 40, 45);        // Dark Red for Cancel

        public ModSyncForm()
        {
            try
            {
                InitializeComponent();
                LoadConfig();
                CreateSidebarLauncherButtons();
                isInitializing = false;
                AutoDetectAllLauncherPaths();
                UpdateLanguageStrings();
                CheckBackupButtonVisibility();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Lỗi khởi chạy ứng dụng HyperMC ModSync: {0}\n\nTrace: {1}", ex.Message, ex.StackTrace), "HyperMC ModSync • Lỗi Khởi Chạy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "HyperMC Executor // ModSync v2.0.0";
            this.Size = new Size(900, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = bgMain;
            this.ForeColor = textMain;
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

            // Load window icon if present
            try
            {
                if (File.Exists("icon.ico"))
                {
                    this.Icon = new Icon("icon.ico");
                }
                else
                {
                    string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");
                    if (File.Exists(iconPath))
                    {
                        this.Icon = new Icon(iconPath);
                    }
                }
            }
            catch { }

            // 1. LEFT SIDEBAR PANEL (Minecraft Launcher Style)
            pnlSidebar = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(220, 741),
                BackColor = bgSidebar
            };
            pnlSidebar.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(borderCol, 1))
                {
                    e.Graphics.DrawLine(pen, 219, 0, 219, 741);
                }
            };
            this.Controls.Add(pnlSidebar);

            // Profile Header at top left (Like Minecraft Launcher Account Switcher)
            pnlProfileHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(219, 70),
                BackColor = Color.FromArgb(20, 21, 24)
            };

            Label lblProfileUser = new Label
            {
                Text = "🎮 HYPERMC",
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold),
                ForeColor = textMain,
                Location = new Point(14, 15),
                AutoSize = true
            };

            Label lblProfileSub = new Label
            {
                Text = string.Format("ModSync  •  {0}", authorCredit),
                Font = new Font("Segoe UI", 8F),
                ForeColor = textMuted,
                Location = new Point(16, 38),
                AutoSize = true
            };

            pnlProfileHeader.Controls.Add(lblProfileUser);
            pnlProfileHeader.Controls.Add(lblProfileSub);
            pnlSidebar.Controls.Add(pnlProfileHeader);

            lblSidebarHeader = new Label
            {
                Text = "LAUNCHERS",
                Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                ForeColor = textMuted,
                Location = new Point(16, 82),
                AutoSize = true
            };
            pnlSidebar.Controls.Add(lblSidebarHeader);

            // Active Green Indicator Bar on Left of selected launcher
            pnlActiveIndicator = new Panel
            {
                Size = new Size(4, 38),
                BackColor = mcGreen,
                Visible = false
            };
            pnlSidebar.Controls.Add(pnlActiveIndicator);

            // 2. MAIN CONTENT PANEL (Right Side)
            pnlMainContent = new Panel
            {
                Location = new Point(220, 0),
                Size = new Size(664, 741),
                BackColor = bgMain
            };
            this.Controls.Add(pnlMainContent);

            // Header Section in Main Content
            lblAppTitle = new Label
            {
                Text = "HYPERMC EXECUTOR  //  MODSYNC",
                Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold),
                ForeColor = textMain,
                AutoSize = true,
                Location = new Point(20, 18)
            };

            lblSubtitle = new Label
            {
                Text = string.Format("{0}  •  {1}", serverName, authorCredit),
                Font = new Font("Segoe UI", 9F),
                ForeColor = textMuted,
                AutoSize = true,
                Location = new Point(22, 46)
            };

            // Language Toggle Button (Top Right)
            btnLangToggle = CreateButton("🌐 VIỆT NAM", 524, 18, 120, 32, false);
            btnLangToggle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            btnLangToggle.Click += (s, e) =>
            {
                isVietnamese = !isVietnamese;
                UpdateLanguageStrings();
                SaveUserSettings(txtPath.Text);
            };

            pnlMainContent.Controls.Add(lblAppTitle);
            pnlMainContent.Controls.Add(lblSubtitle);
            pnlMainContent.Controls.Add(btnLangToggle);

            // Card Panel in Main Content
            Panel pnlCard = new Panel
            {
                Location = new Point(20, 75),
                Size = new Size(624, 645),
                BackColor = bgCard
            };
            pnlCard.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, pnlCard.ClientRectangle, borderCol, ButtonBorderStyle.Solid);
            };
            pnlMainContent.Controls.Add(pnlCard);

            // Section 2: Detected Version / Profile Title & Dropdown
            lblPresetTitle = new Label
            {
                Text = "PHIÊN BẢN / PROFILE ĐÃ PHÁT HIỆN DÀNH CHO LAUNCHER ĐÃ CHỌN",
                Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                ForeColor = textMuted,
                Location = new Point(20, 15),
                AutoSize = true
            };
            pnlCard.Controls.Add(lblPresetTitle);

            cbPathPresets = new ComboBox
            {
                Location = new Point(20, 35),
                Size = new Size(584, 28),
                BackColor = bgInput,
                ForeColor = textMain,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F)
            };
            cbPathPresets.SelectedIndexChanged += CbPathPresets_SelectedIndexChanged;
            pnlCard.Controls.Add(cbPathPresets);

            // Section 3: Dynamic Modpack URL Title & Input Textbox
            lblUrlTitle = new Label
            {
                Text = "LINK TẢI MODPACK (GOOGLE DRIVE / DIRECT LINK)",
                Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                ForeColor = textMuted,
                Location = new Point(20, 75),
                AutoSize = true
            };
            pnlCard.Controls.Add(lblUrlTitle);

            txtModpackUrl = new TextBox
            {
                Location = new Point(20, 95),
                Size = new Size(474, 28),
                BackColor = bgInput,
                ForeColor = textMain,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9F)
            };
            pnlCard.Controls.Add(txtModpackUrl);

            btnPasteUrl = CreateButton("📋 DÁN LINK", 504, 94, 100, 28, false);
            btnPasteUrl.Click += BtnPasteUrl_Click;
            pnlCard.Controls.Add(btnPasteUrl);

            // Section 4: Target Path Title & Textbox
            lblPathTitle = new Label
            {
                Text = "ĐƯỜNG DẪN THƯ MỤC MODS SẼ ĐỒNG BỘ",
                Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                ForeColor = textMuted,
                Location = new Point(20, 135),
                AutoSize = true
            };
            pnlCard.Controls.Add(lblPathTitle);

            txtPath = new TextBox
            {
                Location = new Point(20, 155),
                Size = new Size(474, 28),
                BackColor = bgInput,
                ForeColor = textMain,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 9F)
            };
            txtPath.TextChanged += (s, e) => CheckBackupButtonVisibility();
            pnlCard.Controls.Add(txtPath);

            btnBrowse = CreateButton("DUYỆT...", 504, 154, 100, 28, false);
            btnBrowse.Click += BtnBrowse_Click;
            pnlCard.Controls.Add(btnBrowse);

            // Status Label & Download Speed
            lblStatus = new Label
            {
                Text = "Sẵn sàng đồng bộ modpack.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = textMain,
                Location = new Point(20, 195),
                Size = new Size(400, 22)
            };
            pnlCard.Controls.Add(lblStatus);

            lblProgressText = new Label
            {
                Text = "0 MB / 0 MB (0%)",
                Font = new Font("Consolas", 9F),
                ForeColor = textMuted,
                Location = new Point(420, 195),
                Size = new Size(184, 22),
                TextAlign = ContentAlignment.MiddleRight
            };
            pnlCard.Controls.Add(lblProgressText);

            // Progress Bar
            progressBar = new ProgressBar
            {
                Location = new Point(20, 220),
                Size = new Size(584, 8),
                Style = ProgressBarStyle.Blocks,
                Value = 0
            };
            pnlCard.Controls.Add(progressBar);

            // AUTHENTIC MINECRAFT 3D PIXELATED BUTTON WITH PRESSED SUNKEN EFFECT
            btnSync = new Button
            {
                Text = "⟳  ĐỒNG BỘ MODPACK NGAY",
                Location = new Point(20, 238),
                Size = new Size(410, 48),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSync.FlatAppearance.BorderSize = 0;
            btnSync.MouseEnter += (s, e) => btnSync.Invalidate();
            btnSync.MouseLeave += (s, e) => btnSync.Invalidate();
            btnSync.MouseDown += (s, e) => { isBtnSyncPressed = true; btnSync.Invalidate(); };
            btnSync.MouseUp += (s, e) => { isBtnSyncPressed = false; btnSync.Invalidate(); };

            // Custom GDI+ Painter for 3D Inset/Sunken Minecraft Button
            btnSync.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                Rectangle rect = btnSync.ClientRectangle;

                Point mousePos = btnSync.PointToClient(Cursor.Position);
                bool isHovered = rect.Contains(mousePos);
                bool isPressed = isBtnSyncPressed;

                Color bgCol, topLight, bottomDark, shadowTextCol;
                if (isDownloading)
                {
                    bgCol = isHovered ? Color.FromArgb(190, 45, 50) : Color.FromArgb(160, 35, 40);
                    topLight = Color.FromArgb(220, 80, 85);
                    bottomDark = Color.FromArgb(100, 20, 25);
                    shadowTextCol = Color.FromArgb(60, 10, 15);
                }
                else
                {
                    bgCol = isHovered ? Color.FromArgb(68, 168, 62) : Color.FromArgb(56, 148, 50);
                    topLight = Color.FromArgb(87, 185, 71);
                    bottomDark = Color.FromArgb(29, 89, 24);
                    shadowTextCol = Color.FromArgb(21, 62, 16);
                }

                // If Pressed (lún xuống): swap 3D highlight & shadow colors!
                Color currentTopBorder = isPressed ? bottomDark : topLight;
                Color currentBottomBorder = isPressed ? topLight : bottomDark;

                // 1. Outer Solid Black Pixel Border (2px)
                using (SolidBrush blackBrush = new SolidBrush(Color.Black))
                {
                    g.FillRectangle(blackBrush, rect);
                }

                // 2. Main Center Background
                Rectangle innerRect = new Rectangle(2, 2, rect.Width - 4, rect.Height - 4);
                using (SolidBrush bgBrush = new SolidBrush(bgCol))
                {
                    g.FillRectangle(bgBrush, innerRect);
                }

                // 3. Top & Left 3D Bevel Line (2px thick)
                using (Pen topPen = new Pen(currentTopBorder, 2))
                {
                    g.DrawLine(topPen, 2, 3, rect.Width - 4, 3);            // Top line
                    g.DrawLine(topPen, 3, 2, 3, rect.Height - 4);           // Left line
                }

                // 4. Bottom & Right 3D Bevel Line (2px thick)
                using (Pen bottomPen = new Pen(currentBottomBorder, 2))
                {
                    g.DrawLine(bottomPen, 3, rect.Height - 3, rect.Width - 3, rect.Height - 3);  // Bottom line
                    g.DrawLine(bottomPen, rect.Width - 3, 3, rect.Width - 3, rect.Height - 3);   // Right line
                }

                // 5. Centered Text with Minecraft Pixel Drop-Shadow & Pressed Offset
                string txt = btnSync.Text;
                using (Font textFont = new Font("Segoe UI Black", 11.5F, FontStyle.Bold))
                {
                    SizeF textSize = g.MeasureString(txt, textFont);
                    float tx = (rect.Width - textSize.Width) / 2.0f + (isPressed ? 1.5f : 0f);
                    float ty = (rect.Height - textSize.Height) / 2.0f + (isPressed ? 1.5f : 0f);

                    // Draw Text Shadow (Offset +1.5px)
                    using (SolidBrush shadowBrush = new SolidBrush(shadowTextCol))
                    {
                        g.DrawString(txt, textFont, shadowBrush, tx + 1.5f, ty + 1.5f);
                    }

                    // Draw Main White Text
                    using (SolidBrush textBrush = new SolidBrush(Color.White))
                    {
                        g.DrawString(txt, textFont, textBrush, tx, ty);
                    }
                }
            };

            btnSync.Click += async (s, e) =>
            {
                if (isDownloading)
                {
                    CancelDownload();
                }
                else
                {
                    await StartSyncAsync();
                }
            };
            pnlCard.Controls.Add(btnSync);

            // Open Mods Directory Button
            btnOpenMods = CreateButton("MỞ THƯ MỤC MODS", 440, 238, 164, 48, false);
            btnOpenMods.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnOpenMods.Click += (s, e) => OpenModsFolder();
            pnlCard.Controls.Add(btnOpenMods);

            // Log Label
            lblLogTitle = new Label
            {
                Text = "NHẬT KÝ THỜI GIAN THỰC",
                Font = new Font("Segoe UI Semibold", 8F, FontStyle.Bold),
                ForeColor = textMuted,
                Location = new Point(20, 300),
                AutoSize = true
            };
            pnlCard.Controls.Add(lblLogTitle);

            // Terminal Logs Box
            txtLog = new RichTextBox
            {
                Location = new Point(20, 320),
                Size = new Size(584, 260),
                BackColor = bgMain,
                ForeColor = textMain,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 9F),
                ReadOnly = true
            };
            txtLog.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, txtLog.ClientRectangle, borderCol, ButtonBorderStyle.Solid);
            };
            pnlCard.Controls.Add(txtLog);

            // Clean Old Backups Button (Positioned at bottom of card)
            btnCleanBackups = CreateButton("🗑️ GIẢI PHÓNG DUNG LƯỢNG (XÓA MOD CŨ BACKUP)", 20, 592, 584, 34, false);
            btnCleanBackups.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnCleanBackups.BackColor = Color.FromArgb(45, 30, 35);
            btnCleanBackups.Click += (s, e) => CleanOldModBackups();
            btnCleanBackups.Visible = false;
            pnlCard.Controls.Add(btnCleanBackups);
        }

        private void CreateSidebarLauncherButtons()
        {
            sidebarLauncherBtns.Clear();
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            int startY = 105;
            int btnHeight = 42;
            int spacing = 4;

            AddSidebarBtn("🎮  Minecraft Auth", "MinecraftAuth", Path.Combine(appData, ".minecraft"), startY);
            AddSidebarBtn("🟢  TLauncher", "TLauncher", Path.Combine(appData, ".minecraft"), startY + (btnHeight + spacing) * 1);
            AddSidebarBtn("⚡  TLegacy Launcher", "TLegacy", Path.Combine(appData, ".tlauncher", "legacy", "Minecraft", "game"), startY + (btnHeight + spacing) * 2);
            AddSidebarBtn("🔷  Modrinth App", "Modrinth", Path.Combine(appData, "ModrinthApp", "profiles"), startY + (btnHeight + spacing) * 3);
            AddSidebarBtn("🧊  Prism / MultiMC", "Prism", Path.Combine(appData, "PrismLauncher", "instances"), startY + (btnHeight + spacing) * 4);
            AddSidebarBtn("⚔️  CurseForge", "CurseForge", Path.Combine(userProfile, "curseforge", "minecraft", "Instances"), startY + (btnHeight + spacing) * 5);
            AddSidebarBtn("🚀  SKLauncher", "SKLauncher", Path.Combine(appData, ".sklauncher"), startY + (btnHeight + spacing) * 6);
            AddSidebarBtn("📁  Khác / Custom...", "Custom", "", startY + (btnHeight + spacing) * 7);

            // Select default first item
            if (sidebarLauncherBtns.Count > 0)
            {
                SelectSidebarLauncher(sidebarLauncherBtns[0], "MinecraftAuth", "Minecraft Auth", Path.Combine(appData, ".minecraft"));
            }
        }

        private void AddSidebarBtn(string name, string key, string defaultPath, int yPos)
        {
            Button btn = new Button
            {
                Text = "  " + name,
                Location = new Point(0, yPos),
                Size = new Size(219, 42),
                BackColor = bgSidebar,
                ForeColor = textMain,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => { if (btn != activeSidebarBtn) btn.BackColor = bgSidebarHover; };
            btn.MouseLeave += (s, e) => { if (btn != activeSidebarBtn) btn.BackColor = bgSidebar; };
            btn.Click += (s, e) => SelectSidebarLauncher(btn, key, name.Trim(), defaultPath);

            pnlSidebar.Controls.Add(btn);
            sidebarLauncherBtns.Add(btn);
        }

        private void SelectSidebarLauncher(Button btn, string key, string name, string defaultPath)
        {
            if (activeSidebarBtn != null)
            {
                activeSidebarBtn.BackColor = bgSidebar;
            }
            activeSidebarBtn = btn;
            activeSidebarBtn.BackColor = bgSidebarActive;

            // Move Active Green Indicator Bar to left of this button
            pnlActiveIndicator.Location = new Point(0, btn.Location.Y);
            pnlActiveIndicator.Visible = true;
            pnlActiveIndicator.BringToFront();

            if (isInitializing) return;

            if (key == "Custom" || string.IsNullOrEmpty(defaultPath))
            {
                if (txtPath != null && string.IsNullOrEmpty(txtPath.Text))
                {
                    string defaultFallback = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft", "mods");
                    txtPath.Text = FixPathSlashes(defaultFallback);
                }
                Log(isVietnamese ? "Vui lòng dán hoặc chọn đường dẫn thủ công nếu cần." : "Please paste or select custom directory if needed.");
                return;
            }

            // Scan subfolders/versions specifically for selected launcher
            ScanSubfolderVersions(key, name, defaultPath);
        }

        private void CancelDownload()
        {
            try
            {
                if (currentWebClient != null && currentWebClient.IsBusy)
                {
                    Log(isVietnamese ? "❌ Đã gửi yêu cầu hủy tải về..." : "❌ Cancellation request sent...");
                    currentWebClient.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                Log(string.Format(isVietnamese ? "Lỗi hủy tải: {0}" : "Cancel error: {0}", ex.Message));
            }
        }

        private void CheckBackupButtonVisibility()
        {
            if (isInitializing || txtPath == null) return;
            try
            {
                string modsFolder = FixPathSlashes(txtPath.Text.Trim());
                if (!string.IsNullOrEmpty(modsFolder))
                {
                    DirectoryInfo parentDirInfo = Directory.GetParent(modsFolder);
                    string parentDir = (parentDirInfo != null) ? parentDirInfo.FullName : modsFolder;
                    if (Directory.Exists(parentDir))
                    {
                        string[] backupDirs = Directory.GetDirectories(parentDir, "mods_backup_*", SearchOption.TopDirectoryOnly);
                        if (backupDirs.Length > 0 || hasPatched)
                        {
                            btnCleanBackups.Visible = true;
                            return;
                        }
                    }
                }
            }
            catch { }
            btnCleanBackups.Visible = hasPatched;
        }

        private void CleanOldModBackups(bool silentIfNone = false)
        {
            if (txtPath == null) return;
            string modsFolder = FixPathSlashes(txtPath.Text.Trim());
            if (string.IsNullOrEmpty(modsFolder)) return;

            DirectoryInfo parentDirInfo = Directory.GetParent(modsFolder);
            string parentDir = (parentDirInfo != null) ? parentDirInfo.FullName : modsFolder;

            if (!Directory.Exists(parentDir)) return;

            string[] backupDirs = Directory.GetDirectories(parentDir, "mods_backup_*", SearchOption.TopDirectoryOnly);

            if (backupDirs.Length == 0)
            {
                if (!silentIfNone)
                {
                    string msgNone = isVietnamese ? "Không tìm thấy thư mục sao lưu mod cũ nào cần dọn dẹp!" : "No old mod backup folders found to clean!";
                    MessageBox.Show(msgNone, isVietnamese ? "HyperMC ModSync • Giải Phóng Bộ Nhớ" : "HyperMC ModSync • Storage Cleanup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }

            // Calculate total disk size of backups
            long totalBytes = 0;
            foreach (var dir in backupDirs)
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(dir);
                    foreach (FileInfo fi in di.GetFiles("*", SearchOption.AllDirectories))
                    {
                        totalBytes += fi.Length;
                    }
                }
                catch { }
            }
            double totalMb = totalBytes / 1024.0 / 1024.0;

            string msgPrompt = isVietnamese 
                ? string.Format("Phát hiện {0} bản sao lưu mod cũ (Backup)!\n\nTổng dung lượng chiếm dụng: {1:F1} MB (~{2:F2} GB)\n\nBạn có muốn XÓA SẠCH các bản sao lưu này để GIẢI PHÓNG DUNG LƯỢNG ĐĨA không?", backupDirs.Length, totalMb, totalMb / 1024.0)
                : string.Format("Found {0} old mod backup folder(s)!\n\nTotal disk space: {1:F1} MB (~{2:F2} GB)\n\nDo you want to DELETE these backups to FREE UP DISK SPACE?", backupDirs.Length, totalMb, totalMb / 1024.0);

            if (MessageBox.Show(msgPrompt, isVietnamese ? "HyperMC ModSync • Giải Phóng Bộ Nhớ" : "HyperMC ModSync • Storage Cleanup", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int deletedCount = 0;
                foreach (var dir in backupDirs)
                {
                    try
                    {
                        Directory.Delete(dir, true);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        Log(string.Format("Lỗi xóa backup {0}: {1}", Path.GetFileName(dir), ex.Message));
                    }
                }
                Log(string.Format(isVietnamese ? "✓ Đã dọn dẹp thành công {0} bản sao lưu mod cũ, giải phóng {1:F1} MB đĩa!" : "✓ Successfully deleted {0} backup folder(s), freeing {1:F1} MB disk space!", deletedCount, totalMb));
                string msgCleaned = isVietnamese ? string.Format("Đã dọn dẹp {0} thư mục backup, giải phóng {1:F1} MB bộ nhớ đĩa!", deletedCount, totalMb) : string.Format("Cleaned {0} backup folder(s), freed {1:F1} MB disk space!", deletedCount, totalMb);
                MessageBox.Show(msgCleaned, isVietnamese ? "HyperMC ModSync • Dọn Dẹp Thành Công" : "HyperMC ModSync • Cleanup Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CheckBackupButtonVisibility();
            }
        }

        private void ScanSubfolderVersions(string key, string name, string defaultPath)
        {
            List<LauncherPreset> versionPresets = new List<LauncherPreset>();

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // 1. TLegacy Launcher handling:
            if (key == "TLegacy")
            {
                string gameDir = Path.Combine(appData, ".tlauncher", "legacy", "Minecraft", "game");

                // 1A. Scan home subfolders (Separate folder for version / family)
                string homeDir = Path.Combine(gameDir, "home");
                if (Directory.Exists(homeDir))
                {
                    foreach (var vDir in Directory.GetDirectories(homeDir))
                    {
                        string verName = Path.GetFileName(vDir);
                        versionPresets.Add(new LauncherPreset {
                            Name = string.Format("TLegacy Profile: {0} (Thư mục riêng)", verName),
                            Path = FixPathSlashes(EnsureModsSubfolder(vDir))
                        });
                    }
                }

                // 1B. General Root folder ("Don't use separate folders")
                if (Directory.Exists(gameDir))
                {
                    versionPresets.Add(new LauncherPreset {
                        Name = "TLegacy: Thư mục chung (Don't use separate folders)",
                        Path = FixPathSlashes(EnsureModsSubfolder(gameDir))
                    });
                }
            }

            // 2. Modrinth App profiles: ModrinthApp\profiles\<ProfileName>\mods
            if (key == "Modrinth")
            {
                string[] modrinthRoots = new string[] {
                    Path.Combine(appData, "ModrinthApp", "profiles"),
                    Path.Combine(localAppData, "ModrinthApp", "profiles"),
                    Path.Combine(appData, "com.modrinth.themely", "profiles")
                };
                foreach (var mRoot in modrinthRoots)
                {
                    if (Directory.Exists(mRoot))
                    {
                        foreach (var pDir in Directory.GetDirectories(mRoot))
                        {
                            string pName = Path.GetFileName(pDir);
                            versionPresets.Add(new LauncherPreset {
                                Name = string.Format("Modrinth Profile: {0}", pName),
                                Path = FixPathSlashes(EnsureModsSubfolder(pDir))
                            });
                        }
                    }
                }
            }

            // 3. SKLauncher profiles: .sklauncher\profiles\<ProfileName>\mods
            if (key == "SKLauncher")
            {
                string skPath = Path.Combine(appData, ".sklauncher");
                string skProfiles = Path.Combine(skPath, "profiles");
                if (Directory.Exists(skProfiles))
                {
                    foreach (var dir in Directory.GetDirectories(skProfiles))
                    {
                        string pName = Path.GetFileName(dir);
                        versionPresets.Add(new LauncherPreset {
                            Name = string.Format("SKLauncher Profile: {0}", pName),
                            Path = FixPathSlashes(EnsureModsSubfolder(dir))
                        });
                    }
                }
                if (Directory.Exists(skPath))
                {
                    versionPresets.Add(new LauncherPreset {
                        Name = "SKLauncher (.sklauncher/mods)",
                        Path = FixPathSlashes(EnsureModsSubfolder(skPath))
                    });
                }
            }

            // 4. CurseForge Instances
            if (key == "CurseForge")
            {
                string cfRoot = Path.Combine(userProfile, "curseforge", "minecraft", "Instances");
                if (Directory.Exists(cfRoot))
                {
                    foreach (var cDir in Directory.GetDirectories(cfRoot))
                    {
                        string cName = Path.GetFileName(cDir);
                        versionPresets.Add(new LauncherPreset {
                            Name = string.Format("CurseForge Profile: {0}", cName),
                            Path = FixPathSlashes(EnsureModsSubfolder(cDir))
                        });
                    }
                }
            }

            // 5. Prism Launcher Instances
            if (key == "Prism")
            {
                string prismRoot = Path.Combine(appData, "PrismLauncher", "instances");
                if (Directory.Exists(prismRoot))
                {
                    foreach (var pDir in Directory.GetDirectories(prismRoot))
                    {
                        string pName = Path.GetFileName(pDir);
                        string mcSub = Path.Combine(pDir, ".minecraft");
                        string target = Directory.Exists(mcSub) ? mcSub : pDir;
                        versionPresets.Add(new LauncherPreset {
                            Name = string.Format("Prism Profile: {0}", pName),
                            Path = FixPathSlashes(EnsureModsSubfolder(target))
                        });
                    }
                }
            }

            // 6. Official Minecraft Launcher / TLauncher (.minecraft)
            if (key == "MinecraftAuth" || key == "TLauncher")
            {
                string verDir = Path.Combine(defaultPath, "versions");
                if (Directory.Exists(verDir))
                {
                    foreach (var vDir in Directory.GetDirectories(verDir))
                    {
                        string vName = Path.GetFileName(vDir);
                        versionPresets.Add(new LauncherPreset {
                            Name = string.Format("{0} Version: {1}", name, vName),
                            Path = FixPathSlashes(EnsureModsSubfolder(vDir))
                        });
                    }
                }
                versionPresets.Add(new LauncherPreset {
                    Name = string.Format("{0} (.minecraft/mods)", name),
                    Path = FixPathSlashes(EnsureModsSubfolder(defaultPath))
                });
            }

            // Default fallback item if no subfolder detected
            if (versionPresets.Count == 0)
            {
                versionPresets.Add(new LauncherPreset {
                    Name = string.Format("{0} (Thư mục mặc định)", name),
                    Path = FixPathSlashes(EnsureModsSubfolder(defaultPath))
                });
            }

            if (versionPresets.Count > 0 && cbPathPresets != null)
            {
                cbPathPresets.Items.Clear();
                int selectedIndex = 0;
                for (int i = 0; i < versionPresets.Count; i++)
                {
                    cbPathPresets.Items.Add(versionPresets[i]);
                    // Auto match targetVersion if specified (e.g. "Fabric 1.21.1")
                    if (!string.IsNullOrEmpty(targetVersion) && versionPresets[i].Name.IndexOf(targetVersion, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        selectedIndex = i;
                    }
                }
                cbPathPresets.SelectedIndex = selectedIndex;
                Log(string.Format(isVietnamese ? "✓ Đã chọn {0} cho {1}" : "✓ Configured profile for {1}", versionPresets[selectedIndex].Name, name));
            }
        }

        private void UpdateLanguageStrings()
        {
            if (isVietnamese)
            {
                btnLangToggle.Text = "🌐 VIỆT NAM";
                lblSidebarHeader.Text = "LAUNCHERS / NỀN TẢNG";
                lblPresetTitle.Text = "PHIÊN BẢN / PROFILE ĐÃ PHÁT HIỆN DÀNH CHO LAUNCHER ĐÃ CHỌN";
                lblUrlTitle.Text = "LINK TẢI MODPACK (GOOGLE DRIVE / DIRECT LINK)";
                lblPathTitle.Text = "ĐƯỜNG DẪN THƯ MỤC MODS SẼ ĐỒNG BỘ";
                btnPasteUrl.Text = "📋 DÁN LINK";
                btnBrowse.Text = "DUYỆT...";
                btnSync.Text = isDownloading ? "✖  HỦY TẢI VỀ" : "⟳  ĐỒNG BỘ MODPACK NGAY";
                btnOpenMods.Text = "MỞ THƯ MỤC MODS";
                btnCleanBackups.Text = "🗑️ GIẢI PHÓNG DUNG LƯỢNG (XÓA MOD CŨ BACKUP)";
                lblLogTitle.Text = "NHẬT KÝ THỜI GIAN THỰC";
                if (lblStatus.Text.StartsWith("Ready") || lblStatus.Text.StartsWith("Sẵn sàng"))
                    lblStatus.Text = "Sẵn sàng đồng bộ modpack.";
            }
            else
            {
                btnLangToggle.Text = "🌐 ENGLISH";
                lblSidebarHeader.Text = "LAUNCHERS / PLATFORMS";
                lblPresetTitle.Text = "DETECTED VERSION / PROFILE FOR SELECTED LAUNCHER";
                lblUrlTitle.Text = "MODPACK DOWNLOAD URL (GOOGLE DRIVE / DIRECT LINK)";
                lblPathTitle.Text = "EXACT MODS DIRECTORY TO SYNCHRONIZE";
                btnPasteUrl.Text = "📋 PASTE LINK";
                btnBrowse.Text = "BROWSE...";
                btnSync.Text = isDownloading ? "✖  CANCEL DOWNLOAD" : "⟳  SYNC MODPACK NOW";
                btnOpenMods.Text = "OPEN MODS FOLDER";
                btnCleanBackups.Text = "🗑️ FREE UP DISK SPACE (CLEAN OLD BACKUPS)";
                lblLogTitle.Text = "TERMINAL LOGS";
                if (lblStatus.Text.StartsWith("Sẵn sàng") || lblStatus.Text.StartsWith("Ready"))
                    lblStatus.Text = "Ready to synchronize modpack.";
            }
            if (btnSync != null) btnSync.Invalidate();
        }

        private Button CreateButton(string text, int x, int y, int w, int h, bool isSidebar)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = isSidebar ? bgSidebar : bgSidebarHover,
                ForeColor = textMain,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = borderCol;
            btn.FlatAppearance.BorderSize = 1;
            btn.MouseEnter += (s, e) => { if (btn != btnSync) btn.BackColor = isDownloading ? btnCancel : bgSidebarActive; };
            btn.MouseLeave += (s, e) => { if (btn != btnSync) btn.BackColor = isSidebar ? bgSidebar : bgSidebarHover; };
            return btn;
        }

        private void Log(string message)
        {
            if (txtLog == null) return;
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => Log(message)));
                return;
            }
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            txtLog.AppendText(string.Format("[{0}] {1}\n", timestamp, message));
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private string FixPathSlashes(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            path = path.Trim('"', '\'', ' ');
            while (path.Contains("\\\\"))
            {
                path = path.Replace("\\\\", "\\");
            }
            return path;
        }

        private bool IsWebUrl(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            text = text.Trim().ToLower();
            return text.StartsWith("http://") || text.StartsWith("https://") || text.Contains("drive.google.com") || text.Contains("dropbox.com") || text.Contains("mediafire.com");
        }

        private bool IsWindowsFolderPath(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            text = text.Trim();
            return text.StartsWith("%") || Regex.IsMatch(text, @"^[a-zA-Z]:\\") || text.Contains("\\.minecraft") || text.Contains("\\AppData\\") || text.Contains("\\.tlauncher") || text.Contains("\\ModrinthApp") || text.Contains("\\.sklauncher");
        }

        private string GetConfigFilePath()
        {
            try
            {
                string appDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/');
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).TrimEnd('\\', '/');
                string userDesktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop").TrimEnd('\\', '/');

                // 1. If running directly from Desktop, NEVER spawn config on Desktop! Use AppData!
                if (appDir.Equals(desktopPath, StringComparison.OrdinalIgnoreCase) || 
                    appDir.Equals(userDesktop, StringComparison.OrdinalIgnoreCase))
                {
                    string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HyperMCModSync");
                    if (!Directory.Exists(appDataFolder)) Directory.CreateDirectory(appDataFolder);
                    return Path.Combine(appDataFolder, "config.json");
                }

                // 2. If config.json already exists alongside EXE (in non-desktop folder), use it
                string localConfig = Path.Combine(appDir, "config.json");
                if (File.Exists(localConfig))
                {
                    return localConfig;
                }

                // 3. Otherwise, store cleanly in AppData folder to keep user folders clean!
                string defaultAppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HyperMCModSync");
                if (!Directory.Exists(defaultAppDataFolder)) Directory.CreateDirectory(defaultAppDataFolder);
                return Path.Combine(defaultAppDataFolder, "config.json");
            }
            catch
            {
                string fallbackDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HyperMCModSync");
                if (!Directory.Exists(fallbackDir)) Directory.CreateDirectory(fallbackDir);
                return Path.Combine(fallbackDir, "config.json");
            }
        }

        private void LoadConfig()
        {
            string cfgPathToUse = GetConfigFilePath();
            try
            {
                if (File.Exists(cfgPathToUse))
                {
                    string json = File.ReadAllText(cfgPathToUse);
                    serverName = ExtractJsonValue(json, "serverName") ?? serverName;
                    authorCredit = ExtractJsonValue(json, "authorCredit") ?? authorCredit;
                    targetVersion = ExtractJsonValue(json, "targetVersion") ?? targetVersion;
                    string cfgUrl = ExtractJsonValue(json, "modpackUrl");
                    if (!string.IsNullOrEmpty(cfgUrl)) modpackUrl = cfgUrl;
                    string cfgPath = ExtractJsonValue(json, "minecraftPath");
                    if (!string.IsNullOrEmpty(cfgPath)) minecraftPath = Environment.ExpandEnvironmentVariables(cfgPath);
                }
                else
                {
                    SaveDefaultConfig(cfgPathToUse);
                }

                // Check saved user settings
                if (File.Exists(userSettingsPath))
                {
                    string userJson = File.ReadAllText(userSettingsPath);
                    string lastPath = ExtractJsonValue(userJson, "lastSelectedPath");
                    if (!string.IsNullOrEmpty(lastPath) && Directory.Exists(lastPath))
                    {
                        minecraftPath = lastPath;
                    }
                    string lastUrl = ExtractJsonValue(userJson, "lastModpackUrl");
                    if (!string.IsNullOrEmpty(lastUrl))
                    {
                        modpackUrl = lastUrl;
                    }
                    string lang = ExtractJsonValue(userJson, "language");
                    if (!string.IsNullOrEmpty(lang))
                    {
                        isVietnamese = (lang == "VI");
                    }
                }
            }
            catch (Exception ex)
            {
                Log(string.Format("Lỗi đọc config: {0}. Sử dụng mặc định.", ex.Message));
            }
            if (lblSubtitle != null) lblSubtitle.Text = string.Format("{0}  •  {1}", serverName, authorCredit);
            if (txtModpackUrl != null) txtModpackUrl.Text = modpackUrl;
        }

        private void SaveDefaultConfig(string targetPath)
        {
            string defaultConfig = "{\n" +
                "  \"serverName\": \"HyperMC\",\n" +
                "  \"authorCredit\": \"Tool by c0devX-fonq\",\n" +
                "  \"targetVersion\": \"Fabric 1.21.1\",\n" +
                "  \"modpackUrl\": \"https://drive.google.com/uc?export=download&id=YOUR_FILE_ID\",\n" +
                "  \"minecraftPath\": \"%APPDATA%\\\\.minecraft\"\n" +
                "}";
            string dir = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllText(targetPath, defaultConfig);
            Log(string.Format("Đã tạo file config mặc định tại: {0}", targetPath));
        }

        private void SaveUserSettings(string path)
        {
            try
            {
                string dir = Path.GetDirectoryName(userSettingsPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string langStr = isVietnamese ? "VI" : "EN";
                string currentUrl = (txtModpackUrl != null) ? txtModpackUrl.Text.Trim() : modpackUrl;
                string json = string.Format("{{\n  \"lastSelectedPath\": \"{0}\",\n  \"lastModpackUrl\": \"{1}\",\n  \"language\": \"{2}\"\n}}", FixPathSlashes(path).Replace("\\", "\\\\"), currentUrl.Replace("\\", "\\\\"), langStr);
                File.WriteAllText(userSettingsPath, json);
            }
            catch { }
        }

        private string ExtractJsonValue(string json, string key)
        {
            var match = Regex.Match(json, string.Format("\"{0}\"\\s*:\\s*\"([^\"]+)\"", key));
            return match.Success ? match.Groups[1].Value : null;
        }

        private class LauncherPreset
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public override string ToString() { return Name; }
        }

        private void AutoDetectAllLauncherPaths()
        {
            List<LauncherPreset> presets = new List<LauncherPreset>();

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // 1. Official Minecraft Launcher (Auth) (.minecraft)
            string defaultMc = Path.Combine(appData, ".minecraft");
            presets.Add(new LauncherPreset { Name = "Minecraft Launcher (Official / Auth)", Path = FixPathSlashes(EnsureModsSubfolder(defaultMc)) });

            // Custom Folder item
            string customLabel = isVietnamese ? "📁 Thư mục tùy chọn (Duyệt/Dán thủ công...)" : "📁 Custom Directory (Browse/Paste manually...)";
            presets.Add(new LauncherPreset { Name = customLabel, Path = "" });

            if (cbPathPresets != null)
            {
                cbPathPresets.Items.Clear();
                int selIdx = 0;
                for (int i = 0; i < presets.Count; i++)
                {
                    cbPathPresets.Items.Add(presets[i]);
                    if (!string.IsNullOrEmpty(targetVersion) && presets[i].Name.IndexOf(targetVersion, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        selIdx = i;
                    }
                }

                // Select initial path quietly
                if (!string.IsNullOrEmpty(minecraftPath))
                {
                    string targetMods = FixPathSlashes(EnsureModsSubfolder(minecraftPath));
                    if (txtPath != null) txtPath.Text = targetMods;
                    Log(string.Format(isVietnamese ? "Sử dụng đường dẫn đã lưu: {0}" : "Using saved mods directory: {0}", targetMods));
                }
                else if (cbPathPresets.Items.Count > 0)
                {
                    cbPathPresets.SelectedIndex = selIdx;
                }
            }
        }

        private string EnsureModsSubfolder(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            path = FixPathSlashes(path);
            if (path.EndsWith("mods", StringComparison.OrdinalIgnoreCase)) return path;
            string subMods = Path.Combine(path, "mods");
            return FixPathSlashes(subMods);
        }

        private void CbPathPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing) return;
            LauncherPreset selected = cbPathPresets.SelectedItem as LauncherPreset;
            if (selected != null)
            {
                if (!string.IsNullOrEmpty(selected.Path))
                {
                    if (txtPath != null) txtPath.Text = FixPathSlashes(selected.Path);
                    Log(string.Format(isVietnamese ? "Đã chọn Profile: {0}" : "Selected launcher profile: {0}", selected.Name));
                    SaveUserSettings(selected.Path);
                    CheckBackupButtonVisibility();
                }
            }
        }

        private void BtnPasteUrl_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clipText = Clipboard.GetText().Trim();
                    if (!string.IsNullOrEmpty(clipText))
                    {
                        if (IsWindowsFolderPath(clipText))
                        {
                            // User pasted a Windows folder path into the URL box! Auto route to txtPath!
                            string finalModsPath = FixPathSlashes(EnsureModsSubfolder(clipText));
                            if (txtPath != null) txtPath.Text = finalModsPath;
                            SaveUserSettings(finalModsPath);
                            Log(string.Format(isVietnamese 
                                ? "⚠️ PHÁT HIỆN DÁN NHẦM: Đây là đường dẫn thư mục Windows! Đã tự động chuyển vào ô '4. Đường dẫn Thư mục Mods': {0}" 
                                : "⚠️ AUTO-ROUTED: Windows folder path detected! Saved to Mods Directory box: {0}", finalModsPath));
                            MessageBox.Show(isVietnamese 
                                ? string.Format("Phát hiện bạn vừa dán một Đường Dẫn Thư Mục Windows!\n\nHyperMC ModSync đã tự động định tuyến nội dung này vào ô 'Đường Dẫn Thư Mục Mods' giúp bạn:\n📍 {0}", finalModsPath) 
                                : string.Format("Windows folder path detected!\nHyperMC ModSync automatically routed it to Mods Directory input box:\n📍 {0}", finalModsPath), 
                                isVietnamese ? "HyperMC ModSync • Tự Động Định Tuyến" : "HyperMC ModSync • Auto Route", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            if (txtModpackUrl != null) txtModpackUrl.Text = clipText;
                            SaveUserSettings(txtPath != null ? txtPath.Text : "");
                            Log(string.Format(isVietnamese ? "📋 Đã dán Link Modpack mới từ Clipboard: {0}" : "📋 Pasted new Modpack URL from Clipboard: {0}", txtModpackUrl != null ? txtModpackUrl.Text : clipText));
                        }
                    }
                }
                else
                {
                    MessageBox.Show(isVietnamese ? "Bộ nhớ tạm (Clipboard) không chứa đoạn link nào!" : "Clipboard does not contain any link text!", isVietnamese ? "HyperMC ModSync • Thông Báo" : "HyperMC ModSync • Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Log(string.Format(isVietnamese ? "Lỗi dán Link: {0}" : "Paste link error: {0}", ex.Message));
            }
        }

        private void BtnPaste_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clipText = Clipboard.GetText().Trim();
                    if (!string.IsNullOrEmpty(clipText))
                    {
                        if (IsWebUrl(clipText))
                        {
                            // User pasted a web link / Google Drive link into the folder path box! Auto route to txtModpackUrl!
                            if (txtModpackUrl != null) txtModpackUrl.Text = clipText;
                            SaveUserSettings(txtPath != null ? txtPath.Text : "");
                            Log(string.Format(isVietnamese 
                                ? "⚠️ PHÁT HIỆN DÁN NHẦM: Đây là Link Web/Google Drive! Đã tự động chuyển vào ô '3. Link Tải Modpack': {0}" 
                                : "⚠️ AUTO-ROUTED: Web/Drive URL detected! Saved to Modpack URL box: {0}", clipText));
                            MessageBox.Show(isVietnamese 
                                ? string.Format("Phát hiện bạn vừa dán một Link Web / Google Drive!\n\nHyperMC ModSync đã tự động định tuyến nội dung này vào ô 'Link Tải Modpack' giúp bạn:\n🔗 {0}", clipText) 
                                : string.Format("Web URL detected!\nHyperMC ModSync automatically routed it to Modpack Download URL input box:\n🔗 {0}", clipText), 
                                isVietnamese ? "HyperMC ModSync • Tự Động Định Tuyến" : "HyperMC ModSync • Auto Route", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            string cleanPath = FixPathSlashes(clipText);
                            string finalModsPath = FixPathSlashes(EnsureModsSubfolder(cleanPath));
                            if (txtPath != null) txtPath.Text = finalModsPath;
                            SaveUserSettings(finalModsPath);
                            Log(string.Format(isVietnamese ? "📋 Đã dán đường dẫn từ Clipboard: {0}" : "📋 Pasted path from Clipboard: {0}", finalModsPath));
                        }
                    }
                }
                else
                {
                    MessageBox.Show(isVietnamese ? "Bộ nhớ tạm (Clipboard) không chứa đoạn văn bản đường dẫn nào!" : "Clipboard does not contain any path text!", isVietnamese ? "HyperMC ModSync • Thông Báo" : "HyperMC ModSync • Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Log(string.Format(isVietnamese ? "Lỗi dán từ Clipboard: {0}" : "Clipboard paste error: {0}", ex.Message));
            }
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            // Full Modern Resizable Windows File Explorer Dialog
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = isVietnamese ? "Chọn thư mục chứa Mods hoặc Profile Minecraft của bạn:" : "Select your Minecraft Mods or Launcher Profile Folder:";
                ofd.Filter = isVietnamese ? "Thư mục / File (*.*)|*.*" : "Folders / Files (*.*)|*.*";
                ofd.CheckFileExists = false;
                ofd.CheckPathExists = true;
                ofd.ValidateNames = false;
                ofd.FileName = isVietnamese ? "CHỌN THƯ MỤC NÀY" : "SELECT THIS FOLDER";

                // Direct navigation to current folder ("dẫn vào tận cửa")
                string currentPath = FixPathSlashes(txtPath != null ? txtPath.Text.Trim() : "");
                if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
                {
                    ofd.InitialDirectory = currentPath;
                }
                else
                {
                    DirectoryInfo parentDirInfo = string.IsNullOrEmpty(currentPath) ? null : Directory.GetParent(currentPath);
                    string parent = (parentDirInfo != null) ? parentDirInfo.FullName : null;
                    if (!string.IsNullOrEmpty(parent) && Directory.Exists(parent))
                    {
                        ofd.InitialDirectory = parent;
                    }
                    else
                    {
                        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        if (Directory.Exists(appData)) ofd.InitialDirectory = appData;
                    }
                }

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string chosenPath = Path.GetDirectoryName(ofd.FileName);
                    if (string.IsNullOrEmpty(chosenPath) || !Directory.Exists(chosenPath))
                    {
                        chosenPath = ofd.FileName;
                    }

                    string finalModsPath = FixPathSlashes(EnsureModsSubfolder(chosenPath));
                    if (txtPath != null) txtPath.Text = finalModsPath;
                    SaveUserSettings(finalModsPath);
                    Log(string.Format(isVietnamese ? "Đã chọn thư mục: {0}" : "Custom folder selected: {0}", finalModsPath));
                }
            }
        }

        private void OpenModsFolder()
        {
            string path = FixPathSlashes(txtPath != null ? txtPath.Text.Trim() : "");
            if (string.IsNullOrEmpty(path)) return;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            Process.Start("explorer.exe", path);
        }

        private async Task StartSyncAsync()
        {
            string currentInputUrl = txtModpackUrl != null ? txtModpackUrl.Text.Trim() : "";
            string modsFolder = FixPathSlashes(txtPath != null ? txtPath.Text.Trim() : "");

            // Smart Swap Check before Sync
            if (IsWindowsFolderPath(currentInputUrl) && IsWebUrl(modsFolder))
            {
                // Both swapped! Auto swap back!
                string temp = currentInputUrl;
                currentInputUrl = modsFolder;
                modsFolder = FixPathSlashes(EnsureModsSubfolder(temp));
                if (txtModpackUrl != null) txtModpackUrl.Text = currentInputUrl;
                if (txtPath != null) txtPath.Text = modsFolder;
                Log(isVietnamese ? "⚠️ TỰ ĐỘNG ĐỔI LẠI VỊ TRÍ: Phát hiện dán ngược vị trí giữa Link Modpack và Thư mục Mods." : "⚠️ AUTO SWAPPED: Corrected swapped inputs for URL and Folder Path.");
            }
            else if (IsWebUrl(modsFolder))
            {
                // Web link in folder box!
                currentInputUrl = modsFolder;
                if (txtModpackUrl != null) txtModpackUrl.Text = currentInputUrl;
                if (txtPath != null) txtPath.Text = FixPathSlashes(EnsureModsSubfolder(minecraftPath));
                modsFolder = txtPath != null ? txtPath.Text : "";
                Log(isVietnamese ? "⚠️ TỰ ĐỘNG SỬA VỊ TRÍ: Chuyển Link Web từ ô Thư mục sang ô Link Modpack." : "⚠️ AUTO CORRECTED: Moved Web link to URL box.");
            }

            if (string.IsNullOrEmpty(currentInputUrl) || currentInputUrl.Contains("YOUR_FILE_ID") || currentInputUrl.Contains("link-direct-download-cua-ban.com"))
            {
                string msgErr = isVietnamese ? "Vui lòng dán Link Google Drive hoặc Link Modpack trực tiếp vào ô 'Link Tải Modpack'!" : "Please paste a valid Google Drive or Direct Modpack Download Link into the Modpack URL box!";
                MessageBox.Show(msgErr, isVietnamese ? "HyperMC ModSync • Chưa Nhập Link Modpack" : "HyperMC ModSync • Missing Modpack Link", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Log(isVietnamese ? "LỖI: Link Modpack đầu vào không hợp lệ." : "ERROR: Invalid Modpack URL input.");
                return;
            }

            if (string.IsNullOrEmpty(modsFolder))
            {
                string msgPath = isVietnamese ? "Vui lòng chọn thư mục chứa mods của Minecraft!" : "Please select your Minecraft mods directory!";
                MessageBox.Show(msgPath, isVietnamese ? "HyperMC ModSync • Chưa Chọn Thư Mục Mods" : "HyperMC ModSync • No Folder Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Directory.Exists(modsFolder))
            {
                Directory.CreateDirectory(modsFolder);
            }

            SaveUserSettings(modsFolder);

            isDownloading = true;
            btnSync.Text = isVietnamese ? "✖  HỦY TẢI VỀ" : "✖  CANCEL DOWNLOAD";
            btnSync.Invalidate();
            btnBrowse.Enabled = false;
            btnPasteUrl.Enabled = false;
            if (cbPathPresets != null) cbPathPresets.Enabled = false;
            if (txtModpackUrl != null) txtModpackUrl.Enabled = false;
            lblStatus.Text = isVietnamese ? "Đang kết nối tới máy chủ..." : "Connecting to server...";
            progressBar.Value = 0;

            string tempZip = Path.Combine(Path.GetTempPath(), "modpack_temp.zip");

            try
            {
                string directUrl = ConvertToDirectDownloadLink(currentInputUrl);
                Log(string.Format(isVietnamese ? "Đang kết nối tới nguồn tải: {0}" : "Connecting to download source: {0}", directUrl));

                using (WebClient client = new WebClient())
                {
                    currentWebClient = client;
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) HyperMCExecutorModSync/1.0");

                    Stopwatch sw = Stopwatch.StartNew();
                    client.DownloadProgressChanged += (s, e) =>
                    {
                        double mbReceived = e.BytesReceived / 1024.0 / 1024.0;
                        double mbTotal = e.TotalBytesToReceive / 1024.0 / 1024.0;
                        double speedMb = (e.BytesReceived / 1024.0 / 1024.0) / Math.Max(0.1, sw.Elapsed.TotalSeconds);

                        progressBar.Value = Math.Min(100, Math.Max(0, e.ProgressPercentage));
                        lblStatus.Text = string.Format(isVietnamese ? "Đang tải modpack... ({0:F2} MB/s)" : "Downloading mods... ({0:F2} MB/s)", speedMb);
                        lblProgressText.Text = string.Format("{0:F1} MB / {1:F1} MB ({2}%)", mbReceived, mbTotal, e.ProgressPercentage);
                    };

                    await client.DownloadFileTaskAsync(new Uri(directUrl), tempZip);
                }

                Log(isVietnamese ? "Tải dữ liệu hoàn tất. Đang kiểm tra định dạng file & cấu trúc modpack..." : "Download complete. Validating archive & modpack structure...");

                // 1. Validate ZIP Magic Header (PK\x03\x04)
                if (File.Exists(tempZip))
                {
                    FileInfo fi = new FileInfo(tempZip);
                    double sizeMb = fi.Length / 1024.0 / 1024.0;

                    byte[] header = new byte[4];
                    using (FileStream fs = File.OpenRead(tempZip))
                    {
                        if (fs.Length < 4)
                        {
                            throw new Exception(isVietnamese ? "File tải về quá nhỏ hoặc bị rỗng!" : "Downloaded file is empty!");
                        }
                        fs.Read(header, 0, 4);
                    }

                    // Check if file is NOT a ZIP (magic bytes for zip: 0x50, 0x4B)
                    if (header[0] != 0x50 || header[1] != 0x4B)
                    {
                        string downloadedContent = File.ReadAllText(tempZip);
                        if (downloadedContent.Contains("Google Drive") || downloadedContent.Contains("virus") || downloadedContent.Contains("<html"))
                        {
                            throw new Exception(isVietnamese 
                                ? "Google Drive yêu cầu xác nhận trước khi tải file dung lượng lớn.\n\nGIẢI PHÁP:\n1. Hãy đảm bảo File Google Drive đã bật chế độ 'Bất kỳ ai có liên kết đều xem được'.\n2. Hoặc upload file modpack.zip lên Mediafire, Dropbox hoặc GitHub Release để lấy link tải trực tiếp 100%!"
                                : "Google Drive displayed a confirmation HTML page.\n\nPlease ensure link is set to Public ('Anyone with link can view').");
                        }
                        else
                        {
                            throw new Exception(isVietnamese ? "File tải về từ đường link không phải là file ZIP modpack hợp lệ (Thiếu tiêu đề định dạng ZIP PK)." : "Downloaded file is not a valid ZIP archive.");
                        }
                    }

                    // 2. Validate Modpack Internal Structure (Check for .jar mod files or modpack manifests)
                    int jarCountInZip = 0;
                    bool hasManifestInZip = false;

                    using (ZipArchive testArchive = ZipFile.OpenRead(tempZip))
                    {
                        foreach (ZipArchiveEntry entry in testArchive.Entries)
                        {
                            if (entry.Name.EndsWith(".jar", StringComparison.OrdinalIgnoreCase))
                            {
                                jarCountInZip++;
                            }
                            if (entry.Name.Equals("manifest.json", StringComparison.OrdinalIgnoreCase) ||
                                entry.Name.Equals("modrinth.index.json", StringComparison.OrdinalIgnoreCase) ||
                                entry.Name.Equals("instance.cfg", StringComparison.OrdinalIgnoreCase))
                            {
                                hasManifestInZip = true;
                            }
                        }
                    }

                    if (jarCountInZip == 0 && !hasManifestInZip)
                    {
                        throw new Exception(isVietnamese 
                            ? "File vừa tải về KHÔNG PHẢI là bộ Modpack Minecraft hợp lệ!\n\nLý do: Không tìm thấy bất kỳ file mod .jar nào hoặc file cấu hình Modpack (manifest.json) bên trong file ZIP.\n\nVui lòng kiểm tra lại file đã upload lên Google Drive!"
                            : "Downloaded archive is NOT a valid Minecraft Modpack!\n\nReason: No .jar mod files or modpack manifest found inside the ZIP.");
                    }

                    Log(string.Format(isVietnamese ? "✓ Đã xác nhận bộ Modpack hợp lệ (Phát hiện {0} mod .jar bên trong file ZIP)." : "✓ Validated Modpack archive ({0} .jar mod files detected).", jarCountInZip));

                    // --- PAUSE & CONFIRMATION STEP BEFORE PATCHING ---
                    lblStatus.Text = isVietnamese ? "Tải thành công! Đang chờ xác nhận Patch..." : "Download complete! Awaiting patch confirmation...";
                    Log(string.Format(isVietnamese ? "Đã tải xong file Modpack ({0:F1} MB - {1} mod). Đang chờ xác nhận cài đặt (Patch)..." : "Downloaded Modpack archive ({0:F1} MB - {1} mods). Awaiting patch confirmation...", sizeMb, jarCountInZip));

                    string msgPrompt = isVietnamese 
                        ? string.Format("Tải thành công file Modpack ({0:F1} MB - Phát hiện {1} mod)!\n\nBạn có muốn tiến hành sao lưu mod cũ và CÀI ĐẶT (PATCH) MOD MỚI vào thư mục sau không?\n\n📍 Thư mục đích: {2}", sizeMb, jarCountInZip, modsFolder)
                        : string.Format("Modpack downloaded successfully ({0:F1} MB - {1} mods detected)!\n\nDo you want to backup existing mods and PATCH NEW MODS to this folder?\n\n📍 Target Directory: {2}", sizeMb, jarCountInZip, modsFolder);

                    DialogResult dialogResult = MessageBox.Show(msgPrompt, isVietnamese ? "HyperMC ModSync • Xác Nhận Đồng Bộ Modpack" : "HyperMC ModSync • Confirm Modpack Patch", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (dialogResult != DialogResult.Yes)
                    {
                        Log(isVietnamese ? "❌ Đã hủy quá trình Patch mod theo yêu cầu của bạn." : "❌ Modpack patch cancelled by user.");
                        lblStatus.Text = isVietnamese ? "Đã hủy Patch mod." : "Patch cancelled.";
                        return;
                    }
                }

                lblStatus.Text = isVietnamese ? "Đang sao lưu mod cũ..." : "Backing up old mods...";

                // Create Backup
                DirectoryInfo parentDirInfo = Directory.GetParent(modsFolder);
                string parentDir = (parentDirInfo != null) ? parentDirInfo.FullName : modsFolder;
                string backupFolder = Path.Combine(parentDir, string.Format("mods_backup_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss")));
                
                string[] existingJarFiles = Directory.GetFiles(modsFolder, "*.jar", SearchOption.TopDirectoryOnly);
                if (existingJarFiles.Length > 0)
                {
                    Directory.CreateDirectory(backupFolder);
                    Log(string.Format(isVietnamese ? "Đang sao lưu {0} mod hiện có vào: {1}" : "Backing up {0} existing mod(s) to: {1}", existingJarFiles.Length, backupFolder));
                    foreach (var file in existingJarFiles)
                    {
                        string dest = Path.Combine(backupFolder, Path.GetFileName(file));
                        File.Move(file, dest);
                    }
                }

                lblStatus.Text = isVietnamese ? "Đang giải nén modpack..." : "Extracting new modpack...";
                Log(isVietnamese ? "Đang giải nén các file vào thư mục mods..." : "Extracting zip contents into mods directory...");

                int extractedCount = 0;
                using (ZipArchive archive = ZipFile.OpenRead(tempZip))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name)) continue;

                        string entryName = entry.FullName;
                        if (entryName.StartsWith("mods/", StringComparison.OrdinalIgnoreCase))
                        {
                            entryName = entryName.Substring(5);
                        }
                        if (string.IsNullOrEmpty(entryName)) continue;

                        string destinationPath = Path.Combine(modsFolder, entryName);
                        string destDir = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

                        entry.ExtractToFile(destinationPath, true);
                        extractedCount++;
                    }
                }

                hasPatched = true;
                CheckBackupButtonVisibility();

                Log(string.Format(isVietnamese ? "Đã giải nén thành công {0} file vào thư mục: {1}" : "Successfully extracted {0} file(s) to mods folder: {1}", extractedCount, modsFolder));
                lblStatus.Text = isVietnamese ? "Đồng bộ modpack hoàn tất!" : "Modpack synchronization complete!";
                lblProgressText.Text = "100% COMPLETE";
                progressBar.Value = 100;
                Log(isVietnamese ? "✓ ĐỒNG BỘ MODPACK THÀNH CÔNG. Bạn có thể mở Game ngay bây giờ!" : "✓ MODPACK SYNC SUCCESSFUL. You can now launch Minecraft!");

                string msgDone = isVietnamese ? string.Format("🎮 ĐỒNG BỘ MODPACK THÀNH CÔNG!\n\nĐã cập nhật {0} mod vào thư mục:\n📍 {1}\n\nBạn có thể mở Minecraft Launcher và vào game trải nghiệm ngay bây giờ!", extractedCount, modsFolder)
                                             : string.Format("🎮 MODPACK SYNC SUCCESSFUL!\n\nUpdated {0} mod(s) in folder:\n📍 {1}\n\nYou can launch Minecraft now!", extractedCount, modsFolder);
                MessageBox.Show(msgDone, isVietnamese ? "HyperMC ModSync • Đồng Bộ Thành Công" : "HyperMC ModSync • Sync Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Option to clean old backup folders right after successful patching
                CleanOldModBackups(true);
            }
            catch (Exception ex)
            {
                if (ex is WebException && ((WebException)ex).Status == WebExceptionStatus.RequestCanceled)
                {
                    Log(isVietnamese ? "❌ Đã hủy quá trình tải xuống thành công." : "❌ Download cancelled successfully.");
                    lblStatus.Text = isVietnamese ? "Đã hủy tải về." : "Download cancelled.";
                }
                else
                {
                    Log(string.Format(isVietnamese ? "LỖI trong quá trình đồng bộ: {0}" : "ERROR during sync: {0}", ex.Message));
                    lblStatus.Text = isVietnamese ? "Đồng bộ thất bại! Kiểm tra log." : "Sync failed! Check logs.";
                    string msgFail = isVietnamese ? string.Format("Xảy ra lỗi trong quá trình đồng bộ mod:\n⚠️ {0}", ex.Message) : string.Format("Modpack sync failed:\n⚠️ {0}", ex.Message);
                    MessageBox.Show(msgFail, isVietnamese ? "HyperMC ModSync • Lỗi Đồng Bộ" : "HyperMC ModSync • Sync Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                isDownloading = false;
                currentWebClient = null;
                if (File.Exists(tempZip))
                {
                    try { File.Delete(tempZip); } catch { }
                }
                btnSync.Enabled = true;
                btnBrowse.Enabled = true;
                btnPasteUrl.Enabled = true;
                if (cbPathPresets != null) cbPathPresets.Enabled = true;
                if (txtModpackUrl != null) txtModpackUrl.Enabled = true;
                btnSync.Text = isVietnamese ? "⟳  ĐỒNG BỘ MODPACK NGAY" : "⟳  SYNC MODPACK NOW";
                btnSync.Invalidate();
            }
        }

        private string ConvertToDirectDownloadLink(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            
            // Handle Google Drive links
            if (url.Contains("drive.google.com") || url.Contains("drive.usercontent.google.com"))
            {
                string id = null;
                var matchD = Regex.Match(url, @"/d/([a-zA-Z0-9_-]+)");
                if (matchD.Success) id = matchD.Groups[1].Value;
                else
                {
                    var matchId = Regex.Match(url, @"id=([a-zA-Z0-9_-]+)");
                    if (matchId.Success) id = matchId.Groups[1].Value;
                }

                if (!string.IsNullOrEmpty(id))
                {
                    // Add confirm=t to bypass Google Drive virus scan prompt for large files
                    return string.Format("https://drive.usercontent.google.com/download?id={0}&export=download&confirm=t", id);
                }
            }

            if (url.Contains("dropbox.com"))
            {
                return url.Replace("dl=0", "dl=1");
            }
            return url;
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ModSyncForm());
        }
    }
}
