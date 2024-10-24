using DockerBackup.Business.Helpers;
using FluentAssertions;

namespace DockerBackup.Business.Tests.Helpers;

public class PasswordGeneratorTests : IDisposable
{
    public PasswordGeneratorTests()
    {
        
    }

    public void Dispose()
    {
        
    }

    [Fact]
    public void DefaultOptionsShouldCreateAPassword()
    {
        var options = new PasswordGeneratorOptions();
        var sut = new PasswordGenerator();

        string result = sut.Generate();

        result.Should().NotBeEmpty();
        result.Length.Should().Be(options.PasswordLength);
    }

    [Fact]
    public void OptionsWithLongerTotalCharactersThanPasswordLengthShouldThrowAnException()
    {
        var options = new PasswordGeneratorOptions(){
            LettersAmount = 10,
            NumbersAmount = 4,
            SymbolsAmount = 3
        };
        var sut = new PasswordGenerator(options);

        Assert.Throws<InvalidOperationException>(() =>
        {
            string result = sut.Generate();
        });
    }

    [Fact]
    public void OptionsWithShorterTotalCharactersThanPasswordLengthShouldThrowAnException()
    {
        var options = new PasswordGeneratorOptions()
        {
            LettersAmount = 8,
            NumbersAmount = 4,
            SymbolsAmount = 3
        };
        var sut = new PasswordGenerator(options);

        Assert.Throws<InvalidOperationException>(() =>
        {
            string result = sut.Generate();
        });
    }

    [Fact]
    public void LettersOnlyPasswordShouldntIncludeNumbersOrSymbols()
    {
        var options = new PasswordGeneratorOptions()
        {
            IncludeLetters = true,
            IncludeNumbers = false,
            IncludeSymbols = false
        };
        var sut = new PasswordGenerator(options);

        string result = sut.Generate();

        result.Should().NotBeEmpty();
        result.Length.Should().Be(options.PasswordLength);

        result.Should().NotContainAny(options.ValidNumbers.Select(n => n.ToString()));
        result.Should().NotContainAny(options.ValidSymbols.Select(n => n.ToString()));
    }

    [Fact]
    public void NumbersOnlyPasswordShouldntIncludeLettersOrSymbols()
    {
        var options = new PasswordGeneratorOptions()
        {
            IncludeLetters = false,
            IncludeNumbers = true,
            IncludeSymbols = false
        };
        var sut = new PasswordGenerator(options);

        string result = sut.Generate();

        result.Should().NotBeEmpty();
        result.Length.Should().Be(options.PasswordLength);

        result.Should().NotContainAny(options.ValidLetters.Select(n => n.ToString()));
        result.Should().NotContainAny(options.ValidSymbols.Select(n => n.ToString()));
    }

    [Fact]
    public void SymbolsOnlyPasswordShouldntIncludeLettersOrNumbers()
    {
        var options = new PasswordGeneratorOptions()
        {
            IncludeLetters = false,
            IncludeNumbers = false,
            IncludeSymbols = true
        };
        var sut = new PasswordGenerator(options);

        string result = sut.Generate();

        result.Should().NotBeEmpty();
        result.Length.Should().Be(options.PasswordLength);

        result.Should().NotContainAny(options.ValidLetters.Select(n => n.ToString()));
        result.Should().NotContainAny(options.ValidNumbers.Select(n => n.ToString()));
    }

    [Fact]
    public void PasswordShouldContainTheSpecifiedAmountOfLettersNumbersAndSymbols()
    {
        var options = new PasswordGeneratorOptions()
        {
            LettersAmount = 10,
            NumbersAmount = 4,
            SymbolsAmount = 2
        };
        var sut = new PasswordGenerator(options);

        string result = sut.Generate();

        result.Count(x => options.ValidLetters.Contains(x)).Should().Be(options.LettersAmount);
        result.Count(x => options.ValidNumbers.Contains(x)).Should().Be(options.NumbersAmount);
        result.Count(x => options.ValidSymbols.Contains(x)).Should().Be(options.SymbolsAmount);
    }
}