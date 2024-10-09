using Moq;
using ServerApp.Data;
using ServerApp.Services;
using ServerApp.Models;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ServerApp.Enums;
using GrpcDocumentService;

namespace DocumentServiceTests;

[TestFixture] 
public class DocumentServiceTests
{
    private AppDbContext _dbContext;
    private DocumentService _documentService;

    [SetUp]
    public void SetUp()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _documentService = new DocumentService(_dbContext);

        var documents = new List<ServerApp.Models.Document>
        {
            new() { Id = 1, Amount = 100, Description = "Test Document 1", DocumentStatuses =
                [
                    new DocumentStatus
                    {
                        StatusId = (int)StatusEnum.Created,
                        CreatedAt = DateTime.UtcNow
                    }
                ]
            },

            new() { Id = 2, Amount = 200, Description = "Test Document 2", DocumentStatuses =
                [
                    new DocumentStatus
                    {
                        StatusId = (int)StatusEnum.Created,
                        CreatedAt = DateTime.UtcNow
                    } 
                ]  
            }
        };

        var statuses = new List<ServerApp.Models.Status>
        {
            new() { Id = 1, Name = StatusEnum.Created },
            new() { Id = 2, Name = StatusEnum.Deleted }
        };

        _dbContext.Documents.AddRange(documents);
        _dbContext.Statuses.AddRange(statuses);
        _dbContext.SaveChanges();
    }

    [Test]
    public async Task GetAllDocumentsTest()
    {       
        var request = new Empty();
        var serverCallContext = new Mock<ServerCallContext>();
        
        var response = await _documentService.GetAllDocuments(request, serverCallContext.Object);
        Assert.Multiple(() =>
        {
            Assert.That(response.Documents, Has.Count.EqualTo(2));
            Assert.That(response.Documents.First().Description, Is.EqualTo("Test Document 1"));
        });
    }

    [Test]
    public async Task GetDocumentByIdTest()
    {
        GetDocumentByIdRequest request = new()
        {
            Id = 2
        };
        var serverCallContext = new Mock<ServerCallContext>();

        var response = await _documentService.GetDocumentById(request, serverCallContext.Object);
        Assert.That(response.Document.Description, Is.EqualTo("Test Document 2"));
    }

    [Test]
    public async Task CreateDocumentTest()
    {
        CreateDocumentRequest request = new()
        {
            Amount = 3,
            Description = "Create user UnitTest"
        };
        var serverCallContext = new Mock<ServerCallContext>();

        var response = await _documentService.CreateDocument(request, serverCallContext.Object);
        
        var newUser = (from d in _dbContext.Documents
                      where d.Description == request.Description
                      select d).DefaultIfEmpty();

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusId, Is.EqualTo((int)StatusEnum.Created));
            Assert.That(newUser, Is.Not.Null);
        });
    }

    //[Test]
    //public async Task DeleteDocumentTest()
    //{
    //    DeleteDocumentRequest request = new() { Id = 1 };
    //    var serverCallContext = new Mock<ServerCallContext>();

    //    await _documentService.DeleteDocument(request, serverCallContext.Object);

    //    var deletedDocument = (from d in _dbContext.DocumentStatuses
    //                          where d.DocumentId == request.Id && d.StatusId == (int)StatusEnum.Deleted
    //                          select d).DefaultIfEmpty();
    //    Assert.That(deletedDocument, Is.Not.Null);

    //}

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}