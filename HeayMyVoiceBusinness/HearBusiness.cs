using Google.Cloud.Translation.V2;
using HeayMyVoiceCommon.Contracts;
using HeayMyVoiceCommon.Models;

namespace HeayMyVoiceBusinness
{
    public class HearBusiness : BaseBusiness, IHearService
    {
        private readonly TranslationClient _translationClient;

        public HearBusiness()
        {
            //// Inicializa el cliente de traducción con la clave de API de Google Cloud
            //_translationClient = TranslationClient.CreateFromApiKey(Tools.GetResource("TranslateKeyGoogle"));
        }

        public HearMyVoiceModel PossibleAnswer(string text)
        {
            HearMyVoiceModel hearMyVoice = new HearMyVoiceModel();  
            try
            {

                MLHearMyVoice mLHearMy = new MLHearMyVoice();
                hearMyVoice.PredictionText = mLHearMy.predict(text);
                hearMyVoice.TranslatedText = _translationClient.TranslateText(text, "es").TranslatedText;

                return hearMyVoice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Dispose()
        {
            _translationClient.Dispose();
        }
    }
}
