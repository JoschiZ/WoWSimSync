-- /run WeakAuras.ScanEvents("SHARER_RESET")
-- SHARER_RESET
function(event)
    if not event == "SHARER_RESET" then return end
    aura_env.debugPrint("reset and stop collecting")
    aura_env.isCollecting = false;
    aura_env.displayText = "";
    aura_env.db = {}
    aura_env:UnregisterAllComm();
end