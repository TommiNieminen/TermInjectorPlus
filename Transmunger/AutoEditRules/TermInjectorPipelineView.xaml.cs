
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
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
using System.Windows.Threading;
using YamlDotNet.Serialization;

namespace TermInjector2022
{
    /// <summary>
    /// Interaction logic for TranslateWindow.xaml
    /// </summary>
    public partial class TermInjectorPipelineView: UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private TermInjectorPipeline textProcessor;

        private void InitializePipelineConfigurations()
        {
            this.PipelineConfigurations = new ObservableCollection<TermInjectorPipeline>();
            var pipelineConfigDir = new DirectoryInfo(
                HelperFunctions.GetLocalAppDataPath(TermInjector2022Settings.Default.ConfigDir));

            if (!pipelineConfigDir.Exists)
            {
                pipelineConfigDir.Create();
            }
            var deserializer = new Deserializer();
            foreach (var file in pipelineConfigDir.EnumerateFiles())
            {
                using (var reader = file.OpenText())
                {
                    try
                    {
                        var loadedConfig = deserializer.Deserialize<TermInjectorPipeline>(reader);
                        this.PipelineConfigurations.Add(loadedConfig);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Pipeline config file deserialization failed, file {file.Name}. Exception: {ex.Message}");
                    }
                }
            }
        }

