using Microsoft.ML;
using Microsoft.ML.Data;

class Program

{
    // Variables Globales


    static void Main()
    {
        string trainFile = "C:/Users/Dage6/OneDrive/Escritorio/ML1.txt";
        string modelFile = "C:/Users/Dage6/OneDrive/Escritorio/ML2.zip";
        PredictionEngine<Conversacion, PrediccionConversacion> _predictionEngine;
        ITransformer _model;
        MLContext mlContext = new MLContext(seed: 0);
        var conversaciones = ReadConversationsFromFile("C:/Users/Dage6/OneDrive/Escritorio/ML1.txt");
        IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(conversaciones);
        var pipeline = ProcessData();
        var trainingPipe = BuildTrainerModel(trainingDataView, pipeline);
        saveModelsAsFile();
        Console.Write("Ingresa un mensaje: ");
        var entradaUsuario = Console.ReadLine();
        var result = predict(entradaUsuario);
        Console.WriteLine(result); 
        string predict(string entradaUsuario)
        {
            var model = mlContext.Model.Load(modelFile, out var modelInputSchema);
            var entrada = new Conversacion { Entrada = entradaUsuario };
            _predictionEngine = mlContext.Model.CreatePredictionEngine<Conversacion, PrediccionConversacion>(model);
            var result = _predictionEngine.Predict(entrada);
            return result.RespuestaPredicha;

        }
        void saveModelsAsFile()
        {
            mlContext.Model.Save(_model, trainingDataView.Schema, modelFile);

        }
        IEstimator<ITransformer> ProcessData()
        {
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Salida", outputColumnName: "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "Entrada", outputColumnName: "EntradaFeaturized"))
                .Append(mlContext.Transforms.Concatenate("Features", "EntradaFeaturized"))
                .AppendCacheCheckpoint(mlContext);
           

            return pipeline;
        }
        IEstimator<ITransformer> BuildTrainerModel(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
        {
            // Añadir un clasificador SDCA Maximum Entropy al pipeline
            var trainingPipe = pipeline.Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                                       .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            // Entrenar el modelo con los datos de entrenamiento
            _model = trainingPipe.Fit(trainingDataView);

            return trainingPipe;
        }
        static IEnumerable<Conversacion> ReadConversationsFromFile(string rutaArchivo)
        {
         var conversaciones = new List<Conversacion>();

         foreach (var linea in File.ReadLines(rutaArchivo))
    {
        var partes = linea.Split(',');
        if (partes.Length == 2)
        {
            conversaciones.Add(new Conversacion
            {
                Entrada = partes[0].Trim(),
                Salida = partes[1].Trim()
            });
        }
    }

          return conversaciones;
        }

    }



}

// Class to hold the prediction result
public class PrediccionConversacion
{
    // Specify the name of the column in the prediction output
    [ColumnName("PredictedLabel")]
    public string RespuestaPredicha { get; set; }
}

// Class to represent a conversation with input and output
public class Conversacion
{
    // LoadColumn attribute is used to specify the column index when reading from a file
    [LoadColumn(0)]
    public string Entrada { get; set; }

    [LoadColumn(1)]
    public string Salida { get; set; }
}
