﻿namespace School_Timetable.Structure.Entity;

public sealed class GridRow<TEvent> where TEvent : class
{
    public DateTime RowStartTime { get; set; }
    public IList<GridCell<TEvent>> Cells { get; set; } = [];
}