        private void InitializeAutoEditRuleCollections()
        {
            this.AutoPostEditRuleCollections = new ObservableCollection<AutoEditRuleCollection>();
            this.AutoPreEditRuleCollections = new ObservableCollection<AutoEditRuleCollection>();

            var editRuleDir = new DirectoryInfo(
                HelperFunctions.GetLocalAppDataPath(TermInjector2022Settings.Default.EditRuleDir));

            if (!editRuleDir.Exists)
            {
                editRuleDir.Create();
            }
            var deserializer = new Deserializer();
            foreach (var file in editRuleDir.EnumerateFiles())
            {
                using (var reader = file.OpenText())
                {
                    try
                    {
                        var loadedRuleCollection = deserializer.Deserialize<AutoEditRuleCollection>(reader);
                        switch (loadedRuleCollection.CollectionType)
                        {
                            case "preedit":
                                this.AutoPreEditRuleCollections.Add(loadedRuleCollection);
                                break;
                            case "postedit":
                                this.AutoPostEditRuleCollections.Add(loadedRuleCollection);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Auto edit rule deserialization failed, file {file.Name}. Exception: {ex.Message}");
                    }
                }
            }

        }

        public TermInjectorPipelineView(TermInjector2022TPOptions translationOptions)
        {
            this.InitializeAutoEditRuleCollections();
            this.InitializePipelineConfigurations();
            Guid configGuid;
            //Check if the options contain a guid for an existing terminjector configuration
            if (Guid.TryParse(translationOptions.termInjectorConfigGuid, out configGuid))
            {
                //TODO: Load all configs and check whether the config with this guid exists.
                var configInOptions =
                    this.PipelineConfigurations.SingleOrDefault(x => x.PipelineGuid == translationOptions.termInjectorConfigGuid);
                if (configInOptions != null)
                {
                    this.TermInjectorConfig = configInOptions;
                }
            }
            
            
            this.Title = String.Format(TermInjector2022.Properties.Resources.EditRules_EditRulesTitle,TermInjectorConfig.PipelineName);
            
            InitializeComponent();
            
            InitializeTester();
            this.AutoPreEditRuleCollectionList.ItemsSource = this.TermInjectorConfig.AutoPreEditRuleCollections;
            this.AutoPostEditRuleCollectionList.ItemsSource = this.TermInjectorConfig.AutoPostEditRuleCollections;
        }

        //Add testing controls for each collection
        private void InitializeTester()
        {

            this.RuleTester.Children.Clear();
            this.PreEditTesters = new List<TestPreEditRuleControl>();
            this.PostEditTesters = new List<TestPostEditRuleControl>();

            var inputBoxLabel = "Input to rule collection:";
            var inputOrigin = "Source text";
            
            foreach (var preEditRuleCollection in this.TermInjectorConfig.AutoPreEditRuleCollections)
            {
                
                var title = $"Pre-edit rule collection";
                var testControl =
                    new TestPreEditRuleControl()
                    {
                        RuleCollection = preEditRuleCollection,
                        Title = title,
                        InputBoxLabel = inputBoxLabel,
                        InputOrigin = inputOrigin,
                        TestButtonVisibility = Visibility.Collapsed,
                        ButtonText = "Test all pre- and postediting rules"
                    };

                inputOrigin = $"Output from {preEditRuleCollection.CollectionName}";
                this.PreEditTesters.Add(testControl);
                this.RuleTester.Children.Add(testControl);
            }


            inputBoxLabel = "Input to rule collection:";
            inputOrigin = "MT output";
            foreach (var postEditRuleCollection in this.TermInjectorConfig.AutoPostEditRuleCollections)
            {
                var title = $"Post-edit rule collection";
                var testControl =
                    new TestPostEditRuleControl()
                    {
                        RuleCollection = postEditRuleCollection,
                        Title = title,
                        InputBoxLabel = inputBoxLabel,
                        InputOrigin = inputOrigin,
                        TestButtonVisibility = Visibility.Collapsed,
                        SourceBoxVisibility = Visibility.Collapsed
                    };
                inputOrigin = $"Output from {postEditRuleCollection.CollectionName}";

                this.PostEditTesters.Add(testControl);
                this.RuleTester.Children.Add(testControl);
            }
        }

        public TermInjectorPipeline TermInjectorConfig { get => textProcessor; set => textProcessor = value; }
        public string Title { get; private set; }
        public ObservableCollection<AutoEditRuleCollection> AutoPreEditRuleCollections { get; private set; }
        public ObservableCollection<AutoEditRuleCollection> AutoPostEditRuleCollections { get; private set; }
        public List<TestPreEditRuleControl> PreEditTesters { get; private set; }
        public List<TestPostEditRuleControl> PostEditTesters { get; private set; }
        public ObservableCollection<TermInjectorPipeline> PipelineConfigurations { get; private set; }

        private void CreatePreRule_Click(object sender, RoutedEventArgs e)
        {
            var createRuleWindow = new CreatePreEditRuleWindow();
            ((Window)createRuleWindow).Owner = Application.Current.MainWindow;
            var dialogResult = createRuleWindow.ShowDialog();


            if (dialogResult != null && dialogResult.Value)
            {
                var newRuleCollection = new AutoEditRuleCollection()
                {
                    CollectionName = createRuleWindow.CreatedRule.Description,
                    CollectionGuid = Guid.NewGuid().ToString(),
                    CollectionType = "preedit"
                };
                newRuleCollection.AddRule(createRuleWindow.CreatedRule);
                newRuleCollection.Save();
                this.TermInjectorConfig.AutoPreEditRuleCollectionGuids.Add(newRuleCollection.CollectionGuid);
                this.TermInjectorConfig.SaveConfig();
                this.AutoPreEditRuleCollections.Add(newRuleCollection);
                this.TermInjectorConfig.AutoPreEditRuleCollections.Add(newRuleCollection);
                InitializeTester();
            }
        }
        

        
        private void AddPreRuleCollection_Click(object sender, RoutedEventArgs e)
        {
            var addCollectionWindow = 
                new AddEditRuleCollectionWindow(
                    this.AutoPreEditRuleCollections, 
                    this.TermInjectorConfig.AutoPreEditRuleCollections);
            addCollectionWindow.Owner = Application.Current.MainWindow;
            var dialogResult = addCollectionWindow.ShowDialog();
            if (dialogResult.Value)
            {
                this.AddRuleCollection(
                    addCollectionWindow.RuleCollectionCheckBoxList,
                    this.AutoPreEditRuleCollections,
                    this.TermInjectorConfig.AutoPreEditRuleCollections,
                    this.TermInjectorConfig.AutoPreEditRuleCollectionGuids);
            }
        }

        private void AddRuleCollection(
            ObservableCollection<CheckBoxListItem<AutoEditRuleCollection>> ruleCollectionCheckBoxList,
            ObservableCollection<AutoEditRuleCollection> allCollections, 
            ObservableCollection<AutoEditRuleCollection> modelCollections, 
            ObservableCollection<string> modelGuids)
        {
            foreach (var collection in ruleCollectionCheckBoxList)
            {
                if (!allCollections.Contains(collection.Item))
                {
                    allCollections.Add(collection.Item);
                }

                if (collection.Checked)
                {
                    //Don't read the collection if it was already selected for the model
                    if (!modelCollections.Any(x => x.CollectionGuid == collection.Item.CollectionGuid))
                    {
                        modelCollections.Add(collection.Item);
                        modelGuids.Add(collection.Item.CollectionGuid);
                    }
                }
                else
                {
                    //If a collection was selected for the model and is now unchecked, remove it
                    var collectionPresent = modelCollections.SingleOrDefault(x => x.CollectionGuid == collection.Item.CollectionGuid);
                    if (collectionPresent != null)
                    {
                        modelCollections.Remove(collectionPresent);
                        modelGuids.Remove(collectionPresent.CollectionGuid);
                    }
                }
            }
            this.TermInjectorConfig.SaveConfig();
            InitializeTester(); ;
        }

        private void EditPreRuleCollection_Click(object sender, RoutedEventArgs e)
        {
            var selectedCollection = (AutoEditRuleCollection)this.AutoPreEditRuleCollectionList.SelectedItem;
            
            //Edit a clone of the collection, so that the changes can be canceled in the edit window
            var editCollectionWindow = new EditPreEditRuleCollectionWindow(selectedCollection.Clone());
            editCollectionWindow.Owner = Application.Current.MainWindow;
            var dialogResult = editCollectionWindow.ShowDialog();
            if (dialogResult.Value)
            {
                selectedCollection.CopyValuesFromOtherCollection(editCollectionWindow.RuleCollection);
                selectedCollection.Save();
                this.AutoPreEditRuleCollectionList.Items.Refresh();
                InitializeTester();
            }
        }

        private void RemoveRuleCollection(
            List<AutoEditRuleCollection> selectedCollections,
            ObservableCollection<string> guidList,
            ObservableCollection<AutoEditRuleCollection> collectionList)
        {
            foreach (var selectedCollection in selectedCollections)
            {
                guidList.Remove(selectedCollection.CollectionGuid);
                collectionList.Remove(selectedCollection);
            }
            this.TermInjectorConfig.SaveConfig();
            InitializeTester();
        }

        private void RemovePreRuleCollection_Click(object sender, RoutedEventArgs e)
        {
            var selectedCollections = 
                this.AutoPreEditRuleCollectionList.SelectedItems.Cast<AutoEditRuleCollection>().ToList();
            this.RemoveRuleCollection(
                selectedCollections,
                this.TermInjectorConfig.AutoPreEditRuleCollectionGuids,
                this.TermInjectorConfig.AutoPreEditRuleCollections);
        }

        private void DeletePreRuleCollection_Click(object sender, RoutedEventArgs e)
        {
            var selectedCollection = (AutoEditRuleCollection)this.AutoPreEditRuleCollectionList.SelectedItem;
            var messageBoxResult = selectedCollection.Delete();
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                this.TermInjectorConfig.AutoPreEditRuleCollections.Remove(selectedCollection);
                this.AutoPreEditRuleCollections.Remove(selectedCollection);
                this.TermInjectorConfig.AutoPreEditRuleCollectionGuids.Remove(selectedCollection.CollectionGuid);
                this.TermInjectorConfig.SaveConfig();
                InitializeTester();
            }
        }

