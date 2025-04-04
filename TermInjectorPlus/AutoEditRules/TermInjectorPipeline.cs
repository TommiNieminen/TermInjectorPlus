﻿using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using YamlDotNet.Serialization;

namespace TermInjectorPlus
{
    /// <summary>
    /// This contains all the components of a TermInjector pipeline 
    /// </summary>
    public class TermInjectorPipeline
    {
        [YamlMember(Alias = "name", ApplyNamingConventions = false)]
        public string PipelineName { get; set; }

        [YamlMember(Alias = "is-template", ApplyNamingConventions = false)]
        public bool IsTemplate { get; set; }


        [YamlMember(Alias = "guid", ApplyNamingConventions = false)]
        public string PipelineGuid;

        [YamlMember(Alias = "nested-tp-uri", ApplyNamingConventions = false)]
        public string NestedTranslationProviderUri;

        [YamlMember(Alias = "auto-pre-edit-rule-collection-guids", ApplyNamingConventions = false)]
        public ObservableCollection<string> AutoPreEditRuleCollectionGuids { get; internal set; }

        [YamlMember(Alias = "auto-post-edit-rule-collection-guids", ApplyNamingConventions = false)]
        public ObservableCollection<string> AutoPostEditRuleCollectionGuids { get; internal set; }


        private ITranslationProvider nestedTranslationProvider;

        [YamlIgnore]
        public LanguagePair[] LanguagePairs;


        [YamlIgnore]
        public FileInfo ConfigFile;

        [YamlIgnore]
        public ITranslationProviderCredentialStore CredentialStore;

        public TermInjectorPipeline()
        {
            this.PipelineGuid = System.Guid.NewGuid().ToString();
            this.AutoPostEditRuleCollectionGuids = new ObservableCollection<string>();
            this.AutoPreEditRuleCollectionGuids = new ObservableCollection<string>();
            this.AutoPreEditRuleCollections = new ObservableCollection<AutoEditRuleCollection>();
            this.AutoPostEditRuleCollections = new ObservableCollection<AutoEditRuleCollection>();
        }

        public SearchResults ProcessInput(
            string input, 
            LanguagePair languagePair,
            SearchSettings settings,
            bool applyEditRules=true)
        {
            //Preprocess input with pre-edit rules
            string preeditedInput = input;
            if (applyEditRules)
            {
                foreach (var preEditRuleCollection in this.AutoPreEditRuleCollections)
                {
                    if (!preEditRuleCollection.NoMatchCollection)
                    {
                        preeditedInput = preEditRuleCollection.ProcessPreEditRules(preeditedInput).Result;
                    }
                }
            }

            SearchResults results;
            TranslationUnit tu = new TranslationUnit();
            Segment orgSegment = new Segment();
            orgSegment.Culture = languagePair.SourceCulture;
            orgSegment.Add(preeditedInput);
            tu.SourceSegment = orgSegment;

            if (this.NestedTranslationProvider != null && this.NestedTranslationProvider.SupportsLanguageDirection(languagePair))
            {
                results = this.NestedTranslationProvider.GetLanguageDirection(languagePair).SearchTranslationUnit(settings, tu);
            }
            else
            {
                results = new SearchResults();
                results.SourceSegment = orgSegment;
            }

            //If there are no results, apply the no-match rules
            if (results.Count == 0)
            {
                bool noMatchRulesApplied = false;
                preeditedInput = input;
                foreach (var preEditRuleCollection in this.AutoPreEditRuleCollections)
                {
                    if (preEditRuleCollection.NoMatchCollection)
                    {
                        var autoEditResult = preEditRuleCollection.ProcessPreEditRules(preeditedInput);
                        if (autoEditResult.AppliedReplacements.Any())
                        {
                            noMatchRulesApplied = true;
                        }
                        preeditedInput = autoEditResult.Result;
                    }
                }

                if (noMatchRulesApplied)
                {
                    var noMatchSource = new Segment(languagePair.SourceCulture);
                    noMatchSource.Add(input);
                    var noMatchTarget = new Segment(languagePair.TargetCulture);
                    noMatchTarget.Add(preeditedInput);
                    var noMatchTu = new TranslationUnit(noMatchSource, noMatchTarget);
                    var noMatchResult = this.CreateSearchResult(noMatchSource, noMatchTarget, input, false);
                    results.Add(noMatchResult);
                }
            }
            //Apply post-edit rules
            else
            {
                foreach (var result in results)
                {
                    Segment targetSeg;

                    //It seems that TMs have the target in memorytranslationunit and
                    //MT in translationproposal
                    if (result.TranslationProposal != null)
                    {
                        targetSeg = result.TranslationProposal.TargetSegment;
                    }
                    else
                    {
                        targetSeg = result.MemoryTranslationUnit.TargetSegment;
                    }

                    var plainOutput = targetSeg.ToPlain();
                    foreach (var postEditRuleCollection in this.AutoPostEditRuleCollections)
                    {
                        plainOutput = postEditRuleCollection.ProcessPostEditRules(input, plainOutput).Result;
                    }

                    targetSeg.Clear();
                    targetSeg.Add(plainOutput);
                }
            }

            return results;
        }

        private SearchResult CreateSearchResult(Segment searchSegment, Segment translation,
            string sourceSegment, bool formattingPenalty)
        {
            
            TranslationUnit tu = new TranslationUnit();
            Segment orgSegment = new Segment();
            orgSegment.Add(sourceSegment);
            tu.SourceSegment = orgSegment;
            tu.TargetSegment = translation;

            tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);

            #region "TuProperties"
            int score = 0; //TODO: determine scoring for TM result to return to Studio
            tu.Origin = TranslationUnitOrigin.TM;


