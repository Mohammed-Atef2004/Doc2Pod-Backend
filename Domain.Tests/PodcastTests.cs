using Domain.Enums;
using Domain.Podcasts;
using Domain.Podcasts.Errors;
using FluentAssertions;

public class PodcastTests
{
    private static Podcast CreatePodcast(
        Guid? userId = null,
        Guid? documentId = null,
        PodcastMode mode = PodcastMode.Full,
        string model = "Vibe Voice",
        PodcastStatus status = PodcastStatus.Pending)
    {
        return new Podcast(
            userId ?? Guid.NewGuid(),
            documentId ?? Guid.NewGuid(),
            model,
            mode,
            topic: null,
            startPage: null,
            endPage: null,
            status);
    }

    [Fact]
    public void Constructor_Should_Set_Properties_Correctly()
    {
        var userId = Guid.NewGuid();
        var documentId = Guid.NewGuid();
        string model = "Vibe Voice";
        var podcast = new Podcast(
            userId, documentId,
            model, PodcastMode.Full,
            topic: null, startPage: null, endPage: null,
            PodcastStatus.Pending);

        podcast.UserId.Should().Be(userId);
        podcast.DocumentId.Should().Be(documentId);
        podcast.Mode.Should().Be(PodcastMode.Full);
        podcast.Status.Should().Be(PodcastStatus.Pending);
        podcast.ScriptPath.Should().BeNull();
        podcast.AudioPath.Should().BeNull();
        podcast.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void SetPaths_Should_Set_Script_And_Audio_Paths()
    {
        var podcast = CreatePodcast();
        var result = podcast.SetPaths("script.txt", "audio.mp3");
        result.IsSuccess.Should().BeTrue();
        podcast.ScriptPath.Should().Be("script.txt");
        podcast.AudioPath.Should().Be("audio.mp3");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void SetPaths_Should_Return_Failure_When_ScriptPath_Is_Invalid(string? scriptPath)
    {
        var podcast = CreatePodcast();
        var result = podcast.SetPaths(scriptPath!, "audio.mp3");
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneratePodcastErrors.GenerationFailed);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void SetPaths_Should_Return_Failure_When_AudioPath_Is_Invalid(string? audioPath)
    {
        var podcast = CreatePodcast();

        var result = podcast.SetPaths("script.txt", audioPath!);
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GeneratePodcastErrors.GenerationFailed);
    }

    [Fact]
    public void UpdateStatus_Should_Change_Status()
    {
        var podcast = CreatePodcast();
        var result = podcast.UpdateStatus(PodcastStatus.Processing);

        result.IsSuccess.Should().BeTrue();
        podcast.Status.Should().Be(PodcastStatus.Processing);
    }

    [Fact]
    public void UpdateStatus_Completed_Should_Clear_ErrorMessage()
    {
        var podcast = CreatePodcast();
        podcast.UpdateStatus(PodcastStatus.Failed, "some error");
        var result = podcast.UpdateStatus(PodcastStatus.Completed);
        result.IsSuccess.Should().BeTrue();
        podcast.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void UpdateStatus_Failed_Should_Set_ErrorMessage()
    {

        var podcast = CreatePodcast();
        var result = podcast.UpdateStatus(PodcastStatus.Failed, "Generation failed");
        result.IsSuccess.Should().BeTrue();
        podcast.Status.Should().Be(PodcastStatus.Failed);
        podcast.ErrorMessage.Should().Be("Generation failed");
    }
}