namespace TreeEditorControl.DataNodeAttributes
{
    public class TextBoxPropertyAttribute : NodePropertyAttribute
    {
        public TextBoxPropertyAttribute(bool multiline = false, string propertyName = null) : base(propertyName)
        {
            Multiline = multiline;
        }

        public bool Multiline { get; }
    }
}