            SearchResult searchResult = new SearchResult(tu);
            searchResult.ScoringResult = new ScoringResult();
            searchResult.ScoringResult.BaseScore = score;

            //TODO: determine the confirmation level, possibly conditional on the score
            //e.g.:
            tu.ConfirmationLevel = Sdl.Core.Globalization.ConfirmationLevel.Draft;

            #endregion

            return searchResult;
        }

        [YamlIgnore]
        public ObservableCollection<AutoEditRuleCollection> AutoPostEditRuleCollections { get; set; }

        [YamlIgnore]
        public ObservableCollection<AutoEditRuleCollection> AutoPreEditRuleCollections { get; set; }
        
        //TODO: instantiate the nested translation provider only when needed, do it here in the property
        [YamlIgnore]
        public ITranslationProvider NestedTranslationProvider
        {
            get
            {
                if (this.nestedTranslationProvider == null)
                {
                    if (!String.IsNullOrWhiteSpace(this.NestedTranslationProviderUri))
                    {
                        //TODO: add check for whether translation provider supports language pairs of project
                        //FIX: This seems all messed up, the translation provider should be set up only once
                        this.nestedTranslationProvider =
                            NestedTPFactory.InstantiateNestedTP(
                                this.NestedTranslationProviderUri,
                                this.CredentialStore,
                                this.LanguagePairs);
                        //TODO: if this return null, the uri should be emptied, since the nested tp
                        //has become invalid
                    }
                    else
                    {
                        this.nestedTranslationProvider = null;
                    }

                    return this.nestedTranslationProvider;
                }
                else
                { 
                    return this.nestedTranslationProvider; 
                }
            }
                
        }

        internal void SaveConfig()
        {
            //The configs are saved in the terminjector folder in appdata, using GUIDs as file names.
            var configDir = new DirectoryInfo(
                HelperFunctions.GetLocalAppDataPath(TermInjectorPlusSettings.Default.ConfigDir));
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

            this.ConfigFile = new FileInfo(configPath);
        }

        public static TermInjectorPipeline CreateFromFile(
            FileInfo configFileInfo, 
            ITranslationProviderCredentialStore credentialStore,
            bool createFromTemplate = false)
        {
            TermInjectorPipeline pipeline;
            var deserializer = new Deserializer();
            using (var reader = configFileInfo.OpenText())
            {
                pipeline = deserializer.Deserialize<TermInjectorPipeline>(reader);
            }
            
            pipeline.ConfigFile = configFileInfo;
            if (createFromTemplate)
            {
                //Create a new config duplicating the template, except
                //for the GUID, also mark the config as non-template
                pipeline.PipelineGuid = Guid.NewGuid().ToString();
                pipeline.IsTemplate = false;
            }

            pipeline.CredentialStore = credentialStore;

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
                HelperFunctions.GetLocalAppDataPath(TermInjectorPlusSettings.Default.EditRuleDir));
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

        internal Boolean DeleteTemplate()
        {
            //The configs are saved in the terminjector folder in appdata, using GUIDs as file names.
            var configDir = new DirectoryInfo(
                HelperFunctions.GetLocalAppDataPath(TermInjectorPlusSettings.Default.ConfigDir));
            var templatePath = Path.Combine(
                configDir.FullName, $"{this.PipelineGuid}.yml");
            MessageBoxResult messageBoxResult =
                    System.Windows.MessageBox.Show(
                        "Are you sure you want to delete this template.", "Confirm template deletion", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Cancel)
            {
                return false;
            }
            else //(messageBoxResult == MessageBoxResult.Yes)
            {
                File.Delete(templatePath);
                return true;
            }
        }        

        internal Boolean SaveAsTemplate(ObservableCollection<TermInjectorPipeline> pipelineTemplates)
        {
            var pipelineWithSameName = pipelineTemplates.SingleOrDefault(x => x.PipelineName == this.PipelineName);
            if (pipelineWithSameName != null)
            {
                MessageBoxResult messageBoxResult = 
                    System.Windows.MessageBox.Show(
                        "Click Yes to replace the existing template, or No to create a new template, or Cancel to do neither.","Template with same name exists",MessageBoxButton.YesNoCancel);
                if (messageBoxResult == MessageBoxResult.Cancel)
                {
                    return false;
                }
                else if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var currentGuid = this.PipelineGuid;
                    this.PipelineGuid = pipelineWithSameName.PipelineGuid;
                    this.IsTemplate = true;
                    this.SaveConfig();
                    pipelineTemplates.Remove(pipelineWithSameName);
                    pipelineTemplates.Add(TermInjectorPipeline.CreateFromFile(this.ConfigFile, this.CredentialStore));
                    this.PipelineGuid = currentGuid;
                    this.IsTemplate = false;
                    return false;
                }
                else //Result is No, creates a new template
                {
                    var currentGuid = this.PipelineGuid;
                    this.PipelineGuid = System.Guid.NewGuid().ToString();
                    this.IsTemplate = true;
                    this.SaveConfig();
                    pipelineTemplates.Add(TermInjectorPipeline.CreateFromFile(this.ConfigFile, this.CredentialStore));
                    this.PipelineGuid = currentGuid;
                    this.IsTemplate = false;
                    return true;
                }
            }
            else
            {
                var currentGuid = this.PipelineGuid;
                this.PipelineGuid = System.Guid.NewGuid().ToString();
                this.IsTemplate = true;
                this.SaveConfig();
                pipelineTemplates.Add(TermInjectorPipeline.CreateFromFile(this.ConfigFile, this.CredentialStore));
                this.PipelineGuid = currentGuid;
                this.IsTemplate = false;
                return true;
            }

        }
    }
}
