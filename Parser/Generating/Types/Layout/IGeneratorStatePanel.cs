﻿namespace Parser
{
    public interface IGeneratorStatePanel : IGeneratorPanel
    {
        string Index { get; }
        IGeneratorComponentIndex Component { get; }
    }
}
