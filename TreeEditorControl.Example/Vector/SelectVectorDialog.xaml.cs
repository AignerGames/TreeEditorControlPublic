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

namespace TreeEditorControl.Example
{
    /// <summary>
    /// Interaction logic for SelectVectorDialog.xaml
    /// </summary>
    public partial class SelectVectorDialog : Window
    {
        public SelectVectorDialog(IEnumerable<Vector> vectors)
        {
            InitializeComponent();

            foreach(var vector in vectors)
            {
                _ComboBox.Items.Add(vector);
            }

            if(_ComboBox.Items.Count > 0)
            {
                _ComboBox.SelectedIndex = 0;
            }
        }

        public Vector SelectedVector => _ComboBox.SelectedItem as Vector;

        public bool? ShowDialog(Window window)
        {
            // TODO: Make this as an extension method 

            Owner = window;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            return ShowDialog();
        }

        private void SelectButton_Clicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
