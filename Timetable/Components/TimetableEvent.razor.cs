﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics;
using Timetable.Common.Enums;
using Timetable.Services;

namespace Timetable.Components;

public partial class TimetableEvent
{
    [Inject] private ModalService ModalService { get; set; } = default!;

    private const int MousedownThreshold = 500;
    private readonly Stopwatch _clickStopwatch = new();
    private bool _popoverVisible = false;
    private PopupState _state = PopupState.Detail;

    [Parameter] public Guid EventId { get; set; }
    [Parameter] public string Title { get; set; } = default!;
    [Parameter] public int Span { get; set; }
    [Parameter] public bool IsHeaderEvent { get; set; }
    [Parameter] public int DayColumn { get; set; }
    [Parameter] public int StartSlot { get; set; }
    [Parameter] public int Order { get; set; }
    [Parameter] public int NumberOfEvents { get; set; }
    [Parameter] public RenderFragment DetailTemplate { get; set; } = default!;
    [Parameter] public RenderFragment EditTemplate { get; set; } = default!;
    [Parameter] public RenderFragment DeleteTemplate { get; set; } = default!;

    private string EventStyle => IsHeaderEvent
        ? $"grid-column: {DayColumn + 1} / span {Span}; grid-row: 2;"
        : $"grid-column: {DayColumn + 1}; grid-row: {StartSlot} / span {Span};";

    private void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == 0)
        {
            _clickStopwatch.Restart();
        }
    }

    private void OnMouseUp(MouseEventArgs e)
    {
        if (e.Button != 0 || !_clickStopwatch.IsRunning) return;
        _clickStopwatch.Stop();

        if (_clickStopwatch.ElapsedMilliseconds < MousedownThreshold)
        {
            TogglePopover();
        }
    }

    private void TogglePopover()
    {
        ModalService.Show(Title, builder =>
        {
            builder.AddContent(0, DetailTemplate);
        });

        //_state = PopupState.Detail;
        //_popoverVisible = !_popoverVisible;
        //StateHasChanged();
    }

    private void ToggleEdit()
    {
        _state = PopupState.Edit;
    }

    private void ToggleDelete()
    {
        _state = PopupState.Delete;
    }
}