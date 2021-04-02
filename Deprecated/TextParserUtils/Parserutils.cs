namespace TextParser
{
    public static class ParserUtils
    {
        static public string Trim(string txt)
        {
            return txt.Substring(1, txt.Length - 2);
        }
    }
}
