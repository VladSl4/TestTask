using Moq;
using ServerApp.Data;
using ServerApp.Services;
using ServerApp.Models;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ServerApp.Enums;
using GrpcDocumentService;
using Status = ServerApp.Models.Status;
using Document = ServerApp.Models.Document;


namespace ServerAppTests;

[TestFixture] 
public class DocumentServiceTests
{
    private AppDbContext _dbContext;
    private DocumentService _documentService;
    Mock<ServerCallContext> serverCallContext = new Mock<ServerCallContext>(); 
    List<Document> documents = new()
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

    List<Status> statuses = new List<Status>
        {
            new() { Id = (int)StatusEnum.Created, Name = StatusEnum.Created },
            new() { Id = (int)StatusEnum.Deleted, Name = StatusEnum.Deleted }
        };

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _documentService = new DocumentService(_dbContext);
        
       
        _dbContext.Documents.AddRange(documents);
        _dbContext.Statuses.AddRange(statuses);
        _dbContext.SaveChanges();
    }

    [SetUp]
    public void SetUp()
    {
        serverCallContext.Reset();
    }
    
    [Test]
    public async Task GetAllDocumentsTest()
    {       
        var request = new Empty();
       
        var response = await _documentService.GetAllDocuments(request, serverCallContext.Object);
        var expectedDocuments = documents;

        Assert.Multiple(() =>
        {
            
            for (int i = 0; i < expectedDocuments.Count; ++i)
            {
                Assert.That(response.Documents[i].Id, Is.EqualTo(expectedDocuments[i].Id));
                Assert.That(response.Documents[i].Description, Is.EqualTo(expectedDocuments[i].Description));
                Assert.That(response.Documents[i].Amount, Is.EqualTo(expectedDocuments[i].Amount));
            }
        });

    }

    [Test]
    public async Task GetDocumentByIdTest()
    {
        GetDocumentByIdRequest request = new()
        {
            Id = 2
        };
        

        var response = await _documentService.GetDocumentById(request, serverCallContext.Object);
        Assert.Multiple(() =>
        {
            Assert.That(response.Document.Id, Is.EqualTo(documents[1].Id));
            Assert.That(response.Document.Description, Is.EqualTo(documents[1].Description));
            Assert.That(response.Document.Amount, Is.EqualTo(documents[1].Amount));
        });
    }

    [Test]
    public async Task CreateDocumentTest()
    {
        CreateDocumentRequest request = new()
        {
            Amount = 3,
            Description = "Create user UnitTest"
        };
        

        var response = await _documentService.CreateDocument(request, serverCallContext.Object);
        
        var newUser = (from d in _dbContext.Documents
                      where d.Id == response.Id
                      select d).DefaultIfEmpty();

        Assert.Multiple(() =>
        {
            Assert.That(response.Id == newUser.First().Id);
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

    //[TearDown]
    //public void TearDown()
    //{
    //    _dbContext.Database.EnsureDeleted();
    //    _dbContext.Dispose();
    //}
}

