﻿using System.Linq.Expressions;
using Timetable.Common.Utilities;

namespace Timetable.Structure;

internal sealed class TimetableEventProps<TEvent> where TEvent : class
{
    public Func<TEvent, DateTime> GetDateFrom { get; }
    public Func<TEvent, DateTime> GetDateTo{ get; }
    public Func<TEvent, string?> GetTitle{ get; }
    public Func<TEvent, object?>? GetGroupId{ get; }
    public Action<TEvent, DateTime> SetDateFrom{ get; }
    public Action<TEvent, DateTime> SetDateTo{ get; }
    public Action<TEvent, string?> SetTitle{ get; }
    public Action<TEvent, object?>? SetGroupId{ get; }

    public TimetableEventProps(Expression<Func<TEvent, DateTime>> dateFromSelector, 
        Expression<Func<TEvent, DateTime>> dateToSelector, Expression<Func<TEvent, string?>> titleSelector, 
        Expression<Func<TEvent, object?>>? groupIdSelector = null)
    {
        GetTitle = PropertyHelper.CreateGetter(titleSelector);
        SetTitle = PropertyHelper.CreateSetter(titleSelector);
        GetDateFrom = PropertyHelper.CreateGetter(dateFromSelector);
        SetDateFrom = PropertyHelper.CreateSetter(dateFromSelector);
        GetDateTo = PropertyHelper.CreateGetter(dateToSelector);
        SetDateTo = PropertyHelper.CreateSetter(dateToSelector);

        if (groupIdSelector == null) return;
        
        GetGroupId = PropertyHelper.CreateGetter(groupIdSelector);
        SetGroupId = PropertyHelper.CreateSetter(groupIdSelector);
    }
}