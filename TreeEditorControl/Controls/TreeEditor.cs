using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TreeEditorControl.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TreeEditorControl.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TreeEditorControl.Controls;assembly=TreeEditorControl"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class TreeEditor : Control
    {
        static TreeEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeEditor), new FrameworkPropertyMetadata(typeof(TreeEditor)));
        }

        public TreeEditor()
        {
            BorderBrush = Brushes.DarkGray;
            BorderThickness = new Thickness(1);

            // Not sure if there is a better way to handle this without this delegate workaround...
            CatalogGroupStyleSelectorDelegate = (group, level) => new GroupStyle { ContainerStyleSelector = CatalogGroupStyleSelector };
        }

        public DataTemplateSelector ToolbarCommandTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ToolbarCommandTemplateSelectorProperty); }
            set { SetValue(ToolbarCommandTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ToolbarCommandTemplateSelectorProperty = DependencyProperty.Register(
          nameof(ToolbarCommandTemplateSelector), typeof(DataTemplateSelector), typeof(TreeEditor), new PropertyMetadata(null));

        public DataTemplateSelector CatalogItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CatalogItemTemplateSelectorProperty); }
            set { SetValue(CatalogItemTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty CatalogItemTemplateSelectorProperty = DependencyProperty.Register(
          nameof(CatalogItemTemplateSelector), typeof(DataTemplateSelector), typeof(TreeEditor), new PropertyMetadata(null));

        public GroupStyleSelector CatalogGroupStyleSelectorDelegate
        {
            get { return (GroupStyleSelector)GetValue(CatalogGroupStyleSelectorDelegateProperty); }
            set { SetValue(CatalogGroupStyleSelectorDelegateProperty, value); }
        }

        public static readonly DependencyProperty CatalogGroupStyleSelectorDelegateProperty = DependencyProperty.Register(
          nameof(CatalogGroupStyleSelectorDelegate), typeof(GroupStyleSelector), typeof(TreeEditor), new PropertyMetadata(null));

        public StyleSelector CatalogGroupStyleSelector
        {
            get { return (StyleSelector)GetValue(CatalogGroupStyleSelectorProperty); }
            set { SetValue(CatalogGroupStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty CatalogGroupStyleSelectorProperty = DependencyProperty.Register(
          nameof(CatalogGroupStyleSelector), typeof(StyleSelector), typeof(TreeEditor), new PropertyMetadata(null));

        public DataTemplateSelector TreeViewItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(TreeViewItemTemplateSelectorProperty); }
            set { SetValue(TreeViewItemTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty TreeViewItemTemplateSelectorProperty = DependencyProperty.Register(
          nameof(TreeViewItemTemplateSelector), typeof(DataTemplateSelector), typeof(TreeEditor), new PropertyMetadata(null));

        public StyleSelector TreeViewItemContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(TreeViewItemContainerStyleSelectorProperty); }
            set { SetValue(TreeViewItemContainerStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty TreeViewItemContainerStyleSelectorProperty = DependencyProperty.Register(
          nameof(TreeViewItemContainerStyleSelector), typeof(StyleSelector), typeof(TreeEditor), new PropertyMetadata(null));

        public DataTemplateSelector SelectedNodeInfoTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SelectedNodeInfoTemplateSelectorProperty); }
            set { SetValue(SelectedNodeInfoTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty SelectedNodeInfoTemplateSelectorProperty = DependencyProperty.Register(
          nameof(SelectedNodeInfoTemplateSelector), typeof(DataTemplateSelector), typeof(TreeEditor), new PropertyMetadata(null));

        public Brush SplitterBrush
        {
            get { return (Brush)GetValue(SplitterBrushProperty); }
            set { SetValue(SplitterBrushProperty, value); }
        }

        public static readonly DependencyProperty SplitterBrushProperty = DependencyProperty.Register(
          nameof(SplitterBrush), typeof(Brush), typeof(TreeEditor), new PropertyMetadata(Brushes.LightGray));

        public double SplitterSize
        {
            get { return (double)GetValue(SplitterSizeProperty); }
            set { SetValue(SplitterSizeProperty, value); }
        }

        public static readonly DependencyProperty SplitterSizeProperty = DependencyProperty.Register(
          nameof(SplitterSize), typeof(double), typeof(TreeEditor), new PropertyMetadata(3.0));
    }
}