        private void CreatePostRule_Click(object sender, RoutedEventArgs e)
        {
            var createRuleWindow = new CreatePostEditRuleWindow();
            createRuleWindow.Owner = Application.Current.MainWindow;
            var dialogResult = createRuleWindow.ShowDialog();

            if (dialogResult != null && dialogResult.Value)
            {
                var newRuleCollection = new AutoEditRuleCollection()
                {
                    CollectionName = createRuleWindow.CreatedRule.Description,
                    CollectionGuid = Guid.NewGuid().ToString(),
                    CollectionType = "postedit",
                    GlobalCollection = false
                };

                newRuleCollection.AddRule(createRuleWindow.CreatedRule);
                newRuleCollection.Save();
                this.TermInjectorConfig.AutoPostEditRuleCollectionGuids.Add(newRuleCollection.CollectionGuid);
                this.TermInjectorConfig.SaveConfig();
                this.AutoPostEditRuleCollections.Add(newRuleCollection);
                this.TermInjectorConfig.AutoPostEditRuleCollections.Add(newRuleCollection);
                InitializeTester();
            }
        }

        private void AddPostRuleCollection_Click(object sender, RoutedEventArgs e)
        {
            var addCollectionWindow =
                new AddEditRuleCollectionWindow(
                    AutoPostEditRuleCollections,
                    this.TermInjectorConfig.AutoPostEditRuleCollections);
            addCollectionWindow.Owner = Application.Current.MainWindow;
            var dialogResult = addCollectionWindow.ShowDialog();
            if (dialogResult.Value)
            {
                this.AddRuleCollection(
                    addCollectionWindow.RuleCollectionCheckBoxList,
                    this.AutoPostEditRuleCollections,
                    this.TermInjectorConfig.AutoPostEditRuleCollections,
                    this.TermInjectorConfig.AutoPostEditRuleCollectionGuids);
            }
        }

