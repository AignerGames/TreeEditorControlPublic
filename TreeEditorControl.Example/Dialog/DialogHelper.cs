namespace TreeEditorControl.Example.Dialog
{
    internal static class DialogHelper
    {
        public static string GetHeaderString(string header, string headerInfo, int maxChar = 150)
        {
            if(string.IsNullOrWhiteSpace(headerInfo))
            {
                return header;
            }

            var displayInfo = headerInfo.Length < maxChar ? headerInfo : headerInfo.Substring(0, maxChar);

            displayInfo = displayInfo.Replace('\r', ' ').Replace('\n', ' ');

            return $"{header} ({displayInfo})";
        }
    }
}
