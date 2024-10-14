using GrpcDocumentService;
using Moq;
using ClientApp.Controllers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace ClientAppTests;

[TestFixture]
public class DocumentControllerTests
{
    private Mock<DocumentGrpc.DocumentGrpcClient> _mockGrpcClient;
    private DocumentController _controller;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _mockGrpcClient = new Mock<DocumentGrpc.DocumentGrpcClient>();
        _controller = new DocumentController(_mockGrpcClient.Object);
    }

    [SetUp]
    public void SetUp()
    {
        _mockGrpcClient.Reset();

    }

    [Test]
    public async Task GetAllDocumentsControllerTest()
    {
        var response = new GetAllDocumentsResponse();

        _mockGrpcClient
            .Setup(x => x.GetAllDocumentsAsync(new Empty(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns
            (
                new AsyncUnaryCall<GetAllDocumentsResponse>
                (
                    Task.FromResult(response),
                    Task.FromResult(It.IsAny<Metadata>()),
                    () => default,
                    () => default!,
                    () => { }
                )
            )
            .Verifiable();

        await _controller.GetAllDocuments();

        _mockGrpcClient.Verify(x => x.GetAllDocumentsAsync(new Empty(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetDocumentByIdControllerTest()
    {
        int request = 2;
        var response = new GetDocumentByIdResponse();


        _mockGrpcClient
            .Setup(x => x.GetDocumentByIdAsync(It.Is<GetDocumentByIdRequest>(r => r.Id == request), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns
            (
                new AsyncUnaryCall<GetDocumentByIdResponse>
                (
                    Task.FromResult(response),
                    Task.FromResult(new Metadata()),
                    () => default,
                    () => default,
                    () => { }
                )
            )
            .Verifiable();


        var actual = await _controller.GetDocumentById(request);

        _mockGrpcClient.Verify(x => x.GetDocumentByIdAsync(It.Is<GetDocumentByIdRequest>(r => r.Id == request), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()), Times.Once);
    }

        [Test]
    public async Task GetDocumentByIdExceptionTest()
    {
        var request = 1;

        _mockGrpcClient
            .Setup(x => x.GetDocumentByIdAsync(It.Is<GetDocumentByIdRequest>(r => r.Id == request), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns(
                new AsyncUnaryCall<GetDocumentByIdResponse>(
                    Task.FromException<GetDocumentByIdResponse>(new Exception($"Document with ID {request} is deleted")),
                    Task.FromResult(new Metadata()),
                    () => default,
                    () => default,
                    () => { }
                )
            );

        var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.GetDocumentById(request));

        Assert.That(ex.Message, Is.EqualTo($"Document with ID {request} is deleted"));
    }

    [Test]
    public async Task CreateDocumentControllerTest()
    {
        var request = new CreateDocumentRequest() { Amount = 12, Description = "test"};

        var response = new CreateDocumentResponse();

        _mockGrpcClient
            .Setup(x => x.CreateDocumentAsync( It.IsAny<CreateDocumentRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns
            (
                new AsyncUnaryCall<CreateDocumentResponse>
                (
                 Task.FromResult(response),
                    Task.FromResult(new Metadata()),
                    () => default,
                    () => default,
                    () => { }
                )
            )
            .Verifiable();

        await _controller.CreateDocument(request);
        _mockGrpcClient.Verify(x => x.CreateDocumentAsync(It.IsAny<CreateDocumentRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()));
    }


    [Test]
    public async Task CreateDocumentExceptionTest()
    {
        var request = new CreateDocumentRequest
        {
            Amount = 1001,
            Description = "The quick brown fox jumps over the lazy dog while singing a tune, and the world watches in awe as magic unfolds under the stars."
        };

        _mockGrpcClient
       .Setup(x => x.CreateDocumentAsync(It.IsAny<CreateDocumentRequest>(), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
       .Returns(
           new AsyncUnaryCall<CreateDocumentResponse>(
               Task.FromException<CreateDocumentResponse>(new Exception("Maximum description length is 128 symbols")),
               Task.FromResult(new Metadata()),
               () => default,
               () => default,
               () => { }
           )
       );

        var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.CreateDocument(request));

        Assert.That(ex.Message, Is.EqualTo("Maximum description length is 128 symbols"));
    }


    [Test]
    public async Task DeleteDocumentControllerTest()
    {
        int request = 2;

        _mockGrpcClient
            .Setup(x => x.DeleteDocumentAsync(It.Is<DeleteDocumentRequest>(r => r.Id == request), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns
            (
                new AsyncUnaryCall<Empty>
                (
                 Task.FromResult(new Empty()),
                    Task.FromResult(new Metadata()),
                    () => default,
                    () => default,
                    () => { }
                )
            )
            .Verifiable();

        await _controller.DeleteDocument(request);

        _mockGrpcClient.Verify(x => x.DeleteDocumentAsync(It.Is<DeleteDocumentRequest>(r => r.Id == request), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task DeleteDocumentExceptionTest()
    {
        var request = 1;

        _mockGrpcClient
            .Setup(x => x.DeleteDocumentAsync(It.Is<DeleteDocumentRequest>(r => r.Id == request), It.IsAny<Metadata>(), It.IsAny<DateTime?>(), It.IsAny<CancellationToken>()))
            .Returns(
                new AsyncUnaryCall<Empty>(
                    Task.FromException<Empty>(new Exception($"Document with ID {request} is already deleted")),
                    Task.FromResult(new Metadata()),
                    () => default,
                    () => default,
                    () => { }
                )
            );

        var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.DeleteDocument(request));

        Assert.That(ex.Message, Is.EqualTo($"Document with ID {request} is already deleted"));
    }


}