using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Serialization;

namespace TermInjector2022
{
    /// <summary>
    /// This contains all the components of a TermInjector pipeline 
    /// </summary>
    public class TermInjectorPipeline
    {
        [YamlMember(Alias = "name", ApplyNamingConventions = false)]
        public string PipelineName { get; set; }
        

        [YamlMember(Alias = "guid", ApplyNamingConventions = false)]
        public string PipelineGuid;

        [YamlMember(Alias = "nested-tp-uri", ApplyNamingConventions = false)]
        public string NestedTranslationProviderUri;

        [YamlMember(Alias = "auto-pre-edit-rule-collection-guids", ApplyNamingConventions = false)]
        public ObservableCollection<string> AutoPreEditRuleCollectionGuids { get; internal set; }

        [YamlMember(Alias = "auto-post-edit-rule-collection-guids", ApplyNamingConventions = false)]
        public ObservableCollection<string> AutoPostEditRuleCollectionGuids { get; internal set; }

        private ITranslationProvider nestedTranslationProvider;

        private FileInfo configFile;

        public TermInjectorPipeline()
        {
            this.PipelineGuid = System.Guid.NewGuid().ToString();
            this.AutoPostEditRuleCollectionGuids = new ObservableCollection<string>();
            this.AutoPreEditRuleCollectionGuids = new ObservableCollection<string>();
            this.AutoPreEditRuleCollections = new ObservableCollection<AutoEditRuleCollection>();
            this.AutoPostEditRuleCollections = new ObservableCollection<AutoEditRuleCollection>();
        }

        public string ProcessInput(string input, LanguagePair languagePair, bool applyEditRules=true)
        {
            //Preprocess input with pre-edit rules
            if (applyEditRules)
            {
                foreach (var preEditRuleCollection in this.AutoPreEditRuleCollections)
                {
                    input = preEditRuleCollection.ProcessPreEditRules(input).Result;
                }
            }

            if (this.nestedTranslationProvider == null)
            {
                this.nestedTranslationProvider = NestedTPFactory.InstantiateNestedTP(this.NestedTranslationProviderUri, this.CredentialStore);
            }


            return "";
        }

        public ObservableCollection<AutoEditRuleCollection> AutoPostEditRuleCollections { get; set; }
        public ObservableCollection<AutoEditRuleCollection> AutoPreEditRuleCollections { get; set; }
        public ITranslationProviderCredentialStore CredentialStore { get; private set; }

        internal void SaveConfig()
        {
            //The configs are saved in the terminjector folder in appdata, using GUIDs as file names.
            var configDir = new DirectoryInfo(
                HelperFunctions.GetLocalAppDataPath(TermInjector2022Settings.Default.ConfigDir));
            if (!configDir.Exists)
            {
                configDir.Create();
            }
            
            var configTempPath = Path.Combine(
                configDir.FullName, $"{this.PipelineGuid}_temp.yml");
            var configPath = Path.Combine(
                configDir.FullName, $"{this.PipelineGuid}.yml");
            var serializer = new Serializer();

            //Don't replace current file yet
            using (var writer = File.CreateText(configTempPath))
            {
                serializer.Serialize(writer, this, typeof(TermInjectorPipeline));
            }

            if (!File.Exists(configPath))
            {
                File.Move(configTempPath, configPath);
            }
            else
            {
                //Safe replacement according to Jon Skeet
                string backup = configPath + ".bak";
                File.Delete(backup);
                File.Replace(configTempPath, configPath, backup, true);
                try
                {
                    File.Delete(backup);
                }
                catch
                {
                    // optional:
                    // filesToDeleteLater.Add(backup);
                }
            }

        }

        public static TermInjectorPipeline CreateFromFile(FileInfo configFileInfo, bool assignNewId = false)
        {
            TermInjectorPipeline pipeline;
            var deserializer = new Deserializer();
            using (var reader = configFileInfo.OpenText())
            {
                pipeline = deserializer.Deserialize<TermInjectorPipeline>(reader);
            }
            pipeline.configFile = configFileInfo;
            if (assignNewId)
            {
                pipeline.PipelineGuid = Guid.NewGuid().ToString();
            }
            
            pipeline.AutoPreEditRuleCollections = new ObservableCollection<AutoEditRuleCollection>();
            pipeline.AutoPostEditRuleCollections = new ObservableCollection<AutoEditRuleCollection>();

            TermInjectorPipeline.UpdateCollectionsFromGuids(
                pipeline.AutoPreEditRuleCollectionGuids,
                pipeline.AutoPreEditRuleCollections);

            TermInjectorPipeline.UpdateCollectionsFromGuids(
                pipeline.AutoPostEditRuleCollectionGuids,
                pipeline.AutoPostEditRuleCollections);

            return pipeline;
        }

        private static void UpdateCollectionsFromGuids(
            ObservableCollection<string> guids, ObservableCollection<AutoEditRuleCollection> collections)
        {
            var editRuleDir = new DirectoryInfo(
                HelperFunctions.GetLocalAppDataPath(TermInjector2022Settings.Default.EditRuleDir));
            var editRuleCollectionFiles = editRuleDir.GetFiles("*.yml");

            foreach (var guid in guids)
            {
                var ruleFile =
                    editRuleCollectionFiles.SingleOrDefault(
                        x => x.Name == $"{guid}.yml");

                if (ruleFile != null)
                {
                    collections.Add(AutoEditRuleCollection.CreateFromFile(ruleFile));
                }
            }
        }
    }
}
