namespace MusicApp.Application.Abstractions;
using MusicApp.Application.Abstractions;
public interface IRecommendationModel
{
    float PredictScore(int userId, int songId);
}
