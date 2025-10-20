using Microsoft.ML;
using Microsoft.ML.Data;

namespace MusicApp.Infrastructure.ML;

public class RecommendationModel
{
    private readonly MLContext _ml = new();
    private readonly PredictionEngine<ModelInput, ModelOutput>? _engine;

    public RecommendationModel()
    {
        try
        {
            // eğer model.zip varsa yükle
            var model = _ml.Model.Load("model.zip", out _);
            _engine = _ml.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);
        }
        catch
        {
            _engine = null; // model yoksa fallback
        }
    }

    // Kullanıcı ve şarkı ID’sine göre tahmin skorunu döndürür
    public float PredictScore(int userId, int songId)
    {
        if (_engine is null)
            return 0.5f; // model yoksa varsayılan skor

        var input = new ModelInput
        {
            UserId = (uint)userId,
            SongId = (uint)songId
        };

        var prediction = _engine.Predict(input);
        return prediction.Score;
    }
}

// Model giriş sınıfı (kullanıcı ve şarkı kimlikleri)
public class ModelInput
{
    [KeyType(count: 100000)]
    public uint UserId { get; set; }

    [KeyType(count: 100000)]
    public uint SongId { get; set; }
}

// Model çıktı sınıfı (tahmin sonucu)
public class ModelOutput
{
    public float Score { get; set; }
}
