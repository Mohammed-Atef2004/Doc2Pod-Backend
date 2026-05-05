using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Entities;

public class DocumentTests
{
    [Fact]
    public void Constructor_Should_Set_Properties_Correctly()
    {
        // Arrange
        var fileName = "file.pdf";
        var filePath = "/files/file.pdf";

    // Act
    var document = new Document(fileName, filePath);

        // Assert
        document.FileName.Should().Be(fileName);
        document.FilePath.Should().Be(filePath);
        document.UploadedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        document.Podcasts.Should().BeEmpty();
    }

    [Fact]
    public void AddPodcast_Should_Add_New_Podcast()
    {
        // Arrange
        var document = new Document("file.pdf", "/files/file.pdf");

        // Act
        var podcast = document.AddPodcast(
            PodcastMode.Full,
            topic: null,
            startPage: null,
            endPage: null,
            scriptPath: "script.txt",
            audioPath: "audio.mp3"
        );

        // Assert
        document.Podcasts.Should().HaveCount(1);
        document.Podcasts.Should().Contain(podcast);
    }

    [Fact]
    public void AddPodcast_Should_Throw_When_Same_Mode_Already_Exists()
    {
        // Arrange
        var document = new Document("file.pdf", "/files/file.pdf");

        document.AddPodcast(
            PodcastMode.Full,
            null,
            null,
            null,
            "script.txt",
            "audio.mp3"
        );

        // Act
        Action act = () => document.AddPodcast(
            PodcastMode.Full,
            null,
            null,
            null,
            "script2.txt",
            "audio2.mp3"
        );

        // Assert
        act.Should().Throw<Exception>()
           .WithMessage("Podcast already exists for this mode");
    }

    [Fact]
    public void AddPodcast_Should_Allow_Different_Modes()
    {
        // Arrange
        var document = new Document("file.pdf", "/files/file.pdf");

        // Act
        document.AddPodcast(PodcastMode.Query, null, null, null, "s1", "a1");
        document.AddPodcast(PodcastMode.Full, null, null, null, "s2", "a2");

        // Assert
        document.Podcasts.Should().HaveCount(2);
    }

}
