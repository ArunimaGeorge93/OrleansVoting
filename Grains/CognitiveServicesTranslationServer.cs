using Microsoft.Extensions.Configuration;
using Orleans;
using Voting.Data;
using Voting.Pages;
using VotingContract;
using static System.Net.Mime.MediaTypeNames;

namespace VotingData
{
    public class CognitiveServicesTranslationServer : Grain, ITranslationServer
    {
        public CognitiveServicesTranslationServer(IConfiguration configuration,
            IGrainFactory grainFactory,
            IHttpClientFactory httpClientFactory)
        {
            Configuration = configuration;
            HttpClientFactory = httpClientFactory;
            _grainFactory = grainFactory;
        }

        private IGrainFactory _grainFactory;
        public IConfiguration Configuration { get; }
        public IHttpClientFactory HttpClientFactory { get; }
        //public record LanguageItem(string Name, string Code);
        
        public List<LanguageItem> Languages { get; internal set; } = new List<LanguageItem>();
        //List<LanguageItem> Languages = new List<LanguageItem>();

        public async Task<List<LanguageItem>> GetAllLanguages()
        {
            if (!Languages.Any())
            {
                using (var client = HttpClientFactory.CreateClient())
                using (var request = client.SetupLanguageListRequestFromConfiguration(Configuration))
                {
                    request.Method = HttpMethod.Get;
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    Languages = await response.ParseLanguagesApiCall();
                }
            }

            //Languages.Add(new LanguageItem("test", "test"));
            //Languages.First(_ => _.Code == "test").IsTranslatorReady = true;

            return Languages;
        }

        public Task ToggleLanguageActiveStatus(string language)
        {
            var isReady = !(Languages.First(_ => _.Code == language).IsTranslatorReady);
           // var translator = GrainFactory.GetGrain<ITranslator>(language);
            //Languages.First(_ => _.Code == language).IsTranslatorReady = isReady;
            //Languages.Add(new LanguageItem("test1", "test1"));
            //Languages.First(_ => _.Code == "test1").IsTranslatorReady = true;
            return Task.CompletedTask;
        }

        public Task<List<TranslationResponse>> Translate(string phrase, string phraseLanguage = "en")
        {
            var languagesWithTranslators = Languages.Where(_ => _.IsTranslatorReady).ToList();
            var result = new List<TranslationResponse>();
            var taskList = new List<Task>();

            foreach (var language in languagesWithTranslators)
            {
                taskList.Add(Task.Run(async () =>
                {
                    var translator = _grainFactory.GetGrain<ITranslator>(language.Code);
                    var translatedPhrase = await translator.Translate(phrase);
                    result.Add(new TranslationResponse(language.Code, translatedPhrase, phrase));
                }));
            }

            Task.WhenAll(taskList).Wait();

            return Task.FromResult(result);
        }
    }
}
