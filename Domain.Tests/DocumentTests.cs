using Domain.Documents;
using Domain.Enums;
using FluentAssertions;

namespace Domain.Tests.Entities;

public class DocumentTests
{
    private static Document CreateDocument(
        Guid? userId = null,
        string fileName = "file.pdf",
        string filePath = "/files/file.pdf",
        string fileHash = "abc123hash",
        int pageCount = 10)
    {
        return new Document(
            userId ?? Guid.NewGuid(),
            fileName,
            filePath,
            fileHash,
            pageCount);
    }

    [Fact]
    public void Constructor_Should_Set_Properties_Correctly()
    {
        var userId = Guid.NewGuid();
        var fileName = "file.pdf";
        var filePath = "/files/file.pdf";
        var fileHash = "abc123hash";
        var pageCount = 10;

        var document = new Document(userId, fileName, filePath, fileHash, pageCount);
        document.UserId.Should().Be(userId);
        document.FileName.Should().Be(fileName);
        document.FilePath.Should().Be(filePath);
        document.FileHash.Should().Be(fileHash);
        document.PageCount.Should().Be(pageCount);
        document.UploadedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        document.Podcasts.Should().BeEmpty();
    }
    [Fact]
    public void AddPodcast_Should_Add_New_Podcast()
    {
        var userId = Guid.NewGuid();
        var document = CreateDocument(userId);
        string model = "Vibe Voice";
        var result = document.AddPodcast(
            userId,
            model,
            PodcastMode.Full,
            topic: null,
            startPage: null,
            endPage: null,
            PodcastStatus.Pending);
        result.IsSuccess.Should().BeTrue();
        document.Podcasts.Should().HaveCount(1);
        document.Podcasts.Should().Contain(result.Value);
    }

    [Fact]
    public void AddPodcast_Should_Return_Podcast_With_Correct_Properties()
    {
        var userId = Guid.NewGuid();
        var document = CreateDocument(userId);
        string model = "Vibe Voice";
        var result = document.AddPodcast(
            userId,
            model,
            PodcastMode.Query,
            topic: "AI",
            startPage: 1,
            endPage: 5,
            PodcastStatus.Pending);
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(userId);
        result.Value.DocumentId.Should().Be(document.Id);
        result.Value.Mode.Should().Be(PodcastMode.Query);
        result.Value.Topic.Should().Be("AI");
        result.Value.StartPage.Should().Be(1);
        result.Value.EndPage.Should().Be(5);
        result.Value.Status.Should().Be(PodcastStatus.Pending);
    }

    [Fact]
    public void AddPodcast_Should_Allow_Different_Modes()
    {
        var userId = Guid.NewGuid();
        string model = "Vibe Voice";
        var document = CreateDocument(userId);
        var result1 = document.AddPodcast(userId, model, PodcastMode.Full, null, null, null, PodcastStatus.Pending);
        var result2 = document.AddPodcast(userId, model, PodcastMode.Query, "AI", 1, 5, PodcastStatus.Pending);

        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();
        document.Podcasts.Should().HaveCount(2);
    }
}