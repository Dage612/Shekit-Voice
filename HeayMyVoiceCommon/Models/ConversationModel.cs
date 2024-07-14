using Microsoft.ML.Data;

namespace HeayMyVoiceCommon.Models
{
    public class ConversationModel
    {
        // LoadColumn attribute is used to specify the column index when reading from a file
        [LoadColumn(0)]
        public string Entrada { get; set; }

        [LoadColumn(1)]
        public string Salida { get; set; }
    }
}
