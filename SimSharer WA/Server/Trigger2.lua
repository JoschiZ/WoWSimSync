-- /run WeakAuras.ScanEvents("SHARER_EXPORT_STRINGS")
-- SHARER_EXPORT_STRINGS
function(event)
    if event == "OPTIONS" then return end
    if not event == "SHARER_EXPORT_STRINGS" then return end
    if not aura_env.isCollecting then return end
    local json = aura_env.json;
    if not json then
        return
    end
    
    local simc = _G.LibStub("AceAddon-3.0"):GetAddon("Simulationcraft");
    local f = simc:GetMainFrame(json.stringify(aura_env.db))
    f:Show()
end