using Ethos.Web.Host.Serilog;
using Shouldly;
using Xunit;

namespace Ethos.IntegrationTest.Infrastructure;

public class SerilogTests
{
    [Fact]
    public void RemovePiiFromJsonString_Should_ReplaceSpecifiedFields()
    {
        const string text = @"
            {""userNameOrEmail"":""user@gmail.com"",""password"":""password""}
            {""accessToken"":""eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"",""user"":{}}
        ";

        var result = LogHelper.RemovePiiFromJsonString(text);
        
        const string expected = @"
            {""userNameOrEmail"":""user@gmail.com"",""password"":""*****""}
            {""accessToken"":""*****"",""user"":{}}
        ";
        
        result.ShouldBe(expected);
    }
}