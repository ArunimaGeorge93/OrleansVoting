using Grains;
using VotingContract;
namespace Voting.Data
{
    public sealed partial class TranslatorService
    {
        private readonly IGrainFactory _grainFactory;
        private ITranslationServer _translationServer;

        public TranslatorService(IGrainFactory grainFactory) => _grainFactory = grainFactory;

        public void Initialize(Guid guid) =>
        _translationServer = _grainFactory.GetGrain<ITranslationServer>(guid);

        //public Task<List<LanguageItem>> GetLanguages(Guid guid) =>
        //_translationServer.GetAllLanguages();

    }
}
