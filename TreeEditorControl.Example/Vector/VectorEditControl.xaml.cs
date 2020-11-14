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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TreeEditorControl.Example
{
    /// <summary>
    /// Interaction logic for VectorEditControl.xaml
    /// </summary>
    public partial class VectorEditControl : UserControl
    {
        public VectorEditControl()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
          nameof(Text), typeof(string), typeof(VectorEditControl), new PropertyMetadata(string.Empty));

        public Vector Vector
        {
            get { return (Vector)GetValue(VectorProperty); }
            set { SetValue(VectorProperty, value); }
        }

        public static readonly DependencyProperty VectorProperty = DependencyProperty.Register(
          nameof(Vector), typeof(Vector), typeof(VectorEditControl), new PropertyMetadata(new Vector()));

        public IEnumerable<Vector> VectorSelection
        {
            get { return (IEnumerable<Vector>)GetValue(VectorSelectionProperty); }
            set { SetValue(VectorSelectionProperty, value); }
        }

        public static readonly DependencyProperty VectorSelectionProperty = DependencyProperty.Register(
          nameof(VectorSelection), typeof(IEnumerable<Vector>), typeof(VectorEditControl), new PropertyMetadata(new List<Vector>()));

        private void SelectButton_Clicked(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.MainWindow;
            var dialog = new SelectVectorDialog(VectorSelection);

            if (dialog.ShowDialog(window) == true && dialog.SelectedVector != null)
            {
                Vector.X = dialog.SelectedVector.X;
                Vector.Y = dialog.SelectedVector.Y;
                Vector.Z = dialog.SelectedVector.Z;
            }
        }
    }
}
