namespace SimRunner;

internal sealed record GetCharacterResponse(
    int Id,
    string Name,
    string Realm
    );