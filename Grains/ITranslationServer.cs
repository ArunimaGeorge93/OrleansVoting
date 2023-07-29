using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VotingContract
{
    public interface ITranslationServer : IGrainWithGuidKey
    {
        Task<List<LanguageItem>> GetAllLanguages();
        Task ToggleLanguageActiveStatus(string language);
        Task<List<TranslationResponse>> Translate(string phrase, string phraseLanguage = "en");
    }
    [GenerateSerializer]
    public record LanguageItem(string Name, string Code,bool IsTranslatorReady);
    //public record LanguageItem(string Name, string Code)
    //{
    //    public bool IsTranslatorReady { get; set; }
    //}
    [GenerateSerializer]
    public record TranslationResponse(string Language, string Translation, string OriginalPhrase);
}
