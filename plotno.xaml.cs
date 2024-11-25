using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Paint
{
    public partial class CanvasSettingsWindow : Window
    {
        public int CanvasWidth { get; private set; }
        public int CanvasHeight { get; private set; }
        public bool IsTransparent { get; private set; }

        public CanvasSettingsWindow()
        {
            InitializeComponent();
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtWidth.Text, out int width) &&
                int.TryParse(txtHeight.Text, out int height))
            {
                if (width >= 0 && height >= 0)
                {
                    CanvasWidth = width;
                    CanvasHeight = height;
                }
                else
                {
                    MessageBox.Show("Podano nieprawidłowe wartości dla szerokości i/lub wysokości. Wprowadź wartość większą lub równą 0.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Nie wpisano wartości.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IsTransparent = chkTransparent.IsChecked ?? false;
            DialogResult = true;
        }

    }
}