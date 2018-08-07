namespace Parser
{
    public class ComponentEvent : IComponentEvent
    {
        public ComponentEvent(ComponentInfo info)
        {
            EventSource = info.NameSource;
            ObjectSource = info.ObjectSource;
            ObjectEventSource = info.MemberSource;

            if (info.FixedValueSource != null || info.KeySource != null)
                throw new ParsingException(EventSource.Source, "Events must use the <Object>.<Event> syntax");
        }

        public IDeclarationSource EventSource { get; private set; }
        public IDeclarationSource ObjectSource { get; private set; }
        public IDeclarationSource ObjectEventSource { get; private set; }

        public bool Connect(IDomain domain, ref IObject Object, ref IObjectEvent ObjectEvent)
        {
            bool IsConnected = false;

            if (Object == null)
            {
                foreach (IObject Item in domain.Objects)
                    if (Item.Name == ObjectSource.Name)
                    {
                        Object = Item;
                        break;
                    }

                if (Object == null)
                    throw new ParsingException(ObjectSource.Source, $"Unknown object {ObjectSource.Name}");

                IsConnected = true;
            }

            if (ObjectEvent == null)
            {
                foreach (IObjectEvent Event in Object.Events)
                    if (Event.Name == ObjectEventSource.Name)
                    {
                        ObjectEvent = Event;
                        break;
                    }

                if (ObjectEvent == null)
                    throw new ParsingException(ObjectEventSource.Source, $"Unknown event {ObjectEventSource.Name}");

                IsConnected = true;
            }

            return IsConnected;
        }
    }
}
