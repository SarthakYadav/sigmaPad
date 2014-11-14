using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using ICSharpCode.AvalonEdit;
using ICSharpCode;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Xml;

namespace sigmaPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool selectedonly;
        private List<TabItem> _tabItems;
        private TabItem _tabAdd;
        //private string Lang = "C#";
       


        //private List<IHighlightingDefinition> Langs=new List<ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition>(){ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition{"C#"}};
        private TextEditor t1;
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                // initialize tabItem array
                _tabItems = new List<TabItem>();

                // add a tabItem with + in header 
                _tabAdd = new TabItem();
                _tabAdd.Header = "+";
                // tabAdd.MouseLeftButtonUp += new MouseButtonEventHandler(tabAdd_MouseLeftButtonUp);

                _tabItems.Add(_tabAdd);

                // add first tab
                this.AddTabItem();

                // bind tab control
                tabDynamic.DataContext = CorrectTabAmbiguity(_tabItems);

                tabDynamic.SelectedIndex = 0;

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private TabItem AddTabItem()
        {
            int count = _tabItems.Count;
            // create new tab item
            TabItem tab = new TabItem();
            tab.Header = string.Format("Tab {0}", count);
            tab.Name = string.Format("tab{0}", count);
            tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;
            tab.MouseDoubleClick += new MouseButtonEventHandler(tab_MouseDoubleClick);
            // add controls to tab item, this case here it is the avalonEdit TextEditor
            t1 = new TextEditor();
            t1.Name = "txt";
            //t1 = txt;

            tab.Content = t1;
            // insert tab item right before the last (+) tab item
            _tabItems.Insert(count - 1, tab);
            return tab;
        }
        private void tabAdd_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // clear tab control binding
            tabDynamic.DataContext = null;
            TabItem tab = this.AddTabItem();
            // bind tab control
            tabDynamic.DataContext = CorrectTabAmbiguity(_tabItems);
            // select newly added tab item
            tabDynamic.SelectedItem = tab;
        }
        private void tab_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TabItem tab = sender as TabItem;

            TabProperty dlg = new TabProperty();

            // get existing header text
            dlg.txtTitle.Text = tab.Header.ToString();

            if (dlg.ShowDialog() == true)
            {
                // change header text
                tab.Header = dlg.txtTitle.Text.Trim();
            }
        }

        private void tabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tab = tabDynamic.SelectedItem as TabItem;
            if (tab == null) return;

            if (tab.Equals(_tabAdd))
            {
                // clear tab control binding
                tabDynamic.DataContext = null;

                TabItem newTab = this.AddTabItem();

                // bind tab control
                tabDynamic.DataContext = CorrectTabAmbiguity(_tabItems);

                // select newly added tab item
                tabDynamic.SelectedItem = newTab;
            }
            else
            {
                // your code here...
            }
        }
        private List<TabItem> CorrectTabAmbiguity(List<TabItem> tabItemList)
        {
            for (int i = 0; i < tabItemList.Count - 1; i++)
            {
                if (!tabItemList[i].Name.ToString().ToCharArray().Last().Equals(i + 1))
                {
                    tabItemList[i].Name = "Tab" + (i + 1);
                }
            }
            return tabItemList;
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string tabName = (sender as Button).CommandParameter.ToString();

            var item = tabDynamic.Items.Cast<TabItem>().Where(i => i.Name.Equals(tabName)).SingleOrDefault();

            TabItem tab = item as TabItem;

            if (tab != null)
            {
                if (_tabItems.Count < 3)
                {
                    MessageBox.Show("Cannot remove last tab.");
                }
                else if (MessageBox.Show(string.Format("Are you sure you want to remove the tab '{0}'?", tab.Header.ToString()),
                    "Remove Tab", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // get selected tab
                    TabItem selectedTab = tabDynamic.SelectedItem as TabItem;

                    // clear tab control binding
                    tabDynamic.DataContext = null;

                    _tabItems.Remove(tab);

                    // bind tab control
                    tabDynamic.DataContext = CorrectTabAmbiguity(_tabItems);

                    // select previously selected tab. if that is removed then select first tab
                    if (selectedTab == null || selectedTab.Equals(tab))
                    {
                        selectedTab = _tabItems[0];
                    }
                    tabDynamic.SelectedItem = selectedTab;
                }
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|C# files (*.cs)|*.cs|C++ files (*.cpp)|*.cpp|Html files (*.html)|*.html|C files (*.c)|*.c|Java files (*.java)|*.java|C/C++ header files (*.h)|*.h|Xml files (*.xml)|*.xml|VB files (*.vb)|*.vb|Php files (*.php)|*.php|JavScript Files (*.js)|*.js";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, t1.Text);


        }

        /* private void langSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             var typeConverter = new HighlightingDefinitionTypeConverter();
             switch (langSelect.SelectedItem.ToString())
             {
                 case "C#":

                    
                     break;
                 case "C++":
                     var cppSyntaxHighlighter = (IHighlightingDefinition)typeConverter.ConvertFrom("C#");
                     t1.SyntaxHighlighting = cppSyntaxHighlighter;
                     break;
                 case "ASP.NET":
                     var aspSyntaxHighlighter = (IHighlightingDefinition)typeConverter.ConvertFrom("ASP.NET");
                     t1.SyntaxHighlighting = aspSyntaxHighlighter;
                     break;
                 case "Html":
                     var htmlSyntaxHighlighter = (IHighlightingDefinition)typeConverter.ConvertFrom("Html");
                     t1.SyntaxHighlighting = htmlSyntaxHighlighter;
                     break;
                 case "Java":
                     var javaSyntaxHighlighter = (IHighlightingDefinition)typeConverter.ConvertFrom("Java");
                     t1.SyntaxHighlighting = javaSyntaxHighlighter;
                     break;
                 case "JavaScript":
                     var jsSyntaxHighlighter = (IHighlightingDefinition)typeConverter.ConvertFrom("JavaScript");
                     t1.SyntaxHighlighting = jsSyntaxHighlighter;
                     break;
                 case "PHP":
                     var phpSyntaxHighlighter = (IHighlightingDefinition)typeConverter.ConvertFrom("PHP");
                     t1.SyntaxHighlighting = phpSyntaxHighlighter;
                     break;
                 case "VB":
                     var vbSyntaxHighlighter = (IHighlightingDefinition)typeConverter.ConvertFrom("VB");
                     t1.SyntaxHighlighting = vbSyntaxHighlighter;
                     break;
                 case "Xml":
                     var xmlSyntaxHighlighter = (IHighlightingDefinition)typeConverter.ConvertFrom("XML");
                     t1.SyntaxHighlighting = xmlSyntaxHighlighter;
                     break;
                 default:
                     break;
             }
         }  */

        private void menuItmExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to exit/?", "Exit Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    this.Close();
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        private void _undo_Click(object sender, RoutedEventArgs e)
        {
            t1.Undo();
        }

        private void _cut_Click(object sender, RoutedEventArgs e)
        {
            t1.Cut();
        }

        private void _copy_Click(object sender, RoutedEventArgs e)
        {
            t1.Copy();
        }

        private void _paste_Click(object sender, RoutedEventArgs e)
        {
            t1.Paste();
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|C# files (*.cs)|*.cs|C++ files (*.cpp)|*.cpp|Html files (*.html)|*.html|C files (*.c)|*.c|Java files (*.java)|*.java|C/C++ header files (*.h)|*.h|Xml files (*.xml)|*.xml|VB files (*.vb)|*.vb|Php files (*.php)|*.php|JavScript Files (*.js)|*.js";
            if (openFileDialog.ShowDialog() == true)
                t1.Text = File.ReadAllText(openFileDialog.FileName);
        }

        private void _save_Click(object sender, RoutedEventArgs e)
        {
            t1.Save(t1.Text);
        }

        private void menuNew_Click(object sender, RoutedEventArgs e)
        {
            this.AddTabItem();


        }

        private void _redo_Click(object sender, RoutedEventArgs e)
        {
            t1.Redo();

        }

        private void toggleEditorLineNo_Checked(object sender, RoutedEventArgs e)
        {
            // t1.Margins[0].Width = 20;
        }

        private void langSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //t1.ConfigurationManager.Language = "cs";
        }
        private int lastUsedIndex = 0;
        private void _find_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = finder.Text.ToString();
            if (string.IsNullOrEmpty(searchQuery))
            {
                lastUsedIndex = 0;
                return;
            }

            string editorText = this.t1.Text;

            if (string.IsNullOrEmpty(editorText))
            {
                lastUsedIndex = 0;
                return;
            }

            if (lastUsedIndex >= searchQuery.Count())
            {
                lastUsedIndex = 0;
            }

            int nIndex = editorText.IndexOf(searchQuery, lastUsedIndex);
            if (nIndex != -1)
            {
                var area = this.t1.TextArea;
                this.t1.Select(nIndex, searchQuery.Length);
                lastUsedIndex = nIndex + searchQuery.Length;
            }
            else
            {
                lastUsedIndex = 0;
            }
        }
        
        private void _replace_Click(object sender, RoutedEventArgs e)
        {
            int lastSearchIndex;
            string s = finder.Text.ToString();
            string replacement = replacer.Text.ToString();
            int nIndex = -1;
            if (selectedonly)
            {
                nIndex = t1.Text.IndexOf(s, this.t1.SelectionStart, this.t1.SelectionLength);
            }
            else
            {
                nIndex = t1.Text.IndexOf(s);
            }

            if (nIndex != -1)
            {
                this.t1.Document.Replace(nIndex, s.Length, replacement);


                this.t1.Select(nIndex, replacement.Length);
            }
            else
            {
                lastSearchIndex = 0;
                
            }
        }

        private void selectedTextOnly_Checked(object sender, RoutedEventArgs e)
        {
            selectedonly = true;
        }

    }
}