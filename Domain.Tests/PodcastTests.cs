using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Entities;

public class PodcastTests
{
    [Fact]
    public void Constructor_Should_Set_Properties_Correctly()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var mode = PodcastMode.Full;
        var topic = "AI";
        var startPage = 1;
        var endPage = 10;
        var scriptPath = "script.txt";
        var audioPath = "audio.mp3";

    // Act
    var podcast = new Podcast(
        documentId,
        mode,
        topic,
        startPage,
        endPage,
        scriptPath,
        audioPath
    );

        // Assert
        podcast.Id.Should().NotBe(Guid.Empty);
        podcast.DocumentId.Should().Be(documentId);
        podcast.Mode.Should().Be(mode);
        podcast.Topic.Should().Be(topic);
        podcast.StartPage.Should().Be(startPage);
        podcast.EndPage.Should().Be(endPage);
        podcast.ScriptPath.Should().Be(scriptPath);
        podcast.AudioPath.Should().Be(audioPath);
    }

    [Fact]
    public void Constructor_Should_Allow_Null_Optional_Fields()
    {
        // Arrange
        var documentId = Guid.NewGuid();

        // Act
        var podcast = new Podcast(
            documentId,
            PodcastMode.Full,
            topic: null,
            startPage: null,
            endPage: null,
            scriptPath: "script.txt",
            audioPath: "audio.mp3"
        );

        // Assert
        podcast.Topic.Should().BeNull();
        podcast.StartPage.Should().BeNull();
        podcast.EndPage.Should().BeNull();
    }

    [Fact]
    public void Constructor_Should_Generate_Unique_Id_For_Each_Instance()
    {
        // Arrange
        var documentId = Guid.NewGuid();

        // Act
        var podcast1 = new Podcast(documentId, PodcastMode.Query, null, null, null, "s1", "a1");
        var podcast2 = new Podcast(documentId, PodcastMode.Full, null, null, null, "s2", "a2");

        // Assert
        podcast1.Id.Should().NotBe(podcast2.Id);
    }


}
