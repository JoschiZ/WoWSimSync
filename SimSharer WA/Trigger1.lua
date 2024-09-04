-- SHARER_START_COLLECTION
function(event)
    if event ~= "SHARER_START_COLLECTION" then return end
    aura_env.db = {}
    aura_env.isCollecting = true;
    aura_env:debugPrint("start collecting", aura_env.db);
    aura_env:SendCommMessage(aura_env.messagePrefixes.getProfile, "please send profiles", "GUILD");
end