        private void EditPostRuleCollection_Click(object sender, RoutedEventArgs e)
        {
            var selectedCollection = (AutoEditRuleCollection)this.AutoPostEditRuleCollectionList.SelectedItem;
            
            //Edit a clone of the collection, so that the changes can be canceled in the edit window
            var editCollectionWindow = new EditPostEditRuleCollectionWindow(selectedCollection.Clone());
            editCollectionWindow.Owner = Application.Current.MainWindow;
            var dialogResult = editCollectionWindow.ShowDialog();
            if (dialogResult.Value)
            {
                selectedCollection.CopyValuesFromOtherCollection(editCollectionWindow.RuleCollection);
                selectedCollection.Save();
                this.AutoPostEditRuleCollectionList.Items.Refresh();
                InitializeTester();
            }

        }

        private void RemovePostRuleCollection_Click(object sender, RoutedEventArgs e)
        {
            var selectedCollections =
                this.AutoPostEditRuleCollectionList.SelectedItems.Cast<AutoEditRuleCollection>().ToList();
            this.RemoveRuleCollection(
                    selectedCollections,
                    this.TermInjectorConfig.AutoPostEditRuleCollectionGuids,
                    this.TermInjectorConfig.AutoPostEditRuleCollections);
        }

        private void DeletePostRuleCollection_Click(object sender, RoutedEventArgs e)
        {
            var selectedCollection = (AutoEditRuleCollection)this.AutoPostEditRuleCollectionList.SelectedItem;
            selectedCollection.Delete();
            this.TermInjectorConfig.AutoPostEditRuleCollections.Remove(selectedCollection);
            this.AutoPostEditRuleCollections.Remove(selectedCollection);
            this.TermInjectorConfig.AutoPostEditRuleCollectionGuids.Remove(selectedCollection.CollectionGuid);
            this.TermInjectorConfig.SaveConfig();
            InitializeTester();
        }

