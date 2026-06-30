namespace InglesApp.Application.DTOs.Placement;

public record PlacementQuestionDto(string ExerciseId, string Type, object Content, string CefrLevel);

public record SubmitPlacementAnswerRequest(string ExerciseId, bool Correct, string CurrentLevel);

public record PlacementResultDto(string AssignedLevel, string AssignedSubLevel, int CorrectAnswers, int TotalQuestions);
