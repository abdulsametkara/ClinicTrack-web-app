using Xunit;
using ClinickService.Response;

public class ServiceBasicTests
{
    [Fact]
    public void Responses_Property_Accessors_And_Success_Error_Work()
    {
        var res = new Responses(true, "ok");
        Assert.True(res.IsSuccess);
        Assert.Equal("ok", res.Message);

        var succ = Responses.Success("done");
        Assert.True(succ.IsSuccess);
        Assert.Equal("done", succ.Message);

        var err = Responses.Error("fail");
        Assert.False(err.IsSuccess);
        Assert.Equal("fail", err.Message);
    }

    [Fact]
    public void ResponseGeneric_Property_Accessors_And_Success_Error_Work()
    {
        var rg = new ResponseGeneric<int>(42, true, "ok");
        Assert.True(rg.IsSuccess);
        Assert.Equal("ok", rg.Message);
        Assert.Equal(42, rg.Data);

        var succ = ResponseGeneric<string>.Success("merhaba", "msg");
        Assert.True(succ.IsSuccess);
        Assert.Equal("merhaba", succ.Data);
        Assert.Equal("msg", succ.Message);

        var err = ResponseGeneric<object>.Error("hata");
        Assert.False(err.IsSuccess);
        Assert.Null(err.Data);
        Assert.Equal("hata", err.Message);
    }
}

