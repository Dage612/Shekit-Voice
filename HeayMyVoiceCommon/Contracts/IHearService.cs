using HeayMyVoiceCommon.Models;

namespace HeayMyVoiceCommon.Contracts
{
    public interface IHearService
    {
        HearMyVoiceModel PossibleAnswer(string text);
    }
}
