namespace TreeEditorControl.DataNodeAttributes
{
    public class ObjectPropertyAttribute : NodePropertyAttribute
    {
        public ObjectPropertyAttribute(string propertyName = null, bool singleObjectList = false) : base(propertyName)
        {
            SingleObjectList = singleObjectList;
        }

        /// <summary>
        /// Forces a object property to be handles as a list with one item
        /// </summary>
        public bool SingleObjectList { get; }
    }
}
