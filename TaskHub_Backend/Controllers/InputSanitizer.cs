namespace taskHub.Controllers
{
    public class InputSanitizer
    {

        public static readonly char[] BlackListChar = { '`','\'','"',';','`','<','>','&','|','(',')','[',']','{','}','/',':','!','?','#' };

        public bool CheckForBlackListChars(string input)
        {
            foreach (char c in input)
            {

                if (Array.IndexOf(BlackListChar, c) != -1)
                {
                    Console.WriteLine($"Blacklisted character found: {c}");
                    // Take appropriate action, e.g., reject the input

                    return true;
                }
            }
            return false;
        }
    }
}
