﻿using Timetable.Common.Enums;

namespace Timetable.Structure;

internal sealed class TimetableManager<TEvent> where
    TEvent : class
{
    public required CompiledProps<TEvent> Props { get; init; }
    public IList<TEvent> Events { get; init; } = [];

    public Grid<TEvent> Grid { get; set; } = default!;
    public DateTime CurrentDate { get; set; }
    public DisplayType DisplayType { get; set; }
    public bool HasRowTitle => Grid.RowPrepend.Count > 0;

    public TEvent? MoveEvent(Guid eventId, Guid targetCellId)
    {
        var currentCell = FindCellByEventId(eventId);
        if (currentCell is null) return null;

        var timetableEvent = currentCell.Events.FirstOrDefault(e => e.Id == eventId);
        if (timetableEvent is null) return null;

        var targetCell = FindCellById(targetCellId);
        if (targetCell is null) return null;

        var duration = timetableEvent.DateTo - timetableEvent.DateFrom;
        var newEndDate = targetCell.DateTime.Add(duration);

        timetableEvent.DateFrom = targetCell.DateTime;
        timetableEvent.DateTo = newEndDate;

        currentCell.Events.Remove(timetableEvent);
        targetCell.Events.Add(timetableEvent);

        return timetableEvent.Event;
    }

    public IList<TEvent>? MoveGroupEvents(Guid eventId, Guid targetCellId)
    {
        if (Props.GetGroupId is null) return null;

        var currentCell = FindCellByEventId(eventId);
        if (currentCell is null) return null;

        var timetableEvent = currentCell.Events.FirstOrDefault(e => e.Id == eventId);
        if (timetableEvent is null || timetableEvent.GroupIdentifier is null) return null;

        var groupId = timetableEvent.GroupIdentifier;

        var targetCell = FindCellById(targetCellId);
        if (targetCell is null) return null;

        var updatedEvents = new List<TEvent>();

        foreach (var evt in Events)
        {
            var groupIdentifier = Props.GetGroupId(evt);

            if (groupIdentifier is null || !groupIdentifier.Equals(groupId)) continue;

            var originalDateFrom = Props.GetDateFrom(evt);
            var originalDateTo = Props.GetDateTo(evt);
            var duration = originalDateTo - originalDateFrom;

            var newDateFrom = targetCell.DateTime;
            var newDateTo = newDateFrom.Add(duration);

            Props.SetDateFrom(evt, newDateFrom);
            Props.SetDateTo(evt, newDateTo);

            updatedEvents.Add(evt);
        }

        return updatedEvents;
    }

    private Cell<TEvent>? FindCellByEventId(Guid eventId)
    {
        return Grid.Columns.SelectMany(col => col.Cells)
                           .FirstOrDefault(cell => cell.Events.Any(e => e.Id == eventId));
    }

    private Cell<TEvent>? FindCellById(Guid cellId)
    {
        return Grid.Columns.SelectMany(col => col.Cells)
                           .FirstOrDefault(cell => cell.Id == cellId);
    }
}
