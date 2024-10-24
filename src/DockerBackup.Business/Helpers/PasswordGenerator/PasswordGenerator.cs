using System.Security;
using System.Security.Cryptography;
using System.Text;
using DockerBackup.Business.Interfaces.Helpers;

namespace DockerBackup.Business.Helpers;

public class PasswordGenerator : IPasswordGenerator
{
    private readonly PasswordGeneratorOptions _options;

    public PasswordGenerator(PasswordGeneratorOptions options)
    {
        _options = options;
    }

    public PasswordGenerator()
    {
        _options = new PasswordGeneratorOptions();
    }

    public string Generate()
    {
        int charsSum = (_options.LettersAmount ?? 0) + (_options.NumbersAmount ?? 0) + (_options.SymbolsAmount ?? 0);
        StringBuilder generatedPassword = new StringBuilder();

        if (charsSum > _options.PasswordLength)
        {
            throw new InvalidOperationException("Requested characters total is higher than the password length");
        }

        if (_options.LettersAmount != null && _options.NumbersAmount != null && _options.SymbolsAmount != null && charsSum != _options.PasswordLength)
        {
            throw new InvalidOperationException("Requested characters total is lower than the password length");
        }

        List<KeyValuePair<PasswordCharacterTypes, int>> requestedCharacterTypes = new List<KeyValuePair<PasswordCharacterTypes, int>>();

        if (_options.IncludeLetters)
        {
            requestedCharacterTypes.Add(new KeyValuePair<PasswordCharacterTypes, int>(PasswordCharacterTypes.Letter, _options.LettersAmount ?? _options.PasswordLength));
        }

        if (_options.IncludeNumbers)
        {
            requestedCharacterTypes.Add(new KeyValuePair<PasswordCharacterTypes, int>(PasswordCharacterTypes.Number, _options.NumbersAmount ?? _options.PasswordLength));
        }

        if (_options.IncludeSymbols)
        {
            requestedCharacterTypes.Add(new KeyValuePair<PasswordCharacterTypes, int>(PasswordCharacterTypes.Symbol, _options.SymbolsAmount ?? _options.PasswordLength));
        }

        for (int i = 0; i < _options.PasswordLength; i++)
        {
            int nextCharacterTypeIndex = RandomNumberGenerator.GetInt32(0, requestedCharacterTypes.Count);
            var nextCharacterType = requestedCharacterTypes.ElementAt(nextCharacterTypeIndex);
            char nextChar;

            switch (nextCharacterType.Key)
            {
                case PasswordCharacterTypes.Letter:
                    nextChar = _options.ValidLetters.ElementAt(RandomNumberGenerator.GetInt32(0, _options.ValidLetters.Length));
                    break;
                case PasswordCharacterTypes.Number:
                    nextChar = _options.ValidNumbers.ElementAt(RandomNumberGenerator.GetInt32(0, _options.ValidNumbers.Length));
                    break;
                case PasswordCharacterTypes.Symbol:
                    nextChar = _options.ValidSymbols.ElementAt(RandomNumberGenerator.GetInt32(0, _options.ValidSymbols.Length));
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid character type");
            }

            generatedPassword.Append(nextChar);
            requestedCharacterTypes[nextCharacterTypeIndex] = new KeyValuePair<PasswordCharacterTypes, int>(nextCharacterType.Key, requestedCharacterTypes[nextCharacterTypeIndex].Value - 1);

            if (requestedCharacterTypes[nextCharacterTypeIndex].Value <= 0)
            {
                requestedCharacterTypes.RemoveAt(nextCharacterTypeIndex);
            }
        }

        return generatedPassword.ToString();
    }
}