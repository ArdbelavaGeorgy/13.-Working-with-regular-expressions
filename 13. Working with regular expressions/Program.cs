using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class Location
{
    public string Name { get; set; }
    public float Coefficient { get; set; }
    public string Type { get; set; }
}

public class LocationEntryForm : Form
{
    private TextBox nameTextBox;
    private TextBox coeffTextBox;
    private ComboBox typeComboBox;
    private Button saveButton;
    public Location LocationData { get; private set; }

    public LocationEntryForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Label nameLabel = new Label { Text = "Имя местности:", Left = 20, Top = 20, Width = 100 };
        nameTextBox = new TextBox { Left = 130, Top = 20, Width = 150 };

        Label coeffLabel = new Label { Text = "Коэффициент:", Left = 20, Top = 50, Width = 100 };
        coeffTextBox = new TextBox { Left = 130, Top = 50, Width = 150 };

        Label typeLabel = new Label { Text = "Тип местности:", Left = 20, Top = 80, Width = 100 };
        typeComboBox = new ComboBox { Left = 130, Top = 80, Width = 150 };
        typeComboBox.Items.AddRange(new string[] { "Лес", "Поле", "Город", "Пустыня" });

        saveButton = new Button { Text = "Сохранить", Left = 20, Top = 110, Width = 100 };
        saveButton.Click += SaveButton_Click;

        Controls.AddRange(new Control[] { nameLabel, nameTextBox, coeffLabel, coeffTextBox, typeLabel, typeComboBox, saveButton });
        Text = "Добавить новое местоположение";
        Width = 320;
        Height = 200;
        AutoSize = true;
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(nameTextBox.Text))
        {
            MessageBox.Show("Пожалуйста, введите имя местности.");
            return;
        }

        if (typeComboBox.SelectedItem == null)
        {
            MessageBox.Show("Пожалуйста, выберите тип местности.");
            return;
        }

        if (!float.TryParse(coeffTextBox.Text, out float coeff) || coeff < 0.01f || coeff > 5.0f)
        {
            MessageBox.Show("Коэффициент должен быть числом от 0.01 до 5.0");
            return;
        }

        LocationData = new Location
        {
            Name = nameTextBox.Text,
            Coefficient = coeff,
            Type = typeComboBox.SelectedItem.ToString()
        };
        DialogResult = DialogResult.OK;
        Close();
    }
}

public class MainForm : Form
{
    private TabControl tabControl;
    private TabPage addLocationTab;
    private TabPage calculateRouteTab;
    private Button addLocationButton, calculateTimeButton, resetButton;
    private TextBox baseSpeedTextBox;
    private Label baseSpeedLabel;
    private List<Location> locations = new List<Location>();
    private ListBox historyListBox;
    private ListBox locationListBox;

    public MainForm()
    {
        InitializeComponents();
        WindowState = FormWindowState.Maximized;
    }

    private void InitializeComponents()
    {
        tabControl = new TabControl { Dock = DockStyle.Fill };
        addLocationTab = new TabPage("Добавить местоположения");
        calculateRouteTab = new TabPage("Рассчитать маршрут");

        addLocationButton = new Button { Text = "Добавить местоположение", Dock = DockStyle.Top };
        addLocationButton.Click += AddLocationButton_Click;

        locationListBox = new ListBox { Dock = DockStyle.Fill };

        addLocationTab.Controls.Add(locationListBox);
        addLocationTab.Controls.Add(addLocationButton);

        baseSpeedLabel = new Label { Text = "Базовая скорость (км/ч):", Dock = DockStyle.Top };
        baseSpeedTextBox = new TextBox { Dock = DockStyle.Top };

        calculateTimeButton = new Button { Text = "Рассчитать время", Dock = DockStyle.Top };
        calculateTimeButton.Click += CalculateTimeButton_Click;

        resetButton = new Button { Text = "Сброс", Dock = DockStyle.Top };
        resetButton.Click += ResetButton_Click;

        historyListBox = new ListBox { Dock = DockStyle.Bottom, Height = 100 };

        calculateRouteTab.Controls.AddRange(new Control[] { baseSpeedLabel, baseSpeedTextBox, calculateTimeButton, resetButton, historyListBox });

        tabControl.TabPages.AddRange(new TabPage[] { addLocationTab, calculateRouteTab });
        Controls.Add(tabControl);

        Text = "Ввод данных о местоположениях";
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterScreen;
    }

    private void AddLocationButton_Click(object sender, EventArgs e)
    {
        if (locations.Count >= 2)
        {
            MessageBox.Show("Можно добавить только два местоположения.");
            return;
        }
        var locationForm = new LocationEntryForm();
        if (locationForm.ShowDialog() == DialogResult.OK)
        {
            locations.Add(locationForm.LocationData);
            locationListBox.Items.Add($"Имя: {locationForm.LocationData.Name}, Коэффициент: {locationForm.LocationData.Coefficient}, Тип: {locationForm.LocationData.Type}");
            MessageBox.Show($"Местоположение '{locationForm.LocationData.Name}' добавлено успешно.");
        }
    }

    private void CalculateTimeButton_Click(object sender, EventArgs e)
    {
        if (locations.Count != 2)
        {
            MessageBox.Show("Добавьте ровно два местоположения.");
            return;
        }
        if (!float.TryParse(baseSpeedTextBox.Text, out float baseSpeed) || baseSpeed <= 0)
        {
            MessageBox.Show("Введите корректную базовую скорость (положительное число).");
            return;
        }
        float distance = 100;
        float time = distance / (baseSpeed * locations[0].Coefficient);
        int hours = (int)time;
        int minutes = (int)((time - hours) * 60);
        string result = $"От {locations[0].Name} до {locations[1].Name} за {hours} ч {minutes} мин при скорости {baseSpeed} км/ч. Коэффициенты: {locations[0].Coefficient:F2}, {locations[1].Coefficient:F2}";
        MessageBox.Show(result);
        historyListBox.Items.Insert(0, result);
    }

    private void ResetButton_Click(object sender, EventArgs e)
    {
        locations.Clear();
        locationListBox.Items.Clear();
        baseSpeedTextBox.Text = "";
    }

}

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
