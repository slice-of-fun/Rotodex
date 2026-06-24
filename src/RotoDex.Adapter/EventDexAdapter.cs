using System;
using System.Collections.Generic;
using System.Linq;
using Roto.Core;

namespace RotoDex.Adapter;

public class EventDto
{
    public string Title { get; set; } = string.Empty;
    public int Level { get; set; }
    public int Generation { get; set; }
}

public static class EventDexAdapter
{
    public static List<EventDto>? GetEventsForSpecies(string speciesName)
    {
        if (string.IsNullOrWhiteSpace(speciesName))
            return null;

        if (!Enum.TryParse<Species>(speciesName, true, out var species))
            return null;

        var allEvents = EncounterEvent.GetAllEvents();
        var matchingEvents = allEvents.Where(e => e.Species == (ushort)species).ToList();

        if (matchingEvents.Count == 0)
            return new List<EventDto>();

        return matchingEvents.Select(e => new EventDto
        {
            Title = e.CardTitle ?? "Unknown Event",
            Level = e.Level,
            Generation = e.Generation
        }).ToList();
    }
}
