using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectApi;

namespace TermInjectorPlus
{
    class TermInjectorPlusTPLanguageDirection : ITranslationProviderLanguageDirection
    {
        
        #region "PrivateMembers"
        private TermInjectorPlusTP _provider;
        private LanguagePair _languagePair;
        private TermInjectorPlusTPOptions _options;
        private TermInjectorPlusElementVisitor _visitor;
        private ITranslationProviderLanguageDirection _nestedTpLanguageDirection;
        #endregion

        #region "ITranslationProviderLanguageDirection Members"

        /// <summary>
        /// Instantiates an instance
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="languages"></param>
        #region "ProviderLanguageDirection"
        public TermInjectorPlusTPLanguageDirection(TermInjectorPlusTP provider, LanguagePair languages)
        {
            #region "Instantiate"
            _provider = provider;
            _languagePair = languages;
            _options = _provider.Options;
            _visitor = new TermInjectorPlusElementVisitor(_options);
            this._nestedTpLanguageDirection =
                this._provider.TerminjectorPipeline.NestedTranslationProvider.GetLanguageDirection(_languagePair);
            #endregion

            //TODO: any instantiation code for ILanguageDirection
        }

        #endregion

        public System.Globalization.CultureInfo SourceLanguage
        {
            get { return _languagePair.SourceCulture; }
        }

        public System.Globalization.CultureInfo TargetLanguage
        {
            get { return _languagePair.TargetCulture; }
        }

        public ITranslationProvider TranslationProvider
        {
            get { return _provider; }
        }

        /// <summary>
        /// Performs the actual search -- called by Studio on batch translation, interactive translation, and concordance search
        /// To determine the type of search check the SearchSettings.Mode
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="segment"></param>
        /// <returns></returns>
        #region "SearchSegment"
        public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {

            var plain = segment.ToPlain();
            var results = this._provider.TerminjectorPipeline.ProcessInput(plain, _languagePair);
            return results;
            #endregion
        }
        #endregion


        /// <summary>
        /// Creates the translation unit as it is later shown in the Translation Results
        /// window of SDL Trados Studio. This member also determines the match score
        /// (in our implementation always 100%, as only exact matches are supported)
        /// as well as the confirmation lelvel, i.e. Translated.
        /// </summary>
        /// <param name="searchSegment"></param>
        /// <param name="translation"></param>
        /// <param name="sourceSegment"></param>
        /// <returns></returns>
        #region "CreateSearchResult"
        private SearchResult CreateSearchResult(Segment searchSegment, Segment translation,
            string sourceSegment, bool formattingPenalty)
        {
            #region "TranslationUnit"
            TranslationUnit tu = new TranslationUnit();
            Segment orgSegment = new Segment();
            orgSegment.Add(sourceSegment);
            tu.SourceSegment = orgSegment;
            tu.TargetSegment = translation;
            #endregion

            tu.ResourceId = new PersistentObjectToken(tu.GetHashCode(), Guid.Empty);

            #region "TuProperties"
            int score = 100; //TODO: determine scoring for TM result to return to Studio
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
        #endregion


        public bool CanReverseLanguageDirection
        {
            get { return false; }
        }

        public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
        {
            SearchResults[] results = new SearchResults[segments.Length];
            for (int p = 0; p < segments.Length; ++p)
            {
                results[p] = SearchSegment(settings, segments[p]);
            }
            return results;
        }

        public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
        {
            if (segments == null)
            {
                throw new ArgumentNullException("segments in SearchSegmentsMasked");
            }
            if (mask == null || mask.Length != segments.Length)
            {
                throw new ArgumentException("mask in SearchSegmentsMasked");
            }

            SearchResults[] results = new SearchResults[segments.Length];
            for (int p = 0; p < segments.Length; ++p)
            {
                if (mask[p])
                {
                    results[p] = SearchSegment(settings, segments[p]);
                }
                else
                {
                    results[p] = null;
                }
            }

            return results;
        }

        public SearchResults SearchText(SearchSettings settings, string segment)
        {
            Segment s = new Sdl.LanguagePlatform.Core.Segment(_languagePair.SourceCulture);
            s.Add(segment);
            return SearchSegment(settings, s);
        }

        public SearchResults SearchTranslationUnit(
            SearchSettings settings, 
            TranslationUnit translationUnit)
        {
            return SearchSegment(settings, translationUnit.SourceSegment);
        }

        public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
        {
            SearchResults[] results = new SearchResults[translationUnits.Length];
            for (int p = 0; p < translationUnits.Length; ++p)
            {
                results[p] = SearchSegment(settings, translationUnits[p].SourceSegment);
            }
            return results;
        }

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
            List<SearchResults> results = new List<SearchResults>();

            int i = 0;
            foreach (var tu in translationUnits)
            {
                if (mask == null || mask[i])
                {
                    var result = SearchTranslationUnit(settings, tu);
                    results.Add(result);
                }
                else
                {
                    results.Add(null);
                }
                i++;
            }

            return results.ToArray();
        }



        #region "NotForThisImplementation"
        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnits"></param>
        /// <param name="settings"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {
            return this._nestedTpLanguageDirection.AddTranslationUnitsMasked(translationUnits, settings, mask);
        }

        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnit"></param>
        /// <returns></returns>
        public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            return this._nestedTpLanguageDirection.UpdateTranslationUnit(translationUnit);
        }

        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnits"></param>
        /// <returns></returns>
        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            return this._nestedTpLanguageDirection.UpdateTranslationUnits(translationUnits);
        }
        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnits"></param>
        /// <param name="previousTranslationHashes"></param>
        /// <param name="settings"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            return this._nestedTpLanguageDirection.AddOrUpdateTranslationUnitsMasked(translationUnits, previousTranslationHashes, settings, mask);
        }

        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnit"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
        {
            return this._nestedTpLanguageDirection.AddTranslationUnit(translationUnit, settings);
        }

        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnits"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
        {
            return this._nestedTpLanguageDirection.AddTranslationUnits(translationUnits, settings);
        }

        /// <summary>
        /// Not required for this implementation.
        /// </summary>
        /// <param name="translationUnits"></param>
        /// <param name="previousTranslationHashes"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
        {
            return this._nestedTpLanguageDirection.AddOrUpdateTranslationUnits(
                translationUnits, previousTranslationHashes, settings);
        }
        #endregion
    }
}
