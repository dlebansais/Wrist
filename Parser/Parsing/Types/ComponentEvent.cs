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
                throw new ParsingException(134, EventSource.Source, "Events must use the <Object>.<Event> syntax.");
        }

        public IDeclarationSource EventSource { get; private set; }
        public IDeclarationSource ObjectSource { get; private set; }
        public IDeclarationSource ObjectEventSource { get; private set; }

        public bool Connect(IDomain domain, ref IObject obj, ref IObjectEvent objectEvent)
        {
            bool IsConnected = false;

            if (obj == null)
            {
                foreach (IObject Item in domain.Objects)
                    if (Item.Name == ObjectSource.Name)
                    {
                        obj = Item;
                        break;
                    }

                if (obj == null)
                    throw new ParsingException(135, ObjectSource.Source, $"Unknown object '{ObjectSource.Name}'.");

                obj.SetIsUsed();

                IsConnected = true;
            }

            if (objectEvent == null)
            {
                foreach (IObjectEvent Event in obj.Events)
                    if (Event.NameSource.Name == ObjectEventSource.Name)
                    {
                        objectEvent = Event;
                        break;
                    }

                if (objectEvent == null)
                    throw new ParsingException(136, ObjectEventSource.Source, $"Unknown event '{ObjectEventSource.Name}'.");

                objectEvent.SetIsUsed();

                IsConnected = true;
            }

            return IsConnected;
        }
    }
}
