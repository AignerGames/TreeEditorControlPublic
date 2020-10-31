using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace TreeEditorControl.Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            InitializeComponent();

            DataContext = new ViewModel();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // This workaround is used to remove the (keyboard) focus of text boxes when the "background" is clicked.
            // TODO: Is there a better way to do this? 

            base.OnMouseLeftButtonDown(e);

            if (Content is IInputElement inputElement)
            {
                inputElement.Focusable = true;
                inputElement.Focus();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            (DataContext as ViewModel)?.HandleClosing(e);

            base.OnClosing(e);
        }

    }
}
