﻿namespace Parser
{
    public interface IStatePanel : IPanel
    {
        string Index { get; set; }
        IComponentIndex Component { get; }
    }
}
