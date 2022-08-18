using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


        private FileInfo configFile;

        public TermInjectorPipeline()
        {
            this.PipelineGuid = System.Guid.NewGuid().ToString();
            this.AutoPostEditRuleCollectionGuids = new ObservableCollection<string>();
            this.AutoPreEditRuleCollectionGuids = new ObservableCollection<string>();
        }

        public string ProcessInput(string input, bool applyEditRules)
        {
            return "";
        }

        public ObservableCollection<AutoEditRuleCollection> AutoPostEditRuleCollections { get; set; }
        public ObservableCollection<AutoEditRuleCollection> AutoPreEditRuleCollections { get; set; }

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
                serializer.Serialize(writer, this, typeof(TermInjectorConfig));
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
            return pipeline;
        }
    }
}
