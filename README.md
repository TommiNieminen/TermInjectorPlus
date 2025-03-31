TermInjectorPlus is an updated version of the TermInjector plugin for Trados Studio. TermInjectorPlus can be used with most translation providers (translation memories and machine translation providers) available in Trados. TermInjector acts as a wrapper around a translation provider, and it can modify both the source text sent into the translation provider and the output text that the translation provider returns. TermInjectorPlus is adapted from the **Edit rules** functionality in [OPUS-CAT MT Engine](https://helsinki-nlp.github.io/OPUS-CAT/install) (my main open-source project).

In TermInjectorPlus, you can define three types of rules:

1. **Pre-edit rules**: Rules that modify source text before it is sent to a translation provider (translation memory or machine translation engine). These rules can be used to fix source errors or to modify constructions that are difficult for machine translation engines.
2. **Post-edit rules**: Rules that modify the translations from a translation provider. These rules can be used to fix recurring error in translation provider output.
3. **No-match rules**: Rules that modify the source text, which are only applied in case a translation provider does not provide a match. These rules can be used for instance to convert dates and numbers and to add translations for parts of the source text. No-match rules can also be used without specifying a translation provider.

Regular expressions ([.NET regex flavor](https://docs.microsoft.com/en-us/dotnet/standard/base-types/regular-expression-language-quick-reference)) can be used in the rules. Rules are organized into _rule collections_, which can contain any number of rules. 
