using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One asset per room (e.g. "Bedroom", "Kitchen", "Living Room").
/// Lets you define a room's hazards and their explanations as DATA,
/// so adding a new room is "create asset + fill fields", not new code.
///
/// Create via: Assets > Create > Earthquake Prep > Room Data
/// </summary>
[CreateAssetMenu(fileName = "NewRoomData", menuName = "Earthquake Prep/Room Data")]
public class RoomData : ScriptableObject
{
    [Header("Room Info")]
    public string roomName;

    [TextArea]
    public string roomIntroText;

    [Header("Hazards")]
    public List<HazardEntry> hazards = new List<HazardEntry>();

    [Header("Progression")]
    public string nextSceneName;
}

[System.Serializable]
public class HazardEntry
{
    public string hazardId;
    public string displayName;
    public string explanation;
    public Sprite infoIcon;
}