        private void TestRules_Click(object sender, RoutedEventArgs e)
        {
            string previousTesterOutput = null;
            string rawSource = null;
            foreach (var tester in this.PreEditTesters)
            {
                if (previousTesterOutput != null)
                {
                    tester.SourceText = previousTesterOutput;
                }
                else
                {
                    rawSource = tester.SourceText;
                }
                tester.ProcessRules();
                previousTesterOutput = tester.OutputText;
            }

            //previousTesterOutput will now contain the pre-edited source for machine translation,
            //change it to MT output.
            //Do not apply edit rules here, since they will be visually applied in the tester
            previousTesterOutput = this.TermInjectorConfig.ProcessInput(
                previousTesterOutput,
                applyEditRules:false);

            foreach (var tester in this.PostEditTesters)
            {
                tester.SourceText = rawSource;
                tester.OutputText = previousTesterOutput;
                
                tester.ProcessRules();
                previousTesterOutput = tester.EditedOutputText;
            }

        }
        
        
        private void MoveCollectionDown(
            ListView collectionList, 
            ObservableCollection<AutoEditRuleCollection> ruleCollection,
            ObservableCollection<string> collectionGuids
            )
        {
            var selectedItem = (AutoEditRuleCollection)collectionList.SelectedItem;
            
            var selectedItemIndex = ruleCollection.IndexOf(selectedItem);
            var guidIndex = collectionGuids.IndexOf(selectedItem.CollectionGuid);
            if (selectedItemIndex < ruleCollection.Count - 1)
            {
                ruleCollection.Move(selectedItemIndex, selectedItemIndex + 1);
                collectionGuids.Move(guidIndex, guidIndex + 1);
            }
            this.InitializeTester();
            this.TermInjectorConfig.SaveConfig();
        }

        private void MovePreRuleCollectionDown_Click(object sender, RoutedEventArgs e)
        {
            this.MoveCollectionDown(
                this.AutoPreEditRuleCollectionList, 
                this.TermInjectorConfig.AutoPreEditRuleCollections,
                this.TermInjectorConfig.AutoPreEditRuleCollectionGuids);
        }

        private void MovePostRuleCollectionDown_Click(object sender, RoutedEventArgs e)
        {
            this.MoveCollectionDown(
                this.AutoPostEditRuleCollectionList, 
                this.TermInjectorConfig.AutoPostEditRuleCollections,
                this.TermInjectorConfig.AutoPostEditRuleCollectionGuids);
        }

        private void MoveCollectionUp(
            ListView collectionList, 
            ObservableCollection<AutoEditRuleCollection> ruleCollection,
            ObservableCollection<string> collectionGuids)
        {
            var selectedItem = (AutoEditRuleCollection)collectionList.SelectedItem;
            var selectedItemIndex = ruleCollection.IndexOf(selectedItem);
            var guidIndex = collectionGuids.IndexOf(selectedItem.CollectionGuid);
            if (selectedItemIndex > 0)
            {
                ruleCollection.Move(selectedItemIndex, selectedItemIndex - 1);
                collectionGuids.Move(guidIndex, guidIndex - 1);
            }
            this.InitializeTester();
            this.TermInjectorConfig.SaveConfig();
        }

        private void MovePreRuleCollectionUp_Click(object sender, RoutedEventArgs e)
        {
            this.MoveCollectionUp(
                this.AutoPreEditRuleCollectionList, 
                this.TermInjectorConfig.AutoPreEditRuleCollections,
                this.TermInjectorConfig.AutoPreEditRuleCollectionGuids);
        }

        private void MovePostRuleCollectionUp_Click(object sender, RoutedEventArgs e)
        {
            this.MoveCollectionUp(
                this.AutoPostEditRuleCollectionList, 
                this.TermInjectorConfig.AutoPostEditRuleCollections,
                this.TermInjectorConfig.AutoPostEditRuleCollectionGuids);
        }

        private void Tester_Expanded(object sender, RoutedEventArgs e)
        {
            //Scroll to tester when it is expanded
            var tester = (Expander)sender;
            tester.BringIntoView();
        }
        
    }
}
