﻿using Microsoft.AspNetCore.Components;

namespace Blazor.Timetable.Components.Shared.Forms;

public partial class Dropdown<TEvent, TType> : BaseInput<TEvent, TType>
{
    [Parameter, EditorRequired] public TType[] Options { get; set; } = [];
}