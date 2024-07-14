using HeayMyVoiceCommon.Contracts;
using HeayMyVoiceCommon.Models;
using Microsoft.ML;

namespace HeayMyVoiceBusinness
{

    public class MLHearMyVoice
    {
        private MLContext? mlContext;
        private ITransformer? model;
        private IDataView? trainingDataView;
        private PredictionEngine<ConversationModel, ConversationPredictionModel>? _predictionEngine;
        public MLHearMyVoice()
        {
            mlContext = new MLContext(seed: 0);
            loadModel();
            
        }

        private void loadModel()
        {
            try
            {
                 model = mlContext!.Model.Load(Tools.GetResource("ModelFile"), out var modelInputSchema);
            }
            catch
            {
                var conversaciones = ReadConversationsFromFile(Tools.GetResource("TrainFile"));
                trainingDataView = mlContext!.Data.LoadFromEnumerable(conversaciones);
                var pipeline = processData();
                var trainingPipe = buildTrainerModel(trainingDataView, pipeline);
                saveModelsAsFile();
            }
        }

        static IEnumerable<ConversationModel> ReadConversationsFromFile(string rutaArchivo)
        {
            var conversaciones = new List<ConversationModel>();

            foreach (var linea in File.ReadLines(rutaArchivo))
            {
                var partes = linea.Split(',');
                if (partes.Length == 2)
                {
                    conversaciones.Add(new ConversationModel
                    {
                        Entrada = partes[0].Trim(),
                        Salida = partes[1].Trim()
                    });
                }
            }

            return conversaciones;
        }
        IEstimator<ITransformer> processData()
        {
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Salida", outputColumnName: "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Entrada", outputColumnName: "EntradaFeaturized"))
                .Append(mlContext.Transforms.Concatenate("Features", "EntradaFeaturized"))
                .AppendCacheCheckpoint(mlContext);



            return pipeline;
        }
        IEstimator<ITransformer> buildTrainerModel(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
        {
            // Añadir un clasificador SDCA Maximum Entropy al pipeline
            var trainingPipe = pipeline.Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                                       .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // Entrenar el modelo con los datos de entrenamiento
            model = trainingPipe.Fit(trainingDataView);

            return trainingPipe;
        }
        void saveModelsAsFile()
        {
            mlContext!.Model.Save(model, trainingDataView!.Schema, Tools.GetResource("ModelFile"));

        }
        public string predict(string entradaUsuario)
        {

            var model = mlContext!.Model.Load(Tools.GetResource("ModelFile"), out var modelInputSchema);
            var entrada = new ConversationModel { Entrada = entradaUsuario };
            _predictionEngine = mlContext.Model.CreatePredictionEngine<ConversationModel, ConversationPredictionModel>(model);
            var result = _predictionEngine.Predict(entrada);
            return result.RespuestaPredicha;


        }
    }
}
