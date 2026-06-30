using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Services;

public class SpacedRepetitionService
{
  public UserProgress ApplyReview(UserProgress progress, int quality)
  {
    if (quality is < 0 or > 5)
      throw new ArgumentOutOfRangeException(nameof(quality), "Quality must be between 0 and 5.");

    var updated = new UserProgress
    {
      Id = progress.Id,
      UserId = progress.UserId,
      ItemId = progress.ItemId,
      ItemType = progress.ItemType,
      EaseFactor = progress.EaseFactor,
      Interval = progress.Interval,
      Repetitions = progress.Repetitions,
      NextReviewDate = progress.NextReviewDate,
      CorrectCount = progress.CorrectCount,
      LastReviewed = DateTime.UtcNow
    };

    if (quality >= 3)
    {
      updated.CorrectCount = progress.CorrectCount + 1;

      if (progress.Repetitions == 0)
        updated.Interval = 1;
      else if (progress.Repetitions == 1)
        updated.Interval = 6;
      else
        updated.Interval = (int)Math.Round(progress.Interval * progress.EaseFactor);

      updated.Repetitions = progress.Repetitions + 1;
    }
    else
    {
      updated.Repetitions = 0;
      updated.Interval = 1;
    }

    var easeDelta = 0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02);
    updated.EaseFactor = Math.Max(1.3, progress.EaseFactor + easeDelta);
    updated.NextReviewDate = DateTime.UtcNow.AddDays(updated.Interval);

    return updated;
  }

  public UserProgress CreateInitialProgress(string userId, string itemId, ItemType itemType = ItemType.Vocabulary)
  {
    return new UserProgress
    {
      UserId = userId,
      ItemId = itemId,
      ItemType = itemType,
      EaseFactor = 2.5,
      Interval = 0,
      Repetitions = 0,
      NextReviewDate = DateTime.UtcNow,
      CorrectCount = 0
    };
  }
}
