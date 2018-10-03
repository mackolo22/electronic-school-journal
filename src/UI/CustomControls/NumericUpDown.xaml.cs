using System.Windows;
using System.Windows.Controls;

namespace UI.CustomControls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        private int _minValue;

        public NumericUpDown()
        {
            InitializeComponent();
        }

        public int MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                numericValueTextBox.Text = value.ToString();
            }
        }

        public int MaxValue { get; set; }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set
            {
                SetValue(ValueProperty, value);
                numericValueTextBox.Text = value.ToString();
            }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(NumericUpDown));

        private void ButtonUpValue_Click(object sender, RoutedEventArgs e)
        {
            if (Value + 1 <= MaxValue)
            {
                Value++;
            }
        }

        private void ButtonDownValue_Click(object sender, RoutedEventArgs e)
        {
            if (Value - 1 >= MinValue)
            {
                Value--;
            }
        }
    }
}
