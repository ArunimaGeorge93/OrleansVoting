using Orleans;

namespace VotingContract
{
    public interface ITranslator : IGrainWithStringKey
    {
        Task<string> Translate(string originalPhrase, string originalLanguageCode = "en");
    }
}
