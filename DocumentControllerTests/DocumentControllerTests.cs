using GrpcDocumentService;
using Moq;
using ClientApp.Controllers;
using Google.Protobuf.WellKnownTypes;

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
    public async Task GetAllDocuments_ReturnsDocuments()
    {
        _mockGrpcClient.Setup(x => x.GetAllDocuments(new Empty(), null, null, default)).Returns(new GetAllDocumentsResponse()).Verifiable();

        await _controller.GetAllDocuments();

        _mockGrpcClient.Verify(s => s.GetAllDocuments(new Empty(), null, null, default), Times.AtLeastOnce());
    }
}