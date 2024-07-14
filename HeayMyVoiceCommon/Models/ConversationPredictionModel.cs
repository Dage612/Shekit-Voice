using Microsoft.ML.Data;

namespace HeayMyVoiceCommon.Models
{
    public class ConversationPredictionModel
    {
        // Specify the name of the column in the prediction output
        [ColumnName("PredictedLabel")]
        public string RespuestaPredicha { get; set; }
    }

}
