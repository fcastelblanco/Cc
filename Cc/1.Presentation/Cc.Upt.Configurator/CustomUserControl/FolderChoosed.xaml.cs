using System.Windows;
using System.Windows.Forms;

namespace Cc.Upt.Configurator.CustomUserControl
{
    /// <summary>
    ///     Interaction logic for FolderChoosed.xaml
    /// </summary>
    public partial class FolderChoosed
    {
        public static DependencyProperty LabelValueProperty = DependencyProperty.Register(
            "LabelControlValue",
            typeof(string),
            typeof(FolderChoosed),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnLabelChanged));

        public static DependencyProperty PathValueProperty = DependencyProperty.Register(
            "PathControlValue",
            typeof(string),
            typeof(FolderChoosed),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnPathChanged));

        public static DependencyProperty ToolTipControlTitleProperty = DependencyProperty.Register(
            "ToolTipControlTitleValue",
            typeof(string),
            typeof(FolderChoosed),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnToolTipControlTitleChanged));

        public static DependencyProperty ToolTipControlDescriptionProperty = DependencyProperty.Register(
            "ToolTipControlDescriptionValue",
            typeof(string),
            typeof(FolderChoosed),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnToolTipControlDescriptionChanged));

        public static DependencyProperty CreateParameterAndFillItProperty = DependencyProperty.Register(
           "CreateParameterAndFillItValue",
           typeof(bool?),
           typeof(FolderChoosed),
           new FrameworkPropertyMetadata(
               false,
               FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
               OnCreateParameterAndFillItChanged));

        public static DependencyProperty FillParameterProperty = DependencyProperty.Register(
          "FillParameterValue",
          typeof(bool?),
          typeof(FolderChoosed),
          new FrameworkPropertyMetadata(
              false,
              FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
              OnFillParameterChanged));

        public static DependencyProperty RootFolderProperty = DependencyProperty.Register(
        "RootFolderValue",
        typeof(string),
        typeof(FolderChoosed),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public FolderChoosed()
        {
            InitializeComponent();
        }

        public string RootFolderValue
        {
            get { return (string)GetValue(RootFolderProperty); }
            set { SetValue(RootFolderProperty, value); }
        }

        public string LabelControlValue
        {
            get { return (string)GetValue(LabelValueProperty); }
            set { SetValue(LabelValueProperty, value); }
        }

        public string PathControlValue
        {
            get { return (string)GetValue(PathValueProperty); }
            set { SetValue(PathValueProperty, value); }
        }

        public string ToolTipControlTitleValue
        {
            get { return (string)GetValue(ToolTipControlTitleProperty); }
            set { SetValue(ToolTipControlTitleProperty, value); }
        }

        public string ToolTipControlDescriptionValue
        {
            get { return (string)GetValue(ToolTipControlDescriptionProperty); }
            set { SetValue(ToolTipControlDescriptionProperty, value); }
        }

        public bool? CreateParameterAndFillItValue
        {
            get { return (bool?)GetValue(CreateParameterAndFillItProperty); }
            set { SetValue(CreateParameterAndFillItProperty, value); }
        }

        public bool? FillParameterValue
        {
            get { return (bool?)GetValue(FillParameterProperty); }
            set { SetValue(FillParameterProperty, value); }
        }

        private static void OnLabelChanged(DependencyObject userControl, DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (FolderChoosed)userControl;
            control.LabelContent.Content = (string)eventArgs.NewValue;
        }

        private static void OnPathChanged(DependencyObject userControl, DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (FolderChoosed)userControl;
            control.PathTextBox.Text = eventArgs.NewValue?.ToString() ?? string.Empty;
        }

        private static void OnToolTipControlTitleChanged(DependencyObject userControl,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (FolderChoosed)userControl;
            control.ToolTipTittle.Text = (string)eventArgs.NewValue;
        }

        private static void OnToolTipControlDescriptionChanged(DependencyObject userControl,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (FolderChoosed)userControl;
            control.ToolTipDescription.Text = (string)eventArgs.NewValue;
        }

        private static void OnCreateParameterAndFillItChanged(DependencyObject userControl,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (FolderChoosed)userControl;
            control.CreateParameterAndFillIt.IsChecked = (bool?)eventArgs.NewValue ?? false;
        }

        private static void OnFillParameterChanged(DependencyObject userControl,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var control = (FolderChoosed)userControl;
            control.FillParameter.IsChecked = (bool?)eventArgs.NewValue ?? false;
        }

        private void PathToChoose_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = @"Seleccione la ruta",
                SelectedPath = RootFolderValue
            };

            var theFolderBorwserDialog = folderBrowserDialog.ShowDialog();

            if (theFolderBorwserDialog != DialogResult.OK) return;

            PathControlValue = folderBrowserDialog.SelectedPath;
        }
    }
}