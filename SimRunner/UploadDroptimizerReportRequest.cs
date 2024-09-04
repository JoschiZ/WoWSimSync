using System.Text.Json.Serialization;

namespace SimRunner;

internal sealed record UploadDroptimizerReportRequest(
    [property: JsonPropertyName("report_id")]
    string ReportId, 
    [property: JsonPropertyName("character_id")]
    int CharacterId, 
    [property: JsonPropertyName("character_name")]
    string CharacterName, 
    [property: JsonPropertyName("configuration_name")]
    string ConfigurationName = "Single Target",
    [property: JsonPropertyName("replace_manual_edits")]
    bool ReplaceManualEdits = true,
    [property: JsonPropertyName("clear_conduits")]
    bool ClearConduits = true);