namespace TreeEditorControl.DataNodeAttributes
{
    public class ComboBoxPropertyAttribute : NodePropertyAttribute
    {
        public ComboBoxPropertyAttribute(string valueProviderId = null, string propertyName = null) : base(propertyName)
        {
            ValueProviderId = valueProviderId;
        }

        public string ValueProviderId { get; }
    }
}
