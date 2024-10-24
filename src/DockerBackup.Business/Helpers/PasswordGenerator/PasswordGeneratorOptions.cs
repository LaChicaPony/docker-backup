namespace DockerBackup.Business.Helpers;

public class PasswordGeneratorOptions
{
    public int PasswordLength { get; set; } = 16;
    public bool IncludeLetters { get; set; } = true;
    public bool IncludeNumbers { get; set; } = true;
    public bool IncludeSymbols { get; set; } = true;
    public int? LettersAmount { get; set; } = null;
    public int? NumbersAmount { get; set; } = null;
    public int? SymbolsAmount { get; set; } = null;
    public char[] ValidLetters { get; set; } = new[]
                                                    {
                                                        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
                                                        'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
                                                        'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
                                                        'Y', 'Z',
                                                        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
                                                        'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
                                                        'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
                                                        'y', 'z',
                                                    };
    public char[] ValidNumbers { get; set; } = new[]
                                                    {
                                                        '0', '1', '2', '3', '4',
                                                        '5', '6', '7', '8', '9'
                                                    };
    public char[] ValidSymbols { get; set; } = new[] {  '@', '+', '-', '_', '!',
                                                        '$', '%', '&', '=', '[',
                                                        ']', '{', '}', '^', '/',
                                                        '(', ')', '#'
    };
}