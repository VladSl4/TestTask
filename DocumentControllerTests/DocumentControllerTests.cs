using GrpcDocumentService;
using Moq;
using ClientApp.Controllers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace DocumentControllerTests;

[TestFixture]
public class DocumentControllerTests
{
    private Mock<DocumentGrpc.DocumentGrpcClient> _mockGrpcClient;
    private DocumentController _controller;

    [SetUp]
    public void SetUp()
    {    
        _mockGrpcClient = new Mock<DocumentGrpc.DocumentGrpcClient>();

        _controller = new DocumentController(_mockGrpcClient.Object);  
    }

    [Test]
    public async Task GetAllDocumentsControllerTest()
    {  
        var response = new GetAllDocumentsResponse();

        _mockGrpcClient
            .Setup(x => x.GetAllDocumentsAsync(new Empty(), null, null, default))
            .Returns
            (
                new AsyncUnaryCall<GetAllDocumentsResponse>
                (
                    Task.FromResult(response),       
                    Task.FromResult(new Metadata()),                     
                    () => Grpc.Core.Status.DefaultSuccess,                         
                    () => new Metadata(),                                
                    () => { }                                            
                )
            )
            .Verifiable();

        await _controller.GetAllDocuments();

        _mockGrpcClient.Verify(x => x.GetAllDocumentsAsync(new Empty(), null, null, default), Times.Once);
    }

    [Test]
    public async Task GetDocumentByIdControllerTest()
    {
        var response = new GetDocumentByIdResponse();

        _mockGrpcClient
            .Setup(x => x.GetDocumentByIdAsync(new GetDocumentByIdRequest(), null, null, default))
            .Returns
            (
                new AsyncUnaryCall<GetDocumentByIdResponse>
                (
                    Task.FromResult(response),
                    Task.FromResult(new Metadata()),
                    () => Grpc.Core.Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { }
                )
            )
            .Verifiable();

        await _controller.GetDocumentById(new int());

        _mockGrpcClient.Verify(x => x.GetDocumentByIdAsync(new GetDocumentByIdRequest(), null, null, default), Times.Once);
    }

    [Test]
    public async Task CreateDocumentControllerTest()
    {
        var response = new CreateDocumentResponse();

        _mockGrpcClient
            .Setup(x => x.CreateDocumentAsync(new CreateDocumentRequest(), null, null, default))
            .Returns
            (
                new AsyncUnaryCall<CreateDocumentResponse>
                (
                 Task.FromResult(response),
                    Task.FromResult(new Metadata()),
                    () => Grpc.Core.Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { }
                )
            )
            .Verifiable();

        await _controller.CreateDocument(new CreateDocumentRequest());
        _mockGrpcClient.Verify(x => x.CreateDocumentAsync(new CreateDocumentRequest(), null, null, default));
    }

    [Test]
    public async Task DeleteDocumentControllerTest()
    {
        _mockGrpcClient
            .Setup(x => x.DeleteDocumentAsync(new DeleteDocumentRequest(), null, null, default))
            .Returns
            (
                new AsyncUnaryCall<Empty>
                (
                 Task.FromResult(new Empty()),
                    Task.FromResult(new Metadata()),
                    () => Grpc.Core.Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { }
                )
            )
            .Verifiable();

        await _controller.DeleteDocument(new int());

        _mockGrpcClient.Verify(x=>x.DeleteDocumentAsync(new DeleteDocumentRequest(), null, null, default));
    }

}