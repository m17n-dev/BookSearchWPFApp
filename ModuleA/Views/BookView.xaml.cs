using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace ModuleA.Views {
    /// <summary>
    /// Interaction logic for BookView.xaml
    /// </summary>
    public partial class BookView : UserControl {
        public BookView() {
            InitializeComponent();
        }

        private void ComboBoxAuthor_TextChanged(object sender, TextChangedEventArgs e) {
            ComboBox comboBox = (ComboBox)sender;
            Debug.WriteLine("ComboBoxAuthor_TextChanged() called.", comboBox.Text);
            if (comboBox.SelectedItem != null) {
                Debug.WriteLine("SelectedItem is not Null");
                var exist = ModuleA.Models.AppContext.Instance.AuthorsMaster.Authors
                    .Any(author => author.NameAndId == comboBox.Text);
                if (!exist) {
                    comboBox.Text = null;
                    comboBox.SelectedItem = null;
                    Debug.WriteLine("SelectedItem is Null");
                }
            }

            if (!string.IsNullOrEmpty(comboBox.Text)) {
                Debug.WriteLine("!string.IsNullOrEmpty() called.", comboBox.Text);
                comboBox.IsDropDownOpen = true;
                comboBox.ItemsSource = ModuleA.Models.AppContext.Instance.AuthorsMaster.Authors
                    .Where(author => author.NameAndId != null
                    && author.NameAndId.StartsWith(comboBox.Text, true, CultureInfo.CurrentCulture));
            }
            else {
                comboBox.IsDropDownOpen = false;
                comboBox.ItemsSource = null;
            }
        }

        private void ComboBoxPublisher_TextChanged(object sender, TextChangedEventArgs e) {
            ComboBox comboBox = (ComboBox)sender;
            Debug.WriteLine("ComboBoxPublisher_TextChanged() called.", comboBox.Text);
            if (comboBox.SelectedItem != null) {
                Debug.WriteLine("SelectedItem is not Null");
                var exist = ModuleA.Models.AppContext.Instance.PublishersMaster.Publishers
                    .Any(Publisher => Publisher.NameAndId == comboBox.Text);
                if (!exist) {
                    comboBox.Text = null;
                    comboBox.SelectedItem = null;
                    Debug.WriteLine("SelectedItem is Null");
                }
            }

            if (!string.IsNullOrEmpty(comboBox.Text)) {
                Debug.WriteLine("!string.IsNullOrEmpty() called.", comboBox.Text);
                comboBox.IsDropDownOpen = true;
                comboBox.ItemsSource = ModuleA.Models.AppContext.Instance.PublishersMaster.Publishers
                    .Where(Publisher => Publisher.NameAndId != null
                    && Publisher.NameAndId.StartsWith(comboBox.Text, true, CultureInfo.CurrentCulture));
            }
            else {
                comboBox.IsDropDownOpen = false;
                comboBox.ItemsSource = null;
            }
        }
    }
}
