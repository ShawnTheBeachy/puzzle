using Microsoft.AspNetCore.Components.Forms;

namespace FileParser.BlazorApp.Components.Pages;

public sealed partial class Home : IAsyncDisposable
{
    private IBrowserFile? _file;
    private MemoryStream? _stream;

    private async Task SingleUpload(InputFileChangeEventArgs e)
    {
        Logger.LogInformation("Picked {File}", e.File.Name);

        if (_stream is not null)
            await _stream.DisposeAsync();

        _stream = new MemoryStream();

        try
        {
            await e.File.OpenReadStream().CopyToAsync(_stream);
            _stream.Seek(0, SeekOrigin.Begin);
            Logger.LogInformation("Read {Count} bytes from {File}", _stream.Length, e.File.Name);
            _file = e.File;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_stream is not null)
            await _stream.DisposeAsync();
    }
}
