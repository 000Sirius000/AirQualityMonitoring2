using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace AirQualityMonitoring2
{
    public partial class MainForm : Form
    {
        private Dictionary<string, DistrictInfo> districts;
        private Random random = new Random();
        private Image mapImage;
        private DistrictInfo hoveredDistrict = null;
        // Змінні для збереження масштабу та зсуву карти
        private float currentScale = 1.0f;
        private int mapOffsetX = 0;
        private int mapOffsetY = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadMapImage();
            InitializeDistricts();
            CreateLegendControls();
            SetupTimer();
        }

        private void LoadMapImage()
        {
            // Спробуйте завантажити карту з різних можливих місць
            string[] possiblePaths = {
                Path.Combine(Application.StartupPath, "Resources", "kyiv_map.png"),
                Path.Combine(Application.StartupPath, "kyiv_map.png"),
                Path.Combine(Application.StartupPath, "Resources", "kyiv_map_new.gif"),
                Path.Combine(Application.StartupPath, "kyiv_map_new.gif"),
                "kyiv_map.png",
                "kyiv_map_new.gif"
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        mapImage = Image.FromFile(path);
                        break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            if (mapImage == null)
            {
                MessageBox.Show("Карта не знайдена! Додайте файл kyiv_map.png або kyiv_map.jpg в папку програми або в папку Resources.",
                    "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeDistricts()
        {
            districts = new Dictionary<string, DistrictInfo>();

            // Правий берег
            districts["Оболонський"] = new DistrictInfo("Оболонський", new Point(169, 53), GenerateAQI());
            districts["Подільський"] = new DistrictInfo("Подільський", new Point(157, 130), GenerateAQI());
            districts["Святошинський"] = new DistrictInfo("Святошинський", new Point(84, 190), GenerateAQI());
            districts["Шевченківський"] = new DistrictInfo("Шевченківський", new Point(240, 197), GenerateAQI());
            districts["Солом'янський"] = new DistrictInfo("Солом'янський", new Point(217, 286), GenerateAQI());
            districts["Голосіївський"] = new DistrictInfo("Голосіївський", new Point(284, 359), GenerateAQI());
            districts["Печерський"] = new DistrictInfo("Печерський", new Point(313, 279), GenerateAQI());

            // Лівий берег
            districts["Деснянський"] = new DistrictInfo("Деснянський", new Point(459, 134), GenerateAQI());
            districts["Дніпровський"] = new DistrictInfo("Дніпровський", new Point(344, 190), GenerateAQI());
            districts["Дарницький"] = new DistrictInfo("Дарницький", new Point(451, 298), GenerateAQI());

            // Заповнюємо ComboBox
            districtComboBox.Items.Add("Всі райони");
            foreach (var district in districts.Keys.OrderBy(k => k))
            {
                districtComboBox.Items.Add(district);
            }
            districtComboBox.SelectedIndex = 0;
        }

        private void CreateLegendControls()
        {
            var aqiLevels = new[]
            {
                new { Range = "0-50", Quality = "Добре", Color = Color.FromArgb(0, 200, 0) },
                new { Range = "51-100", Quality = "Помірно", Color = Color.FromArgb(255, 220, 0) },
                new { Range = "101-150", Quality = "Нездорове для чутливих", Color = Color.FromArgb(255, 100, 0) },
                new { Range = "151-200", Quality = "Нездорове", Color = Color.FromArgb(255, 0, 0) },
                new { Range = "201-300", Quality = "Дуже нездорове", Color = Color.FromArgb(180, 0, 180) },
                new { Range = "301+", Quality = "Небезпечне", Color = Color.FromArgb(140, 0, 33) }
            };

            int y = 50;
            foreach (var level in aqiLevels)
            {
                Panel colorBox = new Panel
                {
                    BackColor = level.Color,
                    Location = new Point(10, y),
                    Size = new Size(30, 20),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label rangeLabel = new Label
                {
                    Text = level.Range,
                    Location = new Point(50, y),
                    Size = new Size(60, 20)
                };

                Label qualityLabel = new Label
                {
                    Text = level.Quality,
                    Location = new Point(110, y),
                    Size = new Size(130, 20),
                    Font = new Font("Segoe UI", 9)
                };

                legendPanel.Controls.AddRange(new Control[] { colorBox, rangeLabel, qualityLabel });
                y += 30;
            }

            // Додаткова інформація
            GroupBox infoBox = new GroupBox
            {
                Text = "Інформація",
                Location = new Point(10, y + 20),
                Size = new Size(230, 200)
            };

            Label infoLabel = new Label
            {
                Text = "AQI (Air Quality Index) - індекс якості повітря.\n\n" +
                      "Показники:\n" +
                      "• PM2.5 - дрібні частинки\n" +
                      "• PM10 - крупні частинки\n" +
                      "• NO2 - діоксид азоту\n" +
                      "• SO2 - діоксид сірки\n" +
                      "• CO - монооксид вуглецю\n" +
                      "• O3 - озон",
                Location = new Point(10, 20),
                Size = new Size(210, 170),
                Font = new Font("Segoe UI", 9)
            };

            infoBox.Controls.Add(infoLabel);
            legendPanel.Controls.Add(infoBox);

            // Час оновлення
            Label updateTimeLabel = new Label
            {
                Name = "updateTimeLabel",
                Text = $"Оновлено: {DateTime.Now:HH:mm:ss}",
                Location = new Point(10, legendPanel.Height - 30),
                Size = new Size(230, 20),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            legendPanel.Controls.Add(updateTimeLabel);
        }

        private void MapPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Малюємо карту як фон
            if (mapImage != null)
            {
                // Розраховуємо масштаб для збереження пропорцій
                float scaleX = (float)mapPictureBox.Width / mapImage.Width;
                float scaleY = (float)mapPictureBox.Height / mapImage.Height;
                currentScale = Math.Min(scaleX, scaleY);

                int newWidth = (int)(mapImage.Width * currentScale);
                int newHeight = (int)(mapImage.Height * currentScale);
                mapOffsetX = (mapPictureBox.Width - newWidth) / 2;
                mapOffsetY = (mapPictureBox.Height - newHeight) / 2;

                // Малюємо карту по центру з збереженням пропорцій
                g.DrawImage(mapImage, mapOffsetX, mapOffsetY, newWidth, newHeight);

                // Зберігаємо масштаб для координат точок
                float pointScale = newWidth / 604f; // Базовий розмір карти 604px

                // Малюємо кольорові точки з врахуванням масштабу
                if (districts != null)
                {
                    foreach (var district in districts.Values)
                    {
                        Point scaledCenter = new Point(
                            mapOffsetX + (int)(district.Center.X * pointScale),
                            mapOffsetY + (int)(district.Center.Y * pointScale)
                        );
                        DrawDistrictIndicator(g, district, scaledCenter);
                    }
                }
            }
            else
            {
                // Якщо карти немає, малюємо простий фон
                g.Clear(Color.FromArgb(240, 240, 240));

                // Показуємо повідомлення
                using (Font font = new Font("Segoe UI", 14))
                {
                    string message = "Додайте файл kyiv_map.png або kyiv_map.jpg (604x599)";
                    SizeF size = g.MeasureString(message, font);
                    g.DrawString(message, font, Brushes.Gray,
                        (mapPictureBox.Width - size.Width) / 2,
                        (mapPictureBox.Height - size.Height) / 2);
                }

                // Малюємо точки без карти для тестування
                if (districts != null)
                {
                    // Масштабуємо точки відповідно до розміру PictureBox
                    float scale = Math.Min(mapPictureBox.Width / 604f, mapPictureBox.Height / 599f);
                    int offsetX = (mapPictureBox.Width - (int)(604 * scale)) / 2;
                    int offsetY = (mapPictureBox.Height - (int)(599 * scale)) / 2;

                    foreach (var district in districts.Values)
                    {
                        Point scaledCenter = new Point(
                            offsetX + (int)(district.Center.X * scale),
                            offsetY + (int)(district.Center.Y * scale)
                        );
                        DrawDistrictIndicator(g, district, scaledCenter);
                    }
                }
            }
        }

        private void DrawDistrictIndicator(Graphics g, DistrictInfo district, Point center)
        {
            // Зменшені розміри в 2.5 рази
            int radius = district == hoveredDistrict ? 14 : 12;
            Color aqiColor = GetAQIColor(district.AQI);

            // Малюємо зовнішнє коло з градієнтом
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(center.X - radius, center.Y - radius, radius * 2, radius * 2);

                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(255, aqiColor); // Повністю непрозорий центр
                    brush.SurroundColors = new Color[] { Color.FromArgb(180, aqiColor) }; // Менш прозорі краї
                    g.FillPath(brush, path);
                }

                // Малюємо обводку
                using (Pen pen = new Pen(Color.FromArgb(200, Color.DarkGray), 1))
                {
                    g.DrawEllipse(pen, center.X - radius, center.Y - radius, radius * 2, radius * 2);
                }
            }

            // Малюємо внутрішнє біле коло для тексту
            int innerRadius = radius - 2;
            using (Brush whiteBrush = new SolidBrush(Color.FromArgb(250, Color.White)))
            {
                g.FillEllipse(whiteBrush,
                    center.X - innerRadius,
                    center.Y - innerRadius,
                    innerRadius * 2,
                    innerRadius * 2);
            }

            // Малюємо тільки AQI значення
            using (Font aqiFont = new Font("Segoe UI", 7, FontStyle.Bold))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                Rectangle aqiRect = new Rectangle(
                    center.X - innerRadius,
                    center.Y - innerRadius,
                    innerRadius * 2,
                    innerRadius * 2
                );

                using (Brush aqiBrush = new SolidBrush(aqiColor))
                {
                    g.DrawString(district.AQI.ToString(), aqiFont, aqiBrush, aqiRect, sf);
                }
            }

            // Якщо район виділений, показуємо додаткову інформацію
            if (district == hoveredDistrict)
            {
                DrawDistrictTooltip(g, district, center);
            }
        }

        private void DrawDistrictTooltip(Graphics g, DistrictInfo district, Point center)
        {
            string info = $"{district.Name}\nAQI: {district.AQI}\n{GetAQIQuality(district.AQI)}";

            using (Font font = new Font("Segoe UI", 9))
            {
                SizeF size = g.MeasureString(info, font);

                int x = center.X + 40;
                int y = center.Y - 20;

                // Переміщуємо tooltip, якщо він виходить за межі
                if (x + size.Width > mapPictureBox.Width - 10)
                    x = center.X - (int)size.Width - 40;
                if (y + size.Height > mapPictureBox.Height - 10)
                    y = mapPictureBox.Height - (int)size.Height - 10;
                if (y < 10) y = 10;

                Rectangle rect = new Rectangle(x, y, (int)size.Width + 10, (int)size.Height + 10);

                // Фон tooltip
                using (Brush bgBrush = new SolidBrush(Color.FromArgb(230, Color.White)))
                {
                    g.FillRectangle(bgBrush, rect);
                    g.DrawRectangle(Pens.Gray, rect);
                }

                // Текст
                g.DrawString(info, font, Brushes.Black, x + 5, y + 5);
            }
        }

        private Color GetAQIColor(int aqi)
        {
            // Більш насичені кольори
            if (aqi <= 50) return Color.FromArgb(0, 200, 0);        // Насичений зелений
            if (aqi <= 100) return Color.FromArgb(255, 220, 0);     // Насичений жовтий
            if (aqi <= 150) return Color.FromArgb(255, 100, 0);     // Насичений помаранчевий
            if (aqi <= 200) return Color.FromArgb(255, 0, 0);       // Яскравий червоний
            if (aqi <= 300) return Color.FromArgb(180, 0, 180);     // Насичений фіолетовий
            return Color.FromArgb(140, 0, 33);                       // Темно-бордовий
        }

        private int GenerateAQI()
        {
            int baseAQI = random.Next(30, 120);
            int variation = random.Next(-20, 30);
            return Math.Max(0, Math.Min(500, baseAQI + variation));
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            UpdateAQIData();
            statusLabel.Text = $"Дані оновлено о {DateTime.Now:HH:mm:ss}";
        }

        private void UpdateAQIData()
        {
            foreach (var district in districts.Values)
            {
                int baseLevel = GetDistrictBaseLevel(district.Name);
                district.AQI = GenerateAQIForDistrict(baseLevel);
                district.LastUpdate = DateTime.Now;
            }

            mapPictureBox.Invalidate();

            var updateLabel = legendPanel.Controls["updateTimeLabel"] as Label;
            if (updateLabel != null)
            {
                updateLabel.Text = $"Оновлено: {DateTime.Now:HH:mm:ss}";
            }
        }

        private int GetDistrictBaseLevel(string districtName)
        {
            return districtName switch
            {
                "Голосіївський" => 45,
                "Дарницький" => 85,
                "Деснянський" => 70,
                "Дніпровський" => 75,
                "Оболонський" => 60,
                "Печерський" => 65,
                "Подільський" => 80,
                "Святошинський" => 90,
                "Солом'янський" => 95,
                "Шевченківський" => 70,
                _ => 70
            };
        }

        private int GenerateAQIForDistrict(int baseLevel)
        {
            int hourOfDay = DateTime.Now.Hour;
            int rushHourModifier = (hourOfDay >= 7 && hourOfDay <= 9) || (hourOfDay >= 17 && hourOfDay <= 19) ? 20 : 0;
            int variation = random.Next(-15, 25);
            return Math.Max(0, Math.Min(500, baseLevel + rushHourModifier + variation));
        }

        private void SetupTimer()
        {
            updateTimer.Interval = 60000; // Оновлення кожну хвилину
            updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateAQIData();
        }

        private void MapPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (districts == null) return;

            DistrictInfo newHoveredDistrict = null;

            // Розраховуємо масштаб для координат
            float pointScale = (mapImage != null) ?
                (mapImage.Width * currentScale) / 604f :
                Math.Min(mapPictureBox.Width / 604f, mapPictureBox.Height / 599f);

            foreach (var district in districts.Values)
            {
                // Перераховуємо координати з врахуванням масштабу
                Point scaledCenter = new Point(
                    mapOffsetX + (int)(district.Center.X * pointScale),
                    mapOffsetY + (int)(district.Center.Y * pointScale)
                );

                // Перевіряємо, чи курсор над точкою району (зменшений радіус)
                int distance = (int)Math.Sqrt(
                    Math.Pow(e.X - scaledCenter.X, 2) +
                    Math.Pow(e.Y - scaledCenter.Y, 2)
                );

                if (distance <= 14) // Зменшений радіус області кліку
                {
                    newHoveredDistrict = district;
                    string quality = GetAQIQuality(district.AQI);
                    statusLabel.Text = $"{district.Name}: AQI = {district.AQI} ({quality}) | Оновлено: {district.LastUpdate:HH:mm:ss}";
                    Cursor = Cursors.Hand;
                    break;
                }
            }

            // Оновлюємо виділення тільки якщо змінився район
            if (newHoveredDistrict != hoveredDistrict)
            {
                hoveredDistrict = newHoveredDistrict;
                mapPictureBox.Invalidate();
            }

            if (newHoveredDistrict == null)
            {
                statusLabel.Text = "Наведіть курсор на район для детальної інформації";
                Cursor = Cursors.Default;
            }
        }

        private void MapPictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (districts == null) return;

            // Режим налагодження - показуємо координати кліку (утримуйте Ctrl)
            if (Control.ModifierKeys == Keys.Control)
            {
                // Розраховуємо оригінальні координати на карті 604x599
                float pointScale = (mapImage != null) ?
                    (mapImage.Width * currentScale) / 604f :
                    Math.Min(mapPictureBox.Width / 604f, mapPictureBox.Height / 599f);

                int originalX = (int)((e.X - mapOffsetX) / pointScale);
                int originalY = (int)((e.Y - mapOffsetY) / pointScale);

                MessageBox.Show($"Координати на карті 604x599:\nX: {originalX}\nY: {originalY}\n\nКоординати кліку:\nX: {e.X}\nY: {e.Y}",
                    "Налагодження координат", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Звичайна обробка кліку
            float scale = (mapImage != null) ?
                (mapImage.Width * currentScale) / 604f :
                Math.Min(mapPictureBox.Width / 604f, mapPictureBox.Height / 599f);

            foreach (var district in districts.Values)
            {
                // Перераховуємо координати з врахуванням масштабу
                Point scaledCenter = new Point(
                    mapOffsetX + (int)(district.Center.X * scale),
                    mapOffsetY + (int)(district.Center.Y * scale)
                );

                // Перевіряємо, чи клік на точці району (зменшений радіус)
                int distance = (int)Math.Sqrt(
                    Math.Pow(e.X - scaledCenter.X, 2) +
                    Math.Pow(e.Y - scaledCenter.Y, 2)
                );

                if (distance <= 14) // Зменшений радіус
                {
                    ShowDistrictDetails(district);
                    break;
                }
            }
        }

        private void ShowDistrictDetails(DistrictInfo district)
        {
            MessageBox.Show(
                $"Район: {district.Name}\n" +
                $"AQI: {district.AQI}\n" +
                $"Якість повітря: {GetAQIQuality(district.AQI)}\n" +
                $"Базовий рівень: {GetDistrictBaseLevel(district.Name)}\n" +
                $"Останнє оновлення: {district.LastUpdate:HH:mm:ss}\n\n" +
                $"Рекомендації:\n{GetRecommendations(district.AQI)}",
                $"Детальна інформація - {district.Name}",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private string GetAQIQuality(int aqi)
        {
            if (aqi <= 50) return "Добре";
            if (aqi <= 100) return "Помірно";
            if (aqi <= 150) return "Нездорове для чутливих груп";
            if (aqi <= 200) return "Нездорове";
            if (aqi <= 300) return "Дуже нездорове";
            return "Небезпечне";
        }

        private string GetRecommendations(int aqi)
        {
            if (aqi <= 50) return "Ідеальні умови для прогулянок та занять спортом на відкритому повітрі.";
            if (aqi <= 100) return "Прийнятні умови для активностей на вулиці для більшості людей.";
            if (aqi <= 150) return "Людям з респіраторними захворюваннями слід обмежити тривалі навантаження на вулиці.";
            if (aqi <= 200) return "Уникайте тривалого перебування на вулиці. Закрийте вікна.";
            if (aqi <= 300) return "Залишайтеся вдома. Використовуйте очищувачі повітря.";
            return "Небезпечні умови! Не виходьте на вулицю без крайньої необхідності.";
        }

        private void DistrictComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (districtComboBox.SelectedIndex == 0)
            {
                mapPictureBox.Invalidate();
            }
            else
            {
                string selectedDistrict = districtComboBox.SelectedItem.ToString();
                if (districts != null && districts.ContainsKey(selectedDistrict))
                {
                    ShowDistrictDetails(districts[selectedDistrict]);
                }
            }
        }
    }

    public class DistrictInfo
    {
        public string Name { get; set; }
        public Point Center { get; set; }
        public int AQI { get; set; }
        public DateTime LastUpdate { get; set; }

        public DistrictInfo(string name, Point center, int aqi)
        {
            Name = name;
            Center = center;
            AQI = aqi;
            LastUpdate = DateTime.Now;
        }
    }
}